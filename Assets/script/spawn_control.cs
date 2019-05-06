using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn_control : MonoBehaviour {

	public GameObject boostObj;
	public GameObject[] hazards;
	public GameObject plane;
	public float xOffset;

	private plane_control planeData;
	private int frame;

	// Use this for initialization
	void Start () {
		planeData = plane.GetComponent<plane_control>();
	}
	
	// Update is called once per frame
	void Update () {
		frame++;
		// Spawn Hazards
		if (frame % 15 != 0) { return; } // Frequency limiter
		// Calculate Max/min y if any spawns occur
		float minY = (planeData.altitude / 10 - 100) > planeData.ground.transform.position.y ? (planeData.altitude / 10 - 100) : planeData.ground.transform.position.y + 1;
		float maxY = planeData.altitude / 10 + 100;

		GameObject newHaz = Instantiate(hazards[Random.Range(0, hazards.Length - 1)], transform);
		newHaz.transform.position = new Vector3(30, Random.Range(minY + 4, maxY));

		// Spawn Boost
		if (frame % 15 != 0) { return; } // Frequency limiter
		
		// Instantiate
		GameObject newObj = Instantiate(boostObj, transform);
		newObj.transform.position = new Vector3(30, Random.Range(minY, maxY));
		
		// Delete objects too far out of frame
		foreach (Transform child in transform)
		{
			if (child.transform.position.x < -5)
			{
				Destroy(child.gameObject);
			}
		}
	}
}
