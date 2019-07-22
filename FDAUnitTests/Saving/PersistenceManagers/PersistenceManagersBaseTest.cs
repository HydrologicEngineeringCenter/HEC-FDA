using FdaViewModel.Saving;
using FdaViewModel.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    public class PersistenceManagersBaseTest
    {

        public void createSqliteDatabase(String dbName)
        {
            String tempPath = Path.GetTempPath();
            string newStudyPath = tempPath + "\\" + "Testing" + "\\" + dbName + ".sqlite";
            FdaViewModel.Storage.Connection.Instance.ProjectFile = newStudyPath;
        }

        public void SaveNewTest(String dbName, ChildElement elem, IPersistable manager)
        {
            createSqliteDatabase(dbName);
            manager.SaveNew(elem);
            Assert.IsTrue(isElementInDataBase(elem.Name, manager));
            manager.Remove(elem);

        }

        public void SaveExistingTest(String dbName, ChildElement originalElem, IPersistable manager)
        {
            createSqliteDatabase(dbName);
            manager.SaveNew(originalElem);
            Assert.IsTrue(isElementInDataBase(originalElem.Name, manager));

            ChildElement modifiedElem = originalElem.CloneElement(originalElem);
            modifiedElem.Name = "newName";
            //save existing
            manager.SaveExisting(originalElem, modifiedElem, 0);
            Assert.IsTrue(isElementInDataBase(modifiedElem.Name, manager));

            manager.Remove(modifiedElem);
        }

        public void UndoTest(String dbName, ChildElement originalElem, IPersistableWithUndoRedo manager)
        {
            createSqliteDatabase(dbName);
            //save
            manager.SaveNew(originalElem);
            Assert.IsTrue(isElementInDataBase(originalElem.Name, manager));

            ChildElement modifiedElem = originalElem.CloneElement(originalElem);
            modifiedElem.Name = "newName";
            //save existing
            manager.SaveExisting(originalElem, modifiedElem, 0);
            Assert.IsTrue(isElementInDataBase(modifiedElem.Name, manager));

            ChildElement undoElem = manager.Undo(modifiedElem, 0);
            Assert.IsTrue(undoElem.Equals(originalElem));

            manager.Remove(modifiedElem);
        }

        public void RedoTest(String dbName, ChildElement originalElem, IPersistableWithUndoRedo manager)
        {
            createSqliteDatabase(dbName);
            manager.SaveNew(originalElem);
            Assert.IsTrue(isElementInDataBase(originalElem.Name, manager));

            ChildElement modifiedElem = originalElem.CloneElement(originalElem);
            modifiedElem.Name = "newName";
            //save existing
            manager.SaveExisting(originalElem, modifiedElem, 0);
            Assert.IsTrue(isElementInDataBase(modifiedElem.Name, manager));

            //undo
            ChildElement undoElem = manager.Undo(modifiedElem, 0);
            Assert.IsTrue(undoElem.Equals(originalElem));

            //redo
            ChildElement redoElem = manager.Redo(modifiedElem, 1);
            Assert.IsTrue(redoElem.Equals(modifiedElem));

            manager.Remove(modifiedElem);
        }

        public void RemoveTest(String dbName, ChildElement elem, IPersistable manager)
        {
            createSqliteDatabase(dbName);
            //save
            manager.SaveNew(elem);
            Assert.IsTrue(isElementInDataBase(elem.Name, manager));
            //remove
            manager.Remove(elem);
            Assert.IsFalse(isElementInDataBase(elem.Name, manager));
        }

        public bool isElementInDataBase(String name, IPersistable manager)
        {
            //retrieve from db
            List<ChildElement> elems = manager.Load();

            bool elemFound = false;
            foreach (ChildElement el in elems)
            {
                if (el.Name.Equals(name))
                {
                    elemFound = true;
                    break;
                }
            }
            return elemFound;
        }

    }
}
