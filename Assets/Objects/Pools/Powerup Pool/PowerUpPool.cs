using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPool : RigidbodyPool {

	[SerializeField] GameObject powerupPrefab;

	int powerUpCount;

	void Update () {
		base.SqrDistReturnCheck();
		if(Input.GetKeyDown(KeyCode.I)) NewPowerUp(this.transform.position + Vector3.forward * 3f, PowerUp.PowerUpType.WEAPON);
		if(Input.GetKeyDown(KeyCode.O)) NewPowerUp(this.transform.position + Vector3.forward * 3f, PowerUp.PowerUpType.REPAIR);
		if(Input.GetKeyDown(KeyCode.P)) NewPowerUp(this.transform.position + Vector3.forward * 3f, PowerUp.PowerUpType.RELOAD);
	}

	public override void Initialize () {
		base.Initialize();
		powerUpCount = 0;
	}

	public void NewPowerUp (Vector3 position, PowerUp.PowerUpType type) {
		Rigidbody powerUpRB;
		if(!TryTakeRBFromInactivePool(out powerUpRB)){
			powerUpCount++;
			GameObject powerUpObject = Instantiate(powerupPrefab) as GameObject;
			powerUpObject.transform.parent = this.transform;
			powerUpObject.name = "powerup " + powerUpCount;
			powerUpRB = powerUpObject.GetComponent<Rigidbody>();
		}
		PowerUp powerUp = powerUpRB.gameObject.GetComponent<PowerUp>();
		powerUp.Initialize(type, this);

		powerUpRB.gameObject.SetActive(true);
		powerUpRB.transform.position = position;
//		powerUpRB.transform.localRotation = Quaternion.LookRotation(direction);
//		powerUpRB.velocity = direction * bulletSpeed;
		activeRBs.Add(powerUpRB);
	}

}
