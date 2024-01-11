using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillfeedItem : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    public void Setup(string player, string source)
    {
        text.text = source + " killed " + player;
    }
}
