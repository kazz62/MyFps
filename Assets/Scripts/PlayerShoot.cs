using UnityEngine;
using Mirror;
[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;
    private WeaponManager weaponManager;
    private WeaponData currentWeapon;

    void Start()
    {
        if(cam == null) 
        {
            Debug.LogError("No camera assigned to shoot system");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if(PauseMenu.isOn)
        {
            return;
        }

        if(Input.GetButtonDown("Fire2") && weaponManager.GetCurrentGraphics().scope != null)
        {
            StartCoroutine(weaponManager.Scope());
            return;
        }

        if(Input.GetKeyDown(KeyCode.R) && weaponManager.currentMagazineSize < currentWeapon.magazineSize)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }
        
        if(currentWeapon.fireRate <= 0f)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else 
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal) 
    {
        RpcDoHitEffect(pos, normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    // Function called on server when player shoot (call to server)
    [Command]
    private void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    // Appear shoot effect in every player instance
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(currentWeapon.shootSound);
    }

    [Client]
    private void Shoot()
    {
        if(!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if(weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        weaponManager.currentMagazineSize--;
        Debug.Log("currentMagazine : " + weaponManager.currentMagazineSize);

        CmdOnShoot();

        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            if (hit.collider.tag == "Player")
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage, transform.name);
            }

            CmdOnHit(hit.point, hit.normal);
        }
        
        if(weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }
    }

    [Command]
    private void CmdPlayerShot(string playerId, float damage, string sourceID) 
    {
        Debug.Log(playerId + " has been hit");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage, sourceID);
    }
}
