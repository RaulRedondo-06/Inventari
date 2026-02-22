using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("DB")]
    public InventoryDB db;

    [Header("UI")]
    public TMP_Text listText;
    public TMP_Text titleText;

    [Header("Current Tab")]
    public string currentCategory = "Objetos";

    void Start()
    {
        if (db != null)
            db.LoggedInv();

        ShowObjetos();
    }

    public void ShowObjetos()
    {
        ShowCategory("Objetos");
    }

    public void ShowPokeballs()
    {
        ShowCategory("Pokeballs");
    }

    public void ShowObjetosClave()
    {
        ShowCategory("Objetos clave");
    }

    void ShowCategory(string categoryName)
    {
        currentCategory = categoryName;

        if (titleText != null)
            titleText.text = categoryName;

        Refresh();
    }

    public void Refresh()
    {
        if (db == null || listText == null)
            return;

        List<InventoryDB.InventoryRow> rows = db.GetInventoryByCategory(currentCategory);

        if (rows.Count == 0)
        {
            listText.text = "(buit)";
            return;
        }

        string s = "";
        for (int i = 0; i < rows.Count; i++)
        {
            InventoryDB.InventoryRow r = rows[i];
            s += r.itemName + " x" + r.quantity + "\n";
        }

        listText.text = s;
    }
}