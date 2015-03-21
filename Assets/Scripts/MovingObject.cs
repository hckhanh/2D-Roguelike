using UnityEngine;
using System.Collections;

public abstract	class MovingObject : MonoBehaviour
{

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;
	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start ()
	{
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1 / moveTime;
	}

	protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
	{
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);

		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
			StartCoroutine (SmothMovement (end));
			return true;
		} else
			return false;
	}

	protected IEnumerator SmothMovement (Vector3 end)
	{
		float sqrRemainDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			sqrRemainDistance = (transform.position - end).sqrMagnitude;
			yield return null; // Wait for a frame before evaluating the condition of the loop.
		}
	}

	protected virtual void AttempToMove<T>(int xDir, int yDir)
		where T : Component
	{
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);
		 
		if (hit.transform == null)
			return;

		T hitComponent = hit.transform.GetComponent<T> ();
		if (!canMove && hitComponent != null)
			OnCantMove (hitComponent);
	}

	protected abstract void OnCantMove<T> (T component)
		where T : Component;

}
