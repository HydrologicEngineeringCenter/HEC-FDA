using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results;
public class UncertaintyControlConfigs
{
    /// <summary>
    /// Defines the properties which the "Consequence with Uncertainty" controls must have, unique to each consequence type
    /// </summary>
    public interface IUncertaintyControlConfig
    {
        string PlotTitle { get; }
        string YAxisTitle { get; }
        string MeanFormat { get; }
        string YAxisFormat { get; }
        string TrackerFormat { get; }
        ConsequenceType ConsequenceType { get; }
        IQuartileRowItem CreateRowItem(string frequency, double value, string riskType = null);
    }

    public class DamageWithUncertaintyControlConfig : IUncertaintyControlConfig
    {
        public string PlotTitle { get; }
        public string YAxisTitle { get; }
        public string MeanFormat { get; }
        public string YAxisFormat { get; }
        public string TrackerFormat { get; }
        public ConsequenceType ConsequenceType { get; }

        public DamageWithUncertaintyControlConfig()
        {
            PlotTitle = StringConstants.EAD_DISTRIBUTION;
            YAxisTitle = StringConstants.EXPECTED_ANNUAL_DAMAGE;
            MeanFormat = "C2";
            YAxisFormat = "C0";
            TrackerFormat = "X: {Probability:0.####}, Y: {Value:C0}";
            ConsequenceType = ConsequenceType.Damage;
        }

        public IQuartileRowItem CreateRowItem(string frequency, double value, string riskType = null)
        {
            return new EadRowItem(frequency, value, riskType);
        }
    }

    public class EqADWithUncertaintyControlConfig : IUncertaintyControlConfig
    {
        public string PlotTitle { get; }
        public string YAxisTitle { get; }
        public string MeanFormat { get; }
        public string YAxisFormat { get; }
        public string TrackerFormat { get; }
        public ConsequenceType ConsequenceType { get; }

        public EqADWithUncertaintyControlConfig()
        {
            PlotTitle = StringConstants.EqAD_DISTRIBUTION;
            YAxisTitle = StringConstants.EQUIVALENT_ANNUAL_DAMAGE;
            MeanFormat = "C2";
            YAxisFormat = "C0";
            TrackerFormat = "X: {Probability:0.####}, Y: {Value:C0}";
            ConsequenceType = ConsequenceType.Damage;
        }

        public IQuartileRowItem CreateRowItem(string frequency, double value, string riskType = null)
        {
            return new EqadRowItem(frequency, value, riskType);
        }
    }

    public class LifeLossWithUncertaintyControlConfig : IUncertaintyControlConfig
    {
        public string PlotTitle { get; }
        public string YAxisTitle { get; }
        public string MeanFormat { get; }
        public string YAxisFormat { get; }
        public string TrackerFormat { get; }
        public ConsequenceType ConsequenceType { get; }

        public LifeLossWithUncertaintyControlConfig()
        {
            PlotTitle = "AALL Distribution";
            YAxisTitle = "Average Annual Life Loss";
            MeanFormat = "N4";
            YAxisFormat = "N4";
            TrackerFormat = "X: {Probability:0.####}, Y: {Value:N4}";
            ConsequenceType = ConsequenceType.LifeLoss;
        }

        public IQuartileRowItem CreateRowItem(string frequency, double value, string riskType = null)
        {
            return new LifeLossRowItem(frequency, value, riskType);
        }
    }

    public class DamageReducedWithUncertaintyControlConfig : IUncertaintyControlConfig
    {
        public string PlotTitle { get; }
        public string YAxisTitle { get; }
        public string MeanFormat { get; }
        public string YAxisFormat { get; }
        public string TrackerFormat { get; }
        public ConsequenceType ConsequenceType { get; }

        public DamageReducedWithUncertaintyControlConfig()
        {
            PlotTitle = StringConstants.DAMAGE_REDUCED;
            YAxisTitle = StringConstants.EXPECTED_ANNUAL_DAMAGE;
            MeanFormat = "C2";
            YAxisFormat = "C0";
            TrackerFormat = "X: {Probability:0.####}, Y: {Value:C0}";
            ConsequenceType = ConsequenceType.Damage;
        }

        public IQuartileRowItem CreateRowItem(string frequency, double value, string riskType = null)
        {
            return new EadRowItem(frequency, value, riskType);
        }
    }

    public class EqADReducedWithUncertaintyControlConfig : IUncertaintyControlConfig
    {
        public string PlotTitle { get; }
        public string YAxisTitle { get; }
        public string MeanFormat { get; }
        public string YAxisFormat { get; }
        public string TrackerFormat { get; }
        public ConsequenceType ConsequenceType { get; }

        public EqADReducedWithUncertaintyControlConfig()
        {
            PlotTitle = StringConstants.DAMAGE_REDUCED;
            YAxisTitle = StringConstants.EQUIVALENT_ANNUAL_DAMAGE;
            MeanFormat = "C2";
            YAxisFormat = "C0";
            TrackerFormat = "X: {Probability:0.####}, Y: {Value:C0}";
            ConsequenceType = ConsequenceType.Damage;
        }

        public IQuartileRowItem CreateRowItem(string frequency, double value, string riskType = null)
        {
            return new EqadRowItem(frequency, value, riskType);
        }
    }

    public class LifeLossReducedWithUncertaintyControlConfig : IUncertaintyControlConfig
    {
        public string PlotTitle { get; }
        public string YAxisTitle { get; }
        public string MeanFormat { get; }
        public string YAxisFormat { get; }
        public string TrackerFormat { get; }
        public ConsequenceType ConsequenceType { get; }

        public LifeLossReducedWithUncertaintyControlConfig()
        {
            PlotTitle = "EALL Reduced Distribution";
            YAxisTitle = "Expected Annual Life Loss";
            MeanFormat = "N2";
            YAxisFormat = "N0";
            TrackerFormat = "X: {Probability:0.####}, Y: {Value:N2}";
            ConsequenceType = ConsequenceType.LifeLoss;
        }

        public IQuartileRowItem CreateRowItem(string frequency, double value, string riskType = null)
        {
            return new LifeLossRowItem(frequency, value, riskType);
        }
    }
}
