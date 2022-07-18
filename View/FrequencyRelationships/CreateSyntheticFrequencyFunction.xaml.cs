using HEC.FDA.ViewModel.FrequencyRelationships;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HEC.FDA.View.FrequencyRelationships
{
    /// <summary>
    /// Interaction logic for CreateSyntheticFrequencyFunction.xaml
    /// </summary>
    public partial class CreateSyntheticFrequencyFunction : UserControl
    {
        public CreateSyntheticFrequencyFunction()
        {
            InitializeComponent();
        }

        private void TextBoxFileBrowser_SelectionMade(string fullpath, string filename)
        {
            if (DataContext is CreateSyntheticFrequencyFunctionVM vm)
            {
                vm.Path = fullpath;
                ImportBtn.IsEnabled = true;
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CreateSyntheticFrequencyFunctionVM vm)
            {
                vm.Import();
                ImportBtn.IsEnabled = false;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Close();
        }

    }
}
