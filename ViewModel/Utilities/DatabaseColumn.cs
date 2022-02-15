using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities
{
    //[Author("q0heccdm", "10 / 14 / 2016 10:17:27 AM")]
    public class DatabaseColumn : BaseViewModel //suggest a renaming because technically row item could represent many columns (it represents 2 in the case you have made)
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/14/2016 10:17:27 AM
        #endregion
        #region Fields
        private List<IRowItem> _Data;
        private string _Header;
        #endregion
        #region Properties
        public string Header
        {
            get { return _Header; }
            set { _Header = value; NotifyPropertyChanged(); }
        }
        public List<IRowItem> Data // this is good, and row item should be able to have as many properties as necessary, each representing a column itself.
        {
            get { return _Data; }
            set { _Data = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors

        public DatabaseColumn():base()
        {
            //Data = new List<RowItem> { new RowItem("test1",0),new RowItem("test2",0) };

        }
        public DatabaseColumn(List<IRowItem> listOfRows)
        {
            Data = listOfRows;
        }
        /// <summary>
        /// This assumes that there is only one table. If there are multiple tables it will access the first one.
        /// </summary>
        /// <param name="path">If the path is not a *.dbf, it will be changed to a .dbf extension.</param>
        /// <param name="columnName"></param>
        public DatabaseColumn(string path,string columnName)
        {
            Data = new List<IRowItem>();
            Header = columnName;
            DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(System.IO.Path.ChangeExtension(path, ".dbf"));
            DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);
            List<object> tempList = dtv.GetColumn(columnName).ToList();

            
            
                foreach (object item in tempList)
                {
                    Data.Add(new RowItem(item));
                }
            
            //else
            //{
            //    for (int i = 0; i < tempList.Count; i++)
            //    {
            //        Data.Add(new RowItem(optionalConstantValue));
            //    }
            //}

        }
        public DatabaseColumn(int numberOfRows, string constantValue)
        {
            Data = new List<IRowItem>();
            for (int i = 0; i < numberOfRows; i++)
            {
                Data.Add(new RowItem(constantValue));
            }
        }

     

        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

   


      

    }
}
