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

namespace FdaTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogP3FortranVSCSharp_Click(object sender, RoutedEventArgs e)
        {
        //    Window FortranCSharpWindow = new ModelTester.UnitTests.FortranCSharpLogP3Tester();
        //    FortranCSharpWindow.Show();
        }


        private void LogP3Constructors_Click(object sender, RoutedEventArgs e)
        {
            // FdaTester.ModelTester.UnitTests.LogPearsonIIIConstructorTest.LogP3ConstructorTesting myTest = new ModelTester.UnitTests.LogPearsonIIIConstructorTest.LogP3ConstructorTesting();
        //    Window constructorWindow = new ModelTester.UnitTests.LogPearsonIIIConstructorTest.ConstructorTestingResults();
          //  constructorWindow.Show();
        }

        private void btnLogPearsonIIITestAgainstR_Click(object sender, RoutedEventArgs e)
        {
            //ModelTester.UnitTests.LogPearsonIIITestAgainstR newTest = new ModelTester.UnitTests.LogPearsonIIITestAgainstR("C:\\LogPearsonIIITestAgainstR\\DataForR.txt", 1000, 100);
            //newTest.GenerateRTestFile();
        }

        private void btnComputeTesting_Click(object sender, RoutedEventArgs e)
        {
            //ModelTester.UnitTests.Compute_Testing.SimpleTestCase.SimpleTest();
            ModelTester.UnitTests.Compute_Testing.ComputeTestingWindow ctw = new ModelTester.UnitTests.Compute_Testing.ComputeTestingWindow();
            ctw.Show();
            
        }

        private void btnDatabaseTesting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           // FdaModel.Utilities.Messager.Logger.Instance.Flush();
        }

        private void btnCompositionTesting_Click(object sender, RoutedEventArgs e)
        {
            ModelTester.UnitTests.Functions.Composition.CompositionTestingView compositionView = new ModelTester.UnitTests.Functions.Composition.CompositionTestingView();
            compositionView.Show();
        }
    }
}
