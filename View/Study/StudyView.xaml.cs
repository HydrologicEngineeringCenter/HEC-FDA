using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View.Study
{
    /// <summary>
    /// Interaction logic for StudyView.xaml
    /// </summary>
    public partial class StudyView : UserControl
    {

        public StudyView()
        {

            InitializeComponent();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Study.FdaStudyVM vm = (ViewModel.Study.FdaStudyVM)this.DataContext;
            vm.AddMapsTab(MapTreeView);
            vm.AddCreateNewStudyTab();

        }

        private void lbl_Study_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.Study.FdaStudyVM vm = (ViewModel.Study.FdaStudyVM)this.DataContext;

            lbl_Study.ContextMenu.IsOpen = true;
        }


        public bool WasXClicked { get; set; } = false;
        public bool WasPopOutClicked { get; set; } = false;
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WasXClicked = true;
        }



        private void txt_PopOut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WasPopOutClicked = true;
        }

        private void btn_MapView_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_TabsView_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DynamicTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.Study.FdaStudyVM vm = (ViewModel.Study.FdaStudyVM)this.DataContext;
        
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ViewModel.Study.FdaStudyVM vm = (ViewModel.Study.FdaStudyVM)this.DataContext;
                //todo: testing only
                vm.UpdateMapTabTest();
            }));
        }
    }
}
