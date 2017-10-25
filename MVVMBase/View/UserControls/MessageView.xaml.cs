using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace View.UserControls
{
    public delegate void UpdateDocumentCallBack(Paragraph p);
    /// <summary>
    /// Interaction logic for MessageView.xaml
    /// </summary>
    public partial class MessageView : UserControl
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(Base.Interfaces.IMessage), typeof(MessageView), new PropertyMetadata(MessagesChangedCallback));
        public static readonly DependencyProperty MessageCountProperty = DependencyProperty.Register(nameof(MessageCount), typeof(int), typeof(MessageView), new PropertyMetadata(100, MessageCountChangedCallback));
        private SolidColorBrush[] _errorColors = new SolidColorBrush[] { new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Green), new SolidColorBrush(Colors.Goldenrod), new SolidColorBrush(Colors.Orange), new SolidColorBrush(Colors.LightCoral), new SolidColorBrush(Colors.DarkRed) };
        public Base.Interfaces.IMessage Message
        {
            get { return (Base.Interfaces.IMessage)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public int MessageCount
        {
            get { return (int)GetValue(MessageCountProperty); }
            set { SetValue(MessageCountProperty, value); }
        }
        public MessageView()
        {
            InitializeComponent();
        }
        private static void MessageCountChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MessageView owner = (MessageView)d;
            owner.tb.Inlines.Clear();
        }
        private static void MessagesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MessageView owner = (MessageView)d;
            Base.Interfaces.IMessage mess = e.NewValue as Base.Interfaces.IMessage;
            Run r = new Run();
            if (mess is Base.Interfaces.IErrorMessage)
            {
                Base.Interfaces.IErrorMessage err = mess as Base.Interfaces.IErrorMessage;
                r.Text = err.ToString();
                owner.tb.Inlines.Add(r);
                if (err.ErrorLevel.HasFlag(Base.Enumerations.ErrorLevel.Severe))
                {
                    owner.tb.Inlines.Last().Foreground = owner._errorColors[5];
                }
                else if (err.ErrorLevel.HasFlag(Base.Enumerations.ErrorLevel.Fatal))
                {
                    owner.tb.Inlines.Last().Foreground = owner._errorColors[4];
                }
                else if (err.ErrorLevel.HasFlag(Base.Enumerations.ErrorLevel.Major))
                {
                    owner.tb.Inlines.Last().Foreground = owner._errorColors[3];
                }
                else if (err.ErrorLevel.HasFlag(Base.Enumerations.ErrorLevel.Minor))
                {
                    owner.tb.Inlines.Last().Foreground = owner._errorColors[2];
                }
                else if (err.ErrorLevel.HasFlag(Base.Enumerations.ErrorLevel.Info))
                {
                    owner.tb.Inlines.Last().Foreground = owner._errorColors[1];
                }
            }
            else
            {
                r.Text = mess.Message;
                owner.tb.Inlines.Add(r);
            }
            if (owner.tb.Inlines.Count() > owner.MessageCount)
            {
                owner.tb.Inlines.Remove(owner.tb.Inlines.First());
            }
        }
    }
}
