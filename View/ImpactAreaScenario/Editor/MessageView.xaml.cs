using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HEC.FDA.View.ImpactAreaScenario.Editor
{
    public delegate void UpdateDocumentCallBack(Paragraph p);
        /// <summary>
        /// Interaction logic for MessageView.xaml
        /// </summary>
        public partial class MessageView : UserControl
        {
            public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(IMessage), typeof(MessageView), new PropertyMetadata(MessagesChangedCallback));
            public static readonly DependencyProperty MessageCountProperty = DependencyProperty.Register(nameof(MessageCount), typeof(int), typeof(MessageView), new PropertyMetadata(100, MessageCountChangedCallback));
            private SolidColorBrush[] _errorColors = new SolidColorBrush[] { new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Green), new SolidColorBrush(Colors.Goldenrod), new SolidColorBrush(Colors.Orange), new SolidColorBrush(Colors.LightCoral), new SolidColorBrush(Colors.DarkRed) };
            public IMessage Message
            {
                get { return (IMessage)GetValue(MessageProperty); }
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
                IMessage mess = e.NewValue as IMessage;
                Run r = new Run();
                if (mess is IErrorMessage)
                {
                    IErrorMessage err = mess as IErrorMessage;
                    r.Text = err.ToString();
                    owner.tb.Inlines.Add(r);
                    if (err.ErrorLevel.HasFlag(ErrorLevel.Severe))
                    {
                        owner.tb.Inlines.Last().Foreground = owner._errorColors[5];
                    }
                    else if (err.ErrorLevel.HasFlag(ErrorLevel.Fatal))
                    {
                        owner.tb.Inlines.Last().Foreground = owner._errorColors[4];
                    }
                    else if (err.ErrorLevel.HasFlag(ErrorLevel.Major))
                    {
                        owner.tb.Inlines.Last().Foreground = owner._errorColors[3];
                    }
                    else if (err.ErrorLevel.HasFlag(ErrorLevel.Minor))
                    {
                        owner.tb.Inlines.Last().Foreground = owner._errorColors[2];
                    }
                    else if (err.ErrorLevel.HasFlag(ErrorLevel.Info))
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
