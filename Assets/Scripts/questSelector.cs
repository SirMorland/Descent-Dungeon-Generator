using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class questSelector : MonoBehaviour {
    public GameObject startButton;
    public Sprite[] startButtonSprites;

	public GameObject storage;

    public GameObject[] expansionsBoxes;

	Dictionary<string, string> campaignsByExpansion = new Dictionary<string, string>();
	Dictionary<string, string> questsByCampaign = new Dictionary<string, string>();

	public GameObject campaignLayout;
    public List<GameObject> campaigns = new List<GameObject>();
    public List<GameObject> selectedCampaigns = new List<GameObject>();

    public GameObject expansionCampaignsViewer;
    public GameObject selectedCampaignsViewer;

    public Transform lockedQuest;

	WebClient client = new WebClient();
	string[] allData;

	void Start () {
		try
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			client.Encoding = System.Text.Encoding.UTF8;
			allData = client.DownloadString("https://boardgamegeek.com/thread/printerfriendly/1570101").Split(new string[]{"<td>"}, System.StringSplitOptions.None);
			PlayerPrefs.SetString("data",string.Join("<td>",allData));
		}
		catch
		{
			allData = PlayerPrefs.GetString("data").Split(new string[]{"<td>"}, System.StringSplitOptions.None);
		}

        storage.GetComponent<storage>().allData = allData;

		for (int i = 2; i < allData.Length; i++)
		{
			string[] oneQuest = allData [i].Split (new string[] { "</div>" }, System.StringSplitOptions.None) [1].Split (new string[] { "</td>" }, System.StringSplitOptions.None) [0].Split (new string[] { "<br />" }, System.StringSplitOptions.None);

			if (oneQuest [0].Trim () == "EC")
			{
				string questName = "";
				string campaignName = "";
				string expansionName = "";

				for (int j = 0; j < oneQuest.Length; j++)
				{
					if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "campaign")
					{
						if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2) campaignName = oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim();
					}
					if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "expansion")
					{
						if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2) expansionName = oneQuest[j].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim();
					}
					if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "name")
					{
						if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2) questName = oneQuest[j].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim();
					}
				}

				if (!questsByCampaign.ContainsKey(questName)) questsByCampaign.Add(questName, campaignName);
				if (!campaignsByExpansion.ContainsKey(campaignName)) campaignsByExpansion.Add(campaignName, expansionName);
			}
		}
	}

	void Update ()
    {
        if ((float)Screen.width / (float)Screen.height <= 1.6)
            Camera.main.orthographicSize = 8f / ((float)Screen.width / (float)Screen.height);
        else
            Camera.main.orthographicSize = 5f;
        
        foreach (GameObject box in expansionsBoxes)
        {
            if (Vector3.Distance(new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y , 0f), box.transform.position) < 1 && Input.GetMouseButtonUp(0))
            {
                ShowCampaignsFromExpansion(box.name);
            }
        }

        foreach (Transform child in expansionCampaignsViewer.transform.GetChild(2).transform)
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < child.position.x + 2.25 &&
                Camera.main.ScreenToWorldPoint(Input.mousePosition).x > child.position.x - 2.25 &&
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y < child.position.y &&
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y > child.position.y - 1 && Input.GetMouseButtonDown(0))
            {
                lockedQuest = child;
                lockedQuest.GetComponent<SpriteRenderer>().sortingOrder = 1;
                lockedQuest.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f, 1f);
            }
        }
        foreach (Transform child in selectedCampaignsViewer.transform.GetChild(2).transform)
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < child.position.x + 2.25 &&
                Camera.main.ScreenToWorldPoint(Input.mousePosition).x > child.position.x - 2.25 &&
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y < child.position.y &&
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y > child.position.y - 1 && Input.GetMouseButtonDown(0))
            {
                lockedQuest = child;
                lockedQuest.GetComponent<SpriteRenderer>().sortingOrder = 1;
                lockedQuest.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f, 1f);
            }
        }

        if (lockedQuest != null)
        {
            lockedQuest.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0.5f, 10f);

            if (lockedQuest.position.x <= 2.25)
            {
                if (lockedQuest.parent.transform.parent == selectedCampaignsViewer.transform)
                {
                    lockedQuest.parent = expansionCampaignsViewer.transform.GetChild(2).transform;
                    selectedCampaigns.Remove(lockedQuest.gameObject);
                    campaigns.Add(lockedQuest.gameObject);

                    RefreshShowCampaignsFromExpansions();
                }
            }
            if (lockedQuest.position.x > 2.25)
            {
                if (lockedQuest.parent.transform.parent == expansionCampaignsViewer.transform)
                {
                    lockedQuest.parent = selectedCampaignsViewer.transform.GetChild(2).transform;
                    campaigns.Remove(lockedQuest.gameObject);
                    selectedCampaigns.Add(lockedQuest.gameObject);

                    RefreshShowCampaignsFromExpansions();
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && lockedQuest != null)
        {
            lockedQuest.GetComponent<SpriteRenderer>().sortingOrder = -2;
            lockedQuest.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            lockedQuest = null;
            RefreshShowCampaignsFromExpansions();
        }

        if (selectedCampaigns.Count > 0)
        {
            startButton.GetComponent<SpriteRenderer>().sprite = startButtonSprites[0];

            if (Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 10f), startButton.transform.position) <= 1.0 && Input.GetMouseButtonUp(0))
            {
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            startButton.GetComponent<SpriteRenderer>().sprite = startButtonSprites[1];
        }
	}

    void RefreshShowCampaignsFromExpansions()
    {
        if (campaigns.Count > 0)
        {
            for (int i = 0; i < campaigns.Count; i++)
            {
                if(campaigns[i].transform != lockedQuest) campaigns[i].transform.localPosition = new Vector3(0f, -i, 0f);
            }
            expansionCampaignsViewer.transform.GetChild(1).transform.localPosition = new Vector3(0f, -campaigns.Count - 0.3f, 0f);
        }
        if (campaigns.Count == 0)  expansionCampaignsViewer.transform.GetChild(1).transform.localPosition = new Vector3(0f, -1.3f, 0f);

        if (selectedCampaigns.Count > 0)
        {
            for (int i = 0; i < selectedCampaigns.Count; i++)
            {
                if(selectedCampaigns[i].transform != lockedQuest) selectedCampaigns[i].transform.localPosition = new Vector3(0f, -i, 0f);
            }
            selectedCampaignsViewer.transform.GetChild(1).transform.localPosition = new Vector3(0f, -selectedCampaigns.Count - 0.3f, 0f);
        }
        if (selectedCampaigns.Count == 0)  selectedCampaignsViewer.transform.GetChild(1).transform.localPosition = new Vector3(0f, -1.3f, 0f);
    }

    void ShowCampaignsFromExpansion (string expansion)
	{
        foreach (Transform child in expansionCampaignsViewer.transform.GetChild(2).transform)
        {
            Destroy(child.gameObject);
        }

        campaigns.Clear();

        foreach (string campaign in campaignsByExpansion.Keys)
        {
            if (campaignsByExpansion[campaign] == expansion)
            {
                GameObject newCampaign = Instantiate(campaignLayout);
                campaigns.Add(newCampaign);
                newCampaign.transform.parent = expansionCampaignsViewer.transform.GetChild(2).transform;
                newCampaign.transform.localScale = new Vector3(1f, 1f, 1f);
                newCampaign.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = campaign;
            }
        }

        if (campaigns.Count > 0)
        {
            expansionCampaignsViewer.transform.localScale = new Vector3(1f, 1f, 1f);

            for (int i = 0; i < campaigns.Count; i++)
            {
                campaigns[i].transform.localPosition = new Vector3(0f, -i, 0f);
            }

            expansionCampaignsViewer.transform.GetChild(1).transform.localPosition = new Vector3(0f, -campaigns.Count - 0.3f, 0f);
        }
        if (campaigns.Count == 0)
        {
            expansionCampaignsViewer.transform.localScale = new Vector3(1f, 1f, 1f);

            expansionCampaignsViewer.transform.GetChild(1).transform.localPosition = new Vector3(0f, -1.3f, 0f);
        }
    }
}
