using HEC.FDA.Model.SQLite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes.SQLiteSaving;
public class OccupancyTypeSaver : SQLiteSaverBase<OccupancyType>
{
    private static readonly string _createMetadataTableCommandText =
        $@"
            CREATE TABLE IF NOT EXISTS {OccupancyTypesSQLiteConstants.METADATA_TABLE_NAME} (
                {OccupancyTypesSQLiteConstants.METADATA_ID_HEADER} INTEGER PRIMARY KEY AUTOINCREMENT,
                {OccupancyTypesSQLiteConstants.METADATA_NAME_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.METADATA_DESCRIPTION_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.METADATA_LASTEDIT_HEADER} TEXT NOT NULL
            );";

    private static readonly string _createOccupancyTypesTableCommandText =
        $@"
            CREATE TABLE IF NOT EXISTS {OccupancyTypesSQLiteConstants.OCCTYPES_TABLE_NAME} (
                {OccupancyTypesSQLiteConstants.OCCTYPES_ID_HEADER} INTEGER PRIMARY KEY NOT NULL,
                {OccupancyTypesSQLiteConstants.OCCTYPES_NAME_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.OCCTYPES_DESCRIPTION_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.OCCTYPES_DAMCAT_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.OCCTYPES_FOUND_UNCERTAINTY_HEADER} TEXT NOT NULL
            )";

    private static readonly string _createAssetsTableCommandText =
        $@"
            CREATE TABLE IF NOT EXISTS {OccupancyTypesSQLiteConstants.ASSETS_TABLE_NAME} (
                {OccupancyTypesSQLiteConstants.ASSETS_OCCTYPE_HEADER} TEXT NOT NULL,           
                {OccupancyTypesSQLiteConstants.ASSETS_ASSETCATEGORY_HEADER} TEXT NOT NULL,           
                {OccupancyTypesSQLiteConstants.ASSETS_ISSELECTED_HEADER} INTEGER,
                {OccupancyTypesSQLiteConstants.ASSETS_TYPE_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_BYVALUE_HEADER} INTEGER NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVE_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVENAME_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_DISTOPTIONS_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_CURVEDATA_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_DESCRIPTION_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_UNCERTAINTY_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_RATIOUNCERTAINTY_HEADER} TEXT
            )";

    private static readonly string _insertMetadataCommandText =
        $@"
            INSERT OR IGNORE INTO {OccupancyTypesSQLiteConstants.METADATA_TABLE_NAME} (
                {OccupancyTypesSQLiteConstants.METADATA_NAME_HEADER},            
                {OccupancyTypesSQLiteConstants.METADATA_DESCRIPTION_HEADER},            
                {OccupancyTypesSQLiteConstants.METADATA_LASTEDIT_HEADER}            
            )
            VALUES (
                {OccupancyTypesSQLiteConstants.METADATA_NAME_PARAMETER},            
                {OccupancyTypesSQLiteConstants.METADATA_DESCRIPTION_PARAMETER},            
                {OccupancyTypesSQLiteConstants.METADATA_LASTEDIT_PARAMETER}            
            );";

    private static readonly string _insertOcctypeCommandText =
       $@"
            INSERT OR IGNORE INTO {OccupancyTypesSQLiteConstants.OCCTYPES_TABLE_NAME} (
                {OccupancyTypesSQLiteConstants.OCCTYPES_ID_HEADER},            
                {OccupancyTypesSQLiteConstants.OCCTYPES_NAME_HEADER},            
                {OccupancyTypesSQLiteConstants.OCCTYPES_DESCRIPTION_HEADER},            
                {OccupancyTypesSQLiteConstants.OCCTYPES_DAMCAT_HEADER},            
                {OccupancyTypesSQLiteConstants.OCCTYPES_FOUND_UNCERTAINTY_HEADER}            
            )
            VALUES (
                {OccupancyTypesSQLiteConstants.OCCTYPES_ID_PARAMETER},            
                {OccupancyTypesSQLiteConstants.OCCTYPES_NAME_PARAMETER},            
                {OccupancyTypesSQLiteConstants.OCCTYPES_DESCRIPTION_PARAMETER},            
                {OccupancyTypesSQLiteConstants.OCCTYPES_DAMCAT_PARAMETER},            
                {OccupancyTypesSQLiteConstants.OCCTYPES_FOUND_UNCERTAINTY_PARAMETER}            
            );";

    private static readonly string _insertAssetsCommandText =
       $@"
            INSERT OR IGNORE INTO {OccupancyTypesSQLiteConstants.ASSETS_TABLE_NAME} (
                {OccupancyTypesSQLiteConstants.ASSETS_OCCTYPE_HEADER},                
                {OccupancyTypesSQLiteConstants.ASSETS_ASSETCATEGORY_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_ISSELECTED_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_TYPE_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_BYVALUE_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVE_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVENAME_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_DISTOPTIONS_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_DESCRIPTION_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_CURVEDATA_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_UNCERTAINTY_HEADER},
                {OccupancyTypesSQLiteConstants.ASSETS_RATIOUNCERTAINTY_HEADER}
            )
            VALUES (
                {OccupancyTypesSQLiteConstants.ASSETS_OCCTYPE_PARAMETER},                
                {OccupancyTypesSQLiteConstants.ASSETS_ASSETCATEGORY_PARAMETER},
                {OccupancyTypesSQLiteConstants.ASSETS_ISSELECTED_PARAMETER},
                {OccupancyTypesSQLiteConstants.ASSETS_TYPE_PARAMETER},
                {OccupancyTypesSQLiteConstants.ASSETS_BYVALUE_PARAMETER},
                {OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVE_PARAMETER},
                {OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVENAME_PARAMETER},
                {OccupancyTypesSQLiteConstants.ASSETS_DISTOPTIONS_PARAMETER},
                {OccupancyTypesSQLiteConstants.ASSETS_DESCRIPTION_PARAMETER},
                {OccupancyTypesSQLiteConstants.ASSETS_CURVEDATA_Parameter},
                {OccupancyTypesSQLiteConstants.ASSETS_UNCERTAINTY_PARAMETER},
                {OccupancyTypesSQLiteConstants.ASSETS_RATIOUNCERTAINTY_PARAMETER}
            );";

    public OccupancyTypeSaver(string dbPath) : base(dbPath)
    {
        CreateTables(_connection);
    }
    public override void DeleteFromSQLite(SQLiteFilter filter)
    {
        throw new NotImplementedException();
    }

    public override List<OccupancyType> ReadFromSQLite(SQLiteFilter filter)
    {
        throw new NotImplementedException();
    }

    public override void SaveToSQLite(OccupancyType occtype)
    {
        if (occtype == null)
            return;

        using var cmd = new SQLiteCommand(_connection);
        cmd.CommandText = _insertOcctypeCommandText;
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.OCCTYPES_ID_PARAMETER, occtype.ID);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.OCCTYPES_NAME_PARAMETER, occtype.Name);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.OCCTYPES_DESCRIPTION_PARAMETER, occtype.Description);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.OCCTYPES_DAMCAT_PARAMETER, occtype.DamageCategory);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.OCCTYPES_FOUND_UNCERTAINTY_PARAMETER, occtype.FoundationHeightUncertainty.ToXML());
        cmd.ExecuteNonQuery();

        cmd.CommandText = _insertAssetsCommandText;
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_OCCTYPE_PARAMETER, occtype.Name);
        var structAsset = occtype.StructureItem;
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_ASSETCATEGORY_PARAMETER, "StructureAsset");
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_ISSELECTED_PARAMETER, structAsset.IsChecked ? 1 : 0);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_TYPE_PARAMETER, structAsset.ItemType);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_BYVALUE_PARAMETER, null);
        var curveVM = structAsset.Curve;
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVE_PARAMETER, curveVM.SelectedItem);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVENAME_PARAMETER, curveVM.SelectedItem.Name);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_DISTOPTIONS_PARAMETER, "DistOptionsPlaceHolder");
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_DESCRIPTION_PARAMETER, curveVM.Description);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_CURVEDATA_Parameter, "CurveDataPlaceHolder");
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_UNCERTAINTY_PARAMETER, structAsset.ValueUncertainty.CreateOrdinate().ToXML());
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.ASSETS_RATIOUNCERTAINTY_PARAMETER, null);
        cmd.ExecuteNonQuery();

    }

    public void SaveMetadata(OccupancyTypesElement occtypeElem)
    {
        if (occtypeElem == null)
            return;

        using var cmd = new SQLiteCommand(_connection);
        cmd.CommandText = _insertMetadataCommandText;
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.METADATA_NAME_PARAMETER, occtypeElem.Name);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.METADATA_DESCRIPTION_PARAMETER, occtypeElem.Description);
        cmd.Parameters.AddWithValue(OccupancyTypesSQLiteConstants.METADATA_LASTEDIT_PARAMETER, occtypeElem.LastEditDate);
        cmd.ExecuteNonQuery();
    }

    private static void CreateTables(SQLiteConnection connection)
    {
        using var cmd = new SQLiteCommand(connection);
        cmd.CommandText = _createMetadataTableCommandText;
        cmd.ExecuteNonQuery();
        cmd.CommandText = _createOccupancyTypesTableCommandText;
        cmd.ExecuteNonQuery();
        cmd.CommandText = _createAssetsTableCommandText;
        cmd.ExecuteNonQuery();
    }
}
