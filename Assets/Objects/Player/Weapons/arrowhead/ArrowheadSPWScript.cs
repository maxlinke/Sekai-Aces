using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowheadSPWScript : PlayerSpecialWeapon {

	[SerializeField] int maxAmmo;
	[SerializeField] float fireInterval;

	int ammo;
	float nextFire;

	void Start(){
		Reset();
	}

	public override void Fire(){
		if(CanFire()){
			ammo -= 1;
			nextFire = Time.time + fireInterval;
			Debug.Log("arrowheadSPW - bang " + Time.time);
		}
	}

	public override void Reload(){
		ammo = maxAmmo;
	}

	public override bool IsEmpty(){
		return (ammo <= 0);
	}

	public override bool CanFire(){
		return (!IsEmpty() && (Time.time > nextFire));
	}

	public override bool CanBeReloaded(){
		return (ammo < maxAmmo);
	}

	public override int GetAmmoCount(){
		return ammo;
	}

	public override void Reset(){
		nextFire = Mathf.NegativeInfinity;
		ammo = maxAmmo;
	}
}
