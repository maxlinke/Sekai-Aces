using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSystem : MonoBehaviour {

		[Header("Variables")]
	[SerializeField] float normalFireRate;
	[SerializeField] float increasedFireRate;
	[SerializeField] float weaponSpread;

	[HideInInspector] public PlayerInput playerInput;
	[HideInInspector] public PlayerModel playerModel;
	[HideInInspector] public PlayerSpecialWeapon specialWeapon;

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
//	bool useTripleShot;
	float nextShot;
	Vector3 bulletDirectionScale;

	void Start () {
		normalBulletPool = SimpleBulletPool.GetFriendlyNormalPoolInstance();
		fastBulletPool = SimpleBulletPool.GetFriendlyFastPoolInstance();
		RespawnReset();
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.M))	useFastBullets = !useFastBullets;
		if(Input.GetKeyDown(KeyCode.N)) useIncreasedFireRate = !useIncreasedFireRate;
//		if(Input.GetKeyDown(KeyCode.B)) useTripleShot = !useTripleShot;
		if(playerInput.GetFireInput()){
			if(Time.time > nextShot){
				if(useFastBullets){
					fastBulletPool.NewBullet(transform.position, GetBulletDirection());
//					if(useTripleShot){
//						fastBulletPool.NewBullet(transform.position + new Vector3(-0.4f, 0f, -0.2f));
//						fastBulletPool.NewBullet(transform.position + new Vector3(+0.4f, 0f, -0.2f));
//					}
				}else{
					normalBulletPool.NewBullet(transform.position, GetBulletDirection());
//					if(useTripleShot){
//						normalBulletPool.NewBullet(transform.position + new Vector3(-0.4f, 0f, -0.2f));
//						normalBulletPool.NewBullet(transform.position + new Vector3(+0.4f, 0f, -0.2f));
//					}
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

	public void RespawnReset(){
		specialWeapon.Reset();
		nextShot = Mathf.NegativeInfinity;
		useFastBullets = false;
		useIncreasedFireRate = false;
//		useTripleShot = false;
	}

	public bool SpecialWeaponCanBeReloaded(){
		return specialWeapon.CanBeReloaded();
	}

	public void ReloadSpecialWeapon(){
		specialWeapon.Reload();
		playerModel.Shine(Color.green);
		//TODO reload sound
	}

	Vector3 GetDirectionScaleVector(){
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

	Vector3 GetBulletDirection(){
		Vector3 randomOffset = Random.insideUnitSphere * weaponSpread;
		Vector3 flattenedOffset = Vector3.Scale(randomOffset, bulletDirectionScale);
		return Vector3.forward + flattenedOffset;
	}

}
