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
                {OccupancyTypesSQLiteConstants.METADATA_ID_HEADER}       INTEGER PRIMARY KEY AUTOINCREMENT,
                {OccupancyTypesSQLiteConstants.XML_HEADER}      TEXT NOT NULL
            );";

    private static readonly string _createOccupancyTypesTableCommandText =
        $@"
            CREATE TABLE IF NOT EXISTS";

    private static readonly string _insertCommandText =
        $@"
            INSERT OR IGNORE INTO {OccupancyTypesSQLiteConstants.METADATA_TABLE_NAME} (
                {OccupancyTypesSQLiteConstants.XML_HEADER}            
            )
            VALUES (
                {OccupancyTypesSQLiteConstants.XML_PARAMETER}            
            );";

    public OccupancyTypeSaver(string dbPath) : base(dbPath)
    {
        CreateTable(_connection);
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

        using var insertCommand = new SQLiteCommand(_connection);
        BuildInsertCommand(insertCommand);
        InsertIntoTable(insertCommand, occtype);
    }

    private static void CreateTable(SQLiteConnection connection)
    {
        using var cmd = new SQLiteCommand(connection);
        cmd.CommandText = _createMetadataTableCommandText;
        cmd.ExecuteNonQuery();
    }

    private static void BuildInsertCommand(SQLiteCommand cmd)
    {
        cmd.CommandText = _insertCommandText;
        cmd.Parameters.Add(OccupancyTypesSQLiteConstants.XML_PARAMETER, System.Data.DbType.String);
    }

    private static void InsertIntoTable(SQLiteCommand cmd, OccupancyType occtype)
    {
        var xml = occtype.ToXML();
        cmd.Parameters[OccupancyTypesSQLiteConstants.XML_PARAMETER].Value = xml;
        cmd.ExecuteNonQuery();
    }
}
