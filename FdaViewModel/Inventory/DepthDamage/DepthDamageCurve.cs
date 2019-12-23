using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Statistics;

namespace FdaViewModel.Inventory.DepthDamage
{
    //[Author(q0heccdm, 7 / 18 / 2017 9:24:20 AM)]
    public class DepthDamageCurve
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/18/2017 9:24:20 AM
        #endregion
        #region Fields
        public enum DamageTypeEnum : byte {Structural = 0, Content = 1,Vehicle = 2,Other = 3 };

        private DamageTypeEnum _DamageType;
        private string _Name;
        private IFdaFunction _Curve;
        private string _Description;
        

        #endregion
        #region Properties
        public DamageTypeEnum DamageType
        {
            get { return _DamageType; }
            set { _DamageType = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value;}
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public IFdaFunction Curve
        {
            get { return _Curve; }
            set { _Curve = value; }
        }
        #endregion
        #region Constructors
        //public DepthDamageCurve(string name, UncertainCurveIncreasing curve, DamageTypeEnum damageType)
        //{
        //    _Name = name;
        //    _Curve = curve;
        //    _DamageType = damageType;
        //}
        public DepthDamageCurve(string name, string description, IFdaFunction curve, DamageTypeEnum damageType)
        {
            _Name = name;
            _Curve = curve;
            _Description = description;
            _DamageType = damageType;
        }
        #endregion
        #region Voids
       
        #endregion
        #region Functions
        #endregion
    }
}
