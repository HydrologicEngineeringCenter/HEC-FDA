using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Utilities;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.TableWithPlot;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class ManualStageDamageVM: BaseViewModel
    {
        private TableWithPlotVM _TableWithPlot;
        private ManualStageDamageRowItem _SelectedRow;
        private ObservableCollection<ImpactAreaRowItem> _ImpactAreas;
        private ObservableCollection<String> _DamageCategories;
        private int _SelectedRowIndex = 0;
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
            loadDamageCategories();
            Rows.Add(CreateNewRow(1));
            SelectedRow = Rows[0];
            TableWithPlot = new TableWithPlotVM(CreateDefaultCurve());
        }

        public ManualStageDamageVM(List<StageDamageCurve> curves)
        {
            loadImpactAreas();
            loadDamageCategories();
            int i = 1;
            foreach(StageDamageCurve curve in curves)
            {     
                ManualStageDamageRowItem newRow = new ManualStageDamageRowItem(i, _ImpactAreas, _DamageCategories, curve.ComputeComponent);
                SelectItemsInRow(curve, newRow);
                Rows.Add(newRow);
                i++;
            }
            if(Rows.Count > 0)
            {
                SelectedRow = Rows[0];
            }
        }

        private ManualStageDamageRowItem CreateNewRow(int id)
        {
            return new ManualStageDamageRowItem(id, _ImpactAreas, _DamageCategories, CreateDefaultCurve());
        }

        private void SelectItemsInRow(StageDamageCurve curve, ManualStageDamageRowItem row)
        {
            ImpactAreaRowItem selectedImpArea = curve.ImpArea;
            string selectedDamCat = curve.DamCat;
            foreach(ImpactAreaRowItem ia in _ImpactAreas)
            {
                if(ia.ID == selectedImpArea.ID)
                {
                    row.SelectedImpArea = ia;
                    break;
                }
            }
            foreach (string dc in _DamageCategories)
            {
                if (dc.Equals(selectedDamCat))
                {
                    row.SelectedDamCat = dc;
                    break;
                }
            }
        }

        private ComputeComponentVM CreateDefaultCurve()
        {
            return new ComputeComponentVM("Stage Damage", "Stage", "Damage");
            //return Utilities.UncertainPairedDataFactory.CreateDefaultNormalData("Stage", "Damage", "testName");
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
                ManualStageDamageRowItem currentRI = SelectedRow;
                try
                {
                    //UncertainPairedData coordinatesFunction = currentRI.EditorVM.CreateFunctionFromTables();
                    //currentRI.EditorVM.Function = coordinatesFunction;
                    //currentRI.ComputeComponent = 
                }
                catch(InvalidConstructorArgumentsException ex)
                {
                    String msg = "Unable to copy current coordinates function:" + Environment.NewLine + ex.Message;
                    MessageBox.Show(msg, "Unable to Copy", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int lastRowId = Rows[Rows.Count - 1].ID;
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
                //in theory this call can throw an exception, but we handle that in the validation
                //if we get here, then the curves should be constructable.
                StageDamageCurve curve = new StageDamageCurve(r.SelectedImpArea, r.SelectedDamCat, r.ComputeComponent);
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
            return true;
            //List<string> rowsThatFailed = new List<string>();
            //foreach (ManualStageDamageRowItem r in Rows)
            //{
            //    UncertainPairedData coordFunc = r.EditorVM.CreateFunctionFromTables();
            //    if (coordFunc.Xvals.Length < 2)
            //    {
            //        rowsThatFailed.Add(r.ID.ToString());
            //    }
            //}

            //if(rowsThatFailed.Count>0)
            //{
            //    //\u2022 is a bullet character
            //    String msg = "Manually entered curves must have at least 2 points." + Environment.NewLine + "Curves in error:" + Environment.NewLine + "\t\u2022 ";
            //    MessageBox.Show(msg + string.Join(Environment.NewLine + "\t\u2022 ", rowsThatFailed), "Two Points Required", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            //return rowsThatFailed.Count == 0;
        }

        private bool AreManualCurvesValid()
        {
            //ObservableCollection<ManualStageDamageRowItem> rows = Rows;
            //foreach (ManualStageDamageRowItem r in rows)
            //{
            //    try
            //    {
            //        r.EditorVM.CreateFunctionFromTables();
            //    }
            //    catch (Exception ex)
            //    {
            //        //we have an invalid curve
            //        String msg = "An invalid curve was detected." + Environment.NewLine +
            //            "Invalid curve: " + r.ID + Environment.NewLine + ex.Message;
            //        MessageBox.Show(msg, "Unable to Save", MessageBoxButton.OK, MessageBoxImage.Error);
            //        return false;
            //    }
            //}
            return true;
        }

        private bool DoRowsHaveEqualValues(ManualStageDamageRowItem row1, ManualStageDamageRowItem row2)
        {
            bool areEqual = false;

            //The "!=" below weeds out the row that is itself
            if (row1 != row2)
            {
                //check imp area and dam cat
                if(row1.SelectedImpArea.ToString().Equals(row2.SelectedImpArea.ToString()) &&
                    row1.SelectedDamCat.Equals(row2.SelectedDamCat))
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
                String msg = "Stage-Damage curves must have unique impact area and damage category combinations." + Environment.NewLine +
                            "Repeat curves: " + iDList;
                MessageBox.Show(msg, "Unable to Save", MessageBoxButton.OK, MessageBoxImage.Error);
                AreManualRowsUniqueCombinations = false;
            }
            return AreManualRowsUniqueCombinations;
        }

        private void RowChanged()
        {
            TableWithPlot = new TableWithPlotVM( SelectedRow.ComputeComponent);
        }

    }
}
