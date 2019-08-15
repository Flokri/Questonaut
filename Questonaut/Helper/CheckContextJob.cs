using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Microsoft.AppCenter.Analytics;
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

            Analytics.TrackEvent("Started to check context.");
            Analytics.TrackEvent("The user obect.", new Dictionary<string, string>
            {
                {"User Id", CurrentUser.Instance.User.Id ?? "null"},
                {"Email", CurrentUser.Instance.User.Email ?? "null"},
            });

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

                        if (CheckContextForElement(context, jobInfo, element, db) && db.GetElementCountForToday(element.ID) < element.RepeatPerDay)
                        {
                            db.AddActivity(
                                new QActivity
                                {
                                    StudyId = study.Id,
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
                    await this.dependency.SendNotification("Questonaut", "You're in the right context to answer multiple questions. Go ahead 🚀");
                }
                else
                {
                    await this.dependency.SendNotification("Questonaut", "You're in the right context to answer a question. Go ahead 🚀");
                }
            }

            return true; // this is for iOS - try not to lie about this - return true when you actually do receive new data from the remote method
        }

        private bool CheckContextForElement(QContext context, JobInfo info, QElement element, ActivityDB db)
        {
            bool isRightContext = false;

            bool temp = false;

            try
            {
                if (context.LocationName != null &&
                    context.LocationAction != null)
                {
                    if (context.LocationFallBack)
                    {
                        //only for 24h time format
                        if (db.GetElementCountForToday(element.ID) == 0 && DateTime.Now.Hour > 18 && DateTime.Now.Hour < 19)
                        {
                            temp = true;
                        }
                        else
                        {
                            temp = false;
                        }
                    }
                    else
                    {
                        temp = false;
                    }
                }

                else if (DateTime.Now.TimeOfDay > context.StartTime.TimeOfDay && DateTime.Now.TimeOfDay < context.EndTime.TimeOfDay)
                {
                    temp = true;
                }

                isRightContext = temp;
            }
            catch (Exception e)
            {
                temp = false;
            }
            catch
            {
                temp = false;
            }

            return isRightContext;
        }
    }
}
