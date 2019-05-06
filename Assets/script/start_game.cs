using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start_game : MonoBehaviour {

	public void Begin()
	{
		SceneManager.LoadScene("controls");
	}

	public void End()
	{
		SceneManager.LoadScene("start");
	}
}
