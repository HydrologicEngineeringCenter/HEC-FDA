﻿using HEC.FDA.ViewModel.AggregatedStageDamage;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HEC.FDA.View.AggregatedStageDamage
{
    public class StageDischargeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is FrequencyElementWrapper wrapper)
            {
                if(wrapper.Element != null)
                {
                    if(!wrapper.Element.IsAnalytical)
                    {
                        if(!wrapper.Element.GraphicalUsesFlow)
                        {
                            return Visibility.Hidden;
                        }
                    }
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
