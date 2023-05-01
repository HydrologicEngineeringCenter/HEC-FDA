using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.Results
{
    public class SelectableChildElement : BaseViewModel
    {
        public event EventHandler SelectionChanged;

        private bool _IsSelected = true;
        private IASElement _Element;
        private string _Decoration;
        private string _Tooltip;
        private bool _IsEnabled = true;

        public string Decoration
        {
            get { return _Decoration; }
            set { _Decoration = value; NotifyPropertyChanged(); }
        }
        public IASElement Element
        {
            get { return _Element; }
            set { _Element = value; NotifyPropertyChanged(); }
        }

        public string Tooltip
        {
            get { return _Tooltip; }
            set { _Tooltip = value; NotifyPropertyChanged(); }
        }

        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; NotifyPropertyChanged(); }
        }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; SelectionChanged?.Invoke(this, new EventArgs()); NotifyPropertyChanged(); }
        }

        public SelectableChildElement(IASElement element)
        {
            Update(element);
        }

        public void Update(IASElement updatedElement)
        {
            Element = updatedElement;
            IASTooltipHelper.UpdateTooltip(this);
            if(updatedElement.Results == null)
            {
                IsEnabled = false;
                IsSelected = false;
            }
            else
            {
                IsEnabled = true;
            }
        }

    }
}
