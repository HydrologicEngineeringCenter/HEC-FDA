using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 10:57:35 AM)]
    public class ExteriorInteriorEditorVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 10:57:35 AM
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private readonly string _Title = "Exterior-Interior Curve";

        private Statistics.UncertainCurveDataCollection _Curve;
        #endregion
        #region Properties
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


        #endregion
        #region Constructors
        public ExteriorInteriorEditorVM():base()
        {
            double[] xs = new double[] { 90, 100, 105, 110, 112, 115, 116, 117, 118, 130 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(95), new Statistics.None(96), new Statistics.None(100), new Statistics.None(105), new Statistics.None(106), new Statistics.None(107), new Statistics.None(113), new Statistics.None(119), new Statistics.None(120), new Statistics.None(130) };
            Curve = new Statistics.UncertainCurveIncreasing(xs, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        }

        public ExteriorInteriorEditorVM(string name, string description, Statistics.UncertainCurveDataCollection curve):base()
        {
            Name = name;
            Description = description;
            Curve = curve;
        }
        #endregion
        #region Voids
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
