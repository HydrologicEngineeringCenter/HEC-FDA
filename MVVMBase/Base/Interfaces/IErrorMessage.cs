
namespace Base.Interfaces
{
    public interface IErrorMessage: IMessage
    {
        Enumerations.ErrorLevel ErrorLevel { get; }

    }
}
