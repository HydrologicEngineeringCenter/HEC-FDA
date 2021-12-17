using ViewModel.ImpactArea;
using ViewModel.Inventory;
using ViewModel.WaterSurfaceElevation;
using Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Utilities;

namespace ViewModel.AggregatedStageDamage
{
    public class CalculatedStageDamageVM : BaseViewModel
    {
        public event EventHandler SelectedRowChanged;

        private ObservableCollection<WaterSurfaceElevationElement> _WaterSurfaceElevations;
        private WaterSurfaceElevationElement _SelectedWaterSurfaceElevation;

        private ObservableCollection<InventoryElement> _StructureInventoryElements;
        private InventoryElement _SelectedStructureInventoryElement;
        private CalculatedStageDamageRowItem _SelectedRow;


        public ObservableCollection<CalculatedStageDamageRowItem> Rows { get; set; }
        public CalculatedStageDamageRowItem SelectedRow
        {
            get { return _SelectedRow; }
            set { _SelectedRow = value; NotifyPropertyChanged(); SelectedRowChanged?.Invoke(this, new EventArgs()); }
        }

        public ObservableCollection<InventoryElement> Structures
        {
            get { return _StructureInventoryElements; }
            set { _StructureInventoryElements = value; NotifyPropertyChanged(); }
        }

        public InventoryElement SelectedStructures
        {
            get { return _SelectedStructureInventoryElement; }
            set { _SelectedStructureInventoryElement = value; NotifyPropertyChanged(); }
        }


        public ObservableCollection<WaterSurfaceElevationElement> WaterSurfaceElevations
        {
            get { return _WaterSurfaceElevations; }
            set { _WaterSurfaceElevations = value; NotifyPropertyChanged(); }
        }

        public WaterSurfaceElevationElement SelectedWaterSurfaceElevation
        {
            get { return _SelectedWaterSurfaceElevation; }
            set { _SelectedWaterSurfaceElevation = value; NotifyPropertyChanged(); }
        }

        public int SelectedRowIndex {get;set;}

        public CalculatedStageDamageVM()
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            loadStructureInventories();
            loadDepthGrids();
        }

        public CalculatedStageDamageVM(int wseId, int inventoryID, List<StageDamageCurve> curves)
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            loadStructureInventories();
            loadDepthGrids();
        }

        private void loadDepthGrids()
        {
            WaterSurfaceElevations = new ObservableCollection<WaterSurfaceElevationElement>();
            List<WaterSurfaceElevationElement> WSEElements = StudyCache.GetChildElementsOfType<WaterSurfaceElevationElement>();
            foreach (WaterSurfaceElevationElement elem in WSEElements)
            {
                WaterSurfaceElevations.Add(elem);
            }
            if (WaterSurfaceElevations.Count > 0)
            {
                SelectedWaterSurfaceElevation = WaterSurfaceElevations[0];
            }
        }

        private void loadStructureInventories()
        {
            Structures = new ObservableCollection<InventoryElement>();
            List<InventoryElement> inventoryElements = StudyCache.GetChildElementsOfType<InventoryElement>();
            foreach(InventoryElement elem in inventoryElements)
            {
                Structures.Add(elem);
            }
            if (Structures.Count > 0)
            {
                SelectedStructures = Structures[0];
            }

        }

        //private FdaValidationResult Validate()
        //{
        //    FdaValidationResult vr = new FdaValidationResult();

        //    List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        //    if (impactAreaElements.Count == 0)
        //    {
        //        vr.AddErrorMessage("An impac")
        //    }
        //}

        public void CalculateCurves()
        {
            //todo Richard will implament this method. I am not sure what all you need. You can grab elements from the study cache
            //just like i did in the line below to get the impact area elements. You will need to create "CalculatedStageDamageRowItem"s.
            //These objects basically hold an impact area, damcat, coordinates function. The coordinates function gets created by using
            //the ICoordinatesFunctionsFactory. To get the curves to show up in the UI you just add them to the "Rows" property.

            //todo validation?
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            
            //there should only ever be one impact area. To get to this point we know we have an impact area.
            ImpactAreaElement impactAreaElement = impactAreaElements[0];
            
            InventoryElement inventoryElement = SelectedStructures;
            WaterSurfaceElevationElement waterSurfaceElevationElement = SelectedWaterSurfaceElevation;

            //todo delete these dummy rows once we have the actual compute in place.
            for (int i = 1;i<11;i++)
            {
                List<double> xs = new List<double>();
                List<double> ys = new List<double>();
                for(int j=0;j<=i;j++)
                {
                    xs.Add(j);
                    ys.Add(j);
                }
                ICoordinatesFunction testFunc = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
                Rows.Add(new CalculatedStageDamageRowItem(i , impactAreaElements[0].ImpactAreaRows[0], "testDamCat" + i, testFunc));
            }
            //end dummy rows



        }

        /// <summary>
        /// This should only be called after validation has been called. Creating a coordinates funtion from the table
        /// of values can throw an exception. 
        /// </summary>
        /// <returns></returns>
        public List<StageDamageCurve> GetStageDamageCurves()
        {
            List<StageDamageCurve> curves = new List<StageDamageCurve>();
            foreach (CalculatedStageDamageRowItem r in Rows)
            {
                //in theory this call can throw an exception, but we handle that in the validation
                //if we get here, then the curves should be constructable.
                StageDamageCurve curve = new StageDamageCurve(r.ImpactArea, r.DamageCategory, r.EditorVM.CreateFunctionFromTables());
                curves.Add(curve);
            }

            return curves;
        }


    }
}
