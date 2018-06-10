using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletPool : MonoBehaviour {

	static SimpleBulletPool friendlyNormalPoolInstance;
	static SimpleBulletPool friendlyFastPoolInstance;
	static SimpleBulletPool enemyNormalPoolInstance;

	[SerializeField] BulletPoolType type;

		[Header("Prefabs")]
	[SerializeField] GameObject bulletPrefab;

		[Header("Settings")]
	[SerializeField] float bulletSpeed;
	[SerializeField] int bulletDamage;
	[SerializeField] float bulletRange;

	bool initialized;
	float sqrBulletRange;
	int bulletCount;

	List<Rigidbody> activeBullets;
	List<Rigidbody> inactiveBullets;
	List<Rigidbody> returningBullets;

	enum BulletPoolType{
		FRIENDLY_NORMAL, FRIENDLY_FAST, ENEMY_SLOW
	}

	void Start(){
		bulletCount = 0;
		sqrBulletRange = bulletRange * bulletRange;
		CheckInstance();
		SetInstance();
		inactiveBullets = new List<Rigidbody>();
		activeBullets = new List<Rigidbody>();
		returningBullets = new List<Rigidbody>();
		initialized = true;
	}
	
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

	public void LevelReset(){
		if(initialized){
			for(int i=0; i<activeBullets.Count; i++){
				returningBullets.Add(activeBullets[i]);
			}
			for(int i=0; i<returningBullets.Count; i++){
				ReturnToInactivePool(returningBullets[i]);
			}
			returningBullets.Clear();
		}
	}

	public void NewBullet(Vector3 position, Vector3 direction){
		direction = direction.normalized;
		Debug.DrawRay(position, direction, Color.red, 0f, false);
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
		bullet.transform.localRotation = Quaternion.identity;
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

	public static SimpleBulletPool GetEnemyNormalPoolInstance(){
		return enemyNormalPoolInstance;
	}

	void CheckInstance(){
		switch(type){
		case BulletPoolType.FRIENDLY_NORMAL: 
			if(friendlyNormalPoolInstance != null) throw new UnityException("normal bulletpool instance is not null (singleton...)");
			break;
		case BulletPoolType.FRIENDLY_FAST:
			if(friendlyFastPoolInstance != null) throw new UnityException("normal bulletpool instance is not null (singleton...)");
			break;
		case BulletPoolType.ENEMY_SLOW:
			if(enemyNormalPoolInstance != null) throw new UnityException("normal bulletpool instance is not null (singleton...)");
			break;
		default:
			throw new UnityException("unknown bullet pool type");
		}
	}

	void SetInstance(){
		switch(type){
		case BulletPoolType.FRIENDLY_NORMAL: 
			friendlyNormalPoolInstance = this;
			break;
		case BulletPoolType.FRIENDLY_FAST:
			friendlyFastPoolInstance = this;
			break;
		case BulletPoolType.ENEMY_SLOW:
			enemyNormalPoolInstance = this;
			break;
		default:
			throw new UnityException("unknown bullet pool type");
		}
	}

}
