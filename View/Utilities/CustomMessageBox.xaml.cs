using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : UserControl
    {
        public CustomMessageBox()
        {
            InitializeComponent();

            btn_Yes.Click += Btn_Click;
            btn_No.Click += Btn_Click;
            btn_Ok.Click += Btn_Click;
            btn_Cancel.Click += Btn_Click;
            btn_Close.Click += Btn_Click;
            btn_Abort.Click += Btn_Click;
            btn_Retry.Click += Btn_Click;
            btn_Ignore.Click += Btn_Click;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();

            HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM vm = (HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM)this.DataContext;
            switch (name)
            {

                case "Yes":
                    vm.ClickedButton = HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.Yes;

                    break;
                case "No":
                    vm.ClickedButton = HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.No;

                    break;
                case "OK":
                    vm.ClickedButton = HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.OK;
                   
                    break;
                case "Cancel":
                    vm.ClickedButton = HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.Cancel;

                    break;
                case "Close":
                    vm.ClickedButton = HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.Close;

                    break;
                case "Abort":
                    vm.ClickedButton = HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.Abort;

                    break;
                case "Retry":
                    vm.ClickedButton = HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.Retry;

                    break;
                case "Ignore":
                    vm.ClickedButton = HEC.FDA.ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.Ignore;

                    break;
            }


            var window = Window.GetWindow(this);
            window.Close();

        }

        //private void btn_No_Click(object sender, RoutedEventArgs e)
        //{
        //    var window = Window.GetWindow(this);
        //    window.Close();
        //}

        //private void btn_Ok_Click(object sender, RoutedEventArgs e)
        //{
            
        //}

        //private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        //{
        //    var window = Window.GetWindow(this);
        //    window.Close();
        //}

        //private void btn_Close_Click(object sender, RoutedEventArgs e)
        //{
        //    var window = Window.GetWindow(this);
        //    window.Close();
        //}
    }
}
