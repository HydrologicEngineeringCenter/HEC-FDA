using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.MVVMFramework.Model.Messaging
{
    public interface IDontImplementValidationButMyPropertiesDo: IReportMessage
    {
        bool HasErrors { get; set; }
        ErrorLevel ErrorLevel { get; set; }
        string GetErrorsFromProperties();
        void Validate();
    }
}
