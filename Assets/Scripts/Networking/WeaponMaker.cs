using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponMaker{
	public static void GenerateWeapons(GameObject Unit){
		int WeaponsNum = UnityEngine.Random.Range (1, 3);
		for(int i = 0; i < WeaponsNum; i++){
			int Range = 2000;
			int Damage = UnityEngine.Random.Range(1, 6) * 5;
			int HitChance = UnityEngine.Random.Range(1, 11) * 5;
			int MaxAmmo = UnityEngine.Random.Range(10, 51) * 10;
			int MaxShots = UnityEngine.Random.Range(1, 6) * 10;

            //Debug.LogWarning("Weapon " + Range + " " + Damage + " " + HitChance + " " + MaxAmmo + " " + MaxShots);
            
			//Unit.GetComponent<AttackController>().AddWeapon(new Weapon("Test Weapon " + (i + 1), Range, Damage, HitChance, MaxAmmo, MaxShots));
		}
	}
}
