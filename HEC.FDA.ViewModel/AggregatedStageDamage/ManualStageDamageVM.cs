using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class ManualStageDamageVM: BaseViewModel
    {
        private TableWithPlotVM _TableWithPlot;
        private ManualStageDamageRowItem _SelectedRow;
        private ObservableCollection<ImpactAreaRowItem> _ImpactAreas;
        private ObservableCollection<String> _DamageCategories = new ObservableCollection<string>();
        public ObservableCollection<ManualStageDamageRowItem> Rows { get; } = new ObservableCollection<ManualStageDamageRowItem>();
        public TableWithPlotVM TableWithPlot
        {
            get { return _TableWithPlot; }
            set { _TableWithPlot = value; NotifyPropertyChanged(); }
        }
        public ManualStageDamageRowItem SelectedRow 
        {
            get { return _SelectedRow; }
            set { _SelectedRow = value; NotifyPropertyChanged(); RowChanged(); }
        }

        public ManualStageDamageVM()
        {
            loadImpactAreas();
            LoadDamageCategories();
            Rows.Add(CreateNewRow(1));
            SelectedRow = Rows[0];
            TableWithPlot = new TableWithPlotVM(SelectedRow.ComputeComponent);
        }

        public ManualStageDamageVM(List<StageDamageCurve> curves)
        {
            loadImpactAreas();
            LoadDamageCategories();
            int i = 1;
            foreach(StageDamageCurve curve in curves)
            {     
                ManualStageDamageRowItem newRow = new ManualStageDamageRowItem(i, _ImpactAreas, _DamageCategories, curve.ComputeComponent.Clone(), curve.AssetCategory);
                SelectItemsInRow(curve, newRow);
                Rows.Add(newRow);
                i++;
            }
            if(Rows.Count > 0)
            {
                SelectedRow = Rows[0];
            }
        }

        private void LoadDamageCategories()
        {
            List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(OccupancyTypesElement));
            OccupancyTypesElement occTypesElement = existingElems.First() as OccupancyTypesElement;
            List<string> damageCategories = occTypesElement.GetUniqueDamageCategories();
            foreach (string damageCategory in damageCategories)
            {
                _DamageCategories.Add(damageCategory);
            }
        }

        private ManualStageDamageRowItem CreateNewRow(int id)
        {
            return new ManualStageDamageRowItem(id, _ImpactAreas, _DamageCategories, CreateDefaultCurve());
        }

        private void SelectItemsInRow(StageDamageCurve curve, ManualStageDamageRowItem row)
        {
            ImpactAreaRowItem selectedImpArea = curve.ImpArea;
            foreach(ImpactAreaRowItem ia in _ImpactAreas)
            {
                if(ia.ID == selectedImpArea.ID)
                {
                    row.SelectedImpArea = ia;
                    break;
                }
            }
            string selectedDamCat = curve.DamCat;
            foreach (string dc in _DamageCategories)
            {
                if (dc.Equals(selectedDamCat))
                {
                    row.SelectedDamCat = dc;
                    break;
                }
            }
        }

        private CurveComponentVM CreateDefaultCurve()
        {
            CurveComponentVM curve = DefaultData.StageDamageCurveComputeComponent(); 
            return curve;
        }

        private void loadImpactAreas()
        {
            _ImpactAreas = new ObservableCollection<ImpactAreaRowItem>();
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();

            //there should only ever be one impact area element
            foreach (ImpactAreaElement elem in impactAreaElements)
            {
                foreach (ImpactAreaRowItem row in elem.ImpactAreaRows)
                {
                    _ImpactAreas.Add(row);
                }
            }
        }

        /// <summary>
        /// Adds a row to the list of rows.
        /// </summary>
        public void Add()
        {        
            //for the id, get the last id and add one
            int lastRowId = 0;
            if (Rows.Count != 0)
            {
                lastRowId = Rows[Rows.Count - 1].ID;
            }
            Rows.Add(CreateNewRow(lastRowId+1));
            SelectedRow = Rows.Last();
        }

        public void Copy()
        {
            if(SelectedRow != null)
            {
                int lastRowId = Rows[^1].ID;
                ManualStageDamageRowItem newRow = new ManualStageDamageRowItem(lastRowId+1, SelectedRow);
                Rows.Add(newRow);
            }
        }

        public void Remove()
        {
            //don't allow the removing of the last row
            if (Rows.Count != 1)
            {
                int currentIndex = Rows.IndexOf(SelectedRow);
                if (currentIndex >= 0)
                {
                    Rows.RemoveAt(currentIndex);
                }
                //now set the selected index
                if (currentIndex >= Rows.Count)
                {
                    SelectedRow = Rows[Rows.Count - 1];
                }
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
            foreach (ManualStageDamageRowItem r in Rows)
            {
                StageDamageCurve curve = new StageDamageCurve(r.SelectedImpArea, r.SelectedDamCat, r.ComputeComponent, 
                    r.SelectedAssetCategory, StageDamageConstructionType.USER);
                curves.Add(curve);
            }
            return curves;
        }

        public bool ValidateForm()
        {
            //are there at least 2 points per curve
            bool atLeast2Points = AreThereTwoPointsPerCurve();
            bool areRowsUnique = AreManualRowsUniqueCombinations();
            bool curvesValid = AreManualCurvesValid();
            return areRowsUnique && curvesValid && atLeast2Points;
        }

        private bool AreThereTwoPointsPerCurve()
        {
            List<string> rowsThatFailed = new List<string>();
            foreach (ManualStageDamageRowItem r in Rows)
            {
                UncertainPairedData upd = r.ComputeComponent.SelectedItemToPairedData();
                if (upd.Xvals.Length < 2)
                {
                    rowsThatFailed.Add(r.ID.ToString());
                }
            }

            if (rowsThatFailed.Count > 0)
            {
                //\u2022 is a bullet character
                String msg = "Manually entered curves must have at least 2 points." + Environment.NewLine + "Curves in error:" + Environment.NewLine + "\t\u2022 ";
                MessageBox.Show(msg + string.Join(Environment.NewLine + "\t\u2022 ", rowsThatFailed), "Two Points Required", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return rowsThatFailed.Count == 0;
        }

        private bool AreManualCurvesValid()
        {
            ObservableCollection<ManualStageDamageRowItem> rows = Rows;
            foreach (ManualStageDamageRowItem r in rows)
            {
                try
                {
                    UncertainPairedData upd = r.ComputeComponent.SelectedItemToPairedData();
                }
                catch (Exception ex)
                {
                    //we have an invalid curve
                    String msg = "An invalid curve was detected." + Environment.NewLine +
                        "Invalid curve: " + r.ID + Environment.NewLine + ex.Message;
                    MessageBox.Show(msg, "Unable to Save", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }

        private bool DoRowsHaveEqualValues(ManualStageDamageRowItem row1, ManualStageDamageRowItem row2)
        {
            bool areEqual = false;

            //The "!=" below weeds out the row that is itself
            if (row1 != row2)
            {
                bool iaEqual = row1.SelectedImpArea.Name.Equals(row2.SelectedImpArea.Name);
                bool dcEqual = row1.SelectedDamCat.Equals(row2.SelectedDamCat);
                bool acEqual = row1.SelectedAssetCategory.Equals(row2.SelectedAssetCategory);
                if (iaEqual && dcEqual && acEqual)
                {
                    areEqual = true;
                }
            }
            return areEqual;
        }

        private bool AreManualRowsUniqueCombinations()
        {
            bool AreManualRowsUniqueCombinations = true;
            ObservableCollection<ManualStageDamageRowItem> rows = Rows;
            HashSet<int> repeatRows = new HashSet<int>();
            foreach (ManualStageDamageRowItem row in rows)
            {
                foreach (ManualStageDamageRowItem row2 in rows)
                {
                    if (DoRowsHaveEqualValues(row, row2))
                    {
                        repeatRows.Add(row.ID);
                        repeatRows.Add(row2.ID);
                    }
                }
            }
            if (repeatRows.Count > 0)
            {
                string iDList = string.Join(", ", repeatRows.Select(n => n.ToString()).ToArray());
                String msg = "Stage-Damage curves must have unique impact area, damage category, and asset category combinations." + Environment.NewLine +
                            "Repeat curves: " + iDList;
                MessageBox.Show(msg, "Unable to Save", MessageBoxButton.OK, MessageBoxImage.Error);
                AreManualRowsUniqueCombinations = false;
            }
            return AreManualRowsUniqueCombinations;
        }

        private void RowChanged()
        {
            if (SelectedRow != null)
            {
                TableWithPlot = new TableWithPlotVM(SelectedRow.ComputeComponent);
            }
        }

    }
}
