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
    public class OccTypeAssetWithRatio : OccTypeAsset
    {
        private bool _IsByValue;

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
                UpdateCurrentValueUncertaintyVM(); 
                SomethingChanged(this, EventArgs.Empty); 
                NotifyPropertyChanged(); 
            }
        }

        public OccTypeAssetWithRatio(OccTypeAssetWithRatio item):base(item)
        {
            _ContentByRatioVM = new OtherValueUncertaintyVM(item.ContentByRatioVM.Distribution);
            _ContentByRatioVM.WasModified += SomethingChanged;
            IsByValue = item.IsByValue;
        }

        public OccTypeAssetWithRatio(XElement assetElem):base(assetElem)
        {
            IsByValue = Convert.ToBoolean(assetElem.Attribute("ByValue").Value);
            XElement ratioUncert = assetElem.Element("RatioUncertainty");
            ContinuousDistribution dist = (ContinuousDistribution)ContinuousDistribution.FromXML(ratioUncert.Descendants().First());
            _ContentByRatioVM = new OtherValueUncertaintyVM(dist);
        }

        public OccTypeAssetWithRatio(OcctypeAssetType itemType, bool isChecked, CurveComponentVM curve, ContinuousDistribution valueUncertainty,
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

    }
}
