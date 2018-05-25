using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable{

	void WeaponDamage(int amount);

	void CollisionDamage(int amount);

	void Kill(bool instantly);

}
