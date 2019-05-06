using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class selectors_control : MonoBehaviour {

	public GameObject angleSelector;
	public int intensity = 0;
	public GameObject rotateAboutObj;
	private bool angleIsSet = false;

	// Use this for initialization
	void Start () {
		angleSelector = gameObject.transform.GetChild(0).gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (!angleIsSet) // Continue rotating the ting
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
			Vector3 arrowPos = angleSelector.transform.position;
			Vector3 reference = rotateAboutObj.transform.position;
			float cursorAngle = Mathf.Floor(Mathf.Atan2(mousePos.y - reference.y, mousePos.x - reference.x) * Mathf.Rad2Deg);
			float currentAngle = Mathf.Floor(Mathf.Atan2(arrowPos.y - reference.y, arrowPos.x - reference.x)*Mathf.Rad2Deg);
			float final = cursorAngle - currentAngle;
			if (cursorAngle > 30 || cursorAngle < -1) { final = 0; }
			angleSelector.transform.RotateAround(rotateAboutObj.transform.position, new Vector3(0, 0, 1), final);
			if (Input.GetMouseButtonDown(0))
			{
				angleIsSet = true;
				Game.LaunchAngle = (int)Mathf.Floor(currentAngle);
			}
		} else { // Shift the bar
				 // For now just change scene ;)
			SceneManager.LoadScene("main");
		}
	}
}
