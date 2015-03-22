using UnityEngine;
using System.Collections;

public class ExitSound : MonoBehaviour {

	public AudioClip exitSound;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player")
			SoundManager.instance.PlaySingle (exitSound);
	}
}
