using Functions;
using FunctionsView.Validation;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using paireddata;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private UncertainPairedData _Function;
        private IEnumerable<IMessage> _Messages = new List<IMessage>();
        private bool _IsChartInError;
        private string _XLabel;
        private bool _IsReadOnly;

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
        public UncertainPairedData Function
        {
            get { return _Function; }
            set { _Function = value; }//CreateTables(_Function);  }
        }
        public List<CoordinatesFunctionTableVM> Tables
        {
            get;
            set;
        }

      

        public CoordinatesFunctionEditorVM(bool isReadOnly = false)
        {
            _IsReadOnly = isReadOnly;
            Tables = new List<CoordinatesFunctionTableVM>();
        }
        public CoordinatesFunctionEditorVM(UncertainPairedData function, string xLabel, string yLabel, string chartTitle, bool isReadOnly = false)
        {
            _IsReadOnly = isReadOnly;
            XLabel = xLabel;
            YLabel = yLabel;
            ChartTitle = chartTitle;
            _Function = function;
            Tables = new List<CoordinatesFunctionTableVM>();
            //CreateTables(function);
            //UpdateChartViewModel();
            
        }

        /// <summary>
        /// Copy ctor
        /// </summary>
        /// <param name="vm"></param>
        public CoordinatesFunctionEditorVM(CoordinatesFunctionEditorVM vm)
        {
            _IsReadOnly = vm._IsReadOnly;
            XLabel = vm.XLabel;
            YLabel = vm.YLabel;
            ChartTitle = vm.ChartTitle;
            _Function = vm.Function;
            Tables = new List<CoordinatesFunctionTableVM>();
            //CreateTables(_Function);
            //UpdateChartViewModel();
        }

        ///// <summary>
        ///// This just happens one time when opening the editor.
        ///// </summary>
        //private void SetChartViewModel()
        //{
        //    ICoordinatesFunction func = CreateFunctionFromTables();
        //    if (func.IsLinkedFunction)
        //    {
        //        List<ICoordinatesFunction> functions = ((CoordinatesFunctionLinked)func).Functions;
        //    }
        //}

        //public void UpdateChartViewModel()
        //{

        //    try
        //    {
        //        CoordinatesFunctionEditorChartHelper chartHelper = new CoordinatesFunctionEditorChartHelper(CreateFunctionFromTables(), XLabel, YLabel);
        //        List<SciLineData> lineData = chartHelper.CreateLineData();
        //        CoordinatesChartViewModel.LineData.Set(lineData);           
        //    }
        //    catch (Exception ex)
        //    {
        //        CoordinatesChartViewModel.LineData.Set(new List<SciLineData>());

        //    }
        //}

        ////I need to convert the x,y data from all the lines and flip them to go vertical separated by double.nan
        ////so the y data will be (y5%, y50%, y95%, double.nan
        ////and the x data will be (x1,  x1,    x1, double.nan, x2, x2, x2
        //private NumericLineData CreateNoneData(double[] xs, List<double[]> ys)
        //{
        //    //all the lines should have the same number of xs and ys
        //    foreach (double[] yvals in ys)
        //    {
        //        if (xs.Length != yvals.Length)
        //        {
        //            return new NumericLineData();
        //        }
        //    }

        //    int numLines = ys.Count();
        //    List<double> noneYs = new List<double>();
        //    List<double> noneXs = new List<double>();
        //    for (int j = 0; j < xs.Length; j++)
        //    {
        //        for (int i = 0; i < numLines; i++)
        //        {
        //            noneXs.Add(xs[j]);
        //            noneYs.Add(ys[i][j]);
        //        }
        //        //add the double.nan
        //        noneXs.Add(double.NaN);
        //        noneYs.Add(double.NaN);
        //    }

        //    return new NumericLineData(noneXs.ToArray(), noneYs.ToArray(), ChartTitle, "None", XLabel, YLabel, PlotType.Line)
        //    {
        //        StrokeThickness = 5.0,
        //        Sorted = false,
        //        AntiAliasing = true,
        //        StrokeColor = Colors.GreenYellow,
        //        SymbolSize = 5.0,
        //        SymbolType = SymbolType.Circle,
        //        SymbolFillColor = Colors.Black,
        //        SymbolStrokeColor = Colors.Black,
        //        SymbolStrokeThickness = 1,
        //    };

        //}    

        //private XyySciLineData2D<double, double> InitXyyLineData(double[] xArray, double[] yArray, double[] y1Array, string seriesName)
        //{
        //    return new XyySciLineData2D<double, double>(xArray, yArray, y1Array, CHART_TITLE, seriesName, "xAxisName", "yAxisName")
        //    {
        //        Sorted = false,
        //        ChartTitle = CHART_TITLE,
        //        ChartName = CHART_TITLE,
        //        StrokeColor = Colors.Black,
        //        StrokeThickness = 1.0,
        //        StrokeColorY1 = Colors.Red,
        //        AntiAliasing = true,
        //        FillColor = Color.FromArgb(0x55, 0xFF, 0x00, 0x00),
        //        FillColorY1 = Color.FromArgb(0x55, 0xAD, 0xFF, 0x2F),
        //    };
        //}

        //public bool IsValid
        //{
        //    get
        //    {
        //        bool isValid = Validate(new CoordinatesFunctionEditorVMValidator(), out IEnumerable<IMessage> errors);
        //        _Messages = errors;
        //        return isValid;
        //    }
        //}

        public IEnumerable<IMessage> Messages => _Messages;

        public IMessageLevels State => throw new NotImplementedException();

        ///// <summary>
        ///// This is used when the editor is opened to load the tables from a coordinates function.
        ///// </summary>
        ///// <param name="function"></param>
        //private void CreateTables(UncertainPairedData function)
        //{
        //    List<CoordinatesFunctionRowItem> rows = CreateRows(function);
        //    CreateTables(rows);
        //}

        //public void Save()
        //{
        //}

        //public UncertainPairedData CreateFunctionFromTables()
        //{
        //    //if (Tables.Count == 1)
        //    {
        //        //if there is only one table then we know that this is not
        //        //a linked coordinates function.
        //        return Tables[0].CreateCoordinatesFunctionFromTable();
        //    }
        //    //else
        //    //{
        //    //    return CreateLinkedFunction();
        //    //}
        //}

        ////private ICoordinatesFunction CreateLinkedFunction()
        ////{
        ////    //the interpolators between tables is the same as the interpolator of the last row
        ////    //of the previous table. Since any rows in a table all have the same interpolator
        ////    //i can grab the interpolator from any row in the table.
        ////    List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();
        ////    foreach (CoordinatesFunctionTableVM table in Tables)
        ////    {
        ////        functions.Add(table.CreateCoordinatesFunctionFromTable());
        ////    }
        ////    return ICoordinatesFunctionsFactory.Factory(functions);
        ////}

        ///// <summary>
        ///// This is called anytime an visual update needs to happen, such as changing distribution types.
        ///// </summary>
        ///// <param name="rowItems"></param>
        //private void CreateTables(List<CoordinatesFunctionRowItem> rowItems)
        //{           
        //    //we are about to create the tables, so we need to find out what kinds of data we are dealing with
        //    //ColumnWidths.TableTypes tableType = ColumnWidths.GetTableTypeForRows(rowItems);
        //    //it would be nice to just bind the items source of the list to be the list of tables
        //    //but i have to put the topper and bottom UI pieces in. I could do them outside the list
        //    //but that seemed harder to get it to line up etc.
        //    //lst_tables.Items.Clear();

        //    Tables.Clear();

        //    if (rowItems.Count > 0)
        //    {

        //        ObservableCollection<CoordinatesFunctionRowItem> rows = new ObservableCollection<CoordinatesFunctionRowItem>();
        //        IOrdinateEnum distType = rowItems[0].SelectedDistributionType;
        //        //InterpolationEnum interpType = rowItems[0].SelectedInterpolationType;
        //        rows.Add(rowItems[0]);
        //        for (int i = 1; i < rowItems.Count; i++)
        //        {
        //            CoordinatesFunctionRowItem row = rowItems[i];
        //            if (row.SelectedDistributionType.Equals(distType) )
        //            {
        //                rows.Add(rowItems[i]);
        //            }
        //            else
        //            {
        //                //the dist type changed
        //                CreateTable(rows);
        //                //set the new dist type and add it to the list
        //                distType = row.SelectedDistributionType;
        //                //interpType = row.SelectedInterpolationType;
        //                rows = new ObservableCollection<CoordinatesFunctionRowItem>();
        //                rows.Add(row);
        //            }
        //        }
        //        //need to create the final table
        //        CreateTable(rows);
        //    }
        //}

        //private void UpdateTables()
        //{
        //    List<CoordinatesFunctionRowItem> rows = GetAllRowsFromAllTables();
        //    CreateTables(rows);
        //}

        //private List<CoordinatesFunctionRowItem> GetAllRowsFromAllTables()
        //{
        //    //These rows already have tables listening to the row is leaving event. I need to clear those
        //    List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
        //    foreach(CoordinatesFunctionTableVM table in Tables)
        //    {
        //        rows.AddRange(table.Rows);
        //    }
        //    return rows;
        //}
        //private void Table_RowIsLeaving(CoordinatesFunctionTableVM.RowLeavingEventArgs args)
        //{
        //    CoordinatesFunctionTableVM table = args.Table;
        //    CoordinatesFunctionRowItem row = args.Row;
        //    CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType extractionType = args.ExtractionType;
        //    int tableIndex = Tables.IndexOf(table);
        //    switch(extractionType)
        //    {
        //        case CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType.MovingBack:
        //            {
        //                //if the table is the first table then we know we need to create a new table for the row
        //                if (tableIndex == 0)
        //                {
        //                    InsertTable(0, new ObservableCollection<CoordinatesFunctionRowItem>() { row });
        //                }
        //                else
        //                {
        //                    //we know we aren't the first table, so move this row backward
        //                    MoveRowBackward(tableIndex, row);
        //                }
        //                break;
        //            }
        //        case CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType.MovingForeward:
        //            {
        //                //if the table is the last table then we know we need to create a new table for the row
        //                if (tableIndex == Tables.Count-1)
        //                {
        //                    CreateTable(new ObservableCollection<CoordinatesFunctionRowItem>() { row });
        //                }
        //                else
        //                {
        //                    //we know we aren't the last table, so move this row foreward
        //                    MoveRowForeward(tableIndex, row);
        //                }
        //                break;


                        
        //            }
        //        case CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType.Splitting:
        //            {
        //                //when splitting, the event args will have already removed the row, and the rows after the row
        //                //i just need to create tables for them
        //                //I will leave the previous rows in the existing table
        //                InsertTable(tableIndex + 1, new ObservableCollection<CoordinatesFunctionRowItem>() { row });
        //                InsertTable(tableIndex + 2, args.RowsAfterRow);
        //                break;
        //            }
        //        case CoordinatesFunctionTableVM.RowLeavingEventArgs.RowExtractionType.OnlyRow:
        //            {
        //                //this is the only row left in the table
        //                //this row can eiter go to a previous table, go to the next table, or glue the previous and next table together.
        //                if(tableIndex == 0 && Tables.Count == 1)
        //                {
        //                    //this is the only table, and it is losing its only row
        //                    //create a new table for the row
        //                    CreateTable(new ObservableCollection<CoordinatesFunctionRowItem>() { row });
        //                }
        //                else if(tableIndex == 0)
        //                {
        //                    //then it can only go foreward
        //                    MoveRowForeward(tableIndex, row);
        //                }
        //                else if(tableIndex == Tables.Count-1)
        //                {
        //                    //then it can only go backward
        //                    MoveRowBackward(tableIndex, row);
        //                }
        //                else
        //                {
        //                    //this table is between two tables
        //                    CoordinatesFunctionTableVM nextTable = Tables[tableIndex + 1];
        //                    CoordinatesFunctionTableVM previousTable = Tables[tableIndex - 1];
        //                    bool rowBelongsInNextTable = (nextTable.DistributionType == row.SelectedDistributionType && nextTable.InterpolationType == row.SelectedInterpolationType);
        //                    bool rowBelongsInPreviousTable = (previousTable.DistributionType == row.SelectedDistributionType && previousTable.InterpolationType == row.SelectedInterpolationType);
        //                    if (rowBelongsInPreviousTable && rowBelongsInNextTable)
        //                    {
        //                        //then we need to glue the tables together
        //                        //i will keep the previous table and this row, and the rows from the next table to it
        //                        previousTable.AddRow(row);
        //                        foreach(CoordinatesFunctionRowItem rowItem in nextTable.Rows)
        //                        {
        //                            //i clone it so that i can be sure that i am not carrying around extra listeners to the row
        //                            previousTable.AddRow(rowItem.Clone());
        //                        }
        //                        //get rid of the next table
        //                        Tables.Remove(nextTable);

        //                    }
        //                    else if(rowBelongsInPreviousTable)
        //                    {
        //                        //add row to previous table
        //                        previousTable.AddRow(row);
        //                    }
        //                    else if(rowBelongsInNextTable)
        //                    {
        //                        //add row to next table
        //                        nextTable.InsertRow(0, row);
        //                    }
        //                    else
        //                    {
        //                        //it doesn't belong to either table, so insert a new table for it
        //                        InsertTable(tableIndex, new ObservableCollection<CoordinatesFunctionRowItem>() { row });

        //                    }

        //                }
        //                break;
        //            }
        //    }
        //    UpdateChartViewModel();
        //}

        //private void MoveRowForeward(int tableIndex, CoordinatesFunctionRowItem row)
        //{
        //    //if next table is of the same dist type and interp type then add this row to it
        //    CoordinatesFunctionTableVM nextTable = Tables[tableIndex + 1];
        //    if (nextTable.DistributionType == row.SelectedDistributionType && nextTable.InterpolationType == row.SelectedInterpolationType)
        //    {
        //        nextTable.InsertRow(0, row);
        //    }
        //    //else create a new table
        //    else
        //    {
        //        InsertTable(tableIndex + 1, new ObservableCollection<CoordinatesFunctionRowItem>() { row });
        //    }
        //}

        //private void MoveRowBackward(int tableIndex, CoordinatesFunctionRowItem row)
        //{
        //    //if next table is of the same dist type and interp type then add this row to it
        //    CoordinatesFunctionTableVM previousTable = Tables[tableIndex - 1];
        //    if (previousTable.DistributionType == row.SelectedDistributionType && previousTable.InterpolationType == row.SelectedInterpolationType)
        //    {
        //        previousTable.AddRow(row);
        //    }
        //    //else create a new table
        //    else
        //    {
        //        InsertTable(tableIndex , new ObservableCollection<CoordinatesFunctionRowItem>() { row });
        //    }
        //}

        ///// <summary>
        ///// Turns a coordinates function into a list of row items.
        ///// </summary>
        ///// <param name="function">The ICoordinatesFunction</param>
        ///// <returns></returns>
        //private List<CoordinatesFunctionRowItem> CreateRows(UncertainPairedData function)
        //{
        //    //i have to determine if the function is linked or not.
        //    //List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();

        //    //if (function.IsLinkedFunction)
        //    //{
        //    //    CoordinatesFunctionLinked linkedFunc = (CoordinatesFunctionLinked)function;
        //    //    functions.AddRange(linkedFunc.Functions);
        //    //}
        //    //else
        //    //{
        //    //    //if it is not linked then just add the one function that was passed in.
        //    //    functions.Add(function);
        //    //}
        //    //create a list of all rows from all functions
        //    List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
        //    //foreach (ICoordinatesFunction func in functions)
        //    {
        //        //InterpolationEnum interpolator = func.Interpolator;
        //        for (int i = 0; i < function.xs().Length; i++)
        //        {
        //            double x = function.xs()[i];
        //            IDistribution y = function.ys()[i];
        //            rows.Add(CreateRowItemFromCoordinate(x, y));

        //        }
        //    }
        //    return rows;
        //}
        //private CoordinatesFunctionRowItem CreateRowItemFromCoordinate(double x, IDistribution y)
        //{
        //    //IOrdinateEnum type = coord.Y.Type;
        //    //switch (type)
        //    //{
        //    //    case IOrdinateEnum.Constant:
        //    //        {
        //    CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x,false)
        //            .WithConstantDist(y.CDF(.5))
        //            .Build();
        //    return row;
        //    //        }
        //    //    case IOrdinateEnum.Normal:
        //    //{
        //    //    //IDistributedOrdinate dist = (IDistributedOrdinate)coord.Y;
        //    //    CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x, _IsReadOnly)
        //    //                .WithNormalDist(y., dist.StandardDeviation)
        //    //                .Build();
        //    //            return row;
        //    //        }
        //        //case IOrdinateEnum.Triangular:
        //        //    {
        //        //        IDistributedOrdinate dist = (IDistributedOrdinate)coord.Y;
        //        //        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x, _IsReadOnly)
        //        //            .WithTriangularDist(dist.Mode, dist.Range.Min, dist.Range.Max, interpolator)
        //        //            .Build();
        //        //        return row;
        //        //    }
        //        //case IOrdinateEnum.Uniform:
        //        //    {
        //        //        IDistributedOrdinate dist = (IDistributedOrdinate)coord.Y;
        //        //        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x, _IsReadOnly)
        //        //            .WithUniformDist(dist.Range.Min, dist.Range.Max, interpolator)
        //        //            .Build();
        //        //        return row;
        //        //    }
        //        //case IOrdinateEnum.TruncatedNormal:
        //        //    {
        //        //        IDistributedOrdinate dist = (IDistributedOrdinate)coord.Y;
        //        //        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x, _IsReadOnly)
        //        //            .WithTruncatedNormalDist(dist.Mean, dist.StandardDeviation, dist.Range.Min, dist.Range.Max, interpolator)
        //        //            .Build();
        //        //        return row;
        //        //    }
        //        //default:
        //        //    {
        //        //        throw new ArgumentException("The coordinate has a distributed y value that is not supported.");
        //        //    }
        //    }
        //}

        //private void InsertTable(int index, ObservableCollection<CoordinatesFunctionRowItem> rows)
        //{
        //    CoordinatesFunctionTableVM newTable = new CoordinatesFunctionTableVM(rows, _IsReadOnly);
        //    newTable.RowIsLeavingTable += Table_RowIsLeaving;
        //    newTable.NoMoreRows += NewTable_NoMoreRows;
        //    newTable.TableWasModified += CellWasEdited;
        //    Tables.Insert(index, newTable);
        //    UpdateView?.Invoke(this, new EventArgs());
        //}

        //private void CellWasEdited(object sender, EventArgs e)
        //{
        //    TableChanged?.Invoke(this, new EventArgs());
        //    UpdateChartViewModel();
        //}

        ///// <summary>
        ///// Creates a new coordinates function table using the rows passed in.
        ///// All rows should have the same distribution type and interpolation type.
        ///// </summary>
        ///// <param name="rows"></param>
        ///// <param name="type"></param>
        //private void CreateTable(ObservableCollection<CoordinatesFunctionRowItem> rows)
        //{
        //    CoordinatesFunctionTableVM newTable = new CoordinatesFunctionTableVM(rows, _IsReadOnly);
        //    newTable.RowIsLeavingTable += Table_RowIsLeaving;
        //    newTable.NoMoreRows += NewTable_NoMoreRows;
        //    newTable.TableWasModified += CellWasEdited;
        //    Tables.Add(newTable);
        //    UpdateView?.Invoke(this, new EventArgs());
        //}

        //private void NewTable_NoMoreRows(object sender, EventArgs e)
        //{
        //    //if there are multiple tables then just delete this table
        //    //if this is the last table then put in an empty row
        //    CoordinatesFunctionTableVM table = (CoordinatesFunctionTableVM)sender;
            
        //    Tables.Remove(table);
        //    if (Tables.Count == 0)
        //    {
        //        // add a default table with a single row.
        //        CreateDefaultTable();
        //    }
        //    UpdateView?.Invoke(this, new EventArgs());
        //}

        //private void CreateDefaultTable()
        //{
        //    ObservableCollection<CoordinatesFunctionRowItem> rows = new ObservableCollection<CoordinatesFunctionRowItem>();
        //    rows.Add(new CoordinatesFunctionRowItem(_IsReadOnly));
        //    CreateTable(rows);
            
        //}

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
