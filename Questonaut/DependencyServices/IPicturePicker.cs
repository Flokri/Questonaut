using System;
using System.IO;
using System.Threading.Tasks;

namespace Questonaut.DependencyServices
{
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}
