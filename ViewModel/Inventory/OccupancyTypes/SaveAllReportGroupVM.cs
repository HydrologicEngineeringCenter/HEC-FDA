using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class SaveAllReportGroupVM:BaseViewModel
    {
        private List<String> _SuccessfulList = new List<string>() {};
        private List<String> _UnsuccessfulList = new List<string>() { };

        public List<String> SuccessfulList
        {
            get
            {
                return _SuccessfulList;
            }
            set
            {
                _SuccessfulList = value;
            }
        }

        public List<String> UnsuccessfulList
        {
            get
            {
                return _UnsuccessfulList;
            }
            set
            {
                _UnsuccessfulList = value;
            }
        }

        public string GroupName
        {
            get;set;
        }

        public SaveAllReportGroupVM(string name)
        {
            GroupName = name;
        }


    }
}
