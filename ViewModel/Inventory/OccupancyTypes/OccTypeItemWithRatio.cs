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
    /// This class extends OccTypeItem. The "Content" and "Other" have an extra boolean option
    /// where the curve can be by value or by a ratio of the structure curve.
    /// </summary>
    public class OccTypeItemWithRatio : OccTypeAsset
    {
        private bool _IsByValue;
        private bool _IsNotByValue;

        private ValueUncertaintyVM _CurrentContentValueVM;
        private ValueUncertaintyVM _ContentByRatioVM;

        public ValueUncertaintyVM CurrentValueVM
        {
            get { return _CurrentContentValueVM; }
            set { _CurrentContentValueVM = value; SomethingChanged(this, EventArgs.Empty); NotifyPropertyChanged(); }
        }

        public ValueUncertaintyVM ContentByRatioVM
        {
            get { return _ContentByRatioVM; }
        }

        public bool IsByValue
        {
            get { return _IsByValue; }
            set 
            { 
                _IsByValue = value; 
                IsNotByValue = !value; 
                UpdateCurrentValueUncertaintyVM(); 
                SomethingChanged(this, EventArgs.Empty); 
                NotifyPropertyChanged(); 
            }
        }
        public bool IsNotByValue
        {
            get { return _IsNotByValue; }
            set { _IsNotByValue = value; NotifyPropertyChanged(); }
        }

        public OccTypeItemWithRatio(OccTypeItemWithRatio item):base(item)
        {
            _ContentByRatioVM = new OtherValueUncertaintyVM(item.ContentByRatioVM.Distribution);
            _ContentByRatioVM.WasModified += SomethingChanged;
            IsByValue = item.IsByValue;
        }


        public OccTypeItemWithRatio(XElement assetElem):base(assetElem)
        {
            IsByValue = Convert.ToBoolean(assetElem.Attribute("ByValue").Value);
            XElement ratioUncert = assetElem.Element("RatioUncertainty");
            ContinuousDistribution dist = (ContinuousDistribution)ContinuousDistribution.FromXML(ratioUncert.Descendants().First());
            _ContentByRatioVM = new OtherValueUncertaintyVM(dist);
            //_IsChecked = Convert.ToBoolean(assetElem.Attribute("IsSelected").Value);
            //_Curve = new ComputeComponentVM(assetElem.Element("ComputeComponentVM"));

            //_ItemValueUncertainty = new ValueUncertaintyVM(assetElem.Element("Uncertainty"));


        }

        public OccTypeItemWithRatio(OcctypeAssetType itemType, bool isChecked, ComputeComponentVM curve, ContinuousDistribution valueUncertainty,
            ContinuousDistribution ratioUncertainty, bool isByValue) : base(itemType, isChecked, curve, valueUncertainty)
        {
            _ContentByRatioVM = new OtherValueUncertaintyVM(ratioUncertainty);
            _ContentByRatioVM.WasModified += SomethingChanged;
            IsByValue = isByValue;
        }

        public override XElement ToXML()
        {
            XElement elem = base.ToXML();
            elem.SetAttributeValue("ByValue", _IsByValue);
            XElement ratioElem = new XElement("RatioUncertainty");
            ratioElem.Add(_ContentByRatioVM.CreateOrdinate().ToXML());
            elem.Add(ratioElem);
            return elem;
        }

        private void UpdateCurrentValueUncertaintyVM()
        {
            if (IsByValue)
            {
                CurrentValueVM = ValueUncertainty;
            }
            else
            {
                CurrentValueVM = _ContentByRatioVM;
            }
        }

        public override FdaValidationResult IsItemValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            vr.AddErrorMessage(base.IsItemValid().ErrorMessage);
            FdaValidationResult valueUncertVR = ContentByRatioVM.IsValueUncertaintyValid();
            if (!valueUncertVR.IsValid)
            {
                string errorMessage = ItemType + " value uncertainty:\n" + valueUncertVR.ErrorMessage;
                vr.AddErrorMessage(errorMessage + Environment.NewLine);
            }
            return vr;
        }

    }
}
