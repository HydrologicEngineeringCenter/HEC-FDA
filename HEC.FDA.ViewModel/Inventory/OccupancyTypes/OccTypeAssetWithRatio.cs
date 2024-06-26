﻿using HEC.FDA.ViewModel.TableWithPlot;
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
        private readonly ValueUncertaintyVM _ContentByRatioVM;

        /// <summary>
        /// This is the VM that shows in the UI
        /// </summary>
        public ValueUncertaintyVM CurrentValueVM
        {
            get { return _CurrentContentValueVM; }
            set { _CurrentContentValueVM = value; SomethingChanged(this, EventArgs.Empty); NotifyPropertyChanged(); }
        }

        /// <summary>
        /// This is the VM that clobbers the parent VM if the Use Ratio checkbox is checked. 
        /// </summary>
        public ValueUncertaintyVM ContentByRatioVM
        {
            get { return _ContentByRatioVM; }
        }

        /// <summary>
        /// If true, occupancy type uses the value assigned in the structure inventory as it's most likely, else it uses the assigned ratio of the structure value.
        /// </summary>
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
            XElement ratioElem = new("RatioUncertainty");
            ratioElem.Add(_ContentByRatioVM.CreateOrdinate().ToXML());
            elem.Add(ratioElem);
            return elem;
        }

        private void UpdateCurrentValueUncertaintyVM()
        {
            if (IsByValue)
            {
                CurrentValueVM = ValueUncertainty; // if we're just doing the normal by value, use the value uncertainty defined in the parent occTypeAsset
            }
            else
            {
                CurrentValueVM = _ContentByRatioVM; // if not, we need to be using this child one that includes the ratio.
            }
        }

    }
}
