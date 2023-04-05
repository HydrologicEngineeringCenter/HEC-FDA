using System.IO;
using HEC.FDA.ViewModel.Editors;
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
        private void GrabExistingProjection()
		{
			string[] filesInDirectory =  Directory.GetFiles(Connection.Instance.ProjectionDirectory);
			if(filesInDirectory.Length > 0) { ProjectProjectionPath = filesInDirectory[0]; } 
		}
		public void Save()
		{
            string destination = Connection.Instance.ProjectionDirectory + Path.DirectorySeparatorChar + Path.GetFileName(ProjectProjectionPath);
			if(File.Exists(_projectProjectionPath) && !_projectProjectionPath.Equals(destination) )
			{
                File.Copy(ProjectProjectionPath,destination);
				ProjectProjectionPath = destination;
            }
		}
        #endregion
    }
}
