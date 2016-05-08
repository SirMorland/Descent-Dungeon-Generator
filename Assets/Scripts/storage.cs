using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class storage : MonoBehaviour {
    public Dictionary<string, bool> expansions = new Dictionary<string, bool>();

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);

		expansions.Add("baseGame", false);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
