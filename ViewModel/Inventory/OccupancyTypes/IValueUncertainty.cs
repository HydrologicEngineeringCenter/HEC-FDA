using Statistics;

namespace ViewModel.Inventory.OccupancyTypes
{
    public interface IValueUncertainty
    {
        IDistribution CreateOrdinate();
    }
}
