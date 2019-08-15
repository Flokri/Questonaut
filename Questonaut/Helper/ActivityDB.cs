﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter.Crashes;
using Questonaut.DependencyServices;
using Questonaut.Model;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Questonaut.Helper
{
    public class ActivityDB
    {
        #region instances
        private SQLiteConnection _sqLiteConnection;
        #endregion

        #region constructor
        public ActivityDB()
        {
            _sqLiteConnection = DependencyService.Get<ISQLiteService>().GetSQLiteConnection();
            _sqLiteConnection.CreateTable<QActivity>();
        }
        #endregion

        #region public methods
        public IEnumerable<QActivity> GetActivities()
        {
            return (from u in _sqLiteConnection.Table<QActivity>()
                    select u).ToList();
        }

        public QActivity GetSpecificActivity(int id)
        {
            return _sqLiteConnection.Table<QActivity>().FirstOrDefault(t => t.ID == id);
        }

        public void DeleteActivity(int id)
        {
            _sqLiteConnection.Delete<QActivity>(id);
        }

        public bool DeleteAllActivities()
        {
            try
            {
                foreach (QActivity act in GetActivities())
                {
                    DeleteActivity(act.ID);
                }

                MessagingCenter.Send<string>("Questonaut", "refreshDB");

                return true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }

        public string AddActivity(QActivity activity)
        {
            var data = _sqLiteConnection.Table<QActivity>();
            var d1 = data.Where(x => x.Name == activity.Name && x.ID == activity.ID).FirstOrDefault();
            if (d1 == null)
            {
                _sqLiteConnection.Insert(activity);
                return "Sucessfully Added";
            }
            else
                return "Already Existing";
        }

        public bool CheckStatus(int id)
        {
            var data = _sqLiteConnection.Table<QActivity>();
            var fetched = data.Where(x => x.ID == id).FirstOrDefault();
            if (fetched.Status.Equals("open"))
            {
                return true;
            }
            else if (fetched.Status.Equals("closed"))
            {
                return false;
            }

            return false;
        }

        public bool ChangeStatus(int id)
        {
            var data = _sqLiteConnection.Table<QActivity>();
            var d1 = data.Where(x => x.ID == id).FirstOrDefault();
            if (d1 != null)
            {
                d1.Status = "closed";
                if (d1 != null)
                {
                    _sqLiteConnection.Update(d1);
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        public bool SetActivityAsAnswered(string answer, int id)
        {
            var data = _sqLiteConnection.Table<QActivity>();
            var d1 = data.Where(x => x.ID == id).FirstOrDefault();
            if (d1 != null)
            {
                d1.Status = "closed";
                d1.Answer = answer;
                if (d1 != null)
                {
                    _sqLiteConnection.Update(d1);
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        public int GetElementCountForToday(string elementId)
        {
            var data = _sqLiteConnection.Table<QActivity>();
            try
            {
                if (data.Count() == 0)
                {
                    return 0;
                }
                int count = data.ToList().Where(x => x.ElementId.Equals(elementId)
                                                && CompareDateTimes(x.Date, DateTime.Today))
                                         .Count();
                return count;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            return 0;
        }

        /// <summary>
        /// Get all activities that are not uploaded to the server.
        /// </summary>
        /// <returns>Returns a list with all not uploaded activities.</returns>
        public List<QActivity> GetReadyForUpload()
        {
            var data = _sqLiteConnection.Table<QActivity>();
            try
            {
                if (data.Count() == 0)
                {
                    return null;
                }

                return data.ToList().Where(x => !x.Uploaded && x.Answer != null).ToList();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }

            return null;
        }

        /// <summary>
        /// Set the uploaded status of a list of activities.
        /// </summary>
        /// <param name="activities">The activities that should be updated.</param>
        /// <param name="status">The status the activies should be updated to.</param>
        /// <returns></returns>
        public bool SetUploadStatus(List<QActivity> activities)
        {
            try
            {
                foreach (QActivity act in activities)
                {
                    var data = _sqLiteConnection.Table<QActivity>();
                    var d1 = data.Where(x => x.ID == act.ID).FirstOrDefault();
                    if (d1 != null)
                    {
                        d1.Uploaded = act.Uploaded;
                        if (d1 != null)
                        {
                            _sqLiteConnection.Update(d1);
                            return true;
                        }
                        else
                            return false;
                    }
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CompareDateTimes(DateTime d1, DateTime d2)
        {
            if (d1.Date == d2.Date)
                return true;
            else
                return false;
        }
        #endregion
    }
}
