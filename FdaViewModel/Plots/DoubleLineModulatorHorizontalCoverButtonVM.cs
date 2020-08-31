using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    //[Author(q0heccdm, 1 / 19 / 2018 2:06:31 PM)]
    public class DoubleLineModulatorHorizontalCoverButtonVM: BaseViewModel, ICoverButton
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 1/19/2018 2:06:31 PM
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
        public string Text
        {
            get;set;
        }
        #endregion
        #region Constructors
        public DoubleLineModulatorHorizontalCoverButtonVM(string text):base()
        {
            Text = text;
        }
        #endregion
        #region Voids
        public void ButtonClicked()
        {
            this.Clicked?.Invoke(this, new EventArgs());
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
