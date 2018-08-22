using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenericEnemy : GenericEnemy {

	[SerializeField] TestGenericEnemyMovement movement;

	public override void Initialize (Player[] players, GameplayMode gameplayMode, PlayArea playArea) {
		base.Initialize (players, gameplayMode, playArea);
		movement.Initialize(players, gameplayMode, playArea);
	}

	public override void LevelReset () {
		base.LevelReset ();
		movement.LevelReset();
	}

	public override void Disappear () {
		movement.StartDisappearing();
	}

}
