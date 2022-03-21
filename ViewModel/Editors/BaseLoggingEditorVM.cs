using FdaLogging;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Utilities.Transactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Editors
{
    public abstract class BaseLoggingEditorVM : BaseEditorVM, IDisplayLogMessages
    {
        private ObservableCollection<LogItem> _MessageRows = new ObservableCollection<LogItem>();
        private LoggingLevel _SaveStatusLevel;
        private bool _IsExpanded;
        private bool _SaveStatusVisible;

        /// <summary>
        /// TODO: It looks like this is not being used. I think it is used by the message expander control to expand some panel and display
        /// that the element has saved. I'm not sure what we are doing with the message expander at this time. 3/8/22
        /// </summary>
        public bool SaveStatusVisible
        {
            get { return _SaveStatusVisible; }
            set { _SaveStatusVisible = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Used to color the last couple of words in the top log item in the expander if there are errors or messages
        /// </summary>
        public LoggingLevel SaveStatusLevel
        {
            get { return _SaveStatusLevel; }
            set 
            {
                if (_SaveStatusLevel != value)
                {
                    _SaveStatusLevel = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<LogItem> MessageRows
        {
            get { return _MessageRows; }
            set
            {
                _MessageRows = value;
                NotifyPropertyChanged(nameof(SaveStatusLevel));
                NotifyPropertyChanged("MessageRows");
                NotifyPropertyChanged("MessageCount");
            }
        }
        public int MessageCount
        {
            get { return _MessageRows.Count; }
        }
        /// <summary>
        /// When set to true it will expand the Messages Expander so that the user can see
        /// the latest messages.
        /// </summary>
        public bool IsExpanded 
        {
            get { return _IsExpanded; }
            set
            {          
                if (_IsExpanded != value)
                {
                    _IsExpanded = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// TempErrors should only be used by the validation in BaseViewModel. It will get cleared and updated any time
        /// notifyPropertyChanged gets called.
        /// </summary>
        public List<LogItem> TempErrors 
        { 
            get; 
            set; 
        }
        #region Constructors
        public BaseLoggingEditorVM( EditorActionManager actionManager) : base(actionManager)
        {
        }

        public BaseLoggingEditorVM(ChildElement elem, EditorActionManager actionManager):base(elem, actionManager)
        {
        }
        #endregion
        /// <summary>
        /// Event that bubbles up from the tables in the coordinate function editor so that
        /// I can set HasChanges = true. This way we actually save when the user hits the save button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EditorVM_TableChanged(object sender, EventArgs e)
        {            
            HasChanges = true;
            //something has changed. Turn off the save status header message
            SaveStatusLevel = LoggingLevel.Debug;
        }
       
        private void UpdateSaveStatusLevel()
        {
            //This method will also expand the 
            //basically i want to set the error level to be the highest log level in my list
            //this will change the background color of the Save button
            if(ContainsLogLevel(LoggingLevel.Fatal))
            {
                IsExpanded = true;
                SaveStatusLevel = LoggingLevel.Fatal;
            }
            else if(ContainsLogLevel(LoggingLevel.Error))
            {
                IsExpanded = true;
                SaveStatusLevel = LoggingLevel.Error;
            }
            else if (ContainsLogLevel(LoggingLevel.Warn))
            {
                IsExpanded = true;
                SaveStatusLevel = LoggingLevel.Warn;
            }
            else if(ContainsLogLevel(LoggingLevel.Info))
            {
                SaveStatusLevel = LoggingLevel.Info;
            }
            else
            {
                //this is being used here to just be the default lowest level.
                SaveStatusLevel = LoggingLevel.Debug;
            }
        }

        private bool ContainsLogLevel(LoggingLevel level)
        {
            bool retval = false;
            foreach(LogItem li in MessageRows)
            {
                if (li.LogLevel == level)
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }

        /// <summary>
        /// Gets rid of any temperary messages from the list of messages and adds the new list of temp
        /// messages from "TempErrors" property.
        /// </summary>
        public void UpdateMessages(bool saving = false)
        {
            //there are three places that messages come from.
            // 1.) The sqlite database
            // 2.) Temp messages from the validation of the "rules" (ie. Name cannot be blank)
            // 3.) Temp messages from any object that implements IValidate. These messages come out of the model, stats, functions

            //get rid of any temp logs
            ObservableCollection<LogItem> tempList = new ObservableCollection<LogItem>();
            foreach (LogItem li in MessageRows)
            {
                //exclude any temp logs
                if (!li.IsTempLog())
                {
                    tempList.Add(li);
                }              
            }

            //i want all of these messages to be put on the top of the list, but i want to respect their order. This 
            //means i need to insert at 0 and start with the last in the list
            for (int i = TempErrors.Count-1;i>=0; i--)
            {
                tempList.Insert(0, TempErrors[i]);
            }
            MessageRows = tempList;
            TempErrors.Clear();
            //if we are saving then we want the save status to be visible
            if (saving)
            {
                UpdateSaveStatusLevel();
            }
            else
            {
                SaveStatusLevel = LoggingLevel.Debug;
            }
        }
    }
}
