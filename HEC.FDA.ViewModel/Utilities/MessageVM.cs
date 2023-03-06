namespace HEC.FDA.ViewModel.Utilities
{
    public class MessageVM : BaseViewModel
    {
        private  string _errorMessage;

        public string Message
        {
            get { return _errorMessage;}
            set { _errorMessage = value; NotifyPropertyChanged(); }
            
        }
       public MessageVM():base()
        {
            _errorMessage = "This is my message.";
        }

        public MessageVM(string message):base()
        {
            Message = message;
            //HelpAction.IsVisible = false;
        }
        

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

     
    }
}
