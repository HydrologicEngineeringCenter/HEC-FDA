using HEC.MVVMFramework.Base.Enumerations;

namespace HEC.MVVMFramework.Base.Interfaces
{
    public interface IErrorMessage : IMessage
    {
        ErrorLevel ErrorLevel { get; }

    }
}
