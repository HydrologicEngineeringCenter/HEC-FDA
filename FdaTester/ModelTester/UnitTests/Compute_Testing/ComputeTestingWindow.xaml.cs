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
using System.Collections.ObjectModel;
using FdaModel.Functions.OrdinatesFunctions;

namespace FdaTester.ModelTester.UnitTests.Compute_Testing
{
    /// <summary>
    /// Interaction logic for ComputeTestingWindow.xaml
    /// </summary>
    public partial class ComputeTestingWindow : Window
    {

        private string _Path;
        private System.Collections.ObjectModel.ObservableCollection<BaseComputeTest> _Tests;

        public string Path
        {
            get { return _Path; }
            set { _Path = value;}
        }
        public System.Collections.ObjectModel.ObservableCollection<BaseComputeTest> Tests
        {
            get { return _Tests; }
            set { _Tests = value; }
        }

        public ComputeTestingWindow()
        {
            InitializeComponent();
            txtboxPath.Text = "C:\\Users\\q0heccdm\\Documents\\HEC FDA\\Simple Case 2.txt";

            System.Collections.ObjectModel.ObservableCollection<BaseComputeTest> tests = new System.Collections.ObjectModel.ObservableCollection<BaseComputeTest>();

            //add all the tests here

            //Compute_Testing.Tests.Outflow_Rating or = new Compute_Testing.Tests.Outflow_Rating();
            //Tests.AllFunctionsTest aft = new Compute_Testing.Tests.AllFunctionsTest();
            //Compute_Testing.Tests.InflowFrequency_InflowOutflow_Rating_0_1_3_ ior = new Tests.InflowFrequency_InflowOutflow_Rating_0_1_3_();
            //Tests.SimpleWithLotsOfNumbers stwn = new Tests.SimpleWithLotsOfNumbers();
            //Tests.SimpleTestCase stc = new Tests.SimpleTestCase();

            //tests.Add(aft);
            //tests.Add(or);
            //tests.Add(stc);
            //tests.Add(stwn);
            
            cmbboxTests.ItemsSource = tests;
            cmbboxTests.DisplayMemberPath = "Name";
            

        }

        private void btn_runSimpleCase_Click(object sender, RoutedEventArgs e)
        {
            ////SimpleTestCase.SimpleTest2();
            ////writeToExcel(SimpleTestCase.SimpleTest());
            ////SimpleWithLotsOfNumbers stc = new SimpleWithLotsOfNumbers();
            ////stc.WriteDataOut(stc.MyComputedObject, txt_path.Text);
            //string sourceFile = @"C:\Users\q0heccdm\AppData\Local\Temp\1\HEC\FdaModel_LogFile.txt";

            
            //for (int i = 0; i < 1; i++)
            //{
            //    Compute_Testing.Tests.AllFunctionsTest aft = new Compute_Testing.Tests.AllFunctionsTest();

            //    System.IO.StreamReader sr = new System.IO.StreamReader(sourceFile);
            //    List<string> errMesList = new List<string>();
            //    while(sr.ReadLine() != null)
            //    {
            //        errMesList.Add(sr.ReadLine());
            //    }
            //    object[] errMesArray = errMesList.ToArray(); 

            //    string destFile = @"C:\Users\q0heccdm\Documents\HEC FDA\Compute Testing\Compute Error Messages\Error Messages_" + i + ".txt";
            //    //System.IO.File.Copy(sourceFile, destFile,true);

            //    sr.Close();
            //    System.IO.File.Delete(sourceFile);

            //   ((BaseComputeTest)aft).WriteDataOut(((BaseComputeTest)aft).MyComputedObject, @"C:\Users\q0heccdm\Documents\HEC FDA\Compute Testing\Compute Test Results\Test_" + i + ".txt", errMesArray);

            //    //((BaseComputeTest)cmb_tests.SelectedItem).WriteDataOut(((BaseComputeTest)cmb_tests.SelectedItem).MyComputedObject, txt_path.Text);
            //}
            //MessageBox.Show("finished writing tests");
            ////writeToTXT(SimpleTestCase.SimpleTest2());

        }
        private void writeToTXT(FdaModel.ComputationPoint.Outputs.Realization testResult)
        {
            string excelPath = txtboxPath.Text;

            List<object[]> EXPORTDATA = new List<object[]>();

            List<string> columnNames = new List<string>();

            int numColumns = 0;


            for (int i = 0; i < testResult.Functions.Count(); i++)
            {
                switch (testResult.Functions[i].FunctionType)
                {
                    case FdaModel.Functions.FunctionTypes.InflowFrequency:
                        //float[] x = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;
                        //float[] y = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;

                        double[] x = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Function.Count];
                        ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Function.XValues.CopyTo(x, 0);
                        double[] y = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Function.Count];
                        ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Function.YValues.CopyTo(y, 0);


                        List<object> inflowList = new List<object>();
                        foreach (object obj in x)
                        {
                            inflowList.Add(obj);
                        }
                        EXPORTDATA.Add(inflowList.ToArray());

                        //exportData[numColumns] = inflowList.ToArray();
                        columnNames.Add("Inflow Probability");

                        numColumns++;

                        inflowList = new List<object>();
                        foreach (object obj in y)
                        {
                            inflowList.Add(obj);
                        }
                        EXPORTDATA.Add(inflowList.ToArray());

                        // exportData[numColumns] = inflowList.ToArray();
                        columnNames.Add("Inflow Peak Discharge");

                        numColumns++;

                        break;
                    case FdaModel.Functions.FunctionTypes.InflowOutflow:

                        throw new NotImplementedException();

                        break;

                    case FdaModel.Functions.FunctionTypes.Rating:
                        //float[] xs = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Xs;
                        //float[] ys = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Ys;

                        double[] xs = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Function.Count];
                        ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Function.XValues.CopyTo(xs, 0);
                        double[] ys = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Function.Count];
                        ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).Function.YValues.CopyTo(ys, 0);


                        //string nameer = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).ToString();

                        List<object> newList = new List<object>();
                        foreach (object obj in xs)
                        {
                            newList.Add(obj);
                        }

                        EXPORTDATA.Add(newList.ToArray());
                        //exportData[numColumns] = newList.ToArray();
                        columnNames.Add("Outflow Peak Discharge");
                        numColumns++;

                        newList = new List<object>();
                        foreach (object obj in ys)
                        {
                            newList.Add(obj);
                        }
                        EXPORTDATA.Add(newList.ToArray());
                        //exportData[numColumns] = newList.ToArray();
                        columnNames.Add("Peak Exterior Stage");
                        numColumns++;                       


                        break;
                   
                    //case 2:
                    //    break;
                    //case 3:
                    //    break;
                    default:
                        break;

                }
            }

            
            //Get the AEP
            //object[] aepArray = new object[1] { testResult. testResult.AnnualExceedanceProbabilities };
            //EXPORTDATA.Add(aepArray);
            columnNames.Add("AEP");

            string[] colNames = columnNames.ToArray();
            object[][] exportData = EXPORTDATA.ToArray();



            Utilities.DataExporter.ExportDelimitedColumns(excelPath, exportData, colNames);
            //MessageBox.Show("Finished Writing");

        }

        private void btn_plot_Click(object sender, RoutedEventArgs e)
        {
            //List<FdaModel.Functions.BaseFunction> listOfFunctions = new List<FdaModel.Functions.BaseFunction>();
            // Create ordinates function
            //float[] x = new float[] { 0, 100, 200, 500, 1000 };
            //float[] y = new float[] { 2, 200, 300, 600, 1100 };
            //FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction inflowOutflow = (FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction.OrdinatesFunctionFactory(x, y, FdaModel.Functions.FunctionTypes.InflowOutflow);

            //listOfFunctions.Add(inflowOutflow);
            //FunctionsVisualizer fv = new FunctionsVisualizer(listOfFunctions);
            // fv.Show();


            double[] peakFlowData1 = new double[] { 100, 200, 300, 1000, 5000 };
            FdaModel.Functions.FrequencyFunctions.LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData1);

            double[] xs1 = new double[] { 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            double[] ys1 = new double[] { 50, 100, 200, 300, 400, 500, 600, 700, 1000 };
            OrdinatesFunction three = new OrdinatesFunction(xs1, ys1, FdaModel.Functions.FunctionTypes.Rating);


            double[] x4 = new double[] { 0, 1, 2, 5, 7, 8, 9, 10, 12, 15 };
            double[] y4 = new double[] { 0, 0, 0, 600, 1100, 1300, 1800, 10000, 30000, 100000 };
            OrdinatesFunction seven = new OrdinatesFunction(x4, y4, FdaModel.Functions.FunctionTypes.InteriorStageDamage);


            double[] x5 = new double[] { .2448074f,.3249009f,.4049944f,.4850879f,.633826f };
            double[] y5 = new double[] { 1100,1300,1800,10000,30000 };
            OrdinatesFunction eight = new OrdinatesFunction(x5, y5, FdaModel.Functions.FunctionTypes.DamageFrequency);


            //1. Create computation point
            //FdaModel.ComputationPoint indexPoint = new FdaModel.ComputationPoint("anIndexPoint", "aStream", "intheWoP", 1999);

            //2. Create lp3 function
            double[] peakFlowData = new double[] { 100, 200, 300, 400, 1000 };
            FdaModel.Functions.FrequencyFunctions.LogPearsonIII myLP3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            //// Create ordinates function
            //float[] x = new float[] { 0, 100, 200, 500, 1000 };
            //float[] y = new float[] { 2, 200, 300, 600, 1100 };
            //FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction inflowOutflow = (FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction.OrdinatesFunctionFactory(x, y, FdaModel.Functions.FunctionTypes.InflowOutflow);



            //3. Create ordinates function
            //float[] ys = new float[] { 50, 100, 1000, 5000, 12000 };
            //float[] xs = new float[] { 3, 4, 5, 6, 10 };
            //FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction rating = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(xs, ys, FdaModel.Functions.FunctionTypes.Rating);

            //float[] xval = new float[] { 1, 100, 200, 500, 1000 };
            //float[] yval = new float[] { 3, 4, 5, 6, 10 };
            //FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction exteriorInteriorStage = new OrdinatesFunction(xs, ys, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

            //float[] xvalues = new float[] { 3,4,5,6,10 };
            //float[] yvalues = new float[] { 300,400,500,1000,5000 };
            //FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction interiorStageDamage = new OrdinatesFunction(xvalues,yvalues, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

            //float[] xvalue = new float[] { 0f,.9f,.99f,.999f, .9999f};
            //float[] yvalue = new float[] { 300.66f,489.275f,694.418f,1430.456f,4807.27f};
            //FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction damageFrequency = new OrdinatesFunction(xvalue, yvalue, FdaModel.Functions.FunctionTypes.DamageFrequency);

            //calculate the damage frequency curve
            //FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)myLP3).GetOrdinatesFunction();
            //for (int i = 0; i < iof.Xs.Count(); i++)
            //{
            //    Console.WriteLine("x: " + iof.Xs[i].ToString() + " y: " + iof.Ys[i].ToString());
            //}

            //4. Create Threshold
            //PerformanceThreshold threshold = new PerformanceThreshold(indexPoint, PerformanceThresholdTypes.OtherExteriorStage, 5d);

            //5. Create computable object
            List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>() { zero, three, seven, eight }; //myLP3, rating, interiorStageDamage, damageFrequency };// inflowOutflow, rating };


            Compute_Testing.Tests.Outflow_Rating temp = new Compute_Testing.Tests.Outflow_Rating();

            string sourceFile = @"C:\Users\q0hecjrk\AppData\Local\Temp\HEC\FdaModel_LogFile.txt";

            object[] errMesArray = new object[] { "this is a test" };
            //FdaModel.Utilities.Messager.Logger.Instance.Flush();
            if (System.IO.File.Exists(sourceFile))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(sourceFile);
                List<string> errMesList = new List<string>();
                while (sr.ReadLine() != null)
                {
                    errMesList.Add(sr.ReadLine());
                }
                errMesArray = errMesList.ToArray();

                sr.Close();
                System.IO.File.Delete(sourceFile);
            }
            ((BaseComputeTest)temp).WriteDataOut(((BaseComputeTest)temp).MyComputedObject, @"C:\Users\q0heccdm\Documents\HEC FDA\Compute Testing\Compute Test Results\Single Test_.txt", errMesArray);
            

            //if there is a zero and a two, get rid of the two

            if (temp.MyComputedObject.Functions[2].FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
            { temp.MyComputedObject.Functions.RemoveAt(2); }
            
            //FunctionsVisualizer fv = new FunctionsVisualizer(temp.MyComputedObject.Functions);       //myListOfBaseFunctions);
            //fv.Show();





        }

        private void btn_singleTests_Click(object sender, RoutedEventArgs e)
        {
            //SimpleTestCase.SimpleTest2();
            //writeToExcel(SimpleTestCase.SimpleTest());
            //SimpleWithLotsOfNumbers stc = new SimpleWithLotsOfNumbers();
            Compute_Testing.Tests.InflowFrequency_InflowOutflow_Rating_0_1_3_ temp = new Compute_Testing.Tests.InflowFrequency_InflowOutflow_Rating_0_1_3_();

            //get the errors
            string sourceFile = @"C:\Users\q0heccdm\AppData\Local\Temp\1\HEC\FdaModel_LogFile.txt";


            //for (int i = 0; i < 10; i++)
            //{
            //      Compute_Testing.Tests.AllFunctionsTest aft = new Compute_Testing.Tests.AllFunctionsTest();
            object[] errMesArray = new object[] { "this is a test" };
            if (System.IO.File.Exists(sourceFile))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(sourceFile);
                List<string> errMesList = new List<string>();
                while (sr.ReadLine() != null)
                {
                    errMesList.Add(sr.ReadLine());
                }
                errMesArray = errMesList.ToArray();

                //    string destFile = @"C:\Users\q0heccdm\Documents\HEC FDA\Compute Testing\Compute Error Messages\Error Messages_" + i + ".txt";
                //  System.IO.File.Copy(sourceFile, destFile, true);

                sr.Close();
                System.IO.File.Delete(sourceFile);
            }

                ((BaseComputeTest)temp).WriteDataOut(((BaseComputeTest)temp).MyComputedObject, @"C:\Users\q0heccdm\Documents\HEC FDA\Compute Testing\Compute Test Results\Single Test_.txt", errMesArray);


            //stc.WriteDataOut(stc.MyComputedObject, txt_path.Text);
        }

        private void btnGenerateTestResult_Click(object sender, RoutedEventArgs e)
        {
            ResultsGenerator TestResult = ResultsGenerator.GenerateResult();
            double meanAEP = TestResult.TestResult.AEP.GetMean;
            double meanEAD = TestResult.TestResult.EAD.GetMean;

            MessageBox.Show("The computed AEP is: " + meanAEP + ". The compute EAD is: " + meanEAD + ".");
        }



        //private void writeToExcel(FdaModel.ComputedObject testResult)
        //{

        //    string excelPath = txt_path.Text;

        //    if(System.IO.Path.GetExtension(excelPath) == "")
        //    {
        //        excelPath += ".xlsx";
        //    }

        //    System.Data.DataTable dt = new System.Data.DataTable("simple case");

        //    dt.TableName = "simple case results2";

        //    dt.Columns.Add("AEP", Type.GetType("System.Double"));
        //    dt.Columns.Add("Description", Type.GetType("System.String"));


        //    dt.Rows.Add(testResult.AnnualExceedanceProbabilities,"this is the aep");
        //    // dt.Rows.Add(Double.NaN, "this is the aep");
        //    string name= testResult.ComputedList[0].ToString();
        //    string name2 = testResult.ComputedList[1].ToString();
        //    string name3 = testResult.ComputableObject.Functions[0].ToString();
        //    string name4 = testResult.ComputableObject.Functions[1].ToString();
        //    string name5 = testResult.ComputableObject.ToString();

        //    DataBase_Reader.InMemoryReader imr = new DataBase_Reader.InMemoryReader(dt);

        //    DataBase_Reader.DataTableView dtv = imr.GetTableManager(dt.TableName);
        //    dtv.ExportToXlsx(excelPath);


        //    //a second data table
        //   dt = new System.Data.DataTable("more info");

        //    dt.TableName = "more info";

        //    dt.Columns.Add("stuff", Type.GetType("System.Double"));
        //    dt.Columns.Add("Description stuff", Type.GetType("System.String"));


        //    dt.Rows.Add(testResult.AnnualExceedanceProbabilities, "this is the stuff");

        //    imr = new DataBase_Reader.InMemoryReader(dt);
        //    dtv = imr.GetTableManager(dt.TableName);
        //    dtv.ExportToXlsx(excelPath);

        //    MessageBox.Show("done");

        //}


    }
}
