Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Data
Namespace UserControls
    Public Class TextBoxDescription
        Implements System.ComponentModel.INotifyPropertyChanged
        Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(TextBoxDescription), New FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Private _DescriptionWindow As DescriptionWindow
        Private _MaxHeight
        Public Property Text As String
            Get
                Return GetValue(TextProperty)
            End Get
            Set(value As String)
                SetValue(TextProperty, value)
            End Set
        End Property
        Public ReadOnly Property Left As Double
            Get
                Return Me.PointToScreen(New Point(0, 0)).X
            End Get
        End Property
        Public ReadOnly Property MaxAllowableHeight As Double
            Get
                Return Math.Max(Window.GetWindow(Me).ActualHeight - Me.PointToScreen(New Point(0, 0)).Y - 25, _DescriptionWindow.MinHeight)
            End Get
        End Property
        Public ReadOnly Property Top As Double
            Get
                Return Me.PointToScreen(New Point(0, 0)).Y
            End Get
        End Property
        Sub New()
            ' This call is required by the designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.

        End Sub
        Private Sub ParentMove(sender As System.Object, e As System.EventArgs)
            If IsNothing(_DescriptionWindow) Then Exit Sub
            _DescriptionWindow.Top = Top
            _DescriptionWindow.Left = Left
        End Sub
        Private Sub ParentResize(sender As System.Object, e As System.EventArgs)
            If IsNothing(_DescriptionWindow) Then Exit Sub
            _DescriptionWindow.MaxHeight = MaxAllowableHeight
            _DescriptionWindow.Width = ActualWidth
            _DescriptionWindow.MaxWidth = ActualWidth
            _DescriptionWindow.MinWidth = ActualWidth
            _DescriptionWindow.Top = Top
            _DescriptionWindow.Left = Left
        End Sub
        Private Sub HandlePopOut()
            Dim parent As Window = Window.GetWindow(Me)
            RemoveHandler parent.LocationChanged, AddressOf ParentMove
            RemoveHandler parent.SizeChanged, AddressOf ParentResize
            _DescriptionWindow.MaxHeight = Int32.MaxValue
            _DescriptionWindow.MaxWidth = Int32.MaxValue
            _DescriptionWindow.MinWidth = 100
            Arrow.Visibility = Visibility.Collapsed
            Expand.IsEnabled = False

        End Sub
        Private Sub WindowCloses(sender As System.Object, e As System.EventArgs)
            RemoveHandler _DescriptionWindow.RequestPopOut, AddressOf HandlePopOut
            _DescriptionWindow = Nothing
            Dim parent As Window = Window.GetWindow(Me)
            RemoveHandler parent.LocationChanged, AddressOf ParentMove
            RemoveHandler parent.SizeChanged, AddressOf ParentResize
            Arrow.Visibility = Visibility.Visible
            Expand.IsEnabled = True
        End Sub
        Private Sub Expand_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Expand.Click

            Dim parent As Window = Window.GetWindow(Me)
            AddHandler parent.LocationChanged, AddressOf ParentMove
            AddHandler parent.SizeChanged, AddressOf ParentResize
            Dim location As Point = Me.PointToScreen(New Point(0, 0))
            _DescriptionWindow = New DescriptionWindow()
            _DescriptionWindow.Owner = parent
            _DescriptionWindow.DataContext = Me
            _DescriptionWindow.MaxHeight = MaxAllowableHeight
            _DescriptionWindow.Width = ActualWidth
            _DescriptionWindow.MaxWidth = ActualWidth
            _DescriptionWindow.MinWidth = ActualWidth
            _DescriptionWindow.Top = Top
            _DescriptionWindow.Left = Left
            _DescriptionWindow.Show()
            AddHandler _DescriptionWindow.Closing, AddressOf WindowCloses
            AddHandler _DescriptionWindow.RequestPopOut, AddressOf HandlePopOut
        End Sub

        Private Sub TxtDescription_KeyDown(sender As Object, e As Input.KeyEventArgs)
            Dim t As System.Windows.Controls.TextBox = DirectCast(sender, System.Windows.Controls.TextBox)
            Text = t.Text
        End Sub
    End Class
End Namespace
