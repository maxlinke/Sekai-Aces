using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

	[SerializeField] Light mainLight;
	[SerializeField] TrackFollower trackFollower;

	public Light LevelLight { get { return mainLight; } }
	public GameObject LevelCamContainer { get { return trackFollower.CamContainer; } }
	public Camera LevelCam { get { return trackFollower.Cam; } }

}
