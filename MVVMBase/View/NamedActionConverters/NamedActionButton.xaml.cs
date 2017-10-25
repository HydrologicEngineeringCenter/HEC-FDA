using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Base.Events;

namespace View.NamedActionConverters
{
    /// <summary>
    /// Interaction logic for NamedActionButton.xaml
    /// </summary>
    [Base.Attributes.ReporterDisplayName("abscd")]
    public partial class NamedActionButton : UserControl, Base.Interfaces.IReportMessage
    {
        public static DependencyProperty ButtonNamedActionProperty = DependencyProperty.Register(nameof(NamedAction), typeof(Base.Interfaces.INamedAction), typeof(NamedActionButton), new FrameworkPropertyMetadata(ButtonNamedActionPropertyCallback));
        private RoutedEventHandler _ActionHandler;
        private System.Timers.Timer _t = new System.Timers.Timer(1000);
        public event MessageReportedEventHandler MessageReport;
        private int _pingCount = 0;

        public Base.Interfaces.INamedAction NamedAction
        {
            get
            {
                return (Base.Interfaces.INamedAction)GetValue(ButtonNamedActionProperty);
            }
            set
            {
                SetValue(ButtonNamedActionProperty, value);
            }
        }
        public NamedActionButton()
        {
            InitializeComponent();
            Base.Implementations.MessageHub.Register(this);
            _t.Elapsed += _t_Elapsed;
            _t.Start();
        }

        private void _t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _pingCount += 1;
            ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message("Ping! " + _pingCount + "\n")));
        }

        private static void ButtonNamedActionPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NamedActionButton nab = (NamedActionButton)d;
            Button btn = nab.btn;
            if (nab._ActionHandler != null)
            {
                btn.Click -= nab._ActionHandler;
            }
            Base.Interfaces.INamedAction na = (Base.Interfaces.INamedAction)e.NewValue;
            Binding headerBinding = new Binding("Name");
            headerBinding.Source = na;
            headerBinding.Mode = BindingMode.OneWay;
            headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(btn, System.Windows.Controls.Button.ContentProperty, headerBinding);
            Binding enabledBinding = new Binding("IsEnabled");
            enabledBinding.Source = na;
            enabledBinding.Mode = BindingMode.OneWay;
            enabledBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(btn, System.Windows.Controls.Button.IsEnabledProperty, enabledBinding);
            Binding visibilityBinding = new Binding("IsVisible");
            visibilityBinding.Source = na;
            visibilityBinding.Mode = BindingMode.OneWay;
            visibilityBinding.Converter = new System.Windows.Controls.BooleanToVisibilityConverter();
            visibilityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(btn, System.Windows.Controls.Button.VisibilityProperty, visibilityBinding);
            nab._ActionHandler = (ob, ev) => na.Action(ob, ev);
            btn.Click += nab._ActionHandler;
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
    }
}
