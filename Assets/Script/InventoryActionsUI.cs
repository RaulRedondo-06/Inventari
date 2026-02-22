using UnityEngine;
using TMPro;

public class InventoryActionsUI : MonoBehaviour
{
    public InventoryDB db;
    public InventoryUI ui;

    [Header("Opcional: mensajes")]
    public TMP_Text msgText;

    void ShowMsg(string msg)
    {
        if (msgText != null)
            msgText.text = msg;
    }

    public void AddPotion()
    {
        string msg = db.AddItemByName("Poción", 10);
        ShowMsg(msg);
        ui.Refresh();
    }

    public void UsePotion()
    {
        string msg = db.UseItemByName("Poción");
        ShowMsg(msg);
        ui.Refresh();
    }

    public void AddSuperPotion()
    {
        string msg = db.AddItemByName("Superpoción", 10);
        ShowMsg(msg);
        ui.Refresh();
    }

    public void UseSuperPotion()
    {
        string msg = db.UseItemByName("Superpoción");
        ShowMsg(msg);
        ui.Refresh();
    }

    public void AddAntidote()
    {
        string msg = db.AddItemByName("Antídoto", 10);
        ShowMsg(msg);
        ui.Refresh();
    }

    public void UseAntidote()
    {
        string msg = db.UseItemByName("Antídoto");
        ShowMsg(msg);
        ui.Refresh();
    }

    public void AddPokeball()
    {
        string msg = db.AddItemByName("Poké Ball", 10);
        ShowMsg(msg);
        ui.Refresh();
    }

    public void UsePokeball()
    {
        string msg = db.UseItemByName("Poké Ball");
        ShowMsg(msg);
        ui.Refresh();
    }

    public void AddSuperBall()
    {
        string msg = db.AddItemByName("Super Ball", 10);
        ShowMsg(msg);
        ui.Refresh();
    }

    public void UseSuperBall()
    {
        string msg = db.UseItemByName("Super Ball");
        ShowMsg(msg);
        ui.Refresh();
    }

    public void AddUltraBall()
    {
        string msg = db.AddItemByName("Ultra Ball", 10);
        ShowMsg(msg);
        ui.Refresh();
    }

    public void UseUltraBall()
    {
        string msg = db.UseItemByName("Ultra Ball");
        ShowMsg(msg);
        ui.Refresh();
    }

    public void AddBike()
    {
        string msg = db.AddItemByName("Bicicleta", 1);
        ShowMsg(msg);
        ui.Refresh();
    }

    public void AddMap()
    {
        string msg = db.AddItemByName("Mapa", 1);
        ShowMsg(msg);
        ui.Refresh();
    }
}