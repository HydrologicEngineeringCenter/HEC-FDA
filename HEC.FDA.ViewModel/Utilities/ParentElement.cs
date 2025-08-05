using System.Collections.ObjectModel;


namespace HEC.FDA.ViewModel.Utilities
{
    public abstract class ParentElement: BaseFdaElement
    {
        #region Fields
        protected ObservableCollection<BaseFdaElement> _Elements;

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

        //The "Elements" list needs to be a list of BaseFdaElements because it can hold other parent level elems.
        //We will only be updating child elements.
        public void UpdateElement( ChildElement newElement)
        {
            int index = -1;
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i] is ChildElement childElem)
                {
                    if (childElem.ID == newElement.ID)
                    {
                        index = i;
                        break;
                    }
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
            ele.RequestNavigation += Navigate;
            Elements.Insert(index,ele);
            
            IsExpanded = true;
        }
        public void AddElement(BaseFdaElement ele)
        {
            //the name possibly changed so assign it to the element
            ele.RequestNavigation += Navigate;
            Elements.Add(ele);

            IsExpanded = true;         
        }

        #endregion
    }
}
