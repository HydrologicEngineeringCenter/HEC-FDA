using HEC.MVVMFramework.ViewModel.Implementations;

namespace HEC.FDA.ViewModel.Compute
{
    public class ComputeBase : BaseViewModel
    {

        private int _Progress;
        private string _NumberCompleted;
        private string _ProgressLabel;


        private SubscriberMessageViewModel _MessageVM = new SubscriberMessageViewModel();

        public SubscriberMessageViewModel MessageVM
        {
            get { return _MessageVM; }
        }

        public int Progress
        {
            get { return _Progress; }
            set { _Progress = value; NotifyPropertyChanged(); }
        }

        public string ProgressLabel
        {
            get { return _ProgressLabel; }
            set { _ProgressLabel = value; NotifyPropertyChanged(); }
        }

        public string NumberCompleted
        {
            get { return _NumberCompleted; }
            set { _NumberCompleted = value; NotifyPropertyChanged(); }
        }


        public ComputeBase()
        {

        }

    }
}
