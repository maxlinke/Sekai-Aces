using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {

		[Header("Text Components")]
	[SerializeField] Text dodgeText;
	[SerializeField] Text livesText;
	[SerializeField] Text spwText;

		[Header("Image Components")]
	[SerializeField] Image dodgeBorder;
	[SerializeField] Image livesBorder;
	[SerializeField] Image livesImage;
	[SerializeField] Image spwBorder;
	[SerializeField] Image spwImage;

		[Header("Sprites")]
	[SerializeField] Sprite razorbackPlaneSprite;
	[SerializeField] Sprite razorbackSPWSprite;
	[SerializeField] Sprite waspPlaneSprite;
	[SerializeField] Sprite waspSPWSprite;
	[SerializeField] Sprite griffonPlaneSprite;
	[SerializeField] Sprite griffonSPWSprite;
	[SerializeField] Sprite spectrePlaneSprite;
	[SerializeField] Sprite spectreSPWSprite;

	[HideInInspector] public Color activatedColor;
	[HideInInspector] public Color deactivatedColor;

	public void Initialize (Player.PlaneType planeType) {
		switch(planeType){
		case Player.PlaneType.RAZORBACK:
			livesImage.sprite = razorbackPlaneSprite;
			spwImage.sprite = razorbackSPWSprite;
			break;
		case Player.PlaneType.WASP:
			livesImage.sprite = waspPlaneSprite;
			spwImage.sprite = waspSPWSprite;
			break;
		case Player.PlaneType.GRIFFON:
			livesImage.sprite = griffonPlaneSprite;
			spwImage.sprite = griffonSPWSprite;
			break;
		case Player.PlaneType.SPECTRE:
			livesImage.sprite = spectrePlaneSprite;
			spwImage.sprite = spectreSPWSprite;
			break;
		default:
			throw new UnityException("Unknown plane type \"" + planeType.ToString() + "\"");
		}
	}

	public void SetDodgeDisplayState(bool canDodge){
		if(canDodge){
			dodgeBorder.color = activatedColor;
			dodgeText.color = activatedColor;
		}else{
			dodgeBorder.color = deactivatedColor;
			dodgeText.color = deactivatedColor;
		}
	}

	public void SetLivesDisplayState(bool alive){
		if(alive){
			livesBorder.color = activatedColor;
			livesImage.color = activatedColor;
			livesText.color = activatedColor;
		}else{
			livesBorder.color = deactivatedColor;
			livesImage.color = deactivatedColor;
			livesText.color = deactivatedColor;
		}
	}

	public void SetSPWDisplayState(bool canFire){
		if(canFire){
			spwBorder.color = activatedColor;
			spwImage.color = activatedColor;
			spwText.color = activatedColor;
		}else{
			spwBorder.color = deactivatedColor;
			spwImage.color = deactivatedColor;
			spwText.color = deactivatedColor;
		}
	}

	public void SetLivesNumber(int number){
		livesText.text = "x " + number.ToString();
	}

	public void SetSPWAmmoNumber(int number){
		spwText.text = "x " + number.ToString();
	}

}
