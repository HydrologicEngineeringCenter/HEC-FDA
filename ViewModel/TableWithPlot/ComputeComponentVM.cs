using HEC.FDA.ViewModel.TableWithPlot.Data;
using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;
using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using HEC.MVVMFramework.ViewModel.Implementations;
using paireddata;
using Statistics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.TableWithPlot
{
    public class ComputeComponentVM : BaseViewModel
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
        public ObservableCollection<IDataProvider> Options { get; } = new ObservableCollection<IDataProvider>();
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
        public ComputeComponentVM(string name = "name", string xlabel = "xlabel", string ylabel = "ylabel")
        {
            Name = name;
            XLabel = xlabel;
            YLabel = ylabel;
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

        public ComputeComponentVM Clone()
        {
            XElement thisData = ToXML();
            return new ComputeComponentVM(thisData);
        }

        /// <summary>
        /// Use the default ctor and then call this to assign a specified uncertain paired data
        /// </summary>
        /// <param name="upd"></param>
        public void SetPairedData(UncertainPairedData upd)
        {
            if (upd.Yvals.Length > 0)
            {
                IDistributionEnum distType = upd.Yvals[0].Type;
                switch(distType)
                {
                    case IDistributionEnum.Deterministic:
                        Options[0] = new DeterministicDataProvider(upd);
                        SelectedItem = Options[0];
                        break;
                    case IDistributionEnum.Uniform:
                        Options[1] = new UniformDataProvider(upd);
                        SelectedItem = Options[1];
                        break;
                    case IDistributionEnum.Normal:
                        Options[2] = new NormalDataProvider(upd);
                        SelectedItem = Options[2];
                        break;
                    case IDistributionEnum.Triangular:
                        Options[3] = new TriangularDataProvider(upd);
                        SelectedItem = Options[3];
                        break;
                    case IDistributionEnum.LogNormal:
                        Options[4] = new LogNormalDataProvider(upd);
                        SelectedItem = Options[4];
                        break;
                }
            }

        }

        private void Initialize()
        {
            Options.Add( new DeterministicDataProvider());
            Options.Add(new UniformDataProvider());
            Options.Add(new NormalDataProvider());
            Options.Add(new TriangularDataProvider());
            Options.Add(new LogNormalDataProvider());

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
                foreach (ValidatingBaseViewModel row in opt.Data)
                {
                    row.Validate();
                }
            }
        }
        public UncertainPairedData SelectedItemToPairedData(string damCat = "")
        {
            return SelectedItem.ToUncertainPairedData(XLabel, YLabel, Name, Description, damCat);
        }
        public virtual XElement ToXML()
        {
            XElement ele = new XElement(this.GetType().Name);
            ele.SetAttributeValue("selectedItem", SelectedItem.Name);
            ele.SetAttributeValue("Name", Name);
            foreach (IDataProvider idp in Options)
            {
                XElement child = new XElement(idp.ToUncertainPairedData(XLabel, YLabel, Name, Description, "").WriteToXML());
                child.SetAttributeValue("DistributionProviderType", idp.GetType().Name);
                ele.Add(child);
            }
            return ele;
        }
        public virtual void LoadFromXML(XElement element)
        {
            string selectedItemName = element.Attribute("selectedItem")?.Value;
            foreach (XElement updEl in element.Elements())
            {
                Name = updEl.Attribute("Name")?.Value;
                XLabel = updEl.Attribute("XLabel")?.Value;
                YLabel = updEl.Attribute("YLabel")?.Value;
                Description = updEl.Attribute("Description")?.Value;

                string assemblyName = "HEC.FDA.ViewModel";//this libraries name and the appropriate namespace. "C:\Temp\FDA2.0_Internal\fda-viewmodel.dll"
                string typeName = assemblyName + ".TableWithPlot.Data." + updEl.Attribute("DistributionProviderType").Value;
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
