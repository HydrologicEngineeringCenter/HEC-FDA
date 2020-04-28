using FdaLogging;
using FdaViewModel.Editors;
using FdaViewModel.Saving;
using FdaViewModel.Utilities.Transactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This class exists to move logic out of the main OccupancyTypesEditorVM. This base class
    /// handles the logging between the editor and the Messages control.
    /// </summary>
    public abstract class BaseOcctypeLoggingEditor : BaseEditorVM, IDisplayLogMessages
    {
        private ObservableCollection<LogItem> _MessageRows = new ObservableCollection<LogItem>();
        private LoggingLevel _SaveStatusLevel;
        private bool _IsExpanded;
        private bool _SaveStatusVisible;
        private IOccupancyTypeEditable _SelectedOccType;
        private ObservableCollection<IOccupancyTypeGroupEditable> _OccTypeGroups;

        #region properties
        #region messages
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
        public ObservableCollection<FdaLogging.LogItem> MessageRows
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

        #endregion

        public ObservableCollection<IOccupancyTypeGroupEditable> OccTypeGroups
        {
            get { return _OccTypeGroups; }
            set { _OccTypeGroups = value; NotifyPropertyChanged(); }
        }
        public IOccupancyTypeEditable SelectedOccType
        {
            get { return _SelectedOccType; }
            set
            {
                _SelectedOccType = value;
                if(_SelectedOccType == null)
                {
                    return;
                }
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region constructors
        public BaseOcctypeLoggingEditor(ObservableCollection<IOccupancyTypeGroupEditable> occtypeGroups, EditorActionManager actionManager):base(actionManager)
        {
            OccTypeGroups = occtypeGroups;

            //add events for the logging
            foreach(IOccupancyTypeGroupEditable group in occtypeGroups)
            {
                foreach(IOccupancyTypeEditable ot in group.Occtypes)
                {
                    ot.UpdateMessagesEvent += Occtype_UpdateMessagesEvent;
                }
            }

        }


        /// <summary>
        /// This gets called when one of the individual occtype editable classes gets an update messages call
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Occtype_UpdateMessagesEvent(object sender, EventArgs e)
        {
            if(sender is IOccupancyTypeEditable)
            {
                BaseViewModel senderBVM = (BaseViewModel)sender;
                if(senderBVM.HasFatalError)
                {
                    //the "save" button's is enabled is bound to HasFatalError and the tooltip
                    //message is bound to "Error". In this case the fatal error and error message
                    //are on the occtype editable class and so i need to transfer those to this class
                    //which is the vm that the save button is binding to.
                    this.Error = senderBVM.Error;
                    this.HasFatalError = true;
                }
                TempErrors.AddRange(((IDisplayLogMessages)sender).TempErrors);
            }
            UpdateMessages();
        }
        #endregion

        #region messaging methods

        private List<LogItem> GetTempLogsFromCoordinatesFunctionEditor()
        {
            List<LogItem> logs = new List<LogItem>();
            List<IMessage> messagesFromEditor = new List<IMessage>();
            if (SelectedOccType == null)
            {
                return logs;
            }
            //get messages from the editors
            messagesFromEditor.AddRange(SelectedOccType.StructureEditorVM.Messages);
            messagesFromEditor.AddRange(SelectedOccType.ContentEditorVM.Messages);
            messagesFromEditor.AddRange(SelectedOccType.VehicleEditorVM.Messages);
            messagesFromEditor.AddRange(SelectedOccType.OtherEditorVM.Messages);

            //get messages from the editor's function
            if (SelectedOccType.StructureEditorVM.Function.Messages != null)
            {
                messagesFromEditor.AddRange(SelectedOccType.StructureEditorVM.Function.Messages);
            }
            if (SelectedOccType.ContentEditorVM.Function.Messages != null)
            {
                messagesFromEditor.AddRange(SelectedOccType.ContentEditorVM.Function.Messages);
            }
            if (SelectedOccType.VehicleEditorVM.Function.Messages != null)
            {
                messagesFromEditor.AddRange(SelectedOccType.VehicleEditorVM.Function.Messages);
            }
            if (SelectedOccType.OtherEditorVM.Function.Messages != null)
            {
                messagesFromEditor.AddRange(SelectedOccType.OtherEditorVM.Function.Messages);
            }
            
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
                    //if (li.Message.Equals("Last Saved"))
                    //{
                    //    li.Message = "Last Saved: " + li.Date;
                    //}
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
            for (int i = TempErrors.Count - 1; i >= 0; i--)
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
            IElementManager manager = PersistenceFactory.GetElementManager(CurrentElement);
            //get log messages from DB
            MessageRows = manager.GetLogMessages(CurrentElement.Name);
            UpdateMessages(saving);
        }

        private void UpdateSaveStatusLevel()
        {
            //This method will also expand the 
            //basically i want to set the error level to be the highest log level in my list
            //this will change the background color of the Save button
            if (ContainsLogLevel(LoggingLevel.Fatal))
            {
                IsExpanded = true;
                SaveStatusLevel = LoggingLevel.Fatal;
            }
            else if (ContainsLogLevel(LoggingLevel.Error))
            {
                IsExpanded = true;
                SaveStatusLevel = LoggingLevel.Error;
            }
            else if (ContainsLogLevel(LoggingLevel.Warn))
            {
                IsExpanded = true;
                SaveStatusLevel = LoggingLevel.Warn;
            }
            else if (ContainsLogLevel(LoggingLevel.Info))
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
            foreach (LogItem li in MessageRows)
            {
                if (li.LogLevel == level)
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }
        public void FilterRowsByLevel(FdaLogging.LoggingLevel level)
        {

            MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessagesByLevel(level, CurrentElement.Name);
        }

        public void DisplayAllMessages()
        {
            MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessages(CurrentElement.Name);
        }

      

        #endregion


    }
}
