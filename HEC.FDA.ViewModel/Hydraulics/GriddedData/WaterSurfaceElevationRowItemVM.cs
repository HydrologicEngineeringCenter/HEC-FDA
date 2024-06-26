﻿using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Hydraulics.GriddedData
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:32:06 AM)]
    public class WaterSurfaceElevationRowItemVM:NameValidatingVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:32:06 AM
        #endregion
        #region Fields
        private double _Probability;
        private double _ReturnYear;
        #endregion
        #region Properties
        public bool IsEnabled { get; }
        /// <summary>
        /// full, absolute path to the file
        /// </summary>
        public string Path { get; set; }
        public double Probability
        {
            get { return _Probability; }
            set { _Probability = value; _ReturnYear = 1 / _Probability; NotifyPropertyChanged();NotifyPropertyChanged("ReturnYear"); }
        }
        public double ReturnYear
        {
            get { return _ReturnYear; }
            set { _ReturnYear = value; _Probability = 1 / value; NotifyPropertyChanged();NotifyPropertyChanged("Probability"); }
        }
        #endregion
        #region Constructors
        /// <param name="name"> The name visible to the UI</param>
        /// <param name="path"> The absolute path to the file </param>
        public WaterSurfaceElevationRowItemVM(string name, string path, double probability, bool isEnabled)
        {
            IsEnabled = isEnabled;
            Path = path;
            ReturnYear = 1 / probability;
            Name = name;
            Probability = probability;
        }
        #endregion

        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (Probability <= 0 || Probability >= 1)
            {
                vr.AddErrorMessage("Probability value in row '" + Name + "' has to be between 0 and 1.");
            }
            return vr;
        }
    }
}
