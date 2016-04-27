using UnityEngine;
using System.Collections;

public class mapZoomer : MonoBehaviour {
	public Vector3 oldPos;
	public float distance;
	public float oldDistance;

	public float test;

	// Use this for initialization
	void Start () {
		distance = 1f;
		oldDistance = 1f;
	
	}
	
	// Update is called once per frame
	void Update () {
		test = GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y;

		if (Input.touchCount == 1) {
			if (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x < transform.position.x + GetComponent<SpriteRenderer> ().sprite.bounds.size.x * transform.localScale.x / 2f &&
			   Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x > transform.position.x - GetComponent<SpriteRenderer> ().sprite.bounds.size.x * transform.localScale.x / 2f &&
			   Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y < transform.position.y + GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y / 2f &&
			   Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y > transform.position.y - GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y / 2f) {
				if (Input.GetTouch (0).phase == TouchPhase.Began) {
					oldPos = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
				}
				if (Input.GetTouch (0).phase == TouchPhase.Moved) {
					transform.Translate (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position) - oldPos);
				}
			}
		}

		if (Input.touchCount == 2)
		{
			distance = Vector3.Distance (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), Camera.main.ScreenToWorldPoint (Input.GetTouch (1).position));
			if (Input.GetTouch (0).phase == TouchPhase.Began || Input.GetTouch (1).phase == TouchPhase.Began) oldDistance = distance;
			if (Input.GetTouch (0).phase == TouchPhase.Ended) oldPos = Camera.main.ScreenToWorldPoint (Input.GetTouch (1).position);
			if (Input.GetTouch (1).phase == TouchPhase.Ended) oldPos = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
		}

		if (transform.position.x < -8 + GetComponent<SpriteRenderer> ().sprite.bounds.size.x * transform.localScale.x / 2f)
			transform.position = new Vector3 (-8f + GetComponent<SpriteRenderer> ().sprite.bounds.size.x * transform.localScale.x / 2f, transform.position.y, 0f);
		if (transform.position.x > 8 - GetComponent<SpriteRenderer> ().sprite.bounds.size.x * transform.localScale.x / 2f)
			transform.position = new Vector3 (8f - GetComponent<SpriteRenderer> ().sprite.bounds.size.x * transform.localScale.x / 2f, transform.position.y, 0f);
		if (transform.position.y < -5 + GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y / 2f)
			transform.position = new Vector3 (transform.position.x, -5f + GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y / 2f, 0f);
		if (transform.position.y > 5 - GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y / 2f)
			transform.position = new Vector3 (transform.position.x, 5f - GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y / 2f, 0f);

		transform.localScale += new Vector3((distance - oldDistance)/distance, (distance-oldDistance)/distance, 0f);
		if (transform.localScale.x < 1) transform.localScale = new Vector3 (1f, 1f, 1f);
		if (GetComponent<SpriteRenderer> ().sprite.bounds.size.x * transform.localScale.x > 16) transform.localScale = new Vector3 (16f / GetComponent<SpriteRenderer> ().sprite.bounds.size.x, 16f / GetComponent<SpriteRenderer> ().sprite.bounds.size.x, 1f);
		if (GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y > 10) transform.localScale = new Vector3 (10f / GetComponent<SpriteRenderer> ().sprite.bounds.size.y, 10f / GetComponent<SpriteRenderer> ().sprite.bounds.size.y, 1f);

		if(Input.touchCount == 1) oldPos = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
		oldDistance = distance;

		//transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.x, 1f);
	}
}
