using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    //[Author(q0heccdm, 1 / 12 / 2018 9:34:32 AM)]
    public class DoubleLineModulatorCoverButtonVM : BaseViewModel,ICoverButton
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 1/12/2018 9:34:32 AM
        #endregion
        #region Fields
        public event EventHandler Clicked;

        private bool _IsEnabled;
        #endregion
        #region Properties
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; NotifyPropertyChanged(); }
        }
        //public IndividualLinkedPlotControlVM Parent { get; set; }
        #endregion
        #region Constructors
        public DoubleLineModulatorCoverButtonVM():base()
        {
        }
        //public DoubleLineModulatorCoverButtonVM(IndividualLinkedPlotControlVM parent)
        //{
        //    Parent = parent;
        //}
        #endregion
        #region Voids
        public void ButtonClicked()
        {  
            if (this.Clicked != null)
            {
                this.Clicked(this, new EventArgs());
            }
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

       
        #endregion
        #region Functions
        #endregion

    }
}
