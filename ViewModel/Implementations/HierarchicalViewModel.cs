using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ViewModel.Events;
using ViewModel.Interfaces;

namespace ViewModel.Implementations
{
    public class HierarchicalViewModel : ValidatingBaseViewModel, IHierarchicalViewModel
    {
        private List<IDisplayableNamedAction> _Actions = null;
        private ObservableCollection<IHierarchicalViewModel> _Children;
        private IHierarchicalViewModel _Parent;
        private bool _Expanded = false;
        private bool _IsEnabled = true;
        private bool _IsVisible = true;
        private string _Name;
        public List<IDisplayableNamedAction> Actions
        {
            get
            {
                return _Actions;
            }
        }
        public ObservableCollection<IHierarchicalViewModel> Children
        {
            get
            {
                return _Children;
            }
        }
        public bool Expanded
        {
            get
            {
                return _Expanded;
            }

            set
            {
                _Expanded = value;  NotifyPropertyChanged();
            }
        }
        public IHierarchicalViewModel Parent
        {
            get
            {
                return _Parent;
            }

            set
            {
                _Parent = value; NotifyPropertyChanged();
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }

            set
            {
                _IsEnabled = value; NotifyPropertyChanged();
            }
        }
        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }

            set
            {
                _IsVisible = value; NotifyPropertyChanged();
            }
        }
        public void AddToActionList(IDisplayableNamedAction action)
        {
            if (_Actions == null) { _Actions = new List<IDisplayableNamedAction>(); }
            _Actions.Add(action);
        }
        public void AddChild(IHierarchicalViewModel child, bool allowDuplicateNames = false)
        {
            if (_Children == null) { _Children = new ObservableCollection<IHierarchicalViewModel>(); }
            if (!allowDuplicateNames)
            {
                if (CheckForNameConflict(child))
                {
                    //silently fix the name?
                    int i = 1;
                    string prevname = child.Name;
                    do
                    {
                        child.Name = prevname + "_" + i;
                        i++;
                    } while (CheckForNameConflict(child));
                }
            }
            //check interfaces for events to add handlers for.
            if (child is INavigate)
            {
                INavigate ele = child as INavigate;
                ele.NavigationEvent += Navigate;
            }
            if(child is IReportMessage)
            {
                IReportMessage ele = child as IReportMessage;
                MessageHub.Register(ele);
            }
            //if (child.GetType().GetInterfaces().Contains(typeof(ICanClose)))
            //{
            //    ICanClose ele = child as ICanClose;
            //    ele.Close += RequestClose;
            //}
            
            _Children.Add(child);
            child.Parent = this;
            NotifyPropertyChanged(nameof(Children));
        }

        public void RemoveChild(IHierarchicalViewModel child)
        {
            //check interfaces for events to remove handlers for.
            if (child is INavigate)
            {
                INavigate ele = child as INavigate;
                ele.NavigationEvent -= Navigate;
            }
            //if (child is ICanClose)
            //{
            //    ICanClose ele = child as ICanClose;
            //    ele.Close -= RequestClose;
            //}
            _Children.Remove(child);//what if child doesnt exist?

            NotifyPropertyChanged(nameof(Children));
        }
        private bool CheckForNameConflict(IHierarchicalViewModel candidateChild)
        {
            foreach (IHierarchicalViewModel c in Children)
            {
                if (c.Name.Equals(candidateChild.Name)) return true;
            }
            return false;
        }
        public virtual List<T> GetChildrenOfType<T>() where T : HierarchicalViewModel
        {
            List<T> ret = new List<T>();
            if (_Children != null && _Children.Count != 0)
            {
                foreach (HierarchicalViewModel ele in _Children)
                {
                    T castedEle = ele as T;
                    if (!(castedEle == null))
                    {
                        ret.Add(castedEle);
                    }
                }
            }
            return ret;
        }
        public virtual T GetChildOfTypeAndName<T>(string name) where T : HierarchicalViewModel
        {
            if (_Children != null && _Children.Count != 0)
            {
                foreach (HierarchicalViewModel ele in _Children)
                {
                    T castedEle = ele as T;
                    if (!(castedEle == null))
                    {
                        if (castedEle.Name == name) return castedEle;
                    }
                }
            }
            return null;
        }
        public virtual List<T> GetRelativesOfType<T>() where T : HierarchicalViewModel
        {
            List<T> ret = new List<T>();
            IHierarchicalViewModel root = this; //what if this.Parent == null?
            do
            {
                root = root.Parent;
            } while (root.Parent != null);

            ret = root.GetDescendantsOfType<T>();
            return ret;
        }
        public virtual List<T> GetDescendantsOfType<T>() where T : HierarchicalViewModel
        {
            List<T> ret = new List<T>();
            foreach (IHierarchicalViewModel c in Children)
            {
                T castedEle = c as T;
                if (castedEle != null)
                {
                    ret.Add(castedEle);
                    ret.AddRange(castedEle.GetDescendantsOfType<T>());
                }
                else
                {
                    ret.AddRange(c.GetDescendantsOfType<T>());
                }
            }
            return ret;
        }
        public virtual T GetDescendantOfTypeAndName<T>(string name) where T : HierarchicalViewModel
        {
            IHierarchicalViewModel ret;
            foreach (IHierarchicalViewModel c in Children)
            {
                T castedEle = c as T;
                if (castedEle != null)
                {
                    if (castedEle.Name == name)
                    {
                        return castedEle;
                    }
                    else
                    {
                        ret = castedEle.GetRelativeOfTypeAndName<T>(name);
                        if (ret != null) return ret as T;
                    }
                }
                else
                {
                    ret = c.GetDescendantOfTypeAndName<T>(name);
                    if (ret != null) return ret as T;
                }
            }
            return null;
        }
        public virtual T GetRelativeOfTypeAndName<T>(string name) where T : HierarchicalViewModel
        {
            List<T> ret = new List<T>();
            IHierarchicalViewModel root = this; //what if this.Parent == null?
            do
            {
                root = root.Parent;
            } while (root.Parent != null);

            return root.GetDescendantOfTypeAndName<T>(name);
        }
    }
}
