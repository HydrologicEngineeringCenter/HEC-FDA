using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace HEC.FDA.ViewModel.Watershed
{
    public class TerrainElement : ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private string _FileName;
        #endregion
        #region Properties

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; NotifyPropertyChanged(nameof(FileName)); }
        }
        #endregion
        #region Constructors
        public TerrainElement(string name, string fileName, int id, bool isTemporaryNode = false) : base(id)
        {
            //vrt and auxilary files?  hdf5?
            Name = name;
            _FileName = fileName;

            if (isTemporaryNode)
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.TERRAIN_IMAGE, " -Saving", true);
            }
            else
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.TERRAIN_IMAGE);

                NamedAction remove = new NamedAction();
                remove.Header = StringConstants.REMOVE_MENU;
                remove.Action = RemoveElement;

                NamedAction renameElement = new NamedAction(this);
                renameElement.Header = StringConstants.RENAME_MENU;
                renameElement.Action = Rename;

                List<NamedAction> localactions = new List<NamedAction>();
                localactions.Add(remove);
                localactions.Add(renameElement);

                Actions = localactions;
            }
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            TerrainElement elem = (TerrainElement)elementToClone;
            return new TerrainElement(elementToClone.Name, elem.FileName, elem.ID);
        }

        public override void Rename(object sender, EventArgs e)
        {
            string originalName = Name;
            RenameVM renameViewModel = new RenameVM(this, CloneElement);
            string header = "Rename";
            DynamicTabVM tab = new DynamicTabVM(header, renameViewModel, "Rename",false, false);
            Navigate(tab);
            if (!renameViewModel.WasCanceled)
            {
                string newName = renameViewModel.Name;
                //rename the folders in the study.
                if (!originalName.Equals(newName))
                {
                    try
                    {
                        string sourceFilePath = Connection.Instance.TerrainDirectory + "\\" + originalName;
                        string destinationFilePath = Connection.Instance.TerrainDirectory + "\\" + newName;
                        Directory.Move(sourceFilePath, destinationFilePath);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Renaming the terrain directory failed.\n" + ex.Message, "Rename Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        #endregion
    }
}
