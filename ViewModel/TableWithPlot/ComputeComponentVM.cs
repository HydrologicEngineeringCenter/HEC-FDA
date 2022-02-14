using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Xml.Linq;
using fda_viewmodel.Data;
using fda_viewmodel.Data.Base;
using fda_viewmodel.Data.Interfaces;
using paireddata;
using fda_viewmodel.Rows;

namespace fda_viewmodel
{
    public class ComputeComponentVM : ViewModel.Implementations.ValidatingBaseViewModel
    {
        #region Backing Fields
        private string _name;
        private string _xlabel = "xlabel";
        private string _ylabel = "ylabel";
        private string _description = "description";
        private IDataProvider _selectedItem;
        #endregion
        #region Properties
        public string Units
        {
            get
            {
                return ("(" + XLabel + ", " + YLabel + ")");
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
        public string XLabel
        {
            get
            {
                return _xlabel;
            }
            set
            {
                _xlabel = value;
                NotifyPropertyChanged();
            }
        }
        public string YLabel
        {
            get
            {
                return _ylabel;
            }
            set
            {
                _ylabel = value;
                NotifyPropertyChanged();
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<IDataProvider> Options { get; set; }
        public IDataProvider SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region Constructors
        public ComputeComponentVM()
        {
            Initialize();
            SetValidation();
        }
        public ComputeComponentVM(string name = "name", string xlabel = "xlabel", string ylabel = "ylabel", string description = "description")
        {
            _name = name;
            Initialize();
            SetValidation();
        }
        public ComputeComponentVM(XElement vmEle)
        {
            LoadFromXML(vmEle);
            SetValidation();
        }
        #endregion
        #region Methods
        private void Initialize()
        {
            Options = new ObservableCollection<IDataProvider>()
            {
                new DeterministicDataProvider(),
                new UniformDataProvider(),
                new NormalDataProvider(),
                new TriangularDataProvider(),
                new LogNormalDataProvider()
            };
            SelectedItem = Options.First();
            // This is just so our initial set of data we load is valid. Eventually we can crush this if condition. 
            if (Name == "Fragility Curve")
            {
                Options.First().RemoveRows(new List<int> { 1, 2, 3, 4, 5 });
                var ok = (DeterministicRow)(Options.First().Data[1]);
                ok.Value = 1;
            }
        }
        private void SetValidation()
        {
            foreach (IDataProvider opt in Options)
            {
                foreach (ViewModel.Implementations.ValidatingBaseViewModel row in opt.Data)
                {
                    row.Validate();
                }
            }
        }
        public UncertainPairedData SelectedItemToPairedData()
        {
            return SelectedItem.ToUncertainPairedData(XLabel, YLabel, Name, Description, "residential");
        }
        public virtual XElement ToXML()
        {
            XElement ele = new XElement(this.GetType().Name);
            ele.SetAttributeValue("selectedItem", SelectedItem.Name);
            ele.SetAttributeValue("Name", Name);
            foreach (IDataProvider idp in Options)
            {
                XElement child = new XElement(idp.ToUncertainPairedData(XLabel, YLabel, Name, Description, "residential").WriteToXML());
                child.SetAttributeValue("DistributionProviderType", idp.GetType().Name);
                ele.Add(child);
            }
            return ele;
        }
        public virtual void LoadFromXML(XElement element)
        {
            Options = new ObservableCollection<IDataProvider>();
            string selectedItemName = element.Attribute("selectedItem")?.Value;
            foreach (XElement updEl in element.Elements())
            {
                Name = updEl.Attribute("Name")?.Value;
                XLabel = updEl.Attribute("XLabel")?.Value;
                YLabel = updEl.Attribute("YLabel")?.Value;
                Description = updEl.Attribute("Description")?.Value;

                string assemblyName = "fda_viewmodel";//this libraries name and the appropriate namespace. "C:\Temp\FDA2.0_Internal\fda-viewmodel.dll"
                string typeName = assemblyName + ".Data." + updEl.Attribute("DistributionProviderType").Value;
                ObjectHandle oh = System.Activator.CreateInstance(null, typeName);//requires empty constructor
                BaseDataProvider dist = oh.Unwrap() as BaseDataProvider;

                UncertainPairedData upd = UncertainPairedData.ReadFromXML(updEl);
                dist.UpdateFromUncertainPairedData(upd);
                Options.Add(dist);
            }
            foreach (IDataProvider provider in Options)
            {
                if (provider.Name.Equals(selectedItemName))
                {
                    SelectedItem = provider;
                    break;
                }
            }
        }
        #endregion
    }
}
