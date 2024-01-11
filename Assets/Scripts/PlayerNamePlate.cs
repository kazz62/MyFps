using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNamePlate : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI usernameText;

    [SerializeField]
    private RectTransform healthBarFill;

    [SerializeField]
    private Player player;

    void Update()
    {
        usernameText.text = player.username;
        healthBarFill.localScale = new Vector3(player.GetHealthPct(), 1f, 1f);
    }
}
