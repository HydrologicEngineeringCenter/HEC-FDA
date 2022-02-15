using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for BindableButton.xaml
    /// </summary>
    public partial class BindableButton : UserControl
    {
        public static DependencyProperty NamedActionProperty = DependencyProperty.Register(nameof(NamedAction), typeof(HEC.FDA.ViewModel.Utilities.NamedAction), typeof(BindableButton), new PropertyMetadata(NamedActionChangedCallback));
        public static DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(System.Windows.Style), typeof(BindableButton), new PropertyMetadata(StyleChangedCallback));
        public static DependencyProperty ButtonToolTipProperty = DependencyProperty.Register(nameof(ButtonToolTip), typeof(object), typeof(BindableButton), new PropertyMetadata(ToolTipChangedCallback));
        public static DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(object), typeof(BindableButton), new PropertyMetadata(ImageChangedCallback));
        private RoutedEventHandler _ActionHandler;
        public HEC.FDA.ViewModel.Utilities.NamedAction NamedAction
        {
            get { return (HEC.FDA.ViewModel.Utilities.NamedAction)GetValue(NamedActionProperty); }
            set { SetValue(NamedActionProperty, value); }
        }
        public Image Image
        {
            get { return (Image)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        public object ButtonToolTip
        {
            get { return (object)GetValue(ButtonToolTipProperty); }
            set { SetValue(ButtonToolTipProperty, value); }
        }
        public bool UseImage
        {
            get;
            set;
        }


        public BindableButton()
        {
            InitializeComponent();
        }
        public static void NamedActionChangedCallback(object sender, DependencyPropertyChangedEventArgs args)
        {
            BindableButton owner = (BindableButton)sender;
            Button btn = owner.TheButton;
            if (owner._ActionHandler != null)
            {
                btn.Click -= owner._ActionHandler;
            }
            HEC.FDA.ViewModel.Utilities.NamedAction na = (HEC.FDA.ViewModel.Utilities.NamedAction)args.NewValue;
            if (!owner.UseImage)
            {
                Binding headerBinding = new Binding("Header");
                headerBinding.Source = na;
                headerBinding.Mode = BindingMode.OneWay;
                headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(btn, Button.ContentProperty, headerBinding);
            }

            Binding enabledBinding = new Binding("IsEnabled");
            enabledBinding.Source = na;
            enabledBinding.Mode = BindingMode.OneWay;
            enabledBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(btn, Button.IsEnabledProperty, enabledBinding);
            Binding visibilityBinding = new Binding("IsVisible");
            visibilityBinding.Source = na;
            visibilityBinding.Mode = BindingMode.OneWay;
            visibilityBinding.Converter = new System.Windows.Controls.BooleanToVisibilityConverter();
            visibilityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(btn, Button.VisibilityProperty, visibilityBinding);
            owner._ActionHandler = (ob, ev) => na.Action(ob, ev);
            btn.Click += owner._ActionHandler;
        }
        public static void ImageChangedCallback(object sender, DependencyPropertyChangedEventArgs args)
        {
            BindableButton owner = (BindableButton)sender;
            if (owner.UseImage)
            {
                Button btn = owner.TheButton;
                btn.Content = args.NewValue;
            }

            //Binding headerBinding = new Binding();
            //headerBinding.Source = 
            //headerBinding.Mode = BindingMode.OneWay;
            //headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //BindingOperations.SetBinding(btn, Button.ContentProperty, headerBinding);
        }
        public static void ToolTipChangedCallback(object sender, DependencyPropertyChangedEventArgs args)
        {
            BindableButton owner = (BindableButton)sender;
            Button btn = owner.TheButton;
            btn.ToolTip = args.NewValue;

        }
        public static void StyleChangedCallback(object sender, DependencyPropertyChangedEventArgs args)
        {
            BindableButton owner = (BindableButton)sender;
            Button btn = owner.TheButton;
            btn.Style = (System.Windows.Style)args.NewValue;

        }
    }
}
