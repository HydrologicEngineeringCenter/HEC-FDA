using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 8 / 8 / 2017 1:35:28 PM)]
    public class OccupancyTypesGroupRowItemVM : BaseViewModel
    {

        public event EventHandler UpdateTheListOfRows;

        #region Notes
        // Created By: q0heccdm
        // Created Date: 8/8/2017 1:35:28 PM
        #endregion
        #region Fields
        private string _Name;
        private string _Path;
        //private OccupancyTypesElement _OccupancyTypeGroup;
        private List<IOccupancyType> _ListOfOccTypes;
        private int _NumberOfOccTypes;
        #endregion
        #region Properties
        public string Path
        {
            get { return System.IO.Path.GetFileName(_Path); }
            set { _Path = value; NotifyPropertyChanged(); }
        }
        //public string Name
        //{
        //    get { return _Name; }
        //    set { _Name = value; NotifyPropertyChanged(); }
        //}

        public int NumberOfOccTypes
        {
            get { return _NumberOfOccTypes; }
            set { _NumberOfOccTypes = value; NotifyPropertyChanged(); }
        }

        public List<DepthDamage.DepthDamageCurve> ListOfDepthDamageCurves
        {
            get;set;
        }

        public List<IOccupancyType> ListOfOccTypes
        {
            get { return _ListOfOccTypes; }
            set { _ListOfOccTypes = value; NotifyPropertyChanged(); }
        }


        //public OccupancyTypesElement OccupancyTypeGroup
        //{
        //    get { return _OccupancyTypeGroup; }
        //    set { _OccupancyTypeGroup = value; NotifyPropertyChanged(); }
        //}

        #endregion
        #region Constructors
        public OccupancyTypesGroupRowItemVM():base()
        {

        }
        public OccupancyTypesGroupRowItemVM(string path, string name, List<IOccupancyType> listOfOccTypes,List<DepthDamage.DepthDamageCurve> listOfDepthDamageCurves) : base()
        {

            Name = name;
            Path = path;
            ListOfOccTypes = listOfOccTypes;
            ListOfDepthDamageCurves = listOfDepthDamageCurves;
            NumberOfOccTypes = ListOfOccTypes.Count();
            //OccupancyTypeGroup = new OccupancyTypesElement(ots.OccupancyTypes);

        }
        #endregion
        #region Voids
        public void UpdateTheRows()
        {
            if (this.UpdateTheListOfRows != null)
            {
                this.UpdateTheListOfRows(this, new EventArgs());
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
