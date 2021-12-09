using ViewModel.ImpactArea;
using ViewModel.Inventory.DamageCategory;
using ViewModel.Inventory.OccupancyTypes;
using Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utilities;

namespace ViewModel.AggregatedStageDamage
{
    public class ManualStageDamageVM: BaseViewModel
    {
        public event EventHandler SelectedRowChanged;

        private ManualStageDamageRowItem _SelectedRow;
        private ICoordinatesFunction _DefaultFunction;
        private ObservableCollection<ImpactAreaRowItem> _ImpactAreas;
        private ObservableCollection<String> _DamageCategories;
        public ObservableCollection<ManualStageDamageRowItem> Rows { get; set; }
        public ManualStageDamageRowItem SelectedRow 
        {
            get { return _SelectedRow; }
            set { _SelectedRow = value; NotifyPropertyChanged(); SelectedRowChanged?.Invoke(this, new EventArgs()); }
        }
        public int SelectedRowIndex 
        { 
            get; 
            set; 
        }

        public ManualStageDamageVM()
        {
            loadDefaultCurve();
            //todo: use actual imp areas and dam cats
            loadImpactAreas();
            loadDamageCategories();

            Rows = new ObservableCollection<ManualStageDamageRowItem>();
            Rows.Add(new ManualStageDamageRowItem(1, _ImpactAreas, _DamageCategories, _DefaultFunction));

        }

        public ManualStageDamageVM(List<StageDamageCurve> curves)
        {
            Rows = new ObservableCollection<ManualStageDamageRowItem>();
            //todo what if the impact areas or damcats have changed?
            loadImpactAreas();
            loadDamageCategories();
            int i = 1;
            foreach(StageDamageCurve curve in curves)
            {
                ManualStageDamageRowItem newRow = new ManualStageDamageRowItem(i, _ImpactAreas, _DamageCategories, curve.Function);
                Rows.Add(newRow);
                i++;
            }
        }

        private void loadDefaultCurve()
        {
            List<double> xs = new List<double>() { 0,1,2,3,4,5,6,7,8,9 };
            List<double> ys = new List<double>() { 0,1,2,3,4,5,6,7,8,9 };
            _DefaultFunction = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
        }

        private void loadImpactAreas()
        {
            _ImpactAreas = new ObservableCollection<ImpactAreaRowItem>();
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElements.Count == 0)
            {
                //then we will use the default impact area. It should have an ID of 0
                ImpactAreaRowItem defaultRI = new ImpactAreaRowItem(0,"Default");
                _ImpactAreas.Add(defaultRI);

            }
            else
            {
                //there should only ever be one impact area element
                foreach (ImpactAreaElement elem in impactAreaElements)
                {
                    foreach (ImpactAreaRowItem row in elem.ImpactAreaRows)
                    {
                        _ImpactAreas.Add(row);
                    }
                }
            }
        }

        private void loadDamageCategories()
        {
            List<OccupancyTypesElement> occupancyTypesElements = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();
            _DamageCategories = new ObservableCollection<String>();
            foreach(OccupancyTypesElement elem in occupancyTypesElements)
            {
                List<String> damageCategories = elem.getUniqueDamageCategories();
                damageCategories.ForEach(_DamageCategories.Add);
            }
        }

        public void Add()
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3, 4 };
            List<double> ys = new List<double>() { 0, 1, 2, 3, 4 };
            ICoordinatesFunction defaultFunction = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            //for the id, get the last id and add one
            int lastRowId = Rows[Rows.Count - 1].ID;
            Rows.Add(new ManualStageDamageRowItem(lastRowId+1, _ImpactAreas, _DamageCategories, defaultFunction));
        }

        public void Copy()
        {
            if(SelectedRowIndex >=0)
            {
                ManualStageDamageRowItem currentRI = Rows[SelectedRowIndex];
                try
                {
                    ICoordinatesFunction coordinatesFunction = currentRI.EditorVM.CreateFunctionFromTables();
                    currentRI.EditorVM.Function = coordinatesFunction;
                }
                catch(InvalidConstructorArgumentsException ex)
                {
                    String msg = "Unable to copy current coordinates function:" + Environment.NewLine + ex.Message;
                    MessageBox.Show(msg, "Unable to Copy", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int lastRowId = Rows[Rows.Count - 1].ID;
                ManualStageDamageRowItem newRow = new ManualStageDamageRowItem(lastRowId+1, Rows[SelectedRowIndex]);
                Rows.Add(newRow);
            }
        }

        public void Remove()
        {
            if (SelectedRowIndex >= 0)
            {
                Rows.RemoveAt(SelectedRowIndex);
            }
        }

        /// <summary>
        /// This should only be called after validation has been called. Creating a coordinates funtion from the table
        /// of values can throw an exception. 
        /// </summary>
        /// <returns></returns>
        public List<StageDamageCurve> GetStageDamageCurves()
        {
            List<StageDamageCurve> curves = new List<StageDamageCurve>();
            ObservableCollection<ManualStageDamageRowItem> rows = Rows;
            foreach (ManualStageDamageRowItem r in rows)
            {
                //in theory this call can throw an exception, but we handle that in the validation
                //if we get here, then the curves should be constructable.
                StageDamageCurve curve = new StageDamageCurve(r.SelectedImpArea, r.SelectedDamCat, r.EditorVM.CreateFunctionFromTables());
                curves.Add(curve);
            }

            return curves;
        }

    }
}
