using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenericEnemy : GenericEnemy {

	[SerializeField] TestGenericEnemyMovement movement;
	[SerializeField] SimpleEnemyBulletWeapon weapon;
	[SerializeField] KeyCode debugShootKey;

	public override void Initialize (Player[] players, GameplayMode gameplayMode, PlayArea playArea) {
		base.Initialize (players, gameplayMode, playArea);
		movement.Initialize(players, gameplayMode, playArea);
		if(weapon != null) weapon.Initialize(players, gameplayMode, playArea);
	}

	public override void LevelReset () {
		base.LevelReset ();
		movement.LevelReset();
		if(weapon != null) weapon.LevelReset();
	}

	public override void Disappear () {
		movement.StartDisappearing();
	}

	void Update () {
		if(Input.GetKeyDown(debugShootKey)){
			if(weapon != null) weapon.Shoot();
		}
	}

}
