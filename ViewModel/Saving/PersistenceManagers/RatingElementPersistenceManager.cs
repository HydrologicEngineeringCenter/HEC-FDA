using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class RatingElementPersistenceManager : SavingBase<RatingCurveElement>
    {
        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "rating_curves"; } }
 
        #region constructor
        public RatingElementPersistenceManager(Study.FDACache studyCache):base(studyCache)
        {
        }
        #endregion
    }
}
