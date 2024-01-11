using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager instance;

    public static string loggedInUsername;

    public string lobbySceneName = "Lobby";

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public void LogIn(TextMeshProUGUI username)
    {
        loggedInUsername = username.text;
        Debug.Log("logged in as : " + loggedInUsername);
        SceneManager.LoadScene(lobbySceneName);
    }
}
