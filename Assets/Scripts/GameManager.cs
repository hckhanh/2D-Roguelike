using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private BoardManager boardManager;
	private int level = 3;

	void Awake()
	{
		InitializeScene ();
	}

	void InitializeScene ()
	{
		boardManager = GetComponent<BoardManager> ();
		boardManager.SetupScene (level);
	}

}
