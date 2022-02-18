using System;
using System.Collections.ObjectModel;


namespace HEC.FDA.ViewModel.Utilities
{
    public abstract class ParentElement: BaseFdaElement
    {
        #region Notes
        #endregion
        #region Fields
        public event EventHandler RenameMapTreeViewElement;
        public event EventHandler AddMapTreeViewElementBackIn;

        protected ObservableCollection<BaseFdaElement> _Elements;

        #endregion
        #region Properties
        public ObservableCollection<BaseFdaElement> Elements
        {
            get { return _Elements; }
            set { _Elements = value;  NotifyPropertyChanged(nameof(Elements)); }
        }
  
        #endregion
        #region Constructors
        public ParentElement(): base()
        {
            _Elements = new ObservableCollection<BaseFdaElement>();
        }
        #endregion
        #region Voids
        
        public void RemoveElement(BaseFdaElement element)
        {
            for(int i = 0;i<Elements.Count;i++)
            {
                if(Elements[i].Name.Equals(element.Name))
                {
                    Elements.RemoveAt(i);
                }
            }
            NotifyPropertyChanged();
        }

        public void UpdateElement(BaseFdaElement oldElement, BaseFdaElement newElement)
        {
            int index = -1;
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {               
                Elements.RemoveAt(index);
                InsertElement(index, newElement);
            }
        }

        public void InsertElement(int index, BaseFdaElement ele)
        {
            ele.RenameMapTreeViewElement += RenameMapTreeViewElement;
            ele.AddMapTreeViewElementBackIn += AddMapTreeViewElementBackIn;
            ele.RequestNavigation += Navigate;
            ele.RequestShapefilePaths += ShapefilePaths;
            ele.RequestShapefilePathsOfType += ShapefilePathsOfType;
            ele.RequestAddToMapWindow += AddToMapWindow;
            ele.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            ele.TransactionEvent += AddTransaction;
            Elements.Insert(index,ele);
            
            IsExpanded = true;
        }
        public void AddElement(BaseFdaElement ele, bool newElement = true)
        {
            //the name possibly changed so assign it to the element
            ele.RenameMapTreeViewElement += RenameMapTreeViewElement;
            ele.AddMapTreeViewElementBackIn += AddMapTreeViewElementBackIn;
            ele.RequestNavigation += Navigate;
            ele.RequestShapefilePaths += ShapefilePaths;
            ele.RequestShapefilePathsOfType += ShapefilePathsOfType;
            ele.RequestAddToMapWindow += AddToMapWindow;
            ele.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            ele.TransactionEvent += AddTransaction;
            Elements.Add(ele);

            IsExpanded = true;         
        }

        #endregion
    }
}
