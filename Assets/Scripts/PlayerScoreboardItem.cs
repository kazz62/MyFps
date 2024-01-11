using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreboardItem : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI usernameText;
    [SerializeField]
    TextMeshProUGUI killsText;
    [SerializeField]
    TextMeshProUGUI deathsText;

    public void Setup(Player player)
    {
        usernameText.text = player.username;
        killsText.text = "Kills : " + player.kills;
        deathsText.text = "Deaths : " + player.deaths;
    }

}
