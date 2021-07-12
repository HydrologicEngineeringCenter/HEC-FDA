using Functions;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using FunctionsView.Validation;
using FunctionsView.View;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Utilities;

namespace FunctionsView.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// It would be nice if we could copy and paste all the columns including the distribution
    /// and the interpolator.
    /// </remarks>
    public class CoordinatesFunctionEditorVM : IValidate<CoordinatesFunctionEditorVM>
    {
        private string CHART_TITLE = "test title";
        public event EventHandler UpdateView;
        public event EventHandler TableChanged;
        private ICoordinatesFunction _Function;
        private IEnumerable<IMessage> _Messages = new List<IMessage>();
        private bool _IsChartInError;
        private string _XLabel;
        public string XLabel { get; }
        public string YLabel { get; }
        public string ChartTitle { get; }
        public bool HasLoaded { get; set; }
        public bool IsChartInError
        {
            get
            { return _IsChartInError;  }
            set
            { _IsChartInError = value;  }
        }

        private SciChart2DChartViewModel _chartViewModel = new SciChart2DChartViewModel("chartId");
        public SciChart2DChartViewModel CoordinatesChartViewModel
        {
            get { return _chartViewModel; }
            set { _chartViewModel = value; }
        }
        public ICoordinatesFunction Function
        {
            get { return _Function; }
            set { _Function = value; CreateTables(_Function);  }
        }
        public List<CoordinatesFunctionTableVM> Tables
        {
            get;
            set;
        }


        public CoordinatesFunctionEditorVM()
        {
            Tables = new List<CoordinatesFunctionTableVM>();
        }
        public CoordinatesFunctionEditorVM(ICoordinatesFunction function, string xLabel, string yLabel, string chartTitle)
        {
            
            XLabel = xLabel;
            YLabel = yLabel;
            ChartTitle = chartTitle;
            _Function = function;
            Tables = new List<CoordinatesFunctionTableVM>();
            CreateTables(function);
            UpdateChartViewModel();
            
        }

        /// <summary>
        /// This just happens one time when opening the editor.
        /// </summary>
        private void SetChartViewModel()
        {
            ICoordinatesFunction func = CreateFunctionFromTables();
            if (func.IsLinkedFunction)
            {
                List<ICoordinatesFunction> functions = ((CoordinatesFunctionLinked)func).Functions;
            }
        }

        public void UpdateChartViewModel()
        {
            //CoordinatesChartViewModel.AxisViewModel.ShowMajorGridLines = false;
            //CoordinatesChartViewModel.AxisViewModel.ShowMinorGridLines = false;

            try
            {
                CoordinatesFunctionEditorChartHelper chartHelper = new CoordinatesFunctionEditorChartHelper(CreateFunctionFromTables(), XLabel, YLabel);
                List<SciLineData> lineData = chartHelper.CreateLineData();
                CoordinatesChartViewModel.LineData.Set(lineData);

                //UpdateView?.Invoke(this, new EventArgs());
                
                
                //ICoordinatesFunction func = CreateFunctionFromTables();
                //if(func.IsLinkedFunction)
                //{
                //    List<ICoordinatesFunction> functions = ((CoordinatesFunctionLinked)func).Functions;
                //}

                //var x = new double[func.Coordinates.Count];
                //var y_05 = new double[func.Coordinates.Count];
                //var y_50 = new double[func.Coordinates.Count];
                //var y_95 = new double[func.Coordinates.Count];
                //for (int i = 0; i < func.Coordinates.Count; i++)
                //{
                //    ICoordinate coord = func.Coordinates[i];
                //    x[i] = coord.X.Value();
                //    y_05[i] = coord.Y.Value(.05);
                //    y_50[i] = coord.Y.Value(.5);
                //    y_95[i] = coord.Y.Value(.95);
                //}
                ////var lineData05To50 = InitXyyLineData(x, y_05, y_50,"5% to 50%");
                ////var lineData50To95 = InitXyyLineData(x, y_50, y_95, "50% to 95%");

                ////lineData50To95.StrokeColorY1 = lineData05To50.StrokeColor;
                ////lineData50To95.StrokeColor = lineData05To50.StrokeColorY1;

                //NumericLineData ninetyFivePercent = new NumericLineData();
                //NumericLineData fiftyPercent = new NumericLineData();
                //NumericLineData fivePercent = new NumericLineData();

                //switch (func.Interpolator)
                //{
                //    case InterpolationEnum.Linear:
                //        {

                //            ninetyFivePercent = new NumericLineData(x, y_95, ChartTitle, "95%", XLabel, YLabel, PlotType.Line);
                //            fiftyPercent = new NumericLineData(x, y_50, ChartTitle, "50%", XLabel, YLabel, PlotType.Line);
                //            fivePercent = new NumericLineData(x, y_05, ChartTitle, "5%", XLabel, YLabel, PlotType.Line);
                //            break;
                //        }
                //    case InterpolationEnum.Piecewise:
                //        {
                //            ninetyFivePercent = new NumericLineData(x, y_95, ChartTitle, "95%", XLabel, YLabel, PlotType.Line);
                //            ninetyFivePercent.UseDigitalLine = true;
                //            fiftyPercent = new NumericLineData(x, y_50, ChartTitle, "50%", XLabel, YLabel, PlotType.Line);
                //            fiftyPercent.UseDigitalLine = true;
                //            fivePercent = new NumericLineData(x, y_05, ChartTitle, "5%", XLabel, YLabel, PlotType.Line);
                //            fivePercent.UseDigitalLine = true;
                //            break;
                //        }
                //    case InterpolationEnum.None:
                //        {
                //            ninetyFivePercent = new NumericLineData(x, y_95, ChartTitle, "95%", XLabel, YLabel, PlotType.Scatter)
                //            {
                //                StrokeThickness = 5.0,
                //                Sorted = false,
                //                AntiAliasing = true,
                //                //PaletteProvider = new NoneLinePaletteProvider(),
                //                StrokeColor = Colors.Black,
                //                SymbolSize = 8.0,
                //                SymbolType = SymbolType.Triangle,
                //                SymbolFillColor = Colors.Red,
                //                SymbolStrokeColor = Colors.Purple,
                //                SymbolStrokeThickness = 1,
                //            };
                //            fiftyPercent = new NumericLineData(x, y_50, ChartTitle, "50%", XLabel, YLabel, PlotType.Scatter)
                //            {
                //                StrokeThickness = 5.0,
                //                Sorted = false,
                //                AntiAliasing = true,
                //                //PaletteProvider = new NoneLinePaletteProvider(),
                //                StrokeColor = Colors.Black,
                //                SymbolSize = 8.0,
                //                SymbolType = SymbolType.Circle,
                //                SymbolFillColor = Colors.Black,
                //                SymbolStrokeColor = Colors.Black,
                //                SymbolStrokeThickness = 1,
                //            };
                //            fivePercent = new NumericLineData(x, y_05, ChartTitle, "5%", XLabel, YLabel, PlotType.Scatter)
                //            {
                //                StrokeThickness = 5.0,
                //                Sorted = false,
                //                AntiAliasing = true,
                //                //PaletteProvider = new NoneLinePaletteProvider(),
                //                StrokeColor = Colors.Black,
                //                SymbolSize = 8.0,
                //                SymbolType = SymbolType.Square,
                //                SymbolFillColor = Colors.Blue,
                //                SymbolStrokeColor = Colors.Orange,
                //                SymbolStrokeThickness = 1,
                //            };

                //            break;
                //        }
                //}

                
                ////ninetyFivePercent.XData[3] = 54.0;
                ////ninetyFivePercent.Refresh();

                //// List<XyySciLineData2D<double, double>> lineData = new List<XyySciLineData2D<double, double>>() { lineData1, lineData2 };

                //List<SciLineData> lineData = new List<SciLineData>() { ninetyFivePercent, fiftyPercent,  fivePercent, };
                ////lineData.Add(fivePercent);
                //// lineData.Add(fiftyPercent);
                //// lineData.Add(ninetyFivePercent);
                //CoordinatesChartViewModel.LineData.Set(lineData);
            }
            catch (Exception ex)
            {
                CoordinatesChartViewModel.LineData.Set(new List<SciLineData>());

            }
        }

        //I need to convert the x,y data from all the lines and flip them to go vertical separated by double.nan
        //so the y data will be (y5%, y50%, y95%, double.nan
        //and the x data will be (x1,  x1,    x1, double.nan, x2, x2, x2
        private NumericLineData CreateNoneData(double[] xs, List<double[]> ys)
        {
            //all the lines should have the same number of xs and ys
            foreach (double[] yvals in ys)
            {
                if (xs.Length != yvals.Length)
                {
                    return new NumericLineData();
                }
            }

            int numLines = ys.Count();
            List<double> noneYs = new List<double>();
            List<double> noneXs = new List<double>();
            for (int j = 0; j < xs.Length; j++)
            {
                for (int i = 0; i < numLines; i++)
                {
                    noneXs.Add(xs[j]);
                    noneYs.Add(ys[i][j]);
                }
                //add the double.nan
                noneXs.Add(double.NaN);
                noneYs.Add(double.NaN);
            }

            return new NumericLineData(noneXs.ToArray(), noneYs.ToArray(), ChartTitle, "None", XLabel, YLabel, PlotType.Line)
            {
                StrokeThickness = 5.0,
                Sorted = false,
                AntiAliasing = true,
                //PaletteProvider = new NoneLinePaletteProvider(),
                StrokeColor = Colors.GreenYellow,
                SymbolSize = 5.0,
                SymbolType = SymbolType.Circle,
                SymbolFillColor = Colors.Black,
                SymbolStrokeColor = Colors.Black,
                SymbolStrokeThickness = 1,
            };

        }

        //private NumericLineData CreateNoneLineData(double[] x, double[] y)
        //{
        //    return new NumericLineData(x, y, chartName, seriesName, xLabel, yLabel, PlotType.Line)
        //    {
        //        StrokeThickness = 5.0,
        //        Sorted = false,
        //        AntiAliasing = true,
        //        //PaletteProvider = new NoneLinePaletteProvider(),
        //        StrokeColor = Colors.GreenYellow,
        //        SymbolSize = 5.0,
        //        SymbolType = SymbolType.Circle,
        //        SymbolFillColor = Colors.Black,
        //        SymbolStrokeColor = Colors.Black,
        //        SymbolStrokeThickness = 1,
        //    };
        //}

        private XyySciLineData2D<double, double> InitXyyLineData(double[] xArray, double[] yArray, double[] y1Array, string seriesName)
        {
            return new XyySciLineData2D<double, double>(xArray, yArray, y1Array, CHART_TITLE, seriesName, "xAxisName", "yAxisName")
            {
                Sorted = false,
                ChartTitle = CHART_TITLE,
                ChartName = CHART_TITLE,
                StrokeColor = Colors.Black,
                StrokeThickness = 1.0,
                StrokeColorY1 = Colors.Red,
                AntiAliasing = true,
                FillColor = Color.FromArgb(0x55, 0xFF, 0x00, 0x00),
                FillColorY1 = Color.FromArgb(0x55, 0xAD, 0xFF, 0x2F),
            };
        }

        //we can call this one or the one commented out above. They are probably about the same amount of 
        //time to process.
        //public void UpdateChartViewModel()
        //{
        //    try
        //    {
        //        List<SciLineData> lineData = new List<SciLineData>();

        //        foreach (CoordinatesFunctionTableVM table in Tables)
        //        {
        //            lineData.Add(GetLineDataForTable(table,.05, "5%"));
        //            lineData.Add(GetLineDataForTable(table, .5, "50%"));
        //            lineData.Add(GetLineDataForTable(table, .95, "95%"));

        //        }
        //        CoordinatesChartViewModel.LineData.Set(lineData);
        //    }
        //    catch(Exception ex)
        //    {
        //        //don't do anything? we were unable to create valid coordinates
        //        CoordinatesChartViewModel.LineData.Set(new List<SciLineData>());
        //    }
        //}

        //private SciLineData GetLineDataForTable(CoordinatesFunctionTableVM table, double probability, string seriesName)
        //{
        //    List<SciLineData> lineData = new List<SciLineData>();
        //    double[] xs = new double[table.Rows.Count];
        //    double[] ys = new double[table.Rows.Count];
        //    for(int i = 0;i<table.Rows.Count;i++)
        //    //foreach (CoordinatesFunctionRowItem row in table.Rows)
        //    {
        //        CoordinatesFunctionRowItem row = table.Rows[i];
        //        ICoordinate coord = row.CreateCoordinateFromRow();
        //        xs[i] =coord.X.Value();
        //        ys[i] = coord.Y.Value(probability);
        //    }
        //        return new NumericLineData(xs,ys,"chartName", seriesName,"xAxisName", "yAxisName",PlotType.Line) ;
        //}

        public bool IsValid
        {
            get
            {
                bool isValid = Validate(new CoordinatesFunctionEditorVMValidator(), out IEnumerable<IMessage> errors);
                _Messages = errors;
                return isValid;
            }
        }

        public IEnumerable<IMessage> Messages => _Messages;

        public IMessageLevels State => throw new NotImplementedException();

        //public ICoordinatesFunction Function
        //{
        //get { return (ICoordinatesFunction)this.GetValue(FunctionProperty); }
        //set { this.SetValue(FunctionProperty, value); }
        // }



        /// <summary>
        /// This is used when the editor is opened to load the tables from a coordinates function.
        /// </summary>
        /// <param name="function"></param>
        private void CreateTables(ICoordinatesFunction function)
        {
            List<CoordinatesFunctionRowItem> rows = CreateRows(function);
            //foreach (CoordinatesFunctionRowItem row in rows)
            //{
            //    row.StructureChanged += Row_StructureChanged;
            //}
            CreateTables(rows);
        }

        public void Save()
        {
            //Function = CreateFunctionFromTables();
        }

        public ICoordinatesFunction CreateFunctionFromTables()
        {
            if (Tables.Count == 1)
            {
                //if there is only one table then we know that this is not
                //a linked coordinates function.
                return Tables[0].CreateCoordinatesFunctionFromTable();
            }
            else
            {
                return CreateLinkedFunction();
            }
        }

        private ICoordinatesFunction CreateLinkedFunction()
        {
            //the interpolators between tables is the same as the interpolator of the last row
            //of the previous table. Since any rows in a table all have the same interpolator
            //i can grab the interpolator from any row in the table.
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();
            foreach (CoordinatesFunctionTableVM table in Tables)
            {
                functions.Add(table.CreateCoordinatesFunctionFromTable());
            }
            return ICoordinatesFunctionsFactory.Factory(functions);
        }

        /// <summary>
        /// This is called anytime an visual update needs to happen, such as changing distribution types.
        /// </summary>
        /// <param name="rowItems"></param>
        private void CreateTables(List<CoordinatesFunctionRowItem> rowItems)
        {           
            //we are about to create the tables, so we need to find out what kinds of data we are dealing with
            //ColumnWidths.TableTypes tableType = ColumnWidths.GetTableTypeForRows(rowItems);
            //it would be nice to just bind the items source of the list to be the list of tables
            //but i have to put the topper and bottom UI pieces in. I could do them outside the list
            //but that seemed harder to get it to line up etc.
            //lst_tables.Items.Clear();

            Tables.Clear();

            if (rowItems.Count > 0)
            {
                //lst_tables.Items.Add(new TableTopControl(tableType));

                ObservableCollection<CoordinatesFunctionRowItem> rows = new ObservableCollection<CoordinatesFunctionRowItem>();
                IOrdinateEnum distType = rowItems[0].SelectedDistributionType;
                InterpolationEnum interpType = rowItems[0].SelectedInterpolationType;
                rows.Add(rowItems[0]);
                for (int i = 1; i < rowItems.Count; i++)
                //while (i < _Rows.Count)
                {
                    CoordinatesFunctionRowItem row = rowItems[i];
                    if (row.SelectedDistributionType.Equals(distType) && row.SelectedInterpolationType.Equals(interpType))
                    {
                        rows.Add(rowItems[i]);
                    }
                    else
                    {
                        //the dist type changed
                        CreateTable(rows);
                        //set the new dist type and add it to the list
                        distType = row.SelectedDistributionType;
                        interpType = row.SelectedInterpolationType;
                        rows = new ObservableCollection<CoordinatesFunctionRowItem>();
                        rows.Add(row);
                    }

                }
                //need to create the final table
                CreateTable(rows);
                //UpdateView?.Invoke(this, new EventArgs());
                //lst_tables.Items.Add(new TableBottomControl(tableType));
            }
        }

        private void UpdateTables()
        {
            List<CoordinatesFunctionRowItem> rows = GetAllRowsFromAllTables();
            CreateTables(rows);
        }

        private List<CoordinatesFunctionRowItem> GetAllRowsFromAllTables()
        {
            //These rows already have tables listening to the row is leaving event. I need to clear those
            List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
            foreach(CoordinatesFunctionTableVM table in Tables)
            {
                rows.AddRange(table.Rows);
            }
            return rows;
        }
        private void Table_RowIsLeaving(CoordinatesFunctionTableVM.RowLeavingEventArgs args)
        {
            CoordinatesFunctionTableVM table = args.Table;
            CoordinatesFunctionRowItem row = args.Row;
            CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType extractionType = args.ExtractionType;
            int tableIndex = Tables.IndexOf(table);
            switch(extractionType)
            {
                case CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType.MovingBack:
                    {
                        //if the table is the first table then we know we need to create a new table for the row
                        if (tableIndex == 0)
                        {
                            InsertTable(0, new ObservableCollection<CoordinatesFunctionRowItem>() { row });
                        }
                        else
                        {
                            //we know we aren't the first table, so move this row backward
                            MoveRowBackward(tableIndex, row);
                        }
                        break;
                    }
                case CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType.MovingForeward:
                    {
                        //if the table is the last table then we know we need to create a new table for the row
                        if (tableIndex == Tables.Count-1)
                        {
                            CreateTable(new ObservableCollection<CoordinatesFunctionRowItem>() { row });
                        }
                        else
                        {
                            //we know we aren't the last table, so move this row foreward
                            MoveRowForeward(tableIndex, row);
                        }
                        break;


                        
                    }
                case CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType.Splitting:
                    {
                        //when splitting, the event args will have already removed the row, and the rows after the row
                        //i just need to create tables for them
                        //I will leave the previous rows in the existing table
                        InsertTable(tableIndex + 1, new ObservableCollection<CoordinatesFunctionRowItem>() { row });
                        InsertTable(tableIndex + 2, args.RowsAfterRow);
                        break;
                    }
                case CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType.OnlyRow:
                    {
                        //this is the only row left in the table
                        //this row can eiter go to a previous table, go to the next table, or glue the previous and next table together.
                        if(tableIndex == 0 && Tables.Count == 1)
                        {
                            //this is the only table, and it is losing its only row
                            //create a new table for the row
                            CreateTable(new ObservableCollection<CoordinatesFunctionRowItem>() { row });
                        }
                        else if(tableIndex == 0)
                        {
                            //then it can only go foreward
                            MoveRowForeward(tableIndex, row);
                        }
                        else if(tableIndex == Tables.Count-1)
                        {
                            //then it can only go backward
                            MoveRowBackward(tableIndex, row);
                        }
                        else
                        {
                            //this table is between two tables
                            CoordinatesFunctionTableVM nextTable = Tables[tableIndex + 1];
                            CoordinatesFunctionTableVM previousTable = Tables[tableIndex - 1];
                            bool rowBelongsInNextTable = (nextTable.DistributionType == row.SelectedDistributionType && nextTable.InterpolationType == row.SelectedInterpolationType);
                            bool rowBelongsInPreviousTable = (previousTable.DistributionType == row.SelectedDistributionType && previousTable.InterpolationType == row.SelectedInterpolationType);
                            if (rowBelongsInPreviousTable && rowBelongsInNextTable)
                            {
                                //then we need to glue the tables together
                                //i will keep the previous table and this row, and the rows from the next table to it
                                previousTable.AddRow(row);
                                foreach(CoordinatesFunctionRowItem rowItem in nextTable.Rows)
                                {
                                    //i clone it so that i can be sure that i am not carrying around extra listeners to the row
                                    previousTable.AddRow(rowItem.Clone());
                                }
                                //get rid of the next table
                                Tables.Remove(nextTable);

                            }
                            else if(rowBelongsInPreviousTable)
                            {
                                //add row to previous table
                                previousTable.AddRow(row);
                            }
                            else if(rowBelongsInNextTable)
                            {
                                //add row to next table
                                nextTable.InsertRow(0, row);
                            }
                            else
                            {
                                //it doesn't belong to either table, so insert a new table for it
                                InsertTable(tableIndex, new ObservableCollection<CoordinatesFunctionRowItem>() { row });

                            }

                        }
                        break;
                    }
            }
            //UpdateView?.Invoke(this, new EventArgs());
            UpdateChartViewModel();
        }

        private void MoveRowForeward(int tableIndex, CoordinatesFunctionRowItem row)
        {
            //if next table is of the same dist type and interp type then add this row to it
            CoordinatesFunctionTableVM nextTable = Tables[tableIndex + 1];
            if (nextTable.DistributionType == row.SelectedDistributionType && nextTable.InterpolationType == row.SelectedInterpolationType)
            {
                nextTable.InsertRow(0, row);
            }
            //else create a new table
            else
            {
                InsertTable(tableIndex + 1, new ObservableCollection<CoordinatesFunctionRowItem>() { row });
            }
        }

        private void MoveRowBackward(int tableIndex, CoordinatesFunctionRowItem row)
        {
            //if next table is of the same dist type and interp type then add this row to it
            CoordinatesFunctionTableVM previousTable = Tables[tableIndex - 1];
            if (previousTable.DistributionType == row.SelectedDistributionType && previousTable.InterpolationType == row.SelectedInterpolationType)
            {
                previousTable.AddRow(row);
            }
            //else create a new table
            else
            {
                InsertTable(tableIndex , new ObservableCollection<CoordinatesFunctionRowItem>() { row });
            }
        }

        /// <summary>
        /// Turns a coordinates function into a list of row items.
        /// </summary>
        /// <param name="function">The ICoordinatesFunction</param>
        /// <returns></returns>
        private List<CoordinatesFunctionRowItem> CreateRows(ICoordinatesFunction function)
        {
            //i have to determine if the function is linked or not.
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();

            if (function.IsLinkedFunction)
            {
                CoordinatesFunctionLinked linkedFunc = (CoordinatesFunctionLinked)function;
                functions.AddRange(linkedFunc.Functions);
            }
            else
            {
                //if it is not linked then just add the one function that was passed in.
                functions.Add(function);
            }
            //create a list of all rows from all functions
            List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
            foreach (ICoordinatesFunction func in functions)
            {
                InterpolationEnum interpolator = func.Interpolator;
                foreach (ICoordinate coord in func.Coordinates)
                {
                    rows.Add(CreateRowItemFromCoordinate(coord, interpolator));

                }
            }
            return rows;
        }
        private CoordinatesFunctionRowItem CreateRowItemFromCoordinate(ICoordinate coord, InterpolationEnum interpolator)
        {
            double x = coord.X.Value();
            IOrdinateEnum type = coord.Y.Type;
            switch (type)
            {
                case IOrdinateEnum.Constant:
                    {
                        double y = coord.Y.Value();
                        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x)
                                .WithConstantDist(y, interpolator)
                                .Build();
                        return row;
                    }
                case IOrdinateEnum.Normal:
                    {
                        IDistributedOrdinate dist = (IDistributedOrdinate)coord.Y;
                        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x)
                            .WithNormalDist(dist.Mean, dist.StandardDeviation, interpolator)
                            .Build();
                        return row;
                    }
                case IOrdinateEnum.Triangular:
                    {
                        IDistributedOrdinate dist = (IDistributedOrdinate)coord.Y;
                        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x)
                            .WithTriangularDist(dist.Mode, dist.Range.Min, dist.Range.Max, interpolator)
                            .Build();
                        return row;
                    }
                case IOrdinateEnum.Uniform:
                    {
                        IDistributedOrdinate dist = (IDistributedOrdinate)coord.Y;
                        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x)
                            .WithUniformDist(dist.Range.Min, dist.Range.Max, interpolator)
                            .Build();
                        return row;
                    }
                case IOrdinateEnum.TruncatedNormal:
                    {
                        IDistributedOrdinate dist = (IDistributedOrdinate)coord.Y;
                        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x)
                            .WithTruncatedNormalDist(dist.Mean, dist.StandardDeviation, dist.Range.Min, dist.Range.Max, interpolator)
                            .Build();
                        return row;
                    }
                //case DistributionType.Beta4Parameters:
                //    {
                //        IDistributedValue dist = ((Distribution)coord.Y).GetDistribution;
                //        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x)
                //            .WithBetaDist(dist.dist.Minimum, dist.Maximum, interpolator)
                //            .Build();
                //        return row;

                //    }
                default:
                    {
                        throw new ArgumentException("The coordinate has a distributed y value that is not supported.");
                    }
            }
        }

        private void InsertTable(int index, ObservableCollection<CoordinatesFunctionRowItem> rows)
        {
            CoordinatesFunctionTableVM newTable = new CoordinatesFunctionTableVM(rows);
            newTable.RowIsLeavingTable += Table_RowIsLeaving;
            newTable.NoMoreRows += NewTable_NoMoreRows;
            newTable.TableWasModified += CellWasEdited;
            Tables.Insert(index, newTable);
            UpdateView?.Invoke(this, new EventArgs());

        }

        private void CellWasEdited(object sender, EventArgs e)
        {
            TableChanged?.Invoke(this, new EventArgs());
            UpdateChartViewModel();
        }

        /// <summary>
        /// Creates a new coordinates function table using the rows passed in.
        /// All rows should have the same distribution type and interpolation type.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="type"></param>
        private void CreateTable(ObservableCollection<CoordinatesFunctionRowItem> rows)
        {
            CoordinatesFunctionTableVM newTable = new CoordinatesFunctionTableVM(rows);
            newTable.RowIsLeavingTable += Table_RowIsLeaving;
            newTable.NoMoreRows += NewTable_NoMoreRows;
            newTable.TableWasModified += CellWasEdited;
            Tables.Add(newTable);
            UpdateView?.Invoke(this, new EventArgs());
        }

        private void NewTable_NoMoreRows(object sender, EventArgs e)
        {
            //if there are multiple tables then just delete this table
            //if this is the last table then put in an empty row
            CoordinatesFunctionTableVM table = (CoordinatesFunctionTableVM)sender;
            
            Tables.Remove(table);
            if (Tables.Count == 0)
            {
                // add a default table with a single row.
                CreateDefaultTable();
            }
            UpdateView?.Invoke(this, new EventArgs());
            //UpdateTables();
        }

        private void CreateDefaultTable()
        {
            ObservableCollection<CoordinatesFunctionRowItem> rows = new ObservableCollection<CoordinatesFunctionRowItem>();
            rows.Add(new CoordinatesFunctionRowItem());
            CreateTable(rows);
            
        }

        public bool Validate(IValidator<CoordinatesFunctionEditorVM> validator, out IEnumerable<IMessage> errors)
        {
            //return validator.IsValid(this, out errors);
            //todo: John how does this work now? 4/3/2020
            errors = new List<IMessage>();
            return true;
        }

        IMessageLevels IValidate<CoordinatesFunctionEditorVM>.Validate(IValidator<CoordinatesFunctionEditorVM> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }
    }
}
