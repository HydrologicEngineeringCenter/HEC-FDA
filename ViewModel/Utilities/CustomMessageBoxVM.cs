using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.Utilities
{
    //[Author(q0heccdm, 3 / 14 / 2017 9:02:27 AM)]
    public class CustomMessageBoxVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 3/14/2017 9:02:27 AM
        #endregion
        #region Fields

            [Flags]
            public enum ButtonsEnum:byte
        {
            
            Yes = 1,
            No = 2,
            OK = 4,
            Cancel = 8,
            Close = 16,
            Abort = 32,
            Retry = 64,
            Ignore = 128,

            Yes_No = Yes | No,
            OK_Cancel = OK | Cancel,
            OK_Close = OK | Close,
            Abort_Retry_Ignore = Abort | Retry | Ignore

        }

        private Dictionary<string, bool> _ButtonsDictionary;
        private string _Message;
        private ButtonsEnum _ClickedButton;

        #endregion
        #region Properties
        public ButtonsEnum ClickedButton
        {
            get { return _ClickedButton; }
            set { _ClickedButton = value; }
        }
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        public Dictionary<string, bool> ButtonsDictionary
        {
            get { return _ButtonsDictionary; }
            set { _ButtonsDictionary = value; }
        }
        #endregion
        #region Constructors
        public CustomMessageBoxVM():base()
        {           

        }
        public CustomMessageBoxVM(ButtonsEnum buttonEnum, string message):base()
        {
           // RequestNavigation += Navigate;
            Message = message;

            _ButtonsDictionary = new Dictionary<string, bool>();

            foreach (ButtonsEnum val in Enum.GetValues(typeof(ButtonsEnum)))
            {
                 _ButtonsDictionary.Add(val.ToString(), (buttonEnum & val) > 0); 
            }
            string keyname = _ButtonsDictionary.Keys.ToArray()[0];
        }
        
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
