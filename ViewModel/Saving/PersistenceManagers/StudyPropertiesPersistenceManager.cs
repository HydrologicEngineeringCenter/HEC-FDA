using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class StudyPropertiesPersistenceManager : SavingBase<StudyPropertiesElement>
    {
        public override string TableName => "study_properties";


        public StudyPropertiesPersistenceManager(FDACache studyCache):base(studyCache)
        {
        }

        //todo: how to rename the whole study?
        private void RenameStudy(string oldName, string newName)
        {
            string oldDirectory = Connection.Instance.ProjectDirectory;
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(oldDirectory);
            string newDirectory = di.Parent.FullName + "\\" + newName;

            if (System.IO.Directory.Exists(oldDirectory))
            {
                //the internet says i need to have it open with some kind of fileshare option
                //System.IO.Directory.Move(oldDirectory, newDirectory);
            }
        }

    }
}
