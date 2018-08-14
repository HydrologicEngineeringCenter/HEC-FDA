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

namespace FdaTester.ModelTester.UnitTests.LogPearsonIIIConstructorTest
{
    /// <summary>
    /// Interaction logic for ConstructorTestingResults.xaml
    /// </summary>
    public partial class ConstructorTestingResults : Window
    {
        public ConstructorTestingResults()
        {
            InitializeComponent();
        }

        private void btn_Go_Click(object sender, RoutedEventArgs e)
        {
            lst_cons1.Items.Clear();
            lst_cons2.Items.Clear();

            double mean = Convert.ToDouble(txt_mean.Text);
            double stDev = Convert.ToDouble(txt_stDev.Text);
            double skew = Convert.ToDouble(txt_Skew.Text);
            int por = Convert.ToInt32(txt_POR.Text);

            Random randValue = new Random();
          
            Int32 numberOfTests = 1000;

            for (int j = 0; j < numberOfTests; j++)
            {
                int myPor = randValue.Next(1, 100);
                //create log3 with constructor 1
                FdaModel.FrequencyFunctions.LogPearsonIII cons1 = new FdaModel.FrequencyFunctions.LogPearsonIII(randValue.NextDouble()*8 +2, randValue.NextDouble()*2, randValue.NextDouble()*4-2, myPor);

                //create an array of flow frequencies from cons 1
                double[] cons1Array = new double[100];

                Random rand = new Random();

                for (int i = 0; i < 100; i++)
                {
                    cons1Array[i] = cons1.Function.getDistributedVariable(rand.NextDouble()); //next double between 0-1  
                }


                //create a log3 using constructor 2
                FdaModel.FrequencyFunctions.LogPearsonIII cons2 = new FdaModel.FrequencyFunctions.LogPearsonIII(cons1Array);

                double cons2_Mean = cons2.Function.GetMean;
                double cons2_stDev = cons2.Function.GetStDev;
                double cons2_skew = cons2.Function.GetG;
               

                //create a new log3 using constructor 1 and the data from cons 2
                cons1 = new FdaModel.FrequencyFunctions.LogPearsonIII(cons2_Mean, cons2_stDev, cons2_skew, por);

                double[] cons1Flows = new double[8];
                double[] cons2Flows = new double[8];
                string[] parameters = { "Mean: " + cons2_Mean.ToString(), "St Dev: " + cons2_stDev.ToString(), "Skew: " + cons2_skew.ToString(), "POR: " + myPor.ToString() };
                double[] probs = { .5, .2, .1, .04, .02, .01, .005, .002 };

                cons1Flows[0] = (cons1.Function.getDistributedVariable(1 - .5));
                cons1Flows[1] = (cons1.Function.getDistributedVariable(1 - .2));
                cons1Flows[2] = (cons1.Function.getDistributedVariable(1 - .1));
                cons1Flows[3] = (cons1.Function.getDistributedVariable(1 - .04));
                cons1Flows[4] = (cons1.Function.getDistributedVariable(1 - .02));
                cons1Flows[5] = (cons1.Function.getDistributedVariable(1 - .01));
                cons1Flows[6] = (cons1.Function.getDistributedVariable(1 - .005));
                cons1Flows[7] = (cons1.Function.getDistributedVariable(1 - .002));

                cons2Flows[0] = (cons2.Function.getDistributedVariable(1 - .5));
                cons2Flows[1] = (cons2.Function.getDistributedVariable(1 - .2));
                cons2Flows[2] = (cons2.Function.getDistributedVariable(1 - .1));
                cons2Flows[3] = (cons2.Function.getDistributedVariable(1 - .04));
                cons2Flows[4] = (cons2.Function.getDistributedVariable(1 - .02));
                cons2Flows[5] = (cons2.Function.getDistributedVariable(1 - .01));
                cons2Flows[6] = (cons2.Function.getDistributedVariable(1 - .005));
                cons2Flows[7] = (cons2.Function.getDistributedVariable(1 - .002));

                ModelTester.Utilities.DataExporter.ExportFlowData("C:\\Users\\q0heccdm\\Documents\\FDA2\\Testing Results\\testing_Constructor_1_Flows_vs_Constructor_2_Flows.txt", parameters, probs, cons1Flows, cons2Flows);

            }



            // 100 times then feed into cons 2.
            //then get mean get stdev...then plug into const 1 and test that the 8 probs are identical.I will
            //be able to ask it specifically the 8 probs.


            //for cons 3: the sampleFunction will call cons 3. make sure that everything looks right. check that the values match the values from cons 1 or 2

            //then i can test Frequency Function. test createOrdinatesFunctionFromInterval make sure that t



        }

        private void btn_test3_Click(object sender, RoutedEventArgs e)
        {
            lst_cons1.Items.Clear();
            lst_cons2.Items.Clear();

            Int32 numberOfTests = 1000;

            object[][] data = new object[numberOfTests][];



            double mean = Convert.ToDouble(txt_mean.Text);
            double stDev = Convert.ToDouble(txt_stDev.Text);
            double skew = Convert.ToDouble(txt_Skew.Text);
            int por = Convert.ToInt32(txt_POR.Text);

            Random rand = new Random();

            for (int i = 0; i < numberOfTests ; i++)
            {
                //create LP3 using constructor 1
                FdaModel.FrequencyFunctions.LogPearsonIII cons1 = new FdaModel.FrequencyFunctions.LogPearsonIII(mean, stDev, skew, por);


                //create a new LP3 using constructor 3
                FdaModel.FrequencyFunctions.LogPearsonIII cons3 = (FdaModel.FrequencyFunctions.LogPearsonIII)cons1.SampleFunction(rand.NextDouble(), rand.NextDouble());

                double cons3Mean = cons3.Function.GetMean;
                double cons3StdDev = cons3.Function.GetStDev;
                double cons3skew = cons3.Function.GetG;


                FdaModel.FrequencyFunctions.LogPearsonIII newCons1 = new FdaModel.FrequencyFunctions.LogPearsonIII(cons3Mean, cons3StdDev, cons3skew, por);

                double[] cons1Flows = new double[8];
                double[] cons3Flows = new double[8];
                string[] parameters = { "Mean: " + cons3Mean.ToString(), "St Dev: " + cons3StdDev.ToString(), "Skew: " + cons3skew.ToString(), "POR: " + por.ToString() };
                double[] probs = { .5, .2, .1, .04, .02, .01, .005, .002 };

                cons1Flows[0] = (newCons1.Function.getDistributedVariable(1 - .5));
                cons1Flows[1] = (newCons1.Function.getDistributedVariable(1 - .2));
                cons1Flows[2] = (newCons1.Function.getDistributedVariable(1 - .1));
                cons1Flows[3] = (newCons1.Function.getDistributedVariable(1 - .04));
                cons1Flows[4] = (newCons1.Function.getDistributedVariable(1 - .02));
                cons1Flows[5] = (newCons1.Function.getDistributedVariable(1 - .01));
                cons1Flows[6] = (newCons1.Function.getDistributedVariable(1 - .005));
                cons1Flows[7] = (newCons1.Function.getDistributedVariable(1 - .002));

                cons3Flows[0] = (cons3.Function.getDistributedVariable(1 - .5));
                cons3Flows[1] = (cons3.Function.getDistributedVariable(1 - .2));
                cons3Flows[2] = (cons3.Function.getDistributedVariable(1 - .1));
                cons3Flows[3] = (cons3.Function.getDistributedVariable(1 - .04));
                cons3Flows[4] = (cons3.Function.getDistributedVariable(1 - .02));
                cons3Flows[5] = (cons3.Function.getDistributedVariable(1 - .01));
                cons3Flows[6] = (cons3.Function.getDistributedVariable(1 - .005));
                cons3Flows[7] = (cons3.Function.getDistributedVariable(1 - .002));




               

                ModelTester.Utilities.DataExporter.ExportFlowData("C:\\Users\\q0heccdm\\Documents\\FDA2\\Testing Results\\testingConstructorFlows.txt", parameters, probs, cons1Flows,cons3Flows);


            }
            //string[] columnNames = { "flow1", "flow2" };
            //ModelTester.Utilities.DataExporter.ExportDelimitedColumns("C:\\Users\\q0heccdm\\Documents\\FDA2\\Testing Results\\testingConstructorFlows.txt", data,columnNames);

            //lst_cons1.Items.Add(newCons1.function.getDistributedVariable(1-.5));
            //lst_cons1.Items.Add(cons1.function.getDistributedVariable(1 - .25));

            //lst_cons2.Items.Add(cons3.function.getDistributedVariable(1-.5));



            //cons1Flows[0] = (newCons1.function.getDistributedVariable(1 - .5));
            //lst_cons1.Items.Add(newCons1.function.getDistributedVariable(1 - .2).ToString());
            //lst_cons1.Items.Add(newCons1.function.getDistributedVariable(1 - .1).ToString());
            //lst_cons1.Items.Add(newCons1.function.getDistributedVariable(1 - .04).ToString());
            //lst_cons1.Items.Add(newCons1.function.getDistributedVariable(1 - .02).ToString());
            //lst_cons1.Items.Add(newCons1.function.getDistributedVariable(1 - .01).ToString());
            //lst_cons1.Items.Add(newCons1.function.getDistributedVariable(1 - .005).ToString());
            //lst_cons1.Items.Add(newCons1.function.getDistributedVariable(1 - .002).ToString());

            //lst_cons2.Items.Add(cons3.function.getDistributedVariable(1 - .5).ToString());
            //lst_cons2.Items.Add(cons3.function.getDistributedVariable(1 - .2).ToString());
            //lst_cons2.Items.Add(cons3.function.getDistributedVariable(1 - .1).ToString());
            //lst_cons2.Items.Add(cons3.function.getDistributedVariable(1 - .04).ToString());
            //lst_cons2.Items.Add(cons3.function.getDistributedVariable(1 - .02).ToString());
            //lst_cons2.Items.Add(cons3.function.getDistributedVariable(1 - .01).ToString());
            //lst_cons2.Items.Add(cons3.function.getDistributedVariable(1 - .005).ToString());
            //lst_cons2.Items.Add(cons3.function.getDistributedVariable(1 - .002).ToString());


        }
    }
}
