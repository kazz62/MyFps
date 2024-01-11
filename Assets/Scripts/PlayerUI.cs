using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform thrusterFuelFill;
    [SerializeField]
    private RectTransform healthBarFill;
    private Player player;
    private PlayerController playerController;
    private WeaponManager weaponManager;

    [SerializeField]
    private TextMeshProUGUI ammoText;

    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject scoreboard;

    public void SetPlayer(Player _player)
    {
        player = _player;
        playerController = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    private void Start()
    {
        PauseMenu.isOn = false;
    }

    private void Update() 
    {
        SetFuelAmount(playerController.GetThrusterFuelAmount());
        SetHealthAmount(player.GetHealthPct());
        SetAmmoAmount(weaponManager.currentMagazineSize);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TooglePauseMenu();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }
    }

    public void TooglePauseMenu()
    {   
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new UnityEngine.Vector3(1f, _amount, 1f);
    }

    void SetHealthAmount(float _amount)
    {
        healthBarFill.localScale = new UnityEngine.Vector3(1f, _amount, 1f);
    }

    void SetAmmoAmount(int _amount)
    {
        ammoText.text = _amount.ToString();
    }
}
