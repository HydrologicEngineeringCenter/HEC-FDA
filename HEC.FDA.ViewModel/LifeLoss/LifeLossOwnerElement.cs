using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.LifeLoss;
public class LifeLossOwnerElement : ParentElement
{
    public LifeLossOwnerElement(): base()
    {
        Name = "Life Loss";
        CustomTreeViewHeader = new CustomHeaderVM(Name);
        NamedAction add = new()
        {
            Header = "Import LifeSim Database",
            Action = null
        };
        List<NamedAction> localActions = new()
        {
            add
        };
        Actions = localActions;
    }
}
