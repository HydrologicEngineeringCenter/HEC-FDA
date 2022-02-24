using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using OxyPlot;
using System.Windows.Media.Imaging;

namespace HEC.FDA.View.TableWithPlot
{
    public class ExpandedOxyPlot : OxyPlot.Wpf.PlotView
    {
        private readonly RadioButton _selectButton;
        private readonly RadioButton _panButton;
        private readonly RadioButton _zoomButton;
        private readonly Button _zoomAllButton;
        // 
        private readonly StackPanel _expandableToolsPanel;
        private readonly StackPanel _toolsPanel;
        private readonly Button _expanderButton;
        private readonly System.Windows.Shapes.Path _expanderArrow;
        private readonly SolidColorBrush _bg = new SolidColorBrush(Color.FromArgb(127, 240, 248, 255));
        // 
        private bool _rightMouseTrack = true;
        public ExpandedOxyPlot() : base()
        {
            // Create collapsable toolbar
            _expandableToolsPanel = new StackPanel() { Orientation = Orientation.Horizontal, Name = "ExpandableToolsPanel", VerticalAlignment = System.Windows.VerticalAlignment.Top, HorizontalAlignment = System.Windows.HorizontalAlignment.Left };
            _toolsPanel = new StackPanel() { Orientation = Orientation.Vertical, Name = "ToolsPanel", VerticalAlignment = System.Windows.VerticalAlignment.Top, HorizontalAlignment = System.Windows.HorizontalAlignment.Left, Background = _bg };

            Image selectIcon = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/View;component/Resources/TrackPlot.png")) };

            _selectButton = new RadioButton() { Content = selectIcon, Margin = new Thickness(0, 0, 1, 0), Height = 22, ToolTip = "Track Data", Style = (Style)FindResource(ToolBar.RadioButtonStyleKey) };
            _selectButton.Click += SelectButton_Click;
            // 
            Image panIcon = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/View;component/Resources/PanHand.png")) };
            _panButton = new RadioButton() { Content = panIcon, Margin = new Thickness(0, 1, 1, 0), Height = 22, ToolTip = "Pan", Style = (Style)FindResource(ToolBar.RadioButtonStyleKey) };
            _panButton.Click += PanButton_Click;
            // 
            Image zoomIcon = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/View;component/Resources/ZoomIn.png")) };
            _zoomButton = new RadioButton() { Content = zoomIcon, Margin = new Thickness(0, 1, 1, 0), Height = 22, ToolTip = "Zoom In", Style = (Style)FindResource(ToolBar.RadioButtonStyleKey) };
            _zoomButton.Click += ZoomButton_Click;
            // 
            Image zoomAllIcon = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/View;component/Resources/ZoomAll.png")) };
            _zoomAllButton = new Button() { Content = zoomAllIcon, Margin = new Thickness(0, 1, 1, 0), Height = 22, ToolTip = "Zoom To All (esc)", Style = (Style)FindResource(ToolBar.ButtonStyleKey) };
            _zoomAllButton.Click += ZoomAllButton_Click;

            _toolsPanel.Children.Add(_selectButton);
            _toolsPanel.Children.Add(_panButton);
            _toolsPanel.Children.Add(_zoomButton);
            _toolsPanel.Children.Add(_zoomAllButton);
            // 
            _expanderArrow = new System.Windows.Shapes.Path() { Name = "ExpanderArrow", Margin = new Thickness(1, 0, 1, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, SnapsToDevicePixels = false, Stroke = new SolidColorBrush(Color.FromArgb(230, 70, 88, 102)), StrokeThickness = 2 };
            _expanderArrow.Data = Geometry.Parse("M 4,0 L 0,4 L 4,8");
            // 
            _expanderButton = new Button() { Content = _expanderArrow, Name = "ExpanderButton", VerticalAlignment = System.Windows.VerticalAlignment.Stretch, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, ToolTip = "Hide Tools", Style = (Style)FindResource(ToolBar.ButtonStyleKey) };
            {
                var withBlock = _expanderButton;
                withBlock.Background = _bg;
                withBlock.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 245, 250, 255));
                withBlock.BorderThickness = new Thickness(1);
            }
            BindingOperations.SetBinding(_expanderButton, HeightProperty, new Binding("ActualHeight") { Source = _toolsPanel });
            _expanderButton.Click += ExpanderButtonButton_Click;
            // 
            _expandableToolsPanel.Children.Add(_toolsPanel);
            _expandableToolsPanel.Children.Add(_expanderButton);
            // 
            LayoutUpdated += (object se, EventArgs ea) =>
            {
                {
                    if (Model != null)
                    {
                        if (Model.Title != null)
                        {
                            _expandableToolsPanel.Margin = new Thickness(Model.ActualPlotMargins.Left + Model.Padding.Left + 1, Model.ActualPlotMargins.Top + Model.TitleArea.Bottom - Model.TitlePadding + 1, 0, 0);
                        }
                        else
                        {
                            _expandableToolsPanel.Margin = new Thickness(Model.ActualPlotMargins.Left + Model.Padding.Left + 1, Model.ActualPlotMargins.Top + Model.Padding.Top + 1, 0, 0);
                        }
                    }


                }
            };
            this.Loaded += ExpandedOxyPlot_Loaded;

            // 
            _selectButton.IsChecked = true;
            SelectButton_Click(null, null);
        }
        private void ExpandedOxyPlot_Loaded(object sender, RoutedEventArgs e)
        {
            Grid g = GetTemplateChild(PartGrid) as Grid;
            if (g != null)
            {
                foreach (var child in g.Children)
                {
                    Grid childGrid = child as Grid;
                    if (childGrid != null)
                    {
                        childGrid.Children.Add(_expandableToolsPanel);
                        break;
                    }
                }

            }
        }
        public void EnableRightMouseTrack()
        {
            _rightMouseTrack = true;
            if ((bool)_selectButton.IsChecked)
                SelectButton_Click(null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */);
            if ((bool)_zoomButton.IsChecked)
                ZoomButton_Click(null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */);
            if ((bool)_panButton.IsChecked)
                PanButton_Click(null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */);
        }
        public void DisableRightMouseTrack()
        {
            _rightMouseTrack = false;
            if ((bool)_selectButton.IsChecked)
                SelectButton_Click(null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */);
            if ((bool)_zoomButton.IsChecked)
                ZoomButton_Click(null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */);
            if ((bool)_panButton.IsChecked)
                PanButton_Click(null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */);
        }
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            {
                var withBlock = ActualController;
                withBlock.UnbindAll();
                withBlock.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);
                withBlock.BindMouseDown(OxyMouseButton.Left, PlotCommands.SnapTrack);
                if (_rightMouseTrack)
                    withBlock.BindMouseDown(OxyMouseButton.Right, PlotCommands.SnapTrack);
                withBlock.BindMouseWheel(PlotCommands.ZoomWheel);
                withBlock.BindKeyDown(OxyKey.Escape, PlotCommands.Reset);
            }
        }
        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            {
                var withBlock = ActualController;
                withBlock.UnbindAll();
                withBlock.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);
                withBlock.BindMouseDown(OxyMouseButton.Left, PlotCommands.ZoomRectangle);
                if (_rightMouseTrack)
                    withBlock.BindMouseDown(OxyMouseButton.Right, PlotCommands.SnapTrack);
                withBlock.BindMouseWheel(PlotCommands.ZoomWheel);
                withBlock.BindKeyDown(OxyKey.Escape, PlotCommands.Reset);
            }
        }

        private void PanButton_Click(object sender, RoutedEventArgs e)
        {
            {
                var withBlock = ActualController;
                withBlock.UnbindAll();
                withBlock.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);
                withBlock.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
                if (_rightMouseTrack)
                    withBlock.BindMouseDown(OxyMouseButton.Right, PlotCommands.SnapTrack);
                withBlock.BindMouseWheel(PlotCommands.ZoomWheel);
                withBlock.BindKeyDown(OxyKey.Escape, PlotCommands.Reset);
            }
        }

        private void ZoomAllButton_Click(object sender, RoutedEventArgs e)
        {
            ResetAllAxes();
            InvalidatePlot(false);
            Focus();
        }
        private void ExpanderButtonButton_Click(object sender, RoutedEventArgs e)
        {
            if (_toolsPanel.Visibility == Visibility.Collapsed)
            {
                _toolsPanel.Visibility = Visibility.Visible;
                _expanderArrow.Data = Geometry.Parse("M 4,0 L 0,4 L 4,8");
                _expanderButton.ToolTip = "Hide Tools";
            }
            else
            {
                _toolsPanel.Visibility = Visibility.Collapsed;
                _expanderArrow.Data = Geometry.Parse("M 0,0 L 4,4 L 0,8");
                _expanderButton.ToolTip = "Show Tools";
            }
            Focus();
        }
    }
}
