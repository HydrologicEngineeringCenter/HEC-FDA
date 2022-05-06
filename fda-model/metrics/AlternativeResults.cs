using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metrics
{
    public class AlternativeResults
{
        private int _alternativeID;
        private List<DamageResults> _damageResultList;

        public int AlternativeID
        {
            get { return _alternativeID; }
        }

        public AlternativeResults(int id)
        {
            _alternativeID = id;
            _damageResultList = new List<DamageResults>();
        }

    }
}
