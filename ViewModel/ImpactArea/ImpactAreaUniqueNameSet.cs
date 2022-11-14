using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreaUniqueNameSet: BaseViewModel
    {
        #region Fields
        private string _ColumnName;
        private ObservableCollection<ImpactAreaRowItem> _RowItems;
        #endregion
        #region Properties
        public string ColumnName { get { return _ColumnName; } set { _ColumnName = value; NotifyPropertyChanged(); } }
        public ObservableCollection<ImpactAreaRowItem> RowItems { get { return _RowItems; } set { _RowItems = value; NotifyPropertyChanged(); } }
        #endregion
        #region Constructors
        public ImpactAreaUniqueNameSet(string ColName, object[] polyNames)
        {
            ColumnName = ColName;
            ObservableCollection<object> tmp = new ObservableCollection<object>();
            int i = 0;
            foreach(object name in polyNames)
            {
                tmp.Add(new ImpactAreaRowItem(i,name.ToString()));
                i++;
            }

            RowItems = new ObservableCollection<ImpactAreaRowItem>();

            foreach(ImpactAreaRowItem row in tmp)
            {
                RowItems.Add(row);
            }          
        }     
        #endregion
    }
}
