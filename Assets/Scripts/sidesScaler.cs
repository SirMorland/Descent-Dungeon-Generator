using UnityEngine;
using System.Collections;

public class sidesScaler : MonoBehaviour {
    public GameObject par;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = par.transform.localPosition / 2f;
        transform.localScale = new Vector3(1f, par.transform.localPosition.y / 4.2f, 1f);
	}
}
