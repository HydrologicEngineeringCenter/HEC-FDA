using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Model.Inputs.Functions
{
    internal sealed class OrdinatesFunction: IFunctionBase
    {
        #region Fields
        private Statistics.CurveIncreasing _Function;
        #endregion

        #region Properties     
        public bool IsValid { get; } = false;
        public Statistics.CurveIncreasing Function { get; }
        public IList<Tuple<double, double>> Ordinates { get; }
        #endregion

        #region Constructors
        internal OrdinatesFunction(Statistics.CurveIncreasing function)
        {
            Function = function;
            IsValid = Validate();
            Ordinates = GetOrdinates();
        }
        internal OrdinatesFunction(double[] xs, double[] ys, bool strictOnX, bool strictOnY)
        {
            Function = new Statistics.CurveIncreasing(xs, ys, strictOnX, strictOnY);
            IsValid = Validate();
            Ordinates = GetOrdinates();
        }
        #endregion

        #region IFunctionBase Methods
        public bool ValidateFrequencyValues()
        {
           if (Ordinates[0].Item1 <= 0 ||
               Ordinates[0].Item1 >= 1 ||
               Ordinates[Function.Count - 1].Item1 <= 0 ||
               Ordinates[Function.Count - 1].Item1 >= 1) return false;
           else return true;
        }
        /// <summary>
        /// Since the Y ordinate values are expressed as point estimates, instead of distribution functions, this method returns the intance of ordinates function to which the method refers.
        /// </summary>
        /// <param name="probability"> All input values return the same result. A default value of double.NaN is provided. </param>
        /// <returns> The current instance of the ordinates function. </returns>
        public IFunctionBase Sample(double probability = double.NaN)
        {
            return this;
        }
        public IList<Tuple<double, double>> GetOrdinates()
        {
            return Function.XValues.Zip(Function.YValues, (x, y) => new Tuple<double, double>(x, y)).ToList();
        }
        public IList<Tuple<double, double>> Compose(IList<Tuple<double, double>> transformOrdinates)
        {
            List<Tuple<double, double>> newOrdinates;
            int i = FirstFrequencyIndex(transformOrdinates, out newOrdinates), I = Ordinates.Count - 1;
            if (i == I) return newOrdinates = Ordinates.Select((x, y) => new Tuple<double, double>(x.Item1, double.NaN)).ToList();

            int j = FirstTransformIndex(transformOrdinates), J = transformOrdinates.Count - 1;
            if (j == J) return newOrdinates = Ordinates.Select((x, y) => new Tuple<double, double>(x.Item1, double.NaN)).ToList();
            
            while (i < I + 1)
            {
                newOrdinates.Add(new Tuple<double, double>(Ordinates[i].Item1, transformOrdinates[j].Item2 + (Ordinates[i].Item2 - transformOrdinates[j].Item1) / (transformOrdinates[j + 1].Item1 - transformOrdinates[j].Item1) * (transformOrdinates[j + 1].Item2 - transformOrdinates[j].Item2)));
                while (IsTransformPointInBetweenFrequencyOrdinates(transformOrdinates, i, j) == true)
                {
                    if (j + 1 > J)
                    {
                        AddPointsAboveTransform(newOrdinates, transformOrdinates[J].Item2, ref i);
                        break;
                    }
                    else
                    {
                        j++;
                        newOrdinates.Add(new Tuple<double, double>(Ordinates[i].Item1 + (transformOrdinates[j].Item1 - Ordinates[i].Item2) / (Ordinates[i + 1].Item2 - Ordinates[i].Item2) * (Ordinates[i + 1].Item1 - Ordinates[i].Item1), transformOrdinates[j].Item2));
                    }
                }
                if (IncrementTransformOrdinates(transformOrdinates, i, j) == true) j++;
                if (j == J) AddPointsAboveTransform(newOrdinates, transformOrdinates[J].Item2, ref i);
                i++;  
            }
            return newOrdinates;
        }
        private int FirstFrequencyIndex(IList<Tuple<double, double>> transformOrdinates, out List<Tuple<double, double>> newOrdinates)
        {
            int i = 0, I = Ordinates.Count - 1;
            newOrdinates = new List<Tuple<double, double>>();
            while (Ordinates[i].Item2 < transformOrdinates[0].Item1)
            {
                newOrdinates.Add(new Tuple<double, double>(Ordinates[i].Item1, transformOrdinates[0].Item2));
                if (i == I) break;
                else i++;
            }
            return i;
        }
        private int FirstTransformIndex(IList<Tuple<double, double>> transformOrdinates)
        {
            int j = 0, J = transformOrdinates.Count - 1;
            while (transformOrdinates[j].Item1 < Ordinates[0].Item2)
            {
                if (j == J) break;
                else j++;
            }
            if (!(j == 0 || j == J)) j = j - 1;
            return j;
        }
        private bool IsTransformPointInBetweenFrequencyOrdinates(IList<Tuple<double, double>> transformOrdinates, int i, int j)
        {
            if (i < Ordinates.Count - 1 && j < transformOrdinates.Count - 1 &&
                transformOrdinates[j + 1].Item1 > Ordinates[i].Item2 &&
                transformOrdinates[j + 1].Item1 < Ordinates[i + 1].Item2) return true;
            else return false;
        }
        private bool IncrementTransformOrdinates(IList<Tuple<double,double>> transformOrdinates, int i, int j)
        {
            if (i < Ordinates.Count - 1 && j < transformOrdinates.Count - 1 &&  //changed from j + 1 => j and from Ordinates => transformOrdinates
                transformOrdinates[j + 1].Item1 < Ordinates[i + 1].Item2) return true;
            else return false;
        }
        private IList<Tuple<double, double>> AddPointsAboveTransform(IList<Tuple<double, double>> newOrdinates, double maxTransformValue, ref int i)
        {
            while (i < Ordinates.Count - 1)
            {
                i++;
                newOrdinates.Add(new Tuple<double, double>(Ordinates[i].Item1, maxTransformValue));
            }
            return newOrdinates;
        }
        public string ReportCompositionMessages(ITransformFunction transform)
        {
            StringBuilder compositionMessages = new StringBuilder();
            return compositionMessages.ToString();
        }
        public double GetXfromY(double y)
        {
            if (Ordinates[0].Item2 >= y) return Ordinates[0].Item1;
            if (Ordinates[Ordinates.Count - 1].Item2 <= y) return Ordinates[Ordinates.Count - 1].Item1;

            int i = 0;
            while (Ordinates[i + 1].Item2 < y)
            {
                i++;
            }
            return Ordinates[i].Item1 + (y - Ordinates[i].Item2) / (Ordinates[i + 1].Item2 - Ordinates[i].Item2) * (Ordinates[i + 1].Item1 - Ordinates[i].Item1);
        }
        public double GetYfromX(double x)
        {
            if (Ordinates[0].Item1 >= x) return Ordinates[0].Item2;
            if (Ordinates[Ordinates.Count - 1].Item1 <= x) return Ordinates[Ordinates.Count - 1].Item2;

            int i = 0;
            while (Ordinates[i + 1].Item1 < x)
            {
                i++;
            }
            return Ordinates[i].Item2 + (x - Ordinates[i].Item1) / (Ordinates[i + 1].Item1 - Ordinates[i].Item1) * (Ordinates[i + 1].Item2 - Ordinates[i].Item2);
        }
        public double TrapezoidalRiemannSum()
        {
            double riemannSum = 0;
            for (int i = 0; i < Function.Count - 1; i++)
            {
                riemannSum += (Function.get_Y(i + 1) + Function.get_Y(i)) * (Function.get_X(i + 1) - Function.get_X(i)) / 2;
            }
            return riemannSum;
        }
        
        //private IList<Tuple<double, double>> Aggregate(IList<Tuple<double, double>> addOrdinates)
        //{
        //    int i = 0, I = Ordinates.Count - 1, j = 0, J = addOrdinates.Count - 1;
        //    IList<Tuple<double, double>> aggregatedOrdinates = new List<Tuple<double, double>>();
        //    if (Ordinates[0].Item1 < addOrdinates[0].Item1) i = IndexBelowOverlap(Ordinates, addOrdinates[0].Item1, out aggregatedOrdinates);     
        //    if (Ordinates[0].Item1 > addOrdinates[0].Item1) j = IndexBelowOverlap(addOrdinates, Ordinates[0].Item1, out aggregatedOrdinates);
        //    //if (i == I || j == J) return aggregatedOrdinates;

        //    int n = 0, N = (I - i) + (J - j);
        //    while (n < N + 1)
        //    {
        //        if (Ordinates[i].Item1 < addOrdinates[j].Item1) aggregatedOrdinates.Add(Ordinates[i].Item1, Ordinates[i].Item2 + ())

        //    }
        //    while (i < I + 1)
        //    {
                
        //    }

        //}
        //private IFunctionBase Aggregate(IFunctionBase functionToAggregate)
        //{
        //    IList<Tuple<double, double> addOrdinates
        //    IFunctionBase lowerOrdinates, higherOrdinates;
        //    if (Ordinates[0].Item1 < addOrdinates.GetOrdinates().Ordinates[0].Item1) { lowerOrdinates = Ordinates; higherOrdinates = addOrdinates; }
        //    else { lowerOrdinates = addOrdinates; higherOrdinates = Ordinates; }

        //    int i = 0, j = 0, I = lowerOrdinates.Count, J = higherOrdinates.Count;
        //    while (lowerOrdinates[i].Item1 < higherOrdinates[j].Item1)
        //    {
        //        double newStage = lowerOrdinates[i].Item1;
                
        //    }
        //}


        private int IndexBelowOverlap(IList<Tuple<double, double>> lowerOrdinates, double firstSharedX, out IList<Tuple<double, double>> newOrdinates)
        {
            int n = 0, N = lowerOrdinates.Count - 1;
            newOrdinates = new List<Tuple<double, double>>();
            while (lowerOrdinates[n].Item1 < firstSharedX)
            {
                newOrdinates.Add(lowerOrdinates[n]);
                if (n == N) break;
                else n++;
            }
            return n;
        }
        
        #endregion

        #region IValidateData Methods
        public bool Validate()
        {
            if (Function.Count < 2) { ReportValidationErrors(); return false; }
            for (int i = 0; i < Function.Count - 1; i++)
            {
                if (Function.get_X(i + 1) == Function.get_X(i))
                {
                    Function.RemoveAt(i);
                    if (Function.Count < 2) { ReportValidationErrors(); return false; }
                    else i--;
                }
            }
            if (Function.IsValid == false) { ReportValidationErrors(); return false; }
            return true;
        }
        public IEnumerable<string> ReportValidationErrors()
        {
            int i = 0;
            List<string> errors = Function.GetErrors();
            while (i < Function.Count - 1)
            {
                if (Function.get_X(i) == Function.get_X(i + 1))
                {
                    Function.RemoveAt(i + 1);
                    errors.Add(string.Format("A duplicate ordinate containing the points ({0}, {1}) was removed.", Function.get_X(i), Function.get_Y(i)));
                }
                i++;
            }
            if (Function.Count == 1) errors.Add(string.Format("Only one unique data pair ({0}, {1}) was found. A function requires more than one unique set of ordinates.", Function.get_X(0), Function.get_Y(0)));
            return errors;
        }
        #endregion
    }
}
