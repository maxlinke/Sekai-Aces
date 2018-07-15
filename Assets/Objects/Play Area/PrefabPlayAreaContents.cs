using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPlayAreaContents : MonoBehaviour {

	[Header("Player Spawnpoints")]
	[SerializeField] GameObject playerSpawn;
	[SerializeField] GameObject playerRespawn;

	[Header("Camera")]
	[SerializeField] Camera gameplayCam;
	[SerializeField] GameObject gameplayCamContainer;

	[Header("Light")]
	[SerializeField] Light gameplayLight;

	[Header("Camera Positions")]
	[SerializeField] GameObject topPosition;
	[SerializeField] GameObject sidePosition;
	[SerializeField] GameObject backPosition;

	[Header("Actual Play Areas")]
	[SerializeField] GameObject topArea;
	[SerializeField] GameObject sideArea;
	[SerializeField] GameObject backArea;

	public GameObject PlayerSpawn { get { return playerSpawn; } }
	public GameObject PlayerRespawn { get { return playerRespawn; } }

	public Camera GameplayCam { get { return gameplayCam; } }
	public GameObject GameplayCamContainer { get { return gameplayCamContainer; } }

	public Light GameplayLight { get { return gameplayLight; } }

	public GameObject TopPosition { get { return topPosition; } }
	public GameObject SidePosition { get { return sidePosition; } }
	public GameObject BackPosition { get { return backPosition; } }

	public GameObject TopArea { get { return topArea; } }
	public GameObject SideArea { get { return sideArea; } }
	public GameObject BackArea { get { return backArea; } }

}
