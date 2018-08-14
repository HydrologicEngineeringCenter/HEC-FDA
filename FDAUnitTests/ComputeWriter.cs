using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FDAUnitTests
{
    //[Author(q0heccdm, 5 / 18 / 2017 2:33:31 PM)]
    public static class ComputeWriter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 5/18/2017 2:33:31 PM
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        #endregion
        #region Voids
        public static void WriteRealization(FdaModel.ComputationPoint.Outputs.Realization realization,  string path, object[] errorMessages)
        {
            
            string excelPath = path;

            List<object[]> EXPORTDATA = new List<object[]>();

            List<string> columnNames = new List<string>();



            // ********  Inputs ************************
            List<object> inputFunctions = new List<object>();



            for (int k = 0; k < realization.Condition.Functions.Count; k++)
            {
                inputFunctions.Add(realization.Condition.Functions[k].FunctionType.ToString() + " (" + ((int)realization.Condition.Functions[k].FunctionType).ToString() + ")");
            }
            object[] inputs = inputFunctions.ToArray();
            EXPORTDATA.Add(inputs);
            columnNames.Add("Input Functions");

            // unsuccessfull compute ********************************************
            if (realization.Messages.IsValid == false)
            {
                List<object> composedFunctions = new List<object>();
                for (int k = 0; k < realization.Condition.Functions.Count(); k++)
                {
                    composedFunctions.Add(realization.Condition.Functions[k].FunctionType.ToString() + " (" + ((int)realization.Condition.Functions[k].FunctionType).ToString() + ")");
                }
                object[] output = composedFunctions.ToArray();
                EXPORTDATA.Add(output);
                columnNames.Add("Compute Functions");

                //get the threshold
                object[] threshhold = new object[] { realization.Condition.Threshold.ThresholdValue };

                EXPORTDATA.Add(threshhold);
                columnNames.Add("Threshold");

                //get the threshold type
                object[] threshholdType = new object[] { realization.Condition.Threshold.ThresholdType };
                EXPORTDATA.Add(threshholdType);
                columnNames.Add("Threshold Type");

                //get the computation point
                object[] compPoint = new object[] { realization.Condition.PerformanceComputePoint.ToString() + "(" + ((int)realization.Condition.PerformanceComputePoint).ToString() + ")" };
                EXPORTDATA.Add(compPoint);
                columnNames.Add("Comp Point");

                // print the function values
                for (int i = 0; i < realization.Condition.Functions.Count(); i++)
                {
                    switch (realization.Condition.Functions[i].FunctionType)
                    {
                        case FdaModel.Functions.FunctionTypes.InflowFrequency:// 0
                            //float[] x = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)AllComputableFunctions.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] y = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)AllComputableFunctions.Functions[i]).GetOrdinatesFunction().Ys;

                            ////inverse the probability values
                            //for (int j = 0; j < x.Count(); j++)
                            //{
                            //    x[j] = 1 - x[j];
                            //}

                            //List<object> inflowList = new List<object>();
                            //foreach (object obj in x)
                            //{
                            //    inflowList.Add(obj);
                            //}
                            //EXPORTDATA.Add(inflowList.ToArray());

                            ////exportData[numColumns] = inflowList.ToArray();
                            //columnNames.Add("(0) Inflow Probability");



                            //inflowList = new List<object>();
                            //foreach (object obj in y)
                            //{
                            //    inflowList.Add(obj);
                            //}
                            //EXPORTDATA.Add(inflowList.ToArray());

                            //// exportData[numColumns] = inflowList.ToArray();
                            //columnNames.Add("(0) Inflow Peak Discharge");



                            break;
                        case FdaModel.Functions.FunctionTypes.InflowOutflow: // 1

                            //throw new NotImplementedException();
                            double[] xsInflowOutflow = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.XValues.CopyTo(xsInflowOutflow, 0);
                            double[] ysInflowOutflow = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.YValues.CopyTo(ysInflowOutflow, 0);



                            List<object> newListInflowOutflow = new List<object>();
                            foreach (object obj in xsInflowOutflow)
                            {
                                newListInflowOutflow.Add(obj);
                            }

                            EXPORTDATA.Add(newListInflowOutflow.ToArray());
                            columnNames.Add("(1) Inflow Peak Discharge");

                            newListInflowOutflow = new List<object>();
                            foreach (object obj in ysInflowOutflow)
                            {
                                newListInflowOutflow.Add(obj);
                            }


                            EXPORTDATA.Add(newListInflowOutflow.ToArray());
                            columnNames.Add("(1) Outflow Peak Discharge");


                            break;

                        case FdaModel.Functions.FunctionTypes.OutflowFrequency: //2
                            double[] xsOutflowFreq;
                            double[] ysOutflowFreq;

                            if (realization.Condition.Functions[i].GetType().BaseType == typeof(FdaModel.Functions.FrequencyFunctions.FrequencyFunction))
                            {
                                xsOutflowFreq = new double[((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)realization.Condition.Functions[i]).GetOrdinatesFunction().Function.Count];
                                ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)realization.Condition.Functions[i]).GetOrdinatesFunction().Function.XValues.CopyTo(xsOutflowFreq, 0);
                                ysOutflowFreq = new double[((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)realization.Condition.Functions[i]).GetOrdinatesFunction().Function.Count];
                                ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)realization.Condition.Functions[i]).GetOrdinatesFunction().Function.YValues.CopyTo(ysOutflowFreq, 0);
                            }
                            else
                            {
                                //xsOutflowFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Xs;
                                //ysOutflowFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Ys;


                                xsOutflowFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                                ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.XValues.CopyTo(xsOutflowFreq, 0);
                                ysOutflowFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                                ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.YValues.CopyTo(ysOutflowFreq, 0);


                            }

                            List<object> newListOutflowFreq = new List<object>();
                            foreach (object obj in xsOutflowFreq)
                            {
                                newListOutflowFreq.Add(obj);
                            }

                            EXPORTDATA.Add(newListOutflowFreq.ToArray());
                            columnNames.Add("(2) Annual Exceedance Prob");

                            newListOutflowFreq = new List<object>();
                            foreach (object obj in ysOutflowFreq)
                            {
                                newListOutflowFreq.Add(obj);
                            }


                            EXPORTDATA.Add(newListOutflowFreq.ToArray());
                            columnNames.Add("(2) Outflow Peak Discharge");

                            break;

                        case FdaModel.Functions.FunctionTypes.Rating: //3
                            //float[] xs = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Xs;
                            //float[] ys = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Ys;
                            //string nameer = ((FdaModel.Functions.OrdinatesFunctions.IncreasingOrdinatesFunction)testResult.ComputedList[i]).ToString();
                            double[] xsRating = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.XValues.CopyTo(xsRating, 0);
                            double[] ysRating = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.YValues.CopyTo(ysRating, 0);



                            List<object> newList = new List<object>();
                            foreach (object obj in xsRating)
                            {
                                newList.Add(obj);
                            }

                            EXPORTDATA.Add(newList.ToArray());
                            //exportData[numColumns] = newList.ToArray();
                            columnNames.Add("(3) Outflow Peak Discharge");


                            newList = new List<object>();
                            foreach (object obj in ysRating)
                            {
                                newList.Add(obj);
                            }
                            EXPORTDATA.Add(newList.ToArray());
                            //exportData[numColumns] = newList.ToArray();
                            columnNames.Add("(3) Peak Exterior Stage");


                            break;

                        case FdaModel.Functions.FunctionTypes.ExteriorStageFrequency: //4
                            //double[] xsExteriorStageFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Xs;
                            //double[] ysExteriorStageFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Ys;

                            double[] xsExteriorStageFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.XValues.CopyTo(xsExteriorStageFreq, 0);
                            double[] ysExteriorStageFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.YValues.CopyTo(ysExteriorStageFreq, 0);



                            List<object> xsExteriorStageFreqList = new List<object>();
                            foreach (object obj in xsExteriorStageFreq)
                            {
                                xsExteriorStageFreqList.Add(obj);
                            }

                            EXPORTDATA.Add(xsExteriorStageFreqList.ToArray());
                            columnNames.Add("(4) Annual Exceedance Chance");

                            List<object> ysExteriorStageFreqList = new List<object>();
                            foreach (object obj in ysExteriorStageFreq)
                            {
                                ysExteriorStageFreqList.Add(obj);
                            }

                            EXPORTDATA.Add(ysExteriorStageFreqList.ToArray());
                            columnNames.Add("(4) Exterior Peak Stage");

                            break;

                        case FdaModel.Functions.FunctionTypes.ExteriorInteriorStage: //5
                            //float[] xsExteriorInteriorStage = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Xs;
                            //float[] ysExteriorInteriorStage = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Ys;

                            double[] xsExteriorInteriorStage = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.XValues.CopyTo(xsExteriorInteriorStage, 0);
                            double[] ysExteriorInteriorStage = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.YValues.CopyTo(ysExteriorInteriorStage, 0);


                            List<object> xsExteriorInteriorList = new List<object>();
                            foreach (object obj in xsExteriorInteriorStage)
                            {
                                xsExteriorInteriorList.Add(obj);
                            }

                            EXPORTDATA.Add(xsExteriorInteriorList.ToArray());
                            columnNames.Add("(5) Exterior Peak Stage");

                            List<object> ysExteriorInteriorList = new List<object>();
                            foreach (object obj in ysExteriorInteriorStage)
                            {
                                ysExteriorInteriorList.Add(obj);
                            }

                            EXPORTDATA.Add(ysExteriorInteriorList.ToArray());
                            columnNames.Add("(5) Interior Peak Stage");

                            break;

                        case FdaModel.Functions.FunctionTypes.InteriorStageFrequency: //6
                            //float[] xsInteriorStageFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Xs;
                            //float[] ysInteriorStageFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Ys;

                            double[] xsInteriorStageFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.XValues.CopyTo(xsInteriorStageFreq, 0);
                            double[] ysInteriorStageFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.YValues.CopyTo(ysInteriorStageFreq, 0);


                            List<object> xsInteriorStageFreqList = new List<object>();
                            foreach (object obj in xsInteriorStageFreq)
                            {
                                xsInteriorStageFreqList.Add(obj);
                            }

                            EXPORTDATA.Add(xsInteriorStageFreqList.ToArray());
                            columnNames.Add("(6) Annual Exceedance Prob");

                            List<object> ysInteriorStageFreqList = new List<object>();
                            foreach (object obj in ysInteriorStageFreq)
                            {
                                ysInteriorStageFreqList.Add(obj);
                            }

                            EXPORTDATA.Add(ysInteriorStageFreqList.ToArray());
                            columnNames.Add("(6) Interior Peak Stage");

                            break;

                        case FdaModel.Functions.FunctionTypes.InteriorStageDamage: //7
                            //float[] xsInteriorStageDamage = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Xs;
                            //float[] ysInteriorStageDamage = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Ys;

                            double[] xsInteriorStageDamage = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.XValues.CopyTo(xsInteriorStageDamage, 0);
                            double[] ysInteriorStageDamage = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.YValues.CopyTo(ysInteriorStageDamage, 0);


                            List<object> xsInteriorStageDamageList = new List<object>();
                            foreach (object obj in xsInteriorStageDamage)
                            {
                                xsInteriorStageDamageList.Add(obj);
                            }

                            EXPORTDATA.Add(xsInteriorStageDamageList.ToArray());
                            columnNames.Add("(7) Interior Peak Stage");

                            List<object> ysInteriorStageDamageList = new List<object>();
                            foreach (object obj in ysInteriorStageDamage)
                            {
                                ysInteriorStageDamageList.Add(obj);
                            }

                            EXPORTDATA.Add(ysInteriorStageDamageList.ToArray());
                            columnNames.Add("(7) Aggregated Damage");

                            break;

                        case FdaModel.Functions.FunctionTypes.DamageFrequency: //8
                            //float[] xsLeveeFailure = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Xs;
                            //float[] ysLeveeFailure = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Ys;

                            double[] xsLeveeFailure = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.XValues.CopyTo(xsLeveeFailure, 0);
                            double[] ysLeveeFailure = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.YValues.CopyTo(ysLeveeFailure, 0);



                            List<object> xsLeveeFailureList = new List<object>();
                            foreach (object obj in xsLeveeFailure)
                            {
                                xsLeveeFailureList.Add(obj);
                            }

                            EXPORTDATA.Add(xsLeveeFailureList.ToArray());
                            columnNames.Add("(8) Annual Exceedance Chance");

                            List<object> ysLeveeFailureList = new List<object>();
                            foreach (object obj in ysLeveeFailure)
                            {
                                ysLeveeFailureList.Add(obj);
                            }

                            EXPORTDATA.Add(ysLeveeFailureList.ToArray());
                            columnNames.Add("(8) Aggregated Damage");

                            break;

                        case FdaModel.Functions.FunctionTypes.LeveeFailure: //9
                            //float[] xsLeveeFailure1 = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Xs;
                            //float[] ysLeveeFailure1 = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)AllComputableFunctions.Functions[i]).Ys;

                            double[] xsLeveeFailure1 = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.XValues.CopyTo(xsLeveeFailure1, 0);
                            double[] ysLeveeFailure1 = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Condition.Functions[i]).Function.YValues.CopyTo(ysLeveeFailure1, 0);


                            List<object> xsLeveeFailureList1 = new List<object>();
                            foreach (object obj in xsLeveeFailure1)
                            {
                                xsLeveeFailureList1.Add(obj);
                            }

                            EXPORTDATA.Add(xsLeveeFailureList1.ToArray());
                            columnNames.Add("(8) Stage");

                            List<object> ysLeveeFailureList1 = new List<object>();
                            foreach (object obj in ysLeveeFailure1)
                            {
                                ysLeveeFailureList1.Add(obj);
                            }

                            EXPORTDATA.Add(ysLeveeFailureList1.ToArray());
                            columnNames.Add("(8) Chance of Failure");

                            break;

                        case FdaModel.Functions.FunctionTypes.UnUsed: //9


                            //EXPORTDATA.Add(xsLeveeFailureList1.ToArray());
                            columnNames.Add("(99) Unused");


                            break;

                        default:
                            break;
                    }

                }


            }



            //********************************************************************
            if (realization != null)
            {
                List<object> composedFunctions = new List<object>();
                for (int k = 0; k < realization.Functions.Count(); k++)
                {
                    composedFunctions.Add(realization.Functions[k].FunctionType.ToString() + " (" + ((int)realization.Functions[k].FunctionType).ToString() + ")");
                }
                object[] output = composedFunctions.ToArray();
                EXPORTDATA.Add(output);
                columnNames.Add("Compute Functions");

                //get the threshold
                object[] threshhold = new object[] { realization.Condition.Threshold.ThresholdValue };

                EXPORTDATA.Add(threshhold);
                columnNames.Add("Threshold value");

                //get the threshold type
                object[] threshholdType = new object[] { realization.Condition.Threshold.ThresholdType };
                EXPORTDATA.Add(threshholdType);
                columnNames.Add("Threshold Type");

                //get the computation point
                object[] compPoint = new object[] { realization.Condition.PerformanceComputePoint.ToString() + "(" + ((int)realization.Condition.PerformanceComputePoint).ToString() + ")" };
                EXPORTDATA.Add(compPoint);
                columnNames.Add("Comp Point");


                // print the function values
                for (int i = 0; i < realization.Functions.Count(); i++)
                {
                    switch (realization.Functions[i].FunctionType)
                    {
                        case FdaModel.Functions.FunctionTypes.InflowFrequency:// 0
                            //float[] x = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;//  AllComputableFunctions.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] y = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;

                            double[] x = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.XValues.CopyTo(x, 0);
                            double[] y = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.YValues.CopyTo(y, 0);


                            //inverse the probability values
                            for (int j = 0; j < x.Count(); j++)
                            {
                                x[j] = 1 - x[j];
                            }

                            List<object> inflowList = new List<object>();
                            foreach (object obj in x)
                            {
                                inflowList.Add(obj);
                            }
                            EXPORTDATA.Add(inflowList.ToArray());

                            //exportData[numColumns] = inflowList.ToArray();
                            columnNames.Add("(0) Inflow Probability");



                            inflowList = new List<object>();
                            foreach (object obj in y)
                            {
                                inflowList.Add(obj);
                            }
                            EXPORTDATA.Add(inflowList.ToArray());

                            // exportData[numColumns] = inflowList.ToArray();
                            columnNames.Add("(0) Inflow Peak Discharge");



                            break;
                        case FdaModel.Functions.FunctionTypes.InflowOutflow: // 1

                            //throw new NotImplementedException();
                            //float[] xsInflowOutflow = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] ysInflowOutflow = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;

                            double[] xsInflowOutflow = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.XValues.CopyTo(xsInflowOutflow, 0);
                            double[] ysInflowOutflow = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.YValues.CopyTo(ysInflowOutflow, 0);


                            List<object> newListInflowOutflow = new List<object>();
                            foreach (object obj in xsInflowOutflow)
                            {
                                newListInflowOutflow.Add(obj);
                            }

                            EXPORTDATA.Add(newListInflowOutflow.ToArray());
                            columnNames.Add("(1) Inflow Peak Discharge");

                            newListInflowOutflow = new List<object>();
                            foreach (object obj in ysInflowOutflow)
                            {
                                newListInflowOutflow.Add(obj);
                            }


                            EXPORTDATA.Add(newListInflowOutflow.ToArray());
                            columnNames.Add("(1) Outflow Peak Discharge");


                            break;

                        case FdaModel.Functions.FunctionTypes.OutflowFrequency: //2

                            //float[] xsOutflowFreq = testResult.Functions[i].GetOrdinatesFunction().Xs;
                            //float[] ysOutflowFreq = testResult.Functions[i].GetOrdinatesFunction().Ys;

                            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction ord = realization.Functions[i].GetOrdinatesFunction();



                            double[] xsOutflowFreq = new double[ord.Function.Count];
                            ord.Function.XValues.CopyTo(xsOutflowFreq, 0);
                            double[] ysOutflowFreq = new double[ord.Function.Count];
                            ord.Function.YValues.CopyTo(ysOutflowFreq, 0);


                            for (int j = 0; j < xsOutflowFreq.Count(); j++)
                            {
                                xsOutflowFreq[j] = 1 - xsOutflowFreq[j];
                            }

                            List<object> newListOutflowFreq = new List<object>();
                            foreach (object obj in xsOutflowFreq)
                            {
                                newListOutflowFreq.Add(obj);
                            }

                            EXPORTDATA.Add(newListOutflowFreq.ToArray());
                            columnNames.Add("(2) Annual Exceedance Prob");

                            newListOutflowFreq = new List<object>();
                            foreach (object obj in ysOutflowFreq)
                            {
                                newListOutflowFreq.Add(obj);
                            }


                            EXPORTDATA.Add(newListOutflowFreq.ToArray());
                            columnNames.Add("(2) Outflow Peak Discharge");

                            break;

                        case FdaModel.Functions.FunctionTypes.Rating: //3
                            //float[] xs = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] ys = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;
                            //string nameer = ((FdaModel.Functions.OrdinatesFunctions.IncreasingOrdinatesFunction)testResult.ComputedList[i]).ToString();
                            double[] xs = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.XValues.CopyTo(xs, 0);
                            double[] ys = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.YValues.CopyTo(ys, 0);



                            List<object> newList = new List<object>();
                            foreach (object obj in xs)
                            {
                                newList.Add(obj);
                            }

                            EXPORTDATA.Add(newList.ToArray());
                            //exportData[numColumns] = newList.ToArray();
                            columnNames.Add("(3) Outflow Peak Discharge");


                            newList = new List<object>();
                            foreach (object obj in ys)
                            {
                                newList.Add(obj);
                            }
                            EXPORTDATA.Add(newList.ToArray());
                            //exportData[numColumns] = newList.ToArray();
                            columnNames.Add("(3) Peak Exterior Stage");


                            break;

                        case FdaModel.Functions.FunctionTypes.ExteriorStageFrequency: //4
                            //float[] xsExteriorStageFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] ysExteriorStageFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;

                            double[] xsExteriorStageFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.XValues.CopyTo(xsExteriorStageFreq, 0);
                            double[] ysExteriorStageFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.YValues.CopyTo(ysExteriorStageFreq, 0);


                            for (int j = 0; j < xsExteriorStageFreq.Count(); j++)
                            {
                                xsExteriorStageFreq[j] = 1 - xsExteriorStageFreq[j];
                            }

                            List<object> xsExteriorStageFreqList = new List<object>();
                            foreach (object obj in xsExteriorStageFreq)
                            {
                                xsExteriorStageFreqList.Add(obj);
                            }

                            EXPORTDATA.Add(xsExteriorStageFreqList.ToArray());
                            columnNames.Add("(4) Annual Exceedance Chance");

                            List<object> ysExteriorStageFreqList = new List<object>();
                            foreach (object obj in ysExteriorStageFreq)
                            {
                                ysExteriorStageFreqList.Add(obj);
                            }

                            EXPORTDATA.Add(ysExteriorStageFreqList.ToArray());
                            columnNames.Add("(4) Exterior Peak Stage");

                            break;

                        case FdaModel.Functions.FunctionTypes.ExteriorInteriorStage: //5
                            //float[] xsExteriorInteriorStage = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] ysExteriorInteriorStage = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;

                            double[] xsExteriorInteriorStage = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.XValues.CopyTo(xsExteriorInteriorStage, 0);
                            double[] ysExteriorInteriorStage = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.YValues.CopyTo(ysExteriorInteriorStage, 0);


                            List<object> xsExteriorInteriorList = new List<object>();
                            foreach (object obj in xsExteriorInteriorStage)
                            {
                                xsExteriorInteriorList.Add(obj);
                            }

                            EXPORTDATA.Add(xsExteriorInteriorList.ToArray());
                            columnNames.Add("(5) Exterior Peak Stage");

                            List<object> ysExteriorInteriorList = new List<object>();
                            foreach (object obj in ysExteriorInteriorStage)
                            {
                                ysExteriorInteriorList.Add(obj);
                            }

                            EXPORTDATA.Add(ysExteriorInteriorList.ToArray());
                            columnNames.Add("(5) Interior Peak Stage");

                            break;

                        case FdaModel.Functions.FunctionTypes.InteriorStageFrequency: //6
                            //float[] xsInteriorStageFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] ysInteriorStageFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;

                            double[] xsInteriorStageFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.XValues.CopyTo(xsInteriorStageFreq, 0);
                            double[] ysInteriorStageFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.YValues.CopyTo(ysInteriorStageFreq, 0);



                            for (int j = 0; j < xsInteriorStageFreq.Count(); j++)
                            {
                                xsInteriorStageFreq[j] = 1 - xsInteriorStageFreq[j];
                            }

                            List<object> xsInteriorStageFreqList = new List<object>();
                            foreach (object obj in xsInteriorStageFreq)
                            {
                                xsInteriorStageFreqList.Add(obj);
                            }

                            EXPORTDATA.Add(xsInteriorStageFreqList.ToArray());
                            columnNames.Add("(6) Annual Exceedance Prob");

                            List<object> ysInteriorStageFreqList = new List<object>();
                            foreach (object obj in ysInteriorStageFreq)
                            {
                                ysInteriorStageFreqList.Add(obj);
                            }

                            EXPORTDATA.Add(ysInteriorStageFreqList.ToArray());
                            columnNames.Add("(6) Interior Peak Stage");

                            break;

                        case FdaModel.Functions.FunctionTypes.InteriorStageDamage: //7
                            //float[] xsInteriorStageDamage = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] ysInteriorStageDamage = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;

                            double[] xsInteriorStageDamage = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.XValues.CopyTo(xsInteriorStageDamage, 0);
                            double[] ysInteriorStageDamage = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.YValues.CopyTo(ysInteriorStageDamage, 0);



                            List<object> xsInteriorStageDamageList = new List<object>();
                            foreach (object obj in xsInteriorStageDamage)
                            {
                                xsInteriorStageDamageList.Add(obj);
                            }

                            EXPORTDATA.Add(xsInteriorStageDamageList.ToArray());
                            columnNames.Add("(7) Interior Peak Stage");

                            List<object> ysInteriorStageDamageList = new List<object>();
                            foreach (object obj in ysInteriorStageDamage)
                            {
                                ysInteriorStageDamageList.Add(obj);
                            }

                            EXPORTDATA.Add(ysInteriorStageDamageList.ToArray());
                            columnNames.Add("(7) Aggregated Damage");

                            break;

                        case FdaModel.Functions.FunctionTypes.DamageFrequency: //8
                            //float[] xsDamFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] ysDamFreq = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;

                            double[] xsDamFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.XValues.CopyTo(xsDamFreq, 0);
                            double[] ysDamFreq = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.YValues.CopyTo(ysDamFreq, 0);



                            for (int j = 0; j < xsDamFreq.Count(); j++)
                            {
                                xsDamFreq[j] = 1 - xsDamFreq[j];
                            }

                            List<object> xsLeveeFailureList = new List<object>();
                            foreach (object obj in xsDamFreq)
                            {
                                xsLeveeFailureList.Add(obj);
                            }

                            EXPORTDATA.Add(xsLeveeFailureList.ToArray());
                            columnNames.Add("(8) Annual Exceedance Chance");

                            List<object> ysLeveeFailureList = new List<object>();
                            foreach (object obj in ysDamFreq)
                            {
                                ysLeveeFailureList.Add(obj);
                            }

                            EXPORTDATA.Add(ysLeveeFailureList.ToArray());
                            columnNames.Add("(8) Aggregated Damage");

                            break;

                        case FdaModel.Functions.FunctionTypes.LeveeFailure: //9
                            //float[] xsLeveeFailure1 = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Xs;
                            //float[] ysLeveeFailure1 = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)testResult.Functions[i]).GetOrdinatesFunction().Ys;

                            double[] xsLeveeFailure1 = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.XValues.CopyTo(xsLeveeFailure1, 0);
                            double[] ysLeveeFailure1 = new double[((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.Count];
                            ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)realization.Functions[i]).Function.YValues.CopyTo(ysLeveeFailure1, 0);



                            List<object> xsLeveeFailureList1 = new List<object>();
                            foreach (object obj in xsLeveeFailure1)
                            {
                                xsLeveeFailureList1.Add(obj);
                            }

                            EXPORTDATA.Add(xsLeveeFailureList1.ToArray());
                            columnNames.Add("(9) Ext Stage");

                            List<object> ysLeveeFailureList1 = new List<object>();
                            foreach (object obj in ysLeveeFailure1)
                            {
                                ysLeveeFailureList1.Add(obj);
                            }

                            EXPORTDATA.Add(ysLeveeFailureList1.ToArray());
                            columnNames.Add("(9) Chance of Failure");

                            break;

                        case FdaModel.Functions.FunctionTypes.UnUsed: //99


                            //EXPORTDATA.Add(xsLeveeFailureList1.ToArray());
                            columnNames.Add("(99) Unused");


                            break;

                        default:
                            break;
                    }

                }




                //Get the AEP
                if (double.IsNaN(realization.AnnualExceedanceProbability))
                { }
                else
                {
                    object[] aepArray = new object[1] { 1 - realization.AnnualExceedanceProbability };
                    EXPORTDATA.Add(aepArray);
                    columnNames.Add("AEP");
                }

                if (realization.IsPerformanceOnlyCompute == false)
                {
                    object[] eadArray = new object[1] { realization.ExpectedAnnualDamage };
                    EXPORTDATA.Add(eadArray);
                    columnNames.Add("EAD");
                }


            }


            //error messages
            //object[] errorMesages = new object[1] { errorMessages};

            // EXPORTDATA.Add(errorMessages);
            // columnNames.Add("Error Messages");




            object[] condErr = new object[realization.Condition.Messages.Messages.Count];
            for (int i = 0; i < realization.Condition.Messages.Messages.Count(); i++)
            {
                condErr[i] = ((FdaModel.Utilities.Messager.ErrorMessage)realization.Condition.Messages.Messages[i]).Message;
            }
            EXPORTDATA.Add(condErr);
            columnNames.Add("Condition Errors");


            object[] realErr = new object[realization.Messages.Messages.Count];
            for (int i = 0; i < realization.Messages.Messages.Count(); i++)
            {
                realErr[i] = ((FdaModel.Utilities.Messager.ErrorMessage)realization.Messages.Messages[i]).Message;
            }

            EXPORTDATA.Add(realErr);
            columnNames.Add("Realization Errors");


            string[] colNames = columnNames.ToArray();
            object[][] exportData = EXPORTDATA.ToArray();

            FdaTester.ModelTester.Utilities.DataExporter.ExportDelimitedColumns(excelPath, exportData, colNames);
            //System.Windows.MessageBox.Show("Finished Writing");
        }
        #endregion
        #region Functions
        #endregion
    }
}
