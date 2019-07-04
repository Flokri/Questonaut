using System;
using Microsoft.AppCenter.Crashes;

namespace Questonaut.DependencyServices
{
    /// <summary>
    /// interface to get the current screen dimension
    /// </summary>
    public interface IScreenDimensions
    {
        double GetScreenWidth();
        double GetScreenHeight();
    }
}
