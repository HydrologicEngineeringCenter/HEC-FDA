using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public static class StudyFilesManager
    {


        //todo: validate shapefile?
        public static FdaValidationResult ValidateShapefile()
        {
            FdaValidationResult vr = new FdaValidationResult();

                return vr;
        }


        private static string GetStudyElementDirectoryPath<T>() where T : ChildElement
        {
            string newDirectoryPath = null;
            if (typeof(T) == typeof(HydraulicElement))
            {
            }
            else if (typeof(T) == typeof(IndexPointsElement))
            {
                newDirectoryPath = Connection.Instance.IndexPointsDirectory;
            }
            return newDirectoryPath;
        }

        public static void CopyShapeFile<T>(string sourcePath, string newDirectoryName) where T : ChildElement
        {
            string newDirectoryPath = GetStudyElementDirectoryPath<T>() + "\\" + newDirectoryName;

            string selectedDirectory = Path.GetDirectoryName(sourcePath);
            string selectedFileName = Path.GetFileNameWithoutExtension(sourcePath);
            string[] filesToImport = Directory.GetFiles(selectedDirectory, selectedFileName + ".*");

            Directory.CreateDirectory(newDirectoryPath);
            foreach (string file in filesToImport)
            {
                string newFilePath = newDirectoryPath + "\\" + Path.GetFileName(file);
                File.Copy(file, newFilePath);
            }

        }

        public static void RenameDirectory<T>(string originalName, string newName) where T : ChildElement
        {
            if (!originalName.Equals(newName))
            {
                string originalDirectoryPath = GetStudyElementDirectoryPath<T>() + "\\" + originalName;
                string newDirectoryPath = GetStudyElementDirectoryPath<T>() + "\\" + newName;

                try
                {
                    //"Move" is basically the same as a rename of the directory.
                    Directory.Move(originalDirectoryPath, newDirectoryPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Renaming the directory failed.\n" + ex.Message, "Rename Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }


        //private void CopyWaterSurfaceFilesToStudyDirectory(string path, string nameWithExtension)
        //{
        //    string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name + "\\" + nameWithExtension;
        //    Copy(path, destinationFilePath);
        //}

        //public void CopyDirectoryContents(string sourceDirectory, string targetDirectory)
        //{
        //    DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
        //    DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
        //    CopyAll(diSource, diTarget);
        //}

        //private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        //{
        //    Directory.CreateDirectory(target.FullName);

        //    // Copy each file into the new directory.
        //    foreach (FileInfo fi in source.GetFiles())
        //    {
        //        string newPath = Path.Combine(target.FullName, fi.Name);
        //        fi.CopyTo(newPath, true);
        //    }

        //    // Copy each subdirectory using recursion.
        //    foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        //    {
        //        DirectoryInfo nextTargetSubDir =
        //            target.CreateSubdirectory(diSourceSubDir.Name);
        //        CopyAll(diSourceSubDir, nextTargetSubDir);
        //    }
        //}

    }
}
