using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    //[Author(q0heccdm, 11 / 3 / 2016 9:59:31 AM)]
    public class RowItem:BaseViewModel,IRowItem
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 11/3/2016 9:59:31 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public RowItem(object item)
        {
            DisplayName = item.ToString();
        }
        public RowItem(List<string> ColumnValues)
        {
            RowValues = ColumnValues;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion


        private string _DisplayName;
        private List<string> _RowValues;
            private string _IndexPoint;//cody. it is generally a bad idea to have a very specific field in a very generic class...
            //i suggest that you have a row item interface which requires some methods (if there are any) and use polymorphism of interfaces to satisfy the needs of databasecolumn (or table)
            public string DisplayName
            {
                get { return _DisplayName; }
                set { _DisplayName = value; }
            }
        public List<string> RowValues
        {
            get { return _RowValues; }
            set { _RowValues = value; }
        }

       

            public override void AddValidationRules()
            {
                AddRule(nameof(DisplayName), () => DisplayName != null, "Display Name cannot be null.");
                AddRule(nameof(DisplayName), () => DisplayName != "", "Display Name cannot be null.");
               // AddRule(nameof(IndexPoint), () => Int32.Parse(IndexPoint) >= 0, "Index Point must be greater than or equal to zero.");


            }

        
        }

    
}
