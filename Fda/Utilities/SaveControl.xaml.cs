using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Fda.Utilities
{
    /// <summary>
    /// Interaction logic for SaveControl.xaml
    /// </summary>
    public partial class SaveControl : UserControl
    {
        //public static DependencyProperty ClickProperty = DependencyProperty.Register(nameof(Click), typeof(FdaViewModel.Utilities.NamedAction), typeof(BindableButton), new PropertyMetadata(ClickCallback));


        bool longClick = false;
        //TimeSpan holdTime = TimeSpan.FromMilliseconds(500);
        int holdTime = 200; //milliseconds, i think this is actually slower because of the delay on the repeat buttons
        Stopwatch sw = new Stopwatch();

        public SaveControl()
        {
            InitializeComponent();
        }

        private void btn_Undo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            longClick = false;
            sw.Reset();
            sw.Start();
        }

        private void btn_Undo_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (longClick == false)
            {
                FdaViewModel.Utilities.ISaveUndoRedo vm = (FdaViewModel.Utilities.ISaveUndoRedo)this.DataContext;
                vm.Undo();
            }
        }

        private void btn_Undo_Click(object sender, RoutedEventArgs e)
        {
            if(sw.ElapsedMilliseconds> holdTime && longClick == false)
            {
                longClick = true;
                cmb_UndoDropDown.IsDropDownOpen = true;

            }
        }

        private void btn_Redo_Click(object sender, RoutedEventArgs e)
        {
            if (sw.ElapsedMilliseconds > holdTime && longClick == false)
            {
                longClick = true;
                cmb_RedoDropDown.IsDropDownOpen = true;

            }
        }

        private void btn_Redo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            longClick = false;
            sw.Reset();
            sw.Start();
        }

        private void btn_Redo_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (longClick == false)
            {
                FdaViewModel.Utilities.ISaveUndoRedo vm = (FdaViewModel.Utilities.ISaveUndoRedo)this.DataContext;
                vm.Redo();
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            //img_saveEnabled.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/save_disabled.png"));
            FdaViewModel.Utilities.ISaveUndoRedo vm = (FdaViewModel.Utilities.ISaveUndoRedo)this.DataContext;
            vm.SaveWhileEditing();
        }

        private void btn_Undo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cmb_UndoDropDown.IsDropDownOpen = true;
        }

        private void btn_Redo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cmb_RedoDropDown.IsDropDownOpen = true;

        }
    }
}
