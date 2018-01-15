using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGunController : MonoBehaviour {

	[Tooltip("Flash from the turret barrel.")]
	public GameObject muzzleFlash; 
	private bool muzzleFlashStatus;

	[Tooltip("Flash from the hit target.")]
	public GameObject hitSpark; 
	private bool hitSparkStatus;

	[Tooltip("Light flash from turret cannon.")]
	public Light gunLight;

	[Tooltip("Line renderers for laser.")]
	public LineRenderer gunLine; 

	GameController gameController;
	Transform AttachedToGameObjectTransform;
	GameObject Player;
	RHC_HovercraftController ship;

	bool FireWep = false;
	public float laserTemp = 0.0f;

	float laserCooledAmount = 50.0f;
	float maxLaserTemp = 100.0f;

	bool WeaponOverheated;
	float timer;

	//how long to wait between shots
	float timeBetweenShots = 0.8f;
	float originalTimeBetweenShots = 0.8f;

	float shootTime = 0.1f;
	public GameObject cannonTip;

	Ray shootRay = new Ray();

	//If raycast hits something, the information as to what is hit, is recorded.
	RaycastHit shootHit;

	public AudioClip fireSound;
	private AudioSource fireSoundSource;

	public int damagePerShot = 10;

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


		gunLine.enabled = false;
		muzzleFlashStatus = false;
		hitSparkStatus = false;


	}

	// Use this for initialization
	void Start () {
		SoundsInit();
	}

	void SoundsInit(){

		fireSoundSource = RHC_CreateAudioSource.NewAudioSource(gameObject, "Laser Sound", 5f, 1f, fireSound, false, false, false);

	}
	
	// Update is called once per frame
	void Update ()
	{
		//Add later
		//gameController.UpdateLaserBar (laserTemp, maxLaserTemp);
		if( FireWep && gameController.getShipStatus () != "LANDED" && !WeaponOverheated)
		{
			if (timerShoot ()) {
				LaserShoot ();


			} else {
				DisableEffects ();
			}
		} else {
			WeaponCooling ();
			DisableEffects ();
		}

		if (WeaponOverheated) {
			WeaponCooling ();
		}

	}


	public void FireWeapon()
	{
		FireWep = true;
	}

	public void ReleaseWeapon()
	{
		FireWep = false;
	}

	bool WeaponCooling()
	{
		if (laserTemp > 0.0f)
			laserTemp -=0.25f;


		if (WeaponOverheated && laserTemp > laserCooledAmount)
			return false;
		else if(laserTemp < laserCooledAmount) {
			WeaponOverheated = false;
		}
		return true;
	}

	bool timerShoot()
	{

		//increment timer
		timer += Time.deltaTime;
		//if timer is less than amount of time I should shoot for
		if (timer < shootTime) {
			//shoot
			return true;
		} else {
			if (timer > timeBetweenShots) {
				timer = 0.0f;
				return true;
			} else {
				return false;
			}
		}
	}

	void DisableEffects()
	{
		gunLine.enabled = false;
		gunLight.enabled = false;
		hitSpark.SetActive (false);
		muzzleFlash.SetActive (false);
		timeBetweenShots = Random.Range(0.12f,0.24f);


	}

	void LaserShoot()
	{

		if (laserTemp > maxLaserTemp) {
			WeaponOverheated = true;
			//Debug.Log ("Lasers getting too hot  - entering cooling cycle!");
			return;
		} else {
			laserTemp+= 1.0f;
		}
		// if the temp is over 50% then the time between shots will start to increasing slowing down the rate of fire.
		if(laserTemp > 50.0f)
		{
			timeBetweenShots += 0.25f;
		}
		else{
			timeBetweenShots = originalTimeBetweenShots;
		}

		if (WeaponOverheated)
			return;

		shootRay.origin = cannonTip.transform.position;
		shootRay.direction = cannonTip.transform.forward;

		if (Physics.Raycast (shootRay, out shootHit, 600)) {

			gunLine.SetPosition (0, cannonTip.transform.position);
			gunLine.SetPosition (1, shootHit.point);

			if (!hitSparkStatus) {
				hitSpark = Instantiate(hitSpark, shootHit.point, cannonTip.transform.rotation) as GameObject;
				hitSparkStatus = true;
			}

			if (!muzzleFlashStatus) {
				muzzleFlash = Instantiate(muzzleFlash, shootHit.point, cannonTip.transform.rotation) as GameObject;
				muzzleFlashStatus = true;
			}
			//Debug.Log ("Im shooting and hitting this - " + shootHit.collider.transform.name);
			muzzleFlash.transform.position = cannonTip.transform.position;
			hitSpark.transform.position = shootHit.point;

			muzzleFlash.transform.rotation = cannonTip.transform.rotation;
			muzzleFlash.SetActive(true);
			hitSpark.SetActive(true);
			gunLine.enabled = true;
			gunLight.enabled = true;
			fireSoundSource.Play();
			//Debug.Log ("name -" + shootHit.collider.transform.gameObject.name + " tag -" + shootHit.collider.transform.gameObject.tag);
			if (shootHit.collider.transform.gameObject.tag == "Enemy") {
				//Debug.Log ("Here");
				TriggerDamage (shootHit.collider.transform.gameObject);
			}
		} else {

			DisableEffects ();
		}
	}

	public void FireLaser(bool buttonPressed)
	{
		FireWep = buttonPressed;
	}
		
	public virtual void TriggerDamage(GameObject hitObj) //override this if you have your own health script
	{
		Debug.Log (hitObj.transform.root.name);
//		if(hitObj.GetComponent<TurretSystem_Health> ()!=null)
//			hitObj.GetComponent<TurretSystem_Health> ().TakeDamage (damagePerShot);
//		else
//			if(hitObj.transform.root.GetComponent<TurretSystem_Health> ()!=null)
//			{
//				hitObj.transform.root.GetComponent<TurretSystem_Health>().TakeDamage (damagePerShot);
//			}

	}

}
