using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.UI;

public class mapRandomizer : MonoBehaviour {
	
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
	public GameObject afterSetup;
	public GameObject hourglass;

	bool isItOut;
	bool inMainScreen;

	public List<string> mapTraits;
	public List<Vector3> spawn1x1;
	public List<Vector3> spawn2x1;
	public List<Vector3> spawn2x2;
	public List<Vector3> spawn3x2;

	public GameObject[] baseGameTan1x1;
	public GameObject[] baseGameRed1x1;
	public GameObject[] baseGameTan2x1;
	public GameObject[] baseGameRed2x1;
	public GameObject[] baseGameTan2x2;
	public GameObject[] baseGameRed2x2;
	public GameObject[] baseGameTan3x2;
	public GameObject[] baseGameRed3x2;

	public List<GameObject> tan1x1;
	public List<GameObject> red1x1;
	public List<GameObject> tan2x1;
	public List<GameObject> red2x1;
	public List<GameObject> tan2x2;
	public List<GameObject> red2x2;
	public List<GameObject> tan3x2;
	public List<GameObject> red3x2;

	public Sprite[] playerNumber;
	public string currentMonster;

	public GameObject nameText;
	public GameObject questText;
	public GameObject afterSetupText;
	public GameObject hourglassTimeText;
	public GameObject hourglassOverlordTurnText;
	public GameObject overlordTurnText;

	public Sprite explorationCardColor;
	public Sprite hourglassColor;

	Dictionary<string, int> standardQuestsSpace = new Dictionary<string, int>();
	Dictionary<string, string> standardQuestsMonster = new Dictionary<string, string>();
	Dictionary<string, int> baseGameUniqueQuestsSpace = new Dictionary<string, int>();
	Dictionary<string, string> baseGameUniqueQuestsMonster = new Dictionary<string, string>();
	Dictionary<string, int> nullUniqueQuestsSpace = new Dictionary<string, int>();
	Dictionary<string, string> nullUniqueQuestsMonster = new Dictionary<string, string>();

	public List<string> possibleQuests;

	public string currentQuest;

	public List<GameObject> tokens;

	const string CIVILIZED = "civilized";
	const string DARK = "dark";
	const string BUILDING = "building";
	const string MOUNTAIN = "mountain";
	const string CURSED = "cursed";
	const string COLD = "cold";
	const string HOT = "hot";
	const string WATER = "water";
	const string WILDERNESS = "wilderness";
	const string CAVE = "cave";

	Dictionary<string,string> monstersTrait1 = new Dictionary<string, string> ();
	Dictionary<string,string> monstersTrait2 = new Dictionary<string, string> ();
	Dictionary<string,string> monstersAttackType = new Dictionary<string, string> ();

	public Sprite deadEndIn;
	public Sprite deadEndOut;

	public List<Sprite> baseGameOut;
	public List<Sprite> baseGameIn;

	public List<Sprite> miscOut;
	public List<Sprite> miscIn;

	public List<Sprite> mapsOut;
	public List<Sprite> mapsIn;

	public List<GameObject> activateCards;
	public GameObject[] allActivateCards;
	public Sprite[] rangedMoves;
	public Sprite[] meleeMoves;
	public Sprite[] rangedAttacks;
	public Sprite[] meleeAttacks;

	Vector3 oldPos;
	public float speed;

	WebClient client = new WebClient();
	string[] allData;

	#endregion

	void Start () {
		createdRooms = 0;
		int randomNumber = Random.Range (0, 100);
		if (randomNumber < 50) isItOut = true;
		if (randomNumber >= 50) isItOut = false;

		inMainScreen = true;

		monstersTrait1.Add ("Monster-Dragon-Red", DARK);
		monstersTrait2.Add ("Monster-Dragon-Red", CAVE);
		monstersTrait1.Add ("Monster-Elemental-Red", COLD);
		monstersTrait2.Add ("Monster-Elemental-Red", HOT);
		monstersTrait1.Add ("Monster-Ettin-Red", MOUNTAIN);
		monstersTrait2.Add ("Monster-Ettin-Red", CAVE);
		monstersTrait1.Add ("Monster-Merriod-Red", WATER);
		monstersTrait2.Add ("Monster-Merriod-Red", WILDERNESS);
		monstersTrait1.Add ("Monster-Wolf-Red", DARK);
		monstersTrait2.Add ("Monster-Wolf-Red", WILDERNESS);
		monstersTrait1.Add ("Monster-Flesh-Red", CIVILIZED);
		monstersTrait2.Add ("Monster-Flesh-Red", CURSED);
		monstersTrait1.Add ("Monster-Spider-Red", WILDERNESS);
		monstersTrait2.Add ("Monster-Spider-Red", CAVE);
		monstersTrait1.Add ("Monster-Goblin-Red", BUILDING);
		monstersTrait2.Add ("Monster-Goblin-Red", CAVE);
		monstersTrait1.Add ("Monster-Zombie-Red", BUILDING);
		monstersTrait2.Add ("Monster-Zombie-Red", CURSED);

		monstersAttackType.Add ("Dragon", "melee");
		monstersAttackType.Add ("Elemental", "ranged");
		monstersAttackType.Add ("Ettin", "melee");
		monstersAttackType.Add ("Merriod", "melee");
		monstersAttackType.Add ("Wolf", "melee");
		monstersAttackType.Add ("Flesh", "ranged");
		monstersAttackType.Add ("Spider", "melee");
		monstersAttackType.Add ("Goblin", "ranged");
		monstersAttackType.Add ("Zombie", "melee");

		try
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			client.Encoding = System.Text.Encoding.UTF8;
			allData = client.DownloadString("https://boardgamegeek.com/thread/printerfriendly/1569631").Split(new string[]{"<td>"}, System.StringSplitOptions.None);
			PlayerPrefs.SetString("data",string.Join("<td>",allData));
		}
		catch
		{
			allData = PlayerPrefs.GetString("data").Split(new string[]{"<td>"}, System.StringSplitOptions.None);
		}

		MakeQuestFromWWW ();

		//*
		mapsOut.AddRange (baseGameOut);
		mapsIn.AddRange (baseGameIn);
		mapsOut.AddRange (miscOut);
		mapsIn.AddRange (miscIn);
		//*/

		/*
		mapsOut.Add (baseGameOut [0]);
		mapsOut.Add (baseGameOut [0]);
		mapsIn.Add (baseGameIn [0]);
		mapsIn.Add (baseGameIn [0]);
		*/
	}

	void Update () {
		if ((float)Screen.width / (float)Screen.height <= 1.6) Camera.main.orthographicSize = 8f / ((float)Screen.width / (float)Screen.height);
		else Camera.main.orthographicSize = 5f;

		int numberOfRooms = createdRooms;
		if (isItOut == true) {
			if (Vector3.Distance (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 9.5f), exploreButton.transform.position) <= 1.0f
				&& Input.GetMouseButtonUp (0) && mapsOut.Count <= 0 && explorationCard.GetComponent<explorationCardDiscarder>().isActive == false) {
				SceneManager.LoadScene (0);
			}
			if (Vector3.Distance (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 9.5f), exploreButton.transform.position) <= 1.0f
				&& Input.GetMouseButtonUp (0) && mapsOut.Count > 0 && explorationCard.GetComponent<explorationCardDiscarder>().isActive == false) {
				foreach (Transform child in map.transform) {
					Destroy (child.gameObject);
				}

				createdRooms++;

				int randomNumber = Random.Range (0, mapsOut.Count);
				//int randomNumber = Random.Range (19, 20);
				map.GetComponent<SpriteRenderer> ().sprite = mapsOut [randomNumber];
				mapsOut.Remove (mapsOut [randomNumber]);
				mapsIn.Remove (mapsIn [randomNumber]);

				firstRoom = firstRoomOut;
				map.transform.position = new Vector3 (-3f, 0f, 0f);
				map.transform.localScale = new Vector3 (1f, 1f, 1f);
				mapTraits.Clear ();
				spawn1x1.Clear ();
				spawn2x1.Clear ();
				spawn2x2.Clear ();
				spawn3x2.Clear ();
				currentMonster = "";
				currentQuest = "";
				possibleQuests.Clear ();
				nameText.GetComponent<Text> ().text = "";
				questText.GetComponent<Text> ().text = "";
				afterSetupText.GetComponent<Text> ().text = "";
				hourglassTimeText.GetComponent<Text> ().text = "";
				hourglassOverlordTurnText.GetComponent<Text> ().text = "";
				overlordTurnText.GetComponent<Text> ().text = "";
				afterSetup.SetActive (false);
				hourglass.SetActive (false);


				MakeProps ();

				MakeExplorationCard ();

				MakeMonsters ();
				MakeQuest ();
				//MakeTokens ();
				MakeTokensFromWWW ();
				MakeActivationCardLayouts ();

				if (mapsOut.Count == 0) exploreButton.GetComponent<SpriteRenderer> ().sprite = blackExploreButton;
			}
		}
		if (isItOut == false && numberOfRooms == createdRooms) {
			if (Vector3.Distance (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f), exploreButton.transform.position) <= 1.0f
				&& Input.GetMouseButtonUp (0) && mapsIn.Count <= 0 && explorationCard.GetComponent<explorationCardDiscarder>().isActive == false) {
				SceneManager.LoadScene (0);
			}
			if (Vector3.Distance (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f), exploreButton.transform.position) <= 1.0f
				&& Input.GetMouseButtonUp (0) && mapsIn.Count > 0 && explorationCard.GetComponent<explorationCardDiscarder>().isActive == false) {
				foreach (Transform child in map.transform) {
					Destroy (child.gameObject);
				}

				createdRooms++;

				int randomNumber = Random.Range (0, mapsIn.Count);
				//int randomNumber = Random.Range (19, 20);
				map.GetComponent<SpriteRenderer> ().sprite = mapsIn [randomNumber];
				mapsIn.Remove (mapsIn [randomNumber]);
				mapsOut.Remove (mapsOut [randomNumber]);

				firstRoom = firstRoomIn;
				map.transform.position = new Vector3 (-3f, 0f, 0f);
				map.transform.localScale = new Vector3 (1f, 1f, 1f);
				mapTraits.Clear ();
				spawn1x1.Clear ();
				spawn2x1.Clear ();
				spawn2x2.Clear ();
				spawn3x2.Clear ();
				currentMonster = "";
				currentQuest = "";
				possibleQuests.Clear ();
				nameText.GetComponent<Text> ().text = "";
				questText.GetComponent<Text> ().text = "";
				afterSetupText.GetComponent<Text> ().text = "";
				hourglassTimeText.GetComponent<Text> ().text = "";
				hourglassOverlordTurnText.GetComponent<Text> ().text = "";
				overlordTurnText.GetComponent<Text> ().text = "";
				afterSetup.SetActive (false);
				hourglass.SetActive (false);

				MakeProps ();

				MakeExplorationCard ();

				MakeMonsters ();
				MakeQuest ();
				//MakeTokens ();
				MakeTokensFromWWW ();
				MakeActivationCardLayouts ();

				if (mapsIn.Count == 0) exploreButton.GetComponent<SpriteRenderer> ().sprite = blackExploreButton;
			}
		}

		if (Vector3.Distance (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f), activateButton.transform.position) <= 1.0f
		    && Input.GetMouseButtonUp (0))
		{
			inMainScreen = false;
			speed = 0f;
			backButton.transform.position = new Vector3 (-7f, backButton.transform.position.y, backButton.transform.position.z);

			for (int i = 0; i < activateCards.Count; i++) {
				activateCards [i].transform.position = new Vector3 (i / 2 * 8f, -12.5f - i % 2 * 5f, 0f);
			}
			MakeActivationCardTexts ();
		}
		if (Vector3.Distance (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f), backButton.transform.position) <= 1.0f
			&& Input.GetMouseButtonUp (0)) inMainScreen = true;

		if (inMainScreen == true && Camera.main.transform.position.y < 0)
		{
			Camera.main.transform.Translate (0f, 50f * Time.deltaTime, 0f);
			if (Camera.main.transform.position.y > 0) Camera.main.transform.position = new Vector3 (0f, 0f, -10f);
		}

		if (inMainScreen == false && Camera.main.transform.position.y > -15)
		{
			Camera.main.transform.Translate (0f, -50f * Time.deltaTime, 0f);
			if (Camera.main.transform.position.y < -15) Camera.main.transform.position = new Vector3 (0f, -15f, -10f);
		}

		if (Camera.main.transform.position.y == -15)
		{
			if (Input.GetMouseButtonDown (0)) oldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (Input.GetMouseButton (0))
			{
				speed = Camera.main.ScreenToWorldPoint (Input.mousePosition).x - oldPos.x;
				
				backButton.transform.position =  new Vector3 (backButton.transform.position.x + Camera.main.ScreenToWorldPoint(Input.mousePosition).x - oldPos.x, backButton.transform.position.y, backButton.transform.position.z);

				foreach (GameObject activateCard in activateCards)
				{
					activateCard.transform.position = new Vector3 (activateCard.transform.position.x + Camera.main.ScreenToWorldPoint(Input.mousePosition).x - oldPos.x, activateCard.transform.position.y, activateCard.transform.position.z);
				}
			}

			speed *= 0.95f;

			if (!Input.GetMouseButton(0) && speed != 0)
			{
				backButton.transform.Translate(new Vector3 (speed, 0f, 0f));

				for (int i = 0; i < activateCards.Count; i++) {
					activateCards [i].transform.Translate(new Vector3 (speed, 0f, 0f));
				}
			}

			if (backButton.transform.position.x > -7)
			{
				backButton.transform.position = new Vector3 (-7f, backButton.transform.position.y, backButton.transform.position.z);

				for (int i = 0; i < activateCards.Count; i++) {
					activateCards [i].transform.position = new Vector3 (i / 2 * 8f, -12.5f - i % 2 * 5f, 0f);
				}
			}

			if (activateCards.Count <= 2 && backButton.transform.position.x < -7)
			{
				backButton.transform.position = new Vector3 (-7f, backButton.transform.position.y, backButton.transform.position.z);

				for (int i = 0; i < activateCards.Count; i++) {
					activateCards [i].transform.position = new Vector3 (i / 2 * 8f, -12.5f - i % 2 * 5f, 0f);
				}
			}
			if (activateCards.Count >= 3 && activateCards.Count <= 4 && backButton.transform.position.x < -11)
			{
				backButton.transform.position = new Vector3 (-11f, backButton.transform.position.y, backButton.transform.position.z);

				for (int i = 0; i < activateCards.Count; i++) {
					activateCards [i].transform.position = new Vector3 (i / 2 * 8f - 4f, -12.5f - i % 2 * 5f, 0f);
				}
			}
			if (activateCards.Count >= 5 && activateCards.Count <= 6 && backButton.transform.position.x < -19)
			{
				backButton.transform.position = new Vector3 (-19f, backButton.transform.position.y, backButton.transform.position.z);

				for (int i = 0; i < activateCards.Count; i++) {
					activateCards [i].transform.position = new Vector3 (i / 2 * 8f - 12f, -12.5f - i % 2 * 5f, 0f);
				}
			}
			if (activateCards.Count >= 7 && activateCards.Count <= 8 && backButton.transform.position.x < -27)
			{
				backButton.transform.position = new Vector3 (-27f, backButton.transform.position.y, backButton.transform.position.z);

				for (int i = 0; i < activateCards.Count; i++) {
					activateCards [i].transform.position = new Vector3 (i / 2 * 8f - 20f, -12.5f - i % 2 * 5f, 0f);
				}
			}
			if (activateCards.Count >= 9 && activateCards.Count <= 10 && backButton.transform.position.x < -35)
			{
				backButton.transform.position = new Vector3 (-35f, backButton.transform.position.y, backButton.transform.position.z);

				for (int i = 0; i < activateCards.Count; i++) {
					activateCards [i].transform.position = new Vector3 (i / 2 * 8f - 28f, -12.5f - i % 2 * 5f, 0f);
				}
			}

			oldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}
		
	void MakeProps ()
	{
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "01-A")
		{
			MakeDoorsFor01A ();
			MakeListsFor01A ();
			mapTraits.AddRange (new string[]{ WATER, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "01-B")
		{
			MakeDoorsFor01B ();
			MakeListsFor01B ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "02-A")
		{
			MakeDoorsFor02A ();
			MakeListsFor02A ();
			mapTraits.AddRange (new string[]{ WATER, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "02-B")
		{
			MakeDoorsFor02A ();
			MakeListsFor02B ();
			mapTraits.AddRange (new string[]{ DARK, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "03-A")
		{
			MakeDoorsFor03A ();
			MakeListsFor03A ();
			mapTraits.AddRange (new string[]{ WATER, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "03-B")
		{
			MakeDoorsFor03A ();
			MakeListsFor03B ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "04-A")
		{
			MakeDoorsFor04A ();
			MakeListsFor04A ();
			mapTraits.AddRange (new string[]{ BUILDING, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "04-B")
		{
			MakeDoorsFor04B ();
			MakeListsFor04B ();
			mapTraits.AddRange (new string[]{ DARK, CURSED, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "05-A")
		{
			MakeDoorsFor03A ();
			MakeListsFor03B ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "05-B")
		{
			MakeDoorsFor03A ();
			MakeListsFor03B ();
			mapTraits.AddRange (new string[]{ DARK, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "06-A")
		{
			MakeDoorsFor06A ();
			MakeListsFor06A ();
			mapTraits.AddRange (new string[]{ WATER, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "06-B")
		{
			MakeDoorsFor06A ();
			MakeListsFor06B ();
			mapTraits.AddRange (new string[]{ CIVILIZED, BUILDING, CURSED, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "07-A")
		{
			MakeDoorsFor07A ();
			MakeListsFor07A ();
			mapTraits.AddRange (new string[]{ CIVILIZED, BUILDING, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "07-B")
		{
			MakeDoorsFor07B ();
			MakeListsFor07B ();
			mapTraits.AddRange (new string[]{ DARK, COLD, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "08-A")
		{
			MakeDoorsFor08A ();
			MakeListsFor08A ();
			mapTraits.AddRange (new string[]{ BUILDING, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "08-B")
		{
			MakeDoorsFor08B ();
			MakeListsFor08B ();
			mapTraits.AddRange (new string[]{ CIVILIZED, BUILDING, CURSED, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "09-A")
		{
			MakeDoorsFor09A ();
			MakeListsFor09A ();
			mapTraits.AddRange (new string[]{ BUILDING, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "09-B")
		{
			MakeDoorsFor09A ();
			MakeListsFor09A ();
			mapTraits.AddRange (new string[]{ DARK, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "10-A")
		{
			MakeDoorsFor10A ();
			MakeListsFor10A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "10-B")
		{
			MakeDoorsFor10A ();
			MakeListsFor10A ();
			mapTraits.AddRange (new string[]{ CIVILIZED, BUILDING, CURSED, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "11-A")
		{
			MakeDoorsFor10A ();
			MakeListsFor10A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "11-B")
		{
			MakeDoorsFor10A ();
			MakeListsFor10A ();
			mapTraits.AddRange (new string[]{ BUILDING, CURSED, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "12-A")
		{
			MakeDoorsFor12A ();
			MakeListsFor12A ();
			mapTraits.AddRange (new string[]{ WATER, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "12-B")
		{
			MakeDoorsFor12B ();
			MakeListsFor12B ();
			mapTraits.AddRange (new string[]{ DARK, HOT, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "13-A")
		{
			MakeDoorsFor13A ();
			MakeListsFor13A ();
			mapTraits.AddRange (new string[]{ BUILDING, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "13-B")
		{
			MakeDoorsFor13B ();
			MakeListsFor13B ();
			mapTraits.AddRange (new string[]{ CIVILIZED, BUILDING, CURSED, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "14-A")
		{
			MakeDoorsFor10A ();
			MakeListsFor14A ();
			mapTraits.AddRange (new string[]{ BUILDING, CURSED, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "14-B")
		{
			MakeDoorsFor10A ();
			MakeListsFor14B ();
			mapTraits.AddRange (new string[]{ CIVILIZED, BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "15-A")
		{
			MakeDoorsFor15A ();
			MakeListsFor15A ();
			mapTraits.AddRange (new string[]{ MOUNTAIN, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "15-B")
		{
			MakeDoorsFor15B ();
			MakeListsFor15A ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "16-A")
		{
			MakeDoorsFor16A ();
			MakeListsFor16A ();
			mapTraits.AddRange (new string[]{ MOUNTAIN, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "16-B")
		{
			MakeDoorsFor16A ();
			MakeListsFor16A ();
			mapTraits.AddRange (new string[]{ CIVILIZED, BUILDING, CURSED, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "17-A")
		{
			MakeDoorsFor17A ();
			MakeListsFor17A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "17-B")
		{
			MakeDoorsFor17B ();
			MakeListsFor17B ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "18-A")
		{
			MakeDoorsFor17A ();
			MakeListsFor17A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "18-B")
		{
			MakeDoorsFor17B ();
			MakeListsFor17B ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "19-A")
		{
			MakeDoorsFor19A ();
			MakeListsFor19A ();
			mapTraits.AddRange (new string[]{ BUILDING, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "19-B")
		{
			MakeDoorsFor19B ();
			MakeListsFor19B ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "20-A")
		{
			MakeDoorsFor20A ();
			MakeListsFor20A ();
			mapTraits.AddRange (new string[]{ MOUNTAIN, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "20-B")
		{
			MakeDoorsFor20B ();
			MakeListsFor20B ();
			mapTraits.AddRange (new string[]{ DARK, BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "21-A")
		{
			MakeDoorsFor19B ();
			MakeListsFor21A ();
			mapTraits.AddRange (new string[]{ WATER, WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "21-B")
		{
			MakeDoorsFor19A ();
			MakeListsFor21B ();
			mapTraits.AddRange (new string[]{ BUILDING, WATER, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "22-A")
		{
			MakeDoorsFor06A ();
			MakeListsFor22A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "22-B")
		{
			MakeDoorsFor06A ();
			MakeListsFor22A ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "23-A")
		{
			MakeDoorsFor23A ();
			MakeListsFor23A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "23-B")
		{
			MakeDoorsFor23A ();
			MakeListsFor23A ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "24-A")
		{
			MakeDoorsFor24A ();
			MakeListsFor24A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "24-B")
		{
			MakeDoorsFor24B ();
			MakeListsFor24B ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "25-A")
		{
			MakeDoorsFor24B ();
			MakeListsFor24B ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "25-B")
		{
			MakeDoorsFor24A ();
			MakeListsFor24A ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "26-A")
		{
			MakeDoorsFor26A ();
			MakeListsFor23A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "26-B")
		{
			MakeDoorsFor26A ();
			MakeListsFor23A ();
			mapTraits.AddRange (new string[]{ DARK, BUILDING, CURSED, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "27-A")
		{
			MakeDoorsFor27A ();
			MakeListsFor27A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "27-B")
		{
			MakeDoorsFor27B ();
			MakeListsFor27B ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "28-A")
		{
			MakeDoorsFor28A ();
			MakeListsFor28A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "28-B")
		{
			MakeDoorsFor28B ();
			MakeListsFor28B ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "29-A")
		{
			MakeDoorsFor10A ();
			MakeListsFor16A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "29-B")
		{
			MakeDoorsFor10A ();
			MakeListsFor16A ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "30-A")
		{
			MakeDoorsFor10A ();
			MakeListsFor16A ();
			mapTraits.AddRange (new string[]{ WILDERNESS });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "30-B")
		{
			MakeDoorsFor10A ();
			MakeListsFor16A ();
			mapTraits.AddRange (new string[]{ BUILDING, CAVE });
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "Transition-indoor")
		{
			MakeDoorsForTransitionIndoor ();
			isItOut = false;
		}
		if (map.GetComponent<SpriteRenderer> ().sprite.name == "Transition-outdoor")
		{
			MakeDoorsForTransitionIndoor ();
			isItOut = true;
		}

		if (mapsOut.Count == 0)
		{
			for(int i = 0; i < map.transform.childCount; i++){
				if (map.transform.GetChild (i).name == "Exit(Clone)") map.transform.GetChild (i).GetComponent<SpriteRenderer> ().sprite = null;
				else if (map.transform.GetChild (i).name == "Yellow-Door(Clone)")
				{
					if (isItOut == true) map.transform.GetChild (i).GetComponent<SpriteRenderer> ().sprite = deadEndOut;
					if (isItOut == false) map.transform.GetChild (i).GetComponent<SpriteRenderer> ().sprite = deadEndIn;
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
			new Vector3 (-1.5f,	2.5f,	0f),
			new Vector3 (1.5f,	2.5f,	0f),
			new Vector3 (2.5f,	2.5f,	0f),
			new Vector3 (3.5f,	2.5f,	0f),

			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),
			new Vector3 (3.5f,	1.5f,	0f),

			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
			new Vector3 (3.5f,	-1.5f,	0f),

			new Vector3 (-1.5f,	-2.5f,	0f),
			new Vector3 (1.5f,	-2.5f,	0f),
			new Vector3 (2.5f,	-2.5f,	0f),
			new Vector3 (3.5f,	-2.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (2f,	2.5f,	0f),

			new Vector3 (-1f,	1.5f,	0f),
			new Vector3 (1f,	1.5f,	0f),
			new Vector3 (3f,	1.5f,	0f),

			new Vector3 (0f,	0.5f,	0f),
			new Vector3 (2f,	0.5f,	0f),

			new Vector3 (0f,	-0.5f,	0f),
			new Vector3 (2f,	-0.5f,	0f),

			new Vector3 (-1f,	-1.5f,	0f),
			new Vector3 (1f,	-1.5f,	0f),
			new Vector3 (3f,	-1.5f,	0f),

			new Vector3 (2f,	-2.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (3f,	2f,		0f),

			new Vector3 (-1f,	1f,		0f),
			new Vector3 (1f,	1f,		0f),

			new Vector3 (-1f,	-1f,	0f),
			new Vector3 (1f,	-1f,	0f),

			new Vector3 (3f,	-2f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (2.5f,	2f,		0f),

			new Vector3 (-0.5f,	1f,		0f),

			new Vector3 (-0.5f,	-1f,	0f),

			new Vector3 (2.5f,	-2f,	0f),
		});
	}

	void MakeListsFor01B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-3.5f,	2.5f,	0f),
			new Vector3 (-2.5f,	2.5f,	0f),
			new Vector3 (1.5f,	2.5f,	0f),
			new Vector3 (2.5f,	2.5f,	0f),
			new Vector3 (3.5f,	2.5f,	0f),

			new Vector3 (-3.5f,	1.5f,	0f),
			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),
			new Vector3 (3.5f,	1.5f,	0f),

			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (-3.5f,	-1.5f,	0f),
			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
			new Vector3 (3.5f,	-1.5f,	0f),

			new Vector3 (-3.5f,	-2.5f,	0f),
			new Vector3 (-2.5f,	-2.5f,	0f),
			new Vector3 (1.5f,	-2.5f,	0f),
			new Vector3 (2.5f,	-2.5f,	0f),
			new Vector3 (3.5f,	-2.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-3f,	2.5f,	0f),
			new Vector3 (2f,	2.5f,	0f),

			new Vector3 (-3f,	1.5f,	0f),
			new Vector3 (1f,	1.5f,	0f),
			new Vector3 (3f,	1.5f,	0f),

			new Vector3 (-2f,	0.5f,	0f),
			new Vector3 (0f,	0.5f,	0f),
			new Vector3 (2f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (0f,	-0.5f,	0f),
			new Vector3 (2f,	-0.5f,	0f),

			new Vector3 (-3f,	-1.5f,	0f),
			new Vector3 (1f,	-1.5f,	0f),
			new Vector3 (3f,	-1.5f,	0f),

			new Vector3 (-3f,	-2.5f,	0f),
			new Vector3 (2f,	-2.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-3f,	2f,		0f),
			new Vector3 (2f,	2f,		0f),

			new Vector3 (0f,	1f,		0f),

			new Vector3 (-2f,	0f,		0f),
			new Vector3 (2f,	0f,		0f),

			new Vector3 (0f,	-1f,	0f),

			new Vector3 (-3f,	-2f,	0f),
			new Vector3 (2f,	-2f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (2.5f,	2f,		0f),

			new Vector3 (-1.5f,	0f,		0f),
			new Vector3 (1.5f,	0f,		0f),

			new Vector3 (2.5f,	-2f,	0f),
		});
	}

	void MakeListsFor02A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	2.5f,	0f),
			new Vector3 (-1.5f,	2.5f,	0f),
			new Vector3 (1.5f,	2.5f,	0f),
			new Vector3 (2.5f,	2.5f,	0f),

			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-1.5f,	1.5f,	0f),

			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),

			new Vector3 (-2.5f,	-2.5f,	0f),
			new Vector3 (-1.5f,	-2.5f,	0f),
			new Vector3 (1.5f,	-2.5f,	0f),
			new Vector3 (2.5f,	-2.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	2.5f,	0f),
			new Vector3 (2f,	2.5f,	0f),

			new Vector3 (-2f,	1.5f,	0f),

			new Vector3 (-2f,	0.5f,	0f),
			new Vector3 (2f,	0.5f,	0f),

			new Vector3 (1f,	-0.5f,	0f),

			new Vector3 (-2f,	-1.5f,	0f),
			new Vector3 (0f,	-1.5f,	0f),
			new Vector3 (2f,	-1.5f,	0f),

			new Vector3 (-2f,	-2.5f,	0f),
			new Vector3 (2f,	-2.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-2f,	2f,		0f),

			new Vector3 (2f,	0f,		0f),

			new Vector3 (-2f,	-2f,	0f),
			new Vector3 (2f,	-2f,	0f),
		});
	}

	void MakeListsFor02B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	2.5f,	0f),
			new Vector3 (-1.5f,	2.5f,	0f),
			new Vector3 (1.5f,	2.5f,	0f),
			new Vector3 (2.5f,	2.5f,	0f),

			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),

			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),

			new Vector3 (-2.5f,	-2.5f,	0f),
			new Vector3 (-1.5f,	-2.5f,	0f),
			new Vector3 (1.5f,	-2.5f,	0f),
			new Vector3 (2.5f,	-2.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	2.5f,	0f),
			new Vector3 (2f,	2.5f,	0f),

			new Vector3 (-2f,	1.5f,	0f),
			new Vector3 (0f,	1.5f,	0f),
			new Vector3 (2f,	1.5f,	0f),

			new Vector3 (-2f,	0.5f,	0f),
			new Vector3 (0f,	0.5f,	0f),
			new Vector3 (2f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (0f,	-0.5f,	0f),
			new Vector3 (2f,	-0.5f,	0f),

			new Vector3 (-2f,	-1.5f,	0f),
			new Vector3 (0f,	-1.5f,	0f),
			new Vector3 (2f,	-1.5f,	0f),

			new Vector3 (-2f,	-2.5f,	0f),
			new Vector3 (2f,	-2.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-2f,	2f,		0f),
			new Vector3 (2f,	2f,		0f),

			new Vector3 (0f,	1f,		0f),

			new Vector3 (-2f,	0f,		0f),
			new Vector3 (2f,	0f,		0f),

			new Vector3 (0f,	-1f,	0f),

			new Vector3 (-2f,	-2f,	0f),
			new Vector3 (2f,	-2f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	1f,		0f),
			new Vector3 (1.5f,	1f,		0f),

			new Vector3 (-1.5f,	-1f,	0f),
			new Vector3 (1.5f,	-1f,	0f),
		});
	}

	void MakeListsFor03A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-1f,	0.5f,	0f),
			new Vector3 (1f,	0.5f,	0f),

			new Vector3 (-1f,	-0.5f,	0f),
			new Vector3 (1f,	-0.5f,	0f),

			new Vector3 (-2f,	-1.5f,	0f),
			new Vector3 (2f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-1f,	0f,		0f),
			new Vector3 (1f,	0f,		0f),
		});
	}

	void MakeListsFor03B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),

			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	1.5f,	0f),
			new Vector3 (0f,	1.5f,	0f),
			new Vector3 (2f,	1.5f,	0f),

			new Vector3 (-1f,	0.5f,	0f),
			new Vector3 (1f,	0.5f,	0f),

			new Vector3 (-1f,	-0.5f,	0f),
			new Vector3 (1f,	-0.5f,	0f),

			new Vector3 (-2f,	-1.5f,	0f),
			new Vector3 (2f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-1f,	0f,		0f),
			new Vector3 (1f,	0f,		0f),
		});
	}

	void MakeListsFor04A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	2.5f,	0f),
			new Vector3 (-1.5f,	2.5f,	0f),
			new Vector3 (1.5f,	2.5f,	0f),

			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),

			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),

			new Vector3 (-0.5f,	-2.5f,	0f),
			new Vector3 (0.5f,	-2.5f,	0f),
			new Vector3 (1.5f,	-2.5f,	0f),
			new Vector3 (2.5f,	-2.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	2.5f,	0f),

			new Vector3 (-2f,	1.5f,	0f),
			new Vector3 (0f,	1.5f,	0f),

			new Vector3 (-1f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (0f,	-0.5f,	0f),

			new Vector3 (-1f,	-1.5f,	0f),
			new Vector3 (1f,	-1.5f,	0f),

			new Vector3 (0f,	-2.5f,	0f),
			new Vector3 (2f,	-2.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-2f,	2f,		0f),

			new Vector3 (-2f,	0f,		0f),

			new Vector3 (0f,	-2f,	0f),
			new Vector3 (2f,	-2f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0f,		0f),

			new Vector3 (0.5f,	-2f,	0f),
		});
	}

	void MakeListsFor04B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	2.5f,	0f),
			new Vector3 (-1.5f,	2.5f,	0f),
			new Vector3 (1.5f,	2.5f,	0f),
			new Vector3 (2.5f,	2.5f,	0f),

			new Vector3 (1.5f,	1.5f,	0f),

			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),

			new Vector3 (-2.5f,	-2.5f,	0f),
			new Vector3 (-1.5f,	-2.5f,	0f),
			new Vector3 (-0.5f,	-2.5f,	0f),
			new Vector3 (0.5f,	-2.5f,	0f),
			new Vector3 (1.5f,	-2.5f,	0f),
			new Vector3 (2.5f,	-2.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	2.5f,	0f),
			new Vector3 (2f,	2.5f,	0f),

			new Vector3 (-1f,	0.5f,	0f),
			new Vector3 (1f,	0.5f,	0f),

			new Vector3 (-1f,	-0.5f,	0f),
			new Vector3 (1f,	-0.5f,	0f),

			new Vector3 (-2f,	-1.5f,	0f),
			new Vector3 (0f,	-1.5f,	0f),
			new Vector3 (2f,	-1.5f,	0f),

			new Vector3 (-2f,	-2.5f,	0f),
			new Vector3 (0f,	-2.5f,	0f),
			new Vector3 (2f,	-2.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-1f,	0f,		0f),
			new Vector3 (1f,	0f,		0f),

			new Vector3 (-2f,	-2f,	0f),
			new Vector3 (0f,	-2f,	0f),
			new Vector3 (2f,	-2f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (0.5f,	0f,		0f),

			new Vector3 (-1.5f,	-2f,	0f),
			new Vector3 (1.5f,	-2f,	0f),
		});
	}

	void MakeListsFor06A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),

			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	1.5f,	0f),
			new Vector3 (2f,	1.5f,	0f),

			new Vector3 (1f,	0.5f,	0f),

			new Vector3 (1f,	-0.5f,	0f),

			new Vector3 (-2f,	-1.5f,	0f),
			new Vector3 (2f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (1f,	1f,		0f),

			new Vector3 (1f,	-1f,	0f),
		});
	}

	void MakeListsFor06B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),

			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	1.5f,	0f),
			new Vector3 (0f,	1.5f,	0f),
			new Vector3 (2f,	1.5f,	0f),

			new Vector3 (-1f,	0.5f,	0f),
			new Vector3 (1f,	0.5f,	0f),

			new Vector3 (-1f,	-0.5f,	0f),
			new Vector3 (1f,	-0.5f,	0f),

			new Vector3 (-2f,	-1.5f,	0f),
			new Vector3 (0f,	-1.5f,	0f),
			new Vector3 (2f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-1f,	1f,		0f),
			new Vector3 (1f,	1f,		0f),

			new Vector3 (-1f,	-1f,	0f),
			new Vector3 (1f,	-1f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (0.5f,	1f,		0f),

			new Vector3 (-0.5f,	-1f,	0f),
		});
	}

	void MakeListsFor07A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-1.5f,	1.5f,	0f),

			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	1.5f,	0f),

			new Vector3 (-2f,	0.5f,	0f),
			new Vector3 (0f,	0.5f,	0f),
			new Vector3 (2f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (0f,	-0.5f,	0f),
			new Vector3 (2f,	-0.5f,	0f),

			new Vector3 (2f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-2f,	1f,		0f),

			new Vector3 (0f,	0f,		0f),

			new Vector3 (2f,	-1f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0f,		0f),
			new Vector3 (1.5f,	0f,		0f),
		});
	}

	void MakeListsFor07B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),

			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (1f,	1.5f,	0f),

			new Vector3 (-2f,	0.5f,	0f),
			new Vector3 (0f,	0.5f,	0f),
			new Vector3 (2f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (0f,	-0.5f,	0f),
			new Vector3 (2f,	-0.5f,	0f),

			new Vector3 (-1f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (2f,	1f,		0f),

			new Vector3 (0f,	0f,		0f),

			new Vector3 (-2f,	-1f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (1.5f,	0f,		0f),
			new Vector3 (-1.5f,	0f,		0f),
		});
	}

	void MakeListsFor08A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),

			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-1f,	0.5f,	0f),

			new Vector3 (0f,	-0.5f,	0f),

			new Vector3 (-1f,	-1.5f,	0f),
			new Vector3 (1f,	-1.5f,	0f),
		});
	}

	void MakeListsFor08B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),

			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (1f,	0.5f,	0f),

			new Vector3 (0f,	-0.5f,	0f),

			new Vector3 (-1f,	-1.5f,	0f),
			new Vector3 (1f,	-1.5f,	0f),
		});
	}

	void MakeListsFor09A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),

			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (0f,	0.5f,	0f),

			new Vector3 (0f,	-0.5f,	0f),

			new Vector3 (-1f,	-1.5f,	0f),
			new Vector3 (1f,	-1.5f,	0f),
		});
	}

	void MakeListsFor10A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),

			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-1f,	1.5f,	0f),
			new Vector3 (1f,	1.5f,	0f),

			new Vector3 (0f,	0.5f,	0f),

			new Vector3 (0f,	-0.5f,	0f),

			new Vector3 (-1f,	-1.5f,	0f),
			new Vector3 (1f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (0f,	1f,		0f),

			new Vector3 (0f,	-1f,	0f),
		});
	}

	void MakeListsFor12A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),

			new Vector3 (-1.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
		});
	}

	void MakeListsFor12B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),

			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-1f,	1.5f,	0f),
			new Vector3 (1f,	1.5f,	0f),

			new Vector3 (-1f,	0.5f,	0f),
			new Vector3 (1f,	0.5f,	0f),

			new Vector3 (1f,	-0.5f,	0f),

			new Vector3 (1f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-1f,	1f,		0f),
			new Vector3 (1f,	1f,		0f),

			new Vector3 (1f,	-1f,	0f),
		});
	}

	void MakeListsFor13A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	2.5f,	0f),
			new Vector3 (1.5f,	2.5f,	0f),

			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),

			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	1.5f,	0f),
			new Vector3 (0f,	1.5f,	0f),
			new Vector3 (2f,	1.5f,	0f),

			new Vector3 (-2f,	0.5f,	0f),
			new Vector3 (0f,	0.5f,	0f),
			new Vector3 (2f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (0f,	-0.5f,	0f),
			new Vector3 (2f,	-0.5f,	0f),

			new Vector3 (-2f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-2f,	1f,		0f),
			new Vector3 (2f,	1f,		0f),

			new Vector3 (0f,	0f,		0f),

			new Vector3 (-2f,	-1f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0f,		0f),

			new Vector3 (1.5f,	0f,		0f),
		});
	}

	void MakeListsFor13B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	2.5f,	0f),
			new Vector3 (1.5f,	2.5f,	0f),

			new Vector3 (-2.5f,	1.5f,	0f),
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),
			new Vector3 (2.5f,	1.5f,	0f),

			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (2.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	1.5f,	0f),
			new Vector3 (0f,	1.5f,	0f),
			new Vector3 (2f,	1.5f,	0f),

			new Vector3 (-2f,	0.5f,	0f),
			new Vector3 (0f,	0.5f,	0f),
			new Vector3 (2f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (0f,	-0.5f,	0f),
			new Vector3 (2f,	-0.5f,	0f),

			new Vector3 (2f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-2f,	1f,		0f),
			new Vector3 (2f,	1f,		0f),

			new Vector3 (0f,	0f,		0f),

			new Vector3 (2f,	-1f,	0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0f,		0f),

			new Vector3 (1.5f,	0f,		0f),
		});
	}

	void MakeListsFor14A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),
			new Vector3 (1.5f,	1.5f,	0f),

			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),

			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (1f,	1.5f,	0f),

			new Vector3 (0f,	0.5f,	0f),

			new Vector3 (0f,	-0.5f,	0f),

			new Vector3 (1f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (0f,	1f,		0f),

			new Vector3 (0f,	-1f,	0f),
		});
	}

	void MakeListsFor14B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	1.5f,	0f),
			new Vector3 (-0.5f,	1.5f,	0f),
			new Vector3 (0.5f,	1.5f,	0f),

			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-1f,	1.5f,	0f),

			new Vector3 (0f,	0.5f,	0f),

			new Vector3 (0f,	-0.5f,	0f),

			new Vector3 (-1f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (0f,	1f,		0f),

			new Vector3 (0f,	-1f,	0f),
		});
	}

	void MakeListsFor15A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-2f,	0.5f,	0f),
			new Vector3 (1f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (1f,	-0.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-2f,	0f,		0f),

			new Vector3 (1f,	0f,		0f),
		});
	}

	void MakeListsFor16A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
		});
	}

	void MakeListsFor17A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
		});
	}

	void MakeListsFor17B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
	}

	void MakeListsFor19A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (0f,	0.5f,	0f),
			new Vector3 (2f,	0.5f,	0f),

			new Vector3 (-1f,	-0.5f,	0f),
			new Vector3 (1f,	-0.5f,	0f),

			new Vector3 (-2f,	-1.5f,	0f),
			new Vector3 (0f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (1f,	0f,		0f),

			new Vector3 (-1f,	-1f,	0f),
		});
	}

	void MakeListsFor19B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (0f,	0.5f,	0f),
			new Vector3 (-2f,	0.5f,	0f),

			new Vector3 (-1f,	-0.5f,	0f),
			new Vector3 (1f,	-0.5f,	0f),

			new Vector3 (2f,	-1.5f,	0f),
			new Vector3 (0f,	-1.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-1f,	0f,		0f),

			new Vector3 (1f,	-1f,	0f),
		});
	}

	void MakeListsFor20A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-3.5f,	0.5f,	0f),
			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),
			new Vector3 (3.5f,	0.5f,	0f),

			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),
			new Vector3 (3.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-3f,	0.5f,	0f),
			new Vector3 (-1f,	0.5f,	0f),
			new Vector3 (1f,	0.5f,	0f),
			new Vector3 (3f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (0f,	-0.5f,	0f),
			new Vector3 (2f,	-0.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-2f,	0f,		0f),
			new Vector3 (0f,	0f,		0f),
			new Vector3 (2f,	0f,		0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0f,		0f),
			new Vector3 (1.5f,	0f,		0f),
		});
	}

	void MakeListsFor20B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-3.5f,	0.5f,	0f),
			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),
			new Vector3 (3.5f,	0.5f,	0f),

			new Vector3 (-3.5f,	-0.5f,	0f),
			new Vector3 (-2.5f,	-0.5f,	0f),
			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
			new Vector3 (2.5f,	-0.5f,	0f),

			new Vector3 (2.5f,	-1.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-3f,	0.5f,	0f),
			new Vector3 (-1f,	0.5f,	0f),
			new Vector3 (1f,	0.5f,	0f),
			new Vector3 (3f,	0.5f,	0f),

			new Vector3 (-2f,	-0.5f,	0f),
			new Vector3 (0f,	-0.5f,	0f),
			new Vector3 (2f,	-0.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-2f,	0f,		0f),
			new Vector3 (0f,	0f,		0f),
			new Vector3 (2f,	0f,		0f),
		});
		spawn3x2.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0f,		0f),
			new Vector3 (1.5f,	0f,		0f),
		});
	}

	void MakeListsFor21A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (-0.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
			new Vector3 (2.5f,	-1.5f,	0f),
		});
	}

	void MakeListsFor21B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),

			new Vector3 (-2.5f,	-1.5f,	0f),
			new Vector3 (-1.5f,	-1.5f,	0f),
			new Vector3 (1.5f,	-1.5f,	0f),
		});
	}

	void MakeListsFor22A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
		});
		spawn2x1.AddRange (new Vector3[] {
			new Vector3 (-1f,	0.5f,	0f),
			new Vector3 (1f,	0.5f,	0f),

			new Vector3 (-1f,	-0.5f,	0f),
			new Vector3 (1f,	-0.5f,	0f),
		});
		spawn2x2.AddRange (new Vector3[] {
			new Vector3 (-1f,	0f,		0f),
			new Vector3 (1f,	0f,		0f),
		});
	}

	void MakeListsFor23A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
		});
	}

	void MakeListsFor24A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-2.5f,	0.5f,	0f),
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
			new Vector3 (1.5f,	-0.5f,	0f),
		});
	}

	void MakeListsFor24B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0.5f,	0f),
			new Vector3 (-0.5f,	0.5f,	0f),
			new Vector3 (0.5f,	0.5f,	0f),
			new Vector3 (1.5f,	0.5f,	0f),
			new Vector3 (2.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),
			new Vector3 (-0.5f,	-0.5f,	0f),
			new Vector3 (0.5f,	-0.5f,	0f),
		});
	}

	void MakeListsFor27A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-0.5f,	1.5f,	0f),

			new Vector3 (-0.5f,	0.5f,	0f),

			new Vector3 (0.5f,	-0.5f,	0f),

			new Vector3 (0.5f,	-1.5f,	0f),
		});
	}

	void MakeListsFor27B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (0.5f,	1.5f,	0f),

			new Vector3 (0.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),

			new Vector3 (-0.5f,	-1.5f,	0f),
		});
	}

	void MakeListsFor28A()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (-1.5f,	0.5f,	0f),

			new Vector3 (-0.5f,	0.5f,	0f),

			new Vector3 (0.5f,	-0.5f,	0f),

			new Vector3 (1.5f,	-0.5f,	0f),
		});
	}

	void MakeListsFor28B()
	{
		spawn1x1.AddRange (new Vector3[] {
			new Vector3 (0.5f,	0.5f,	0f),

			new Vector3 (1.5f,	0.5f,	0f),

			new Vector3 (-1.5f,	-0.5f,	0f),

			new Vector3 (-0.5f,	-0.5f,	0f),
		});
	}

	#endregion

	#region MakeMonster

	void MakeMonsters()
	{
		List<List<Vector3>> spawnGroups = new List<List<Vector3>>();
		if (spawn1x1.Count > 0) spawnGroups.Add (spawn1x1);
		if (spawn2x1.Count > 0) spawnGroups.Add (spawn2x1);
		if (spawn2x2.Count > 0) spawnGroups.Add (spawn2x2);
		if (spawn3x2.Count > 0) spawnGroups.Add (spawn3x2);
		if (spawnGroups.Count > 0) {
			List<Vector3> selectedSpawnGroup;

			while(true){
				selectedSpawnGroup = spawnGroups [Random.Range (0, spawnGroups.Count)];
				bool isOk = false;
				foreach (string trait in mapTraits) {
					if (selectedSpawnGroup == spawn1x1)
					{
						foreach (GameObject monster in red1x1)
						{
							if (monstersTrait1 [monster.name] == trait) isOk = true;
							if (monstersTrait2 [monster.name] == trait) isOk = true;
						}
					}
					if (selectedSpawnGroup == spawn2x1)
					{
						foreach (GameObject monster in red2x1)
						{
							if (monstersTrait1 [monster.name] == trait) isOk = true;
							if (monstersTrait2 [monster.name] == trait) isOk = true;
						}
					}
					if (selectedSpawnGroup == spawn2x2)
					{
						foreach (GameObject monster in red2x2)
						{
							if (monstersTrait1 [monster.name] == trait) isOk = true;
							if (monstersTrait2 [monster.name] == trait) isOk = true;
						}
					}
					if (selectedSpawnGroup == spawn3x2)
					{
						foreach (GameObject monster in red3x2)
						{
							if (monstersTrait1 [monster.name] == trait) isOk = true;
							if (monstersTrait2 [monster.name] == trait) isOk = true;
						}
					}
				}
				if (isOk == true) break;
			}

			if (selectedSpawnGroup == spawn1x1) {
				while (true) {
					int randomNumber = Random.Range (0, red1x1.Count);
					bool isOk = false;
					foreach (string trait in mapTraits) {
						if (trait == monstersTrait1 [red1x1 [randomNumber].name])
							isOk = true;
						if (trait == monstersTrait2 [red1x1 [randomNumber].name])
							isOk = true;
					}
					if (isOk == true) {
						SpawnMonsters (randomNumber, red1x1);
						break;
					}
				}
			}
			if (selectedSpawnGroup == spawn2x1) {
				while (true) {
					int randomNumber = Random.Range (0, red2x1.Count);
					bool isOk = false;
					foreach (string trait in mapTraits) {
						if (trait == monstersTrait1 [red2x1 [randomNumber].name])
							isOk = true;
						if (trait == monstersTrait2 [red2x1 [randomNumber].name])
							isOk = true;
					}
					if (isOk == true) {
						SpawnMonsters (randomNumber, red2x1);
						break;
					}
				}
			}
			if (selectedSpawnGroup == spawn2x2) {
				while (true) {
					int randomNumber = Random.Range (0, red2x2.Count);
					bool isOk = false;
					foreach (string trait in mapTraits) {
						if (trait == monstersTrait1 [red2x2 [randomNumber].name])
							isOk = true;
						if (trait == monstersTrait2 [red2x2 [randomNumber].name])
							isOk = true;
					}
					if (isOk == true) {
						SpawnMonsters (randomNumber, red2x2);
						break;
					}
				}
			}
			if (selectedSpawnGroup == spawn3x2) {
				while (true) {
					int randomNumber = Random.Range (0, red3x2.Count);
					bool isOk = false;
					foreach (string trait in mapTraits) {
						if (trait == monstersTrait1 [red3x2 [randomNumber].name])
							isOk = true;
						if (trait == monstersTrait2 [red3x2 [randomNumber].name])
							isOk = true;
					}
					if (isOk == true) {
						SpawnMonsters (randomNumber, red3x2);
						break;
					}
				}
			}
		}
	}

	void SpawnMonsters (int monsterNumber, List<GameObject> monsterGroups)
	{
		if (monsterGroups [monsterNumber].name == "Monster-Dragon-Red")
		{
			int randomNumber = Random.Range (0, spawn3x2.Count);
			SpawnMonster (monsterNumber, red3x2, spawn3x2 [randomNumber], 2);

			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (-1f, 0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (0f, 0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (1f, 0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (-1f, -0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (0f, -0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (1f, -0.5f, 0f));

			spawn3x2.Remove (spawn3x2 [randomNumber]);

			randomNumber = Random.Range (0, spawn3x2.Count);
			SpawnMonster (monsterNumber, tan3x2, spawn3x2 [randomNumber], 0);

			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (-1f, 0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (0f, 0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (1f, 0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (-1f, -0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (0f, -0.5f, 0f));
			spawn1x1.Remove (spawn3x2 [randomNumber] + new Vector3 (1f, -0.5f, 0f));

			spawn3x2.Remove (spawn3x2 [randomNumber]);
		}
		if (monsterGroups [monsterNumber].name == "Monster-Elemental-Red" || monsterGroups [monsterNumber].name == "Monster-Ettin-Red" || monsterGroups [monsterNumber].name == "Monster-Merriod-Red")
		{
			int randomNumber = Random.Range (0, spawn2x2.Count);
			SpawnMonster (monsterNumber, red2x2, spawn2x2 [randomNumber], 2);

			spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (-0.5f, 0.5f, 0f));
			spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (0.5f, 0.5f, 0f));
			spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (-0.5f, -0.5f, 0f));
			spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (0.5f, -0.5f, 0f));
							
			spawn2x2.Remove (spawn2x2 [randomNumber]);

			randomNumber = Random.Range (0, spawn2x2.Count);
			SpawnMonster (monsterNumber, tan2x2, spawn2x2 [randomNumber], 0);

			spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (-0.5f, 0.5f, 0f));
			spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (0.5f, 0.5f, 0f));
			spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (-0.5f, -0.5f, 0f));
			spawn1x1.Remove (spawn2x2 [randomNumber] + new Vector3 (0.5f, -0.5f, 0f));

			spawn2x2.Remove (spawn2x2 [randomNumber]);
		}
		if (spawn2x1.Count >= 4 && monsterGroups [monsterNumber].name == "Monster-Wolf-Red")
		{
			int randomNumber = Random.Range (0, spawn2x1.Count);
			SpawnMonster (monsterNumber, red2x1, spawn2x1 [randomNumber], 0);

			spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (-0.5f, 0f, 0f));
			spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (0.5f, 0f, 0f));

			spawn2x1.Remove (spawn2x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn2x1.Count);
			SpawnMonster (monsterNumber, tan2x1, spawn2x1 [randomNumber], 0);

			spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (-0.5f, 0f, 0f));
			spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (0.5f, 0f, 0f));

			spawn2x1.Remove (spawn2x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn2x1.Count);
			SpawnMonster (monsterNumber, tan2x1, spawn2x1 [randomNumber], 1);

			spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (-0.5f, 0f, 0f));
			spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (0.5f, 0f, 0f));

			spawn2x1.Remove (spawn2x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn2x1.Count);
			SpawnMonster (monsterNumber, tan2x1, spawn2x1 [randomNumber], 2);

			spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (-0.5f, 0f, 0f));
			spawn1x1.Remove (spawn2x1 [randomNumber] + new Vector3 (0.5f, 0f, 0f));

			spawn2x1.Remove (spawn2x1 [randomNumber]);
		}
		if (spawn1x1.Count >= 4 && monsterGroups [monsterNumber].name == "Monster-Flesh-Red")
		{
			int randomNumber = Random.Range (0, spawn1x1.Count);
			SpawnMonster (monsterNumber, red1x1, spawn1x1 [randomNumber], 0);
			spawn1x1.Remove (spawn1x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn1x1.Count);
			SpawnMonster (monsterNumber, tan1x1, spawn1x1 [randomNumber], 0);
			spawn1x1.Remove (spawn1x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn1x1.Count);
			SpawnMonster (monsterNumber, tan1x1, spawn1x1 [randomNumber], 1);
			spawn1x1.Remove (spawn1x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn1x1.Count);
			SpawnMonster (monsterNumber, tan1x1, spawn1x1 [randomNumber], 2);
			spawn1x1.Remove (spawn1x1 [randomNumber]);
		}
		if (spawn1x1.Count >= 5 && (monsterGroups [monsterNumber].name == "Monster-Spider-Red" || monsterGroups [monsterNumber].name == "Monster-Goblin-Red" || monsterGroups [monsterNumber].name == "Monster-Zombie-Red"))
		{
			int randomNumber = Random.Range (0, spawn1x1.Count);
			SpawnMonster (monsterNumber, red1x1, spawn1x1 [randomNumber], 0);
			spawn1x1.Remove (spawn1x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn1x1.Count);
			SpawnMonster (monsterNumber, tan1x1, spawn1x1 [randomNumber], 0);
			spawn1x1.Remove (spawn1x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn1x1.Count);
			SpawnMonster (monsterNumber, tan1x1, spawn1x1 [randomNumber], 0);
			spawn1x1.Remove (spawn1x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn1x1.Count);
			SpawnMonster (monsterNumber, tan1x1, spawn1x1 [randomNumber], 1);
			spawn1x1.Remove (spawn1x1 [randomNumber]);

			randomNumber = Random.Range (0, spawn1x1.Count);
			SpawnMonster (monsterNumber, tan1x1, spawn1x1 [randomNumber], 2);
			spawn1x1.Remove (spawn1x1 [randomNumber]);
   		}
	}

	void SpawnMonster(int monsterNumber, List<GameObject> monsterGroups, Vector3 spawnLocation, int numberOfPlayers)
	{
		GameObject newMonster = Instantiate (monsterGroups [monsterNumber]);
		newMonster.transform.parent = map.transform;
		newMonster.transform.localPosition = spawnLocation;
		newMonster.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = playerNumber [numberOfPlayers];
		currentMonster = newMonster.name.Substring(8,newMonster.name.Length-19);
	}

	#endregion

	void MakeQuest ()
	{
		//if(currentMonster != "") possibleQuests.Add ("killAll" + currentMonster + "s");

		foreach (string key in standardQuestsSpace.Keys)
		{
			if (standardQuestsSpace [key] <= spawn1x1.Count)
			{
				possibleQuests.Add (key);
			}
		}

		foreach (string key in standardQuestsMonster.Keys)
		{
			if (standardQuestsMonster [key] == currentMonster)
			{
				possibleQuests.Add (key);
			}
		}

		foreach (string key in baseGameUniqueQuestsSpace.Keys)
		{
			if (baseGameUniqueQuestsSpace [key] <= spawn1x1.Count)
			{
				possibleQuests.Add (key);
			}
		}

		foreach (string key in baseGameUniqueQuestsMonster.Keys)
		{
			if (baseGameUniqueQuestsMonster [key] == currentMonster)
			{
				possibleQuests.Add (key);
			}
		}

		foreach (string key in nullUniqueQuestsSpace.Keys)
		{
			if (nullUniqueQuestsSpace [key] <= spawn1x1.Count)
			{
				possibleQuests.Add (key);
			}
		}

		foreach (string key in nullUniqueQuestsMonster.Keys)
		{
			if (nullUniqueQuestsMonster [key] == currentMonster)
			{
				possibleQuests.Add (key);
			}
		}

		if(possibleQuests.Count > 0) MakeQuestFromWWW (possibleQuests [Random.Range (0, possibleQuests.Count)]);
		if (possibleQuests.Count == 0)
		{
			nameText.GetComponent<Text> ().text = "Nothing happens";
			afterSetupText.GetComponent<Text>().text = "AFTER SETUP: Discard this Exploration card.";
			afterSetup.SetActive (true);
		}
	}

	/*
	void MakeQuest (string questName)
	{
		currentQuest = questName;

		foreach(Sprite questImage in questImages){
			if (questImage.name == questName) explorationCard.transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite = questImage;
		}
		foreach(Sprite afterSetupImage in afterSetupImages){
			if (afterSetupImage.name == questName) explorationCard.transform.GetChild (2).GetComponent<SpriteRenderer> ().sprite = afterSetupImage;
		}
		if (questName == "killAll" + currentMonster + "s" && createdRooms == 1)
		{
			explorationCard.transform.GetChild (2).GetComponent<SpriteRenderer> ().sprite = null;
		}
		foreach(Sprite overlordTurn in specificOverlordTurn){
			if (overlordTurn.name == questName) explorationCard.transform.GetChild (3).GetComponent<SpriteRenderer> ().sprite = overlordTurn;
		}

		baseGameUniqueQuestsSpace.Remove (questName);
		baseGameUniqueQuestsMonster.Remove (questName);
		nullUniqueQuestsSpace.Remove (questName);
		nullUniqueQuestsMonster.Remove (questName);
	}*/
		
	void MakeQuestFromWWW ()
	{
		for (int i = 2; i < allData.Length; i++)
		{
			string[] oneQuest = allData[i].Split(new string[] { "</div>" }, System.StringSplitOptions.None)[1].Split(new string[] { "</td>" }, System.StringSplitOptions.None)[0].Split(new string[] { "<br />" }, System.StringSplitOptions.None);

			string questName = "";

			for (int j = 0; j < oneQuest.Length; j++)
			{
				if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim() == "name")
				{
					if(oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2) questName = oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim();
				}
			}

			string category = "";

			for (int j = 0; j < oneQuest.Length; j++)
			{
				if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim() == "category")
				{
					if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2) category = oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim ();
				}
			}

			for (int j = 0; j < oneQuest.Length; j++)
			{
				if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim() == "spaceRequirement")
				{
					if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2) {
						int spaceRequirement = 100;
						int.TryParse (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim (), out spaceRequirement);
						if (category == "standard") standardQuestsSpace.Add (questName, spaceRequirement);
						if (category == "baseGame") baseGameUniqueQuestsSpace.Add (questName, spaceRequirement);
						if (category == "null") nullUniqueQuestsSpace.Add (questName, spaceRequirement);
					}
				}
			}

			for (int j = 0; j < oneQuest.Length - 1; j++)
			{
				if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim() == "monsterRequirement")
				{
					if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2) {
						if (category == "standard") standardQuestsMonster.Add (questName, oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim());
						if (category == "baseGame") baseGameUniqueQuestsMonster.Add (questName, oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim());
						if (category == "null") nullUniqueQuestsMonster.Add (questName, oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim());
					}
				}
			}
		}
	}

	void MakeQuestFromWWW (string questName)
	{
		currentQuest = questName;

		for (int i = 2; i < allData.Length; i++)
		{
			string[] oneQuest = allData[i].Split(new string[] { "</div>" }, System.StringSplitOptions.None)[1].Split(new string[] { "</td>" }, System.StringSplitOptions.None)[0].Split(new string[] { "<br />" }, System.StringSplitOptions.None);

			for (int j = 0; j < oneQuest.Length; j++)
			{
				if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim() == "name")
				{
					if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2)
					{
						if (oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim () == questName)
						{
							nameText.GetComponent<Text> ().text = oneQuest [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim ();

							for (int k = 0; k < oneQuest.Length; k++)
							{
								if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim () == "quest")
								{
									if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2)
									{
										questText.GetComponent<Text> ().text = oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim ().Replace("/","\n");
									}
								}
								if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim () == "afterSetup")
								{
									if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2)
									{
										afterSetupText.GetComponent<Text> ().text = "AFTER SETUP: " + oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim ();
										afterSetup.SetActive (true);
									}
								}
								if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim () == "hourglassTime")
								{
									if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2)
									{
										hourglassTimeText.GetComponent<Text> ().text = oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim ();
										hourglass.SetActive (true);
									}
								}
								if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim () == "hourglassOverlordTurn")
								{
									if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2)
									{
										hourglassOverlordTurnText.GetComponent<Text> ().text = oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim ();
									}
								}
								if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim () == "overlordTurn")
								{
									if (oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2)
									{
										overlordTurnText.GetComponent<Text> ().text = oneQuest [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim ();
									}
								}
							}
						}
					}
				}
			}
		}

		baseGameUniqueQuestsSpace.Remove (questName);
		baseGameUniqueQuestsMonster.Remove (questName);
		nullUniqueQuestsSpace.Remove (questName);
		nullUniqueQuestsMonster.Remove (questName);
	}

	void MakeTokens ()
	{
		if (currentQuest == "villagerInStress") MakeToken (tokens [Random.Range(10,12)]);

		if (spawn1x1.Count >= 4)
		{
			for(int i = 0; i < 4; i++)
			{
				MakeToken (tokens [i]);
			}
		}
	}

	void MakeToken (GameObject token)
	{
		GameObject newSearchToken = Instantiate(token);
		newSearchToken.transform.parent = map.transform;
		int randomNumber = Random.Range(0,spawn1x1.Count);
		newSearchToken.transform.localPosition = spawn1x1[randomNumber];
		spawn1x1.Remove (spawn1x1 [randomNumber]);
	}

	void MakeTokensFromWWW ()
	{
		for (int i = 1; i < allData.Length; i++)
		{
			string[] questOne = allData[i].Split(new string[] { "<div >" }, System.StringSplitOptions.None);

			string[] questOneStats = questOne[0].Split(new string[] { "</div>" }, System.StringSplitOptions.None)[1].Split(new string[] { "<br />" }, System.StringSplitOptions.None);

			for (int j = 0; j < questOneStats.Length - 1; j++)
			{
				if (questOneStats [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim() == "name")
				{
					if (questOneStats [j].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2)
					{
						if (questOneStats [j].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim () == currentQuest)
						{
							for (int k = 0; k < questOneStats.Length - 1; k++)
							{
								if (questOneStats [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [0].Trim() == "token")
								{
									if (questOneStats [k].Split (new string[] { ":" }, System.StringSplitOptions.None).Length == 2)
									{
										foreach (GameObject token in tokens)
										{
											if (token.name == questOneStats [k].Split (new string[] { ":" }, System.StringSplitOptions.None) [1].Trim ()) MakeToken (token);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		if (spawn1x1.Count >= 4)
		{
			for(int i = 0; i < 4; i++)
			{
				MakeToken (tokens [i]);
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
