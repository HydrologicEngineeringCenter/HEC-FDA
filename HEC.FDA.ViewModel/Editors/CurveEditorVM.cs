﻿using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.Editors
{
    public abstract class CurveEditorVM : BaseEditorVM
    {
        private TableWithPlotVM _TableWithPlot;

        #region properties
        public TableWithPlotVM TableWithPlot
        {
            get { return _TableWithPlot; }
            set { _TableWithPlot = value; NotifyPropertyChanged(); }
        }    
        #endregion

        #region constructors
        public CurveEditorVM(CurveComponentVM defaultCurve, EditorActionManager actionManager) :base(actionManager)
        {
            TableWithPlot = new TableWithPlotVM(defaultCurve);
            TableWithPlot.WasModified += TableDataChanged;
        }

        public CurveEditorVM(CurveChildElement elem, EditorActionManager actionManager) :base(elem, actionManager)
        {
            CurveComponentVM comp = new CurveComponentVM( elem.CurveComponentVM.ToXML());
            TableWithPlot = new TableWithPlotVM(comp);
            TableWithPlot.WasModified += TableDataChanged;
        }

        private void TableDataChanged(object sender, EventArgs e)
        {
            HasChanges = true;
        }

        public void UpdateCurveMetaData()
        {
            _TableWithPlot.CurveComponentVM.Name = Name;
        }
        #endregion
    }
}
