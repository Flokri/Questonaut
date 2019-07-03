using System;
using System.Threading;

namespace Questonaut.config.configreader
{
    interface ISettingsReader
    {
        T Load<T>() where T : class, new();
        T LoadSettings<T>() where T : class, new();

        object Load(Type type);
        object LoadSelection(Type type);
    }
}
