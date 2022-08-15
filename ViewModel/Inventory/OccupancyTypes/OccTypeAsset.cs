using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System;
using System.Linq;
using System.Xml.Linq;
using ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// The "Item" is either structure, content, or other.
    /// This class holds the value uncertainty, the "is selected", and the curve
    /// </summary>
    public class OccTypeAsset:BaseViewModel
    {
        public event EventHandler DataModified;

        private bool _IsChecked;
        private ComputeComponentVM _Curve;
        private TableWithPlotVM _ItemTableWithPlot;
        private ValueUncertaintyVM _ItemValueUncertainty;
 
        

        /// <summary>
        /// This enum is only being used so that i can use it as a string name in any error messages.
        /// </summary>
        public enum OcctypeAssetType
        {
            structure,
            content,
            vehicle,
            other
        }
        public OcctypeAssetType ItemType
        {
            get;
        }

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; SomethingChanged(this, EventArgs.Empty); NotifyPropertyChanged(); }
        }
        public ComputeComponentVM Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }

        public TableWithPlotVM TableWithPlot
        {
            get { return _ItemTableWithPlot; }
            set { _ItemTableWithPlot = value; NotifyPropertyChanged(); }
        }

        public ValueUncertaintyVM ValueUncertainty
        {
            get { return _ItemValueUncertainty; }
            set
            {
                _ItemValueUncertainty = value;
                _ItemValueUncertainty.WasModified += SomethingChanged;
            }
        }

        //todo: get rid of this ctor and call toxml and use that one?
        public OccTypeAsset(OccTypeAsset item)
        {
            ItemType = item.ItemType;
            IsChecked = item.IsChecked;
            Curve = new ComputeComponentVM(item.Curve.ToXML());
            TableWithPlot = new TableWithPlotVM(Curve);
            TableWithPlot.WasModified += SomethingChanged;
            ValueUncertainty = new MonetaryValueUncertaintyVM(item.ValueUncertainty.Distribution);
        }

        public OccTypeAsset(XElement assetElem)
        {
            _IsChecked = Convert.ToBoolean( assetElem.Attribute("IsSelected").Value);
            _Curve = new ComputeComponentVM(assetElem.Element("ComputeComponentVM"));

            XElement uncertElem = assetElem.Element("Uncertainty");

            ContinuousDistribution dist = (ContinuousDistribution)ContinuousDistribution.FromXML(uncertElem.Descendants().First());

            ValueUncertainty = new MonetaryValueUncertaintyVM(dist);
        }

        public virtual XElement ToXML()
        {
            XElement assetElem = new XElement("Asset");
            assetElem.SetAttributeValue("IsSelected", _IsChecked);
            assetElem.SetAttributeValue("Type", ItemType);

            assetElem.Add(_Curve.ToXML());
            
            assetElem.Add(CreateUncertaintyElement());

            return assetElem;
        }

        private XElement CreateUncertaintyElement()
        {
            XElement uncertElem = new XElement("Uncertainty");

            uncertElem.Add(_ItemValueUncertainty.CreateOrdinate().ToXML());

            //uncertElem.SetAttributeValue("UncertType", _ItemValueUncertainty.GetType());            
         
            ////for the uncertainty type, if deterministic, the currentVM will be null
            //if (_ItemValueUncertainty.CurrentVM == null)
            //{
            //    XElement deterministicElem = new XElement("Deterministic");
            //    uncertElem.Add(deterministicElem);
            //}
            //else
            //{
            //    uncertElem.Add(_ItemValueUncertainty.CurrentVM.CreateOrdinate().ToXML());
            //}
            return uncertElem;
        }

        public OccTypeAsset(OcctypeAssetType itemType, bool isChecked, ComputeComponentVM curve, ContinuousDistribution valueUncertainty)
        {
            ItemType = itemType;
            IsChecked = isChecked;
            Curve = curve;
            TableWithPlot = new TableWithPlotVM(Curve);
            TableWithPlot.WasModified += SomethingChanged;
            ValueUncertainty = new MonetaryValueUncertaintyVM(valueUncertainty);
        }
        public void SomethingChanged(object sender, EventArgs e)
        {
            DataModified?.Invoke(this, EventArgs.Empty);
        }

        public virtual FdaValidationResult IsItemValid()
        {
            FdaValidationResult vr = new FdaValidationResult();

            FdaValidationResult valueUncertVR = ValueUncertainty.IsValueUncertaintyValid();
            //I want to indicate what "item type" this is.
            if(!valueUncertVR.IsValid)
            {
                string errorMessage = ItemType + " value uncertainty:\n" + valueUncertVR.ErrorMessage;
                vr.AddErrorMessage(errorMessage + Environment.NewLine);
            }
            FdaValidationResult fdaValidationResult = TableWithPlot.GetTableErrors();
            vr.AddErrorMessage(fdaValidationResult.ErrorMessage);
            return vr;
        }
    }
}
