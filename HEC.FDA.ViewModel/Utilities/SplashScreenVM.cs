using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities
{
    public class SplashScreenVM : BaseViewModel
    {
        public static bool TermsAndConditionsAccepted
        {
            get { return AreTermsAndConditionsAccepted(); }
            set
            {
                if (value == true)
                {
                    RecordTermsAndConditionsAcceptance();
                }
            }
        }

        /// <summary>
        /// Checks that terms and conditions have been accepted by this user profile, for this version of FDA. Returns true if the version is found in the text file 
        /// </summary>
        public static bool AreTermsAndConditionsAccepted()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string directoryPath = Path.Combine(appDataPath, "HEC", "HEC-FDA");
            string filePath = Path.Combine(directoryPath, "terms_and_conditions_accepted.txt");

            if (Path.Exists(filePath))
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    if (line.Contains(version))
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public static void RecordTermsAndConditionsAcceptance()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string directoryPath = Path.Combine(appDataPath, "HEC", "HEC-FDA");
            string filePath = Path.Combine(directoryPath, "terms_and_conditions_accepted.txt");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using StreamWriter writer = new StreamWriter(filePath, true);
            writer.WriteLine($"{version}");
        }
    }
}
