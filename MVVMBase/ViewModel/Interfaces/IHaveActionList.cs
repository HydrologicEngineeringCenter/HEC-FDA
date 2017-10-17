using System.Collections.Generic;

namespace ViewModel.Interfaces
{
    public interface IHaveActionList
    {
        List<IDisplayableNamedAction> Actions { get;}
        void AddToActionList(IDisplayableNamedAction action);
    }
}
