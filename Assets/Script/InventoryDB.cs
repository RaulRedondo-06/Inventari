using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;

public class InventoryDB : MonoBehaviour
{
    IDbConnection OpenDB()
    {
        string path = "URI=file:" + Application.persistentDataPath + "/Game.sqlite";
        IDbConnection c = new SqliteConnection(path);
        c.Open();
        return c;
    }

    public void LoggedInv()
    {
        IDbConnection db = OpenDB();
        int invId = GetLoggedInvId(db);
        db.Close();
    }

    [System.Serializable]
    public class InventoryRow
    {
        public int stackId;
        public int itemId;
        public string itemName;
        public int quantity;
        public string categoryName;
        public int isStackable;
        public int maxStack;
        public string description;
    }

    int GetUserId(IDbConnection db, string username)
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "SELECT UserID FROM Users WHERE Username='" + username + "'";
        IDataReader r = cmd.ExecuteReader();

        if (r.Read())
            return r.GetInt32(0);

        return -1;
    }

    int EnsureInventoryForUser(IDbConnection db, int userId)
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "SELECT InvID FROM Inventories WHERE UserID=" + userId;
        IDataReader r = cmd.ExecuteReader();

        if (r.Read())
            return r.GetInt32(0);

        IDbCommand cmdInv = db.CreateCommand();
        cmdInv.CommandText = "INSERT INTO Inventories (UserID) VALUES (" + userId + ")";
        cmdInv.ExecuteNonQuery();

        IDbCommand cmdInvId = db.CreateCommand();
        cmdInvId.CommandText = "SELECT InvID FROM Inventories WHERE UserID=" + userId;
        IDataReader r2 = cmdInvId.ExecuteReader();

        if (r2.Read())
            return r2.GetInt32(0);

        return -1;
    }

    int GetLoggedInvId(IDbConnection db)
    {
        string username = PlayerPrefs.GetString("logged_user", "");

        if (username == "")
            return -1;

        int userId = GetUserId(db, username);

        if (userId < 0)
            return -1;

        return EnsureInventoryForUser(db, userId);
    }

    bool GetItemData(IDbConnection db, string itemName, out int itemId, out int isStackable, out int maxStack)
    {
        itemId = -1;
        isStackable = 0;
        maxStack = 1;

        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ItemID, IsStackable, MaxStack FROM Items WHERE Name='" + itemName + "'";
        IDataReader r = cmd.ExecuteReader();

        if (!r.Read())
            return false;

        itemId = r.GetInt32(0);
        isStackable = r.GetInt32(1);
        maxStack = r.GetInt32(2);

        return true;
    }

    bool HasAnyStack(IDbConnection db, int invId, int itemId)
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "SELECT 1 FROM Stacks WHERE InvID=" + invId + " AND ItemID=" + itemId + " LIMIT 1";
        IDataReader r = cmd.ExecuteReader();

        if (r.Read())
            return true;

        return false;
    }

    bool GetLastStack(IDbConnection db, int invId, int itemId, out int stackId, out int qty)
    {
        stackId = -1;
        qty = 0;

        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText =
            "SELECT StackID, Quantity FROM Stacks " +
            "WHERE InvID=" + invId + " AND ItemID=" + itemId + " " +
            "ORDER BY StackID DESC LIMIT 1";

        IDataReader r = cmd.ExecuteReader();

        if (!r.Read())
            return false;

        stackId = r.GetInt32(0);
        qty = r.GetInt32(1);
        return true;
    }

    void InsertStack(IDbConnection db, int invId, int itemId, int qty)
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText =
            "INSERT INTO Stacks (InvID, ItemID, Quantity) VALUES (" +
            invId + ", " + itemId + ", " + qty + ")";
        cmd.ExecuteNonQuery();
    }

    void UpdateStackQuantity(IDbConnection db, int stackId, int qty)
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "UPDATE Stacks SET Quantity=" + qty + " WHERE StackID=" + stackId;
        cmd.ExecuteNonQuery();
    }

    void DeleteStack(IDbConnection db, int stackId)
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "DELETE FROM Stacks WHERE StackID=" + stackId;
        cmd.ExecuteNonQuery();
    }

    public List<InventoryRow> GetInventoryByCategory(string categoryName)
    {
        IDbConnection db = OpenDB();

        List<InventoryRow> list = new List<InventoryRow>();
        int invId = GetLoggedInvId(db);

        if (invId < 0)
        {
            db.Close();
            return list;
        }

        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText =
            "SELECT s.StackID, i.ItemID, i.Name, s.Quantity, c.Name, i.IsStackable, i.MaxStack, i.Description " +
            "FROM Stacks s " +
            "JOIN Items i ON i.ItemID = s.ItemID " +
            "JOIN Categories c ON c.CategoryID = i.CategoryID " +
            "WHERE s.InvID=" + invId + " AND c.Name='" + categoryName + "' " +
            "ORDER BY i.Name ASC, s.StackID ASC";

        IDataReader r = cmd.ExecuteReader();

        while (r.Read())
        {
            InventoryRow row = new InventoryRow();

            row.stackId = r.GetInt32(0);
            row.itemId = r.GetInt32(1);
            row.itemName = r.GetString(2);
            row.quantity = r.GetInt32(3);
            row.categoryName = r.GetString(4);
            row.isStackable = r.GetInt32(5);
            row.maxStack = r.GetInt32(6);

            if (r.IsDBNull(7))
                row.description = "";
            else
                row.description = r.GetString(7);

            list.Add(row);
        }

        db.Close();
        return list;
    }

    public string AddItemByName(string itemName, int amount)
    {
        if (amount <= 0)
            return "Quantitat invàlida";

        IDbConnection db = OpenDB();
        int invId = GetLoggedInvId(db);

        if (invId < 0)
        {
            db.Close();
            return "No hi ha usuari loguejat";
        }

        int itemId;
        int isStackable;
        int maxStack;

        bool ok = GetItemData(db, itemName, out itemId, out isStackable, out maxStack);
        if (!ok)
        {
            db.Close();
            return "Item no existeix";
        }

        if (isStackable == 0)
        {
            bool already = HasAnyStack(db, invId, itemId);
            if (already)
            {
                db.Close();
                return "Ja tens aquest objecte clau";
            }

            InsertStack(db, invId, itemId, 1);
            db.Close();
            return "Afegit (clau)";
        }

        int remaining = amount;

        while (remaining > 0)
        {
            int lastStackId;
            int lastQty;

            bool hasLast = GetLastStack(db, invId, itemId, out lastStackId, out lastQty);

            if (hasLast)
            {
                if (lastQty < maxStack)
                {
                    int free = maxStack - lastQty;
                    int add;

                    if (remaining <= free)
                        add = remaining;
                    else
                        add = free;

                    UpdateStackQuantity(db, lastStackId, lastQty + add);
                    remaining = remaining - add;
                }
                else
                {
                    int addNew;

                    if (remaining <= maxStack)
                        addNew = remaining;
                    else
                        addNew = maxStack;

                    InsertStack(db, invId, itemId, addNew);
                    remaining = remaining - addNew;
                }
            }
            else
            {
                int addNew;

                if (remaining <= maxStack)
                    addNew = remaining;
                else
                    addNew = maxStack;

                InsertStack(db, invId, itemId, addNew);
                remaining = remaining - addNew;
            }
        }

        db.Close();
        return "Afegit correctament";
    }

    public string UseItemByName(string itemName)
    {
        IDbConnection db = OpenDB();
        int invId = GetLoggedInvId(db);

        if (invId < 0)
        {
            db.Close();
            return "No hi ha usuari loguejat";
        }

        int itemId;
        int isStackable;
        int maxStack;

        bool ok = GetItemData(db, itemName, out itemId, out isStackable, out maxStack);
        if (!ok)
        {
            db.Close();
            return "Item no existeix";
        }

        int stackId;
        int qty;

        bool has = GetLastStack(db, invId, itemId, out stackId, out qty);
        if (!has)
        {
            db.Close();
            return "No el tens";
        }

        qty = qty - 1;

        if (qty <= 0)
        {
            DeleteStack(db, stackId);
            db.Close();
            return "Gastat (-1 stack)";
        }

        UpdateStackQuantity(db, stackId, qty);
        db.Close();
        return "Gastat (-1)";
    }


}