Public Class DescriptionWindow
    Implements System.ComponentModel.INotifyPropertyChanged
    Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    Private _Desription As String
    Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call
    End Sub
    Sub New(ByVal desc As String)
        InitializeComponent()
        Description = desc
    End Sub
    'dependency property
    Public Shared ReadOnly DescriptionTextProperty As DependencyProperty = DependencyProperty.Register("DescriptionText", GetType(String), GetType(DescriptionWindow))

    Public Property Description As String
        Get
            Return CBool(GetValue(DescriptionTextProperty))
        End Get
        Set(ByVal value As String)
            SetValue(DescriptionTextProperty, value)
        End Set
    End Property


    'Public Property Description As String
    '    Get
    '        Return _Desription
    '    End Get
    '    Set(value As String)
    '        _Desription = value
    '        NotifyPropertyChanged("Description")
    '    End Set
    'End Property
    Private Sub NotifyPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(info))
    End Sub
    Private Sub CmdClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Private Sub Cmdok_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Cmdok.Click
        Me.DialogResult = True
        Me.Close()
    End Sub
    Private Sub TextBox1_KeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs) Handles TextBox1.KeyDown
        Description = TextBox1.Text
    End Sub
End Class
