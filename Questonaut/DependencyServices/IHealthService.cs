using System;
namespace Questonaut.DependencyServices
{
    public interface IHealthService
    {
        void FetchSteps(Action<double> completionHandler);
        void FetchMetersWalked(Action<double> completionHandler);
        void FetchActiveMinutes(Action<double> completionHandler);
        void GetHealthPermissionAsync(Action<bool> completion);
        void FetchActiveEnergyBurned(Action<double> completionHandler);
    }
}
