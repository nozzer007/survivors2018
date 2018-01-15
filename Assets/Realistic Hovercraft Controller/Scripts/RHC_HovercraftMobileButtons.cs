//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RHC_HovercraftMobileButtons : MonoBehaviour {

	private RHC_HovercraftController[] hoverControllers;

	public RHC_HovercraftUIButton accelerateButton;
	//public RHC_HovercraftUIButton reverseButton;
	public RHC_HovercraftUIButton leftButton;
	public RHC_HovercraftUIButton rightButton;
	public RHC_HovercraftUIButton actionButton;
	public RHC_HovercraftUIButton laserButton;

	internal float gasInput = 0f;
	internal float actionInput = 0f;
	internal float laserInput = 0f;
	internal float steerInput = 0f;

	public Image actionComponentImage;

	public Sprite FLYING;
	public Sprite LANDED;
	public Sprite READYTOLAND;

	GameController gameController;

	string actionImage;

	void Awake()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' in RHC_HoverMobileButtons script");
		}
	}
	void Start(){

		UpdateActionImage ();

		hoverControllers = GameObject.FindObjectsOfType<RHC_HovercraftController> ();

	}

	void UpdateActionImage()
	{
		actionImage = gameController.getShipStatus ();

	//	if (actionImage == "FLYING")
	//		actionComponentImage.sprite = FLYING;	
		if(actionImage=="LANDED")
			actionComponentImage.sprite = LANDED;
		if(actionImage=="FLYING")
			actionComponentImage.sprite = READYTOLAND;

}
	void OnEnable(){

		RHC_HovercraftController.OnRHCSpawned += RHCHovercraftController_OnRHCSpawned;

	}

	void RHCHovercraftController_OnRHCSpawned (){

		hoverControllers = GameObject.FindObjectsOfType<RHC_HovercraftController> ();
		
	}

	void Update(){

		//gasInput = accelerateButton.input + (-reverseButton.input);
		gasInput = accelerateButton.input;
		actionInput = actionButton.input;
		laserInput = laserButton.input;

		steerInput = -leftButton.input + rightButton.input;

		for (int i = 0; i < hoverControllers.Length; i++) {

			if ((hoverControllers [i]._controllerType == RHC_HovercraftController.ControllerType.MobileNGUIController || hoverControllers [i]._controllerType == RHC_HovercraftController.ControllerType.MobileUIController) && hoverControllers [i].canControl) {
				hoverControllers [i].gasInput = gasInput;
				hoverControllers [i].steerInput = steerInput;
				hoverControllers [i].actionInput = actionInput;
				hoverControllers [i].laserInput = laserInput;
			}

		}
		UpdateActionImage ();
	}

	void OnDisable(){

		RHC_HovercraftController.OnRHCSpawned -= RHCHovercraftController_OnRHCSpawned;

	}

}
