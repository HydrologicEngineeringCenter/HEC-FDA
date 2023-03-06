using System.Windows.Controls;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : UserControl
    {
        public MessageWindow()
        {
            InitializeComponent();
        }
        //public MessageWindow(HEC.FDA.ViewModel.Utilities.MessageVM initializedVM)
        //{
            
        //    InitializeComponent();
        //    //Resources["vm"] = new HEC.FDA.ViewModel.Utilities.MessageVM(message);
        //    HEC.FDA.ViewModel.Utilities.MessageVM tempvm = (HEC.FDA.ViewModel.Utilities.MessageVM) Resources["vm"];

        //    tempvm.Message = initializedVM.Message;
        //    tempvm.Title = initializedVM.Title;
        //    this.Title = tempvm.Title;
            
        //    //tempvm.copyFrom(initializedvm);
        //    //initalixedvm.copyTo(tempvm);
        //}
    }
}
