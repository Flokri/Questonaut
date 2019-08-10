using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Plugin.CloudFirestore;
using Questonaut.Controller;
using Questonaut.Model;
using Shiny.Jobs;
using Shiny.Locations;
using Xamarin.Forms;

namespace Questonaut.Helper
{
    public class CheckContextJob : Shiny.Jobs.IJob
    {
        private readonly CoreDelegateServices dependency;

        public CheckContextJob(CoreDelegateServices dependency)
        {
            this.dependency = dependency;
        }

        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            var db = new ActivityDB();
            int activityCount = 0;

            foreach (QStudy study in CurrentUser.Instance.User.ActiveStudiesObjects)
            {
                if (study.Elements != null)
                {
                    foreach (QElement element in study.Elements)
                    {
                        var contextDoc = await CrossCloudFirestore.Current
                            .Instance
                            .GetCollection(QContext.CollectionPath)
                            .GetDocument(element.LinkToContext)
                            .GetDocumentAsync();
                        QContext context = contextDoc.ToObject<QContext>();

                        if (CheckContextForElement(context, jobInfo) && db.GetElementCountForToday(element.ID) < element.RepeatPerDay)
                        {
                            db.AddActivity(
                                new QActivity
                                {
                                    Name = study.Title,
                                    Date = DateTime.Now,
                                    Description = element.Description,
                                    ElementId = element.ID,
                                    Status = "open",
                                    Link = element.LinkToUserElement,
                                });
                            MessagingCenter.Send<string>("Questonaut", "refreshDB");
                            activityCount++;

                        }
                    }
                }
            }

            if (activityCount > 0)
            {
                if (activityCount >= 2)
                {
                    await this.dependency.SendNotification("Questonaut", "You're in the right context to answer a question. Go ahead 🚀");
                }
                else
                {
                    await this.dependency.SendNotification("Questonaut", "You're in the right context to answer multiple questions. Go ahead 🚀");
                }
            }

            return true; // this is for iOS - try not to lie about this - return true when you actually do receive new data from the remote method
        }

        private bool CheckContextForElement(QContext context, JobInfo info)
        {
            bool isRightContext = false;

            try
            {
                if (DateTime.Now.TimeOfDay > context.StartTime.TimeOfDay && DateTime.Now.TimeOfDay < context.EndTime.TimeOfDay)
                {
                    isRightContext = true;
                }

                if (context.LocationName != null &&
                    context.LocationAction != null &&
                    context.Location != null)
                {
                    if (info.Parameters.ContainsKey("region"))
                    {
                        var region = info.Parameters["region"];
                        if (region != null)
                        {
                            var splitted = ((GeofenceRegion)region).Identifier.Split('|');

                            if (splitted.Length == 2 && splitted[0].Equals(context.LocationName))
                            {
                                isRightContext = true;
                            }
                            else
                            {
                                isRightContext = false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                isRightContext = false;
            }

            return isRightContext;
        }
    }
}
