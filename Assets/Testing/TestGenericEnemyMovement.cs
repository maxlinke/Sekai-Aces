using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenericEnemyMovement : GenericEnemyMovement {

	[SerializeField] float oscillationSpeed;
	[SerializeField] float oscillationStrength;

	bool disappearing;

	public override void LevelReset () {
		base.LevelReset();
		rb.constraints = RigidbodyConstraints.FreezeRotation;
		SetTranslationConstraintsFromMode(gameplayMode);
		disappearing = false;
	}

	public void StartDisappearing () {
		disappearing = true;
		maxVelocity = 10000f;
		maxAcceleration = 8f;
	}

	void FixedUpdate () {
		if (disappearing) {
			Vector3 direction = GetDisappearDirection ();
			Debug.DrawRay(transform.position, direction * 2f, Color.red, 0f, false);
			MoveInDirection(direction);
		} else {
			float phase = oscillationSpeed * 2f * Mathf.PI * Time.time;
			Vector3 offset = new Vector3(Mathf.Sin(phase), Mathf.Cos(phase), 0f);
			offset *= oscillationStrength;
			Vector3 direction = Vector3.back + offset;
			MoveInDirection(direction);
		}
	}

	Vector3 GetDisappearDirection () {
		Vector3 delta = transform.position - playArea.transform.position;
		switch(gameplayMode){
		case GameplayMode.TOPDOWN:
			return new Vector3(Mathf.Sign(delta.x), 0f, 0f);
		case GameplayMode.SIDE:
			return new Vector3(0f, Mathf.Sign(delta.y), 0f);
		case GameplayMode.BACK:
			return new Vector3(Mathf.Sign(delta.x), 0f, 0f);
		default:
			throw new UnityException("Unknown GameplayMode \"" + gameplayMode.ToString() + "\"");
		}
	}

}
