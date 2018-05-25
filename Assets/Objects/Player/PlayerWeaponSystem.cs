using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSystem : MonoBehaviour {

		[Header("Variables")]
	[SerializeField] float normalFireRate;
	[SerializeField] float increasedFireRate;

	[HideInInspector] public PlayerInput playerInput;
	[HideInInspector] public PlayerModel playerModel;
	[HideInInspector] public PlayerSpecialWeapon specialWeapon;

	SimpleBulletPool normalBulletPool;
	SimpleBulletPool fastBulletPool;

	bool useFastBullets;
	bool useIncreasedFireRate;
	float nextShot;

	void Start () {
		normalBulletPool = SimpleBulletPool.GetFriendlyNormalPoolInstance();
		fastBulletPool = SimpleBulletPool.GetFriendlyFastPoolInstance();
		Reset();
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.M))	useFastBullets = !useFastBullets;
		if(Input.GetKeyDown(KeyCode.N)) useIncreasedFireRate = !useIncreasedFireRate;
		if(playerInput.GetFireInput()){
			if(Time.time > nextShot){
				if(useFastBullets){
					fastBulletPool.NewBullet(transform.position);
				}else{
					normalBulletPool.NewBullet(transform.position);
				}
				if(useIncreasedFireRate){
					nextShot = Time.time + (1f / increasedFireRate);
				}else{
					nextShot = Time.time + (1f / normalFireRate);
				}
			}
		}
		if(playerInput.GetSpecialInputDown()){
			if(!specialWeapon.IsEmpty()){
				if(specialWeapon.CanFire()){
					specialWeapon.Fire();
				}else{
					//TODO uisounds.playerrorsound() or something like that
					Debug.Log("wait");
				}
			}else{
				//TODO uisounds.playerrorsound() or something like that
				Debug.Log("empty");
			}
		}
	}

	public void Reset(){
		specialWeapon.Reset();
		nextShot = Mathf.NegativeInfinity;
		useFastBullets = false;
		useIncreasedFireRate = false;
	}

	public bool SpecialWeaponCanBeReloaded(){
		return specialWeapon.CanBeReloaded();
	}

	public void ReloadSpecialWeapon(){
		specialWeapon.Reload();
		playerModel.Shine(Color.green);
		//TODO reload sound
	}

}
