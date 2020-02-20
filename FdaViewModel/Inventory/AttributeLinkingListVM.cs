using FdaViewModel.Inventory.OccupancyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory
{
    //[Author(q0heccdm, 6 / 23 / 2017 10:19:38 AM)]
    public class AttributeLinkingListVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/23/2017 10:19:38 AM
        #endregion
        #region Fields
        private List<string> _OccupancyTypesInFile;
        private List<string> _OccupancyTypesInStudy;
        private bool _UseDefaultsIsChecked = true;
        private bool _FromFileIsChecked;
        private List<IOccupancyType> _ListOfSelectedOccupancyTypes = new List<IOccupancyType>();
        private List<OccupancyTypes.OccupancyTypesElement> _ListOfSelectedOccTypeGroups = new List<OccupancyTypes.OccupancyTypesElement>();
        private List<string> _SelectedListOfOccTypeStrings = new List<string>();

        #endregion
        #region Properties
        public bool IsInEditMode { get; set; }
        public List<string> SelectedListOfOccTypeStrings
        {
            get { return _SelectedListOfOccTypeStrings; }
            set { _SelectedListOfOccTypeStrings = value; NotifyPropertyChanged(); }
        }
        public List<OccupancyTypes.OccupancyTypesElement> ListOfOccTypeGroups { get; set; }
        public List<OccupancyTypes.OccupancyTypesElement> ListOfSelectedOccTypeGroups
        {
            get { return _ListOfSelectedOccTypeGroups; }
            set { _ListOfSelectedOccTypeGroups = value; UpdateSelectedListOfOccTypeStrings(); NotifyPropertyChanged(); }
        }


        public Dictionary<string,string> OccupancyTypesDictionary
        {
            get;set;
        }
        //public List<Consequences_Assist.ComputableObjects.OccupancyType> ListOfSelectedOccupancyTypes
        //{
        //    get {return _ListOfSelectedOccupancyTypes;}
        //    set { _ListOfSelectedOccupancyTypes = value; NotifyPropertyChanged(); }
        //}
        public bool UseDefaultsIsChecked
        {
            get { return _UseDefaultsIsChecked; }
            set { _UseDefaultsIsChecked = value; NotifyPropertyChanged(); }
        }

        public bool FromFileIsChecked
        {
            get { return _FromFileIsChecked; }
            set { _FromFileIsChecked = value; NotifyPropertyChanged(); }
        }
        public List<string> OccupancyTypesInFile
        {
            get { return _OccupancyTypesInFile; }
            set { _OccupancyTypesInFile = value; NotifyPropertyChanged(); }
        }
        public List<string> OccupancyTypesInStudy
        {
            get { return _OccupancyTypesInStudy; }
            set { _OccupancyTypesInStudy = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public AttributeLinkingListVM() : base()
        {

        }

        public AttributeLinkingListVM(List<string> occTypesInFile,List<OccupancyTypes.OccupancyTypesElement> occtypeGroups, List<string> occtypeNamesFromStudy)
        {

            OccupancyTypesInFile = occTypesInFile;
            ListOfOccTypeGroups = occtypeGroups;
            OccupancyTypesInStudy = occtypeNamesFromStudy;

            LoadDictionary();
                

            

        }


        #endregion
        #region Voids
        private void UpdateSelectedListOfOccTypeStrings()
        {
            List<string> dummyList = new List<string>(); //this is used to trigger the setter
            
            foreach (OccupancyTypes.OccupancyTypesElement e in ListOfSelectedOccTypeGroups)
            {
                
                    foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in e.ListOfOccupancyTypes)
                    {

                        dummyList.Add(ot.Name + " from " + e.Name);
                    }
                    
                
            }
            //add a blank option if one doesn't exist already
            if(dummyList.Count>0 && dummyList[0] != "")
            {
                dummyList.Insert(0, "");
            }
            SelectedListOfOccTypeStrings = dummyList;
        }
        public void AddSelectedGroupToList(string elementName)
        {
            //List<Consequences_Assist.ComputableObjects.OccupancyType> listOfSelectedOccTypes = new List<Consequences_Assist.ComputableObjects.OccupancyType>();
            foreach(OccupancyTypes.OccupancyTypesElement ele in ListOfSelectedOccTypeGroups)
            {
                if(ele.Name == elementName)
                {
                    return; // this element is already in the list. This duplication is caused when i open the inventory back up in edit mode and i recheck the previously checked groups
                    //this cause the program to try to re-add the groups, but they are already in the selected group list.
                }

                  
            }
            List<OccupancyTypes.OccupancyTypesElement> dummyList = ListOfSelectedOccTypeGroups;
            foreach (OccupancyTypes.OccupancyTypesElement e in ListOfOccTypeGroups)
            {
                if(e.Name == elementName)
                {
                    dummyList.Add(e);
                    break;
                  
                }
            }
            ListOfSelectedOccTypeGroups = dummyList; // i do this to trigger the setter
        }
        
        public void RemoveSelectedGroupFromList(string elementName)
        {
            List<OccupancyTypes.OccupancyTypesElement> dummyList = ListOfSelectedOccTypeGroups;
            foreach (OccupancyTypes.OccupancyTypesElement e in ListOfOccTypeGroups)
            {
                if (e.Name == elementName)
                {
                    dummyList.Remove(e);
                    break;

                }
            }
            ListOfSelectedOccTypeGroups = dummyList; // i do this to trigger the setter
        }

        private void LoadDictionary()
        {
            OccupancyTypesDictionary = new Dictionary<string, string>();
            foreach(string s in OccupancyTypesInFile)
            {
                OccupancyTypesDictionary.Add(s, "");
            }
        }
        
        public void UpdateOcctypeDictionary(string occTypeFromFile, string occTypeFromStudy)
        {
            if(OccupancyTypesDictionary.ContainsKey(occTypeFromFile))
            {
                OccupancyTypesDictionary[occTypeFromFile] = occTypeFromStudy;
            }
        }



        #endregion
        #region Functions
        /// <summary>
        /// returns an array of string. [0] is the occtype name, [1] is the group name
        /// </summary>
        /// <param name="combinedString"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public string[] ParseOccTypeNameAndGroupNameFromCombinedString(string combinedString)
        {
            string[] occtypeAndGroupName = new string[2];

            if (combinedString == "" || combinedString == null)
            {
                occtypeAndGroupName[0] = "";
                occtypeAndGroupName[1] = "";
                return occtypeAndGroupName;
            }
            string occTypeName = "";
            string groupName = "";

            foreach (OccupancyTypes.OccupancyTypesElement ele in ListOfSelectedOccTypeGroups)
            {
                if (combinedString.Length - ele.Name.Length - 6 > 0)
                {

                    if (combinedString.Substring(combinedString.Length - ele.Name.Length - 6) == " from " + ele.Name)
                    {
                        groupName = combinedString.Substring(combinedString.Length - ele.Name.Length);
                        occTypeName = combinedString.Remove(combinedString.Length - ele.Name.Length - 6);
                        occtypeAndGroupName[0] = occTypeName;
                        occtypeAndGroupName[1] = groupName;
                        return occtypeAndGroupName;

                    }
                }
            }
            
            throw new Exception("could not find group name");
            //return occtypeAndGroupName;
        }
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

      
    }
}
