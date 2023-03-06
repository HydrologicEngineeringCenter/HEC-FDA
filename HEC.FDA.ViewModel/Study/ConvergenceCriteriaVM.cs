using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Study
{
    public class ConvergenceCriteriaVM:BaseViewModel
    {
        private double _confidence = DefaultData.ConvergenceConfidence;
        private double _tolerance = DefaultData.ConvergenceTolerance;
        private int _min = DefaultData.ConvergenceMinIterations;
        private int _max = DefaultData.ConvergenceMaxIterations;
        public double Confidence
        {
            get { return _confidence; }
            set { _confidence = value;  NotifyPropertyChanged();  }
        }
        public double Tolerance
        {
            get {  return _tolerance; }
            set{  _tolerance = value; NotifyPropertyChanged();}
        }
        public int Min
        {
            get  {   return _min;  }
            set {  _min = value;  NotifyPropertyChanged(); NotifyPropertyChanged(nameof(Max));  }
        }
        public int Max
        {
            get  { return _max; }
            set { _max = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(Min)); }
        }
        public ConvergenceCriteriaVM(XElement ele)
        {
            FromXML(ele);
            Initialize();
        }
        public ConvergenceCriteriaVM()
        {
            Initialize();
        }
        private void Initialize()
        {
            AddRule(nameof(Min), () => Min > 100, "Min iterations must be more than 100.");
            AddRule(nameof(Max), () => Max > Min, "Min iterations must be less than Max iterations.");
            AddRule(nameof(Min), () => Min < Max, "Min iterations must be less than Max iterations.");
            AddRule(nameof(Max), () => Max < double.MaxValue, "Max iterations must be less than the maximum value for a double.");
            AddRule(nameof(Confidence), () => Confidence < 100, "Confidence must be less than 100.");
            AddRule(nameof(Confidence), () => Confidence > 50, "Confidence must be greater than 50.");
            AddRule(nameof(Tolerance), () => Tolerance >= .001, "Tolerance must be greater than .001.");
            AddRule(nameof(Tolerance), () => Tolerance <= 1, "Tolerance must be less than 1.");

            Validate();
        }
        public XElement ToXML()
        {
            XElement ele = new XElement("ConvergenceCriteriaVM");
            ele.SetAttributeValue(nameof(Min), Min);
            ele.SetAttributeValue(nameof(Max), Max);
            ele.SetAttributeValue(nameof(Confidence), Confidence);
            ele.SetAttributeValue(nameof(Tolerance), Tolerance);
            return ele;
        }
        public void FromXML(XElement ele)
        {
            Max = int.Parse(ele.Attribute(nameof(Max)).Value);
            Min = int.Parse(ele.Attribute(nameof(Min)).Value);
            Tolerance = double.Parse(ele.Attribute(nameof(Tolerance)).Value);
            Confidence = double.Parse(ele.Attribute(nameof(Confidence)).Value);
        }
        public ConvergenceCriteria ToConvergenceCriteria()
        {
            Statistics.Distributions.Normal sn = new Statistics.Distributions.Normal(0, 1);
            double zAlpha = sn.InverseCDF(.5 + Confidence / 200);
            return new ConvergenceCriteria(Min, Max, zAlpha, Tolerance);
        }
    }
}
