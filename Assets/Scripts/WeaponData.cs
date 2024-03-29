using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "MyFps/Weapon Data")]
public class WeaponData : ScriptableObject
{
   public string name = "Submachine Gun";
   public float damage = 10f;
   public float range = 100f;
   public float fireRate = 0f;
   public int magazineSize = 10;
   public float reloadTime = 1.5f;
   public float scopeTime = 0.15f;
   public float scopeFOV = 15f;

   public GameObject graphics;

   public AudioClip shootSound;
   public AudioClip reloadSound;
}
