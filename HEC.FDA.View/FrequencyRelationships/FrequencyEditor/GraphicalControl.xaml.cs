﻿using System.Windows.Controls;

namespace HEC.FDA.View.FrequencyRelationships.FrequencyEditor;
/// <summary>
/// Interaction logic for NewGraphicalControl.xaml
/// </summary>
public partial class GraphicalControl : UserControl
{
    public GraphicalControl()
    {
        InitializeComponent();
        _userEntryDataGrid.ColumnNameMappings.Add("X", "Exc. Prob");
        _userEntryDataGrid.ColumnNameMappings.Add("Value", "Value");
    }
}
