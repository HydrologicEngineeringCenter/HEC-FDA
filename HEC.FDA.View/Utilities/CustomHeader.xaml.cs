using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HEC.FDA.View.Utilities
{
    public partial class CustomHeader : UserControl
    {
        public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(CustomHeader),
            new FrameworkPropertyMetadata(false, OnIsSelectedChanged));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        private bool _isMouseOverClickable;
        public CustomHeader()
        {
            InitializeComponent();
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomHeader)d;
            control.UpdateBackground();
        }

        private void ClickableArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Toggle or set true; choose behavior you want
            IsSelected = !IsSelected;
            e.Handled = true;
        }

        private void ClickableArea_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOverClickable = true;
            UpdateBackground();
        }

        private void ClickableArea_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOverClickable = false;
            UpdateBackground();
        }

        private void UpdateBackground()
        {
            if (ClickableArea == null)
                return;

            if (IsSelected)
            {
                // Selected color
                ClickableArea.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCCE5FF"));
            }
            else if (_isMouseOverClickable)
            {
                // Hover color (only when *not* selected)
                ClickableArea.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE0F0FF"));
            }
            else
            {
                // Default background
                ClickableArea.Background = Brushes.Transparent;
            }
        }
    }
}
