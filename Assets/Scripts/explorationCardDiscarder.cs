using UnityEngine;
using System.Collections;

public class explorationCardDiscarder : MonoBehaviour {
	public Sprite[] explorationCardColor;
	public Sprite[] hourglass;
    public Sprite[] types;
    public Sprite[] typesBlack;
	public bool isActive;

	// Use this for initialization
	void Start () {
		isActive = false;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.main.ScreenToWorldPoint (Input.mousePosition).x < transform.position.x + GetComponent<SpriteRenderer> ().sprite.bounds.size.x * transform.localScale.x / 2f &&
			Camera.main.ScreenToWorldPoint (Input.mousePosition).x > transform.position.x - GetComponent<SpriteRenderer> ().sprite.bounds.size.x * transform.localScale.x / 2f &&
			Camera.main.ScreenToWorldPoint (Input.mousePosition).y < transform.position.y + GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y / 2f &&
			Camera.main.ScreenToWorldPoint (Input.mousePosition).y > transform.position.y - GetComponent<SpriteRenderer> ().sprite.bounds.size.y * transform.localScale.y / 2f &&
		   Input.GetMouseButtonDown (0))
		{
			bool currentActiveStatus = isActive;
			if (currentActiveStatus == true)
			{
				isActive = false;
				GetComponent<SpriteRenderer> ().sprite = explorationCardColor [1];
                transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite = hourglass[1];
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i] == transform.GetChild(3).GetComponent<SpriteRenderer>().sprite) transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = typesBlack[i];
                }
			}
			if (currentActiveStatus == false)
			{
				isActive = true;
				GetComponent<SpriteRenderer> ().sprite = explorationCardColor [0];
                transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite = hourglass[0];
                for (int i = 0; i < typesBlack.Length; i++)
                {
                    if (typesBlack[i] == transform.GetChild(3).GetComponent<SpriteRenderer>().sprite) transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = types[i];
                }
			}
		}
	
	}
}
