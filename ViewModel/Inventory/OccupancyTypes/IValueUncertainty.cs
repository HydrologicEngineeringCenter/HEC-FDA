using HEC.FDA.ViewModel.Utilities;
using Statistics;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public interface IValueUncertainty
    {
        ContinuousDistribution CreateOrdinate();
        FdaValidationResult IsValid();
    }
}
