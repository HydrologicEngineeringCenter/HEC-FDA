using CommunityToolkit.Mvvm.ComponentModel;

namespace VisualScratchSpace.ViewModel;
public partial class CheckableItem : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isChecked = true;

    [ObservableProperty]
    private string _value;
}
