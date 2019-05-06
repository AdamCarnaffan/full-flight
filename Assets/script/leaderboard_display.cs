using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class leaderboard_display : MonoBehaviour {

	public Canvas[] rows;
	public Image token;

	// Use this for initialization
	void Start () {
		StartCoroutine(GetLB());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public IEnumerator GetLB()
	{
		string postURL = "localhost/fullflight/getLB.php?ind=" + Game.PlayIndex.ToString();

		WWW post = new WWW(postURL);

		yield return post;
		// Process return
		if (post.error != null)
		{
			Debug.Log("An error occured submitting the score");
		}
		else
		{
			// Display leaderboard data
			string[] data = new string[4];
			int i = 0;
			foreach (string player in post.text.Split(';'))
			{
				if (i >= rows.Length) { break; }
				data = player.Split(',');
				rows[i].transform.Find("Rank").GetComponent<Text>().text = data[0];
				rows[i].transform.Find("Name").GetComponent<Text>().text = data[1];
				rows[i].transform.Find("Distance").GetComponent<Text>().text = data[2] + "m";
				if (data[3] != "0")
				{
					token.transform.position = new Vector2(token.transform.position.x, rows[i].transform.position.y - 10);
				}
				i++;
			}
		}
	}
}
