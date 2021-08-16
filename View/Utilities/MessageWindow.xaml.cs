using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace View.Utilities
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
        //public MessageWindow(ViewModel.Utilities.MessageVM initializedVM)
        //{
            
        //    InitializeComponent();
        //    //Resources["vm"] = new ViewModel.Utilities.MessageVM(message);
        //    ViewModel.Utilities.MessageVM tempvm = (ViewModel.Utilities.MessageVM) Resources["vm"];

        //    tempvm.Message = initializedVM.Message;
        //    tempvm.Title = initializedVM.Title;
        //    this.Title = tempvm.Title;
            
        //    //tempvm.copyFrom(initializedvm);
        //    //initalixedvm.copyTo(tempvm);
        //}
    }
}
