using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class storage : MonoBehaviour {
    public string[] allData;

    public  List<string> selectedCampaigns = new List<string>();

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
