using CommunityToolkit.Mvvm.ComponentModel;

namespace VisualScratchSpace.ViewModel;
public partial class CheckableItem : ObservableObject
{
    /// <summary>
    /// The name of the item displayed in the UI
    /// </summary>
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isChecked = true;

    /// <summary>
    /// The actual value of the checkable item which is included in a table name
    /// </summary>
    [ObservableProperty]
    private string _value;
}
