using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

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
 

        #region constructor
        public RatingElementPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #endregion

        #region utilities
       
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
            return new RatingCurveElement(itemElem, id);
        }

        #endregion

    }
}
