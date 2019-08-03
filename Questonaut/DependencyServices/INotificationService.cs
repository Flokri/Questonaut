using System;
namespace Questonaut.DependencyServices
{
    public interface INotificationService
    {
        void SendText(string title, string description);
        void Init();
    }
}
