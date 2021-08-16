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

    public class CoordinatesFunctionTableVMTests
    {

        private ObservableCollection<CoordinatesFunctionRowItem> CreateTestConstantRows()
        {
            ObservableCollection<CoordinatesFunctionRowItem> rows = new ObservableCollection<CoordinatesFunctionRowItem>();
            for(int i = 0;i<10;i++)
            {
                CoordinatesFunctionRowItemBuilder builder = new CoordinatesFunctionRowItemBuilder(i, true)
                .WithConstantDist(i*2, InterpolationEnum.None);
                rows.Add(builder.Build());
            }
            return rows;
        }

        /// <summary>
        /// Tests that a TableVM can be constructed from row items and that it can create an ICoordinatesFunction from its constant data.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Fact]
        public void CoordinatesFunctionTableVM_CreateCoordinatesFunctionFromTable_ConstantValues_Returns_ICoordinateFunction()
        {

            ObservableCollection<CoordinatesFunctionRowItem> rows = CreateTestConstantRows();
            CoordinatesFunctionTableVM tableVM = new CoordinatesFunctionTableVM(rows, true);
            ICoordinatesFunction func = tableVM.CreateCoordinatesFunctionFromTable();

            Assert.True(func.Coordinates.Count == 10);
            for (int i = 0; i < 10; i++)
            {
                Assert.True(func.Coordinates[i].X.Value() == i);
                Assert.True(func.Coordinates[i].Y.Value() == i * 2);
            }
        }
    }
}
