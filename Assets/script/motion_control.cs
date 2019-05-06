using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motion_control : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveObjects(Vector2 distance)
	{
		foreach (Transform child in transform) {
			if (child.tag == "ground")
			{
				child.position = child.position - new Vector3(0, distance.y, 0);
				foreach (Transform secondChild in child.transform)
				{
					secondChild.position = secondChild.position - new Vector3(distance.x, 0);
					if (secondChild.position.x < -3)
					{
						secondChild.position = secondChild.position + new Vector3((float)0.177 + (float)3.485 * (4 - 1), 0, 0); // Should be variable number of frames
					}
				}
			} else
			{
				child.position = child.position - new Vector3(distance.x, distance.y, 0);
			}
		}
	}
}
