using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEnemy : MonoBehaviour {

	public abstract void Initialize (Player[] players, GameplayMode mode);

	public abstract void LevelReset ();

}
