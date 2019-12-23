using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    //[Author(q0heccdm, 12 / 19 / 2017 1:35:38 PM)]
    public class IndividualLinkedPlotCoverButtonVM : BaseViewModel,ICoverButton
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/19/2017 1:35:38 PM
        #endregion
        #region Fields
        public event EventHandler Clicked;

        private bool _IsEnabled;
        #endregion
        #region Properties
        public string Content { get; set; }
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; NotifyPropertyChanged(); }
        }
            //public IndividualLinkedPlotControlVM Parent { get; set; }
        #endregion
        #region Constructors
        public IndividualLinkedPlotCoverButtonVM(string buttonContent):base()
        {
            Content = buttonContent;
        }
        //public IndividualLinkedPlotCoverButtonVM(IndividualLinkedPlotControlVM parent)
        //{
        //    Parent = parent;
        //}

        #endregion
        #region Voids
        public void ButtonClicked()
        {
            //raise event?
            //or pass the parent in and use that
            if (this.Clicked != null)
            {
                this.Clicked(this, new EventArgs());
            }
        }

        public override void AddValidationRules()
        {
            // throw new NotImplementedException();
        }

    
        #endregion
        #region Functions
        #endregion

    }
}
