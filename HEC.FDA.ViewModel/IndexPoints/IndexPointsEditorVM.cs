﻿using HEC.CS.Collections;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
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
            AddValidationRules();
        }

        public IndexPointsEditorVM(IndexPointsElement element, List<string> indexPoints, EditorActionManager actionManager) : base(element, actionManager)
        {
            Name = element.Name;
            ListOfRows.AddRange(indexPoints);
            Description = element.Description;
            SelectedPath = Storage.Connection.Instance.IndexPointsDirectory + "\\" + Name;
            AddValidationRules();
        }
        #endregion
        #region Voids
        private void AddValidationRules()
        {
            AddRule(nameof(ListOfRows), () =>
            {
                return ListOfRows.Count > 0;
            }, "No index points have been defined.");
        }

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
            }
            else
            {
                ListOfRows.Clear();
                DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(Path.ChangeExtension(SelectedPath, ".dbf"));
                DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);
                
                for (int i = 0; i < dtv.ColumnNames.Count(); i++)
                {
                    if (dtv.ColumnNames[i] == SelectedUniqueName)
                    {
                        object[] colObjects = dtv.GetColumn(i);
                        List<string> names = new List<string>();
                        colObjects.ToList().ForEach(x => names.Add(x.ToString()));
                        List<string> uniqueNames = names.Distinct().ToList();
                        ListOfRows.AddRange(uniqueNames);
                    }
                }
                //we need to trigger the ListOfRows property rules to re-evaluate
                NotifyPropertyChanged(nameof(ListOfRows));
            }
        }
        #endregion

        public override void Save()
        {
            int id = GetElementID<IndexPointsElement>();
            IndexPointsElement elementToSave = new IndexPointsElement(Name, Description, ListOfRows.ToList(), id);

            if (IsCreatingNewElement)
            {
                StudyFilesManager.CopyFilesWithSameName(SelectedPath, Name, elementToSave.GetType());
            }
            else
            {
                StudyFilesManager.RenameDirectory(OriginalElement.Name, Name, elementToSave.GetType());
            }
            //this call handles the sqlite data
            Save(elementToSave);
        }

    }
}