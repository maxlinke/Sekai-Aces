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
	[SerializeField] Vector3 bulletVector;
	[SerializeField] int bulletDamage;

	List<GameObject> activeBullets;
	List<GameObject> inactiveBullets;
	List<GameObject> returningBullets;

	int bulletCount;

	float oobX;
	float oobZ;

	enum BulletPoolType{
		FRIENDLY_NORMAL, FRIENDLY_FAST, ENEMY_NORMAL
	}

	void Start(){
		bulletCount = 0;
		CheckInstance();
		SetInstance();
		inactiveBullets = new List<GameObject>();
		activeBullets = new List<GameObject>();
		returningBullets = new List<GameObject>();
		oobX = Mathf.Abs(PlayAreaScript.GetDimensions().x / 2f) + 1f;
		oobZ = Mathf.Abs(PlayAreaScript.GetDimensions().z / 2f) + 1f;
	}
	
	void Update(){
		for(int i=0; i<activeBullets.Count; i++){
			if(Mathf.Abs(activeBullets[i].transform.localPosition.z) > oobZ || Mathf.Abs(activeBullets[i].transform.localPosition.x) > oobX){
				returningBullets.Add(activeBullets[i]);
			}else{
				activeBullets[i].transform.localPosition += bulletVector * Time.deltaTime;
			}
		}
		for(int i=0; i<returningBullets.Count; i++){
			ReturnToInactivePool(returningBullets[i]);
		}
		returningBullets.Clear();
	}

	void FixedUpdate(){
		
	}

	void OnCollisionEnter(Collision collision){
		GameObject bullet = collision.contacts[0].thisCollider.gameObject;
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.WeaponDamage(bulletDamage);
		}
		ReturnToInactivePool(bullet);
	}

	public void NewBullet(Vector3 position){
		GameObject bullet;
		if(!TryTakeBulletFromInactivePool(out bullet)){
			bulletCount++;
			bullet = Instantiate(bulletPrefab) as GameObject;
			bullet.transform.parent = this.transform;
			bullet.name = "bullet " + bulletCount;
		}
		bullet.SetActive(true);
		bullet.transform.localPosition = transform.InverseTransformPoint(position);
		bullet.transform.localRotation = Quaternion.identity;
		activeBullets.Add(bullet);
	}

	bool TryTakeBulletFromInactivePool(out GameObject bullet){
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

	void ReturnToInactivePool(GameObject bullet){
		if(bullet.activeSelf){
			bullet.SetActive(false);
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
		case BulletPoolType.ENEMY_NORMAL:
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
		case BulletPoolType.ENEMY_NORMAL:
			enemyNormalPoolInstance = this;
			break;
		default:
			throw new UnityException("unknown bullet pool type");
		}
	}

}
