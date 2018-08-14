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

namespace FdaTester.ModelTester.UnitTests.Functions.Composition
{

    public partial class CompositionTestingView : Window
    {
        public CompositionTestingView()
        {
            InitializeComponent();
        }

        private void btnFrequencyFunctionComposition_Click(object sender, RoutedEventArgs e)
        {
            // 1. Option a: Generate a frequency function with random statistics.
            //FrequencyFunctionGenerator logPearsonIII = FrequencyFunctionGenerator.LogPearsonGenerator();
            //FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction ordinatesLPIII = logPearsonIII.LPIIIFunction.GetOrdinatesFunction();

            // 1. Option b: Generate a frequency function with specific statistics.
            FdaModel.Functions.FrequencyFunctions.LogPearsonIII logPearsonIII = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(2.5d, 0.56d, -1.61, 200);
            logPearsonIII.FunctionType = FdaModel.Functions.FunctionTypes.OutflowFrequency;
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction ordinatesLPIII = logPearsonIII.GetOrdinatesFunction();

            // 2. Generate ordinates function
            OrdinatesFunctionGenerator stageDischargeFunction = OrdinatesFunctionGenerator.FromFrequencyFunctionOrdinatesGenerator(logPearsonIII, FdaModel.Functions.FunctionTypes.Rating);

            //3. Compose function new stage frequency function
            List<FdaModel.Utilities.Messager.ErrorMessage> errors = new List<FdaModel.Utilities.Messager.ErrorMessage>();
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction stageFrequencyFunction = logPearsonIII.Compose(stageDischargeFunction.OrdinatesFunction, ref errors);

            // 4. Write out data.

            object[] ordLP3Xs = ordinatesLPIII.Function.XValues.Cast<object>().ToArray();
            object[] ordLP3Ys = ordinatesLPIII.Function.YValues.Cast<object>().ToArray();
            object[] stageDisXs = stageDischargeFunction.OrdinatesFunction.Function.XValues.Cast<object>().ToArray();
            object[] stageDisYs = stageDischargeFunction.OrdinatesFunction.Function.YValues.Cast<object>().ToArray();
            object[] stageFreqXs = stageFrequencyFunction.Function.XValues.Cast<object>().ToArray();
            object[] stageFreqYs = stageFrequencyFunction.Function.YValues.Cast<object>().ToArray();


            object[][] exportData = new object[6][] { ordLP3Xs,ordLP3Ys, stageDisXs, stageDisYs, stageFreqXs, stageFreqYs };
            Utilities.DataExporter.ExportDelimitedColumns("C:\\Temp\\test.txt", exportData, new string[6] { "P(FLOW > Flow)", "Flow" , "Flow" , "Stage" , "P(STAGE > Stage)", "Stage" });
        }

        private void btnOrdinatesFunctionComposition_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
