using HEC.FDA.Statistics.Distributions;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public interface IValueUncertainty
    {
        ContinuousDistribution CreateOrdinate();
        FdaValidationResult IsValid();
    }
}
