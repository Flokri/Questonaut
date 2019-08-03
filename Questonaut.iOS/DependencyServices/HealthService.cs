﻿using System;
using Xamarin.Forms;
using Foundation;
using HealthKit;
using System.Threading.Tasks;
using Questonaut.iOS.DependencyServices;
using Questonaut.DependencyServices;

[assembly: Dependency(typeof(HealthService))]
namespace Questonaut.iOS.DependencyServices
{
    public class HealthService : IHealthService
    {
        NSNumberFormatter numberFormatter;
        public HKHealthStore HealthStore { get; set; }

        NSSet DataTypesToWrite
        {
            get
            {
                return NSSet.MakeNSObjectSet<HKObjectType>(new HKObjectType[] {

                });
            }
        }

        NSSet DataTypesToRead
        {
            get
            {
                return NSSet.MakeNSObjectSet<HKObjectType>(new HKObjectType[] {
                    HKQuantityType.Create(HKQuantityTypeIdentifier.Height),
                    HKCharacteristicType.Create(HKCharacteristicTypeIdentifier.DateOfBirth),
                    HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
                    HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning),
                    HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime),
                    HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned),
                    HKQuantityType.Create(HKQuantityTypeIdentifier.HeartRate)
                });
            }
        }

        public void GetHealthPermissionAsync(Action<bool> completion)
        {
            if (HKHealthStore.IsHealthDataAvailable)
            {
                HealthStore = new HKHealthStore();
                HealthStore.RequestAuthorizationToShare(DataTypesToWrite, DataTypesToRead, (bool authorized, NSError error) =>
                {
                    completion(true);
                });
            }
            else
            {
                completion(false);
            }
        }

        public void FetchSteps(Action<double> completionHandler)
        {
            var calendar = NSCalendar.CurrentCalendar;
            var startDate = DateTime.Today;
            var endDate = DateTime.Now;
            var stepsQuantityType = HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount);

            var predicate = HKQuery.GetPredicateForSamples((NSDate)startDate, (NSDate)endDate, HKQueryOptions.StrictStartDate);

            var query = new HKStatisticsQuery(stepsQuantityType, predicate, HKStatisticsOptions.CumulativeSum,
                            (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) =>
                            {
                                if (error != null && completionHandler != null)
                                    completionHandler(0.0f);

                                var totalSteps = results.SumQuantity();
                                if (totalSteps == null)
                                    totalSteps = HKQuantity.FromQuantity(HKUnit.Count, 0.0);

                                completionHandler(totalSteps.GetDoubleValue(HKUnit.Count));
                            });
            HealthStore.ExecuteQuery(query);
        }

        public void FetchMetersWalked(Action<double> completionHandler)
        {
            var calendar = NSCalendar.CurrentCalendar;
            var startDate = DateTime.Today;
            var endDate = DateTime.Now;
            var stepsQuantityType = HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning);

            var predicate = HKQuery.GetPredicateForSamples((NSDate)startDate, (NSDate)endDate, HKQueryOptions.StrictStartDate);

            var query = new HKStatisticsQuery(stepsQuantityType, predicate, HKStatisticsOptions.CumulativeSum,
                            (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) =>
                            {
                                if (error != null && completionHandler != null)
                                    completionHandler(0);

                                var distance = results.SumQuantity();
                                if (distance == null)
                                    distance = HKQuantity.FromQuantity(HKUnit.Meter, 0);

                                completionHandler(distance.GetDoubleValue(HKUnit.Meter));
                            });
            HealthStore.ExecuteQuery(query);
        }

        public void FetchActiveMinutes(Action<double> completionHandler)
        {
            var calendar = NSCalendar.CurrentCalendar;
            var startDate = DateTime.Today;
            var endDate = DateTime.Now;
            var stepsQuantityType = HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime);

            var predicate = HKQuery.GetPredicateForSamples((NSDate)startDate, (NSDate)endDate, HKQueryOptions.StrictStartDate);

            var query = new HKStatisticsQuery(stepsQuantityType, predicate, HKStatisticsOptions.CumulativeSum,
                            (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) =>
                            {
                                if (error != null && completionHandler != null)
                                    completionHandler(0);

                                var totalMinutes = results.SumQuantity();
                                if (totalMinutes == null)
                                    totalMinutes = HKQuantity.FromQuantity(HKUnit.Minute, 0);

                                completionHandler(totalMinutes.GetDoubleValue(HKUnit.Minute));
                            });
            HealthStore.ExecuteQuery(query);
        }


        public void FetchActiveEnergyBurned(Action<double> completionHandler)
        {
            var calendar = NSCalendar.CurrentCalendar;
            var startDate = DateTime.Today;
            var endDate = DateTime.Now;
            var stepsQuantityType = HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned);

            var predicate = HKQuery.GetPredicateForSamples((NSDate)startDate, (NSDate)endDate, HKQueryOptions.StrictStartDate);

            var query = new HKStatisticsQuery(stepsQuantityType, predicate, HKStatisticsOptions.CumulativeSum,
                            (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) =>
                            {
                                if (error != null && completionHandler != null)
                                    completionHandler(0);

                                var energyBurned = results.SumQuantity();
                                if (energyBurned == null)
                                    energyBurned = HKQuantity.FromQuantity(HKUnit.Calorie, 0);

                                completionHandler(energyBurned.GetDoubleValue(HKUnit.Calorie));
                            });
            HealthStore.ExecuteQuery(query);
        }
    }
}
