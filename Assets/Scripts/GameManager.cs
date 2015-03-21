using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
	public float turnDelay = 0.1f;							//Delay between each Player turn.
	public int playerFoodPoints = 100;						//Starting value for Player food points.
	public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
	[HideInInspector]
	public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.

	private BoardManager boardManager;
	private int level = 1;
	private List<Enemy> enemies;
	private bool enemyMoving;

	// UI
	private bool doingSetup;
	private Text levelText;
	private GameObject levelImage;

	void Awake ()
	{
		if (instance == null)
			instance = this;
		else
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);
		
		enemies = new List<Enemy> ();
		InitializeScene ();
	}

	void OnLevelWasLoaded (int indexScene)
	{
		level++;
		InitializeScene ();
	}
	
	void Update ()
	{
		if (playersTurn || enemyMoving || doingSetup)
			return;

		StartCoroutine (MoveEnemy ());
	}

	void InitializeScene ()
	{
		doingSetup = true;
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Day " + level;
		levelImage.SetActive (true);
		Invoke ("HideLevelImage", levelStartDelay);

		enemies.Clear ();
		boardManager = GetComponent<BoardManager> ();
		boardManager.SetupScene (level);
	}

	private void HideLevelImage ()
	{
		levelImage.SetActive (false);
		doingSetup = false;
	}

	public void GameOver ()
	{
		levelText.text = string.Format ("After {0} days, you starved!", level);
		levelImage.SetActive (true);
		enabled = false;
	}

	IEnumerator MoveEnemy ()
	{
		enemyMoving = true;
		yield return new WaitForSeconds (turnDelay);

		if (enemies.Count == 0)
			yield return new WaitForSeconds (turnDelay);

		for (int i = 0; i < enemies.Count; i++) {
			enemies [i].MoveEnemy ();
			yield return new WaitForSeconds (enemies [i].moveTime);
		}
		playersTurn = true;
		enemyMoving = false;
	}

	public void AddEnemyToScript (Enemy script)
	{
		enemies.Add (script);
	}
}
