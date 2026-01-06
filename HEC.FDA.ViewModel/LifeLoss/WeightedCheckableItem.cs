using CommunityToolkit.Mvvm.ComponentModel;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.LifeLoss;
public partial class WeightedCheckableItem : CheckableItem
{
    [ObservableProperty]
    private double _weight;
}
