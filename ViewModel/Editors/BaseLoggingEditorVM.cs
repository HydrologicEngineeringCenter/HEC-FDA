using FdaLogging;
using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Utilities;
using ViewModel.Saving;
using ViewModel.Utilities;
using ViewModel.Utilities.Transactions;

namespace ViewModel.Editors
{
    public abstract class BaseLoggingEditorVM : BaseEditorVM, IDisplayLogMessages
    {
        private ObservableCollection<LogItem> _MessageRows = new ObservableCollection<LogItem>();
        private LoggingLevel _SaveStatusLevel;
        private bool _IsExpanded;
        private CoordinatesFunctionEditorVM _EditorVM;
        private bool _SaveStatusVisible;

        public bool SaveStatusVisible
        {
            get { return _SaveStatusVisible; }
            set { _SaveStatusVisible = value; NotifyPropertyChanged(); }
        }
        public CoordinatesFunctionEditorVM EditorVM
        {
            get { return _EditorVM; }
            set { _EditorVM = value; NotifyPropertyChanged(); }
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
        public BaseLoggingEditorVM(UncertainPairedData defaultCurve, string xLabel, string yLabel, string chartTitle, EditorActionManager actionManager) : base(actionManager)
        {
            EditorVM = new CoordinatesFunctionEditorVM(defaultCurve, xLabel, yLabel, chartTitle);
            EditorVM.TableChanged += EditorVM_TableChanged;
        }

        public BaseLoggingEditorVM(ChildElement elem, string xLabel, string yLabel, string chartTitle, EditorActionManager actionManager):base(elem, actionManager)
        {
            if (elem.Curve != null)
            {
                EditorVM = new CoordinatesFunctionEditorVM(elem.Curve, xLabel, yLabel, chartTitle);
            }
            else
            {
                EditorVM = new CoordinatesFunctionEditorVM();
            }
            EditorVM.TableChanged += EditorVM_TableChanged;
            ReloadMessages();
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
        public void FilterRowsByLevel(LoggingLevel level)
        {
            MessageRows = PersistenceFactory.GetElementManager(CurrentElement).GetLogMessagesByLevel(level, CurrentElement.Name);
        }

        public void DisplayAllMessages()
        {
            MessageRows = PersistenceFactory.GetElementManager(CurrentElement).GetLogMessages(CurrentElement.Name);
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

            //get IMessages from the coord func editor
            //and convert them into temp log messages
            List<LogItem> funcLogs = GetTempLogsFromCoordinatesFunctionEditor();
            //add them to the temp errors so that they will be included
            TempErrors.AddRange(funcLogs);

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
        /// <summary>
        /// Grabs the logs from the sqlite database. Adds the "TempErrors". Adds any messages that are on the IFdaFunction's CoordinatesFunction. 
        /// </summary>
        public void ReloadMessages(bool saving = false)
        {
            if (CurrentElement != null)
            {
                IElementManager manager = PersistenceFactory.GetElementManager(CurrentElement);
                //get log messages from DB
                MessageRows = manager.GetLogMessages(CurrentElement.Name);
                UpdateMessages(saving);
            }
        }

        private List<LogItem> GetTempLogsFromCoordinatesFunctionEditor()
        {
            List<LogItem> logs = new List<LogItem>();
            List<IMessage> messagesFromEditor = new List<IMessage>();
            if(EditorVM == null || EditorVM.Function == null)
            {
                return logs;
            }
            //todo: waiting to see what we are going to do with logging
            //get messages from the editor
            //messagesFromEditor.AddRange(EditorVM.Messages);

            //get messages from the editor's function

            //if( EditorVM.Function.Messages != null)
            //{
            //    messagesFromEditor.AddRange(EditorVM.Function.Messages);
            //}
            foreach (IMessage message in messagesFromEditor)
            {
                LoggingLevel level = TranslateValidationLevelToLogLevel(message.Level);
                logs.Add(LogItemFactory.FactoryTemp(level, message.Notice));
            }
            //order the list by the log level. Highest on top
            var sortedLogList = logs
                .OrderByDescending(x => (int)(x.LogLevel))
                .ToList();
            
            return logs;
        }

        private LoggingLevel TranslateValidationLevelToLogLevel(IMessageLevels level)
        {
            LoggingLevel logLevel = LoggingLevel.Info;
            switch (level)
            {
                case IMessageLevels.FatalError:
                    {
                        logLevel = LoggingLevel.Fatal;
                        break;
                    }
                case IMessageLevels.Error:
                    {
                        logLevel = LoggingLevel.Error;
                        break;
                    }
                case IMessageLevels.Message:
                    {
                        logLevel = LoggingLevel.Warn;
                        break;
                    }
            }
            return logLevel;
        }

    }
}
