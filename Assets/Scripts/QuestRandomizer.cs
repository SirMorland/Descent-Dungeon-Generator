using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuestRandomizer : MonoBehaviour {

    #region Attribuutit

    int createdRooms;

    public GameObject exploreButton;
    public Sprite blackExploreButton;
    public GameObject activateButton;
    public GameObject backButton;

    GameObject firstRoom;
    public GameObject firstRoomOut;
    public GameObject firstRoomIn;
    public GameObject entrance;
    public GameObject exit;
    public GameObject door;

    public GameObject map;
    public GameObject explorationCard;
    public GameObject type;
    public GameObject afterSetup;
    public GameObject hourglass;
    public GameObject twoActions;

    bool finale;
    bool inMainScreen;

    public List<string> mapTraits;
    public List<Vector3> spawn1x1;
    public List<Vector3> spawn2x1;
    public List<Vector3> spawn2x2;
    public List<Vector3> spawn3x2;

    public List<GameObject> monsters;

    public Sprite[] playerNumber;
    public Sprite[] types;
    public string currentMonster;

    public GameObject nameText;
    public GameObject questText;
    public GameObject afterSetupText;
    public GameObject firstActionText;
    public GameObject hourglassTimeText;
    public GameObject hourglassOverlordTurnText;
    public GameObject overlordTurnText;

    public Sprite explorationCardColor;
    public Sprite hourglassColor;

    Dictionary<string, string> questsByType = new Dictionary<string, string>();

    public List<string> normalQuests = new List<string>();
    public List<string> startingQuests = new List<string>();
    public List<string> firstQuests = new List<string>();
    public List<string> secondQuests = new List<string>();
    public List<string> thirdQuests = new List<string>();

    public string currentQuest;

    public List<GameObject> tokens;

    Dictionary<string,string> monstersAttackType = new Dictionary<string, string> ();

    public Sprite deadEndIn;
    public Sprite deadEndOut;

    public List<Sprite> mapTiles;

    public List<GameObject> activateCards;
    public GameObject[] allActivateCards;
    public Sprite[] rangedMoves;
    public Sprite[] meleeMoves;
    public Sprite[] rangedAttacks;
    public Sprite[] meleeAttacks;

    public GameObject infoText;

    Vector3 oldPos;
    public float speed;

    string[] allData;
    List<string> selectedCampaigns;

    #endregion

	void Start () 
    {
        createdRooms = 0;
        finale = false;
        inMainScreen = true;

        monstersAttackType.Add ("Dragon", "melee");
        monstersAttackType.Add ("Elemental", "ranged");
        monstersAttackType.Add ("Ettin", "melee");
        monstersAttackType.Add ("Merriod", "melee");
        monstersAttackType.Add ("Wolf", "melee");
        monstersAttackType.Add ("Flesh", "ranged");
        monstersAttackType.Add ("Spider", "melee");
        monstersAttackType.Add ("Goblin", "ranged");
        monstersAttackType.Add ("Zombie", "melee");
        monstersAttackType.Add ("Lieutenant-Token-1", "melee");
        monstersAttackType.Add ("Lieutenant-Token-2", "melee");
        monstersAttackType.Add ("Lieutenant-Token-3", "melee");
        monstersAttackType.Add ("Lieutenant-Token-4", "ranged");
        monstersAttackType.Add ("Lieutenant-Token-5", "melee");
        monstersAttackType.Add ("Lieutenant-Token-6", "ranged");

        allData = GameObject.FindGameObjectWithTag("storage").GetComponent<storage>().allData;
        selectedCampaigns = GameObject.FindGameObjectWithTag("storage").GetComponent<storage>().selectedCampaigns;

        MakeQuests();
	}
	
    void Update ()
    {
        if ((float)Screen.width / (float)Screen.height <= 1.6) Camera.main.orthographicSize = 8f / ((float)Screen.width / (float)Screen.height);
        else Camera.main.orthographicSize = 5f;

        if (createdRooms > 0)  infoText.GetComponent<Text>().text = "ROOM: " + createdRooms + "\nTILE: " + map.GetComponent<SpriteRenderer>().sprite.name;
        else infoText.GetComponent<Text>().text = "";

        if (Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 10f), exploreButton.transform.position) <= 1
            && Input.GetMouseButtonUp(0) && finale == true && explorationCard.GetComponent<explorationCardDiscarder>().isActive == false)
        {
            Destroy(GameObject.FindGameObjectWithTag("storage"));
            SceneManager.LoadScene(0);
        }
        if (Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 10f), exploreButton.transform.position) <= 1
            && Input.GetMouseButtonUp(0) && finale == false && explorationCard.GetComponent<explorationCardDiscarder>().isActive == false)
        {
            foreach (Transform child in map.transform)
            {
                Destroy(child.gameObject);
            }
            map.transform.position = new Vector3(-3f, 0f, 0f);
            map.transform.localScale = new Vector3(1f, 1f, 1f);
            mapTraits.Clear();
            spawn1x1.Clear();
            spawn2x1.Clear();
            spawn2x2.Clear();
            spawn3x2.Clear();
            currentMonster = "";
            currentQuest = "";
            nameText.GetComponent<Text>().text = "";
            questText.GetComponent<Text>().text = "";
            afterSetupText.GetComponent<Text>().text = "";
            firstActionText.GetComponent<Text>().text = "";
            hourglassTimeText.GetComponent<Text>().text = "";
            hourglassOverlordTurnText.GetComponent<Text>().text = "";
            overlordTurnText.GetComponent<Text>().text = "";
            type.SetActive(false);
            afterSetup.SetActive(false);
            hourglass.SetActive(false);
            twoActions.SetActive(false);

            createdRooms++;

            if (createdRooms == 1)
            {
                MakeMapFromQuest(startingQuests[0]);
            }
            else
            {
                if (firstQuests.Count == 0 && secondQuests.Count == 0 && thirdQuests.Count > 0)
                {
                    int randomNumber = Random.Range(0, thirdQuests.Count);
                    MakeMapFromQuest(thirdQuests[randomNumber]);
                    thirdQuests.RemoveAt(randomNumber);
                    if (thirdQuests.Count == 0)
                    {
                        finale = true;
                        exploreButton.GetComponent<SpriteRenderer> ().sprite = blackExploreButton;
                    }
                }
                if (firstQuests.Count == 0 && secondQuests.Count > 0)
                {
                    int randomNumber = Random.Range(0, secondQuests.Count);
                    MakeMapFromQuest(secondQuests[randomNumber]);
                    secondQuests.RemoveAt(randomNumber);
                }
                if (firstQuests.Count > 0)
                {
                    int randomNumber = Random.Range(0, firstQuests.Count);
                    MakeMapFromQuest(firstQuests[randomNumber]);
                    firstQuests.RemoveAt(randomNumber);
                }
            }

            if (map.GetComponent<SpriteRenderer> ().sprite.name.Substring(map.GetComponent<SpriteRenderer> ().sprite.name.Length - 1) == "A") firstRoom = firstRoomOut;
            if (map.GetComponent<SpriteRenderer> ().sprite.name.Substring(map.GetComponent<SpriteRenderer> ().sprite.name.Length - 1) == "B") firstRoom = firstRoomIn;

            MakeProps();
            MakeMonsters();
            MakeTokensFromWWW();
            MakeExplorationCard();
            MakeQuestFromWWW(currentQuest);
            MakeActivationCardLayouts();
        }

        if (Vector3.Distance (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f), activateButton.transform.position) <= 1.0f
            && Input.GetMouseButtonUp (0)) {
            inMainScreen = false;
            speed = 0f;
            backButton.transform.position = new Vector3 (-7f, backButton.transform.position.y, backButton.transform.position.z);

            for (int i = 0; i < activateCards.Count; i++) {
                activateCards [i].transform.position = new Vector3 (i / 2 * 8f, -12.5f - i % 2 * 5f, 0f);
            }
            MakeActivationCardTexts ();
        }
        if (Vector3.Distance (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f), backButton.transform.position) <= 1.0f
            && Input.GetMouseButtonUp (0))
            inMainScreen = true;

        if (inMainScreen == true && Camera.main.transform.position.y < 0) {
            Camera.main.transform.Translate (0f, 50f * Time.deltaTime, 0f);
            if (Camera.main.transform.position.y > 0)
                Camera.main.transform.position = new Vector3 (0f, 0f, -10f);
        }

        if (inMainScreen == false && Camera.main.transform.position.y > -15) {
            Camera.main.transform.Translate (0f, -50f * Time.deltaTime, 0f);
            if (Camera.main.transform.position.y < -15)
                Camera.main.transform.position = new Vector3 (0f, -15f, -10f);
        }

        if (Camera.main.transform.position.y == -15)
        {
            if (Input.GetMouseButtonDown(0))
                oldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButton(0))
            {
                speed = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - oldPos.x;

                backButton.transform.position = new Vector3(backButton.transform.position.x + Camera.main.ScreenToWorldPoint(Input.mousePosition).x - oldPos.x, backButton.transform.position.y, backButton.transform.position.z);

                foreach (GameObject activateCard in activateCards)
                {
                    activateCard.transform.position = new Vector3(activateCard.transform.position.x + Camera.main.ScreenToWorldPoint(Input.mousePosition).x - oldPos.x, activateCard.transform.position.y, activateCard.transform.position.z);
                }
            }

            speed *= 0.95f;

            if (!Input.GetMouseButton(0) && speed != 0)
            {
                backButton.transform.Translate(new Vector3(speed, 0f, 0f));

                for (int i = 0; i < activateCards.Count; i++)
                {
                    activateCards[i].transform.Translate(new Vector3(speed, 0f, 0f));
                }
            }

            if (backButton.transform.position.x > -7)
            {
                backButton.transform.position = new Vector3(-7f, backButton.transform.position.y, backButton.transform.position.z);

                for (int i = 0; i < activateCards.Count; i++)
                {
                    activateCards[i].transform.position = new Vector3(i / 2 * 8f, -12.5f - i % 2 * 5f, 0f);
                }
            }

            if (activateCards.Count <= 2 && backButton.transform.position.x < -7)
            {
                backButton.transform.position = new Vector3(-7f, backButton.transform.position.y, backButton.transform.position.z);

                for (int i = 0; i < activateCards.Count; i++)
                {
                    activateCards[i].transform.position = new Vector3(i / 2 * 8f, -12.5f - i % 2 * 5f, 0f);
                }
            }

            int helpNumber = -(activateCards.Count + 1) / 2 * 8 + 5;

            if (activateCards.Count >= 3 && backButton.transform.position.x < helpNumber)
            {
                backButton.transform.position = new Vector3(helpNumber, backButton.transform.position.y, backButton.transform.position.z);

                for (int i = 0; i < activateCards.Count; i++)
                {
                    activateCards[i].transform.position = new Vector3(i / 2 * 8f + helpNumber + 7f, -12.5f - i % 2 * 5f, 0f);
                }
            }

            oldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void MakeQuests()
    {
        for (int i = 2; i < allData.Length; i++)
        {
            string[] oneQuest = allData [i].Split (new string[] { "</div>" }, System.StringSplitOptions.None) [1].Split (new string[] { "</td>" }, System.StringSplitOptions.None) [0].Split (new string[] { "<br />" }, System.StringSplitOptions.None);

            if (oneQuest [0].Trim () == "EC")
            {
                string questName = "";
                string questType = "";

                for (int j = 0; j < oneQuest.Length; j++)
                {
                    if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "campaign")
                    {
                        if (oneQuest[j].Split(new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                        {
                            if (selectedCampaigns.Contains(oneQuest[j].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim()))
                            {
                                for (int k = 0; k < oneQuest.Length; k++)
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "name")
                                    {
                                        if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2) questName = oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim();
                                    }
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "type")
                                    {
                                        if (oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2) questType = oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim();
                                    }
                                }
                            }
                        }
                    }
                }
                if (!questsByType.ContainsKey(questName) && questName != "" && questType != "") questsByType.Add(questName, questType);
            }
        }

        foreach (string key in questsByType.Keys)
        {
            if (questsByType[key] == "normal") normalQuests.Add(key);
            if (questsByType[key] == "starting") startingQuests.Add(key);
            if (questsByType[key] == "1") firstQuests.Add(key);
            if (questsByType[key] == "2") secondQuests.Add(key);
            if (questsByType[key] == "3") thirdQuests.Add(key);
        }

        if (thirdQuests.Count == 0)
        {
            int randomNumber = Random.Range(0, normalQuests.Count);
            thirdQuests.Add(normalQuests[randomNumber]);
            if (normalQuests.Count > 8)  normalQuests.RemoveAt(randomNumber);
        }
        if (secondQuests.Count == 0)
        {
            int randomNumber = Random.Range(0, normalQuests.Count);
            secondQuests.Add(normalQuests[randomNumber]);
            if (normalQuests.Count > 8)  normalQuests.RemoveAt(randomNumber);
        }
        if (firstQuests.Count == 0)
        {
            int randomNumber = Random.Range(0, normalQuests.Count);
            firstQuests.Add(normalQuests[randomNumber]);
            if (normalQuests.Count > 8)  normalQuests.RemoveAt(randomNumber);
        }
        if (startingQuests.Count == 0)
        {
            int randomNumber = Random.Range(0, normalQuests.Count);
            startingQuests.Add(normalQuests[randomNumber]);
            if (normalQuests.Count > 8)  normalQuests.RemoveAt(randomNumber);
        }

        Shuffle(startingQuests);
        Shuffle(firstQuests);
        Shuffle(secondQuests);
        Shuffle(thirdQuests);

        if (startingQuests.Count > 1) startingQuests.RemoveRange(1, startingQuests.Count - 1);
        if (firstQuests.Count > 1) firstQuests.RemoveRange(1, firstQuests.Count - 1);
        if (secondQuests.Count > 1) secondQuests.RemoveRange(1, secondQuests.Count - 1);
        if (thirdQuests.Count > 1) thirdQuests.RemoveRange(1, thirdQuests.Count - 1);

        int addedQuest = 0;

        for(int i = 0; i < 2; i++)
        {
            int randomNumber = Random.Range(0, normalQuests.Count);
            thirdQuests.Add(normalQuests[randomNumber]);
            addedQuest++;
            if (normalQuests.Count >= 9 - addedQuest)  normalQuests.RemoveAt(randomNumber);
        }
        for(int i = 0; i < 3; i++)
        {
            int randomNumber = Random.Range(0, normalQuests.Count);
            secondQuests.Add(normalQuests[randomNumber]);
            addedQuest++;
            if (normalQuests.Count >= 9 - addedQuest)  normalQuests.RemoveAt(randomNumber);
        }
        for(int i = 0; i < 3; i++)
        {
            int randomNumber = Random.Range(0, normalQuests.Count);
            firstQuests.Add(normalQuests[randomNumber]);
            addedQuest++;
            if (normalQuests.Count >= 9 - addedQuest)  normalQuests.RemoveAt(randomNumber);
        }
    }

    void Shuffle(List<string> aList)
    {
        List<string> newList = new List<string>();
        newList.AddRange(aList);
        int i = 0;
        while (newList.Count > 0)
        {
            int randomNumber = Random.Range(0, newList.Count);
            aList[i] = newList[randomNumber];
            newList.RemoveAt(randomNumber);
        }
    }

    void MakeMapFromQuest(string quest)
    {
        currentQuest = quest;

        for (int i = 2; i < allData.Length; i++)
        {
            string[] oneQuest = allData [i].Split (new string[] { "</div>" }, System.StringSplitOptions.None) [1].Split (new string[] { "</td>" }, System.StringSplitOptions.None) [0].Split (new string[] { "<br />" }, System.StringSplitOptions.None);

            if (oneQuest [0].Trim () == "EC")
            {
                for (int j = 0; j < oneQuest.Length; j++)
                {
                    if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "name")
                    {
                        if (oneQuest[j].Split(new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                        {
                            if (currentQuest == oneQuest[j].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim())
                            {
                                for (int k = 0; k < oneQuest.Length; k++)
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "tile")
                                    {
                                        if (oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                        {
                                            foreach (Sprite mapTile in mapTiles)
                                            {
                                                if (mapTile.name == oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim())
                                                {
                                                    map.GetComponent<SpriteRenderer>().sprite = mapTile;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }   

    void MakeProps ()
    {
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "01-A")
        {
            MakeDoorsFor01A ();
            MakeListsFor01A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "01-B")
        {
            MakeDoorsFor01B ();
            MakeListsFor01B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "02-A")
        {
            MakeDoorsFor02A ();
            MakeListsFor02A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "02-B")
        {
            MakeDoorsFor02A ();
            MakeListsFor02B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "03-A")
        {
            MakeDoorsFor03A ();
            MakeListsFor03A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "03-B")
        {
            MakeDoorsFor03A ();
            MakeListsFor03B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "04-A")
        {
            MakeDoorsFor04A ();
            MakeListsFor04A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "04-B")
        {
            MakeDoorsFor04B ();
            MakeListsFor04B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "05-A")
        {
            MakeDoorsFor03A ();
            MakeListsFor03B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "05-B")
        {
            MakeDoorsFor03A ();
            MakeListsFor03B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "06-A")
        {
            MakeDoorsFor06A ();
            MakeListsFor06A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "06-B")
        {
            MakeDoorsFor06A ();
            MakeListsFor06B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "07-A")
        {
            MakeDoorsFor07A ();
            MakeListsFor07A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "07-B")
        {
            MakeDoorsFor07B ();
            MakeListsFor07B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "08-A")
        {
            MakeDoorsFor08A ();
            MakeListsFor08A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "08-B")
        {
            MakeDoorsFor08B ();
            MakeListsFor08B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "09-A")
        {
            MakeDoorsFor09A ();
            MakeListsFor09A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "09-B")
        {
            MakeDoorsFor09A ();
            MakeListsFor09A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "10-A")
        {
            MakeDoorsFor10A ();
            MakeListsFor10A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "10-B")
        {
            MakeDoorsFor10A ();
            MakeListsFor10A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "11-A")
        {
            MakeDoorsFor10A ();
            MakeListsFor10A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "11-B")
        {
            MakeDoorsFor10A ();
            MakeListsFor10A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "12-A")
        {
            MakeDoorsFor12A ();
            MakeListsFor12A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "12-B")
        {
            MakeDoorsFor12B ();
            MakeListsFor12B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "13-A")
        {
            MakeDoorsFor13A ();
            MakeListsFor13A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "13-B")
        {
            MakeDoorsFor13B ();
            MakeListsFor13B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "14-A")
        {
            MakeDoorsFor10A ();
            MakeListsFor14A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "14-B")
        {
            MakeDoorsFor10A ();
            MakeListsFor14B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "15-A")
        {
            MakeDoorsFor15A ();
            MakeListsFor15A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "15-B")
        {
            MakeDoorsFor15B ();
            MakeListsFor15A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "16-A")
        {
            MakeDoorsFor16A ();
            MakeListsFor16A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "16-B")
        {
            MakeDoorsFor16A ();
            MakeListsFor16A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "17-A")
        {
            MakeDoorsFor17A ();
            MakeListsFor17A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "17-B")
        {
            MakeDoorsFor17B ();
            MakeListsFor17B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "18-A")
        {
            MakeDoorsFor17A ();
            MakeListsFor17A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "18-B")
        {
            MakeDoorsFor17B ();
            MakeListsFor17B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "19-A")
        {
            MakeDoorsFor19A ();
            MakeListsFor19A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "19-B")
        {
            MakeDoorsFor19B ();
            MakeListsFor19B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "20-A")
        {
            MakeDoorsFor20A ();
            MakeListsFor20A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "20-B")
        {
            MakeDoorsFor20B ();
            MakeListsFor20B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "21-A")
        {
            MakeDoorsFor19B ();
            MakeListsFor21A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "21-B")
        {
            MakeDoorsFor19A ();
            MakeListsFor21B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "22-A")
        {
            MakeDoorsFor06A ();
            MakeListsFor22A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "22-B")
        {
            MakeDoorsFor06A ();
            MakeListsFor22A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "23-A")
        {
            MakeDoorsFor23A ();
            MakeListsFor23A();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "23-B")
        {
            MakeDoorsFor23A ();
            MakeListsFor23A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "24-A")
        {
            MakeDoorsFor24A ();
            MakeListsFor24A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "24-B")
        {
            MakeDoorsFor24B ();
            MakeListsFor24B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "25-A")
        {
            MakeDoorsFor24B ();
            MakeListsFor24B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "25-B")
        {
            MakeDoorsFor24A ();
            MakeListsFor24A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "26-A")
        {
            MakeDoorsFor26A ();
            MakeListsFor23A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "26-B")
        {
            MakeDoorsFor26A ();
            MakeListsFor23A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "27-A")
        {
            MakeDoorsFor27A ();
            MakeListsFor27A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "27-B")
        {
            MakeDoorsFor27B ();
            MakeListsFor27B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "28-A")
        {
            MakeDoorsFor28A ();
            MakeListsFor28A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "28-B")
        {
            MakeDoorsFor28B ();
            MakeListsFor28B ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "29-A")
        {
            MakeDoorsFor10A ();
            MakeListsFor16A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "29-B")
        {
            MakeDoorsFor10A ();
            MakeListsFor16A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "30-A")
        {
            MakeDoorsFor10A ();
            MakeListsFor16A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "30-B")
        {
            MakeDoorsFor10A ();
            MakeListsFor16A ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "Transition-indoor")
        {
            MakeDoorsForTransitionIndoor ();
        }
        if (map.GetComponent<SpriteRenderer> ().sprite.name == "Transition-outdoor")
        {
            MakeDoorsForTransitionIndoor ();
        }

        if (finale == true)
        {
            for(int i = 0; i < map.transform.childCount; i++){
                if (map.transform.GetChild (i).name == "Exit(Clone)") map.transform.GetChild (i).GetComponent<SpriteRenderer> ().sprite = null;
                else if (map.transform.GetChild (i).name == "Yellow-Door(Clone)")
                {
                    if (map.GetComponent<SpriteRenderer> ().sprite.name.Substring(map.GetComponent<SpriteRenderer> ().sprite.name.Length - 1) == "A") map.transform.GetChild (i).GetComponent<SpriteRenderer> ().sprite = deadEndOut;
                    if (map.GetComponent<SpriteRenderer> ().sprite.name.Substring(map.GetComponent<SpriteRenderer> ().sprite.name.Length - 1) == "B") map.transform.GetChild (i).GetComponent<SpriteRenderer> ().sprite = deadEndIn;
                }
            }
        }
    }
        
    #region MakeDoors

    void MakeDoorsFor01A()
    {
        int randomNumber = Random.Range (0, 3);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 3f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 3f, 0f));
            MakeRightDoor ("exit", new Vector3 (4f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -3f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (0f, -3f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (0f, -3f, 0f));
            MakeRightDoor ("exit", new Vector3 (4f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 3f, 0f));
        }
        if (randomNumber  == 2)
        {
            if (createdRooms == 1)
            {
                map.transform.Translate (-1f, 0f, 0f);
                MakeRightDoor ("firstRoom", new Vector3 (4f, 0f, 0f));
            }
            else MakeRightDoor ("entrance", new Vector3 (4f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 3f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -3f, 0f));
        }
    }

    void MakeDoorsFor01B()
    {
        int randomNumber = Random.Range (0, 3);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 3f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 3f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-4f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -3f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (0f, -3f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (0f, -3f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-4f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 3f, 0f));
        }
        if (randomNumber  == 2)
        {
            if (createdRooms == 1)
            {
                map.transform.Translate (1f, 0f, 0f);
                MakeLeftDoor ("firstRoom", new Vector3 (-4f, 0f, 0f));
            }
            else MakeLeftDoor ("entrance", new Vector3 (-4f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 3f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -3f, 0f));
        }
    }

    void MakeDoorsFor02A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 3f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 3f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -3f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (0f, -3f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (0f, -3f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 3f, 0f));
        }
    }

    void MakeDoorsFor03A()
    {
        int randomNumber = Random.Range (0, 3);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-3f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-3f, 0f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -2f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (0f, -2f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (0f, -2f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, 0f, 0f));
        }
        if (randomNumber  == 2)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (3f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (3f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -2f, 0f));
        }
    }

    void MakeDoorsFor04A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 3f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 3f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (3f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (3f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 3f, 0f));
        }
    }

    void MakeDoorsFor04B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 3f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 3f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-3f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-3f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 3f, 0f));
        }
    }

    void MakeDoorsFor06A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (3f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (3f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-3f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-3f, 0f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, 0f, 0f));
        }
    }

    void MakeDoorsFor07A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (1f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (1f, 2f, 0f));
            MakeDownDoor ("exit", new Vector3 (-1f, -2f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (-1f, -2f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (-1f, -2f, 0f));
            MakeUpDoor ("exit", new Vector3 (1f, 2f, 0f));
        }
    }

    void MakeDoorsFor07B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (-1f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (-1f, 2f, 0f));
            MakeDownDoor ("exit", new Vector3 (1f, -2f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (1f, -2f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (1f, -2f, 0f));
            MakeUpDoor ("exit", new Vector3 (-1f, 2f, 0f));
        }
    }

    void MakeDoorsFor08A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 2f, 0f));
            MakeRightDoor ("exit", new Vector3 (2f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (2f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (2f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 2f, 0f));
        }
    }

    void MakeDoorsFor08B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 2f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-2f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-2f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-2f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 2f, 0f));
        }
    }

    void MakeDoorsFor09A()
    {
        int randomNumber = Random.Range (0, 3);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-2f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-2f, 0f, 0f));
            MakeRightDoor ("exit", new Vector3 (2f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 2f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 2f, 0f));
            MakeRightDoor ("exit", new Vector3 (2f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-2f, 0f, 0f));
        }
        if (randomNumber  == 2)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (2f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (2f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-2f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 2f, 0f));
        }
    }

    void MakeDoorsFor10A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (2f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (2f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-2f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-2f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-2f, 0f, 0f));
            MakeRightDoor ("exit", new Vector3 (2f, 0f, 0f));
        }
    }

    void MakeDoorsFor12A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (0f, -3f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (0f, -3f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (3f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (3f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -3f, 0f));
        }
    }

    void MakeDoorsFor12B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (0f, -3f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (0f, -3f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-3f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-3f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -3f, 0f));
        }
    }

    void MakeDoorsFor13A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 3f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 3f, 0f));
            MakeDownDoor ("exit", new Vector3 (-2f, -3f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (-2f, -3f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (-2f, -3f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 3f, 0f));
        }
    }

    void MakeDoorsFor13B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 3f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 3f, 0f));
            MakeDownDoor ("exit", new Vector3 (2f, -3f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (2f, -3f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (2f, -3f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 3f, 0f));
        }
    }

    void MakeDoorsFor15A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 2f, 0f));
            MakeDownDoor ("exit", new Vector3 (-2f, -2f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (-2f, -2f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (-2f, -2f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 2f, 0f));
        }
    }

    void MakeDoorsFor15B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (-1f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (-1f, 2f, 0f));
            MakeDownDoor ("exit", new Vector3 (1f, -2f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (1f, -2f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (1f, -2f, 0f));
            MakeUpDoor ("exit", new Vector3 (-1f, 2f, 0f));
        }
    }

    void MakeDoorsFor16A()
    {
        int randomNumber = Random.Range (0, 4);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-2f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-2f, 0f, 0f));
            MakeRightDoor ("exit", new Vector3 (2f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 2f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -2f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 2f, 0f));
            MakeRightDoor ("exit", new Vector3 (2f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-2f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -2f, 0f));
        }
        if (randomNumber  == 2)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (2f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (2f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-2f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 2f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -2f, 0f));
        }
        if (randomNumber  == 3)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (0f, -2f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (0f, -2f, 0f));
            MakeRightDoor ("exit", new Vector3 (2f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-2f, 0f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 2f, 0f));
        }
    }

    void MakeDoorsFor17A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (-1f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (-1f, 2f, 0f));
            MakeRightDoor ("exit", new Vector3 (2f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (2f, -1f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (2f, -1f, 0f));
            MakeUpDoor ("exit", new Vector3 (-1f, 2f, 0f));
        }
    }

    void MakeDoorsFor17B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (1f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (1f, 2f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-2f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-2f, -1f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-2f, -1f, 0f));
            MakeUpDoor ("exit", new Vector3 (1f, 2f, 0f));
        }
    }

    void MakeDoorsFor19A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (3f, -1f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (3f, -1f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-3f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-3f, 0f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, -1f, 0f));
        }
    }

    void MakeDoorsFor19B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (3f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (3f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-3f, -1f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-3f, -1f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, 0f, 0f));
        }
    }

    void MakeDoorsFor20A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (3f, -2f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (3f, -2f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-4f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1)
            {
                map.transform.Translate (1f, 0f, 0f);
                MakeLeftDoor ("firstRoom", new Vector3 (-4f, -1f, 0f));
            }
            else MakeLeftDoor ("entrance", new Vector3 (-4f, -1f, 0f));
            MakeDownDoor ("exit", new Vector3 (3f, -2f, 0f));
        }
    }

    void MakeDoorsFor20B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (-3f, -2f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (-3f, -2f, 0f));
            MakeRightDoor ("exit", new Vector3 (4f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1)
            {
                map.transform.Translate (-1f, 0f, 0f);
                MakeRightDoor ("firstRoom", new Vector3 (4f, -1f, 0f));
            }
            else MakeRightDoor ("entrance", new Vector3 (4f, -1f, 0f));
            MakeDownDoor ("exit", new Vector3 (-3f, -2f, 0f));
        }
    }

    void MakeDoorsFor23A()
    {
        int randomNumber = Random.Range (0, 3);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-3f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-3f, 0f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (0f, -1f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (0f, -1f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, 0f, 0f));
        }
        if (randomNumber  == 2)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (3f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (3f, 0f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -1f, 0f));
        }
    }

    void MakeDoorsFor24A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (-2f, -1f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (-2f, -1f, 0f));
            MakeRightDoor ("exit", new Vector3 (3f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (3f, 0f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (3f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (-2f, -1f, 0f));
        }
    }

    void MakeDoorsFor24B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (2f, -1f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (2f, -1f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-3f, 0f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-3f, 0f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-3f, 0f, 0f));
            MakeDownDoor ("exit", new Vector3 (2f, -1f, 0f));
        }
    }

    void MakeDoorsFor26A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (0f, 2f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (0f, 2f, 0f));
            MakeDownDoor ("exit", new Vector3 (0f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (0f, -1f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (0f, -1f, 0f));
            MakeUpDoor ("exit", new Vector3 (0f, 2f, 0f));
        }
    }

    void MakeDoorsFor27A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (1f, 1f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (1f, 1f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-1f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-1f, -1f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-1f, -1f, 0f));
            MakeRightDoor ("exit", new Vector3 (1f, 1f, 0f));
        }
    }

    void MakeDoorsFor27B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeRightDoor ("firstRoom", new Vector3 (1f, -1f, 0f));
            else MakeRightDoor ("entrance", new Vector3 (1f, -1f, 0f));
            MakeLeftDoor ("exit", new Vector3 (-1f, 1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-1f, 1f, 0f));
            else MakeLeftDoor ("entrance", new Vector3 (-1f, 1f, 0f));
            MakeRightDoor ("exit", new Vector3 (1f, -1f, 0f));
        }
    }

    void MakeDoorsFor28A()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (1f, 1f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (1f, 1f, 0f));
            MakeDownDoor ("exit", new Vector3 (-1f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (-1f, -1f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (-1f, -1f, 0f));
            MakeUpDoor ("exit", new Vector3 (1f, 1f, 0f));
        }
    }

    void MakeDoorsFor28B()
    {
        int randomNumber = Random.Range (0, 2);
        if (randomNumber  == 0)
        {
            if (createdRooms == 1) MakeUpDoor ("firstRoom", new Vector3 (-1f, 1f, 0f));
            else MakeUpDoor ("entrance", new Vector3 (-1f, 1f, 0f));
            MakeDownDoor ("exit", new Vector3 (1f, -1f, 0f));
        }
        if (randomNumber  == 1)
        {
            if (createdRooms == 1) MakeDownDoor ("firstRoom", new Vector3 (1f, -1f, 0f));
            else MakeDownDoor ("entrance", new Vector3 (1f, -1f, 0f));
            MakeUpDoor ("exit", new Vector3 (-1f, 1f, 0f));
        }
    }

    void MakeDoorsForTransitionIndoor()
    {
        if (createdRooms == 1) MakeLeftDoor ("firstRoom", new Vector3 (-1f, 0f, 0f));
        else MakeLeftDoor ("entrance", new Vector3 (-1f, 0f, 0f));
        MakeRightDoor ("exit", new Vector3 (1f, 0f, 0f));
    }

    void MakeUpDoor(string doorType, Vector3 position)
    {
        if(doorType == "firstRoom") MakeOneDoor (firstRoom, position + new Vector3(0f,1f,0f), new Vector3 (0f, 0f, 270f));
        if(doorType == "entrance") MakeOneDoor (entrance, position + new Vector3 (0f, -0.5f, 0f), new Vector3 (0f, 0f, 270f));
        if (doorType == "exit")
        {
            MakeOneDoor (exit, position + new Vector3 (0f, -0.5f, 0f), new Vector3 (0f, 0f, 90f));
            MakeOneDoor (door, position, new Vector3 (0f, 0f, 90f));
        }
    }
    void MakeDownDoor(string doorType, Vector3 position)
    {
        if(doorType == "firstRoom") MakeOneDoor (firstRoom, position + new Vector3(0f, -1f, 0f), new Vector3 (0f, 0f, 90f));
        if(doorType == "entrance") MakeOneDoor (entrance, position + new Vector3 (0f, 0.5f, 0f), new Vector3 (0f, 0f, 90f));
        if (doorType == "exit")
        {
            MakeOneDoor (exit, position + new Vector3 (0f, 0.5f, 0f), new Vector3 (0f, 0f, 270f));
            MakeOneDoor (door, position, new Vector3 (0f, 0f, 270f));
        }
    }
    void MakeLeftDoor(string doorType, Vector3 position)
    {
        if(doorType == "firstRoom") MakeOneDoor (firstRoom, position + new Vector3(-1f, 0f, 0f), new Vector3 (0f, 0f, 0f));
        if(doorType == "entrance") MakeOneDoor (entrance, position + new Vector3 (0.5f, 0f, 0f), new Vector3 (0f, 0f, 0f));
        if (doorType == "exit")
        {
            MakeOneDoor (exit, position + new Vector3 (0.5f, 0f, 0f), new Vector3 (0f, 0f, 180f));
            MakeOneDoor (door, position, new Vector3 (0f, 0f, 180f));
        }
    }
    void MakeRightDoor(string doorType, Vector3 position)
    {
        if(doorType == "firstRoom") MakeOneDoor (firstRoom, position + new Vector3(1f, 0f, 0f), new Vector3 (0f, 0f, 180f));
        if(doorType == "entrance") MakeOneDoor (entrance, position + new Vector3 (-0.5f, 0f, 0f), new Vector3 (0f, 0f, 180f));
        if (doorType == "exit")
        {
            MakeOneDoor (exit, position + new Vector3 (-0.5f, 0f, 0f), new Vector3 (0f, 0f, 0f));
            MakeOneDoor (door, position, new Vector3 (0f, 0f, 0f));
        }
    }

    void MakeOneDoor(GameObject door, Vector3 position, Vector3 angle)
    {
        GameObject newDoor = Instantiate (door);
        newDoor.transform.parent = map.transform;
        newDoor.transform.localPosition = position;
        newDoor.transform.rotation = Quaternion.Euler (angle);
    }

    #endregion

    #region MakeLists

    void MakeListsFor01A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 2.5f,   0f),
            new Vector3 (1.5f,  2.5f,   0f),
            new Vector3 (2.5f,  2.5f,   0f),
            new Vector3 (3.5f,  2.5f,   0f),

            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),
            new Vector3 (3.5f,  1.5f,   0f),

            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
            new Vector3 (3.5f,  -1.5f,  0f),

            new Vector3 (-1.5f, -2.5f,  0f),
            new Vector3 (1.5f,  -2.5f,  0f),
            new Vector3 (2.5f,  -2.5f,  0f),
            new Vector3 (3.5f,  -2.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (2f,    2.5f,   0f),

            new Vector3 (-1f,   1.5f,   0f),
            new Vector3 (1f,    1.5f,   0f),
            new Vector3 (3f,    1.5f,   0f),

            new Vector3 (0f,    0.5f,   0f),
            new Vector3 (2f,    0.5f,   0f),

            new Vector3 (0f,    -0.5f,  0f),
            new Vector3 (2f,    -0.5f,  0f),

            new Vector3 (-1f,   -1.5f,  0f),
            new Vector3 (1f,    -1.5f,  0f),
            new Vector3 (3f,    -1.5f,  0f),

            new Vector3 (2f,    -2.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (3f,    2f,     0f),

            new Vector3 (-1f,   1f,     0f),
            new Vector3 (1f,    1f,     0f),

            new Vector3 (-1f,   -1f,    0f),
            new Vector3 (1f,    -1f,    0f),

            new Vector3 (3f,    -2f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (2.5f,  2f,     0f),

            new Vector3 (-0.5f, 1f,     0f),

            new Vector3 (-0.5f, -1f,    0f),

            new Vector3 (2.5f,  -2f,    0f),
        });
    }

    void MakeListsFor01B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-3.5f, 2.5f,   0f),
            new Vector3 (-2.5f, 2.5f,   0f),
            new Vector3 (1.5f,  2.5f,   0f),
            new Vector3 (2.5f,  2.5f,   0f),
            new Vector3 (3.5f,  2.5f,   0f),

            new Vector3 (-3.5f, 1.5f,   0f),
            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),
            new Vector3 (3.5f,  1.5f,   0f),

            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (-3.5f, -1.5f,  0f),
            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
            new Vector3 (3.5f,  -1.5f,  0f),

            new Vector3 (-3.5f, -2.5f,  0f),
            new Vector3 (-2.5f, -2.5f,  0f),
            new Vector3 (1.5f,  -2.5f,  0f),
            new Vector3 (2.5f,  -2.5f,  0f),
            new Vector3 (3.5f,  -2.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-3f,   2.5f,   0f),
            new Vector3 (2f,    2.5f,   0f),

            new Vector3 (-3f,   1.5f,   0f),
            new Vector3 (1f,    1.5f,   0f),
            new Vector3 (3f,    1.5f,   0f),

            new Vector3 (-2f,   0.5f,   0f),
            new Vector3 (0f,    0.5f,   0f),
            new Vector3 (2f,    0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (0f,    -0.5f,  0f),
            new Vector3 (2f,    -0.5f,  0f),

            new Vector3 (-3f,   -1.5f,  0f),
            new Vector3 (1f,    -1.5f,  0f),
            new Vector3 (3f,    -1.5f,  0f),

            new Vector3 (-3f,   -2.5f,  0f),
            new Vector3 (2f,    -2.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-3f,   2f,     0f),
            new Vector3 (2f,    2f,     0f),

            new Vector3 (0f,    1f,     0f),

            new Vector3 (-2f,   0f,     0f),
            new Vector3 (2f,    0f,     0f),

            new Vector3 (0f,    -1f,    0f),

            new Vector3 (-3f,   -2f,    0f),
            new Vector3 (2f,    -2f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (2.5f,  2f,     0f),

            new Vector3 (-1.5f, 0f,     0f),
            new Vector3 (1.5f,  0f,     0f),

            new Vector3 (2.5f,  -2f,    0f),
        });
    }

    void MakeListsFor02A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 2.5f,   0f),
            new Vector3 (-1.5f, 2.5f,   0f),
            new Vector3 (1.5f,  2.5f,   0f),
            new Vector3 (2.5f,  2.5f,   0f),

            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-1.5f, 1.5f,   0f),

            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),

            new Vector3 (-2.5f, -2.5f,  0f),
            new Vector3 (-1.5f, -2.5f,  0f),
            new Vector3 (1.5f,  -2.5f,  0f),
            new Vector3 (2.5f,  -2.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   2.5f,   0f),
            new Vector3 (2f,    2.5f,   0f),

            new Vector3 (-2f,   1.5f,   0f),

            new Vector3 (-2f,   0.5f,   0f),
            new Vector3 (2f,    0.5f,   0f),

            new Vector3 (1f,    -0.5f,  0f),

            new Vector3 (-2f,   -1.5f,  0f),
            new Vector3 (0f,    -1.5f,  0f),
            new Vector3 (2f,    -1.5f,  0f),

            new Vector3 (-2f,   -2.5f,  0f),
            new Vector3 (2f,    -2.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-2f,   2f,     0f),

            new Vector3 (2f,    0f,     0f),

            new Vector3 (-2f,   -2f,    0f),
            new Vector3 (2f,    -2f,    0f),
        });
    }

    void MakeListsFor02B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 2.5f,   0f),
            new Vector3 (-1.5f, 2.5f,   0f),
            new Vector3 (1.5f,  2.5f,   0f),
            new Vector3 (2.5f,  2.5f,   0f),

            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),

            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),

            new Vector3 (-2.5f, -2.5f,  0f),
            new Vector3 (-1.5f, -2.5f,  0f),
            new Vector3 (1.5f,  -2.5f,  0f),
            new Vector3 (2.5f,  -2.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   2.5f,   0f),
            new Vector3 (2f,    2.5f,   0f),

            new Vector3 (-2f,   1.5f,   0f),
            new Vector3 (0f,    1.5f,   0f),
            new Vector3 (2f,    1.5f,   0f),

            new Vector3 (-2f,   0.5f,   0f),
            new Vector3 (0f,    0.5f,   0f),
            new Vector3 (2f,    0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (0f,    -0.5f,  0f),
            new Vector3 (2f,    -0.5f,  0f),

            new Vector3 (-2f,   -1.5f,  0f),
            new Vector3 (0f,    -1.5f,  0f),
            new Vector3 (2f,    -1.5f,  0f),

            new Vector3 (-2f,   -2.5f,  0f),
            new Vector3 (2f,    -2.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-2f,   2f,     0f),
            new Vector3 (2f,    2f,     0f),

            new Vector3 (0f,    1f,     0f),

            new Vector3 (-2f,   0f,     0f),
            new Vector3 (2f,    0f,     0f),

            new Vector3 (0f,    -1f,    0f),

            new Vector3 (-2f,   -2f,    0f),
            new Vector3 (2f,    -2f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 1f,     0f),
            new Vector3 (1.5f,  1f,     0f),

            new Vector3 (-1.5f, -1f,    0f),
            new Vector3 (1.5f,  -1f,    0f),
        });
    }

    void MakeListsFor03A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-1f,   0.5f,   0f),
            new Vector3 (1f,    0.5f,   0f),

            new Vector3 (-1f,   -0.5f,  0f),
            new Vector3 (1f,    -0.5f,  0f),

            new Vector3 (-2f,   -1.5f,  0f),
            new Vector3 (2f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-1f,   0f,     0f),
            new Vector3 (1f,    0f,     0f),
        });
    }

    void MakeListsFor03B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),

            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   1.5f,   0f),
            new Vector3 (0f,    1.5f,   0f),
            new Vector3 (2f,    1.5f,   0f),

            new Vector3 (-1f,   0.5f,   0f),
            new Vector3 (1f,    0.5f,   0f),

            new Vector3 (-1f,   -0.5f,  0f),
            new Vector3 (1f,    -0.5f,  0f),

            new Vector3 (-2f,   -1.5f,  0f),
            new Vector3 (2f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-1f,   0f,     0f),
            new Vector3 (1f,    0f,     0f),
        });
    }

    void MakeListsFor04A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 2.5f,   0f),
            new Vector3 (-1.5f, 2.5f,   0f),
            new Vector3 (1.5f,  2.5f,   0f),

            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),

            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),

            new Vector3 (-0.5f, -2.5f,  0f),
            new Vector3 (0.5f,  -2.5f,  0f),
            new Vector3 (1.5f,  -2.5f,  0f),
            new Vector3 (2.5f,  -2.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   2.5f,   0f),

            new Vector3 (-2f,   1.5f,   0f),
            new Vector3 (0f,    1.5f,   0f),

            new Vector3 (-1f,   0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (0f,    -0.5f,  0f),

            new Vector3 (-1f,   -1.5f,  0f),
            new Vector3 (1f,    -1.5f,  0f),

            new Vector3 (0f,    -2.5f,  0f),
            new Vector3 (2f,    -2.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-2f,   2f,     0f),

            new Vector3 (-2f,   0f,     0f),

            new Vector3 (0f,    -2f,    0f),
            new Vector3 (2f,    -2f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0f,     0f),

            new Vector3 (0.5f,  -2f,    0f),
        });
    }

    void MakeListsFor04B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 2.5f,   0f),
            new Vector3 (-1.5f, 2.5f,   0f),
            new Vector3 (1.5f,  2.5f,   0f),
            new Vector3 (2.5f,  2.5f,   0f),

            new Vector3 (1.5f,  1.5f,   0f),

            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),

            new Vector3 (-2.5f, -2.5f,  0f),
            new Vector3 (-1.5f, -2.5f,  0f),
            new Vector3 (-0.5f, -2.5f,  0f),
            new Vector3 (0.5f,  -2.5f,  0f),
            new Vector3 (1.5f,  -2.5f,  0f),
            new Vector3 (2.5f,  -2.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   2.5f,   0f),
            new Vector3 (2f,    2.5f,   0f),

            new Vector3 (-1f,   0.5f,   0f),
            new Vector3 (1f,    0.5f,   0f),

            new Vector3 (-1f,   -0.5f,  0f),
            new Vector3 (1f,    -0.5f,  0f),

            new Vector3 (-2f,   -1.5f,  0f),
            new Vector3 (0f,    -1.5f,  0f),
            new Vector3 (2f,    -1.5f,  0f),

            new Vector3 (-2f,   -2.5f,  0f),
            new Vector3 (0f,    -2.5f,  0f),
            new Vector3 (2f,    -2.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-1f,   0f,     0f),
            new Vector3 (1f,    0f,     0f),

            new Vector3 (-2f,   -2f,    0f),
            new Vector3 (0f,    -2f,    0f),
            new Vector3 (2f,    -2f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (0.5f,  0f,     0f),

            new Vector3 (-1.5f, -2f,    0f),
            new Vector3 (1.5f,  -2f,    0f),
        });
    }

    void MakeListsFor06A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),

            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   1.5f,   0f),
            new Vector3 (2f,    1.5f,   0f),

            new Vector3 (1f,    0.5f,   0f),

            new Vector3 (1f,    -0.5f,  0f),

            new Vector3 (-2f,   -1.5f,  0f),
            new Vector3 (2f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (1f,    1f,     0f),

            new Vector3 (1f,    -1f,    0f),
        });
    }

    void MakeListsFor06B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),

            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   1.5f,   0f),
            new Vector3 (0f,    1.5f,   0f),
            new Vector3 (2f,    1.5f,   0f),

            new Vector3 (-1f,   0.5f,   0f),
            new Vector3 (1f,    0.5f,   0f),

            new Vector3 (-1f,   -0.5f,  0f),
            new Vector3 (1f,    -0.5f,  0f),

            new Vector3 (-2f,   -1.5f,  0f),
            new Vector3 (0f,    -1.5f,  0f),
            new Vector3 (2f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-1f,   1f,     0f),
            new Vector3 (1f,    1f,     0f),

            new Vector3 (-1f,   -1f,    0f),
            new Vector3 (1f,    -1f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (0.5f,  1f,     0f),

            new Vector3 (-0.5f, -1f,    0f),
        });
    }

    void MakeListsFor07A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-1.5f, 1.5f,   0f),

            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   1.5f,   0f),

            new Vector3 (-2f,   0.5f,   0f),
            new Vector3 (0f,    0.5f,   0f),
            new Vector3 (2f,    0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (0f,    -0.5f,  0f),
            new Vector3 (2f,    -0.5f,  0f),

            new Vector3 (2f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-2f,   1f,     0f),

            new Vector3 (0f,    0f,     0f),

            new Vector3 (2f,    -1f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0f,     0f),
            new Vector3 (1.5f,  0f,     0f),
        });
    }

    void MakeListsFor07B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),

            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (1f,    1.5f,   0f),

            new Vector3 (-2f,   0.5f,   0f),
            new Vector3 (0f,    0.5f,   0f),
            new Vector3 (2f,    0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (0f,    -0.5f,  0f),
            new Vector3 (2f,    -0.5f,  0f),

            new Vector3 (-1f,   -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (2f,    1f,     0f),

            new Vector3 (0f,    0f,     0f),

            new Vector3 (-2f,   -1f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (1.5f,  0f,     0f),
            new Vector3 (-1.5f, 0f,     0f),
        });
    }

    void MakeListsFor08A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),

            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-1f,   0.5f,   0f),

            new Vector3 (0f,    -0.5f,  0f),

            new Vector3 (-1f,   -1.5f,  0f),
            new Vector3 (1f,    -1.5f,  0f),
        });
    }

    void MakeListsFor08B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),

            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (1f,    0.5f,   0f),

            new Vector3 (0f,    -0.5f,  0f),

            new Vector3 (-1f,   -1.5f,  0f),
            new Vector3 (1f,    -1.5f,  0f),
        });
    }

    void MakeListsFor09A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),

            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (0f,    0.5f,   0f),

            new Vector3 (0f,    -0.5f,  0f),

            new Vector3 (-1f,   -1.5f,  0f),
            new Vector3 (1f,    -1.5f,  0f),
        });
    }

    void MakeListsFor10A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),

            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-1f,   1.5f,   0f),
            new Vector3 (1f,    1.5f,   0f),

            new Vector3 (0f,    0.5f,   0f),

            new Vector3 (0f,    -0.5f,  0f),

            new Vector3 (-1f,   -1.5f,  0f),
            new Vector3 (1f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (0f,    1f,     0f),

            new Vector3 (0f,    -1f,    0f),
        });
    }

    void MakeListsFor12A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),

            new Vector3 (-1.5f, 0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
        });
    }

    void MakeListsFor12B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),

            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-1f,   1.5f,   0f),
            new Vector3 (1f,    1.5f,   0f),

            new Vector3 (-1f,   0.5f,   0f),
            new Vector3 (1f,    0.5f,   0f),

            new Vector3 (1f,    -0.5f,  0f),

            new Vector3 (1f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-1f,   1f,     0f),
            new Vector3 (1f,    1f,     0f),

            new Vector3 (1f,    -1f,    0f),
        });
    }

    void MakeListsFor13A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 2.5f,   0f),
            new Vector3 (1.5f,  2.5f,   0f),

            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),

            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   1.5f,   0f),
            new Vector3 (0f,    1.5f,   0f),
            new Vector3 (2f,    1.5f,   0f),

            new Vector3 (-2f,   0.5f,   0f),
            new Vector3 (0f,    0.5f,   0f),
            new Vector3 (2f,    0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (0f,    -0.5f,  0f),
            new Vector3 (2f,    -0.5f,  0f),

            new Vector3 (-2f,   -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-2f,   1f,     0f),
            new Vector3 (2f,    1f,     0f),

            new Vector3 (0f,    0f,     0f),

            new Vector3 (-2f,   -1f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0f,     0f),

            new Vector3 (1.5f,  0f,     0f),
        });
    }

    void MakeListsFor13B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 2.5f,   0f),
            new Vector3 (1.5f,  2.5f,   0f),

            new Vector3 (-2.5f, 1.5f,   0f),
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),
            new Vector3 (2.5f,  1.5f,   0f),

            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (2.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   1.5f,   0f),
            new Vector3 (0f,    1.5f,   0f),
            new Vector3 (2f,    1.5f,   0f),

            new Vector3 (-2f,   0.5f,   0f),
            new Vector3 (0f,    0.5f,   0f),
            new Vector3 (2f,    0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (0f,    -0.5f,  0f),
            new Vector3 (2f,    -0.5f,  0f),

            new Vector3 (2f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-2f,   1f,     0f),
            new Vector3 (2f,    1f,     0f),

            new Vector3 (0f,    0f,     0f),

            new Vector3 (2f,    -1f,    0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0f,     0f),

            new Vector3 (1.5f,  0f,     0f),
        });
    }

    void MakeListsFor14A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),
            new Vector3 (1.5f,  1.5f,   0f),

            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),

            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (1f,    1.5f,   0f),

            new Vector3 (0f,    0.5f,   0f),

            new Vector3 (0f,    -0.5f,  0f),

            new Vector3 (1f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (0f,    1f,     0f),

            new Vector3 (0f,    -1f,    0f),
        });
    }

    void MakeListsFor14B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 1.5f,   0f),
            new Vector3 (-0.5f, 1.5f,   0f),
            new Vector3 (0.5f,  1.5f,   0f),

            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-1f,   1.5f,   0f),

            new Vector3 (0f,    0.5f,   0f),

            new Vector3 (0f,    -0.5f,  0f),

            new Vector3 (-1f,   -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (0f,    1f,     0f),

            new Vector3 (0f,    -1f,    0f),
        });
    }

    void MakeListsFor15A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-2f,   0.5f,   0f),
            new Vector3 (1f,    0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (1f,    -0.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-2f,   0f,     0f),

            new Vector3 (1f,    0f,     0f),
        });
    }

    void MakeListsFor16A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
        });
    }

    void MakeListsFor17A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
        });
    }

    void MakeListsFor17B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
    }

    void MakeListsFor19A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (0f,    0.5f,   0f),
            new Vector3 (2f,    0.5f,   0f),

            new Vector3 (-1f,   -0.5f,  0f),
            new Vector3 (1f,    -0.5f,  0f),

            new Vector3 (-2f,   -1.5f,  0f),
            new Vector3 (0f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (1f,    0f,     0f),

            new Vector3 (-1f,   -1f,    0f),
        });
    }

    void MakeListsFor19B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (0.5f,  -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (0f,    0.5f,   0f),
            new Vector3 (-2f,   0.5f,   0f),

            new Vector3 (-1f,   -0.5f,  0f),
            new Vector3 (1f,    -0.5f,  0f),

            new Vector3 (2f,    -1.5f,  0f),
            new Vector3 (0f,    -1.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-1f,   0f,     0f),

            new Vector3 (1f,    -1f,    0f),
        });
    }

    void MakeListsFor20A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-3.5f, 0.5f,   0f),
            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),
            new Vector3 (3.5f,  0.5f,   0f),

            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),
            new Vector3 (3.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-3f,   0.5f,   0f),
            new Vector3 (-1f,   0.5f,   0f),
            new Vector3 (1f,    0.5f,   0f),
            new Vector3 (3f,    0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (0f,    -0.5f,  0f),
            new Vector3 (2f,    -0.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-2f,   0f,     0f),
            new Vector3 (0f,    0f,     0f),
            new Vector3 (2f,    0f,     0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0f,     0f),
            new Vector3 (1.5f,  0f,     0f),
        });
    }

    void MakeListsFor20B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-3.5f, 0.5f,   0f),
            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),
            new Vector3 (3.5f,  0.5f,   0f),

            new Vector3 (-3.5f, -0.5f,  0f),
            new Vector3 (-2.5f, -0.5f,  0f),
            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
            new Vector3 (2.5f,  -0.5f,  0f),

            new Vector3 (2.5f,  -1.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-3f,   0.5f,   0f),
            new Vector3 (-1f,   0.5f,   0f),
            new Vector3 (1f,    0.5f,   0f),
            new Vector3 (3f,    0.5f,   0f),

            new Vector3 (-2f,   -0.5f,  0f),
            new Vector3 (0f,    -0.5f,  0f),
            new Vector3 (2f,    -0.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-2f,   0f,     0f),
            new Vector3 (0f,    0f,     0f),
            new Vector3 (2f,    0f,     0f),
        });
        spawn3x2.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0f,     0f),
            new Vector3 (1.5f,  0f,     0f),
        });
    }

    void MakeListsFor21A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (-0.5f, -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
            new Vector3 (2.5f,  -1.5f,  0f),
        });
    }

    void MakeListsFor21B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),

            new Vector3 (-2.5f, -1.5f,  0f),
            new Vector3 (-1.5f, -1.5f,  0f),
            new Vector3 (1.5f,  -1.5f,  0f),
        });
    }

    void MakeListsFor22A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
        });
        spawn2x1.AddRange (new Vector3[] {
            new Vector3 (-1f,   0.5f,   0f),
            new Vector3 (1f,    0.5f,   0f),

            new Vector3 (-1f,   -0.5f,  0f),
            new Vector3 (1f,    -0.5f,  0f),
        });
        spawn2x2.AddRange (new Vector3[] {
            new Vector3 (-1f,   0f,     0f),
            new Vector3 (1f,    0f,     0f),
        });
    }

    void MakeListsFor23A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
        });
    }

    void MakeListsFor24A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-2.5f, 0.5f,   0f),
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
            new Vector3 (1.5f,  -0.5f,  0f),
        });
    }

    void MakeListsFor24B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0.5f,   0f),
            new Vector3 (-0.5f, 0.5f,   0f),
            new Vector3 (0.5f,  0.5f,   0f),
            new Vector3 (1.5f,  0.5f,   0f),
            new Vector3 (2.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),
            new Vector3 (-0.5f, -0.5f,  0f),
            new Vector3 (0.5f,  -0.5f,  0f),
        });
    }

    void MakeListsFor27A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-0.5f, 1.5f,   0f),

            new Vector3 (-0.5f, 0.5f,   0f),

            new Vector3 (0.5f,  -0.5f,  0f),

            new Vector3 (0.5f,  -1.5f,  0f),
        });
    }

    void MakeListsFor27B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (0.5f,  1.5f,   0f),

            new Vector3 (0.5f,  0.5f,   0f),

            new Vector3 (-0.5f, -0.5f,  0f),

            new Vector3 (-0.5f, -1.5f,  0f),
        });
    }

    void MakeListsFor28A()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (-1.5f, 0.5f,   0f),

            new Vector3 (-0.5f, 0.5f,   0f),

            new Vector3 (0.5f,  -0.5f,  0f),

            new Vector3 (1.5f,  -0.5f,  0f),
        });
    }

    void MakeListsFor28B()
    {
        spawn1x1.AddRange (new Vector3[] {
            new Vector3 (0.5f,  0.5f,   0f),

            new Vector3 (1.5f,  0.5f,   0f),

            new Vector3 (-1.5f, -0.5f,  0f),

            new Vector3 (-0.5f, -0.5f,  0f),
        });
    }

    #endregion

    #region MakeMonster

    void MakeMonsters()
    {
        for (int i = 2; i < allData.Length; i++)
        {
            string[] oneQuest = allData [i].Split (new string[] { "</div>" }, System.StringSplitOptions.None) [1].Split (new string[] { "</td>" }, System.StringSplitOptions.None) [0].Split (new string[] { "<br />" }, System.StringSplitOptions.None);

            if (oneQuest [0].Trim () == "EC")
            {
                for (int j = 0; j < oneQuest.Length; j++)
                {
                    if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "name")
                    {
                        if (oneQuest[j].Split(new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                        {
                            if (currentQuest == oneQuest[j].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim())
                            {
                                for (int k = 0; k < oneQuest.Length; k++)
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "monster")
                                    {
                                        if (oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                        {
                                            SpawnMonsters(oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }

    void SpawnMonsters (string monster)
    {
        if (monster == "Dragon")
        {
            int randomNumber = Random.Range (0, spawn3x2.Count);
            SpawnMonster ("Monster-" + monster + "-Red", spawn3x2 [randomNumber], 2);

            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (-1f, 0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (0f, 0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (1f, 0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (-1f, -0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (0f, -0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (1f, -0.5f, 0f));

            spawn3x2.Remove (spawn3x2 [randomNumber]);

            randomNumber = Random.Range (0, spawn3x2.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn3x2 [randomNumber], 0);

            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (-1f, 0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (0f, 0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (1f, 0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (-1f, -0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (0f, -0.5f, 0f));
            spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (1f, -0.5f, 0f));

            spawn3x2.Remove (spawn3x2 [randomNumber]);
        }
        if (monster == "Elemental" || monster == "Ettin" || monster == "Merriod")
        {
            int randomNumber = Random.Range (0, spawn2x2.Count);
            SpawnMonster ("Monster-" + monster + "-Red", spawn2x2 [randomNumber], 2);

            spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (-0.5f, 0.5f, 0f));
            spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (0.5f, 0.5f, 0f));
            spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (-0.5f, -0.5f, 0f));
            spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (0.5f, -0.5f, 0f));

            spawn2x2.Remove (spawn2x2 [randomNumber]);

            randomNumber = Random.Range (0, spawn2x2.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn2x2 [randomNumber], 0);

            spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (-0.5f, 0.5f, 0f));
            spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (0.5f, 0.5f, 0f));
            spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (-0.5f, -0.5f, 0f));
            spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (0.5f, -0.5f, 0f));

            spawn2x2.Remove (spawn2x2 [randomNumber]);
        }
        if (monster == "Wolf")
        {
            int randomNumber = Random.Range (0, spawn2x1.Count);
            SpawnMonster ("Monster-" + monster + "-Red", spawn2x1 [randomNumber], 0);

            spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (-0.5f, 0f, 0f));
            spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (0.5f, 0f, 0f));

            spawn2x1.Remove (spawn2x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn2x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn2x1 [randomNumber], 0);

            spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (-0.5f, 0f, 0f));
            spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (0.5f, 0f, 0f));

            spawn2x1.Remove (spawn2x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn2x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn2x1 [randomNumber], 1);

            spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (-0.5f, 0f, 0f));
            spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (0.5f, 0f, 0f));

            spawn2x1.Remove (spawn2x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn2x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn2x1 [randomNumber], 2);

            spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (-0.5f, 0f, 0f));
            spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (0.5f, 0f, 0f));

            spawn2x1.Remove (spawn2x1 [randomNumber]);
        }
        if (monster == "Flesh")
        {
            int randomNumber = Random.Range (0, spawn1x1.Count);
            SpawnMonster ("Monster-" + monster + "-Red", spawn1x1 [randomNumber], 0);
            spawn1x1.Remove (spawn1x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn1x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn1x1 [randomNumber], 0);
            spawn1x1.Remove (spawn1x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn1x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn1x1 [randomNumber], 1);
            spawn1x1.Remove (spawn1x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn1x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn1x1 [randomNumber], 2);
            spawn1x1.Remove (spawn1x1 [randomNumber]);
        }
        if (monster == "Spider" || monster == "Goblin" || monster == "Zombie")
        {
            int randomNumber = Random.Range (0, spawn1x1.Count);
            SpawnMonster ("Monster-" + monster + "-Red", spawn1x1 [randomNumber], 0);
            spawn1x1.Remove (spawn1x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn1x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn1x1 [randomNumber], 0);
            spawn1x1.Remove (spawn1x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn1x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn1x1 [randomNumber], 0);
            spawn1x1.Remove (spawn1x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn1x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn1x1 [randomNumber], 1);
            spawn1x1.Remove (spawn1x1 [randomNumber]);

            randomNumber = Random.Range (0, spawn1x1.Count);
            SpawnMonster ("Monster-" + monster + "-Tan", spawn1x1 [randomNumber], 2);
            spawn1x1.Remove (spawn1x1 [randomNumber]);
        }
    }

    void SpawnMonster(string monsterName, Vector3 spawnLocation, int numberOfPlayers)
    {
        foreach (GameObject monster in monsters)
        {
            if (monster.name == monsterName)
            {
                GameObject newMonster = Instantiate (monster);
                newMonster.transform.parent = map.transform;
                newMonster.transform.localPosition = spawnLocation;
                newMonster.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = playerNumber [numberOfPlayers];
                currentMonster = newMonster.name.Substring(8,newMonster.name.Length-19);
            }
        }
    }

    #endregion

    void MakeTokensFromWWW ()
    {
        bool searchTokensAdded = false;

        for (int i = 1; i < allData.Length; i++)
        {
            string[] oneQuest = allData[i].Split(new string[] { "</div>" }, System.StringSplitOptions.None)[1].Split(new string[] { "</td>" }, System.StringSplitOptions.None)[0].Split(new string[] { "<br />" }, System.StringSplitOptions.None);

            for (int j = 0; j < oneQuest.Length; j++)
            {
                if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "name")
                {
                    if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                    {
                        if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim () == currentQuest)
                        {
                            for (int k = 0; k < oneQuest.Length - 1; k++)
                            {
                                if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "token")
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                    {
                                        if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim ().Contains ("Search-Token") == true) searchTokensAdded = true;

                                        foreach (GameObject token in tokens)
                                        {
                                            if (token.name == oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim ()) MakeToken (token);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (spawn1x1.Count >= 4 && searchTokensAdded == false)
        {
            MakeToken (tokens [1]);
            MakeToken (tokens [1]);
            MakeToken (tokens [2]);
            MakeToken (tokens [3]);

        }
    }

    void MakeToken (GameObject token)
    {
        GameObject newSearchToken = Instantiate(token);
        newSearchToken.transform.parent = map.transform;
        int randomNumber = Random.Range(0,spawn1x1.Count);
        newSearchToken.transform.localPosition = spawn1x1[randomNumber];
        spawn1x1.Remove (spawn1x1 [randomNumber]);

        if (token.name.Contains ("Lieutenant"))
        {
            bool isAlready = false;
            foreach (GameObject activateCard in activateCards)
            {
                if (activateCard.name.Substring (8,activateCard.name.Length - 15) == token.name) isAlready = true;
            }
            if (isAlready == false)
            {
                foreach (GameObject activateCard in allActivateCards)
                {
                    if (activateCard.name.Substring (8) == token.name) MakeActivationCardLayout (activateCard);
                }
            }
        }
    }

    void MakeExplorationCard ()
    {
        explorationCard.transform.localScale = new Vector3 (1f, 1f, 1f);
        explorationCard.GetComponent<SpriteRenderer> ().sprite = explorationCardColor;
        hourglass.GetComponent<SpriteRenderer> ().sprite = hourglassColor;
        explorationCard.GetComponent<explorationCardDiscarder> ().isActive = true;
    }
        
    void MakeQuestFromWWW (string questName)
    {
        for (int i = 2; i < allData.Length; i++)
        {
            string[] oneQuest = allData[i].Split(new string[] { "</div>" }, System.StringSplitOptions.None)[1].Split(new string[] { "</td>" }, System.StringSplitOptions.None)[0].Split(new string[] { "<br />" }, System.StringSplitOptions.None);

            for (int j = 0; j < oneQuest.Length; j++)
            {
                if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim() == "name")
                {
                    if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                    {
                        if (oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim () == questName)
                        {
                            nameText.GetComponent<Text> ().text = oneQuest [j].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim ();

                            for (int k = 0; k < oneQuest.Length; k++)
                            {
                                if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim () == "quest")
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                    {
                                        questText.GetComponent<Text> ().text = oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim ().Replace("/","\n");
                                    }
                                }
                                if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim () == "type")
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                    {
                                        if (oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim() == "starting")
                                        {
                                            type.GetComponent<SpriteRenderer>().sprite = types[0];
                                            type.SetActive (true);
                                        }
                                        if (oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim() == "1")
                                        {
                                            type.GetComponent<SpriteRenderer>().sprite = types[1];
                                            type.SetActive (true);
                                        }
                                        if (oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim() == "2")
                                        {
                                            type.GetComponent<SpriteRenderer>().sprite = types[2];
                                            type.SetActive (true);
                                        }
                                        if (oneQuest[k].Split(new string[] { ":" }, 2, System.StringSplitOptions.None)[1].Trim() == "3")
                                        {
                                            type.GetComponent<SpriteRenderer>().sprite = types[3];
                                            type.SetActive (true);

                                            finale = true;
                                            exploreButton.GetComponent<SpriteRenderer> ().sprite = blackExploreButton;
                                            for(int l = 0; l < map.transform.childCount; l++){
                                                if (map.transform.GetChild (l).name == "Exit(Clone)") map.transform.GetChild (l).GetComponent<SpriteRenderer> ().sprite = null;
                                                else if (map.transform.GetChild (l).name == "Yellow-Door(Clone)")
                                                {
                                                    if (map.GetComponent<SpriteRenderer> ().sprite.name.Substring(map.GetComponent<SpriteRenderer> ().sprite.name.Length - 1) == "A") map.transform.GetChild (l).GetComponent<SpriteRenderer> ().sprite = deadEndOut;
                                                    if (map.GetComponent<SpriteRenderer> ().sprite.name.Substring(map.GetComponent<SpriteRenderer> ().sprite.name.Length - 1) == "B") map.transform.GetChild (l).GetComponent<SpriteRenderer> ().sprite = deadEndIn;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim () == "afterSetup")
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                    {
                                        afterSetupText.GetComponent<Text> ().text = "AFTER SETUP: " + oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim ();
                                        afterSetup.SetActive (true);
                                    }
                                }
                                if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim () == "firstAction")
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                    {
                                        firstActionText.GetComponent<Text> ().text = oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim ();
                                        twoActions.SetActive (true);
                                    }
                                }
                                if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim () == "hourglassTime")
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                    {
                                        hourglassTimeText.GetComponent<Text> ().text = oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim ();
                                        hourglass.SetActive (true);
                                    }
                                }
                                if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim () == "hourglassOverlordTurn")
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                    {
                                        hourglassOverlordTurnText.GetComponent<Text> ().text = oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim ();
                                    }
                                }
                                if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [0].Trim () == "overlordTurn")
                                {
                                    if (oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None).Length == 2)
                                    {
                                        overlordTurnText.GetComponent<Text> ().text = oneQuest [k].Split (new string[] { ":" }, 2, System.StringSplitOptions.None) [1].Trim ();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void MakeActivationCardLayouts ()
    {
        bool isAlready = false;
        foreach (GameObject activateCard in activateCards)
        {
            if (activateCard.name.Substring (8,activateCard.name.Length - 15) == currentMonster) isAlready = true;
        }
        if (isAlready == false)
        {
            foreach (GameObject activateCard in allActivateCards)
            {
                if (activateCard.name.Substring (8) == currentMonster) MakeActivationCardLayout (activateCard);
            }
        }

        for (int i = 0; i < activateCards.Count; i++)
        {
            activateCards [i].transform.position = new Vector3 (i/2 * 8f, -12.5f - i%2 * 5f, 0f);
        }
    }

    void MakeActivationCardLayout (GameObject activateCard)
    {
        GameObject newActivateCard = Instantiate (activateCard);
        activateCards.Add (newActivateCard);
    }

    void MakeActivationCardTexts ()
    {
        foreach (GameObject activateCard in activateCards)
        {
            if (monstersAttackType [activateCard.name.Substring (8, activateCard.name.Length - 15)] == "melee")
            {
                int randomNumber = Random.Range (0, meleeMoves.Length);
                activateCard.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = meleeMoves [randomNumber];
                activateCard.transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite = meleeAttacks [0];
            }
            if (monstersAttackType [activateCard.name.Substring (8, activateCard.name.Length - 15)] == "ranged")
            {
                int randomNumber = Random.Range (0, rangedMoves.Length);
                activateCard.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = rangedMoves [randomNumber];
                activateCard.transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite = rangedAttacks [randomNumber];
            }
        }
    }
}
