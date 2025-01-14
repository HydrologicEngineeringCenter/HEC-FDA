using HEC.FDA.ViewModel.TableWithPlot.Base;
using Statistics.Distributions;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    public class ParameterEntryVM : BaseLP3TableWithPlot
    {
        #region Properties
        public double Mean
        {
            get => LP3Distribution.Mean;
            set
            {
                LP3Distribution.Mean = value;
                NotifyPropertyChanged();
                UpdatePlot();
            }
        }
        public double Standard_Deviation
        {
            get => LP3Distribution.StandardDeviation;
            set
            {
                LP3Distribution.StandardDeviation = value;
                NotifyPropertyChanged();
                UpdatePlot();
            }
        }
        public double Skew
        {
            get => LP3Distribution.Skewness;
            set
            {
                LP3Distribution.Skewness = value;
                NotifyPropertyChanged();
                UpdatePlot();
            }
        }
        public int SampleSize
        {
            get => (int)LP3Distribution.SampleSize;
            set
            {
                LP3Distribution.SampleSize = value;
                NotifyPropertyChanged();
                UpdatePlot();
            }
        }
        #endregion

        #region Constructors
        public ParameterEntryVM(XElement xElement)
        {
            FromXML(xElement);
            Validate();
        }
        public ParameterEntryVM()
        {
            LP3Distribution = new LogPearson3(3.5, 0.22, 0.1, 60);
            Validate();
        }
        #endregion



    }
}
