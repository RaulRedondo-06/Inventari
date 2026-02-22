using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;

public class GameDB : MonoBehaviour
{
    void Start()
    {
        string path = "URI=file:" + Application.persistentDataPath + "/Game.sqlite";
        Debug.Log(path);
        IDbConnection db = new SqliteConnection(path);
        db.Open();

        IDbCommand cmd = db.CreateCommand();

        // USUARI
        cmd.CommandText =
            "CREATE TABLE IF NOT EXISTS Users (" +
            "UserID INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Username TEXT UNIQUE, " +
            "Password TEXT)";
        cmd.ExecuteNonQuery();

        // CATEGORIES
        cmd.CommandText =
            "CREATE TABLE IF NOT EXISTS Categories (" +
            "CategoryID INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Name TEXT UNIQUE)";
        cmd.ExecuteNonQuery();

        // ITEMS
        cmd.CommandText =
            "CREATE TABLE IF NOT EXISTS Items (" +
            "ItemID INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Name TEXT UNIQUE, " +
            "CategoryID INTEGER, " +
            "IsStackable INTEGER, " +
            "MaxStack INTEGER, " +
            "Description TEXT)";
        cmd.ExecuteNonQuery();

        // INVENTARI
        cmd.CommandText =
            "CREATE TABLE IF NOT EXISTS Inventories (" +
            "InvID INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "UserID INTEGER UNIQUE)";
        cmd.ExecuteNonQuery();

        // STACKS
        cmd.CommandText =
            "CREATE TABLE IF NOT EXISTS Stacks (" +
            "StackID INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "InvID INTEGER, " +
            "ItemID INTEGER, " +
            "Quantity INTEGER)";
        cmd.ExecuteNonQuery();

        InsertObject(cmd);

        db.Close();
    }

    void InsertObject(IDbCommand cmd)
    {
        InsertCategory(cmd, "Objetos");
        InsertCategory(cmd, "Pokeballs");
        InsertCategory(cmd, "Objetos clave");

        InsertItem(cmd, "Poción", "Objetos", 1, 99);
        InsertItem(cmd, "Superpoción", "Objetos", 1, 99);
        InsertItem(cmd, "Antídoto", "Objetos", 1, 99);

        InsertItem(cmd, "Poké Ball", "Pokeballs", 1, 99);
        InsertItem(cmd, "Super Ball", "Pokeballs", 1, 99);
        InsertItem(cmd, "Ultra Ball", "Pokeballs", 1, 99);

        InsertItem(cmd, "Bicicleta", "Objetos clave", 0, 1);
        InsertItem(cmd, "Mapa", "Objetos clave", 0, 1);
    }

    void InsertCategory(IDbCommand cmd, string name)
    {
        try
        {
            cmd.CommandText = "INSERT INTO Categories (Name) VALUES ('" + name + "')";
            cmd.ExecuteNonQuery();
        }
        catch { }
    }

    void InsertItem(IDbCommand cmd, string item, string cat, int stack, int max)
    {
        try
        {
            cmd.CommandText =
                "INSERT INTO Items (Name, CategoryID, IsStackable, MaxStack) " + "VALUES ('" + item + "', (SELECT CategoryID FROM Categories WHERE Name='" + cat + "'), " + stack + ", " + max + ")";
            cmd.ExecuteNonQuery();
        }
        catch { }
    }
}