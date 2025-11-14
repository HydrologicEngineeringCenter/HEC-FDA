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
                {OccupancyTypesSQLiteConstants.ASSETS_ISSELECTED_HEADER} INTEGER NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_TYPE_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_BYVALUE_HEADER} INTEGER NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVE_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_SELECTEDCURVENAME_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_DISTOPTIONS_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_CURVEDATA_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_DESCRIPTION_HEADER} TEXT NOT NULL,
                {OccupancyTypesSQLiteConstants.ASSETS_UNCERTAINTY_HEADER} TEXT,
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

        //using var insertCommand = new SQLiteCommand(_connection);
        //BuildInsertCommand(insertCommand);
        //InsertIntoTable(insertCommand, occtype);
    }

    public void SaveMetadata(OccupancyTypesElement occtypeElem)
    {
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


    //private static void BuildInsertCommand(SQLiteCommand cmd)
    //{
    //    cmd.CommandText = _insertMetadataCommandText;
    //    cmd.Parameters.Add(OccupancyTypesSQLiteConstants.XML_PARAMETER, System.Data.DbType.String);
    //}

    //private static void InsertIntoTable(SQLiteCommand cmd, OccupancyType occtype)
    //{
    //    var xml = occtype.ToXML();
    //    cmd.Parameters[OccupancyTypesSQLiteConstants.XML_PARAMETER].Value = xml;
    //    cmd.ExecuteNonQuery();
    //}
}
