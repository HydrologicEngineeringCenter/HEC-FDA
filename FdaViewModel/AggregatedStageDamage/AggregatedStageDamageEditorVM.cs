using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageEditorVM:BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private bool _ReadOnly = false;
        private readonly string _Title = "Aggregated Stage Damage";
        private Statistics.UncertainCurveDataCollection _Curve;
        private Inventory.DamageCategory.DamageCategoryRowItem _DamageCategory;
        private List<Inventory.DamageCategory.DamageCategoryRowItem> _damagecategories;//and the selected damage category?
        #endregion
        #region Properties
        public string Name {
            get { return _Name; }
            set { _Name = value;  NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
 
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value;  NotifyPropertyChanged(); }
        }
        public string PlotTitle
        {
            get { return _Title; }
        }
        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value;  NotifyPropertyChanged(); }
        }
        AggregatedStageDamageElement CurrentElement { get; set; }

        #endregion
        #region Constructors
        public AggregatedStageDamageEditorVM() : base()
        {
            double[] xValues = new double[16] { 95, 97, 99, 101, 103, 105, 107, 110, 112, 115, 117, 120, 122, 125, 127, 130 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[16] { new Statistics.None(0), new Statistics.None(0), new Statistics.None(0), new Statistics.None(1), new Statistics.None(3), new Statistics.None(10), new Statistics.None(50), new Statistics.None(1000), new Statistics.None(2000), new Statistics.None(4000), new Statistics.None(8000), new Statistics.None(10000), new Statistics.None(11000), new Statistics.None(11500), new Statistics.None(11750), new Statistics.None(11875) };

            Curve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

        }

        public AggregatedStageDamageEditorVM(AggregatedStageDamageElement elem):base()
        {
            CurrentElement = elem;
            CurrentElement.ChangeIndex = 0;
            Name = elem.Name;
            Description = elem.Description;
            Curve = elem.Curve;
        }
        #endregion
        #region Voids
        public override void Undo()
        {
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());
            if (CurrentElement.ChangeIndex < changeTableView.NumberOfRows - 1)
            {
                //disable the undo button somehow?
                AggregatedStageDamageElement prevElement = (AggregatedStageDamageElement)CurrentElement.GetPreviousElementFromChangeTable(CurrentElement.ChangeIndex + 1);
                if (prevElement != null)// null if out of range index
                {
                    Name = prevElement.Name;
                    LastEditDate = prevElement.LastEditDate;
                    Description = prevElement.Description;
                    Curve = prevElement.Curve;
                    CurrentElement.ChangeIndex += 1;
                }
            }
        }

        public override void Redo()
        {
            //get the previous state
            if (CurrentElement.ChangeIndex > 0)
            {
                AggregatedStageDamageElement nextElement = (AggregatedStageDamageElement)CurrentElement.GetNextElementFromChangeTable(CurrentElement.ChangeIndex - 1);
                if (nextElement != null)// null if out of range index
                {
                    Name = nextElement.Name;
                    LastEditDate = nextElement.LastEditDate;
                    Description = nextElement.Description;
                    Curve = nextElement.Curve;
                    CurrentElement.ChangeIndex -= 1;
                }
            }
        }
        public override void AddValidationRules()
        {
            //AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank");

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
