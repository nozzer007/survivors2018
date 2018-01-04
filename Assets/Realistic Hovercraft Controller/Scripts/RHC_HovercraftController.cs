//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Rigidbody))]
public class RHC_HovercraftController : MonoBehaviour {

	//Rigidbody
	private Rigidbody rigid;
	private ActionController actionController;
	public Transform COM;

	//Receive Player Inputs and Engine Running Bool
	public bool canControl = true;
	public bool engineRunning = true;

	//Controller Types
	public enum ControllerType{KeyboardController, MobileUIController, MobileNGUIController};
	public ControllerType _controllerType;

	//Stabilizer Thrusters
	public List<Transform> stabilizerThrusters = new List<Transform>();

	public float stabilizerForce = 0f;
	public float stabilizerDamperForce = 0f;
	public float stabilizerConstant = 10000f;
	public float stabilizerDamperConstant = 1000f;

	private float oldLenght;
	private float currentLenght;
	private float springVelocity;

	//Torques
	public float motorTorque = 20000f;
	public float steerTorque = 5000f;

	//Engine Torque Curve Depends On Craft Speed
	public AnimationCurve engineTorqueCurve;

	//Speeds
	private float speed = 0f;
	public float maximumSpeed = 250f;

	//Inputs
	[HideInInspector]public float gasInput = 0f;
	[HideInInspector]public float actionInput = 0f;
	[HideInInspector]public float steerInput = 0f;

	private float maxFuel = 2000.0f;
	private float fuelRemaining;
	private float fuelUsing;
	private float tickOver = 0.02f;

	//Stabilizer Variables
	private float stability = .5f;
	private float reflection = 100f;
	public float stableHeight = 3f;
	public float maximumAngularVelocity = 2f;

	//Sounds
	public AudioClip engineSound;
	private AudioSource engineSoundSource;
	public AudioClip[] crashSounds;
	private AudioSource crashSoundSource;

	//Particles
	private List<ParticleSystem> _thrusterSmoke = new List<ParticleSystem>();
	public GameObject thrusterGroundSmoke;
	public ParticleSystem[] thrusterParticles;

	//Lights
	public Light[] headLights;
	public Light[] particleLights;

	//Contach Particles
	public GameObject contactSparkle;
	public int maximumContactSparkle = 5;
	private List<ParticleSystem> contactSparkeList = new List<ParticleSystem>();

	//Damage
	struct originalMeshVerts{
		public Vector3[] meshVerts;
	}
	
	public bool useDamage = true;
	public MeshFilter[] deformableMeshFilters;
	
	public float randomizeVertices = 1f;
	public float damageRadius = .5f;
	
	// Comparing Original Vertex Position And Last Vertex Position To Decide Mesh Is Repaired Or Not.
	private float minimumVertDistanceForDamagedMesh = .002f;
	
	private Vector3[] colliderVerts;
	private originalMeshVerts[] originalMeshData;
	[HideInInspector]public bool sleep = true;
	
	// Maximum Vert Distance For Limiting Damage. Setting 0 Will Disable The Limit.
	public float maximumDamage = .5f;
	private float minimumCollisionForce = 5f;
	public float damageMultiplier = 1f;

	public bool repairNow = false;
	
	private Vector3 localVector;
	private Quaternion rot = Quaternion.identity;

	public delegate void RHCSpawned();
	public static event RHCSpawned OnRHCSpawned;

	private GameController gameController;

	private string action;
	private string shipStatus;

	private Vector3 originalRotationPosition;

	private ShipGunController laser;

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
		RestoreFuel ();
	}
	
	void Start (){

		rigid = GetComponent<Rigidbody>();
		rigid.centerOfMass = COM.localPosition;
		rigid.maxAngularVelocity = maximumAngularVelocity;

		actionController = GetComponent<ActionController> ();

		SoundsInit();
		ParticlesInit();
		DamageInit();

		for (int i = 0; i < stabilizerThrusters.Count; i++) {

			if (!stabilizerThrusters [i].GetComponent<RHC_Thruster> ())
				stabilizerThrusters [i].gameObject.AddComponent<RHC_Thruster> ();

		}

		if (OnRHCSpawned != null)
			OnRHCSpawned();

		originalRotationPosition = this.transform.rotation.eulerAngles;
	}

	void SoundsInit(){

		engineSoundSource = RHC_CreateAudioSource.NewAudioSource(gameObject, "Engine Sound", 5f, 1f, engineSound, true, true, false);

	}

	void ParticlesInit(){

		if(thrusterGroundSmoke){

			for(int i = 0; i < stabilizerThrusters.Count; i++){

				GameObject ps = (GameObject)Instantiate(thrusterGroundSmoke, transform.position, transform.rotation) as GameObject;
				_thrusterSmoke.Add(ps.GetComponent<ParticleSystem>());
				ps.transform.SetParent(stabilizerThrusters[i].transform);
				ps.transform.localPosition = Vector3.zero;
				ParticleSystem.EmissionModule em = ps.GetComponent<ParticleSystem> ().emission;
				em.enabled = false;

			}

		}

		if(contactSparkle){
			
			for(int i = 0; i < maximumContactSparkle; i++){
				GameObject sparks = (GameObject)Instantiate(contactSparkle, transform.position, Quaternion.identity) as GameObject;
				sparks.transform.SetParent(transform);
				contactSparkeList.Add(sparks.GetComponent<ParticleSystem>());
				ParticleSystem.EmissionModule em = sparks.GetComponent<ParticleSystem> ().emission;
				em.enabled = false;
			}
			
		}

	}

	public void DamageInit (){
		
		if (deformableMeshFilters.Length == 0){
			MeshFilter[] allMeshFilters = GetComponentsInChildren<MeshFilter>();
			List <MeshFilter> properMeshFilters = new List<MeshFilter>();
			foreach(MeshFilter mf in allMeshFilters){
				properMeshFilters.Add(mf);
			}
			deformableMeshFilters = properMeshFilters.ToArray(); 
		}
		
		LoadOriginalMeshData();
		
	}

	void LoadOriginalMeshData(){
		
		originalMeshData = new originalMeshVerts[deformableMeshFilters.Length];
		
		for (int i = 0; i < deformableMeshFilters.Length; i++){
			originalMeshData[i].meshVerts = deformableMeshFilters[i].mesh.vertices;
		}
		
	}
	
	void Repairing(){
		
		if (!sleep && repairNow){
			
			int k;
			sleep = true;
			for(k = 0; k < deformableMeshFilters.Length; k++){
				Vector3[] vertices = deformableMeshFilters[k].mesh.vertices;
				if(originalMeshData==null)
					LoadOriginalMeshData();
				for (int i = 0; i < vertices.Length; i++){
					vertices[i] += (originalMeshData[k].meshVerts[i] - vertices[i]) * (Time.deltaTime * 2f);
					if((originalMeshData[k].meshVerts[i] - vertices[i]).magnitude >= minimumVertDistanceForDamagedMesh)
						sleep = false;
				}
				deformableMeshFilters[k].mesh.vertices=vertices;
				deformableMeshFilters[k].mesh.RecalculateNormals();
				deformableMeshFilters[k].mesh.RecalculateBounds();
			}
			
			if(sleep)
				repairNow = false;
			
		}
		
	}
	
	void DeformMesh(Mesh mesh, Vector3[] originalMesh, Collision collision, float cos, Transform meshTransform, Quaternion rot, float MeshScale){
		
		Vector3[] vertices = mesh.vertices;
		
		foreach (ContactPoint contact in collision.contacts){
			
			Vector3 point =meshTransform.InverseTransformPoint(contact.point);
			
			for (int i = 0; i < vertices.Length; i++){
				if ((point - vertices[i]).magnitude < damageRadius){
					vertices[i] += rot * ((localVector * (damageRadius - (point - vertices[i]).magnitude) / damageRadius) * cos + (UnityEngine.Random.onUnitSphere * (randomizeVertices / 500f)));
					if (maximumDamage > 0 && ((vertices[i] - originalMesh[i]).magnitude) > maximumDamage){
						vertices[i] = originalMesh[i] + (vertices[i] - originalMesh[i]).normalized * (maximumDamage);
					}
				}
			}
			
		}
		
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		
	}

	void Update(){


		if(useDamage)
			Repairing();

	}

	void FixedUpdate(){
		//if the ship is not in a landing zone
		//allow other inputs
		shipStatus = gameController.getShipStatus();
		if (shipStatus != "LEVELCOMPLETE") {
			if (fuelRemaining > 0) {
				if (!engineRunning)
					return;
				if (shipStatus == "FLYING") {
					AllowFlyingControls ();
					if (actionInput > 0) {
						FireLaser (true);
					} else {
						FireLaser (false);
					}		
				}

				if (shipStatus == "LANDED") {
					if (actionInput > 0)
						TakeOff ();

				}

				if (shipStatus == "READYTOLAND") {
					if (actionInput > 0)
						Land ();

				}
		
			} else {
				Debug.Log ("YOU RAN OUT OF FUEL");
				gameController.LoseALife ();
				StopShipMovement ();
				engineRunning = false;
				Particles ();
				Lights ();

			}
		}

	}


	void AllowFlyingControls()
	{
			//Debug.Log ("In Control");
			Inputs ();
			Particles ();
			Sounds ();
			Lights ();
			Engine ();
			Stabilizer ();
	}
	public string ReturnFuel()
	{
		//float remainingFuelBar = fuelRemaining / maxFuel;
		//int int_remainingFuelBar = (int)remainingFuelBar;
		return "FUEL : " + fuelRemaining.ToString();
	}
	void StopShipMovement ()
	{
		gasInput = 0;
		//stabilise rigidbody movement by setting velocity to zero.
		rigid.velocity = Vector3.zero;
		rigid.angularVelocity = Vector3.zero;
		speed = 0;
	}

	public void RestoreFuel()
	{
		if (fuelRemaining < maxFuel) {
			fuelRemaining = maxFuel;
			engineRunning = true;
		}
	}

	public void Land()
	{
		if (!gameController.LandingPressed) {
			Debug.Log ("Land");	

			gameController.LandingPressed = true;
			gameController.SetLandingIndexFromLastLandedPad ();
			gameController.readyToFly ();

//get the ship to rotate to the original 
			this.transform.eulerAngles = originalRotationPosition;
			if (gameController.inZoneType == "FUEL")
				RestoreFuel ();
		}
	}

	public void TakeOff()
	{

		if (!gameController.LandingPressed)
		{
			Debug.Log ("Take Off");
			gameController.LandingPressed = true;
			rigid.AddForce (transform.up * 8, ForceMode.VelocityChange);
			rigid.AddForce (transform.forward * 5, ForceMode.VelocityChange);
		}


	}

	void FireLaser(bool buttonPressed)
	{
		Debug.Log ("Fire Laser");
		laser = GetComponent<ShipGunController> ();
		laser.FireLaser (buttonPressed);

	}

	void Inputs(){

		if(_controllerType == ControllerType.KeyboardController){

			if(canControl){
				gasInput = Input.GetAxis("Vertical");
				steerInput = Input.GetAxis("Horizontal");
			}else{
				gasInput = 0f;
				steerInput = 0f;
			}

		}

	}


	void Engine(){

		fuelRemaining -= (gasInput + tickOver);

		//Debug.Log ("(Fuel using-" + gasInput + ") (Fuel remaining-"+fuelRemaining+")");
		Debug.Log("FUEL UPDATING");
		speed = rigid.velocity.magnitude * 3.6f;

		if(speed < maximumSpeed)
			rigid.AddForceAtPosition(transform.forward * ((motorTorque * Mathf.Clamp(gasInput, -.25f, 1f)) * engineTorqueCurve.Evaluate(speed)), COM.position, ForceMode.Force);

		//rigid.AddRelativeTorque(Vector3.right * ((-motorTorque / 25f) * gasInput), ForceMode.Force);

		rigid.AddRelativeTorque(Vector3.up * ((steerTorque * Mathf.Lerp(1f, .25f, speed / maximumSpeed)) * steerInput), ForceMode.Force);
		rigid.AddRelativeTorque(Vector3.forward * ((-steerTorque / 1f) * steerInput), ForceMode.Force);

		Vector3 locVel = transform.InverseTransformDirection(rigid.velocity);
		locVel = new Vector3 (Mathf.Lerp(locVel.x, 0f, Time.fixedDeltaTime * 1f), locVel.y, locVel.z);
		rigid.velocity = transform.TransformDirection (locVel);
		rigid.angularVelocity = Vector3.Lerp(rigid.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 1f);

	}

	void Stabilizer(){

		Vector3 predictedUp = Quaternion.AngleAxis(rigid.velocity.magnitude * Mathf.Rad2Deg * stability / reflection, rigid.angularVelocity) * transform.up;
		Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);

		rigid.AddTorque(torqueVector * reflection * reflection);

	}

	void Sounds(){
		
		if(engineRunning){
			engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, Mathf.Clamp(Mathf.Abs(gasInput), .3f, 1f), Time.deltaTime * 2f);
			engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, Mathf.Clamp(Mathf.Abs(gasInput) + Mathf.Lerp(0f, .25f, speed / maximumSpeed), .45f, 1.5f), Time.deltaTime * 2f);
		}else{
			engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, Time.deltaTime * 2f);
			engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, 0f, Time.deltaTime * 2f);
		}

	}

	void Particles(){

		if(!engineRunning){

			foreach(ParticleSystem ps in thrusterParticles){
				ParticleSystem.EmissionModule em = ps.emission;
				if (em.enabled)
					em.enabled = false;
			}

			foreach(ParticleSystem ps in _thrusterSmoke){
				ParticleSystem.EmissionModule em = ps.emission;
				if (em.enabled)
					em.enabled = false;
			}
			
			foreach(Light l in particleLights){
				l.intensity = Mathf.Lerp(l.intensity, 0f, Time.deltaTime * 1f);
			}

			return;

		}

		foreach(ParticleSystem ps in thrusterParticles){
			ParticleSystem.EmissionModule em = ps.emission;
			if (!em.enabled)
				em.enabled = true;
			ps.startSpeed = Mathf.Clamp(-gasInput, -1f, .3f);
		}

		foreach(Light l in particleLights){
			l.intensity = Mathf.Lerp(.25f, 2f, Mathf.Abs(gasInput * 2f));
		}

		for(int i = 0; i < stabilizerThrusters.Count; i++){

			if(!thrusterGroundSmoke)
				return;
			
			RaycastHit hit;

			if(Physics.Raycast(stabilizerThrusters[i].position, stabilizerThrusters[i].forward, out hit, stableHeight * 2f) && hit.transform.root != transform){

				ParticleSystem.EmissionModule em = _thrusterSmoke[i].emission;

				if(!em.enabled)
					em.enabled = true;
				
				_thrusterSmoke[i].transform.position = hit.point;

			}else{

				ParticleSystem.EmissionModule em = _thrusterSmoke[i].emission;

				if(em.enabled)
					em.enabled = false;

			}

		}

	}

	void Lights(){

		if(Input.GetKeyDown(KeyCode.L) && canControl){
			foreach(Light l in headLights)
				l.enabled = !l.enabled;
		}

	}

	void CollisionParticles(Vector3 contactPoint){
		
		for(int i = 0; i < contactSparkeList.Count; i++){
			
			if(contactSparkeList[i].isPlaying)
				return;
			
			contactSparkeList[i].transform.position = contactPoint;
			ParticleSystem.EmissionModule em = contactSparkeList[i].emission;
			em.enabled = true;
			contactSparkeList[i].Play();

		}
		
	}

	void OnCollisionEnter(Collision col){

		if(col.relativeVelocity.magnitude < 2.5f)
			return;

		if(crashSounds.Length > 0){

			crashSoundSource = RHC_CreateAudioSource.NewAudioSource(gameObject, "Crash Sound", 5f, 1f, crashSounds[Random.Range(0, crashSounds.Length)], false, false, true);
			crashSoundSource.pitch = Random.Range(.85f, 1f);
			crashSoundSource.Play();

		}

		CollisionParticles(col.contacts[0].point);

		if(useDamage){
			
			CollisionParticles(col.contacts[0].point);
			
			Vector3 colRelVel = col.relativeVelocity;
			colRelVel *= 1f - Mathf.Abs(Vector3.Dot(transform.up,col.contacts[0].normal));
			
			float cos = Mathf.Abs(Vector3.Dot(col.contacts[0].normal, colRelVel.normalized));
			
			if(colRelVel.magnitude * cos >= minimumCollisionForce){
				
				sleep = false;
				
				localVector = transform.InverseTransformDirection(colRelVel) * (damageMultiplier / 50f);
				
				if (originalMeshData == null)
					LoadOriginalMeshData();
				
				for (int i = 0; i < deformableMeshFilters.Length; i++){
					DeformMesh(deformableMeshFilters[i].mesh, originalMeshData[i].meshVerts, col, cos / deformableMeshFilters[i].transform.lossyScale.x, deformableMeshFilters[i].transform, rot, deformableMeshFilters[i].transform.lossyScale.x);
				}
				
			}
			
		}

	}

	void OnDrawGizmos(){

		RaycastHit hit;

		for(int i = 0; i < stabilizerThrusters.Count; i++){

			if(!stabilizerThrusters[i]){
				stabilizerThrusters.RemoveAt(i);
				return;
			}

			if(Physics.Raycast(stabilizerThrusters[i].position, stabilizerThrusters[i].forward, out hit, stableHeight * 2f) && hit.transform.root != transform){

				Debug.DrawRay(stabilizerThrusters[i].position, stabilizerThrusters[i].forward * hit.distance, new Color(Mathf.Lerp(1f, 0f, hit.distance / (stableHeight * 1.5f)), Mathf.Lerp(0f, 1f, hit.distance / (stableHeight * 1.5f)), 0f, 1f));
				Gizmos.color = new Color(Mathf.Lerp(1f, 0f, hit.distance / (stableHeight * 1.5f)), Mathf.Lerp(0f, 1f, hit.distance / (stableHeight * 1.5f)), 0f, 1f);
				Gizmos.DrawSphere(hit.point, .5f);

			}

			Matrix4x4 temp = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(stabilizerThrusters[i].position, stabilizerThrusters[i].rotation, Vector3.one);
			Gizmos.DrawFrustum(Vector3.zero, 30f, 5f, .1f, 1f);
			Gizmos.matrix = temp;

		}

	}

}
