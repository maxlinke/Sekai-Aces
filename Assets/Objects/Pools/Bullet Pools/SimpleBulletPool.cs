using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletPool : RigidbodyPool {

	static SortedList<BulletPoolType, SimpleBulletPool> map;

	[SerializeField] BulletPoolType type;

		[Header("Prefabs")]
	[SerializeField] GameObject bulletPrefab;

		[Header("Settings")]
	[SerializeField] float bulletSpeed;
	[SerializeField] int bulletDamage;

	int bulletCount;

	public float BulletSpeed { get { return bulletSpeed; } }

	public enum BulletPoolType {
		FRIENDLY_NORMAL, FRIENDLY_FAST, ENEMY_SLOW, ENEMY_NORMAL, ENEMY_FAST
	}

	static SimpleBulletPool () {
		map = new SortedList<BulletPoolType, SimpleBulletPool>();
	}

	void Update () {
		base.SqrDistReturnCheck();
	}

	void OnDestroy () {
		map.Remove(this.type);
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
			bulletScript.effectPool = GetProperEffectPool();
			bullet = bulletObject.GetComponent<Rigidbody>();
		}
		bullet.gameObject.SetActive(true);
		bullet.transform.position = position;
		bullet.transform.localRotation = Quaternion.LookRotation(direction);
		bullet.velocity = direction * bulletSpeed;
		activeRBs.Add(bullet);
	}

	public static SimpleBulletPool GetPool(BulletPoolType type){
		SimpleBulletPool output;
		if(map.TryGetValue(type, out output)){
			return output;
		}else{
			throw new UnityException("No pool in the map for type \"" + type.ToString() + "\". Maybe it wasn't instantiated?");
		}
	}

	void CheckAndSetInstance () {
		if(!map.ContainsKey(this.type)){
			map.Add(this.type, this);
		}else{
			throw new UnityException("There is already a pool in the map for type \"" + this.type.ToString() + "\" (Singleton violation)");
		}
	}

	ParticleEffectPool GetProperEffectPool () {
		switch(type){
		case BulletPoolType.ENEMY_SLOW: return ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.BULLETHIT_ENEMY);
		case BulletPoolType.ENEMY_NORMAL: return ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.BULLETHIT_ENEMY);
		case BulletPoolType.ENEMY_FAST: return ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.BULLETHIT_ENEMY);
		case BulletPoolType.FRIENDLY_NORMAL: return ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.BULLETHIT_FRIENDLY);
		case BulletPoolType.FRIENDLY_FAST: return ParticleEffectPool.GetPool(ParticleEffectPool.EffectType.BULLETHIT_FRIENDLY);
		default: throw new UnityException("unsupported bullettype \"" + type.ToString() + "\"");
		}
	}

}
