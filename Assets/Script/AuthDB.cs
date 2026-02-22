using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;

public class AuthDB : MonoBehaviour
{
    IDbConnection db;

    void Start()
    {
        string path = "URI=file:" + Application.persistentDataPath + "/Game.sqlite";
        db = new SqliteConnection(path);
        db.Open();
    }

    void OnDestroy()
    {
        if (db != null)
            db.Close();
    }

    public string Register(string user, string pass)
    {
        if (pass.Length < 8)
            return "Contrasenya mínim 8 caràcters";

        try
        {
            IDbCommand cmd = db.CreateCommand();
            cmd.CommandText =
                "INSERT INTO Users (Username, Password) VALUES ('" + user + "','" + pass + "')";
            cmd.ExecuteNonQuery();
            return "Registre correcte";
        }
        catch
        {
            return "Usuari ja existent";
        }
    }

    public string Login(string user, string pass)
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "SELECT Password FROM Users WHERE Username='" + user + "'";
        IDataReader r = cmd.ExecuteReader();

        if (!r.Read())
            return "Usuari no trobat";

        if (r.GetString(0) != pass)
            return "Contrasenya incorrecta";

        return "OK";
    }
}