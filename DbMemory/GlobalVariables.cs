using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbMemory
{
    static public class GlobalVariables
    {
        static public Study mp_fdaStudy = new Study();

        public enum SIZE_DBF_CHAR
        {
            DBF_FIELDNAME_LENGTH_ALLOC = 11,
            DBF_FIELDNAME_LENGTH = 10,
            DBF_TAG_EXPRESSION = 200,
            DBF_TAG_FILTER = 500,
            TAG_DESCRIPTOR_SIZE = 21,
            FIELD_DESCRIPTOR_SIZE = 21
        };

    }
}
