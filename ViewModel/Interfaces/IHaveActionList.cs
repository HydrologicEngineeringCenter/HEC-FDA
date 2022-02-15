using System.Collections.Generic;

namespace HEC.MVVMFramework.ViewModel.Interfaces
{
    public interface IHaveActionList
    {
        List<IDisplayableNamedAction> Actions { get; }
        void AddToActionList(IDisplayableNamedAction action);
    }
}
