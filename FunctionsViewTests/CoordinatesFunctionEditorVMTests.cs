using Functions;
using Functions.CoordinatesFunctions;
using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace FunctionsViewTests
{
    [ExcludeFromCodeCoverage]
    public class CoordinatesFunctionEditorVMTests
    {

        private ObservableCollection<CoordinatesFunctionRowItem> CreateTestConstantRows()
        {
            ObservableCollection<CoordinatesFunctionRowItem> rows = new ObservableCollection<CoordinatesFunctionRowItem>();
            for (int i = 0; i < 10; i++)
            {
                CoordinatesFunctionRowItemBuilder builder = new CoordinatesFunctionRowItemBuilder(i)
                .WithConstantDist(i * 2, InterpolationEnum.None);
                rows.Add(builder.Build());
            }
            return rows;
        }

        private CoordinatesFunctionTableVM CreateTable()
        {
            ObservableCollection<CoordinatesFunctionRowItem> rows = CreateTestConstantRows();
            return new CoordinatesFunctionTableVM(rows);
        }

        /// <summary>
        /// Tests that if the EditorVM gets passed a Linked Coordinates Function then it can return the correct linked coordinates function.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionEditorVM_CreateFunctionFromTables_LinkedFunction_Returns_ICoordinateFunction()
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3, 4 };
            List<double> ys = new List<double>() { 10, 11, 12, 13, 14 };

            List<double> xs2 = new List<double>() { 10, 11, 12, 13, 14 };
            List<double> ys2 = new List<double>() { 100, 110, 120, 130, 140 };

            ICoordinatesFunction func1 = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            ICoordinatesFunction func2 = ICoordinatesFunctionsFactory.Factory(xs2, ys2, InterpolationEnum.Piecewise);
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>() { func1, func2 };
            List<InterpolationEnum> interps = new List<InterpolationEnum>();
            interps.Add(func1.Interpolator);
            ICoordinatesFunction linkedFunc = ICoordinatesFunctionsFactory.Factory(functions, interps);

            CoordinatesFunctionEditorVM editorVM = new CoordinatesFunctionEditorVM(linkedFunc);
            ICoordinatesFunction returnedFunction = editorVM.CreateFunctionFromTables();

            Assert.True(returnedFunction.Equals(linkedFunc));

        }

        /// <summary>
        /// Tests that if the EditorVM gets passed a Constant Coordinates Function and then a row gets added that is of a different distribution type
        /// then the editor will return the correct linked coordinates function.
        /// </summary>
        //[Fact]
        //public void CoordinatesFunctionEditorVM_CreateFunctionFromTables_ConstantToLinkedFunction_Returns_ICoordinateFunction()
        //{
        //    //Create the initial linked function
        //    List<double> xs = new List<double>() { 0, 1, 2, 3, 4 };
        //    List<double> ys = new List<double>() { 10, 11, 12, 13, 14 };

        //    ICoordinatesFunction originalConstantFunction = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);

        //    List<double> xs2 = new List<double>() { 10 };
        //    List<double> ys2 = new List<double>() { 100 };

        //    ICoordinatesFunction func2 = ICoordinatesFunctionsFactory.Factory(xs2, ys2, InterpolationEnum.Piecewise);
        //    List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>() { func1, func2 };
        //    List<InterpolationEnum> interps = new List<InterpolationEnum>();
        //    interps.Add(func1.Interpolator);
        //    ICoordinatesFunction linkedFunc = ICoordinatesFunctionsFactory.Factory(functions, interps);

        //    CoordinatesFunctionEditorVM editorVM = new CoordinatesFunctionEditorVM(linkedFunc);

        //    //remove the last row of the last table
        //    editorVM.Tables[1].rem


        //    ICoordinatesFunction returnedFunction = editorVM.CreateFunctionFromTables();

        //    Assert.True(returnedFunction.Equals(linkedFunc));

        //}


        /// <summary>
        /// Tests that a EditorVM can be constructed from a coordinatesFunction and can return a coordinates function.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionEditorVM_CreateFunctionFromTables_ConstantFunction_Returns_ICoordinateFunction()
        {
            //if there is only one table then the editor can just ask the table to create the function

            //if there are more than one table then the editor asks all the tables for functions and then
            //uses that list to create a CoordinatesFunctionLinked.
            List<double> xs = new List<double>() { 0, 1, 2, 3, 4, 5 };
            List<double> ys = new List<double>() { 10, 20, 30, 40, 50, 60 };

            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(xs, ys);

            CoordinatesFunctionEditorVM editorVM = new CoordinatesFunctionEditorVM(func);
            ICoordinatesFunction returnedFunction = editorVM.CreateFunctionFromTables();

            Assert.True(returnedFunction.Equals(func));
        }

        /// <summary>
        /// Tests with multiple tables the EditorVM can create a linked coordinates function.
        /// </summary> 
        [Fact]
        public void CoordinatesFunctionEditorVM_CreateFunctionFromTables_ConstantAndNormalValues_Returns_ICoordinateFunction()
        {
            //if there is only one table then the editor can just ask the table to create the function

            //if there are more than one table then the editor asks all the tables for functions and then
            //uses that list to create a CoordinatesFunctionLinked.
            List<double> xs = new List<double>() { 0, 1, 2, 3, 4, 5 };
            List<double> ys = new List<double>() { 10, 20, 30, 40, 50, 60 };

            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(xs, ys);

            CoordinatesFunctionEditorVM editorVM = new CoordinatesFunctionEditorVM(func);
            ICoordinatesFunction returnedFunction = editorVM.CreateFunctionFromTables();

            Assert.True(returnedFunction.Equals(func));
        }
    }
}
