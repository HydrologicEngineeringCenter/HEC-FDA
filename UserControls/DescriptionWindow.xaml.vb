Imports System.Windows
Imports System.Windows.Media
Namespace UserControls
    Public Class DescriptionWindow
        Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(DescriptionWindow), New FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))
        Public Event RequestPopOut()
        Private _HasPoppedOut As Boolean
        Public Property Text As String
            Get
                Return GetValue(TextProperty)
            End Get
            Set(value As String)
                SetValue(TextProperty, value)
            End Set
        End Property
        Sub New()
            ' This call is required by the designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call
        End Sub
        Private Sub TextBox1_KeyDown(sender As Object, e As Input.KeyEventArgs)
            Dim t As System.Windows.Controls.TextBox = DirectCast(sender, System.Windows.Controls.TextBox)
            Text = t.Text
        End Sub
        Private Sub ButtonName_Click(sender As Object, e As RoutedEventArgs)
            Me.Close()
        End Sub
        Private Sub PopOut_Click(sender As Object, e As RoutedEventArgs)
            RaiseEvent RequestPopOut()
        End Sub
        Private Sub Thumb_DragStarted(sender As Object, e As Windows.Controls.Primitives.DragStartedEventArgs)
            If Not _HasPoppedOut Then
                RaiseEvent RequestPopOut()
                _HasPoppedOut = True
                'Arrow.Visibility = Visibility.Collapsed
                'XGrid.Visibility = Visibility.Visible
            End If
        End Sub
        Private Sub Thumb_DragDelta(sender As Object, e As Windows.Controls.Primitives.DragDeltaEventArgs)
            Top += e.VerticalChange
            Left += e.HorizontalChange
        End Sub
    End Class
End Namespace
