using HEC.CS.Collections;
using HEC.FDA.ViewModel.Editors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.IndexPoints
{
    public class IndexPointsEditorVM: BaseEditorVM
    {
        #region Fields
        private string _Path;
        private List<string> _UniqueFields;
        private string _SelectedUniqueName;
        #endregion
        #region Properties

        public string SelectedPath
        {
            get { return _Path; }
            set { _Path = value; LoadUniqueNames(); NotifyPropertyChanged(); }
        }
        public CustomObservableCollection<string> ListOfRows { get; } = new CustomObservableCollection<string>();

        public List<string> UniqueFields
        {
            get { return _UniqueFields; }
            set { _UniqueFields = value; NotifyPropertyChanged(); }
        }
        public string SelectedUniqueName
        {
            get { return _SelectedUniqueName; }
            set { _SelectedUniqueName = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public IndexPointsEditorVM(EditorActionManager actionManager) : base(actionManager)
        {
        }

        public IndexPointsEditorVM(IndexPointsChildElement element, List<string> indexPoints, EditorActionManager actionManager) : base(element, actionManager)
        {
            Name = element.Name;
            ListOfRows.AddRange(indexPoints);
            Description = element.Description;
            SelectedPath = Storage.Connection.Instance.IndexPointsDirectory + "\\" + Name;
        }
        #endregion
        #region Voids
        /// <summary>
        /// This method grabs all the column headers from the dbf and loads them into a unique name combobox.
        /// </summary>
        /// <param name="path"></param>
        public void LoadUniqueNames()
        {
            if (IsCreatingNewElement)
            {
                if (!File.Exists(Path.ChangeExtension(_Path, "dbf")))
                {
                    MessageBox.Show("This path has no associated *.dbf file.", "File Doesn't Exist", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(Path.ChangeExtension(_Path, ".dbf"));
                    DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

                    List<string> uniqueNameList = dtv.ColumnNames.ToList();

                    UniqueFields = uniqueNameList;
                }
            }
        }

        public void LoadTheRows()
        {
            if (!File.Exists(Path.ChangeExtension(SelectedPath, "dbf")))
            {
                MessageBox.Show("This path has no associated *.dbf file.", "No dbf File", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(Path.ChangeExtension(SelectedPath, ".dbf"));
            DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

            for (int i = 0; i < dtv.ColumnNames.Count(); i++)
            {
                if (dtv.ColumnNames[i] == SelectedUniqueName)
                {
                    object[] colObjects = dtv.GetColumn(i);
                    List<string> names = new List<string>();
                    colObjects.ToList().ForEach(x => names.Add((string)x));
                    List<string> uniqueNames = names.Distinct().ToList();
                    ListOfRows.AddRange(uniqueNames);
                }
            }
        }
        #endregion

        public override void Save()
        {
            int id = GetElementID(Saving.PersistenceFactory.GetIndexPointsPersistenceManager());
            IndexPointsChildElement elementToSave = new IndexPointsChildElement(Name, Description, ListOfRows.ToList(), id);

            string newDirectoryPath = Storage.Connection.Instance.IndexPointsDirectory + "\\" + Name;
            string newFilePath = newDirectoryPath + "\\" + Path.GetFileName(SelectedPath);
            if (IsCreatingNewElement)
            {
                //handle the shapefile
                Directory.CreateDirectory(newDirectoryPath);
                File.Copy(SelectedPath, newFilePath);
            }
            else
            {
                //might have to update the directory name
                if (!OriginalElement.Name.Equals(Name))
                {
                    string originalDirectoryPath = Storage.Connection.Instance.IndexPointsDirectory + "\\" + OriginalElement.Name;
                    try
                    {
                        //"Move" is basically the same as a rename of the directory.
                        Directory.Move(originalDirectoryPath, newDirectoryPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Renaming the terrain directory failed.\n" + ex.Message, "Rename Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            //this call handles the sqlite data
            Save(elementToSave);
        }

    }
}
