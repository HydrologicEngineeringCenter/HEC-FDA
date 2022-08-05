using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class LeveePersistenceManager : SavingBase
    {
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int ELEVATION_COL = 4;
        private const int IS_DEFAULT_COL = 5;
        private const int CURVE_COL = 6;



        public override string TableName
        {
            get { return "levee_features"; }
        }

        public LeveePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        /// <summary>
        /// Turns the element into an object[] for the row in the parent table
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override object[] GetRowDataFromElement(ChildElement element)
        {
            CurveChildElement curveChildElement = element as CurveChildElement; 
            return new object[] { element.Name, element.LastEditDate, element.Description, ((LeveeFeatureElement)element).Elevation, ((LeveeFeatureElement)element).IsDefaultCurveUsed, curveChildElement.ComputeComponentVM.ToXML().ToString() };
        }

        /// <summary>
        /// Creates an element from the row in the parent table.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            string xmlString = (string)rowData[XML_COL];
            XDocument doc = XDocument.Parse(xmlString);
            XElement itemElem = doc.Element(IndexPointsElement.INDEX_POINTS_TAG);
            return new LeveeFeatureElement(itemElem, id);

        }

        #endregion

        public void SaveNew(ChildElement element)
        {
            base.SaveNew(element);
        }
        public void Remove(ChildElement element)
        {
            base.Remove(element);
        }

        public override void Load()
        {
            List<ChildElement> levees = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (LeveeFeatureElement elem in levees)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }
    }
}
