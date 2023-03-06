using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;

namespace HEC.FDA.View.Utilities
{
    class ContextMenuConverter : IValueConverter
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
            if (value == null) {
                c.Visibility = System.Windows.Visibility.Collapsed;
                c.IsEnabled = false;
                return c; }
            if (value.GetType() == typeof(List<ViewModel.Utilities.NamedAction>))
            {
                c = new System.Windows.Controls.ContextMenu();
                List<ViewModel.Utilities.NamedAction> Actions = (List<ViewModel.Utilities.NamedAction>)value;
                foreach (HEC.FDA.ViewModel.Utilities.NamedAction Action in Actions)
                {
                    System.Windows.Controls.MenuItem mi = new System.Windows.Controls.MenuItem();
                    if(Action.Header == "seperator") { System.Windows.Controls.Separator s = new System.Windows.Controls.Separator(); c.Items.Add(s);continue; }
                    if (Action.Path!=null)
                    {
                        string[] names = Action.Path.Split(new Char[] { '.' });
                        string firstheader = names[0];
                        bool isNewMI = true;
                        foreach (System.Windows.Controls.Control item in c.Items)
                        {
                            if (item is System.Windows.Controls.MenuItem)
                            {
                                System.Windows.Controls.MenuItem newItem = item as System.Windows.Controls.MenuItem;
                                if (newItem.Header.Equals(firstheader))
                                {
                                    mi = newItem;
                                    isNewMI = false;
                                }
                            }
                        }
                        if (isNewMI) mi.Header = firstheader;
                        System.Windows.Controls.MenuItem basemi = new System.Windows.Controls.MenuItem();
                        //basemi.DataContext = Action;
                        Binding tooltipBinding = new Binding("ToolTip");
                        tooltipBinding.Source = Action;
                        tooltipBinding.Mode = BindingMode.OneWay;
                        tooltipBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(basemi, System.Windows.Controls.MenuItem.ToolTipProperty, tooltipBinding);
                        ToolTipService.SetShowOnDisabled(basemi, true);

                        Binding mybinding = new Binding("IsEnabled");
                        mybinding.Source = Action;
                        mybinding.Mode = BindingMode.TwoWay;
                        mybinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(basemi, System.Windows.Controls.MenuItem.IsEnabledProperty, mybinding);

                        Binding visibilityBinding = new Binding("IsVisible");
                        visibilityBinding.Source = Action;
                        visibilityBinding.Mode = BindingMode.OneWay;
                        visibilityBinding.Converter = new System.Windows.Controls.BooleanToVisibilityConverter();
                        visibilityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(basemi, System.Windows.Controls.MenuItem.VisibilityProperty, visibilityBinding);

                        Binding headerBinding = new Binding("Header");
                        headerBinding.Source = Action;
                        headerBinding.Mode = BindingMode.OneWay;
                        headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(basemi, System.Windows.Controls.MenuItem.HeaderProperty, headerBinding);

                        //basemi.Header = names.Last();
                        basemi.Click += (ob, ev) => Action.Action(Action,ev);
                        System.Windows.Controls.MenuItem tmpmi = new System.Windows.Controls.MenuItem();
                        tmpmi = mi;
                        for (int i = 1; i < names.Count(); i++)
                        {
                            bool miExisted = false;
                            foreach (System.Windows.Controls.Control item in tmpmi.Items)
                            {
                                if (item is System.Windows.Controls.MenuItem)
                                {
                                    System.Windows.Controls.MenuItem newItem = item as System.Windows.Controls.MenuItem;
                                    if (newItem.Header.Equals(names[i]))
                                    {
                                        tmpmi = newItem;
                                        miExisted = true;
                                        isNewMI = false;
                                    }
                                }
                            }
                            if (!miExisted)
                            {
                                System.Windows.Controls.MenuItem submi = new System.Windows.Controls.MenuItem();
                                submi.Header = names[i];
                                tmpmi.Items.Add(submi);
                                tmpmi = submi;
                            }
                        }
                        tmpmi.Items.Add(basemi);

                        if (isNewMI) { c.Items.Add(mi); }
                    }
                    else
                    {
                        Binding tooltipBinding = new Binding("ToolTip");
                        tooltipBinding.Source = Action;
                        tooltipBinding.Mode = BindingMode.OneWay;
                        tooltipBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(mi, System.Windows.Controls.MenuItem.ToolTipProperty, tooltipBinding);
                        ToolTipService.SetShowOnDisabled(mi, true);

                        Binding headerBinding = new Binding("Header");
                        headerBinding.Source = Action;
                        headerBinding.Mode = BindingMode.OneWay;
                        headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(mi, System.Windows.Controls.MenuItem.HeaderProperty, headerBinding);

                        Binding enabledBinding = new Binding("IsEnabled");
                        enabledBinding.Source = Action;
                        enabledBinding.Mode = BindingMode.OneWay;
                        enabledBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(mi, System.Windows.Controls.MenuItem.IsEnabledProperty, enabledBinding);

                        Binding visibilityBinding = new Binding("IsVisible");
                        visibilityBinding.Source = Action;
                        visibilityBinding.Mode = BindingMode.OneWay;
                        visibilityBinding.Converter = new System.Windows.Controls.BooleanToVisibilityConverter();
                        visibilityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(mi, System.Windows.Controls.MenuItem.VisibilityProperty, visibilityBinding);

                        mi.Click += (ob, ev) => Action.Action(Action, ev);
                        c.Items.Add(mi);
                    }
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
