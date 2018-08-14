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
        public Inventory.DamageCategory.DamageCategoryRowItem DamageCategory
        {
            get { return _DamageCategory; }
            set { _DamageCategory = value; NotifyPropertyChanged(); }
        }
        public List<Inventory.DamageCategory.DamageCategoryRowItem> DamageCategories
        {
            get { return _damagecategories; }
            set { _damagecategories = value; NotifyPropertyChanged(); }
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
        #endregion
        #region Constructors
        public AggregatedStageDamageEditorVM() : base()
        {
            double[] xValues = new double[16] { 95, 97, 99, 101, 103, 105, 107, 110, 112, 115, 117, 120, 122, 125, 127, 130 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[16] { new Statistics.None(0), new Statistics.None(0), new Statistics.None(0), new Statistics.None(1), new Statistics.None(3), new Statistics.None(10), new Statistics.None(50), new Statistics.None(1000), new Statistics.None(2000), new Statistics.None(4000), new Statistics.None(8000), new Statistics.None(10000), new Statistics.None(11000), new Statistics.None(11500), new Statistics.None(11750), new Statistics.None(11875) };

            Curve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

        }
        //public AggregatedStageDamageEditorVM() : base()
        //{
        //    //DamageCategories = items;
        //    //DamageCategory = items.FirstOrDefault();
        //    //double[] xValues = new double[] { 0, 1, 2, 5, 7, 8, 9, 10, 12, 15, 20 };
        //    //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(3), new Statistics.None(3), new Statistics.None(3), new Statistics.None(600), new Statistics.None(1100), new Statistics.None(1300), new Statistics.None(1800), new Statistics.None(10000), new Statistics.None(30000), new Statistics.None(100000), new Statistics.None(500000) };

        //    double[] xValues = new double[16] { 95, 97, 99, 101, 103, 105, 107, 110, 112, 115, 117, 120, 122, 125, 127, 130 };
        //    Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[16] { new Statistics.None(0), new Statistics.None(0), new Statistics.None(0), new Statistics.None(1), new Statistics.None(3), new Statistics.None(10), new Statistics.None(50), new Statistics.None(1000), new Statistics.None(2000), new Statistics.None(4000), new Statistics.None(8000), new Statistics.None(10000), new Statistics.None(11000), new Statistics.None(11500), new Statistics.None(11750), new Statistics.None(11875)};

        //    Curve = new Statistics.UncertainCurveIncreasing(xValues,yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        //}
        public AggregatedStageDamageEditorVM(string name, string description, Inventory.DamageCategory.DamageCategoryRowItem damagecategory, List<Inventory.DamageCategory.DamageCategoryRowItem> damcats, Statistics.UncertainCurveDataCollection curve) : base()
        {
            Name = name;
            Description = description;
            DamageCategories = damcats;
            DamageCategory = damagecategory;
            Curve = curve;
        }
        #endregion
        #region Voids
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
