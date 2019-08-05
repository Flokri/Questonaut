using System;
namespace Questonaut.Validators
{
    public interface IValidator
    {
        string Message { get; set; }
        bool Check(string value);
    }
}
