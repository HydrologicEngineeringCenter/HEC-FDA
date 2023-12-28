using HEC.MVVMFramework.Base.Enumerations;

namespace HEC.MVVMFramework.Model.Messaging
{
    public interface IDontImplementValidationButMyPropertiesDo
    {
        bool HasErrors { get; set; }
        ErrorLevel ErrorLevel { get; set; }
        string GetErrorsFromProperties();
        void Validate();
    }
}
