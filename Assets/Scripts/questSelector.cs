using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class questSelector : MonoBehaviour {
	public GameObject storage;

	Dictionary<string, string> campaignsByExpansion = new Dictionary<string, string>();
	Dictionary<string, string> questsByCampaign = new Dictionary<string, string>();

	public GameObject campaignLayout;
	List<GameObject> campaigns = new List<GameObject>();

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
        
	}

    void RefreshShowCampaignsFromExpansions()
    {
        foreach (string expansion in storage.GetComponent<storage>().expansions.Keys)
        {
            if (storage.GetComponent<storage>().expansions[expansion] == true) ShowCampaignsFromExpansion(expansion);
        }
    }

    void ShowCampaignsFromExpansion (string expansion)
	{
		GameObject newCampaign = Instantiate(campaignLayout);
		campaigns.Add(newCampaign);
	}
}
