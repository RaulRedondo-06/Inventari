using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AuthUI : MonoBehaviour
{
    public AuthDB db;

    public TMP_InputField user;
    public TMP_InputField password;

    public GameObject popup;
    public TMP_Text popupText;

    public void Register()
    {
        string msg = db.Register(user.text, password.text);
        Popup(msg);
    }

    public void Login()
    {
        string msg = db.Login(user.text, password.text);

        if (msg == "OK")
        {
            PlayerPrefs.SetString("logged_user", user.text);
            PlayerPrefs.Save();

            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Popup(msg);
        }
    }

    void Popup(string msg)
    {
        popup.SetActive(true);
        popupText.text = msg;
    }

    public void ClosePopup()
    {
        popup.SetActive(false);
    }
}