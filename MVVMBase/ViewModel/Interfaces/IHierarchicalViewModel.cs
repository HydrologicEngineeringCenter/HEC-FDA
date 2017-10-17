using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Interfaces
{
    public interface IHierarchicalViewModel: IBaseViewModel, IExpandable, IHaveActionList, INavigate, IDisplayToUI
    {
        IHierarchicalViewModel Parent { get; set; }
        System.Collections.ObjectModel.ObservableCollection<IHierarchicalViewModel> Children { get; }
        List<T> GetChildrenOfType<T>() where T : HierarchicalViewModel;
        T GetChildOfTypeAndName<T>(string name) where T : HierarchicalViewModel;
        List<T> GetRelativesOfType<T>() where T : HierarchicalViewModel;
        T GetRelativeOfTypeAndName<T>(string name) where T : HierarchicalViewModel;
        List<T> GetDescendantsOfType<T>() where T : HierarchicalViewModel;
        T GetDescendantOfTypeAndName<T>(string name) where T : HierarchicalViewModel;
        void AddChild(IHierarchicalViewModel child, bool allowDuplicateNames = false);
        void RemoveChild(IHierarchicalViewModel child);
    }
}
