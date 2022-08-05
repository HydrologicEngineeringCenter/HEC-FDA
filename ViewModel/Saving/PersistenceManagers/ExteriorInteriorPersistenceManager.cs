using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class ExteriorInteriorPersistenceManager:SavingBase
    {
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int CURVE_COL = 4;

        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "exterior_interior_curves"; } }

        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) }; }
        }    

        public ExteriorInteriorPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        public override object[] GetRowDataFromElement(ChildElement element)
        {
            if(element is CurveChildElement curveChildElement)
            {
                return new object[] { element.Name, element.LastEditDate, element.Description, curveChildElement.ComputeComponentVM.ToXML().ToString() };
            }
            else
            {
                return null;
            }
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            XElement curveXML =XElement.Parse( (string)rowData[CURVE_COL]);
            ComputeComponentVM computeComponentVM = new ComputeComponentVM(curveXML);

            ExteriorInteriorElement ele = new ExteriorInteriorElement((string)rowData[0], (string)rowData[LAST_EDIT_DATE_COL],
                (string)rowData[DESC_COL], computeComponentVM, id);
            return ele;
        }
        #endregion

        public void SaveNew(ChildElement element)
        {
            //save to parent table
            base.SaveNew(element);
        }

        public void Remove(ChildElement element)
        {
            base.Remove(element);
        }

        public override void Load()
        {
            List<ChildElement> exteriorInteriors = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (ExteriorInteriorElement elem in exteriorInteriors)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

    }
}
