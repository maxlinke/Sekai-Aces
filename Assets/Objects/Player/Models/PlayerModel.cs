using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour {

	[SerializeField] MeshRenderer normalMR;
	[SerializeField] MeshRenderer shinyMR;
	[SerializeField] Material normalMat;
	[SerializeField] Material transparentMat;
	MaterialPropertyBlock shinyPropBlock;

	Color shineColor;
	float shineIntensity;
	bool isBlinking;
	bool transparentBlink;
	float nextBlink;

	const float blinkInterval = 0.1f;
	const float shineFalloff = 4f;

	void Start(){
		shinyPropBlock = new MaterialPropertyBlock();
		Reset();
	}

	void Update(){
		shinyPropBlock.SetColor("_Color", shineColor);
		shinyPropBlock.SetFloat("_Intensity", shineIntensity);
		shinyPropBlock.SetVector("_DotVector", Vector4.Lerp(new Vector4(0,1,1,0), new Vector4(0,1,-1,0), shineIntensity));
		shinyMR.SetPropertyBlock(shinyPropBlock);
		ShineManager();
		BlinkManager();
	}

	public void Hide(){
		SetMeshRenderersActive(false);
	}

	public void Unhide(){
		SetMeshRenderersActive(true);
	}

	void SetMeshRenderersActive(bool value){
		normalMR.enabled = value;
		shinyMR.enabled = value;
	}

	public void Reset(){
		Unhide();
		normalMR.material = normalMat;
		transform.localEulerAngles = Vector3.zero;
		shineColor = new Color(0,0,0,1);
		shineIntensity = 0f;
		isBlinking = false;
	}

	public void SetLocalEulerAngles(Vector3 localEulerAngles){
		transform.localEulerAngles = localEulerAngles;
	}

	public void SetBlinking(bool blink){
		if(blink){
			normalMR.material = transparentMat;
			transparentBlink = true;
			nextBlink = Time.time + blinkInterval;
		}else{
			normalMR.material = normalMat;
		}
		isBlinking = blink;
	}

	public void Shine(Color color){
		shineColor = color;
		shineIntensity = 1f;
	}

	void ShineManager(){
		if(shineIntensity > 0f){
			shineIntensity -= Time.deltaTime * shineFalloff;
			if(shineIntensity < 0f) shineIntensity = 0f;
		}
	}

	void BlinkManager(){
		if(isBlinking){
			if(Time.time >= nextBlink){
				transparentBlink = !transparentBlink;
				if(transparentBlink){
					normalMR.material = transparentMat;
				}else{
					normalMR.material = normalMat;
				}nextBlink = Time.time + blinkInterval;
			}
		}
	}

}
