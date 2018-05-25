using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreMenu : MonoBehaviour {

		[Header("Components")]
	[SerializeField] Image fadeInScreen;
	[SerializeField] GameObject pressAnyKeyScreen;
	[SerializeField] GameObject mainMenu;

		[Header("Settings")]
	[SerializeField] float fadeInTime;
	[SerializeField] float fadeInDelay;

	void Awake(){
		fadeInScreen.gameObject.SetActive(true);
		pressAnyKeyScreen.SetActive(true);
		mainMenu.SetActive(false);
		StartCoroutine(FadeInUI());	
	}

	void Update(){
		if(Input.anyKeyDown){
			OpenMainMenuAndDisableSelf();
		}
	}

	IEnumerator FadeInUI(){
		Color c = fadeInScreen.color;
		fadeInScreen.color = new Color(c.r, c.g, c.b, 1f);
		yield return new WaitForSeconds(fadeInDelay);
		float endTime = Time.time + fadeInTime;
		while(Time.time < endTime){
			if(fadeInScreen.gameObject.activeSelf){
				float alpha = (endTime - Time.time ) / fadeInTime;
				fadeInScreen.color = new Color(c.r, c.g, c.b, alpha);
				yield return null;
			}else{
				yield break;
			}
		}
		fadeInScreen.gameObject.SetActive(false);
	}

	void OpenMainMenuAndDisableSelf(){
		mainMenu.SetActive(true);
		this.gameObject.SetActive(false);
	}

}
