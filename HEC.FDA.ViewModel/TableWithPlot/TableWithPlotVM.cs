using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using HEC.FDA.ViewModel.TableWithPlot.Enumerables;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Events;
using HEC.MVVMFramework.ViewModel.Implementations;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Statistics.Distributions;
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
        private bool _normalXAxis = false;
        private bool _logYAxis = false;
        #endregion

        #region Properties
        public bool NormalXAxis
        {
            get { return _normalXAxis; }
        }
        public bool LogYAxis
        {
            get { return _logYAxis; }
        }
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
        public TableWithPlotVM(CurveComponentVM curveComponentVM, bool reverseXAxis = false, bool normalXAxis = false, bool logYAxis = false)
        {
            _reverseXAxis = reverseXAxis;
            _normalXAxis = normalXAxis;
            _logYAxis = logYAxis;
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
            InitPlotModel();
            AddHandlers();
        }
        private void InitPlotModel()
        {
            _plotModel = new ViewResolvingPlotModel();
            _plotModel.Title = _curveComponentVM.Name;
            AddLegend();
            if (_reverseXAxis)
            {
                AddAxes();
            }
            SelectedItemToPlotModel();
        }

        private void AddLegend()
        {
            Legend legend = new Legend();
            legend.LegendPosition = LegendPosition.BottomRight;
            _plotModel.Legends.Add(legend);
        }

        private void AddAxes()
        {
            AddXAxis();
            AddYAxis();
        }

        private void AddYAxis()
        {
            Axis y;
            if (_logYAxis)
            {
                y = new LogarithmicAxis();
            }
            else
            {
                y = new LinearAxis();
            }
            y.Position = AxisPosition.Left;
        }

        private void AddXAxis()
        {
            LinearAxis x = new LinearAxis();
            x.Position = AxisPosition.Bottom;
            if (_reverseXAxis)
            {
                x.StartPosition = 1;
                x.EndPosition = 0;
            }
            if (_normalXAxis)
            {
                x.LabelFormatter = _probabilityFormatter;
            }
            _plotModel.Axes.Add(x);
        }

        private static string _probabilityFormatter(double d)
        {
            Normal standardNormal = new Normal(0, 1);
            double value = standardNormal.CDF(d);
            string stringval = value.ToString("0.0000");
            return stringval;
        }

        public XElement ToXML()
        {
            XElement ele = new XElement(this.GetType().Name);
            ele.SetAttributeValue(nameof(ReverseXAxis), ReverseXAxis);
            ele.SetAttributeValue(nameof(LogYAxis), LogYAxis);
            ele.SetAttributeValue(nameof(NormalXAxis), NormalXAxis);
            ele.Add(CurveComponentVM.ToXML());
            return ele;
        }
        private void LoadFromXML(XElement ele)
        {
            _reverseXAxis = bool.Parse(ele.Attribute(nameof(ReverseXAxis)).Value);
            _logYAxis = bool.Parse(ele.Attribute(nameof(LogYAxis)).Value);
            _normalXAxis = bool.Parse(ele.Attribute(nameof(NormalXAxis)).Value);
            var elements = ele.Descendants();
            XElement computeCompElement = elements.First();
            string componentType = computeCompElement.Name.ToString();
            if (componentType == "GraphicalVM")
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
                    string yPropertyName = pi.Name;
                    string displayName = dispAsLineAttribute.DisplayName;
                    bool dashed = dispAsLineAttribute.Dashed;
                    ColorEnum color = dispAsLineAttribute.Color;
                    AddLineSeries(displayName, color, yPropertyName, dashed);
                }
            }
            _plotModel.InvalidatePlot(true);
        }

        private void AddLineSeries(string displayName, ColorEnum color, string yPropertyName, bool dashed)
        {
            LineSeries lineSeries = new LineSeries();
            lineSeries.Title = displayName;
            switch (color)
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
            if (dashed)
            {
                lineSeries.LineStyle = LineStyle.Dash;
            }
            if (NormalXAxis)
            {
                lineSeries.DataFieldX = "ZScore";
                lineSeries.TrackerFormatString = "X: {X:0.####}, Y: {4:F2} ";
                lineSeries.CanTrackerInterpolatePoints = false;

            }
            else
            {
                lineSeries.DataFieldX = "X";
            }

            lineSeries.DataFieldY = yPropertyName;
            lineSeries.ItemsSource = _curveComponentVM.SelectedItem.Data;
            _plotModel.Series.Add(lineSeries);
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
