using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
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
            Message = message;

            _ButtonsDictionary = new Dictionary<string, bool>();

            foreach (ButtonsEnum val in Enum.GetValues(typeof(ButtonsEnum)))
            {
                 _ButtonsDictionary.Add(val.ToString(), (buttonEnum & val) > 0); 
            }
            string keyname = _ButtonsDictionary.Keys.ToArray()[0];
        }
        //public CustomMessageBoxVM(bool Yes_Button, bool No_Button, bool OK_Button, bool Cancel_Button, bool Close_Button, bool Abort_Button = false, bool Retry_Button = false, bool Ignore_Button = false)
        //{
        //    _ButtonsDictionary = new Dictionary<string, bool>();
        //    ButtonsEnum myButtons = new ButtonsEnum();

        //    if (Yes_Button == true)
        //    {
        //        myButtons = myButtons | ButtonsEnum.Yes;
        //    }
            
        //    if (No_Button) { myButtons =myButtons | ButtonsEnum.No; }


        //    if((myButtons & ButtonsEnum.Yes) == ButtonsEnum.Yes)
        //    {
        //        _ButtonsDictionary.Add("Yes", true);
        //    }
        //    if ((myButtons & ButtonsEnum.No) == ButtonsEnum.No)
        //    {
        //        _ButtonsDictionary.Add("No", true);
        //    }
        //    if ((myButtons & ButtonsEnum.OK) == ButtonsEnum.OK)
        //    {
        //        _ButtonsDictionary.Add("OK", true);
        //    }
        //    if ((myButtons & ButtonsEnum.Cancel) == ButtonsEnum.Cancel)
        //    {
        //        _ButtonsDictionary.Add("Cancel", true);
        //    }
        //    if ((myButtons & ButtonsEnum.Close) == ButtonsEnum.Close)
        //    {
        //        _ButtonsDictionary.Add("Close", true);
        //    }

        //}

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
