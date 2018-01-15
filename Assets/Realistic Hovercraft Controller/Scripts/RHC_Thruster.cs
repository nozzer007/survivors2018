using UnityEngine;
using System.Collections;

public class RHC_Thruster : MonoBehaviour {

	private RHC_HovercraftController hoverController;
	private Rigidbody rigid;
	private float fuelInput = 1f;

	internal float springForce;
	internal float damperForce;
	internal float springConstant;
	internal float damperConstant;
	internal float restLenght;

	private float previouseLenght;
	private float currentLenght;
	private float springVelocity;
	private GameController gameController;



	void Awake()
	{

		hoverController = GetComponentInParent<RHC_HovercraftController> ();
		rigid = hoverController.GetComponent<Rigidbody> ();
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' in RHC_Thruster script");
		}

	}
		
	void FixedUpdate () {
		
		if (!hoverController || !rigid) {
			enabled = false;
			return;
		}
		if (gameController.getShipStatus () == "LANDED" || !hoverController.engineRunning)
			return;
		fuelInput = Mathf.Lerp (fuelInput, hoverController.engineRunning ? 1f : 0f, Time.fixedDeltaTime);

		springConstant = hoverController.stabilizerConstant;
		damperConstant = hoverController.stabilizerDamperConstant;
		restLenght = hoverController.stableHeight;

		
		RaycastHit hit;

		if(Physics.Raycast(transform.position, -hoverController.transform.up, out hit, restLenght + .5f)){

			if (hit.collider.tag == "LandingZone" || gameController.getShipStatus () == "LANDED" || hit.collider.tag == "Survivor") {
				//Debug.Log("Thruster HIT whilst landed returning-" + hit.collider.tag);
				return;
			}
			previouseLenght = currentLenght;
			currentLenght = restLenght - (hit.distance - .5f);
			springVelocity = (currentLenght - previouseLenght) / Time.fixedDeltaTime;

			//Debug.Log("S="+springForce + ":D="+damperForce+":F="+(fuelInput-hoverController.fuelRetard));

			//Has fuel flow been slowed down to land the ship, if yes then signifanctly eas dmaping to allow for landing.
			if (hoverController.fuelRetard != 0) {
				springForce = 0;
				damperForce = 0;
			} else {
				springForce = springConstant * currentLenght;
				damperForce = damperConstant * springVelocity;
			}

			rigid.AddForceAtPosition((hoverController.transform.up * (springForce + damperForce)) * (fuelInput-hoverController.fuelRetard), transform.position);
			//rigid.AddForceAtPosition((hoverController.transform.up * (springForce + damperForce)) * 0.04f, transform.position);

		}

	}

}