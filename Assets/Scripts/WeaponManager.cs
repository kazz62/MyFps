using UnityEngine;
using Mirror;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private WeaponData primaryWeapon;

    private WeaponData currentWeapon;
    private WeaponGraphics currentGraphics;

    [SerializeField]
    private string weaponLayerName = "Weapon";
    [SerializeField]
    private Transform weaponHolder;
    [HideInInspector]
    public int currentMagazineSize;

    public bool isReloading = false;
    void Start()
    {  
        EquipWeapon(primaryWeapon);
    }

    public void EquipWeapon(WeaponData _weapon)
    {
        currentWeapon = _weapon;
        currentMagazineSize = _weapon.magazineSize;

        GameObject weaponIns = Instantiate(currentWeapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = weaponIns.GetComponent<WeaponGraphics>();

        if(currentGraphics == null)
        {
            Debug.LogError("No script WeaponGraphics on the weapon : " + weaponIns.name);
        }

        if(isLocalPlayer)
        {
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public IEnumerator Reload()
    {
        if(isReloading)
        {
            yield break;
        }

        Debug.Log("Reloading ...");

        isReloading = true;
        CmdOnReload();
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        currentMagazineSize = currentWeapon.magazineSize;

        isReloading = false;

        Debug.Log("Reloading Finished");
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator animator = currentGraphics.GetComponent<Animator>();
        if(animator != null)
        {
            animator.SetTrigger("Reload");
        }
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(currentWeapon.reloadSound);
    }
}
