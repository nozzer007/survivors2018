using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject player;
	public float smoothing = 5f;
	public float smoothingLanding = 5f;
	float transitionRate = 0.3f;

	//public Vector3 offset;
	private GameController gameController;

	float lastCameraPosition;
	public float cameraZoom;

	float lastCameraZoom;
	public float zoomPoint;

	private GameObject landingCam;
	public Vector3 landingCamTargetPos;
	public Quaternion landingCamTargetRotation;

	public Quaternion originalRotation;



	private float new_xDelta = -9f;
	private float new_yDelta = 10.6f;
	private float new_zDelta = -9f;



	Vector3 targetCamPos;
	//bool moveToLanding = false;
	// Use this for initialization

	void Awake()
	{

	}

	void Start () {
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();

		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' in CameraController script");
		}				

		lastCameraZoom = gameController.cam_howCloseZoom;
		originalRotation = transform.rotation;
	}

	// Update is called once per frame
	void Update () {

		if (player != null && this.enabled) {


				//make the camera follow the position of the player
				targetCamPos = new Vector3 (player.transform.position.x + new_xDelta,player.transform.position.y + new_yDelta,player.transform.position.z + new_zDelta);
				transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
				transform.rotation = Quaternion.Slerp (transform.rotation, originalRotation, Time.deltaTime * 2);

				//Debug.Log ("not landing " + originalRotation);
			}
			// set camera zoom to the speed of the ship
			cameraZoom = player.GetComponent<Rigidbody> ().velocity.magnitude;

			//get current zoom
			zoomPoint = gameController.cam_howCloseZoom;

			//checks if the zoom mode has just changed. if it has it goes into this code
			if (lastCameraZoom != gameController.cam_howCloseZoom) {
				if (lastCameraZoom < gameController.cam_howCloseZoom) {
					zoomPoint = smoothTransitionToZoomPoint (lastCameraZoom, gameController.cam_howCloseZoom);

				}

				if (lastCameraZoom > gameController.cam_howCloseZoom) {
					zoomPoint = smoothTransitionToZoomPoint (lastCameraZoom, gameController.cam_howCloseZoom);
				}
				cameraZoom = zoomPoint;
				cameraZoom += 0.0001f;
				lastCameraZoom = zoomPoint;
			}else{
				lastCameraZoom = gameController.cam_howCloseZoom;
			}


			// only move camera zoom if the camera is within the zoom point
			if (cameraZoom > zoomPoint && cameraZoom < gameController.cam_howFarZoom)
			{
				//Camera.main.orthographicSize = Mathf.Lerp (lastCameraPosition, cameraZoom, Time.deltaTime * transitionRate);
				//		if(Camera.main != null)
				//Camera.main.orthographicSize = smoothTransitionToZoomPoint(cameraZoom, lastCameraPosition);
//				Camera.main.fieldOfView  = smoothTransitionToZoomPoint(cameraZoom, lastCameraPosition);

			}



	}

	float smoothTransitionToZoomPoint(float in_fromThis, float in_toThis)
	{
		float smoothZoom;
		smoothZoom = Mathf.Lerp (in_fromThis, in_toThis, Time.deltaTime * transitionRate);			
		lastCameraPosition = smoothZoom;
		//CameraImpact()Debug.Log ("Camera Zoom " + smoothZoom);
		return smoothZoom;
	}


}
