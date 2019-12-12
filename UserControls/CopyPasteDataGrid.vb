'Imports System.Reflection
Imports System.Windows.Controls.Primitives
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Data
Imports System.Collections.ObjectModel

Namespace Controls
    Public Class CopyPasteDataGrid(Of T)
        Inherits DataGrid
        Public Event PreviewPasteData()
        Public Event DataPasted()
        Public Event PreviewAddRows()

        Public Event PreviewLastRowEnter()
        Public Event PreviewLastRowTab(ByVal cellIndex As Integer)

        Public Event RowsAdded(ByVal startrow As Integer, ByVal numrows As Integer)

        Public Event PreviewDeleteRows(ByVal rowindices As List(Of Int32), ByRef Cancel As Boolean)
        Public Event RowsDeleted(ByVal rowindices As List(Of Int32))
        Public Property AllowAddDeleteRows As Boolean = True
        Public Property PasteAddsRows As Boolean = True
        Public Sub New()
            '
        End Sub
        Private Sub Me_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles Me.PreviewKeyDown
            If IsLastRowSelected() Then
                If e.Key = Key.Enter Then
                    RaiseEvent PreviewLastRowEnter()
                ElseIf e.Key = Key.Tab Then
                    RaiseEvent PreviewLastRowTab(GetHighestSelectedColumn)
                End If

            End If
        End Sub
        Private Sub Me_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl) Then
                If e.Key = Key.V Then
                    RaiseEvent PreviewPasteData()
                    PasteClipboard()
                End If
            End If


        End Sub
        Private Function GetHighestSelectedColumn() As Integer
            Dim HighestSelectedColumn As Int32 = -1
            For Each cellInfo As DataGridCellInfo In Me.SelectedCells
                If cellInfo.Column.DisplayIndex > HighestSelectedColumn Then
                    HighestSelectedColumn = cellInfo.Column.DisplayIndex
                End If
            Next
            Return HighestSelectedColumn
        End Function
        'Private Function IsLastRowSelected() As Boolean
        '    'raise event with necessary info and let the listener handle it
        '    Dim RowIndex As Int32 = -1
        '    For Each cellInfo As DataGridCellInfo In Me.SelectedCells
        '        If Me.Items.IndexOf(cellInfo.Item) > RowIndex Then
        '            RowIndex = Me.Items.IndexOf(cellInfo.Item)
        '        End If
        '    Next
        '    If RowIndex = Me.Items.Count Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function


        Private Sub CopyPasteDataGrid_PreviewMouseRightButtonUp(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles Me.PreviewMouseRightButtonUp
            Dim dep As DependencyObject = DirectCast(e.OriginalSource, DependencyObject)

            While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridCell) AndAlso Not (TypeOf dep Is DataGridColumnHeader)
                dep = VisualTreeHelper.GetParent(dep)
            End While

            If dep Is Nothing Then
                Return
            End If

            'If TypeOf dep Is DataGridColumnHeader Then
            '    ' do something
            '    Dim columnHeader As DataGridColumnHeader = TryCast(dep, DataGridColumnHeader)
            'End If

            If TypeOf dep Is DataGridCell Then
                Dim cell As DataGridCell = TryCast(dep, DataGridCell)
                If Me.SelectedCells.Count <= 1 Then
                    Me.UnselectAllCells()
                    cell.IsSelected = True
                End If
                If cell.IsSelected = False Then
                    Me.UnselectAllCells()
                    cell.IsSelected = True
                End If
                cell.ContextMenu = New ContextMenu
                Dim CopyMenuItem As New MenuItem
                CopyMenuItem.Header = "Copy"
                AddHandler CopyMenuItem.Click, AddressOf CopyClipBoard
                If Me.SelectedCells.Count <= 0 Then CopyMenuItem.IsEnabled = False
                cell.ContextMenu.Items.Add(CopyMenuItem)

                If IsReadOnly = False Then

                    Dim PasteMenuItem As New MenuItem
                    PasteMenuItem.Header = "Paste"
                    AddHandler PasteMenuItem.Click, AddressOf MenuItemPasteClipboard
                    Try
                        Dim clipboardData As String()() = DirectCast(Clipboard.GetText(), String).Split(ControlChars.Lf).[Select](Function(row) row.Split(ControlChars.Tab).[Select](Function(Clipboardcell) If(Clipboardcell.Length > 0 AndAlso Clipboardcell(Clipboardcell.Length - 1) = ControlChars.Cr, Clipboardcell.Substring(0, Clipboardcell.Length - 1), Clipboardcell)).ToArray()).Where(Function(a) a.Any(Function(b) b.Length > 0)).ToArray()
                        If clipboardData.Length = 0 Then PasteMenuItem.IsEnabled = False
                    Catch ex As Exception
                        PasteMenuItem.IsEnabled = False
                    End Try
                    cell.ContextMenu.Items.Add(PasteMenuItem)

                    If AllowAddDeleteRows = True Then
                        Dim InsertRows As New MenuItem
                        InsertRows.Header = "Insert Row(s) Above"
                        AddHandler InsertRows.Click, AddressOf InsertRowItems
                        cell.ContextMenu.Items.Add(InsertRows)

                        Dim InsertRowsBelow As New MenuItem
                        InsertRowsBelow.Header = "Insert Row(s) Below"
                        InsertRowsBelow.IsEnabled = Not IsLastRowSelected()
                        AddHandler InsertRowsBelow.Click, AddressOf InsertRowItemsBelow
                        cell.ContextMenu.Items.Add(InsertRowsBelow)

                        Dim AddRows As New MenuItem
                        AddRows.Header = "Add Rows"
                        AddHandler AddRows.Click, AddressOf AddSelectedRows
                        cell.ContextMenu.Items.Add(AddRows)

                        Dim DeleteRows As New MenuItem
                        DeleteRows.Header = "Delete Row(s)"
                        AddHandler DeleteRows.Click, AddressOf DeleteSelectedRows
                        cell.ContextMenu.Items.Add(DeleteRows)
                    End If
                End If
                cell.ContextMenu.IsOpen = True
            End If
        End Sub
        Private Function IsLastRowSelected() As Boolean
            Dim UniqueRows As New List(Of Int32), RowIndex As Int32
            For Each cellInfo As DataGridCellInfo In Me.SelectedCells
                RowIndex = Me.Items.IndexOf(cellInfo.Item)
                If RowIndex = Me.Items.Count - 1 Then
                    Return True
                End If
            Next

            Return False
        End Function
        Private Sub DeleteSelectedRows(sender As Object, e As System.Windows.RoutedEventArgs)
            Dim UniqueRows As New List(Of Int32), RowIndex As Int32
            For Each cellInfo As DataGridCellInfo In Me.SelectedCells
                RowIndex = Me.Items.IndexOf(cellInfo.Item)
                If UniqueRows.Contains(RowIndex) = False Then UniqueRows.Add(RowIndex)
            Next
            '
            Dim Cancel As Boolean
            RaiseEvent PreviewDeleteRows(UniqueRows, Cancel)
            If Cancel = True Then Exit Sub
            '
            UniqueRows.Sort()
            Dim items As ComponentModel.IEditableCollectionView = Me.Items
            For i As Int32 = 0 To UniqueRows.Count - 1
                If items.CanRemove Then
                    items.RemoveAt(UniqueRows(i) - i)
                End If
            Next
            RaiseEvent RowsDeleted(UniqueRows)
        End Sub

        Private Sub AddSelectedRows(sender As Object, e As RoutedEventArgs)
            RaiseEvent PreviewAddRows()
            Dim UniqueRows As List(Of Integer) = GetSelectedRows()
            Dim InsertAtRow As Int32 = Me.Items.Count

            'Dim RowType As Type = Me.Items(UniqueRows(0)).GetType
            'insert the rows
            'Dim sourceRows As ObservableCollection(Of T) = CType(Me.ItemsSource, ObservableCollection(Of T))
            'For i As Int32 = 1 To UniqueRows.Count
            '    sourceRows.Insert(InsertAtRow, Activator.CreateInstance(RowType))
            'Next
            RaiseEvent RowsAdded(InsertAtRow, UniqueRows.Count)
        End Sub

        Private Sub InsertRowItemsBelow(sender As Object, e As RoutedEventArgs)
            RaiseEvent PreviewAddRows()
            Dim UniqueRows As List(Of Integer) = GetSelectedRows()

            Dim InsertAtRow As Int32 = UniqueRows.Min
            'Dim RowType As Type = Me.Items(UniqueRows(0)).GetType
            'insert the rows
            'Dim sourceRows As ObservableCollection(Of T) = CType(Me.ItemsSource, ObservableCollection(Of T))
            'For i As Int32 = 1 To UniqueRows.Count
            '    sourceRows.Insert(InsertAtRow, Activator.CreateInstance(RowType))
            'Next
            RaiseEvent RowsAdded(InsertAtRow + 1, UniqueRows.Count)
        End Sub

        Private Sub InsertRowItems(sender As Object, e As System.Windows.RoutedEventArgs)
            RaiseEvent PreviewAddRows()
            Dim UniqueRows As List(Of Integer) = GetSelectedRows()

            Dim InsertAtRow As Int32 = UniqueRows.Min
            'Dim RowType As Type = Me.Items(UniqueRows(0)).GetType
            'insert the rows
            'Dim sourceRows As ObservableCollection(Of T) = CType(Me.ItemsSource, ObservableCollection(Of T))
            'For i As Int32 = 1 To UniqueRows.Count
            '    sourceRows.Insert(InsertAtRow, Activator.CreateInstance(RowType))
            'Next
            RaiseEvent RowsAdded(InsertAtRow, UniqueRows.Count)
        End Sub

        Private Function GetSelectedRows() As List(Of Integer)
            'Convert the selected cells into a list of rows by index
            Dim UniqueRows As New List(Of Int32), RowIndex As Int32
            For Each cellInfo As DataGridCellInfo In Me.SelectedCells
                RowIndex = Me.Items.IndexOf(cellInfo.Item)
                If UniqueRows.Contains(RowIndex) = False Then UniqueRows.Add(RowIndex)
            Next

            Return UniqueRows
        End Function

        Private Sub CopyClipBoard(sender As Object, e As System.Windows.RoutedEventArgs)
            ApplicationCommands.Copy.Execute(Nothing, Me)
        End Sub
        Private Sub MenuItemPasteClipboard(sender As Object, e As System.Windows.RoutedEventArgs)
            RaiseEvent PreviewPasteData()
            PasteClipboard()
        End Sub
        Public Sub PasteClipboard()
            Try
                Dim clipboardData As String()() = DirectCast(Clipboard.GetText(), String).Split(ControlChars.Lf).[Select](Function(row) row.Split(ControlChars.Tab).[Select](Function(cell) If(cell.Length > 0 AndAlso cell(cell.Length - 1) = ControlChars.Cr, cell.Substring(0, cell.Length - 1), cell)).ToArray()).Where(Function(a) a.Any(Function(b) b.Length > 0)).ToArray()
                Dim RowIndex, ColumnIndex As Int32
                If Me.SelectedCells.Count = 1 Then
                    'fill beyond selected cell
                    Dim p As ObservableCollection(Of T) = CType(Me.ItemsSource, ObservableCollection(Of T))
                    Dim cellinfo As DataGridCellInfo = Me.SelectedCells(0)
                    RowIndex = Me.Items.IndexOf(cellinfo.Item)
                    ColumnIndex = cellinfo.Column.DisplayIndex
                    For i As Int32 = 0 To clipboardData.Count - 1
                        If (RowIndex + i) > Me.Items.Count - 1 Then
                            If PasteAddsRows = False Then
                                Me.Items.Refresh()
                                Exit Sub
                            Else
                                Dim RowType As Type = Me.Items(RowIndex).GetType
                                p.Add(Activator.CreateInstance(RowType))
                            End If
                        End If
                        For j As Int32 = 0 To clipboardData(i).Count - 1
                            If (ColumnIndex + j) > Me.Columns.Count - 1 Then Continue For
                            If Me.Columns(ColumnIndex + j).IsReadOnly Then Continue For
                            Dim binding As Binding = TryCast(TryCast(Me.Columns(ColumnIndex + j), DataGridBoundColumn).Binding, Binding)
                            Dim RowType As Type = Me.Items(RowIndex + i).GetType
                            Dim y As System.Reflection.PropertyInfo = RowType.GetProperty(binding.Path.Path)
                            y.SetValue(Me.Items(RowIndex + i), Convert.ChangeType(clipboardData(i)(j), y.PropertyType), Nothing)
                        Next
                    Next
                ElseIf Me.SelectedCells.Count > 1 Then
                    'Test for continuous selection
                    Dim Rows, Columns As New List(Of Int32)
                    For Each cellInfo As DataGridCellInfo In Me.SelectedCells
                        RowIndex = Me.Items.IndexOf(cellInfo.Item)
                        ColumnIndex = cellInfo.Column.DisplayIndex
                        Rows.Add(RowIndex)
                        Columns.Add(ColumnIndex)
                    Next

                    Rows.Sort()
                    Dim RowMax As Int32 = Rows(Rows.Count - 1)
                    RowIndex = Rows(0)
                    Columns.Sort()
                    ColumnIndex = Columns(0)
                    Dim ColumnMax As Int32 = Columns(Columns.Count - 1)
                    Dim CellCheck As DataGridCell
                    For i As Int32 = RowIndex To RowMax
                        For j As Int32 = ColumnIndex To ColumnMax
                            CellCheck = GetCell(i, j)
                            If CellCheck.IsSelected = False Then
                                MessageBox.Show("Invalid selection, selected cells must be continuous.", "Invalid Selection", MessageBoxButton.OK, MsgBoxStyle.Information)
                                Exit Sub
                            End If
                        Next
                    Next

                    For i As Int32 = 0 To clipboardData.Count - 1
                        If (RowIndex + i) > RowMax Then
                            Me.Items.Refresh()
                            Exit Sub
                        End If
                        For j As Int32 = 0 To clipboardData(i).Count - 1
                            If (ColumnIndex + j) > ColumnMax Then Continue For
                            If Me.Columns(ColumnIndex + j).IsReadOnly Then Continue For
                            Dim binding As Binding = TryCast(TryCast(Me.Columns(ColumnIndex + j), DataGridBoundColumn).Binding, Binding)
                            Dim RowType As Type = Me.Items(RowIndex + i).GetType
                            Dim y As System.Reflection.PropertyInfo = RowType.GetProperty(binding.Path.Path)
                            y.SetValue(Me.Items(RowIndex + i), Convert.ChangeType(clipboardData(i)(j), y.PropertyType), Nothing)
                        Next
                    Next
                End If

                Me.Items.Refresh()
                RaiseEvent DataPasted()
            Catch ex As Exception
                MessageBox.Show("Error pasting data from clipboard.", "Error in paste from clipboard", MessageBoxButton.OK, MsgBoxStyle.Information)
            End Try

        End Sub
        Public Function GetRow(index As Integer) As DataGridRow
            Dim row As DataGridRow = DirectCast(Me.ItemContainerGenerator.ContainerFromIndex(index), DataGridRow)
            If row Is Nothing Then
                ' May be virtualized, bring into view and try again.
                Me.UpdateLayout()
                Me.ScrollIntoView(Me.Items(index))
                row = DirectCast(Me.ItemContainerGenerator.ContainerFromIndex(index), DataGridRow)
            End If
            Return row
        End Function
        Public Function GetCell(rowindex As Integer, column As Integer) As DataGridCell
            Dim row As DataGridRow = GetRow(rowindex)
            If row IsNot Nothing Then
                Dim presenter As DataGridCellsPresenter = GetTheVisualChild(Of DataGridCellsPresenter)(row)


                If presenter Is Nothing Then
                    Me.ScrollIntoView(row, Me.Columns(column))
                    presenter = GetTheVisualChild(Of DataGridCellsPresenter)(row)
                End If
                If presenter Is Nothing Then Return Nothing
                Dim cell As DataGridCell = DirectCast(presenter.ItemContainerGenerator.ContainerFromIndex(column), DataGridCell)
                Return cell
            End If
            Return Nothing
        End Function
        Public Function GetTheVisualChild(Of T As Visual)(parent As Visual) As T
            Dim child As T = Nothing
            Dim numVisuals As Integer = VisualTreeHelper.GetChildrenCount(parent)
            For i As Integer = 0 To numVisuals - 1
                Dim v As Visual = DirectCast(VisualTreeHelper.GetChild(parent, i), Visual)
                child = TryCast(v, T)
                If child Is Nothing Then
                    child = GetTheVisualChild(Of T)(v)
                End If
                If child IsNot Nothing Then
                    Exit For
                End If
            Next
            Return child
        End Function
    End Class
End Namespace

