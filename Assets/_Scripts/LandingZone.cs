using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingZone : MonoBehaviour {

	private GameController gameController;
	public string ZoneName;
	public string ZoneType = "EmptyZone";
	public int LandingZoneIndex;

	void Awake()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' in ShipGunGeneric script");
		}
	}

	// Update is called once per frame
	void Update () {


	}

	void OnTriggerEnter(Collider col)
	{
		//hidden
		if (col.transform.gameObject.name == "Hovercraft") {

			//landed,inLandingzone
			gameController.UpdateFlightStatus(false,true,ZoneType);
			gameController.LandingPressed = false;
			gameController.SetLandingZoneIndex (LandingZoneIndex);
		}

	}

	void OnTriggerExit(Collider col)
	{
		//hidden
		if (col.transform.gameObject.name == "Hovercraft") {

			//landed,inLandingzone
			//col.gameObject.GetComponent<RHC_HovercraftController>().RestoreFuel();
			gameController.UpdateFlightStatus(false,false,ZoneType);
			gameController.LandingPressed = false;

		}
	}
}
