using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSystem : MonoBehaviour {

		[Header("Variables")]
	[SerializeField] float normalFireRate;
	[SerializeField] float increasedFireRate;
	[SerializeField] float weaponSpread;
	[SerializeField] int collisionDamage;

	[HideInInspector] public PlayerInput playerInput;
	[HideInInspector] public PlayerModel playerModel;
	[HideInInspector] public PlayerSpecialWeapon specialWeapon;
	[HideInInspector] public PlayerGUI gui;

	SimpleBulletPool normalBulletPool;
	SimpleBulletPool fastBulletPool;

	GameplayMode mode;
	public GameplayMode Mode{
		set{
			mode = value;
			bulletDirectionScale = GetDirectionScaleVector();
		}
	}

	bool useFastBullets;
	bool useIncreasedFireRate;
	float nextShot;
	Vector3 bulletDirectionScale;

	void Start () {
		normalBulletPool = SimpleBulletPool.GetPool(SimpleBulletPool.BulletPoolType.FRIENDLY_NORMAL);
		fastBulletPool = SimpleBulletPool.GetPool(SimpleBulletPool.BulletPoolType.FRIENDLY_FAST);
		RespawnReset();
	}
	
	void Update () {
		if(Time.timeScale > 0f){
			if(Input.GetKeyDown(KeyCode.M))	useFastBullets = !useFastBullets;	//TODO remove this. its just for testing
			if(Input.GetKeyDown(KeyCode.N)) useIncreasedFireRate = !useIncreasedFireRate;

			if(playerInput.GetFireInput()){
				if(Time.time > nextShot){
					if(useFastBullets){
						fastBulletPool.NewBullet(transform.position, GetBulletDirection());
					}else{
						normalBulletPool.NewBullet(transform.position, GetBulletDirection());
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
						gui.SetSPWAmmoNumber(specialWeapon.GetAmmoCount());
						gui.SetSPWDisplayState(false);
					}else{
						//TODO uisounds.playerrorsound() or something like that
						Debug.Log("wait");
					}
				}else{
					//TODO uisounds.playerrorsound() or something like that
					Debug.Log("empty");
				}
			}
			gui.SetSPWAmmoNumber(specialWeapon.GetAmmoCount());
			gui.SetSPWDisplayState(specialWeapon.CanFire());
		}
	}

	public void RespawnReset () {
		specialWeapon.Reset();
		nextShot = Mathf.NegativeInfinity;
		useFastBullets = false;
		useIncreasedFireRate = false;
		gui.SetSPWAmmoNumber(specialWeapon.GetAmmoCount());
		gui.SetSPWDisplayState(true);
	}

	public bool WeaponCanBeUpgraded () {
		return !(useFastBullets && useIncreasedFireRate);
	}

	public void UpgradeWeapon () {
		useFastBullets = true;
		useIncreasedFireRate = true;
		playerModel.Shine(new Color(1f, 1f, 0f));
	}

	public bool SpecialWeaponCanBeReloaded () {
		return specialWeapon.CanBeReloaded();
	}

	public void ReloadSpecialWeapon () {
		specialWeapon.Reload();
		playerModel.Shine(new Color(0.2f, 0.4f, 1f));
		//TODO reload sound
	}

	public void DealCollisionDamage (IDamageable damageable) {
		damageable.CollisionDamage(collisionDamage);
	}

	Vector3 GetDirectionScaleVector () {
		switch(mode){
		case GameplayMode.TOPDOWN:
			return new Vector3(1,0,1);
		case GameplayMode.SIDE:
			return new Vector3(0,1,1);
		case GameplayMode.BACK:
			return new Vector3(1,1,0);
		default :
			throw new UnityException("Unknown GameplayMode \"" + mode.ToString() + "\"");
		}
	}

	Vector3 GetBulletDirection () {
		Vector3 randomOffset = Random.insideUnitSphere * weaponSpread;
		Vector3 flattenedOffset = Vector3.Scale(randomOffset, bulletDirectionScale);
		return Vector3.forward + flattenedOffset;
	}

}
