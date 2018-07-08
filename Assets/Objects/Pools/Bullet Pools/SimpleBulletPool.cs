using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletPool : RigidbodyPool {

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

	int bulletCount;

	enum BulletPoolType {
		FRIENDLY_NORMAL, FRIENDLY_FAST, ENEMY_SLOW, ENEMY_NORMAL, ENEMY_FAST
	}

	public float BulletSpeed { get { return bulletSpeed; } }
	
	void Update () {
		base.SqrDistReturnCheck();
	}

	public override void Initialize () {
		base.Initialize();
		bulletCount = 0;
		CheckAndSetInstance();
	}

	public void NewBullet (Vector3 position, Vector3 direction) {
		direction = direction.normalized;
		Rigidbody bullet;
		if(!TryTakeRBFromInactivePool(out bullet)){
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
		activeRBs.Add(bullet);
	}

	public static SimpleBulletPool GetFriendlyNormalPoolInstance () {
		return friendlyNormalPoolInstance;
	}

	public static SimpleBulletPool GetFriendlyFastPoolInstance () {
		return friendlyFastPoolInstance;
	}

	public static SimpleBulletPool GetEnemySlowPoolInstance () {
		return enemySlowPoolInstance;
	}

	public static SimpleBulletPool GetEnemyNormalPoolInstance () {
		return enemyNormalPoolInstance;
	}

	public static SimpleBulletPool GetEnemyFastPoolInstance () {
		return enemyFastPoolInstance;
	}

	void CheckAndSetInstance () {
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
