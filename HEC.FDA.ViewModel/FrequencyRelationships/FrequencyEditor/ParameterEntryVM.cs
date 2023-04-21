using HEC.FDA.ViewModel.TableWithPlot;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.ViewModel.Implementations;
using Statistics.Distributions;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    public class ParameterEntryVM: CurveComponentVM
    {
        private LogPearson3 _lP3Distriution;

        public LogPearson3 LP3Distribution
        {
            get { return _lP3Distriution; }
            set { _lP3Distriution = value; }
        }

        public double Mean
        {
            get
            {
                return LP3Distribution.Mean;
            }

            set
            {
                LP3Distribution.Mean = value;
                NotifyPropertyChanged();
            }
        }
        public double Standard_Deviation
        {
            get
            {
                return LP3Distribution.StandardDeviation;
            }
            set
            {
                LP3Distribution.StandardDeviation = value;
                NotifyPropertyChanged();
            }
        }
        public double Skew
        {
            get
            {
                return LP3Distribution.Skewness;
            }
            set
            {
                LP3Distribution.Skewness = value;
                NotifyPropertyChanged();
            }
        }
        public int SampleSize
        {
            get
            {
                return (int)LP3Distribution.SampleSize;
            }
            set
            {
                LP3Distribution = new LogPearson3(Mean, Standard_Deviation, Skew, value);
                NotifyPropertyChanged();
            }
        }
        public ParameterEntryVM(XElement xElement)
        {
            LoadFromXML(xElement);
            Initialize();
        }
        public ParameterEntryVM()
        {
            LP3Distribution = new LogPearson3(3, 1, 1, 40);
            Initialize();
        }
        public void LoadFromXML(XElement ele)
        {
            var childs = ele.Descendants();
            var distEle = childs.First();
            LP3Distribution = (LogPearson3)Statistics.ContinuousDistribution.FromXML(distEle);
        }
        public XElement ToXML()
        {
            XElement ele = new XElement(GetType().Name);
            ele.Add(LP3Distribution.ToXML());
            return ele;
        }
        public void Initialize()
        {
            Validate();
        }
    }
}
