using Mirror;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    private string remoteLayerName = "RemotePlayer";
    [SerializeField]
    private string dontDrawLayerName = "DontDraw";

    [SerializeField]
    private GameObject playerGraphics;
    [SerializeField]
    private GameObject playerNameplateGraphics;

    [SerializeField]
    private GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;
    private void Start()
    {
        if(!isLocalPlayer) 
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else 
        {

            // Disabled graphic part of local player for camera fps view
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
            Util.SetLayerRecursively(playerNameplateGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Create UI for local player
            playerUIInstance = Instantiate(playerUIPrefab);

            // Config UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if(ui == null)
            {
                Debug.LogError("No PlayerUI component on PlayerUIInstace");
            }
            else 
            {
                ui.SetPlayer(GetComponent<Player>());
            }
            GetComponent<Player>().Setup();
        } 
    }

    [Command]
    void CmdSetUsername(string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);
        if(player != null)
        {
            Debug.Log(username + " has joined");
            player.username = username;
        }
    }

    private void DisableComponents() 
    {
        // Disable components on other players for our instance
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        RegisterPlayerAndSetUsername();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        RegisterPlayerAndSetUsername();
    }

    private void RegisterPlayerAndSetUsername()
    {
        string netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPLayer(netId, player);
        CmdSetUsername(transform.name, UserAccountManager.loggedInUsername);
    }

    private void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);        
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
        }
        GameManager.UnregisterPlayer(transform.name);
    }
}
