using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutB : MonoBehaviour
{
    [SerializeField] private string loginSceneName = "LoginScene";

    public void Logout()
    {
        PlayerPrefs.DeleteKey("logged_user");
        PlayerPrefs.Save();
        SceneManager.LoadScene(loginSceneName);
    }
}
