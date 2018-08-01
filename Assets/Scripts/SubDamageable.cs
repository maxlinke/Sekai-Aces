using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubDamageable : MonoBehaviour, IDamageable {

	//put this on colliders that are not part of the main damageable
	//as in the enemy has moving parts that can be shot but the enemy itself is managed somewhere else

	[SerializeField] GameObject mainDamageable;
	IDamageable damageable;

	void Start () {
		damageable = mainDamageable.GetComponent<IDamageable>();
	}

	public void CollisionDamage (int amount) {
		damageable.CollisionDamage(amount);
	}

	public void WeaponDamage (int amount) {
		damageable.WeaponDamage(amount);
	}
}
