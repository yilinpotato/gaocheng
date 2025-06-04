using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WeaponManager : MonoBehaviour
{
    public GameObject[] availableWeapons;
    public Transform weaponHolder;

    public void GiveRandomWeapon()
    {
        foreach (Transform child in weaponHolder)
        {
            Destroy(child.gameObject); // 清除旧武器
        }

        int index = Random.Range(0, availableWeapons.Length);
        GameObject weapon = Instantiate(availableWeapons[index], weaponHolder.position, Quaternion.identity);
        weapon.transform.SetParent(weaponHolder);
    }
}
