using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour {

	[SerializeField] GameObject playerSpawn;
	[SerializeField] GameObject playerRespawn;

		[Header("Cameras")]
	[SerializeField] Camera gameplayCam;
	[SerializeField] Camera levelCamera;

		[Header("Lights")]
	[SerializeField] Light gameplayLight;
	[SerializeField] Light levelLight;

		[Header("Camera Positions")]
	[SerializeField] GameObject topPosition;
	[SerializeField] GameObject sidePosition;
	[SerializeField] GameObject backPosition;

		[Header("Actual Play Areas")]
	[SerializeField] GameObject topArea;
	[SerializeField] GameObject sideArea;
	[SerializeField] GameObject backArea;

	GameplayMode currentMode;

	/*
		ABSOLUTELY NO STARTCOROUTINE CALLS IN HERE; THEY ARE ALL TO BE MADE BY THE GAMECONTROLLER!!!
	*/

	void Start () {
		levelCamera.fieldOfView = gameplayCam.fieldOfView;
	}
	
	void Update () {
		
	}

	void LateUpdate(){
		Vector3 lvlCamToLight = levelLight.transform.position - levelCamera.transform.position;
		Vector3 lvlLightDir = levelLight.transform.forward;
		Vector3 transformedLvlCamtoLight = levelCamera.transform.InverseTransformDirection(lvlCamToLight);
		Vector3 transformedLvlLightDir = levelCamera.transform.InverseTransformDirection(lvlLightDir);
		Vector3 retransformedCamToLight = gameplayCam.transform.TransformDirection(transformedLvlCamtoLight);
		Vector3 retransformedLightDir = gameplayCam.transform.TransformDirection(transformedLvlLightDir);
		gameplayLight.transform.position = gameplayCam.transform.position + retransformedCamToLight;
		gameplayLight.transform.forward = retransformedLightDir;
	}

	public void SetMode(GameplayMode newMode){
		this.currentMode = newMode;
	}

	public void SetAreaToMode(GameplayMode newMode){
		DeactivateAllPlayAreas();
		GetCorrespondingPlayArea(newMode).SetActive(true);
	}

	public void SetCamsToMode(GameplayMode newMode){
		GameObject camPoint = GetCorrespondingCamPoint(newMode);
		gameplayCam.transform.localPosition = camPoint.transform.localPosition;
		gameplayCam.transform.localRotation = camPoint.transform.localRotation;
		SyncLevelCamera();
	}

	public void PutPlayerObjectToSpawnPoint(GameObject playerObject, float offset){
		Vector3 offsetVec = GetSpawnOffsetVec(currentMode) * offset;
		playerObject.transform.localPosition = FlattenLocalPointToArea(currentMode, playerSpawn.transform.localPosition + offsetVec);
	}

	public IEnumerator TransitionPlayerToMode(GameObject playerObject, GameplayMode newMode, float transitionDuration){
		Vector3 startPos = playerObject.transform.localPosition;
		Quaternion startRot = playerObject.transform.localRotation;
		Vector3 normalizedTargetPos = GetNormalizedTargetPoint(startPos, currentMode, newMode);
		Vector3 endPos = DenormalizePoint(normalizedTargetPos, newMode);
		Quaternion endRot = Quaternion.identity;
		float progress = 0f;
		while(progress < 1f){
			progress += (Time.deltaTime / transitionDuration);
			playerObject.transform.localPosition = Vector3.Lerp(startPos, endPos, progress);
			playerObject.transform.localRotation = Quaternion.Lerp(startRot, endRot, progress);
			yield return null;
		}
		playerObject.transform.localPosition = endPos;
		playerObject.transform.localRotation = endRot;
	}

	public IEnumerator TransitionPlayerWhileRespawning(GameObject playerObject, float offset, float respawnDuration){
		Vector3 offsetVec = GetSpawnOffsetVec(currentMode) * offset;
		Vector3 startPos = playerRespawn.transform.localPosition + offsetVec;
		Vector3 endPos = FlattenLocalPointToArea(currentMode, playerSpawn.transform.localPosition + offsetVec);
		float progress = 0f;
		while(progress < 1f){
			playerObject.transform.localPosition = Vector3.Lerp(startPos, endPos, progress);
			progress += (Time.deltaTime / respawnDuration);
			yield return null;
		}
		playerObject.transform.localPosition = endPos;
	}

	public IEnumerator TransitionCamerasToMode(GameplayMode newMode, float transitionDuration){
		Vector3 startPos = gameplayCam.transform.localPosition;
		Quaternion startRot = gameplayCam.transform.localRotation;
		GameObject newPoint = GetCorrespondingCamPoint(newMode);
		Vector3 endPos = newPoint.transform.localPosition;
		Quaternion endRot = newPoint.transform.localRotation;
		float progress = 0f;
		while(progress < 1f){
			progress += (Time.deltaTime / transitionDuration);
			gameplayCam.transform.localPosition = Vector3.Lerp(startPos, endPos, progress);
			gameplayCam.transform.localRotation = Quaternion.Lerp(startRot, endRot, progress);
			SyncLevelCamera();
			yield return null;
		}
		gameplayCam.transform.localPosition = endPos;
		gameplayCam.transform.localRotation = endRot;
		SyncLevelCamera();
	}

	Vector3 GetNormalizedTargetPoint(Vector3 origPoint, GameplayMode origMode, GameplayMode targetMode){
		Vector3 origAreaScale = GetCorrespondingPlayArea(origMode).transform.localScale;
		float normX, normY, normZ;
		switch(origMode){
		case GameplayMode.TOPDOWN:
			normX = origPoint.x / origAreaScale.x;
			normZ = origPoint.z / origAreaScale.z;
			if(targetMode.Equals(GameplayMode.TOPDOWN)) return new Vector3(normX, 0f, normZ);
			else if(targetMode.Equals(GameplayMode.SIDE)) return new Vector3(0f, normX, normZ);
			else if(targetMode.Equals(GameplayMode.BACK)) return new Vector3(normX, normZ, 0f);
			else throw GetUnknownModeException(targetMode);
		case GameplayMode.SIDE:
			normY = origPoint.y / origAreaScale.y;
			normZ = origPoint.z / origAreaScale.z;
			if(targetMode.Equals(GameplayMode.TOPDOWN)) return new Vector3(normY, 0f, normZ);
			else if(targetMode.Equals(GameplayMode.SIDE)) return new Vector3(0f, normY, normZ);
			else if(targetMode.Equals(GameplayMode.BACK)) return new Vector3(normZ, normY, 0f);
			else throw GetUnknownModeException(targetMode);
		case GameplayMode.BACK:
			normX = origPoint.x / origAreaScale.x;
			normY = origPoint.y / origAreaScale.y;
			if(targetMode.Equals(GameplayMode.TOPDOWN)) return new Vector3(normX, 0f, normY);
			else if(targetMode.Equals(GameplayMode.SIDE)) return new Vector3(0f, normY, normX);
			else if(targetMode.Equals(GameplayMode.BACK)) return new Vector3(normX, normY, 0f);
			else throw GetUnknownModeException(targetMode);
		default:
			throw GetUnknownModeException(origMode);
		}
	}

	Vector3 DenormalizePoint(Vector3 normedPoint, GameplayMode targetMode){
		Vector3 targetAreaScale = GetCorrespondingPlayArea(targetMode).transform.localScale;
		return Vector3.Scale(normedPoint, targetAreaScale);
	}

	void SyncLevelCamera(){
		levelCamera.transform.localPosition = gameplayCam.transform.localPosition;
		levelCamera.transform.localRotation = gameplayCam.transform.localRotation;
	}

	void DeactivateAllPlayAreas(){
		topArea.SetActive(false);
		sideArea.SetActive(false);
		backArea.SetActive(false);
	}

	GameObject GetCorrespondingPlayArea(GameplayMode mode){
		switch(mode){
		case(GameplayMode.TOPDOWN):
			return topArea;
		case(GameplayMode.SIDE):
			return sideArea;
		case(GameplayMode.BACK):
			return backArea;
		default:
			throw GetUnknownModeException(mode);
		}
	}

	GameObject GetCorrespondingCamPoint(GameplayMode mode){
		switch(mode){
		case(GameplayMode.TOPDOWN):
			return topPosition;
		case(GameplayMode.SIDE):
			return sidePosition;
		case(GameplayMode.BACK):
			return backPosition;
		default:
			throw GetUnknownModeException(mode);
		}
	}

	Vector3 GetSpawnOffsetVec(GameplayMode mode){
		switch(currentMode){
		case GameplayMode.TOPDOWN:
			return Vector3.left;
		case GameplayMode.SIDE:
			return Vector3.up;
		case GameplayMode.BACK:
			return Vector3.left;
		default:
			throw GetUnknownModeException(currentMode);
		}
	}

	Vector3 FlattenLocalPointToArea(GameplayMode mode, Vector3 point){
		switch(currentMode){
		case GameplayMode.TOPDOWN:
			return new Vector3(point.x, 0f, point.z);
		case GameplayMode.SIDE:
			return new Vector3(0f, point.y, point.z);
		case GameplayMode.BACK:
			return new Vector3(point.x, point.y, 0f);
		default:
			throw GetUnknownModeException(currentMode);
		}
	}

	UnityException GetUnknownModeException(GameplayMode mode){
		return new UnityException("Unknown GameplayMode \"" + mode.ToString() + "\"");
	}

}
