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
                CoordinatesFunctionRowItemBuilder builder = new CoordinatesFunctionRowItemBuilder(i, false)
                .WithConstantDist(i * 2, InterpolationEnum.None);
                rows.Add(builder.Build());
            }
            return rows;
        }

        private CoordinatesFunctionTableVM CreateTable()
        {
            ObservableCollection<CoordinatesFunctionRowItem> rows = CreateTestConstantRows();
            return new CoordinatesFunctionTableVM(rows, false);
        }



       


    
    }
}
