using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using HEC.MVVMFramework.ViewModel.Events;
using HEC.MVVMFramework.ViewModel.Implementations;
using OxyPlot;
using System.Reflection;
using System.Xml.Linq;
using paireddata;
using System;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.Collections;

namespace HEC.FDA.ViewModel.TableWithPlot
{
    public class TableWithPlotVM : ValidatingBaseViewModel, MVVMFramework.ViewModel.Interfaces.IUpdatePlot
    {
        public event EventHandler WasModified;

        #region Backing Fields
        private PlotModel _plotModel;
        private ComputeComponentVM _computeComponentVM;
        private bool _plotExtended = true;
        private bool _tableExtended = true;
        public event UpdatePlotEventHandler UpdatePlotEvent;
        private bool _reverseXAxis;
        #endregion

        #region Properties
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set
            {
                _plotModel = value;
                NotifyPropertyChanged();
            }
        }
        public ComputeComponentVM ComputeComponentVM
        {
            get { return _computeComponentVM; }
        }
        public bool PlotExtended
        {
            get { return _plotExtended; }
            set
            {
                _plotExtended = value;
                NotifyPropertyChanged();
            }
        }
        public bool TableExtended
        {
            get { return _tableExtended; }
            set
            {
                _tableExtended = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public TableWithPlotVM(ComputeComponentVM computeComponentVM, bool reverseXAxis = false)
        {
            _reverseXAxis = reverseXAxis;
            _computeComponentVM = computeComponentVM;
            Initialize();
        }

        #endregion

        #region Methods
        private void Initialize()
        {
            _computeComponentVM.PropertyChanged += _computeComponentVM_PropertyChanged;
            _plotModel = new PlotModel();
            _plotModel.Title = _computeComponentVM.Name;
            _plotModel.LegendPosition = LegendPosition.BottomRight;

            if (_reverseXAxis)
            {
                OxyPlot.Axes.LinearAxis x = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Bottom,
                    StartPosition = 1,
                    EndPosition = 0
                };
                _plotModel.Axes.Add(x);
            }

            SelectedItemToPlotModel();
            AddHandlers();
        }
        
        public XElement ToXML()
        {
            XElement ele = new XElement(this.GetType().Name);
            ele.SetAttributeValue("ReverseXAxis", _reverseXAxis);
            ele.Add(ComputeComponentVM.ToXML());
            return ele;
        }

        public UncertainPairedData GetUncertainPairedData()
        {
            return ComputeComponentVM.SelectedItem.ToUncertainPairedData(ComputeComponentVM.XLabel, ComputeComponentVM.YLabel, ComputeComponentVM.Name, ComputeComponentVM.Description, "testCategory?");
        }

        private void AddHandlers() //Make sure new rows get added to this.
        {
            foreach (IDataProvider idp in _computeComponentVM.Options)
            {
                idp.Data.CollectionChanged += Data_CollectionChanged;
                foreach (SequentialRow row in idp.Data)
                {
                    row.PropertyChanged += Row_PropertyChanged;
                }
            }
        }
        private void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (SequentialRow item in e.OldItems)
                {
                    item.PropertyChanged -= Row_PropertyChanged;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (SequentialRow item in e.NewItems)
                {
                    item.PropertyChanged += Row_PropertyChanged;
                }
            }
            _plotModel.InvalidatePlot(true);
        }

        private void Row_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _plotModel.InvalidatePlot(true);
            WasModified?.Invoke(this, e);
        }

        private void SelectedItemToPlotModel()
        {
            _plotModel.Series.Clear();
            PropertyInfo[] pilist = _computeComponentVM.SelectedItem.Data[0].GetType().GetProperties();
            foreach (PropertyInfo pi in pilist)
            {
                DisplayAsLineAttribute dispAsLineAttribute = (DisplayAsLineAttribute)pi.GetCustomAttribute(typeof(DisplayAsLineAttribute));
                if (dispAsLineAttribute != null)
                {
                    OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries();
                    lineSeries.Title = dispAsLineAttribute.DisplayName;
                    switch (dispAsLineAttribute.Color)
                    {
                        case Enumerables.ColorEnum.Black:
                            lineSeries.Color = OxyColors.Black;//OxyColor.FromRgb(0, 0, 0);
                            break;
                        case Enumerables.ColorEnum.Blue:
                            lineSeries.Color = OxyColors.Blue;// OxyColor.FromRgb(0, 0, 255);
                            break;
                        case Enumerables.ColorEnum.Red:
                            lineSeries.Color = OxyColors.Red;//OxyColor.FromRgb(255, 0, 0);
                            break;
                        default:
                            break;
                    }
                    if (dispAsLineAttribute.Dashed)
                    {
                        lineSeries.LineStyle = LineStyle.Dash;
                    }
                    lineSeries.DataFieldX = "X";
                    lineSeries.DataFieldY = pi.Name;
                    lineSeries.ItemsSource = _computeComponentVM.SelectedItem.Data;
                    _plotModel.Series.Add(lineSeries);
                }
            }
            _plotModel.InvalidatePlot(true);
        }

        private void _computeComponentVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_computeComponentVM.SelectedItem))
            {
                SelectedItemToPlotModel();
                WasModified?.Invoke(sender, e);
            }
        }

        public void InvalidatePlotModel(bool updateData)
        {
            PlotModel.InvalidatePlot(updateData);
        }

        public void UpdatePlot(object sender, UpdatePlotEventArgs e)
        {
            UpdatePlotEvent?.Invoke(sender, e);
        }

        public FdaValidationResult GetTableErrors()
        {
            FdaValidationResult vr = new FdaValidationResult();
            int i = 0;
            foreach (object row in _computeComponentVM.SelectedItem.Data)
            {
                i++;
                if (row is SequentialRow sequentialRow)
                {
                    sequentialRow.Validate();
                    if (sequentialRow.HasErrors)
                    {
                        vr.AddErrorMessage("Errors in row: " + i);
                        IEnumerable enumerable = sequentialRow.GetErrors();
                        List<string> errors = enumerable as List<string>;
                        vr.AddErrorMessages(errors);
                        vr.AddErrorMessage("\n");
                    }
                }
            }
            return vr;
        }

        #endregion
    }
}
