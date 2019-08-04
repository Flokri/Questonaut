using System;
using System.Collections.Generic;
using System.Linq;
using Questonaut.DependencyServices;
using Questonaut.Model;
using SQLite.Net;
using Xamarin.Forms;

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
        #endregion
    }
}
