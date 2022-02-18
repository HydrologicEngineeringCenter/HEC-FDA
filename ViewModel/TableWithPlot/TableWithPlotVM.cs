using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using HEC.MVVMFramework.ViewModel.Events;
using HEC.MVVMFramework.ViewModel.Implementations;
using OxyPlot;
using System.Reflection;
using System.Xml.Linq;
using paireddata;

namespace HEC.FDA.ViewModel.TableWithPlot
{
    public class TableWithPlotVM : ValidatingBaseViewModel, MVVMFramework.ViewModel.Interfaces.IUpdatePlot
    {
        #region Backing Fields
        private PlotModel _plotModel;
        private ComputeComponentVM _computeComponentVM;
        //private bool _plotExtended = true;
        //private bool _tableExtended = true;
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
        //public bool PlotExtended
        //{
        //    get { return _plotExtended; }
        //    set { _plotExtended = value;
        //        NotifyPropertyChanged();
        //    }
        //}
        //public bool TableExtended
        //{
        //    get { return _tableExtended; }
        //    set { _tableExtended = value;
        //        NotifyPropertyChanged();
        //    }
        //}


        #region NamedActions
        //private NamedAction _plotExtender;
        //private NamedAction _tableExtender;
        //public NamedAction PlotExtender
        //{
        //    get { return _plotExtender; }
        //    set
        //    {
        //        _plotExtender = value;
        //        NotifyPropertyChanged();
        //    }
        //}
        //public NamedAction TableExtender
        //{
        //    get { return _tableExtender; }
        //    set
        //    {
        //        _tableExtender = value;
        //        NotifyPropertyChanged();
        //    }
        //}
        #endregion

        #endregion

        #region Constructors
        public TableWithPlotVM(ComputeComponentVM computeComponentVM, bool reverseXAxis = false)
        {
            _reverseXAxis = reverseXAxis;
            _computeComponentVM = computeComponentVM;
            Initialize();
        }

        #endregion
        public TableWithPlotVM(XElement ele)
        {
            LoadFromXML(ele);
            Initialize();

        }

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

            //PlotExtender = new NamedAction();
            //PlotExtender.Name = ">";
            //PlotExtender.Action = PlotExtenderAction;
            //TableExtender = new NamedAction();
            //TableExtender.Name= "<";
            //TableExtender.Action = TableExtenderAction;
        }
        //private void PlotExtenderAction(object sender, EventArgs e)
        //{
        //    if(PlotExtender.Name == "<")
        //    {
        //        PlotExtender.Name = ">";
        //    }
        //    else
        //    {
        //        PlotExtender.Name = "<";
        //    }
        //    PlotExtended = !PlotExtended;
        //}
        //private void TableExtenderAction(object sender, EventArgs e)
        //{
        //    if (TableExtender.Name == "<")
        //    {
        //        TableExtender.Name = ">";
        //    }
        //    else
        //    {
        //        TableExtender.Name = "<";
        //    }
        //    TableExtended = !TableExtended;
        //}
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

        private void LoadFromXML(XElement ele)
        {
            //_reverseXAxis = bool.Parse(ele.Attribute("ReverseXAxis").Value);
            //var elements = ele.Descendants();
            //XElement computeCompElement = elements.First();
            //string componentType = (computeCompElement.Attribute("Name").Value);
            //switch (componentType)
            //{
            //    case "Fragility Curve":
            //        _computeComponentVM = new FragilityComponentVM(computeCompElement);
            //        break;
            //    case "Rating Curve":
            //        _computeComponentVM = new ComputeComponentVM(computeCompElement);
            //        break;
            //    case "Graphical Flow Frequency":
            //        _computeComponentVM = new GraphicalVM(computeCompElement);
            //        break;
            //    case "Stage-Damage Curve":
            //        _computeComponentVM = new ComputeComponentVM(computeCompElement);
            //        break;
            //}
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
        #endregion
    }
}
