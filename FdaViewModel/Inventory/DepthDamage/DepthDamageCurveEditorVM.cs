using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;


namespace FdaViewModel.Inventory.DepthDamage
{
    //[Author(q0heccdm, 8 / 1 / 2017 3:52:10 PM)]
    public class DepthDamageCurveEditorVM : BaseViewModel
    {

        public event EventHandler TheRowsNeedUpdating;


        #region Notes
        // Created By: q0heccdm
        // Created Date: 8/1/2017 3:52:10 PM
        #endregion
        #region Fields
        private Dictionary<string, DepthDamageCurve> _CurveDictionary;
        private DepthDamageCurve _SelectedDepthDamageCurve;
        private List<string> _DamageTypeEnums;
        private ObservableCollection<DepthDamageCurve> _ListOfDepthDamageCurves;
        private DepthDamageCurveEditorControlVM _CurrentDepthDamageCurveVM;
        private DepthDamageCurveEditorControlVM _PreviousDepthDamageCurveVM;
        private ObservableCollection<DepthDamageCurveEditorControlVM> _ListOfDepthDamageVMs;
        //private Statistics.UncertainCurveDataCollection _SelectedDepthDamageCurve_Curve;
        //private string _SelectedDamageType;
        #endregion
        #region Properties
            public ObservableCollection<DepthDamageCurveEditorControlVM> ListOfDepthDamageVMs
        {
            get { return _ListOfDepthDamageVMs; }
            set { _ListOfDepthDamageVMs = value; NotifyPropertyChanged(); }
        }
        public DepthDamageCurveEditorControlVM CurrentDepthDamageCurveVM
        {
            get { return _CurrentDepthDamageCurveVM; }
            set { _CurrentDepthDamageCurveVM = value; NotifyPropertyChanged(); }
        }
        public DepthDamageCurveEditorControlVM PreviousDepthDamageCurveVM
        {
            get { return _PreviousDepthDamageCurveVM; }
            set { _PreviousDepthDamageCurveVM = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<DepthDamageCurve> ListOfDepthDamageCurves
        {
            get { return _ListOfDepthDamageCurves; }
            set { _ListOfDepthDamageCurves = value;NotifyPropertyChanged(); }
        }
        public Statistics.UncertainCurveDataCollection SelectedDepthDamageCurve_Curve
        {
            get { return _SelectedDepthDamageCurve.Curve; }
            set { _SelectedDepthDamageCurve.Curve = value; NotifyPropertyChanged(); }
        }

        public DepthDamageCurve SelectedDepthDamageCurve
        {
            get { return _SelectedDepthDamageCurve; }
            set { _SelectedDepthDamageCurve = value; NotifyPropertyChanged(); }// SelectedDepthDamageCurve_Curve = _SelectedDepthDamageCurve.Curve;  NotifyPropertyChanged(); }
        }

        public  Dictionary<string, DepthDamageCurve> CurveDictionary
        {
            get { return _CurveDictionary; }
            set { _CurveDictionary = value; }
        }

        public List<string> DamageTypeEnums
        {
            get { return _DamageTypeEnums; }
            set { _DamageTypeEnums = value; NotifyPropertyChanged(); }
        }

        
       

        //public string SelectedDamageType
        //{
        //    get { if (SelectedDepthDamageCurve == null) { return null; } return _SelectedDepthDamageCurve.DamageType.ToString(); }
        //    set
        //    {
        //        DepthDamageCurve.DamageTypeEnum newEnum;
        //        if(Enum.TryParse(value, out newEnum))
        //        {
        //            //success
        //            _SelectedDepthDamageCurve.DamageType = newEnum;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}


        #endregion
        #region Constructors
        public DepthDamageCurveEditorVM() : base()
        {
            CurveDictionary = DepthDamageCurveData.CurveDictionary;
            //SelectedDepthDamageCurve = CurveDictionary.FirstOrDefault().Value;
            //string[] enumValues = Enum.GetNames(typeof(DepthDamageCurve.DamageTypeEnum));
            //DamageTypeEnums = enumValues.ToList();
            _ListOfDepthDamageVMs = new ObservableCollection<DepthDamageCurveEditorControlVM>();
            //_ListOfDepthDamageCurves = new ObservableCollection<DepthDamageCurve>();
            //LoadTheListOfDepthDamageCurves();
            LoadTheListOfDepthDamageVMs();

            if (ListOfDepthDamageVMs.Count > 0)
            {
                _CurrentDepthDamageCurveVM = ListOfDepthDamageVMs[0];
            }


        }
        #endregion
        #region Voids
        private void LoadTheListOfDepthDamageVMs()
        {
            List<string> keys = CurveDictionary.Keys.ToList();

            foreach (string key in keys)
            {
                //DepthDamageCurveEditorControlVM newRow = new DepthDamageCurveEditorControlVM(ddc);
                //newRow.CheckForNameConflict += new EventHandler(CheckForNameConflict); 

                if (CurveDictionary.ContainsKey(key))
                {
                    ListOfDepthDamageVMs.Add(new DepthDamageCurveEditorControlVM(key, CurveDictionary[key]));

                }

            }
        }

        public void CheckForNameConflict(string newName)
        {
            //string newName = ((CheckForNameConflictEventArgs)e).Name;
            foreach(DepthDamageCurveEditorControlVM row in ListOfDepthDamageVMs)
            {
                if(row.Name == newName)
                {
                    //need to rename 

                }
            }
        }

        //public void UpdateTheRows(object sender,EventArgs e)
        //{
        //    {
        //        if (this.TheRowsNeedUpdating != null)
        //        {
        //            this.TheRowsNeedUpdating(this, new EventArgs());
        //        }
        //    }
        //}

        public void ChangeSelectedDepthDamageCurve(DepthDamageCurveEditorControlVM ddc)
        {
            
            //CurrentDepthDamageCurveVM = new DepthDamageCurveEditorControlVM(ddc.Curve);
        }

        private void LoadTheListOfDepthDamageCurves()
        {

            foreach (DepthDamageCurve curve in CurveDictionary.Values)
            {
                _ListOfDepthDamageCurves.Add(curve);
            }
            //ListCollectionView lcv = new ListCollectionView(collectionOfCurves);

            //lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FdaViewModel.Inventory.DepthDamage.DepthDamageCurve.DamageType)));
            //lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(FdaViewModel.Inventory.DepthDamage.DepthDamageCurve.DamageType), System.ComponentModel.ListSortDirection.Ascending));
            //lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(FdaViewModel.Inventory.DepthDamage.DepthDamageCurve.Name), System.ComponentModel.ListSortDirection.Ascending));

            //DepthDamageListView.ItemsSource = lcv;
        }

        public void LanuchCopyDepthDamageWindow()
        {
            OccupancyTypes.CreateNewDamCatVM vm = new OccupancyTypes.CreateNewDamCatVM();

            Navigate(vm, true, true, "Enter Depth Damage Curve Name");
            if (vm.WasCancled == false)
            {
                if (vm.HasError == false)
                {
                    DepthDamageCurve ddc = new DepthDamageCurve(vm.Name, CurrentDepthDamageCurveVM.Description, CurrentDepthDamageCurveVM.Curve, CurrentDepthDamageCurveVM.DamageType);
                    DepthDamageCurveEditorControlVM newRow = new DepthDamageCurveEditorControlVM(vm.Name,ddc);

                    ListOfDepthDamageVMs.Add(newRow);
                }
            }
        }

        public void LanuchNewDepthDamageWindow()
        {
            OccupancyTypes.CreateNewDamCatVM vm = new OccupancyTypes.CreateNewDamCatVM();

            Navigate(vm, true, true, "Enter Depth Damage Curve Name");
            if (vm.WasCancled == false)
            {
                if (vm.HasError == false)
                {

                    CheckForNameConflict(vm.Name);

                    DepthDamageCurve ddc = new DepthDamageCurve(vm.Name, "", new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None), CurrentDepthDamageCurveVM.DamageType);
                    DepthDamageCurveEditorControlVM newRow = new DepthDamageCurveEditorControlVM(vm.Name, ddc);

                    ListOfDepthDamageVMs.Add(newRow);
                }
            }
        }

        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {        
            //throw new NotImplementedException();
        }
    }
}
