using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Camera mainCamera;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private ParticleSystem shootVFX;
	[SerializeField] private GameObject[] clip;
	[SerializeField] private GameObject extraMagPrefab;
	[SerializeField] private GameObject uiCanvas;
	[SerializeField] private float recoilForce;
	[SerializeField] private GameObject[] extraMagGO;
	private int ammo = 6;
	private int extraMags = 30;
	// Start is called before the first frame update
	void Start()
	{
		extraMagGO = new GameObject[extraMags];
		ammoManagement();
		instantiateMags();
	}

	// Update is called once per frame
	void Update()
	{
		turnTowardsMouse();
		inputManager();
		removeVelocity();
	}
	// Sets the rotation of the player towards the mouse position relative to the camera
	private void turnTowardsMouse()
	{
		Ray ray = mainCamera.ScreenPointToRay(new Vector2(Mouse.current.position.x.magnitude, Mouse.current.position.y.magnitude));
		if (Physics.Raycast(ray, out RaycastHit raycastHit))
		{
			transform.LookAt(new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z));
		}
	}
	// Manages the input of the player
	private void inputManager()
	{
		if (Input.GetButtonDown("Fire1") && ammo > 0)
		{
			resolveFiring();
		}
		if (Input.GetButtonDown("Fire2") && extraMags > 0 && ammo < 3)
		{
			reload();
		}
		if (Input.GetButton("Jump"))
		{
			brake();
		}
		else
		{
			gameObject.GetComponent<Rigidbody>().drag = 1;
		}
	}
	// Remove velocity when low
	private void removeVelocity()
	{
		Vector3 velocity = gameObject.GetComponent<Rigidbody>().velocity;
		float minVelocity = 0.3f;
		if ((velocity.x < minVelocity && velocity.y < minVelocity && velocity.z < minVelocity) && (velocity.x > -minVelocity && velocity.y > -minVelocity && velocity.z > -minVelocity))
		{
			gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}
	
	// Resolves the firing of the gun by the player
	private void resolveFiring()
	{
		Vector3 playerPos = new(transform.position.x, transform.position.y+.235f,transform.position.z);
		GameObject bullet = Instantiate(bulletPrefab, playerPos, transform.rotation);
		bullet.transform.Translate(Vector3.forward);
		bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
		shootVFX.Play();
		ammo--;
		ammoManagement();
		gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * recoilForce);
	}
	// Stops the player from moving
	private void brake()
	{
		gameObject.GetComponent<Rigidbody>().drag = 5;
	}
	// Ammo management
	private void ammoManagement()
	{
		hideAllClips();
		clip[ammo].SetActive(true);
	}
	private void reload()
	{
		ammo = 6;
		extraMags--;
		Destroy(extraMagGO[extraMags]);
		ammoManagement();
	}
	private void hideAllClips()
	{
		foreach (GameObject clip in clip)
		{
			clip.SetActive(false);
		}
	}
	private void instantiateMags()
	{
		for(int i = 0; i < extraMags; i++)
		{
			GameObject extraMag = Instantiate(extraMagPrefab, new Vector3(1700-40*i, 66, 0), Quaternion.identity, uiCanvas.transform);
			extraMagGO[i] = extraMag;
			Debug.Log(i + " " + extraMagGO[i].name);
		}
	}
}