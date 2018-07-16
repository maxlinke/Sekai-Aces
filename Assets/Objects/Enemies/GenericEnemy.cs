using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEnemy : MonoBehaviour {

	[SerializeField] protected Rigidbody rb;

	public abstract void Initialize (Player[] players, GameplayMode mode);

	public abstract void LevelReset ();

	protected void SetRBTranslationConstraints (GameplayMode mode) {
		RigidbodyConstraints rotationConstraints = rb.constraints & RigidbodyConstraints.FreezeRotation;
		RigidbodyConstraints positionConstraints;
		switch(mode){
		case GameplayMode.TOPDOWN:
			positionConstraints = RigidbodyConstraints.FreezePositionY;
			break;
		case GameplayMode.SIDE:
			positionConstraints = RigidbodyConstraints.FreezePositionX;
			break;
		default:
			throw new UnityException("unknown mode " + mode.ToString());
		}
		rb.constraints = positionConstraints | rotationConstraints;
	}

}
