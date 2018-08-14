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

namespace FdaTester.ModelTester.UnitTests.LogP3_Fortran_vs_CSharp
{
    /// <summary>
    /// Interaction logic for createFlowData.xaml
    /// </summary>
    
    public partial class createFlowData : Window
    {
        private string _filePath;
        public createFlowData()
        {
            InitializeComponent();
        }

        private void btn_FileBrowse_Click(object sender, RoutedEventArgs e)
        {
            string FileName = null;
            Microsoft.Win32.SaveFileDialog OpenFileDialog = new Microsoft.Win32.SaveFileDialog();

            //OpenFileDialog.Filter = "Excel (*.xlsx)|*.xlsx" & "| All files (*.*)|*.*";

            if (OpenFileDialog.ShowDialog() == true)
            {
                txt_filePath.Text = OpenFileDialog.FileName.ToString();
                _filePath = txt_filePath.Text;
                //this.Focus();
            }
        }

        private void btn_Calculate_Click(object sender, RoutedEventArgs e)
        {

            LogPearsonIIIFunctionTester testobject = LogPearsonIIIFunctionTester.TestObjectGenerator(_filePath, "test", LogPearsonIIIFunctionTester.TestFunctionGenerator(), LogPearsonIIIFunctionTester.TestProbabilitiesGenerator());

            List<LogPearsonIIIFunctionTester> listOfTestObjects = new List<LogPearsonIIIFunctionTester>();

            string filePath = txt_filePath.Text;

            if (System.IO.File.Exists(filePath)) { System.IO.File.Delete(filePath); }

            //mean: 1-9; st dev: 0-2; skew: -2 - 2; por: 1-300;
            if (rad_skew.IsChecked == true)
            {
                for (double k =  Convert.ToDouble(txt_startIndex.Text); k <= Convert.ToDouble(txt_endIndex.Text) + .0001; k += Convert.ToDouble(txt_step.Text)) //TEST: change the range that k goes through. Then change which parameter gets k in the line below.
                {
                    if (k <.000001 && k>-.000001) { continue; }
                    double[] testParams = { Convert.ToDouble(txt_mean.Text),Convert.ToDouble(txt_stDev.Text), k, Convert.ToInt32(txt_POR.Text) }; //mean, st dev, skew, por
                    float[] testProbs = LogPearsonIIIFunctionTester.TestProbabilitiesGenerator();

                    LogPearsonIIIFunctionTester myLog3 = new LogPearsonIIIFunctionTester(filePath, testParams, testProbs);

                    myLog3.cSharpFlowValues = myLog3.CSharpFlow();
                    myLog3.fortranFlowValues = myLog3.FortranFlows();

                    myLog3.absoluteDiffValues = myLog3.GetAbsoluteDifference();
                    myLog3.percentDiffValues = myLog3.GetPercentDifference();

                    listOfTestObjects.Add(myLog3);
                }
            }
            else if(rad_mean.IsChecked == true) //1-9
            {
                for (double k = Convert.ToDouble(txt_startIndex.Text); k <= Convert.ToDouble(txt_endIndex.Text) + .0001; k += Convert.ToDouble(txt_step.Text)) //TEST: change the range that k goes through. Then change which parameter gets k in the line below.
                {
                    
                    double[] testParams = { k, Convert.ToDouble(txt_stDev.Text), Convert.ToDouble(txt_skew.Text), Convert.ToInt32(txt_POR.Text) }; //mean, st dev, skew, por
                    float[] testProbs = LogPearsonIIIFunctionTester.TestProbabilitiesGenerator();

                    LogPearsonIIIFunctionTester myLog3 = new LogPearsonIIIFunctionTester(filePath, testParams, testProbs);

                    myLog3.cSharpFlowValues = myLog3.CSharpFlow();
                    myLog3.fortranFlowValues = myLog3.FortranFlows();

                    myLog3.absoluteDiffValues = myLog3.GetAbsoluteDifference();
                    myLog3.percentDiffValues = myLog3.GetPercentDifference();

                    listOfTestObjects.Add(myLog3);
                }
            }
            else if (rad_stDev.IsChecked == true) //0-2
            {
                for (double k = Convert.ToDouble(txt_startIndex.Text); k <= Convert.ToDouble(txt_endIndex.Text) + .0001; k += Convert.ToDouble(txt_step.Text)) //TEST: change the range that k goes through. Then change which parameter gets k in the line below.
                {

                    double[] testParams = { Convert.ToDouble(txt_mean.Text), k, Convert.ToDouble(txt_skew.Text), Convert.ToInt32(txt_POR.Text) }; //mean, st dev, skew, por
                    float[] testProbs = LogPearsonIIIFunctionTester.TestProbabilitiesGenerator();

                    LogPearsonIIIFunctionTester myLog3 = new LogPearsonIIIFunctionTester(filePath, testParams, testProbs);

                    myLog3.cSharpFlowValues = myLog3.CSharpFlow();
                    myLog3.fortranFlowValues = myLog3.FortranFlows();

                    myLog3.absoluteDiffValues = myLog3.GetAbsoluteDifference();
                    myLog3.percentDiffValues = myLog3.GetPercentDifference();

                    listOfTestObjects.Add(myLog3);
                }
            }
            else if (rad_por.IsChecked == true) //0-2
            {
                for (double k = Convert.ToDouble(txt_startIndex.Text); k <= Convert.ToDouble(txt_endIndex.Text) + .0001; k += Convert.ToDouble(txt_step.Text)) //TEST: change the range that k goes through. Then change which parameter gets k in the line below.
                {

                    double[] testParams = { Convert.ToDouble(txt_mean.Text), Convert.ToDouble(txt_stDev.Text), Convert.ToDouble(txt_skew.Text), k }; //mean, st dev, skew, por
                    float[] testProbs = LogPearsonIIIFunctionTester.TestProbabilitiesGenerator();

                    LogPearsonIIIFunctionTester myLog3 = new LogPearsonIIIFunctionTester(filePath, testParams, testProbs);

                    myLog3.cSharpFlowValues = myLog3.CSharpFlow();
                    myLog3.fortranFlowValues = myLog3.FortranFlows();

                    myLog3.absoluteDiffValues = myLog3.GetAbsoluteDifference();
                    myLog3.percentDiffValues = myLog3.GetPercentDifference();

                    listOfTestObjects.Add(myLog3);
                }
            }
            else
            {
                double[] testParams = { Convert.ToDouble(txt_mean.Text), Convert.ToDouble(txt_stDev.Text), Convert.ToDouble(txt_skew.Text), Convert.ToInt32(txt_POR.Text) }; //mean, st dev, skew, por
                float[] testProbs = LogPearsonIIIFunctionTester.TestProbabilitiesGenerator();

                LogPearsonIIIFunctionTester myLog3 = new LogPearsonIIIFunctionTester(filePath, testParams, testProbs);

                myLog3.cSharpFlowValues = myLog3.CSharpFlow();
                myLog3.fortranFlowValues = myLog3.FortranFlows();

                myLog3.absoluteDiffValues = myLog3.GetAbsoluteDifference();
                myLog3.percentDiffValues = myLog3.GetPercentDifference();

                listOfTestObjects.Add(myLog3);
            }

            Utilities.DataExporter.ExportDelimitedColumns(filePath, listOfTestObjects);
            Utilities.DataExporter.writeTestResultsToXML(System.IO.Path.ChangeExtension(_filePath,"xml"), listOfTestObjects);
            MessageBox.Show("Finished Writing File.", "Done");
        }
        
    }
}
