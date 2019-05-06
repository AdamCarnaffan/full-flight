using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frame_control : MonoBehaviour {

	//public GameObject plane;
	public Vector2 frameDimmensions;
	//private plane_control planeData;

	// Use this for initialization
	void Start () {
		//planeData = plane.GetComponent<plane_control>();
	}
	
	// Update is called once per frame
	void Update () {
		// Move frames that have left view to end
		foreach (Transform child in transform)
		{
			if (child.position.x < -10)
			{
				child.position = child.position + new Vector3(10 + (float)9.6 * (frameDimmensions.x - 1), 0, 0); // Should be variable number of frames
			}
			if (child.position.y < -8)
			{
				child.position = child.position + new Vector3(0, 10 + 8 * (frameDimmensions.y - 1), 0); // Should indicate variable number of frames
			} else if (child.position.y > 8)
			{
				child.position = child.position - new Vector3(0, 10 + 8 * (frameDimmensions.y - 1), 0); // Should indicate variable number of frames
			}
		}
	}
}
