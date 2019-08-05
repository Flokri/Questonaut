using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.CloudFirestore;
using Questonaut.Controller;
using Questonaut.Model;
using Shiny.Jobs;

namespace Questonaut.Helper
{
    public class CheckContextJob : Shiny.Jobs.IJob
    {
        private readonly CoreDelegateServices dependency;
        private readonly ActivityDB activityDB;

        public CheckContextJob(CoreDelegateServices dependency)
        {
            this.dependency = dependency;
            activityDB = new ActivityDB();
        }

        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
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

                        if (CheckContextForElement(context) && activityDB.GetElementCountForToday(element.ID) <= element.RepeatPerDay)
                        {
                            activityDB.AddActivity(
                                new QActivity
                                {
                                    Name = study.Title,
                                    Date = DateTime.Now,
                                    Description = element.Description,
                                    ElementId = element.ID,
                                    Status = "open",
                                    Link = context.ID
                                });

                            await this.dependency.SendNotification(study.Title, "You're in the right context answer a question. Go ahead 🚀");
                        }
                    }
                }
            }

            return true; // this is for iOS - try not to lie about this - return true when you actually do receive new data from the remote method
        }

        private bool CheckContextForElement(QContext context)
        {
            bool isRightContext = false;

            if (DateTime.Now.TimeOfDay > context.StartTime.TimeOfDay && DateTime.Now.TimeOfDay < context.EndTime.TimeOfDay)
            {
                isRightContext = true;
            }

            return isRightContext;
        }
    }
}
