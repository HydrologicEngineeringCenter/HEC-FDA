using System.IO;
using HEC.FDA.ViewModel.Storage;
using HEC.MVVMFramework.ViewModel.Implementations;

namespace HEC.FDA.ViewModel.Study
{
	public class ProjectionPickerVM : ValidatingBaseViewModel
    {
        #region Backing Fields
        private string _projectProjectionPath;
        #endregion
        #region Properties
        public string ProjectProjectionPath
		{
			get { return _projectProjectionPath; }
			set { _projectProjectionPath = value; NotifyPropertyChanged();NotifyPropertyChanged(nameof(ASCIIProjection)); }
		}
		public string ASCIIProjection
		{
			get
			{
				if (File.Exists(_projectProjectionPath))
				{
					return File.ReadAllText(_projectProjectionPath);
				}
				else
				{
					return "";
				}
			}
		}
        #endregion
        #region Constructors
        public ProjectionPickerVM()
		{
			GrabExistingProjection();
        }
        #endregion
        #region Methods
		/// <summary>
		/// Always uses the first file in the directory. Should only ever be one file. 
		/// </summary>
        private void GrabExistingProjection()
		{
			string[] filesInDirectory =  Directory.GetFiles(Connection.Instance.ProjectionDirectory);
			if(filesInDirectory.Length > 0) { ProjectProjectionPath = filesInDirectory[0]; } 
		}
		
		public void Save()
		{
            string destination = Connection.Instance.ProjectionDirectory + Path.DirectorySeparatorChar + Path.GetFileName(ProjectProjectionPath);
            //check if the user is saving pointed at the existing imported file. If so, do nothing, else, archive anything that's there and copy the new file in. 
            if (File.Exists(_projectProjectionPath) && !_projectProjectionPath.Equals(destination) )
			{
                ArchiveExistingProjections();
                File.Copy(ProjectProjectionPath,destination);
				ProjectProjectionPath = destination;
            }
		} 
        /// <summary>
        /// Checks if there are already files in the projection directory. If there are, moves them to a subfolder called archive. Creates archive if it doesn't exist. Overwrites on Copy 
        /// </summary>
        private static void ArchiveExistingProjections()
        {
            string[] filesInDirectory = Directory.GetFiles(Connection.Instance.ProjectionDirectory);
            if (filesInDirectory.Length == 0)
            {
                return; //nothing to archive
            }
            string archiveDirectory = Path.Combine(Connection.Instance.ProjectionDirectory, "archive");
            if (!Directory.Exists(archiveDirectory))
            {
                Directory.CreateDirectory(archiveDirectory);
            }
            foreach (string filePath in filesInDirectory)
            {
                string fileName = Path.GetFileName(filePath);
                string destinationPath = Path.Combine(archiveDirectory, fileName);
                File.Copy(filePath, destinationPath, true);
                File.Delete(filePath);
            }
        }
        #endregion
    }
}
