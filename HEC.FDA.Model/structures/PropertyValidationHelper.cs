using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Implementations;

namespace HEC.FDA.Model.structures;

public class PropertyValidationHelper
{
    public bool HasErrors { get; set; }
    public ErrorLevel ErrorLevel { get; set; }
    public void ResetErrors(Validation validationErrorLogger)
    {
        HasErrors = true;
        if (ErrorLevel < validationErrorLogger.ErrorLevel)
        {
            ErrorLevel = validationErrorLogger.ErrorLevel;
        }
    }
    public void ValidateProperty(Validation validationErrorLogger)
    {
        validationErrorLogger.Validate();
        if (validationErrorLogger.HasErrors)
        {
            ResetErrors(validationErrorLogger);
        }
    }
}