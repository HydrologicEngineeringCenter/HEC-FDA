using HEC.FDA.ViewModel.Utilities;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Watershed
{
    public class TerrainElement : ChildElement, IHaveStudyFiles
    {
        #region Notes
        #endregion
        #region Fields
        public const string TERRAIN_XML_TAG = "Terrain";
        private const string SELECTED_PATH_XML_TAG = "SelectedPath";

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
        public TerrainElement(string name, string fileName, int id, bool isTemporaryNode = false) : base(name, "", "", id)
        {
            _FileName = fileName;

            if (isTemporaryNode)
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.GetImage(typeof(TerrainElement)), " -Saving");
            }
            else
            {
                AddDefaultActions(canDuplicate: false);
            }
        }

        public TerrainElement(XElement terrainElement, int id) : base(terrainElement, id)
        {
            FileName = terrainElement.Attribute(SELECTED_PATH_XML_TAG).Value;
            AddDefaultActions(canDuplicate: false);
        }

        public override XElement ToXML()
        {
            XElement terrainElement = new XElement(TERRAIN_XML_TAG);
            terrainElement.Add(CreateHeaderElement());
            terrainElement.SetAttributeValue(SELECTED_PATH_XML_TAG, FileName);
            return terrainElement;
        }

        public bool Equals(TerrainElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }
            if (FileName != elem.FileName)
            {
                isEqual = false;
            }

            return isEqual;
        }

        #endregion
    }
}
