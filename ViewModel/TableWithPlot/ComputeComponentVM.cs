using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.TableWithPlot.Data;
using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;
using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Implementations;
using Statistics;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.TableWithPlot
{
    public class CurveComponentVM : BaseViewModel
    {
        #region Backing Fields
        private string _name;
        private string _xlabel = "xlabel";
        private string _ylabel = "ylabel";
        private string _description = "description";
        private IDataProvider _selectedItem;
        private bool _IsStrictMonotonic;
        private DistributionOptions _DistributionOptions;
        private bool _IsDepthPercentDamage;
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
                NotifyPropertyChanged(nameof(Units));
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
                NotifyPropertyChanged(nameof(Units));
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
        public CurveComponentVM(string name = "default_name", string xlabel = "default_xlabel", string ylabel = "default_ylabel", DistributionOptions distOptions = DistributionOptions.DEFAULT, bool isStrictMonotonic = false, bool isDepthPercentDamage=false)
        {
            _DistributionOptions = distOptions;
            _IsStrictMonotonic = isStrictMonotonic;
            Name = name;
            XLabel = xlabel;
            YLabel = ylabel;
            _IsDepthPercentDamage = isDepthPercentDamage;
            Initialize();
            SetValidation();
        }

        public CurveComponentVM(XElement vmEle)
        {
            LoadFromXML(vmEle);
            SetValidation();
        }
        #endregion
        #region Methods

        public CurveComponentVM Clone()
        {
            XElement thisData = ToXML();
            return new CurveComponentVM(thisData);
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

                switch (_DistributionOptions)
                {
                    case DistributionOptions.DEFAULT:
                        switch (distType)
                        {
                            case IDistributionEnum.Deterministic:
                                Options[0] = new DeterministicDataProvider(upd, _IsStrictMonotonic);
                                SelectedItem = Options[0];
                                break;
                            case IDistributionEnum.Uniform:
                                Options[1] = new UniformDataProvider(upd, _IsStrictMonotonic);
                                SelectedItem = Options[1];
                                break;
                            case IDistributionEnum.Normal:
                                Options[2] = new NormalDataProvider(upd, _IsStrictMonotonic);
                                SelectedItem = Options[2];
                                break;
                            case IDistributionEnum.Triangular:
                                Options[3] = new TriangularDataProvider(upd, _IsStrictMonotonic);
                                SelectedItem = Options[3];
                                break;
                            case IDistributionEnum.LogNormal:
                                Options[4] = new LogNormalDataProvider(upd, _IsStrictMonotonic);
                                SelectedItem = Options[4];
                                break;
                        }
                        break;
                    case DistributionOptions.DETERMINISTIC_ONLY:
                        if(distType == IDistributionEnum.Deterministic)
                        {
                            Options[0] = new DeterministicDataProvider(upd, _IsStrictMonotonic);
                            SelectedItem = Options[0];
                        }
                        else
                        {
                            //todo: throw exception?
                        }
                        break;
                    case DistributionOptions.HISTOGRAM_ONLY:
                        Options[0] = new HistogramDataProvider(upd, _IsStrictMonotonic);
                        //todo: add here

                        break;

                }
              
            }

        }

        private void Initialize()
        {
            switch(_DistributionOptions)
            {
                case DistributionOptions.DEFAULT:
                    if (_IsDepthPercentDamage)
                {
                    Options.Add(new DeterministicDataProvider(DefaultData.DepthPercentDamageDefaultCurve(IDistributionEnum.Deterministic), _IsStrictMonotonic));
                    Options.Add(new UniformDataProvider(DefaultData.DepthPercentDamageDefaultCurve(IDistributionEnum.Uniform), _IsStrictMonotonic));
                    Options.Add(new NormalDataProvider(DefaultData.DepthPercentDamageDefaultCurve(IDistributionEnum.Normal), _IsStrictMonotonic));
                    Options.Add(new TriangularDataProvider(DefaultData.DepthPercentDamageDefaultCurve(IDistributionEnum.Triangular), _IsStrictMonotonic));
                    Options.Add(new LogNormalDataProvider(DefaultData.DepthPercentDamageDefaultCurve(IDistributionEnum.LogNormal), _IsStrictMonotonic));
                } else
                {
                    Options.Add(new DeterministicDataProvider(_IsStrictMonotonic));
                    Options.Add(new UniformDataProvider(_IsStrictMonotonic));
                    Options.Add(new NormalDataProvider(_IsStrictMonotonic));
                    Options.Add(new TriangularDataProvider(_IsStrictMonotonic));
                    Options.Add(new LogNormalDataProvider(_IsStrictMonotonic));
                }
                 break;
                case DistributionOptions.DETERMINISTIC_ONLY:
if (_IsDepthPercentDamage)
                {
                    Options.Add(new DeterministicDataProvider(DefaultData.DepthPercentDamageDefaultCurve(IDistributionEnum.Deterministic), _IsStrictMonotonic));
                } else
                {
                    Options.Add(new DeterministicDataProvider(_IsStrictMonotonic));
                }                    
                break;
                case DistributionOptions.HISTOGRAM_ONLY:
                    Options.Add(new HistogramDataProvider(_IsStrictMonotonic));
                    break;
            }

            SelectedItem = Options.First();
        }

        public void SetMinMaxValues(double minY, double maxY)
        {
            foreach (BaseDataProvider opt in Options)
            {
                opt.yMin = minY;
                opt.yMax = maxY;
                opt.SetGlobalMaxAndMin();
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
        public UncertainPairedData SelectedItemToPairedData(string damCat = "", string assetCategory = "")
        {
            return SelectedItem.ToUncertainPairedData(XLabel, YLabel, Name, Description, damCat, assetCategory);
        }
        public virtual XElement ToXML()
        {
            XElement ele = new XElement(this.GetType().Name);
            ele.SetAttributeValue("selectedItem", SelectedItem.Name);
            ele.SetAttributeValue("Name", Name);
            ele.SetAttributeValue("DistributionOptions", _DistributionOptions.ToString());
            ele.SetAttributeValue("Description", Description);

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
            Description = element.Attribute("Description")?.Value;

            //for backwards compatibility
            if (element.Attribute("DistributionOptions") == null)
            {
                //this is an old study, there were only two options, deterministic only and default.
                bool isDeterministicOnly = Convert.ToBoolean(element.Attribute("DeterministicOnly")?.Value);
                if (isDeterministicOnly)
                {
                    _DistributionOptions = DistributionOptions.DETERMINISTIC_ONLY;
                }
                else
                {
                    _DistributionOptions = DistributionOptions.DEFAULT;
                }
            }
            else
            {
                Enum.TryParse(element.Attribute("DistributionOptions").Value, out DistributionOptions distOptions);
                _DistributionOptions = distOptions;
            }

            Name = element.Attribute("Name")?.Value;

            Options.Clear();
            foreach (XElement updEl in element.Elements())
            {
                string assemblyName = "HEC.FDA.ViewModel";//this libraries name and the appropriate namespace. "C:\Temp\FDA2.0_Internal\fda-viewmodel.dll"
                string typeName = assemblyName + ".TableWithPlot.Data." + updEl.Attribute("DistributionProviderType").Value;
                ObjectHandle oh = Activator.CreateInstance(null, typeName);//requires empty constructor
                BaseDataProvider dist = oh.Unwrap() as BaseDataProvider;

                UncertainPairedData upd = UncertainPairedData.ReadFromXML(updEl);
                XLabel = upd.XLabel;
                YLabel = upd.YLabel;
                Name = upd.Name;
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
