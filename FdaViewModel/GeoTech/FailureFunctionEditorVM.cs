using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 2:11:22 PM)]
    public class FailureFunctionEditorVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 2:11:22 PM
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private readonly string _Title = "Failure Function Curve";
        private Statistics.UncertainCurveDataCollection _Curve;
        private List<LeveeFeatureElement> _LateralStructureList;
        private LeveeFeatureElement _SelectedLateralStructure;
        #endregion
        #region Properties
        public LeveeFeatureElement SelectedLateralStructure
        {
            get { return _SelectedLateralStructure; }
            set { _SelectedLateralStructure = value; NotifyPropertyChanged();  }
        }
        public List<LeveeFeatureElement> LateralStructureList
        {
            get { return _LateralStructureList; }
            set { _LateralStructureList = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        public string PlotTitle
        {
            get { return _Title; }
        }
        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        FailureFunctionElement CurrentElement { get; set; }

        #endregion
        #region Constructors
        public FailureFunctionEditorVM(List<LeveeFeatureElement> leveeList):base()
        {
            double[] xValues = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(0), new Statistics.None(.1), new Statistics.None(.2), new Statistics.None(.3), new Statistics.None(.4), new Statistics.None(.5), new Statistics.None(.6), new Statistics.None(.7), new Statistics.None(.8), new Statistics.None(.9), new Statistics.None(.95), new Statistics.None(.99) };
            Curve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            LateralStructureList = leveeList;
        }

        public FailureFunctionEditorVM(FailureFunctionElement elem, List<LeveeFeatureElement> latStructList) :base()// string name, string description, Statistics.UncertainCurveDataCollection curve,LeveeFeatureElement selectedLevee, List<LeveeFeatureElement> latStructList):base()
        {
            CurrentElement = elem;
            CurrentElement.ChangeIndex = 0;
            Name = elem.Name;
            Description = elem.Description;// description;
            Curve = elem.FailureFunctionCurve;// curve;
            LateralStructureList = latStructList;
            SelectedLateralStructure = elem.SelectedLateralStructure;// selectedLevee;
        }
        #endregion
        #region Voids
        public override void Undo()
        {
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());
            if (CurrentElement.ChangeIndex < changeTableView.NumberOfRows - 1)
            {
                //disable the undo button somehow?
                FailureFunctionElement prevElement = (FailureFunctionElement)CurrentElement.GetPreviousElementFromChangeTable(CurrentElement.ChangeIndex + 1);
                if (prevElement != null)// null if out of range index
                {
                    Name = prevElement.Name;
                    LastEditDate = prevElement.LastEditDate;
                    Description = prevElement.Description;
                    Curve = prevElement.FailureFunctionCurve;
                    CurrentElement.ChangeIndex += 1;
                }
            }
        }

        public override void Redo()
        {
            //get the previous state
            if (CurrentElement.ChangeIndex > 0)
            {
                FailureFunctionElement nextElement = (FailureFunctionElement)CurrentElement.GetNextElementFromChangeTable(CurrentElement.ChangeIndex - 1);
                if (nextElement != null)// null if out of range index
                {
                    Name = nextElement.Name;
                    LastEditDate = nextElement.LastEditDate;
                    Description = nextElement.Description;
                    Curve = nextElement.FailureFunctionCurve;
                    CurrentElement.ChangeIndex -= 1;
                }
            }
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
        #endregion

    }
}
