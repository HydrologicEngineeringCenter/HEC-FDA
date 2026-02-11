using CommunityToolkit.Mvvm.ComponentModel;

namespace HEC.FDA.ViewModel.Utilities;
public partial class WeightedCheckableItem : CheckableItem
{
    [ObservableProperty]
    private double _weight;

    [ObservableProperty]
    private bool _isEnabled;
}
