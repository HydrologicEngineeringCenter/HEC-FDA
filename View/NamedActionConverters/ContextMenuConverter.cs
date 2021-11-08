using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace View.NamedActionConverters
{
    public class ContextMenuConverter : IValueConverter
    {
        #region Notes
        //this is a tool that converts a list of viewmodel named action into a contextmenu
        //ideally it will ultimately support binding so that the enabled and visible attributes can update during runtime.
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        #endregion
        #region Voids
        #endregion
        #region Functions
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Controls.ContextMenu c = new System.Windows.Controls.ContextMenu();
            if (value == null)
            {
                c.Visibility = System.Windows.Visibility.Collapsed;
                c.IsEnabled = false;
                return c;
            }
            if (value.GetType() == typeof(List<ViewModel.Interfaces.IDisplayableNamedAction>))
            {
                c = new System.Windows.Controls.ContextMenu();
                List<ViewModel.Interfaces.IDisplayableNamedAction> Actions = (List<ViewModel.Interfaces.IDisplayableNamedAction>)value;
                foreach (ViewModel.Interfaces.IDisplayableNamedAction Action in Actions)
                {
                    System.Windows.Controls.MenuItem mi = new System.Windows.Controls.MenuItem();

                    Binding headerBinding = new Binding("Name");
                    headerBinding.Source = Action;
                    headerBinding.Mode = BindingMode.OneWay;
                    headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    BindingOperations.SetBinding(mi, System.Windows.Controls.MenuItem.HeaderProperty, headerBinding);
                    Binding enabledBinding = new Binding("Enabled");
                    enabledBinding.Source = Action;
                    enabledBinding.Mode = BindingMode.OneWay;
                    enabledBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    BindingOperations.SetBinding(mi, System.Windows.Controls.MenuItem.IsEnabledProperty, enabledBinding);
                    Binding visibilityBinding = new Binding("Visible");
                    visibilityBinding.Source = Action;
                    visibilityBinding.Mode = BindingMode.OneWay;
                    visibilityBinding.Converter = new System.Windows.Controls.BooleanToVisibilityConverter();
                    visibilityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    BindingOperations.SetBinding(mi, System.Windows.Controls.MenuItem.VisibilityProperty, visibilityBinding);
                    mi.Click += (ob, ev) => Action.Action(ob, ev);
                    c.Items.Add(mi);

                }
                if (!c.HasItems)
                {
                    c.Visibility = System.Windows.Visibility.Collapsed;
                    c.IsEnabled = false;
                }
                return c;
            }
            return c;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
