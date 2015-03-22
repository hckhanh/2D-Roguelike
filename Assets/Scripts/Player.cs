using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject
{

	public int wallDamge = 1;
	public int pointPerFood = 10;
	public int pointPerSoda = 20;
	public float restartLevelDelay = 1;
	public Text foodText;
	private Animator animator;
	private int food;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
	private Vector2 touchOrigin = -Vector2.one;
	#endif

	// Use this for initialization
	protected override void Start ()
	{
		animator = GetComponent<Animator> ();
		food = GameManager.instance.playerFoodPoints;
		foodText.text = "Food " + food;
		base.Start ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0;
		int vertical = 0;

		#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		if (Input.touchCount > 0) {
			Touch myTouch = Input.touches [0]; // Single touch game.
			if (myTouch.phase == TouchPhase.Began)
				touchOrigin = myTouch.rawPosition;
			else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
				print ("Ended!");
				Vector2 touchEnded = myTouch.position;
				float x = touchOrigin.x - touchEnded.x;
				float y = touchOrigin.y - touchEnded.y;

				if (Mathf.Abs (x) > Mathf.Abs (y))
					horizontal = x > 0 ? -1 : 1;
				else
					vertical = y > 0 ? -1 : 1;
			}
		}

		#endif

		if (horizontal != 0 || vertical != 0)
			AttempToMove<Wall> (horizontal, vertical);
	}

	protected override void AttempToMove<T> (int xDir, int yDir)
	{
		food--;
		foodText.text = "Food " + food;

		base.AttempToMove<T> (xDir, yDir);

		RaycastHit2D hit;
		if (Move (xDir, yDir, out hit)) {
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		}

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
	}

	private void CheckIfGameOver ()
	{
		if (food <= 0) {
			GameManager.instance.GameOver ();
			SoundManager.instance.PlaySingle (gameOverSound);
			SoundManager.instance.musicSource.Stop ();
		}
	}

	protected override void OnCantMove<T> (T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamge);
		animator.SetTrigger ("PlayerChop");
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Exit") {
			enabled = false;
		} else if (other.tag == "Food") {
			food += pointPerFood;
			other.gameObject.SetActive (false);
			foodText.text = string.Format ("+{0} Food: {1}", pointPerFood, food);
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
		} else if (other.tag == "Soda") {
			food += pointPerSoda;
			other.gameObject.SetActive (false);
			foodText.text = string.Format ("+{0} Food: {1}", pointPerSoda, food);
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
		}
	}

	void OnDisable() // Save player food points after playing a level
	{
		GameManager.instance.playerFoodPoints = food;
		Invoke ("Restart", restartLevelDelay);
	}

	public void LoseFood (int loss)
	{
		animator.SetTrigger ("PlayerHit");
		food -= loss;
		foodText.text = string.Format ("-{0} Food: {1}", loss, food);
		CheckIfGameOver ();
	}

	private void Restart ()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

}
