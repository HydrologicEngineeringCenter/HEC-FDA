using HEC.FDA.ViewModel.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for TabHeaderControl.xaml
    /// </summary>
    public partial class TabHeaderControl : UserControl
    {
        private bool _MouseDown;
        private Point _StartPoint;

        public TabHeaderControl()
        {
            InitializeComponent();
        }

        private void btn_PopOut_Click(object sender, RoutedEventArgs e)
        {
            DynamicTabVM vm = (DynamicTabVM)this.DataContext;
            vm.PopTabIntoWindow();
        }

            private void btn_Close_Click(object sender, RoutedEventArgs e)
        {

            DynamicTabVM vm = (DynamicTabVM)this.DataContext;
            vm.RemoveTab();

            //DependencyObject currentControl = (DependencyObject)sender;
            //Type targetType = typeof(TabItem);

            //while (currentControl != null)
            //{
            //    if (currentControl.GetType() == targetType)
            //    {
            //        //DynamicTabControl.SelectedItem = currentControl;
            //    }
            //    else
            //    {
            //        currentControl = LogicalTreeHelper.GetParent(currentControl);
            //    }
            //}
            //ViewModel.Study.FdaStudyVM vm = (HEC.FDA.ViewModel.Study.FdaStudyVM)this.DataContext;
            ////if (vm.Tabs[DynamicTabControl.SelectedIndex].CanDelete == true)
            //{
            //    //vm.Tabs.RemoveAt(DynamicTabControl.SelectedIndex);
            //}
        }

        //private void btn_PopOut_Click(object sender, RoutedEventArgs e)
        //{
        //    DynamicTabVM vm = (DynamicTabVM)this.DataContext;
        //    vm.PopTabIntoWindow();
        //}

        //private void TextBlock_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_MouseDown && HasDraggedMinimumDistance(e))
        //    {
        //        Mouse.Capture(null);//releases the capture
        //        if (this.DataContext is DynamicTabVM)
        //        {
        //            DynamicTabVM vm = (DynamicTabVM)this.DataContext;
        //            if (vm.CanPopOut)
        //            {
        //                vm.PopTabIntoWindowDragging();
        //            }
        //        }
        //    }

        //}

       //private bool HasDraggedMinimumDistance(MouseEventArgs e)
       // {
       //     return Math.Abs(e.GetPosition(null).X - _StartPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
       //     Math.Abs(e.GetPosition(null).Y - _StartPoint.Y) >= SystemParameters.MinimumVerticalDragDistance;           
       // }

       // private void TextBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
       // {
       //     _MouseDown = false;
       //     Mouse.Capture(null);//releases the capture
       // }
       

       // private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
       // {
       //     _MouseDown = true;
       //     _StartPoint = e.GetPosition(null);
       //     Mouse.Capture(txt_block);
       
       // }

        
    }
}
