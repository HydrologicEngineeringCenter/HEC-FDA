using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Events;
using HEC.MVVMFramework.ViewModel.Implementations;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.TableWithPlot
{
    public class TableWithPlotVM : ValidatingBaseViewModel, MVVMFramework.ViewModel.Interfaces.IUpdatePlot
    {
        public event EventHandler WasModified;

        #region Backing Fields
        private ViewResolvingPlotModel _plotModel = new ViewResolvingPlotModel();
        private CurveComponentVM _curveComponentVM;
        private bool _plotExtended = true;
        private bool _tableExtended = true;
        public event UpdatePlotEventHandler UpdatePlotEvent;
        private bool _reverseXAxis;
        #endregion

        #region Properties
        public bool ReverseXAxis
        {
            get { return _reverseXAxis; }
        }
        public PlotModel PlotModel
        {
            get { return _plotModel; }
        }
        public CurveComponentVM CurveComponentVM
        {
            get { return _curveComponentVM; }
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
        public TableWithPlotVM(CurveComponentVM curveComponentVM, bool reverseXAxis = false)
        {
            _reverseXAxis = reverseXAxis;
            _curveComponentVM = curveComponentVM;
            Initialize();
        }
        public TableWithPlotVM(XElement ele)
        {
            LoadFromXML(ele);
            Initialize();

        }
        #endregion

        #region Methods
        private void Initialize()
        {
            _curveComponentVM.PropertyChanged += _curveComponentVM_PropertyChanged;
            InitPlotModel(_plotModel);
            SelectedItemToPlotModel();

            AddHandlers();
        }

        public void InitModel()
        {
            _plotModel = new ViewResolvingPlotModel();
            InitPlotModel(_plotModel);
            SelectedItemToPlotModel();
        }

        private void InitPlotModel(PlotModel plotModel)
        {
            plotModel.Title = _curveComponentVM.Name;
            
            Legend legend = new Legend();
            legend.LegendPosition= LegendPosition.BottomRight;
            plotModel.Legends.Add(legend);

            if (_reverseXAxis)
            {
                LinearAxis x = new LinearAxis()
                {
                    Position = AxisPosition.Bottom,
                    StartPosition = 1,
                    EndPosition = 0
                };
                plotModel.Axes.Add(x);
            }
        }

        public XElement ToXML()
        {
            XElement ele = new XElement(this.GetType().Name);
            ele.SetAttributeValue(nameof(ReverseXAxis), ReverseXAxis);
            ele.Add(CurveComponentVM.ToXML());
            return ele;
        }
        private void LoadFromXML(XElement ele)
        {
            _reverseXAxis = bool.Parse(ele.Attribute(nameof(ReverseXAxis)).Value);
            var elements = ele.Descendants();
            XElement computeCompElement = elements.First();
            string componentType = computeCompElement.Name.ToString();
            if(componentType == "GraphicalVM")
            {
                    _curveComponentVM = new GraphicalVM(computeCompElement);
            }
        }
        public UncertainPairedData GetUncertainPairedData()
        {
            return CurveComponentVM.SelectedItem.ToUncertainPairedData(CurveComponentVM.XLabel, CurveComponentVM.YLabel, CurveComponentVM.Name, CurveComponentVM.Description, "testCategory?");
        }

        private void AddHandlers() //Make sure new rows get added to this.
        {
            foreach (IDataProvider idp in _curveComponentVM.Options)
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
            PropertyInfo[] pilist = _curveComponentVM.SelectedItem.Data[0].GetType().GetProperties();
            foreach (PropertyInfo pi in pilist)
            {
                DisplayAsLineAttribute dispAsLineAttribute = (DisplayAsLineAttribute)pi.GetCustomAttribute(typeof(DisplayAsLineAttribute));
                if (dispAsLineAttribute != null)
                {
                    LineSeries lineSeries = new LineSeries();
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
                    lineSeries.ItemsSource = _curveComponentVM.SelectedItem.Data;
                    _plotModel.Series.Add(lineSeries);
                }
            }
            _plotModel.InvalidatePlot(true);
        }

        private void _curveComponentVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_curveComponentVM.SelectedItem))
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
            foreach (object row in _curveComponentVM.SelectedItem.Data)
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
