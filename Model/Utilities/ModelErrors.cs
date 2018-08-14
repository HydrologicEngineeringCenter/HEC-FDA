using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaModel.Utilities.Messager;

namespace FdaModel
{
    public class ModelErrors
    {
        #region Fields
        private bool _IsValid;
        private List<ErrorMessage> _Messages;
        #endregion

        #region Propertiers
        public bool IsValid
        {
            get
            {
                return _IsValid;
            }
            private set
            {
                _IsValid = value;
            }
        }
        public List<ErrorMessage> Messages
        {
            get
            {
                return _Messages;
            }
            private set
            {
                _Messages = value;
            }
        }
        #endregion

        #region Constructor
        public ModelErrors(List<ErrorMessage> messages)
        {
            List<ErrorMessage> Messages = new List<ErrorMessage>();
            IsValid = true;
            for(int i = 0; i < messages.Count; i++)
            {
                if(messages[i].ErrorLevel == ErrorMessageEnum.Fatal)
                {
                    IsValid = false;
                }
                Messages.Add(new ErrorMessage(messages[i].Message, ErrorMessageEnum.Model | messages[i].ErrorLevel));
            }
        }

        public ModelErrors(ErrorMessage message)
        {
            if(message.ErrorLevel == ErrorMessageEnum.Fatal)
            {
                IsValid = false;
            }
            else
            {
                IsValid = true;
            }
            Messages.Add(new ErrorMessage(message.Message, ErrorMessageEnum.Model | message.ErrorLevel));
        }

        public ModelErrors( )
        {
            IsValid = true;
            Messages = new List<ErrorMessage>( );
        }
        #endregion

        #region Voids
        public void AddMessages(List<ErrorMessage> newMessages)
        {
            for(int i = 0; i < newMessages.Count; i++)
            {
                if(newMessages[i].ErrorLevel == ErrorMessageEnum.Fatal)
                {
                    IsValid = false;
                }
                Messages.Add(new ErrorMessage(newMessages[i].Message, ErrorMessageEnum.Model | newMessages[i].ErrorLevel));
            }
        }

        public void AddMessage(ErrorMessage newMessage)
        {
            if(newMessage.ErrorLevel == ErrorMessageEnum.Fatal)
            {
                IsValid = false;
            }
            Messages.Add(new ErrorMessage(newMessage.Message, ErrorMessageEnum.Model | newMessage.ErrorLevel));
        }

        public void ClearMessages( )
        {
            IsValid = true;
            Messages.Clear( );
        }

        public void ReportMessages()
        {
            for(int i = 0; i < Messages.Count; i++)
            {
                Logger.Instance.ReportMessage(Messages[i]);
            }
        }

        public static void ReportMessage(ErrorMessage newMessage)
        {
            Logger.Instance.ReportMessage(newMessage);
        }

     
        #endregion
    }
}
