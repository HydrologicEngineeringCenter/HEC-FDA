using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.GeoTech
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
        public LeveeFeatureEditorVM(ComputeComponentVM defaultCurve, EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {
            //set default elevation
            Elevation = DefaultCurveData.DefaultLeveeElevation;
        }
        public LeveeFeatureEditorVM(CurveChildElement element, EditorActionManager actionManager) : base(element, actionManager)
        {
            Elevation = ((LateralStructureElement)element).Elevation;
            IsUsingDefault = ((LateralStructureElement)element).IsDefaultCurveUsed;

            //tell the table that the y values have to be between 0 and 1
            TableWithPlot.ComputeComponentVM.SetMinMaxValues(0, 1);
        }
        #endregion

        public override void Save()
        {
            int id = GetElementID<LateralStructureElement>();
            LateralStructureElement elem = new LateralStructureElement(Name, DateTime.Now.ToString("G"), Description, Elevation, IsUsingDefault, TableWithPlot.ComputeComponentVM, id);
            base.Save(elem);
        }
    }
}
