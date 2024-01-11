using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    
    [SerializeField]
    private float maxHealth = 100f;

    [SyncVar]
    private float currentHealth;
    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }

    [SyncVar]
    public string username = "Player";

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[]  disableOnDeath;

    [SerializeField]
    private GameObject[]  disableGameObjectOnDeath;

    private bool[] wasEnabledOnStart;

    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;
    
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip destroySound;

    public void Setup() 
    {
        if(isLocalPlayer)
        {
            // Camera change
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }    
        CmdBroadcastNewPlayerSetup() ;
    }

    [Command(requiresAuthority =false)]
    private void CmdBroadcastNewPlayerSetup() 
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup)
        {
            wasEnabledOnStart = new bool[disableOnDeath.Length];
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                wasEnabledOnStart[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }

        SetDefaults();
    }

    public void SetDefaults() 
    {
        isDead = false;
        currentHealth = maxHealth;

        // Enable player script's
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabledOnStart[i];
        }

        // Enable player GameObject when Die
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(true);
        }

        // Enable player collider
        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = true;
        }

        // Spawn particle effect 
        GameObject _gfxIns = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);
    }

    private IEnumerator Respawn() 
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        Setup();
    }

    private void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(25,"Player1");
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount, string sourceID)
    {
        if(isDead)
        {
            return;
        }

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(hitSound);

        currentHealth -= amount;
        Debug.Log(transform.name + " is now : " + currentHealth + " HP.");

        if(currentHealth <= 0)
        {
            audioSource.PlayOneShot(destroySound);
            Die(sourceID);
        }
    }

    private void Die(string sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(sourceID);

        if(sourcePlayer != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
        } 

        deaths++;

        // Disable player component when Die
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // Disable player GameObject when Die
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(false);
        }

        // Disable player collider
        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = false;
        }

        // Death particle effect 
        GameObject _gfxIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        // Camera change
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " has been eliminated.");

        StartCoroutine(Respawn());
    }
}
