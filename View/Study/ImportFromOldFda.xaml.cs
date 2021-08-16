using ViewModel.Study;
using Importer;
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

namespace View.Study
{
    /// <summary>
    /// Interaction logic for ImportFromOldFda.xaml
    /// </summary>
    public partial class ImportFromOldFda : UserControl
    {
        public ImportFromOldFda()
        {
            InitializeComponent();
            
        }

        private void Vm_Import(object sender, EventArgs e)
        {
            if (sender is ImportFromOldFdaVM)
            {
                ImportFromOldFdaVM vm = (ImportFromOldFdaVM)sender;
                AsciiImport import = new AsciiImport();
                //this line should write all the applicable data to the database.
                //It creates the view model objects and then writes them to the sqlite db in the "flush" 
                //for each fda element type. The sqlite db needs to already exist.
                import.ImportAsciiData(vm.ImportFilePath, AsciiImport.ImportOptions.ImportEverything);
            }
        }

        private void TextBoxFolderBrowser_SelectionMade(string fullpath)
        {
            ImportFromOldFdaVM vm = (ImportFromOldFdaVM)this.DataContext;
            vm.FolderPath = fullpath;
        }

        private void TextBoxFileBrowser_SelectionMade(string fullpath, string filename)
        {
            ImportFromOldFdaVM vm = (ImportFromOldFdaVM)this.DataContext;
            vm.ImportFilePath = fullpath;
            txtbox.Text = fullpath;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ImportFromOldFdaVM vm = (ImportFromOldFdaVM)this.DataContext;
            vm.Import += Vm_Import;
        }
    }
}
