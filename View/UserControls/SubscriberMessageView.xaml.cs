using System;
using System.Windows;
using System.Windows.Controls;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Implementations;

namespace HEC.MVVMFramework.View.UserControls
{
    /// <summary>
    /// Interaction logic for SubscriberMessageView.xaml
    /// </summary>
    public partial class SubscriberMessageView : UserControl
    {
        public static readonly DependencyProperty FilterLevelProperty = DependencyProperty.Register(nameof(FilterLevel), typeof(ErrorLevel), typeof(SubscriberMessageView), new PropertyMetadata(ErrorLevel.Unassigned, FilterLevelChangedCallback));
        public static readonly DependencyProperty SenderTypeFilterProperty = DependencyProperty.Register(nameof(SenderTypeFilter), typeof(System.Type), typeof(SubscriberMessageView), new PropertyMetadata(null, SenderTypeFilterChangedCallback));
        public static readonly DependencyProperty MessageTypeFilterProperty = DependencyProperty.Register(nameof(MessageTypeFilter), typeof(System.Type), typeof(SubscriberMessageView), new PropertyMetadata(null, MessageTypeFilterChangedCallback));
        public static readonly DependencyProperty MessageCountProperty = DependencyProperty.Register(nameof(MessageCount), typeof(int), typeof(SubscriberMessageView), new PropertyMetadata(100, MessageCountChangedCallback));
        public static readonly DependencyProperty InstanceFilterProperty = DependencyProperty.Register(nameof(InstanceFilter), typeof(int), typeof(SubscriberMessageView), new PropertyMetadata(null, InstanceFilterPropertyChangedCallback));

        private ViewModel.Implementations.SubscriberMessageViewModel _vm = new ViewModel.Implementations.SubscriberMessageViewModel();
        public int MessageCount
        {
            get { return (int)GetValue(MessageCountProperty); }
            set { SetValue(MessageCountProperty, value); }
        }
        public int InstanceFilter
        {
            get 
            { 
                return (int)GetValue(InstanceFilterProperty);
            }
            set
            {
                SetValue(InstanceFilterProperty, value);
            }
        }
        public ErrorLevel FilterLevel
        {
            get
            {
                return (ErrorLevel)GetValue(FilterLevelProperty);
            }
            set
            {
                SetValue(FilterLevelProperty, value);
            }
        }
        public Type SenderTypeFilter
        {
            get
            {
                return (Type)GetValue(SenderTypeFilterProperty);
            }
            set
            {
                SetValue(SenderTypeFilterProperty, value);
            }
        }
        public Type MessageTypeFilter
        {
            get
            {
                return (Type)GetValue(MessageTypeFilterProperty);
            }
            set
            {
                SetValue(MessageTypeFilterProperty, value);
            }
        }
        public SubscriberMessageView()
        {
            InitializeComponent();
            DataContext = _vm;
            _vm.FilterLevel = FilterLevel;
            _vm.SenderTypeFilter = SenderTypeFilter;
            _vm.MessageTypeFilter = MessageTypeFilter;
            _vm.InstanceFilter = InstanceFilter;
            MessageHub.Subscribe(_vm);
        }
        private static void FilterLevelChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubscriberMessageView v = d as SubscriberMessageView;
            v._vm.FilterLevel = v.FilterLevel;
        }
        private static void SenderTypeFilterChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubscriberMessageView v = d as SubscriberMessageView;
            v._vm.SenderTypeFilter = v.SenderTypeFilter;
        }
        private static void MessageTypeFilterChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubscriberMessageView v = d as SubscriberMessageView;
            v._vm.MessageTypeFilter = v.MessageTypeFilter;
        }
        private static void MessageCountChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubscriberMessageView v = d as SubscriberMessageView;
            v._vm.MessageCount = v.MessageCount;
        }
        private static void InstanceFilterPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubscriberMessageView v = d as SubscriberMessageView;
            v._vm.InstanceFilter = v.InstanceFilter;
        }
    }
}
