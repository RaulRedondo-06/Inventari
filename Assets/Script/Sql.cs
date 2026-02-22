using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;

public class Sql : MonoBehaviour
{

    private int hitCount;
    private IDbConnection CreateAndOpenDatabase() 
    {
        string dbUri = "URI=file:" + Application.dataPath + "/MyDatabase.sqlite"; 
        IDbConnection dbConnection = new SqliteConnection(dbUri); 
        dbConnection.Open(); 

        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText =
            "CREATE TABLE IF NOT EXISTS HitCountTableSimple (id INTEGER PRIMARY KEY, hits INTEGER )";
        dbCommandCreateTable.ExecuteReader();

        return dbConnection;
    }

    private void OnMouseDown()
    {
        hitCount++;

        IDbConnection dbConnection = CreateAndOpenDatabase(); 
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand(); 
        dbCommandInsertValue.CommandText =
            "INSERT OR REPLACE INTO HitCountTableSimple (id, hits) VALUES (0, " + hitCount + ")"; 
        dbCommandInsertValue.ExecuteNonQuery(); 
        dbConnection.Close(); 
    }


}


