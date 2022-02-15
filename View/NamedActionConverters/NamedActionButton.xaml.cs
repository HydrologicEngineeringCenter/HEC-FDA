using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.MVVMFramework.View.NamedActionConverters
{
    /// <summary>
    /// Interaction logic for NamedActionButton.xaml
    /// </summary>
    public partial class NamedActionButton : UserControl
    {
        public static DependencyProperty ButtonNamedActionProperty = DependencyProperty.Register(nameof(NamedAction), typeof(INamedAction), typeof(NamedActionButton), new FrameworkPropertyMetadata(ButtonNamedActionPropertyCallback));
        private RoutedEventHandler _ActionHandler;
        public event MessageReportedEventHandler MessageReport;

        public INamedAction NamedAction
        {
            get
            {
                return (INamedAction)GetValue(ButtonNamedActionProperty);
            }
            set
            {
                SetValue(ButtonNamedActionProperty, value);
            }
        }
        public NamedActionButton()
        {
            InitializeComponent();

        }
        private static void ButtonNamedActionPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NamedActionButton nab = (NamedActionButton)d;
            Button btn = nab.btn;
            if (nab._ActionHandler != null)
            {
                btn.Click -= nab._ActionHandler;
            }
            INamedAction na = (INamedAction)e.NewValue;
            Binding headerBinding = new Binding("Name");
            headerBinding.Source = na;
            headerBinding.Mode = BindingMode.OneWay;
            headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(btn, System.Windows.Controls.Button.ContentProperty, headerBinding);
            if (na is ViewModel.Interfaces.IDisplayToUI)
            {
                Binding enabledBinding = new Binding(nameof(ViewModel.Interfaces.IDisplayToUI.IsEnabled));
                enabledBinding.Source = na;
                enabledBinding.Mode = BindingMode.OneWay;
                enabledBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(btn, System.Windows.Controls.Button.IsEnabledProperty, enabledBinding);
                Binding visibilityBinding = new Binding(nameof(ViewModel.Interfaces.IDisplayToUI.IsVisible));
                visibilityBinding.Source = na;
                visibilityBinding.Mode = BindingMode.OneWay;
                visibilityBinding.Converter = new System.Windows.Controls.BooleanToVisibilityConverter();
                visibilityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(btn, System.Windows.Controls.Button.VisibilityProperty, visibilityBinding);
            }
            nab._ActionHandler = (ob, ev) => na.Action(ob, ev);
            btn.Click += nab._ActionHandler;
        }
    }
}
