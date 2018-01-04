using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingZoneExit : MonoBehaviour {

	private GameController gameController;
	public string ZoneName;

	void Awake()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' in LandongZone Exit script");
		}
	}

	// Update is called once per frame
	void Update () {


	}

	void OnTriggerEnter(Collider col)
	{
		//if this is a landing zone entry, update the flight status
		if (col.transform.gameObject.name == "Hovercraft")
		{
			//landed,inLandingzone
			if (gameController.getShipStatus () == "LANDED")
				return;
			gameController.UpdateFlightStatus(true,false,"EXIT");

			gameController.LandingPressed = true;
		}

	}

	void OnTriggerExit(Collider col)
	{

		//hidden
		if (col.transform.gameObject.name == "Hovercraft") {

			//landed,inLandingzone
			gameController.UpdateFlightStatus(false,false,"EXIT");
			gameController.LandingPressed = false;

		}
	}
}

