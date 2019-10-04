using System;
using FdaModel.Utilities.Attributes;

namespace FdaModel.Utilities.Messager
{
    [Author("William Lehman", "05/26/2016")]
    /// <summary>
    /// ErrorMessagesEnum is a byte flag for severity, currently there are only 4 errors possible.
    /// Report is used when the error should be reported for the users information, but should not affect the functionality of the model, viewmodel or view.
    /// Minor is used when an error exists that has little or no impact on results.
    /// Major is used when an error exists that would have a significant impact on results. These error message must be cleared before full functionality of the model, viewmodel or view is restored.
    /// Fatal is used when an exception is caught by the programmer.
    /// ViewModel will be used when it is an issue relating to the ViewModel
    /// Model will be used when it is an issue relating to the Model
    /// View will be used when it is an issue relating to the view (probably will be an unused specification)
    /// 
    /// Flags can be combined : ErrorMessagesEnum MyFlag =  ErrorMessagesEnum.ViewModel | ErrorMessagesEnum.Fatal would result in a flag 0001 1000
    /// this can then be compared elsewhere:
    /// if ((MyFlag & ErrorMessagesEnum.ViewModel)>0){ //the error is from the ViewModel}
    /// </summary>
    [Flags]
    public enum ErrorMessageEnum : byte
    {
        UnAssigned = 0x00,  //0000 0000
        Report = 0x01,      //0000 0001
        Minor = 0x02,       //0000 0010
        Major = 0x04,       //0000 0100
        Fatal = 0x08,       //0000 1000
        ViewModel = 0x10,   //0001 0000
        Model = 0x20,       //0010 0000 
        View = 0x40        //0100 0000 
    }
}

