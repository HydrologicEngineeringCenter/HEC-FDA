using Statistics;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public interface IValueUncertainty
    {
        ContinuousDistribution CreateOrdinate();
    }
}
