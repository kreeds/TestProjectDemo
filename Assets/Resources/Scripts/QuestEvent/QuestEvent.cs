using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventBase
{
	public enum EventType
	{
		Dialog,
		Choice,

		EventTypeCount
	}

	public int energyCost;
	public EventType eventType;
	public int eventGroup;
	public EventBase nextEvent;

	public virtual void DumpContents()
	{
	}

	public virtual EventBase FindLeaf(int groupIndex)
	{
		if (nextEvent != null)
			return nextEvent.FindLeaf (groupIndex);
		else if (eventGroup == groupIndex) {
			return this;
		}
		return null;
	}

	public virtual EventBase FindBranch(int groupIndex)
	{
		if (nextEvent != null)
			return nextEvent.FindBranch (groupIndex);

		return null;
	}
};

public class EventDialog : EventBase
{
	public int characterID;
	public string dialogLine;

	public override void DumpContents()
	{
		Debug.Log (characterID + ": " + dialogLine);
		if (nextEvent != null)
			nextEvent.DumpContents ();
	}

};

public class EventChoice : EventBase
{
	public string[] choiceOptions;
	public List<EventBase> nextEvents;

	public EventChoice()
	{
		nextEvents = new List<EventBase> ();
		eventType = EventType.Choice;
	}


	public override void DumpContents()
	{
		if (nextEvent != null)
			nextEvent.DumpContents ();
		else {
			int i = 0;
			foreach (EventBase childNodes in nextEvents){
				Debug.Log(i + ") " + choiceOptions[i++]);
				childNodes.DumpContents ();
			}
		}
	}

	public override EventBase FindLeaf(int groupIndex)
	{
		EventBase leafEvent = null;
		foreach (EventBase childNode in nextEvents) {
			EventBase temp = childNode.FindLeaf(groupIndex);
			if (temp != null){
				return temp;
			}
		}
		return null;
	}

	public override EventBase FindBranch(int groupIndex)
	{
		foreach (EventBase child in nextEvents) {
			EventBase leafBranch = child.FindBranch (groupIndex);
			if (leafBranch != null)
				return leafBranch;
		}
		if (eventGroup == groupIndex)
			return this;

		return null;
	}
};

public class Character
{
	public int _id;
	public string _name;
	public int _isL2d;
	public int xpos;
	public int ypos;
	public int hairId;
	public int clothesId;
	public Character(int id = 0, string name = "")
	{
		_id = id;
		_name = name;
	}
};

public class Scene
{
	public List<Character> characterList;
	public List<Drama> dialogList;

	public Vector2 playerPos; //position of player at start of scene

	public int bgID;
}

public class Drama
{
	public string dramaFile;
	public Vector2 loc;
	public Vector2 playerPos; //position of player at start of convo
	public int direction;
}

public class QuestEvent : MonoBehaviour {

	enum ParseMode
	{
		None,
		CharaSetup,
		PlayerSetup,
		DramaLocation,
		DramaFilename,

		Character,
		Dialog,
		Choice,
	}
	[SerializeField]UILabel			nameLabel;
	[SerializeField]UILabel			textLabel;

	[SerializeField]UILabel			playerText;
	[SerializeField]UILabel			otherText;

	[SerializeField]UITexture		backgroundTexture;

	[SerializeField]UIGrid			choiceRoot;

	[SerializeField]Collider		textCollider;

	[SerializeField]GameObject		eventBase;
	[SerializeField]GameObject		bubbleGroup;

	[SerializeField]LAppModelProxy[] eventCharas;

	[SerializeField]SceneFadeInOut fader;

	[SerializeField]UIPanel			scenePanel;

	[SerializeField]LAppModelProxy	playerChara;
	
	HUDService m_hudService;


	int 				charaInSceneCount;

	Character[]			charactersInScene;
	EventBase			openingEvent;

	EventBase			currentEvent;

	bool 				displayingChoice;

	List<QuestChoiceOption>  choiceList;
	static List<Character> characterList;

	static Scene currentScene;

	// Use this for initialization
	void Start () {
//		Service.Init();	
//		m_hudService = Service.Get<HUDService>();
//		m_hudService.StartScene();

		choiceList = new List<QuestChoiceOption> ();
		charaInSceneCount = 0;

		LoadScene ();
		InitializeScene ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitializeScene()
	{
		eventBase.SetActive (false);

		if (currentScene == null)
			return;

		int i = 0;
		foreach (Character chara in currentScene.characterList) {
			eventCharas[i].transform.localPosition = new Vector3(chara.xpos, chara.ypos, -7);

			eventCharas[i].SetHair(chara.hairId);
			eventCharas[i].SetClothes(chara.clothesId);

			++i;
		}

		foreach (Drama drama in currentScene.dialogList) {
			
			GameObject obj = NGUITools.AddChild(bubbleGroup, Resources.Load("Prefabs/AreaNode") as GameObject);

			AreaNode an =  obj.GetComponent<AreaNode>();
			an.Init(0, false, true, "Bubble", drama.loc);
			an.receiver = gameObject;
			an.callback = "OnBubbleClicked";

			Vector3 scale = obj.transform.localScale;
			scale.x *= drama.direction;
			obj.transform.localScale = scale;
		}

		backgroundTexture.mainTexture = Resources.Load ("Texture/bg0" + currentScene.bgID) as Texture;
	}

	static public void LoadScene()
	{
		TextAsset asset = Resources.Load ("TextData/Quest1") as TextAsset;
		if (asset != null) {
			LoadQuest (asset.text.Split ('\n'));
		}
	}

	static void LoadQuest(string[] questData)
	{
		currentScene = new Scene ();
		currentScene.characterList = new List<Character> ();
		currentScene.dialogList = new List<Drama> ();
		ParseMode parseMode = ParseMode.None;

		Character currentCharacter = null;

		Drama currentDrama = null;

		foreach (string line in questData) {
			if (line.Contains("BG")){
				string[] parts = line.Split(':');
				currentScene.bgID = int.Parse(parts[1]);
			}
			if (line.Contains("<CharaSetup>")){
				parseMode = ParseMode.CharaSetup;
				continue;
			}

			if (line.Contains("<PlayerSetup>")){
				parseMode = ParseMode.PlayerSetup;
				continue;
			}
			if (line.Contains("<Drama>")){
				parseMode = ParseMode.DramaLocation;
				currentDrama = new Drama();
				continue;
			}
			if (line.Contains("<DramaEnd>")){
				currentScene.dialogList.Add(currentDrama);
			}
			switch (parseMode){
			case ParseMode.PlayerSetup:
				if (line.Contains(":")){
					string[] parts = line.Split(':');
					if (line.Contains("x")){
						currentScene.playerPos.x = int.Parse(parts[1]);
					}
					if (line.Contains("y")){
						currentScene.playerPos.y = int.Parse(parts[1]);
					}
				}
				break;

			case ParseMode.CharaSetup:
				if (line.Contains("<Chara>")){
					currentCharacter = new Character(currentScene.characterList.Count);
				}
				if (line.Contains("<CharaEnd>")){
					currentScene.characterList.Add(currentCharacter);
				}
				if (line.Contains(":")){
					string[] parts = line.Split(':');
					if (line.Contains("x")){
						currentCharacter.xpos = int.Parse(parts[1]);
					}
					if (line.Contains("y")){
						currentCharacter.ypos = int.Parse(parts[1]);
					}
					if (line.Contains("hair")){
						currentCharacter.hairId = int.Parse(parts[1]);
					}
					if (line.Contains("clothes")){
						currentCharacter.clothesId = int.Parse(parts[1]);
					}
					if (line.Contains("name")){
						currentCharacter._name = parts[1];
					}
				}
				break;

			case ParseMode.DramaLocation:
				if (line.Contains(":")){
					string[] parts = line.Split(':');
					if (line.Contains("x:")){
						currentDrama.loc.x = int.Parse(parts[1]);
					}
					if (line.Contains("y:")){
						currentDrama.loc.y = int.Parse(parts[1]);
					}
					if (line.Contains("DramaFilename")){
						currentDrama.dramaFile = parts[1];
					}
					if (line.Contains("side")){
						currentDrama.direction = int.Parse(parts[1]);
					}
				}
				break;
			}
		}
	}

	void LoadEvent(string filename)
	{
		TextAsset asset = Resources.Load (filename) as TextAsset;
		if (asset != null) {
			LoadEvent (asset.text.Split ('\n'));
		}
	}

	void LoadEvent(string[] questDialog){
		
		ParseMode parseMode = ParseMode.None;

		EventBase currentEvent = null;
		characterList = new List<Character>();

		int currentBranch = 0;
		int maxBranch = currentBranch;

		List<int> branchQueue = new List<int> ();
		List<int> priorityQueue = new List<int> ();
		List<int> finishedQueue = new List<int> (); //for pending branches to be merged later

		bool mergePending = false;
		priorityQueue.Add (currentBranch);

		foreach (string dataLine in questDialog) {
			if (dataLine == "<Chara>"){
				parseMode = ParseMode.Character;
				continue;
			} else if (dataLine == "<Dialog>"){
				parseMode = ParseMode.Dialog;
				continue;
			} else if (dataLine == "<Choice>"){
				parseMode = ParseMode.Choice;
				continue;
			} else if (dataLine == "<End>"){
				break;
			} else if (dataLine == "<ChoiceEnd>"){
				finishedQueue.Insert (0, currentBranch);
				priorityQueue.RemoveAt (0);
				currentBranch = priorityQueue[0];

				if (currentBranch == branchQueue[0])
					mergePending = true;

				continue;
			}
			
			EventBase nextEvent = null;
			switch (parseMode){
			case ParseMode.Character:
			{
				string[] parts = dataLine.Split(new char[] {',',':'});
				characterList.Add(new Character(int.Parse(parts[0]), parts[1]));
			}
				break;

			case ParseMode.Dialog:
			{
				string[] parts = dataLine.Split(new char[] {'^'});

				EventDialog newDialogLine = new EventDialog();
				newDialogLine.eventGroup = currentBranch;

				newDialogLine.characterID = int.Parse(parts[0]);
				newDialogLine.dialogLine = parts[1];

				nextEvent = newDialogLine;
			}
				break;

			case ParseMode.Choice:
			{
				string[] parts = dataLine.Split(new char[] {'^'});
				EventChoice newChoice = new EventChoice();
				newChoice.eventGroup = currentBranch;
				newChoice.choiceOptions = new string[parts.Length];

				int i = 0;

				foreach (string choiceLine in parts){
					newChoice.choiceOptions[i++] = choiceLine;
					maxBranch = maxBranch + i;
					priorityQueue.Insert(0, maxBranch);
				}

				branchQueue.Insert(0, currentBranch); //store last branch node
				currentBranch = priorityQueue[0]; //remove first node from pq

				parseMode = ParseMode.Dialog;
//				branchQueue.Add (currentBranch);

				nextEvent = newChoice;
			}
				break;
			}

			if (currentEvent == null){
					openingEvent = nextEvent;
				}
			else{
				if (currentEvent.eventGroup == nextEvent.eventGroup)
					currentEvent.nextEvent = nextEvent;
				else{

					int lastBranch = branchQueue[0];
					if (lastBranch != priorityQueue[0]) //branch splitting
					{
						currentEvent = openingEvent.FindBranch(lastBranch);
//						while (currentEvent.eventGroup != lastBranch && currentEvent.eventType != EventBase.EventType.Choice){
//							currentEvent = currentEvent.nextEvent;
//						}
						EventChoice branchEvent = (EventChoice)currentEvent;
						branchEvent.nextEvents.Add (nextEvent);
					}else{ //branch merging
						
						foreach (int finishedIndex in finishedQueue){
							currentEvent = openingEvent.FindLeaf(finishedIndex);
							currentEvent.nextEvent = nextEvent;
						}
						finishedQueue.Clear ();
						branchQueue.RemoveAt(0);
					}
				}

			}
			currentEvent = nextEvent;
		}

		charactersInScene = characterList.ToArray ();

		currentEvent = openingEvent;
		currentEvent.DumpContents ();
//		while (currentEvent != null) {
//			EventDialog dialog = currentEvent as EventDialog;
//			Debug.Log(dialog.dialogLine + "\n");
//
//			currentEvent = currentEvent.nextEvent;
//		}
	}

	void ShowCurrentDialog()
	{
		if (currentEvent == null)
			return; //go to next scene here
		if (currentEvent.eventType == EventBase.EventType.Dialog) {
			EventDialog dialog = currentEvent as EventDialog;

			if (dialog.characterID != 0){
				otherText.text = dialog.dialogLine;
				nameLabel.text = characterList[dialog.characterID]._name;
				playerText.text = "";
			}else{
				playerText.text = dialog.dialogLine;
			}

			displayingChoice = false;
		} else if (currentEvent.eventType == EventBase.EventType.Choice) {
			EventChoice choiceEvent = currentEvent as EventChoice;

			int i = 0;
			foreach (string str in choiceEvent.choiceOptions){
				GameObject obj = NGUITools.AddChild(choiceRoot.gameObject, Resources.Load("Prefabs/QuestChoice") as GameObject);
				QuestChoiceOption choiceOption = obj.GetComponent<QuestChoiceOption>();
				choiceOption.Initialize(gameObject, i++, str);

				choiceList.Add(choiceOption);
			}

			choiceRoot.repositionNow = true;

			textCollider.enabled = false;
		}
	}

	void OnNext()
	{
		if (displayingChoice == false) {
			currentEvent = currentEvent.nextEvent;
			ShowCurrentDialog ();
		} else {
			ShowCurrentDialog ();
		}
	}

	void OnChoiceSelected(int selected){
		displayingChoice = true;

		EventChoice choiceEvent = currentEvent as EventChoice;
		if (choiceEvent.nextEvents.Count > 0){
			currentEvent = choiceEvent.nextEvents [selected];
		}
		else {
			currentEvent = choiceEvent.nextEvent;
		} 

		foreach (QuestChoiceOption option in choiceList) {
			Destroy(option.gameObject);
		}

		choiceList.Clear ();

//		ShowCurrentDialog ();
		textLabel.text = choiceEvent.choiceOptions [selected];
		nameLabel.text = characterList [0]._name;
		textCollider.enabled = true;
	}

	void OnBubbleClicked(int selected){
		bubbleGroup.SetActive (false);

		Drama selectedDrama = currentScene.dialogList [selected];

		float dialogLoc = selectedDrama.loc.x;

		Vector3 pos = scenePanel.transform.localPosition;
		float diff = pos.x - dialogLoc;
		pos.x = -dialogLoc;
		scenePanel.transform.localPosition = pos;

		Vector4 clipPos = scenePanel.clipRange;
		clipPos.x = dialogLoc;
		scenePanel.clipRange = clipPos;

		LoadEvent (selectedDrama.dramaFile);

		pos = eventBase.transform.localPosition;
		pos.x = dialogLoc;
		eventBase.transform.localPosition = pos;
		eventBase.SetActive (true);

		pos = playerChara.transform.localPosition;
		pos.x = currentScene.playerPos.x;

		playerChara.transform.localPosition = pos;

		currentEvent = openingEvent; //move it to after the event dialog is loaded
		ShowCurrentDialog ();
		displayingChoice = false;


	}
}
