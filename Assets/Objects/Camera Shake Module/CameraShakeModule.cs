using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeModule : MonoBehaviour, IPlayerPrefSettingsObserver {

	[SerializeField] GameObject[] shakeObjects;

	static CameraShakeModule instance;

	Vector3 shakeOffset;
	float universalShakeMultiplier;

	void Start () {
		if(instance != null) throw new UnityException("Camera Shaker Instance is not null (Singleton violation)");
		instance = this;
		AddSelfToPlayerPrefObserverList();
		ReloadSettings();
	}
	
	void Update () {
		for(int i=0; i<shakeObjects.Length; i++){
			shakeObjects[i].transform.localPosition = shakeOffset;
		}
		shakeOffset = Vector3.zero;
	}

	IEnumerator ShakeCoroutine (float strength, float duration, float speed) {
		float shakeStrength = strength;
		float shakeDuration = duration;
		float shakeSpeed = speed;
		float shakeStart = Time.time;
		float shakeEnd = Time.time + duration;
		while(Time.time <= shakeEnd){
			float perlinInput = Time.time * shakeSpeed;
			float perlinX = Mathf.PerlinNoise(perlinInput, 0f);
			float perlinY = Mathf.PerlinNoise(0f, perlinInput);
			if(float.IsNaN(perlinX)) perlinX = 0f;
			if(float.IsNaN(perlinY)) perlinY = 0f;
			float progress = Mathf.Clamp01((Time.time - shakeStart) / shakeDuration);
			shakeOffset += new Vector3(perlinX, perlinY) * (1f - progress) * shakeStrength * universalShakeMultiplier;
			yield return null;
		}
	}

	public static void Shake (float strength, float duration, float speed) {
		instance.StartShakeCoroutine(strength, duration, speed);
	}

	void StartShakeCoroutine (float strength, float duration, float speed) {
		StartCoroutine(ShakeCoroutine(strength, duration, speed));
	}

	public void StopAllShaking () {
		StopAllCoroutines();
	}

	public void AddSelfToPlayerPrefObserverList () {
		PlayerPrefManager.AddObserver(this);
	}

	public void NotifyPlayerSettingsChanged () {
		ReloadSettings();
	}

	void ReloadSettings () {
		universalShakeMultiplier = PlayerPrefManager.GetFloat("game_screenshakemultiplier");
	}

}
