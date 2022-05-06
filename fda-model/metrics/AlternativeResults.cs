using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;

namespace metrics
{
    public class AlternativeResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private int _alternativeID;
        private List<DamageResults> _damageResultsList;
        #endregion

        #region Properties
        public int AlternativeID
        {
            get { return _alternativeID; }
        }
        public List<DamageResults> DamageResultsList
        {
            get
            {
                return _damageResultsList;
            }
        }
        public event MessageReportedEventHandler MessageReport;

        #endregion

        #region Constructor
        public AlternativeResults(int id)
        {
            _alternativeID = id;
            _damageResultsList = new List<DamageResults>();
        }
        #endregion
        #region Methods
        public void AddDamageResults(int impactAreaID)
        {
            DamageResults damageResults = GetDamageResults(impactAreaID);
            if (damageResults.IsNull)
            {
                DamageResults newDamageResults = new DamageResults(impactAreaID);
                _damageResultsList.Add(newDamageResults);
            }
        }
        public void AddDamageResults(DamageResults damageResultsToAdd)
        {
            DamageResults damageResults = GetDamageResults(damageResultsToAdd.ImpactAreaID);
            if (damageResults.IsNull)
            {
                _damageResultsList.Add(damageResultsToAdd);
            }
        }
        public DamageResults GetDamageResults(int impactAreaID)
        {
            foreach (DamageResults damageResults in _damageResultsList)
            {
                if(damageResults.ImpactAreaID.Equals(impactAreaID))
                {
                    return damageResults;
                }
            }
            DamageResults dummyDamageResults = new DamageResults(impactAreaID);
            ReportMessage(this, new MessageEventArgs(new Message("The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned.")));
            return dummyDamageResults;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion

    }
}
