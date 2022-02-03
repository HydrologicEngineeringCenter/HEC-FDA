using paireddata;
using Statistics.Distributions;
using System.Collections.Generic;
using ViewModel.Editors;
using ViewModel.Utilities;

namespace ViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 1:20:23 PM)]
    public class LeveeFeatureEditorVM : CurveEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 1:20:23 PM
        #endregion
        #region Fields
      
        private double _Elevation = 0;
        private bool _UsingDefaultCurve = true;
        #endregion
        #region Properties
       
        public double Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; NotifyPropertyChanged(); }
        }
        public bool IsUsingDefault
        {
            get { return _UsingDefaultCurve; }
            set { _UsingDefaultCurve = value;NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public LeveeFeatureEditorVM(UncertainPairedData defaultCurve, Editors.EditorActionManager actionManager) : base(defaultCurve,"Probability","Stage","chartTitle", actionManager)
        {

        }
        public LeveeFeatureEditorVM( ChildElement element, Editors.EditorActionManager actionManager) : base(element, "Probability", "Stage", "chartTitle",actionManager)
        {
            Elevation = ((LeveeFeatureElement)element).Elevation;
            IsUsingDefault = ((LeveeFeatureElement)element).IsDefaultCurveUsed;
        }
        #endregion
        #region Voids
        private void UpdateChart(double newElevationValue)
        {
            //if (_UsingDefaultCurve)
            //{
            //    //if this is the first time then there will only be one coordinate
            //    CoordinatesFunctionTableVM firstTable =  EditorVM.Tables[0];
            //    CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(1, false)
            //                            .WithConstantDist(newElevationValue)
            //                            .Build();
            //    if (firstTable.Rows.Count > 1)
            //    {
            //        //replace the second row
            //        firstTable.DeleteRows(new List<int> { 1 });
            //        firstTable.AddRow(row);
            //    }
            //    else
            //    {
            //        //add the second row
            //        firstTable.AddRow(row);
            //    }
            //}
        }
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be null.");
        }

        public override UncertainPairedData GetCoordinatesFunction()
        {
            if(IsUsingDefault)
            {
                //in this case then we create a special default coordinates function
                List<double> defaultXs = new List<double>() { Elevation, Elevation + .000000000000001 };
                List<Deterministic> defaultYs = new List<Deterministic>() { new Deterministic(0), new Deterministic(1) };

                return new UncertainPairedData(defaultXs.ToArray(), defaultYs.ToArray(), "Elevation", "Probability", "Failure Function", "", -1);
            }
            else
            {
                return base.GetCoordinatesFunction();
            }
        }
    }
}
