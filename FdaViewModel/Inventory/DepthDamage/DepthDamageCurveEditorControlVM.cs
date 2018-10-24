using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.DepthDamage
{
    //[Author(q0heccdm, 8 / 15 / 2017 9:18:56 AM)]

    public class CheckForNameConflictEventArgs : EventArgs
    {
        public string Name { get; }
        public CheckForNameConflictEventArgs(string name)
        {
            Name = name;
        }
    }

    public class DepthDamageCurveEditorControlVM : BaseViewModel
    {
        public event EventHandler ThisRowNeedsUpdating;
        public event EventHandler CheckForNameConflict;

        #region Notes
        // Created By: q0heccdm
        // Created Date: 8/15/2017 9:18:56 AM
        #endregion
        #region Fields


        private DepthDamageCurve.DamageTypeEnum _DamageType;
        private string _Name;
        private Statistics.UncertainCurveDataCollection _Curve;
        private string _Description;
        private List<string> _DamageTypeEnums;

        #endregion
        #region Properties
        public string DisplayName
        {
            get;set;
        }
        public DepthDamageCurve.DamageTypeEnum DamageType
        {
            get { return _DamageType; }
            set { _DamageType = value; }
        }
        public List<string> DamageTypeEnums
        {
            get { return _DamageTypeEnums; }
            set { _DamageTypeEnums = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value; }
        }
        #endregion
        #region Constructors
        public DepthDamageCurveEditorControlVM():base()
        {
            
        }
        public DepthDamageCurveEditorControlVM(string displayName, DepthDamageCurve ddc) : base()
        {
            DisplayName = displayName;
            string[] enumValues = Enum.GetNames(typeof(DepthDamageCurve.DamageTypeEnum));
            DamageTypeEnums = enumValues.ToList();

            _Name = ddc.Name;
            _Description = ddc.Description;
            _DamageType = ddc.DamageType;
            _Curve = ddc.Curve;
        }
        #endregion
        #region Voids
        public void CheckWithParentForNameConflict(string name)
        {
            if (this.CheckForNameConflict != null)
            {
                this.CheckForNameConflict(this, new CheckForNameConflictEventArgs(name));
            }
        }
        public void UpdateTheRow()
        {
            if (this.ThisRowNeedsUpdating != null)
            {
                this.ThisRowNeedsUpdating(this, new EventArgs());
            }
        }
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

     
    }
}
