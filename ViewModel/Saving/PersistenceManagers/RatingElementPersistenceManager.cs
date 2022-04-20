using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.TableWithPlot;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class RatingElementPersistenceManager : SavingBase
    {
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int CURVE_COL = 4;

        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "rating_curves"; } }

        /// <summary>
        /// Names of the columns in the parent table
        /// </summary>
        public override string[] TableColumnNames
        {
            get {return new string[]{ NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE};}
        }
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[]{ typeof(string), typeof(string), typeof(string), typeof(string) }; }
        }      

        #region constructor
        public RatingElementPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #endregion

        #region utilities

        /// <summary>
        /// Turns the element into an object[] for the row in the parent table
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override object[] GetRowDataFromElement(ChildElement element)
        {
            if(element.Description == null)
            {
                element.Description = "";
            }
            
            if(element is CurveChildElement curveChildElement)
            {

                return new object[] { element.Name, element.LastEditDate, element.Description, curveChildElement.ComputeComponentVM.ToXML().ToString()};
            }
            else
            {
                return null;
            }          
        }
       
        /// <summary>
        /// Creates an element from the row in the parent table.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            string curveXML = (string)rowData[CURVE_COL];
            ComputeComponentVM computeComponentVM = new ComputeComponentVM(XElement.Parse(curveXML));
            RatingCurveElement rc = new RatingCurveElement((string)rowData[NAME_COL], (string)rowData[LAST_EDIT_DATE_COL],
                (string)rowData[DESC_COL], computeComponentVM, id);
            return rc;
        }

        #endregion

        /// <summary>
        /// Saves a new element to the parent table and the change table
        /// </summary>
        /// <param name="element"></param>
        public void SaveNew(ChildElement element)
        {
            //save to parent table
            base.SaveNew(element);
        }
       
        /// <summary>
        /// Remove the element from the parent table, all references to it in the change table, and all references to it in the log tables.
        /// </summary>
        /// <param name="element"></param>
        public void Remove(ChildElement element)
        {
            base.Remove(element);
        }

        /// <summary>
        /// Loads the elements from the parent table and puts them into the study cache.
        /// </summary>
        public override void Load()
        {
            List<ChildElement> ratings = CreateElementsFromRows( TableName, rowData => CreateElementFromRowData(rowData));
            foreach (RatingCurveElement elem in ratings)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

    }
}
