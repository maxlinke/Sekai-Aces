﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletPool : ObjectPool {

	static SimpleBulletPool friendlyNormalPoolInstance;
	static SimpleBulletPool friendlyFastPoolInstance;

	static SimpleBulletPool enemySlowPoolInstance;
	static SimpleBulletPool enemyNormalPoolInstance;
	static SimpleBulletPool enemyFastPoolInstance;

	[SerializeField] BulletPoolType type;

		[Header("Prefabs")]
	[SerializeField] GameObject bulletPrefab;

		[Header("Settings")]
	[SerializeField] float bulletSpeed;
	[SerializeField] int bulletDamage;
	[SerializeField] float bulletRange;

	float sqrBulletRange;
	int bulletCount;

	List<Rigidbody> activeBullets;
	List<Rigidbody> inactiveBullets;
	List<Rigidbody> returningBullets;

	enum BulletPoolType{
		FRIENDLY_NORMAL, FRIENDLY_FAST, ENEMY_SLOW, ENEMY_NORMAL, ENEMY_FAST
	}

	public float BulletSpeed{get{return bulletSpeed;}}
	
	void Update(){
		for(int i=0; i<activeBullets.Count; i++){
			if(activeBullets[i].transform.localPosition.sqrMagnitude > sqrBulletRange){
				returningBullets.Add(activeBullets[i]);
			}
		}
		for(int i=0; i<returningBullets.Count; i++){
			ReturnToInactivePool(returningBullets[i]);
		}
		returningBullets.Clear();
	}

	public override void Initialize () {
		bulletCount = 0;
		sqrBulletRange = bulletRange * bulletRange;
		CheckAndSetInstance();
		inactiveBullets = new List<Rigidbody>();
		activeBullets = new List<Rigidbody>();
		returningBullets = new List<Rigidbody>();
	}

	public override void ResetPool () {
		returningBullets.AddRange(activeBullets);
		for(int i=0; i<returningBullets.Count; i++){
			ReturnToInactivePool(returningBullets[i]);
		}
		returningBullets.Clear();
	}

	public void NewBullet(Vector3 position, Vector3 direction){
		direction = direction.normalized;
		Rigidbody bullet;
		if(!TryTakeBulletFromInactivePool(out bullet)){
			bulletCount++;
			GameObject bulletObject = Instantiate(bulletPrefab) as GameObject;
			bulletObject.transform.parent = this.transform;
			bulletObject.name = "bullet " + bulletCount;
			SimpleBulletScript bulletScript = bulletObject.GetComponent<SimpleBulletScript>();
			bulletScript.damage = bulletDamage;
			bulletScript.pool = this;
			bullet = bulletObject.GetComponent<Rigidbody>();
		}
		bullet.gameObject.SetActive(true);
		bullet.transform.position = position;
		bullet.transform.localRotation = Quaternion.LookRotation(direction);
		bullet.velocity = direction * bulletSpeed;
		activeBullets.Add(bullet);
	}

	bool TryTakeBulletFromInactivePool(out Rigidbody bullet){
		int count = inactiveBullets.Count;
		if(count > 0){
			int index = count - 1;
			bullet = inactiveBullets[index];
			inactiveBullets.RemoveAt(index);
			return true;
		}else{
			bullet = null;
			return false;
		}
	}

	public void ReturnToInactivePool(Rigidbody bullet){
		if(bullet.gameObject.activeSelf){
			bullet.gameObject.SetActive(false);
			activeBullets.Remove(bullet);
			inactiveBullets.Add(bullet);
		}
	}

	public static SimpleBulletPool GetFriendlyNormalPoolInstance(){
		return friendlyNormalPoolInstance;
	}

	public static SimpleBulletPool GetFriendlyFastPoolInstance(){
		return friendlyFastPoolInstance;
	}

	public static SimpleBulletPool GetEnemySlowPoolInstance(){
		return enemySlowPoolInstance;
	}

	public static SimpleBulletPool GetEnemyNormalPoolInstance(){
		return enemyNormalPoolInstance;
	}

	public static SimpleBulletPool GetEnemyFastPoolInstance(){
		return enemyFastPoolInstance;
	}

	void CheckAndSetInstance(){
		switch(type){
		case BulletPoolType.FRIENDLY_NORMAL: 
			if(friendlyNormalPoolInstance != null) throw new UnityException(type.ToString() + " bulletpool instance is not null (singleton...)");
			else friendlyNormalPoolInstance = this;
			break;
		case BulletPoolType.FRIENDLY_FAST:
			if(friendlyFastPoolInstance != null) throw new UnityException(type.ToString() + " bulletpool instance is not null (singleton...)");
			else friendlyFastPoolInstance = this;
			break;
		case BulletPoolType.ENEMY_SLOW:
			if(enemySlowPoolInstance != null) throw new UnityException(type.ToString() + " bulletpool instance is not null (singleton...)");
			else enemySlowPoolInstance = this;
			break;
		case BulletPoolType.ENEMY_NORMAL:
			if(enemyNormalPoolInstance != null) throw new UnityException(type.ToString() + " bulletpool instance is not null (singleton...)");
			else enemyNormalPoolInstance = this;
			break;
		case BulletPoolType.ENEMY_FAST:
			if(enemyFastPoolInstance != null) throw new UnityException(type.ToString() + " bulletpool instance is not null (singleton...)");
			else enemyFastPoolInstance = this;
			break;
		default:
			throw new UnityException("unknown bullet pool type \"" + type.ToString() + "\"");
		}
	}

}
