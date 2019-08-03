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
                new QActivity(){ ID = 1, Name =  "Question", Date = DateTime.Now, Description="Answerd a question based on a step context.", Status="open"},
                new QActivity(){ID = 2, Name = "Rating", Date = DateTime.Now.AddDays(-1), Description="Rating based on a location context.",Status="open"},
                new QActivity(){ ID = 3, Name =  "Multiple Choice", Date = DateTime.Now.AddDays(-2), Description="Answer a multiple choice question based on a time context",Status="closed"},
                new QActivity(){ID = 4, Name = "Question", Date = DateTime.Now.AddDays(-4), Description="Answerd a question based on a location context.",Status="closed"},
                new QActivity(){ID = 5, Name = "Rating", Date = DateTime.Now.AddDays(-4), Description="Answerd a question based on a location context.",Status="closed"},
            };

            return Activities.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        #endregion
    }
}
