using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class plane_control : MonoBehaviour {

	public int maxBoost = 100;
	public Slider fuelSlider;
	public Image[] altitudeNeedles;
	public Image speedNeedle;
	public Image vsiNeedle;
	public Text distanceDisplay;
	public float altitude;
	public float speed;
	public float speedLimit;
	public GameObject motionControl;
	public GameObject ground;

	private int currentBoost;
	private float verticalSpeed;
	private float timeAtZero;
	private bool isOver;
	private float[] initialAltitudeArrowAngles;
	private Rigidbody rb;
	private motion_control env;
	private float distance;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		env = motionControl.GetComponent<motion_control>();
		currentBoost = maxBoost;
		fuelSlider.maxValue = maxBoost;
		fuelSlider.minValue = 0f;
		UpdateFuelUI();

		distance = 0;
		timeAtZero = 0;
		isOver = false;

		// Get the initial rotations of the arrows
		int max = altitudeNeedles.Length;
		initialAltitudeArrowAngles = new float[max];
		for (int i = 0; i < max; i++)
		{
			initialAltitudeArrowAngles[i] = altitudeNeedles[i].transform.rotation.z;
		}

		// Apply initial condition for plane
		RotatePlane(new Vector3(0, 0, Game.LaunchAngle));
		if (Game.LaunchAngle > 0)
		{
			env.MoveObjects(new Vector2(0, 55 * Mathf.Sin(Game.LaunchAngle * Mathf.Deg2Rad)));
		}
	}
	
	// Update is called once per frame
	void Update () {
		float planeAngle = Vector3.Angle(transform.forward, new Vector3(1, 0, 0));
		float wingAngle = Vector3.Angle(transform.right, new Vector3(0,1,0));
		Vector2 frameTrans;
		// Do the movement stuffs
		speed = speed - (float)3.2;
		frameTrans = new Vector2(speed / 1000, 0); // Do a small update

		// Decrese further if angled up
		if (planeAngle > 100)
		{
			speed = speed - (float)0.0006*speed;
			frameTrans = frameTrans + new Vector2(0, (float)0.009 * speed*(planeAngle - 95) / 900);
		} else 
		// Increase if angled down
		if (planeAngle < 80)
		{
			speed = speed + (float)0.0005 * speed;
			if (speed > speedLimit) { speed = speedLimit; }
			frameTrans = frameTrans + new Vector2(0, -(float)0.009 * speed*(85 - planeAngle) / 900);
		}

		// Lower Limit speed to zero
		if (speed < 0) { speed = 0; }
		// Drop altitude if plane is going less than 250
		if (speed < 1500 && altitude > 0)
		{
			if (speed > 0)
			{
				frameTrans = frameTrans + new Vector2(0, -(float)0.002 * (1500 - speed) / 50);
				timeAtZero = 0;
			} else
			{
				timeAtZero = timeAtZero + (float)0.01;
				if (timeAtZero > 10) { timeAtZero = 4; }
				frameTrans = frameTrans + new Vector2(0, -timeAtZero * 4 / 5);
			}
		}
		// Limit translation
		if (frameTrans.y + altitude < 0)
		{
			frameTrans = new Vector2(frameTrans.x, -altitude);
		}

		// Update plane position
		env.MoveObjects(frameTrans/35);
		// Calculate VSI val
		verticalSpeed = frameTrans.y/10;

		// Update Altitude & Speed values based on frame
		altitude = (gameObject.GetComponent<Collider>().bounds.min.y - ground.transform.position.y - (float)1.5)*10;
		distance = distance + speed*Time.deltaTime / 360;
		UpdateStatsUI();
		if (altitude < 1)
		{
			EndGame();
		} else if (altitude > 43000) // Flying too high is big bad
		{
			// Huge penalty (should inform user)
			speed = speed / (float)1.125;
		}

		// Check collisions manually
		RaycastHit[] result = rb.SweepTestAll(new Vector3(1, 0, 0), (float)0.2);
		foreach (RaycastHit t in result)
		{
			HandleCollision(t.collider);
		}

		// Get input for changes
		if (Input.GetKey(KeyCode.D))
		{
			if (planeAngle > 100 || planeAngle < 80)
			{
				// Huge penalty (should inform user)
				speed = speed / (float)1.125;
				return;
			}
			if (wingAngle < 120)
			{
				RotatePlane(new Vector3(-1, 0, 0));
			}
		} else if (Input.GetKey(KeyCode.A))
		{
			if (planeAngle > 100 || planeAngle < 80)
			{
				// Huge penalty (should inform user)
				speed = speed / (float)1.125;
				return;
			}
			if (wingAngle > 60)
			{
				RotatePlane(new Vector3(1, 0, 0));
			}
		} else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
		{
			if (wingAngle > 105 || wingAngle < 75)
			{
				// Huge penalty (should inform user)
				speed = speed / (float)1.125;
				return;
			}
			if (Input.GetKey(KeyCode.S))
			{
				RotatePlane(new Vector3(0, 0, -1));
			}
			else
			{
				RotatePlane(new Vector3(0, 0, 1));
			}
		} else if (Input.GetKey(KeyCode.Space))
		{
			if (speed >= speedLimit || isOver) { return; }
			currentBoost--;
			if (currentBoost < 0)
			{
				currentBoost = 0;
			} else
			{
				speed = speed + 10;
			}
			UpdateFuelUI();
		}
	}

	void HandleCollision(Collider other) 
	{
		if (other.gameObject.tag == "fuel")
		{
			// get fuels
			currentBoost = currentBoost + (int)Mathf.Ceil(maxBoost * (float)0.4);
			if (currentBoost > maxBoost) { currentBoost = maxBoost; }
			Destroy(other.gameObject);
			UpdateFuelUI();
		}
		else if (other.gameObject.tag == "hazard")
		{
			hazard_spec otherData = other.gameObject.GetComponent<hazard_spec>();
			// Apply penalties
			speed = speed / (1 + Mathf.Pow((float)1.3, otherData.damageScale) / 10);
			RotatePlane(new Vector3(UnityEngine.Random.Range(-1, 1) * 12*otherData.damageScale, 0, 0)); // Turbulence
			RotatePlane(new Vector3(0, 0, 90-Vector3.Angle(transform.forward, new Vector3(1, 0, 0)))); // Level the plane if unlevel
			// Destroy object
			if (otherData.destroyOnContact)
			{
				Destroy(other.gameObject);
			}
		}
	}

	private void RotatePlane(Vector3 factor)
	{
		if (factor.y != 0) { return; }
		float planeAngle = Vector3.Angle(transform.forward, new Vector3(1, 0, 0));
		float wingAngle = Vector3.Angle(transform.right, new Vector3(0, 1, 0));
		if (factor.z != 0 && factor.x == 0)
		{
			if ((factor.z > 0 && planeAngle < 140) || (factor.z < 0 && planeAngle > 40))
			{
				transform.Rotate(0, 0, factor.z, Space.World);
			}
		} else if (factor.x != 0 && factor.z == 0)
		{
			if (factor.x > 0 && wingAngle > 60 || factor.x < 0 && wingAngle < 120)
			{
				transform.Rotate(factor.x, 0, 0, Space.World);
			}
		}
	}

	private void UpdateStatsUI()
	{
		// Update Speed
		Vector2 currentRotation = new Vector2(Vector3.Angle(speedNeedle.transform.up, new Vector3(1, 0, 0)),
											Vector3.Angle(speedNeedle.transform.up, new Vector3(0, 1, 0))); // if x > 90 then 360-Y else Y
		float currentRot = 0;
		if (currentRotation.x > 90)
		{
			currentRot = 360 - (float)Math.Floor(currentRotation.y);
		}
		else
		{
			currentRot = (float)Math.Floor(currentRotation.y);
		}
		float knots = speed/100 * (float)1.852; // Speed/100 = km/h
		speedNeedle.rectTransform.Rotate(new Vector3(0, 0, (float)Math.Floor(currentRot) - (float)Math.Floor(knots*3/2)));
		// Update Altitude
		float altitudeTarg = altitude;
		for (int i = 0; i < altitudeNeedles.Length; i++)
		{
			currentRotation = new Vector2(Vector3.Angle(altitudeNeedles[i].transform.up, new Vector3(1, 0, 0)),
											Vector3.Angle(altitudeNeedles[i].transform.up, new Vector3(0, 1, 0))); // if x > 90 then 360-Y else Y
			currentRot = 0;
			if (currentRotation.x > 90)
			{
				currentRot = 360 - (float)Math.Floor(currentRotation.y);
			} else
			{
				currentRot = (float)Math.Floor(currentRotation.y);
			}
			float targetValue = (float)(altitudeTarg % Math.Pow(10, i + 3));
			altitudeTarg = altitudeTarg - targetValue;
			float rot = (float)Math.Floor(targetValue * 360 / (float)Math.Pow(10, i + 3));
			altitudeNeedles[i].rectTransform.Rotate(new Vector3(0, 0, (currentRot - initialAltitudeArrowAngles[i]) - rot));
		}

		// Update VSI
		currentRotation = new Vector2(Vector3.Angle(vsiNeedle.transform.up, new Vector3(-1, 0, 0)),
											Vector3.Angle(vsiNeedle.transform.up, new Vector3(0, 1, 0))); // if x > 90 then 360-Y else Y
		currentRot = currentRotation.x;
		if (currentRotation.y > 90) { currentRot = currentRot * -1; }

		vsiNeedle.rectTransform.Rotate(new Vector3(0, 0, currentRot - verticalSpeed*(float)54.807*5));
	}

	void FixedUpdate()
	{
		// Update distance score display
		distanceDisplay.text = "Distance:  " + ((int)Math.Floor(distance)).ToString() + "m";
	}

	void UpdateFuelUI()
	{
		fuelSlider.value = currentBoost;
	}

	void EndGame()
	{
		Game.Distance = (int)distance;
		SceneManager.LoadScene("submitScore");
	}
}
