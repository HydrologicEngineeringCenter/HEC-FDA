using Functions;
using System;
using System.Collections.Generic;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal class FrequencyFunction<YType> : ImpactAreaFunctionBase<YType>, IFrequencyFunction<YType>
    {
        public override string XLabel => throw new NotImplementedException();

        public override string YLabel => throw new NotImplementedException();

        internal FrequencyFunction(ICoordinatesFunction<double, YType> function) : base(function, ImpactAreaFunctionEnum.DamageFrequency)
        {
        }

        // #region Notes
        // /* Entity Notes
        // 1. If Entity cannot be trained to deal with IFunction (or I choose not use it because of this) this would inherit BaseFunction: public sealed FrequencyFunction: BaseFunction
        // */
        // #endregion

        // #region Fields
        //// private bool _IsValid;
        // //private Statistics.LogPearsonIII _Function;
        // #endregion

        // #region Properties   
        // public bool IsValid
        // {
        //     get
        //     {
        //         return Validate();
        //     }
        //     private set
        //     {
        //         _IsValid = value;
        //     }
        // }
        // public Statistics.LogPearsonIII Function
        // {
        //     get
        //     {
        //         return _Function;
        //     }
        //     private set
        //     {
        //         _Function = value;
        //         _IsValid = Validate();
        //     }
        // }
        // #endregion

        // #region Constructors
        // internal FrequencyFunction(Statistics.LogPearsonIII function)
        // {
        //     Function = function;
        //     if (IsValid == false) ReportValidationErrors();
        // }
        // #endregion

        // #region IFunctionBase Methods
        // public bool ValidateFrequencyValues()
        // {
        //     return true;
        // }
        // public IFunctionBase Sample(double randomNumber)
        // {
        //     return new FrequencyFunction(new Statistics.LogPearsonIII(Function.CreateAnalyticalBootstrap(Convert.ToInt16(randomNumber))));
        // }
        // public IList<Tuple<double, double>> GetOrdinates()
        // {
        //     throw new NotImplementedException();
        // }
        // public IList<Tuple<double, double>> Compose(IList<Tuple<double, double>> transformOrdinates)
        // {
        //     List<Tuple<double, double>> newOrdinates = new List<Tuple<double, double>>();

        //     double p = FindFirstFrequencyValue(transformOrdinates), P = 0.9999;
        //     if (p >= P) return new List<Tuple<double, double>>() { new Tuple<double, double>(0.0001, double.NaN), new Tuple<double, double>(0.9999, double.NaN) };
        //     else { newOrdinates.Add(new Tuple<double, double>(p, transformOrdinates[0].Item2)); p = IncrementProbabilityValue(p); }

        //     int j = FindFirstTransformIndex(transformOrdinates), J = transformOrdinates.Count - 1;
        //     if (j == J) return new List<Tuple<double, double>>() { new Tuple<double, double>(0.0001, double.NaN), new Tuple<double, double>(0.9999, double.NaN) };

        //     double incrY = (transformOrdinates[J].Item2 - transformOrdinates[j].Item2) / 100, nextY =  transformOrdinates[j].Item2 + (incrY * (j + 1));
        //     double matchValue = Function.getDistributedVariable(p);

        //     do
        //     {
        //         newOrdinates.Add(new Tuple<double, double>(p, transformOrdinates[j].Item2 + (matchValue - transformOrdinates[j].Item1) / (transformOrdinates[j + 1].Item1 - transformOrdinates[j].Item1) * (transformOrdinates[j + 1].Item2 - transformOrdinates[j].Item2)));

        //         if (newOrdinates[newOrdinates.Count - 1].Item2 == nextY) nextY += incrY;
        //         if (newOrdinates[newOrdinates.Count - 1].Item2 == transformOrdinates[j + 1].Item2) j++;
        //         if (newOrdinates[newOrdinates.Count - 1].Item1 == p) p = IncrementProbabilityValue(p);

        //         if (j == J)
        //         {
        //             newOrdinates.Add(new Tuple<double, double>(p, transformOrdinates[J].Item2));
        //             break;
        //         }
        //         else
        //         {
        //             matchValue = Math.Min(Function.getDistributedVariable(p),
        //                          Math.Min(transformOrdinates[j].Item1 + (nextY - transformOrdinates[j].Item2) / (transformOrdinates[j + 1].Item2 - transformOrdinates[j].Item2) * (transformOrdinates[j + 1].Item1 - transformOrdinates[j].Item1),
        //                          transformOrdinates[j + 1].Item1));
        //         }
        //     } while (p < P);
        //     if (p >= P && j < J) newOrdinates.Add(new Tuple<double, double>(p, transformOrdinates[j].Item2 + (matchValue - transformOrdinates[j].Item1) / (transformOrdinates[j + 1].Item1 - transformOrdinates[j].Item1) * (transformOrdinates[j + 1].Item2 - transformOrdinates[j].Item2))); 
        //     return newOrdinates;
        // }
        // private double FindFirstFrequencyValue(IList<Tuple<double, double>> transformOrdinates)
        // {            
        //     double p = 0.0001, lastP = p;
        //     while (Function.getDistributedVariable(p) < transformOrdinates[0].Item1)
        //     {
        //         if (p >= 0.9999) { lastP = p; break; }
        //         else { lastP = p; p = IncrementProbabilityValue(p); }
        //     }
        //     return lastP;
        // }
        // private double IncrementProbabilityValue(double p)
        // {
        //     if (p == 0) return 0.0001; 
        //     if (p < 0.01) return 0.01;
        //     if (p > 0.99) return 0.9999;
        //     else return p + 0.01;
        // }
        // private int FindFirstTransformIndex(IList<Tuple<double, double>> transformOrdinates)
        // {
        //     double minMatchValue = Function.getDistributedVariable(0.0001);

        //     int j = 0, J = transformOrdinates.Count - 1;
        //     while (transformOrdinates[j].Item1 < minMatchValue)
        //     {
        //         if (j == J) break;
        //         else j++;
        //     }
        //     if (!(j == 0 || j == J)) j = j - 1;
        //     return j;
        // }
        // public double GetXfromY(double y)
        // {
        //     return Function.GetCDF(y);
        // }
        // public double GetYfromX(double x)
        // {
        //     return Function.getDistributedVariable(x);
        // }
        // public double TrapezoidalRiemannSum()
        // {
        //     double riemannSum = 0;
        //     IList<Tuple<double, double>> ordinates = GetOrdinates();
        //     for (int i = 0; i < ordinates.Count - 1; i++)
        //     {
        //         riemannSum += (ordinates[i + 1].Item2 + ordinates[i].Item2) * (ordinates[i + 1].Item1 - ordinates[i].Item1) / 2;
        //     }
        //     return riemannSum;
        // }

        // #endregion

        // #region IValidateData Methods
        // public bool Validate()
        // {
        //     if (Double.IsNaN(Function.getDistributedVariable(0.9999)) ||
        //         Double.IsNaN(Function.getDistributedVariable(0.0001)) ||
        //         Function.getDistributedVariable(0.998) > 30000000 || // Larger than Amazon Flood Flow
        //         Function.getDistributedVariable(0.001) < 0) return false;
        //     else return true;
        // }
        // public IEnumerable<string> ReportValidationErrors()
        // {
        //     List<string> messages = new List<string>();
        //     if (Double.IsNaN(Function.getDistributedVariable(0.9999))) { IsValid = false; messages.Add("The provided Log Pearson III parameters produce flows for the 0.001 (1,000 year average annual return period) annual chance exceedance event that can not be computed (e.g. return a double.NAN result). This value is invalid. If you believe the Log Pearon III parameters for this function are valid please contact HEC."); }
        //     if (Double.IsNaN(Function.getDistributedVariable(0.0001))) { IsValid = false; messages.Add("The provided Log Pearson III parameters produce flows for the 0.999 (1 year average annual return period) annual chance exceedance event that can not be computed (e.g. return a double.NAN result). This value is invalid. If you believe the Log Pearon III parameters for this function are valid please contact HEC."); }
        //     if (Function.getDistributedVariable(0.998) > 30000000) { IsValid = false; messages.Add("The provided Log Pearson III parameters produce flows for the 0.002 (500 year average annual return period) annual chance exceedance event that exceed 30,000,000 million cubic feet per second). This value is invalid."); }
        //     if (Function.getDistributedVariable(0.001) < 0) { IsValid = false; messages.Add("The provided Log Pearson III parameters produce flows for the 0.999 (1 year average annual return period) annual chance exceedance event that are invalid (e.g. not positive). This value is invalid."); }
        //     if (messages.Count == 0) { IsValid = true; messages.Add("The Log Pearson function is valid."); }
        //     return messages;
        // }
        // #endregion

        // #region Discarded Compose Helper Functions
        // private int IncrementTransformOrdinates(List<Tuple<double, double>> newOrdinates, List<Tuple<double, double>> transformOrdinates, int j)
        // {
        //     int J = transformOrdinates.Count - 1;
        //     while (newOrdinates[newOrdinates.Count - 1].Item2 > transformOrdinates[j + 1].Item2)
        //     {
        //         if (j == J) break;
        //         else j++;
        //     }
        //     return j;
        // }
        // private double FindNextMatchValue(double nextFrequencyMatch, double nextTransformMatch)
        // {
        //     if (nextFrequencyMatch < nextTransformMatch) return nextFrequencyMatch;
        //     else return nextTransformMatch;
        // }
        // private double FindNextTransformMatchValue(List<Tuple<double, double>> transformOrdinates, double nextY, int j)
        // {
        //     if (j == transformOrdinates.Count - 1) return transformOrdinates[j].Item1;
        //     if (transformOrdinates[j + 1].Item2 < nextY) return transformOrdinates[j + 1].Item1;
        //     else return transformOrdinates[j].Item1 + (nextY - transformOrdinates[j].Item2) / (transformOrdinates[j + 1].Item2 - transformOrdinates[j].Item2) * (transformOrdinates[j + 1].Item1 - transformOrdinates[j].Item1);
        // }
        // #endregion
        public IComputableFrequencyFunction Sample(double p)
        {
            throw new NotImplementedException();
        }
    }
}
