using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSelectRenderer : MonoBehaviour {

		[Header("Components")]
	[SerializeField] GameObject container;
	[SerializeField] MeshRenderer arrowheadMR;
	[SerializeField] MeshRenderer griffonMR;
	[SerializeField] MeshRenderer razorbackMR;
	[SerializeField] MeshRenderer spectreMR;
	[SerializeField] MeshRenderer waspMR;

		[Header("Materials")]
	[SerializeField] Material normalMaterial;
	[SerializeField] Material lockedMaterial;

		[Header("Settings")]
	[SerializeField] float spinTime;

	void Start(){
		HideAll();
	}
	
	void Update(){
		float rotation = 360f * Time.time / spinTime;
		container.transform.localEulerAngles = new Vector3(0f, rotation, 0f);
	}

	public void ShowPlane(Player.PlaneType planeType, bool bright){
		HideAll();
		MeshRenderer planeMR;
		switch(planeType){
		case Player.PlaneType.ARROWHEAD:
			planeMR = arrowheadMR;
			break;
		case Player.PlaneType.GRIFFON:
			planeMR = griffonMR;
			break;
		case Player.PlaneType.RAZORBACK:
			planeMR = razorbackMR;
			break;
		case Player.PlaneType.SPECTRE:
			planeMR = spectreMR;
			break;
		case Player.PlaneType.WASP:
			planeMR = waspMR;
			break;
		default:
			throw new UnityException("Unknown plane type (" + planeType.ToString() + ")");
		}
		planeMR.enabled = true;
		SetMaterial(planeMR, bright);
	}

	void HideAll(){
		arrowheadMR.enabled = false;
		griffonMR.enabled = false;
		razorbackMR.enabled = false;
		spectreMR.enabled = false;
		waspMR.enabled = false;
	}

	void SetMaterial(MeshRenderer mr, bool bright){
		if(bright) mr.material = normalMaterial;
		else mr.material = lockedMaterial;
	}

}
