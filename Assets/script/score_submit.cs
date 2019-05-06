using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class score_submit : MonoBehaviour {

	public Text statText;
	public Text inputText;

	// Use this for initialization
	void Start () {
		// Add distance to stat text
		statText.text = "You travelled " + ((int)Game.Distance).ToString() + "m!\n Submit your score to join the leaderboard.";

		if (Game.PlayerName != null)
		{
			inputText.text = Game.PlayerName;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
		{
			SendName();
		}
	}

	public void SendName()
	{
		Game.PlayerName = inputText.text;
		if (Game.PlayerName.Length < 1) { return; }
		StartCoroutine(postName());
	}

	private IEnumerator postName()
	{
		string postURL = "localhost/fullflight/submit.php?name=" + Game.PlayerName + "&score=" + Game.Distance.ToString();

		WWW post = new WWW(postURL);
		yield return post;

		if (post.error != null)
		{
			Debug.Log("An error occured submitting the score");
		} else
		{
			int ind;
			int.TryParse(post.text, out ind);
			Game.PlayIndex = ind;
			SceneManager.LoadScene("leaderboard");
		}
	}
}
