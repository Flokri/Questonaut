using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.CloudFirestore;
using Questonaut.Controller;
using Questonaut.Model;
using Shiny.Jobs;
using Xamarin.Forms;

namespace Questonaut.Helper
{
    public class UploadActivitiesJob : Shiny.Jobs.IJob
    {
        #region instances
        /// <summary>
        /// The core services containg all the important services like notification and communication.
        /// </summary>
        private readonly CoreDelegateServices dependency;
        #endregion

        #region constructor
        public UploadActivitiesJob()
        {
            this.dependency = dependency;
        }
        #endregion

        #region interface implementation
        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            try
            {
                var db = new ActivityDB();
                List<QActivity> data = db.GetReadyForUpload();

                Analytics.TrackEvent("Upload all the activities");
                Analytics.TrackEvent("The user obect.", new Dictionary<string, string>
                {
                    {"User Id", CurrentUser.Instance.User.Id ?? "null"},
                    {"Email", CurrentUser.Instance.User.Email ?? "null"},
                });

                var contextDoc = await CrossCloudFirestore.Current
                    .Instance
                    .GetCollection(QUser.CollectionPath)
                    .GetDocument(CurrentUser.Instance.User.Id)
                    .GetDocumentAsync();
                QUser user = contextDoc.ToObject<QUser>();

                foreach (QActivity act in data)
                {
                    user.Studies[act.StudyId].Add(act.Answer);
                    act.Uploaded = true;
                }

                if (data == null || data.Count == 0 || data.Where(x => x.Uploaded).Count() == 0)
                    return false;

                var result = await CurrentUser.Instance.UpdateUser("Studies", user.Studies);

                if (result)
                {
                    db.SetUploadStatus(data);
                }

                return true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }
        #endregion
    }
}
