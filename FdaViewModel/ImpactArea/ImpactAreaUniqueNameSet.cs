using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace FdaViewModel.ImpactArea
{
    public class ImpactAreaUniqueNameSet: BaseViewModel
    {
        #region Notes
        #endregion
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
            foreach(string name in polyNames)
            {
                tmp.Add(new ImpactAreaRowItem(name, 0.0F, tmp));
            }

            RowItems = new ObservableCollection<ImpactAreaRowItem>();

            foreach(ImpactAreaRowItem row in tmp)
            {
                RowItems.Add(row);
            }
            
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
