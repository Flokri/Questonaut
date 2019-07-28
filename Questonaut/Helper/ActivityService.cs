using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Questonaut.Model;

namespace Questonaut.Helper
{
    public class ActivityService
    {
        #region instances
        private ObservableCollection<QActivity> _activities;
        #endregion

        #region constructor
        public ActivityService() { Activities = new ObservableCollection<QActivity>(); }
        #endregion 

        #region properties
        public ObservableCollection<QActivity> Activities
        {
            get => _activities;
            set => _activities = value;
        }
        #endregion

        #region public methods
        public IList<QActivity> GetActivitiesAsync(int pageIndex, int pageSize)
        {
            //sync the list with the saves activities
            //this code is just for testing
            Activities = new ObservableCollection<QActivity> {
                new QActivity(){ ID = 1, Name =  "Recoreded steps", Date = DateTime.Now, Description="Recorded steps for a custome questionnair."},
                new QActivity(){ID = 2, Name = "Recoreded location", Date = DateTime.Now.AddDays(-1), Description="Recorded location for a custome questionnair."},
                new QActivity(){ ID = 3, Name =  "Recoreded steps", Date = DateTime.Now, Description="Recorded steps for a custome questionnair."},
                new QActivity(){ID = 4, Name = "Recoreded location", Date = DateTime.Now.AddDays(-4), Description="Recorded location for a custome questionnair."},
                new QActivity(){ID = 5, Name = "Recoreded location", Date = DateTime.Now.AddDays(-4), Description="Recorded location for a custome questionnair."},
            };

            return Activities.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        #endregion
    }
}
