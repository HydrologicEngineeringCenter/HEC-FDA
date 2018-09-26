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

namespace Fda.Utilities
{
    /// <summary>
    /// Interaction logic for SaveControl.xaml
    /// </summary>
    public partial class SaveControl : UserControl
    {
        //public static DependencyProperty ClickProperty = DependencyProperty.Register(nameof(Click), typeof(FdaViewModel.Utilities.NamedAction), typeof(BindableButton), new PropertyMetadata(ClickCallback));


        //public object Click
        //{
        //    get { return (object)GetValue(ClickProperty); }
        //    set { SetValue(ClickProperty, value); }
        //}

        public SaveControl()
        {
            InitializeComponent();
        }

        //public static void ClickCallback(object sender, DependencyPropertyChangedEventArgs args)
        //{
        //}
    }
}
