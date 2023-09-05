using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.structures;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using SixLabors.ImageSharp.Formats.Gif;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using static HEC.FDA.Model.structures.OccupancyType;

namespace HEC.FDA.ViewModel.Inventory
{
    //[Author(q0heccdm, 12 / 1 / 2016 2:21:18 PM)]
    public class InventoryElement : ChildElement, IHaveStudyFiles
    {

        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/1/2016 2:21:18 PM
        #endregion
        #region Fields
        private const string INVENTORY_MAPPINGS = "InventoryMappings";
        private const string IMPORTED_FROM_OLD_FDA = "ImportedFromOldFDA";
        private const string OCCTYPE_MAPPINGS = "OcctypeMappings";
        private const string OCCTYPE_MAPPING = "OcctypeMapping";
        private const string SHAPEFILE_OCCTYPE = "ShapefileOcctype";
        private const string GROUP_ID = "GroupID";
        private const string ID_ATTRIBUTE_NAME = "ID";
        private readonly Dictionary<string, OccupancyTypes.OcctypeReference> _OcctypeMapping = new();
        #endregion
        #region Properties
        public bool IsImportedFromOldFDA { get; set; }
        public StructureSelectionMapping SelectionMappings {get;}
        public Dictionary<String, OccupancyTypes.OcctypeReference> OcctypeMapping
        {
            get { return _OcctypeMapping; }
        }
        #endregion
        #region Constructors
        public InventoryElement(string name, string description, StructureSelectionMapping selections, 
            Dictionary<String, OccupancyTypes.OcctypeReference> occtypeMapping,  bool isImportedFromOldFDA, int id) 
            : base(name,"", description, id)
        {
            _OcctypeMapping = occtypeMapping;
            SelectionMappings = selections;
            IsImportedFromOldFDA = isImportedFromOldFDA;
            AddDefaultActions(EditElement, StringConstants.EDIT_STRUCTURES_MENU);
        }

        public InventoryElement(XElement inventoryElem, int id)
           : base(inventoryElem, id)
        {
            IsImportedFromOldFDA = Convert.ToBoolean( inventoryElem.Attribute(IMPORTED_FROM_OLD_FDA).Value);

            XElement mappingsElem = inventoryElem.Element(INVENTORY_MAPPINGS);
            SelectionMappings = new StructureSelectionMapping(mappingsElem);
            ReadDictionaryFromXML(mappingsElem);
            AddDefaultActions(EditElement,StringConstants.EDIT_STRUCTURES_MENU);
        }

        #endregion

        private void ReadDictionaryFromXML(XElement mappingsElem)
        {
            XElement occtypeMappings = mappingsElem.Element(OCCTYPE_MAPPINGS);
            IEnumerable<XElement> occtypeMappingElements = occtypeMappings.Elements(OCCTYPE_MAPPING);
            foreach (XElement occtypeMappingElement in occtypeMappingElements)
            {
                string shapefileOcctypeName = occtypeMappingElement.Attribute(SHAPEFILE_OCCTYPE).Value;
                int groupID = Convert.ToInt32(occtypeMappingElement.Attribute(GROUP_ID).Value);
                int id = Convert.ToInt32(occtypeMappingElement.Attribute(ID_ATTRIBUTE_NAME).Value);
                OccupancyTypes.OcctypeReference otRef = new(groupID, id);
                _OcctypeMapping.Add(shapefileOcctypeName, otRef);
            }
        }

        public void EditElement(object sender, EventArgs e)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);
            ImportStructuresFromShapefileVM vm = new(this, actionManager);
            string header = "Edit " + Name;
            DynamicTabVM tab = new(header, vm, "EditInventory" + Name);
            Navigate(tab, false, false);

        }

        public override XElement ToXML()
        {
            XElement inventoryElem = new(StringConstants.ELEMENT_XML_TAG);
            inventoryElem.Add(CreateHeaderElement());
            inventoryElem.SetAttributeValue(IMPORTED_FROM_OLD_FDA, IsImportedFromOldFDA);
            XElement selectionMappingsElem = SelectionMappings.ToXML();

            XElement occtypesElem = new(OCCTYPE_MAPPINGS);
            foreach (KeyValuePair<string, OccupancyTypes.OcctypeReference> pair in _OcctypeMapping)
            {
                occtypesElem.Add(CreateOcctypeMappingXElement(pair.Key, pair.Value));
            }
            selectionMappingsElem.Add(occtypesElem);
            inventoryElem.Add(selectionMappingsElem);
            return inventoryElem;
        }

        private static XElement CreateOcctypeMappingXElement(String shapefileOcctype, OccupancyTypes.OcctypeReference fDAOcctype)
        {
            XElement rowElem = new(OCCTYPE_MAPPING);
            rowElem.SetAttributeValue(SHAPEFILE_OCCTYPE, shapefileOcctype);
            rowElem.SetAttributeValue(GROUP_ID, fDAOcctype.GroupID);
            rowElem.SetAttributeValue(ID_ATTRIBUTE_NAME, fDAOcctype.ID);
            return rowElem;
        }

        /// <summary>
        /// Gets a file from this inventory elements folder in the study directory.
        /// </summary>
        /// <param name="extension">the extension needs to include the period. ie: ".dbf"</param>
        /// <returns></returns>
        public string GetFilePath(string extension)
        {
            string path = null;
            string[] files = Directory.GetFiles(Connection.Instance.InventoryDirectory + "\\" + Name);
            foreach (string file in files)
            {
                if(Path.GetExtension(file).Equals(extension))
                {
                    path = file;
                    break;
                }
            }
            return path;
        }

        

        private static string GetImpactAreaDirectory(string impactAreaName)
        {
            return Connection.Instance.ImpactAreaDirectory + "\\" + impactAreaName;
        }

        private static string GetImpactAreaShapefile(string impactAreaName)
        {
            return Directory.GetFiles(GetImpactAreaDirectory(impactAreaName), "*.shp")[0];
        }

        private string GetStructuresDirectory()
        {
            return Connection.Instance.InventoryDirectory + "\\" + Name;
        }

        private string GetStructuresPointShapefile()
        {
            return Directory.GetFiles(GetStructuresDirectory(), "*.shp")[0];
        }

        public Model.structures.Inventory CreateModelInventory(ImpactAreaElement impactAreaElement)
        {
            Dictionary<string, OccupancyType> occtypeMappings = CreateModelOcctypesMapping();
            string pointShapefilePath = GetStructuresPointShapefile();
            string impAreaShapefilePath = GetImpactAreaShapefile(impactAreaElement.Name);
            string terrainPath = InventoryColumnSelectionsVM.getTerrainFile();
            StudyPropertiesElement studyProperties = StudyCache.GetStudyPropertiesElement();
            double priceIndex = studyProperties.UpdatedPriceIndex;
            Model.structures.Inventory inv = new(pointShapefilePath, impAreaShapefilePath,
                SelectionMappings, occtypeMappings, SelectionMappings.IsUsingTerrainFile,terrainPath, priceIndex);
            return inv;
        }

        public static FirstFloorElevationUncertainty CreateFirstFloorUncertainty(ContinuousDistribution ordinate)
        {
            FirstFloorElevationUncertainty elevationUncertainty = null;
            IDistributionEnum ordType = ordinate.Type;

            switch(ordType)
            {
                case IDistributionEnum.Deterministic:
                    elevationUncertainty = new FirstFloorElevationUncertainty();
                    break;
                case IDistributionEnum.Normal:
                    double normalStDev = ((Normal)ordinate).StandardDeviation;
                    elevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, normalStDev);
                    break;
                case IDistributionEnum.LogNormal:
                    double logNormalStDev = ((LogNormal)ordinate).StandardDeviation;
                    elevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.LogNormal, logNormalStDev);
                    break;
                case IDistributionEnum.Triangular:
                    double triMin = ((Triangular)ordinate).Min;
                    double triMax = ((Triangular)ordinate).Max;
                    elevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Triangular, triMin, triMax);
                    break;
                case IDistributionEnum.Uniform:
                    double uniMin = ((Uniform)ordinate).Min;
                    double uniMax = ((Uniform)ordinate).Max;
                    elevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Triangular, uniMin, uniMax);
                    break;
            }

            return elevationUncertainty;
        }

        public static ValueRatioWithUncertainty CreateValueRatioWithUncertainty(ContinuousDistribution ordinate)
        {
            ValueRatioWithUncertainty valueUncertainty = null;
            IDistributionEnum ordType = ordinate.Type;

            switch (ordType)
            {
                case IDistributionEnum.Deterministic:
                    valueUncertainty = new ValueRatioWithUncertainty();
                    break;
                case IDistributionEnum.Normal:
                    double normalMean = ((Normal)ordinate).Mean;
                    double normalStDev = ((Normal)ordinate).StandardDeviation;
                    valueUncertainty = new ValueRatioWithUncertainty(IDistributionEnum.Normal, normalStDev, normalMean);
                    break;
                case IDistributionEnum.LogNormal:
                    double logNormalMean = ((LogNormal)ordinate).Mean;
                    double logNormalStDev = ((LogNormal)ordinate).StandardDeviation;
                    valueUncertainty = new ValueRatioWithUncertainty(IDistributionEnum.LogNormal, logNormalStDev, logNormalMean);
                    break;
                case IDistributionEnum.Triangular:
                    double triMin = ((Triangular)ordinate).Min;
                    double triMax = ((Triangular)ordinate).Max;
                    double triMostLikely = ((Triangular)ordinate).MostLikely;
                    valueUncertainty = new ValueRatioWithUncertainty(IDistributionEnum.Triangular, triMin, triMostLikely, triMax);
                    break;
                case IDistributionEnum.Uniform:
                    double uniMin = ((Uniform)ordinate).Min;
                    double uniMax = ((Uniform)ordinate).Max;
                    valueUncertainty = new ValueRatioWithUncertainty(IDistributionEnum.Uniform, uniMin, -9999, uniMax); //Uniform doesn't have a most likely. that -9999 wont get used. 
                    break;

            }

            return valueUncertainty;
        }

        public static ValueUncertainty CreateValueUncertainty(ContinuousDistribution ordinate)
        {
            ValueUncertainty valueUncertainty = null;
            IDistributionEnum ordType = ordinate.Type;

            switch (ordType)
            {
                case IDistributionEnum.Deterministic:
                    valueUncertainty = new ValueUncertainty(IDistributionEnum.Deterministic, ((Deterministic)ordinate).Value);
                    break;
                case IDistributionEnum.Normal:
                    double normalStDev = ((Normal)ordinate).StandardDeviation;
                    valueUncertainty = new ValueUncertainty(IDistributionEnum.Normal, normalStDev);
                    break;
                case IDistributionEnum.LogNormal:
                    double logNormalStDev = ((LogNormal)ordinate).StandardDeviation;
                    valueUncertainty = new ValueUncertainty(IDistributionEnum.LogNormal, logNormalStDev);
                    break;
                case IDistributionEnum.Triangular:
                    double triMin = ((Triangular)ordinate).Min;
                    double triMax = ((Triangular)ordinate).Max;
                    valueUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, triMin, triMax);
                    break;
                case IDistributionEnum.Uniform:
                    double uniMin = ((Uniform)ordinate).Min;
                    double uniMax = ((Uniform)ordinate).Max;
                    valueUncertainty = new ValueUncertainty(IDistributionEnum.Uniform, uniMin, uniMax);
                    break;

            }

            return valueUncertainty;
        }

        public FdaValidationResult AreMappingsValid()
        {
            FdaValidationResult vr = new();
            int numOcctypesNotFound = 0;

            foreach (OccupancyTypes.OcctypeReference otRef in _OcctypeMapping.Values)
            {
                OccupancyTypes.OccupancyType ot = otRef.GetOccupancyType();
                if(ot == null)
                {
                    //we didn't find the occtype. We could write out to the user the group id and the occtype id that we didn't find but i don't think
                    //that is useful since that occtype doesn't exist in the db anymore. It would just be meaningless numbers for the user.
                    numOcctypesNotFound++;
                }
            }
            if(numOcctypesNotFound > 0)
            {
                vr.AddErrorMessage("The structure inventory " + Name + " points at occupancy types that no longer exist. " + numOcctypesNotFound + " were not found. " +
                    "Edit the structure inventory and reassign the occupancy types.");
            }
            return vr;
        }

        private static OccupancyType CreateModelOcctype(OccupancyTypes.OcctypeReference otRef)
        {
            OccupancyTypes.OccupancyType ot = otRef.GetOccupancyType();
            var modelOT = CreateModelOcctypeFromVMOcctype(ot);
            return modelOT;
        }

        public static OccupancyType CreateModelOcctypeFromVMOcctype(OccupancyTypes.OccupancyType ot)

        {
            UncertainPairedData structureUPD = ot.StructureItem.Curve.SelectedItemToPairedData();
            UncertainPairedData contentUPD = ot.ContentItem.Curve.SelectedItemToPairedData();
            UncertainPairedData vehicleUPD = ot.VehicleItem.Curve.SelectedItemToPairedData();
            UncertainPairedData otherUPD = ot.OtherItem.Curve.SelectedItemToPairedData();

            ContinuousDistribution foundationHeightUncertainty = ot.FoundationHeightUncertainty;
            FirstFloorElevationUncertainty firstFloorElevationUncertainty = CreateFirstFloorUncertainty(foundationHeightUncertainty);

            ValueUncertainty structureUncertainty = CreateValueUncertainty(ot.StructureItem.ValueUncertainty.CreateOrdinate());
            ValueUncertainty contentUncertainty = CreateValueUncertainty(ot.ContentItem.ValueUncertainty.CreateOrdinate());
            ValueUncertainty vehicleUncertainty = CreateValueUncertainty(ot.VehicleItem.ValueUncertainty.CreateOrdinate());
            ValueUncertainty otherUncertainty = CreateValueUncertainty(ot.OtherItem.ValueUncertainty.CreateOrdinate());

            OccupancyTypeBuilder builder = Builder()
                .WithName(ot.Name)
                .WithDamageCategory(ot.DamageCategory)
                .WithStructureDepthPercentDamage(structureUPD)
                .WithStructureValueUncertainty(structureUncertainty)
                .WithFirstFloorElevationUncertainty(firstFloorElevationUncertainty);

            if (ot.OtherItem.IsChecked)
            {
                builder.WithOtherDepthPercentDamage(otherUPD);
                if (!ot.OtherItem.IsByValue)
                {
                    builder.WithOtherToStructureValueRatio(CreateValueRatioWithUncertainty(ot.OtherItem.ContentByRatioVM.CreateOrdinate())); // Look at renaming contentbyratioVM
                }
                else
                {
                    builder.WithOtherValueUncertainty(otherUncertainty);
                }
            }

            if (ot.ContentItem.IsChecked)
            {
                builder.WithContentDepthPercentDamage(contentUPD);
                if (!ot.ContentItem.IsByValue)
                {
                    builder.WithContentToStructureValueRatio(CreateValueRatioWithUncertainty(ot.ContentItem.ContentByRatioVM.CreateOrdinate()));
                }
                else
                {
                    builder.WithContentValueUncertainty(contentUncertainty);
                }
            }

            if (ot.VehicleItem.IsChecked)
            {
                builder.WithVehicleDepthPercentDamage(vehicleUPD);
                builder.WithVehicleValueUncertainty(vehicleUncertainty);
            }
            return builder.Build();
        }

        private Dictionary<string, OccupancyType> CreateModelOcctypesMapping()
        {

            Dictionary<string, OccupancyType> occtypesMapping = new();
            foreach(KeyValuePair< string, OccupancyTypes.OcctypeReference> entry in _OcctypeMapping)

            {
                OccupancyType ot = CreateModelOcctype(entry.Value);              
                //todo: log ot error messages?
                occtypesMapping.Add(entry.Key, ot);
            }
      
            return occtypesMapping;
        }

    }
}
