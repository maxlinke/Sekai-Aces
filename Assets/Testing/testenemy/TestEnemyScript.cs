using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyScript : MonoBehaviour, IDamageable {

	[SerializeField] MeshRenderer mr;
	[SerializeField] Color normalColor;
	[SerializeField] Color damageColor;
	[SerializeField] float fallOffTime;
	[SerializeField] float oscillateTime;
	[SerializeField] float oscillateWidth;
	[SerializeField] float fireInterval;

	[SerializeField] GenericEnemyShootingBehavior gesb;

	MaterialPropertyBlock mpb;
	float lastDamageTime;
	float currentFallOffTime;
	float nextFire;
//	SimpleBulletPool bp;

	void Start () {
//		bp = SimpleBulletPool.GetEnemySlowPoolInstance();

		gesb.Initialize(GameObject.FindObjectsOfType<Player>());
		gesb.RespawnReset(GameplayMode.TOPDOWN);

		currentFallOffTime = fallOffTime;
		lastDamageTime = Mathf.NegativeInfinity;
		mpb = new MaterialPropertyBlock();
		mr.GetPropertyBlock(mpb);
	}
	
	void Update () {
//		if(bp == null){
//			bp = SimpleBulletPool.GetEnemySlowPoolInstance();
//		}
//		if(Time.time > nextFire && bp != null){
		if(Time.time > nextFire){
//			bp.NewBullet(transform.position, new Vector3(Mathf.Sin(Time.time), 0f, Mathf.Cos(Time.time)));
			gesb.Shoot();
			nextFire = Time.time + fireInterval;
		}
		float lerpVal = Mathf.Clamp01((Time.time - lastDamageTime) / currentFallOffTime);
		mpb.SetColor("_Color", Color.Lerp(damageColor, normalColor, lerpVal));
		mr.SetPropertyBlock(mpb);
		transform.localPosition = new Vector3(Mathf.Sin(Mathf.PI * 2f * Time.time / oscillateTime) * oscillateWidth, 0, transform.localPosition.z);
	}

	public void WeaponDamage(int amount){
		Damage(amount);
	}

	public void CollisionDamage(int amount){
		Damage(amount);
	}

	void Damage(int amount){
		lastDamageTime = Time.time;
		currentFallOffTime = fallOffTime * amount;
	}

	public void Kill(bool instantly){
		lastDamageTime = Time.time;
	}

	void DoCollisionDamage(GameObject otherObject){
		IDamageable damageable = otherObject.GetComponent<IDamageable>();
		if(damageable != null) damageable.CollisionDamage(1);
	}

	void OnCollisionEnter(Collision collision){
		DoCollisionDamage(collision.collider.gameObject);
	}

	void OnCollisionStay(Collision collision){
		DoCollisionDamage(collision.collider.gameObject);
	}

}
