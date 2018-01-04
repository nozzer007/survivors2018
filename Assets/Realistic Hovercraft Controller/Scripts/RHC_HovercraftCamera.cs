//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class RHC_HovercraftCamera : MonoBehaviour{

	// The target we are following
	public Transform player;
	private Rigidbody playerRigid;
	private Camera cam;

	// The distance in the x-z plane to the target
	public float distance = 6.0f;
	
	// the height we want the camera to be above the target
	public float height = 2.0f;
	
	public float heightOffset = .75f;
	public float heightDamping = 5.0f;
	public float rotationDamping = 5.0f;
	public bool useSmoothRotation = true;
	
	public float minimumFOV = 50f;
	public float maximumFOV = 80f;
	
	public float maximumTilt = 25f;
	private float tiltAngle = 0f;
	
	void Start(){
		
		// Early out if we don't have a target
		if (!player){
			if(GameObject.FindObjectOfType<RHC_HovercraftController>())
				player = GameObject.FindObjectOfType<RHC_HovercraftController>().transform;
			else
				return;
		}

		playerRigid = player.GetComponent<Rigidbody>();
		cam = GetComponent<Camera>();
		
	}
	
	void Update(){
		
		// Early out if we don't have a target
		if (!player)
			return;
		
		if(playerRigid != player.GetComponent<Rigidbody>())
			playerRigid = player.GetComponent<Rigidbody>();
		
		//Tilt Angle Calculation.
		tiltAngle = Mathf.Lerp (tiltAngle, (Mathf.Clamp (player.InverseTransformDirection(playerRigid.velocity).x, -35, 35)), Time.deltaTime * 2f);

		if(!cam)
			cam = GetComponent<Camera>();

		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Lerp (minimumFOV, maximumFOV, (playerRigid.velocity.magnitude * 3.6f) / 150f), Time.deltaTime * 5f);
		
	}
	
	void LateUpdate (){
		
		// Early out if we don't have a target
		if (!player || !playerRigid)
			return;
		
		float speed = (playerRigid.transform.InverseTransformDirection(playerRigid.velocity).z) * 3.6f;
		
		// Calculate the current rotation angles.
		float wantedRotationAngle = player.eulerAngles.y;
		float wantedHeight = player.position.y + height;
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		if(useSmoothRotation)
			rotationDamping = Mathf.Lerp(0f, 5f, (playerRigid.velocity.magnitude * 3.6f) / 100f);
		
		if(speed < -10)
			wantedRotationAngle = player.eulerAngles.y + 180;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = player.position;
		transform.position -= currentRotation * Vector3.forward * distance;

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
		
		// Always look at the target
		transform.LookAt (new Vector3(player.position.x, player.position.y + heightOffset, player.position.z));
		transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y, Mathf.Clamp(tiltAngle, -10f, 10f));
		
	}

}