using System.Collections.Generic;
using System.Data;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class SaveAllReportVM:BaseViewModel
    {
        public DataTable SaveReportDataTable { get; set; }
        //public List<SaveAllReportGroupVM> SuccessfulGroups { get; set; }
        //public List<SaveAllReportGroupVM> UnsuccessfulGroups { get; set; }
        public List<string> OriginalGroupNames { get; set; }
        public List<string> NewGroupNames { get; set; }

        public SaveAllReportVM(List<string> originalGroupNames, List<string> newGroupNames, List<SaveAllReportGroupVM> groups)
        {
            OriginalGroupNames = originalGroupNames;
            NewGroupNames = newGroupNames;
            SetDimensions(560, 310, 200, 200);
            CreateDataTable(groups);
        }

        private void CreateDataTable( List<SaveAllReportGroupVM> groups)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Group", typeof(string));
            dt.Columns.Add("Saved", typeof(string));
            dt.Columns.Add("Cannot Save*", typeof(string));

            foreach(SaveAllReportGroupVM group in groups)
            {
                List<object[]> rows = CreateRowsForGroup(group);
                foreach(object[] row in rows)
                {
                    dt.Rows.Add(row);
                }
            }

            SaveReportDataTable = dt;

        }

        private List<object[]> CreateRowsForGroup(SaveAllReportGroupVM group)
        {
            List<object[]> rows = new List<object[]>();
            string groupName = group.GroupName;
            int numSaved = group.SuccessfulList.Count;
            int numNotSaved = group.UnsuccessfulList.Count;
            int higherNum = numSaved;
            if(numNotSaved > numSaved)
            {
                higherNum = numNotSaved;
            }

            //handle the first row special because it has the group name
           
            
            for(int i = 0;i<higherNum;i++)
            {

                rows.Add(GetRowValueForGroup(group, i));
            }
            return rows;
        }

        private object[] GetRowValueForGroup(SaveAllReportGroupVM group, int index)
        {
            string groupVal = "";
            string savedVal = "";
            string notSavedVal = "";

            if (index == 0)
            {
                groupVal = group.GroupName;
            }
            if (group.SuccessfulList.Count > index)
            {
                savedVal = group.SuccessfulList[index];
            }
            if (group.UnsuccessfulList.Count > index)
            {
                notSavedVal = group.UnsuccessfulList[index];
            }
            return new object[3] { groupVal, savedVal, notSavedVal };
        }
    }
}
