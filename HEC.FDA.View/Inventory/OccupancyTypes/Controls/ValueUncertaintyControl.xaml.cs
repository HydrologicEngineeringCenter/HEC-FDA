﻿using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Inventory.OccupancyTypes.Controls
{
    /// <summary>
    /// Interaction logic for ValueUncertaintyControl.xaml
    /// </summary>
    public partial class ValueUncertaintyControl : UserControl
    {
        public static readonly DependencyProperty ValueUncertaintyVMProperty = DependencyProperty.Register("ValueUncertaintyVM", typeof(ValueUncertaintyVM), typeof(ValueUncertaintyControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ValueUncertaintyVMChangedCallBack)));

        public ValueUncertaintyVM ValueUncertaintyVM
        {
            get { return (ValueUncertaintyVM)this.GetValue(ValueUncertaintyVMProperty); }
            set { this.SetValue(ValueUncertaintyVMProperty, value); }
        }
        public ValueUncertaintyControl()
        {
            InitializeComponent();
        }

        private static void ValueUncertaintyVMChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueUncertaintyControl owner = d as ValueUncertaintyControl;
            ValueUncertaintyVM valueUncertaintyVM = e.NewValue as ValueUncertaintyVM;
            owner.ValueUncertaintyVM = valueUncertaintyVM;
        }

    }
}
