using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.ViewModel.Implementations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HEC.MVVMFramework.View.NamedActionConverters
{
    /// <summary>
    /// Interaction logic for ImageNamedActionButton.xaml
    /// </summary>
    public partial class ImageNamedActionButton : UserControl
    {
        public static DependencyProperty ButtonNamedActionProperty = DependencyProperty.Register(nameof(NamedAction), typeof(INamedAction), typeof(ImageNamedActionButton), new FrameworkPropertyMetadata(null,ButtonNamedActionPropertyCallback, ButtonNamedActionPropertyCallbackCoerce), new ValidateValueCallback(IsValidNamedAction));
        public static DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(object), typeof(ImageNamedActionButton), new PropertyMetadata(ImageChangedCallback));
        public static DependencyProperty ButtonToolTipProperty = DependencyProperty.Register(nameof(ButtonToolTip), typeof(object), typeof(ImageNamedActionButton), new PropertyMetadata(ToolTipChangedCallback));
        public static DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(System.Windows.Style), typeof(ImageNamedActionButton), new PropertyMetadata(StyleChangedCallback));
        private RoutedEventHandler _ActionHandler;
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
        private bool _useImage = true;
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
        public ImageNamedActionButton()
        {
            InitializeComponent();
        }
        public static bool IsValidNamedAction(object value)
        {
            return true;
        }
        private static object ButtonNamedActionPropertyCallbackCoerce(DependencyObject d, object basevalue)
        {
            //the only thing i can think of is to put a default name in the named action that is in the default dependency property and trigger the property callback to disable the button if the property doesnt exist.
            if (basevalue == null)
            {
                return null;
            }
            return basevalue;
        }
        private static void ButtonNamedActionPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageNamedActionButton nab = (ImageNamedActionButton)d;
            Button btn = nab.btn;
            if (nab._ActionHandler != null)
            {
                btn.Click -= nab._ActionHandler;
            }
            INamedAction na = (INamedAction)e.NewValue;
            if (na == null)
            {
                btn.Visibility = Visibility.Collapsed;
                return;
            }else
            {
                if(na is NamedAction)
                {
                    NamedAction vna = na as NamedAction;
                    if (!vna.IsVisible)
                    {
                        btn.Visibility = Visibility.Collapsed;
                    }
                }
                btn.Visibility = Visibility.Visible;
            }
            if (!nab._useImage)
            {
                Binding headerBinding = new Binding(nameof(INamedAction.Name));
                headerBinding.Source = na;
                headerBinding.Mode = BindingMode.OneWay;
                headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(btn, Button.ContentProperty, headerBinding);
            }
            if(na is ViewModel.Interfaces.IDisplayToUI)
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
        public static void ImageChangedCallback(object sender, DependencyPropertyChangedEventArgs args)
        {
            ImageNamedActionButton owner = (ImageNamedActionButton)sender;
            owner._useImage = args.NewValue != null;
            if (owner._useImage)
            {
                Button btn = owner.btn;
                btn.Content = args.NewValue;
            }
        }
        public static void ToolTipChangedCallback(object sender, DependencyPropertyChangedEventArgs args)
        {
            ImageNamedActionButton owner = (ImageNamedActionButton)sender;
            Button btn = owner.btn;
            btn.ToolTip = args.NewValue;
        }
        public static void StyleChangedCallback(object sender, DependencyPropertyChangedEventArgs args)
        {
            ImageNamedActionButton owner = (ImageNamedActionButton)sender;
            Button btn = owner.btn;
            btn.Style = (System.Windows.Style)args.NewValue;

        }
    }
}
