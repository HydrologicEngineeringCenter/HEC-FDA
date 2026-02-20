using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Compute;

public partial class ProgressControl : UserControl
{
    private INotifyCollectionChanged _subscribedCollection;

    public ProgressControl()
    {
        InitializeComponent();

        // Subscribe when ItemsSource changes (after binding resolves)
        DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ListView))
            .AddValueChanged(MessagesListView, OnItemsSourceChanged);
    }

    private void OnItemsSourceChanged(object sender, System.EventArgs e)
    {
        // Unsubscribe from old collection
        if (_subscribedCollection != null)
        {
            _subscribedCollection.CollectionChanged -= OnMessagesCollectionChanged;
            _subscribedCollection = null;
        }

        // Subscribe to new collection
        if (MessagesListView.ItemsSource is INotifyCollectionChanged collection)
        {
            _subscribedCollection = collection;
            collection.CollectionChanged += OnMessagesCollectionChanged;
        }
    }

    private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && MessagesListView.Items.Count > 0)
        {
            var lastItem = MessagesListView.Items[MessagesListView.Items.Count - 1];
            MessagesListView.ScrollIntoView(lastItem);
        }
    }
}
