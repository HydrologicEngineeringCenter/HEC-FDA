Public Class TextBoxDescription
    Implements System.ComponentModel.INotifyPropertyChanged
    Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    Public Shared ReadOnly DescriptionProperty As System.Windows.DependencyProperty = System.Windows.DependencyProperty.Register("Description", GetType(String), GetType(TextBoxDescription))
    'Public Shared ReadOnly DescriptionWindowProperty As System.Windows.DependencyProperty = System.Windows.DependencyProperty.Register("DescriptionWindow", GetType(Consequences_Assist.Controls.DescriptionWindow), GetType(Consequences_Assist.Controls.TextBoxDescription), New System.Windows.PropertyMetadata(New DescriptionWindow("programmaticOverridesequence start!"), New System.Windows.PropertyChangedCallback(AddressOf OnDescriptionChanged), New System.Windows.CoerceValueCallback(AddressOf CoerceValue)))
    Private _DescriptionWindow As New DescriptionWindow
    Private _OldText As String
    Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Sub New(ByVal descriptionstring As String)
        InitializeComponent()
        _DescriptionWindow.Description = descriptionstring
    End Sub
    Public Property DescriptionWindow As DescriptionWindow
        Get
            Return _DescriptionWindow
        End Get
        Set(value As DescriptionWindow)
            _DescriptionWindow = value
            NotifyPropertyChanged("DescriptionWindow")
        End Set
    End Property

    Public Property Description As String
        Get
            Return "Hello"
        End Get
        Set(value As String)
            _DescriptionWindow.Description = value
            SetValue(DescriptionProperty, value)
            NotifyPropertyChanged("Description")
        End Set
    End Property
    Private Shared Sub OnDescriptionChanged()

    End Sub
    Private Overloads Shared Function CoerceValue()
        Return Nothing
    End Function
    Private Sub NotifyPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(info))
    End Sub
    Private Sub Expand_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Expand.Click
        _OldText = DescriptionWindow.Description
        _DescriptionWindow.ShowDialog()
        If _DescriptionWindow.DialogResult Then
            DescriptionWindow = _DescriptionWindow
        Else
            'user pressed cancel or red x
            DescriptionWindow.Description = _OldText
        End If
        DescriptionWindow = New DescriptionWindow(_DescriptionWindow.Description)
    End Sub
End Class
