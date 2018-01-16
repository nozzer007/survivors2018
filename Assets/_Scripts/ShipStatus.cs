using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStatus : MonoBehaviour {

	private GameController gameController;

	public bool landed = true;
	public bool inLandingZone = true;
	public string inZoneType;

	public bool LandingPressed = false;
	int lastLandedIndex;
	public bool fireLaser = false;


	void Awake()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' in ShipStatus script");
		}
	}

	public string getShipStatus()
	{
		string retStatus = returnShipStatus ();

		if (retStatus != "FLYING")
		{
			gameController.setCameraZoom (4, 15);
		}
		else
		{
			gameController.setCameraZoom (2.5f, 15);
		}
		return retStatus;
	}

	public string returnShipStatus()
	{
		//I've pressed the Action button triggered in RHC_HoverController
		//Im in the landing zone and landed
		if (landed && inLandingZone) {
			fireLaser = false;
			return "LANDED";
		}
		//Im in the landing zone but not landed
		if (!landed && inLandingZone) {
			fireLaser = false;
			return "READYTOLAND";
		}
		if (inZoneType == "FINAL") {
			return "LEVELCOMPLETE";
		}
		if (!inLandingZone) {
			fireLaser = true;
			return "FLYING";
		}
		return "FLYING";
	}

	public void UpdateFlightStatus(bool _landed,bool _inLandingzone,string _inZoneType)
	{
		landed = _landed;
		inLandingZone = _inLandingzone;
		inZoneType = _inZoneType;
	}
		
}
