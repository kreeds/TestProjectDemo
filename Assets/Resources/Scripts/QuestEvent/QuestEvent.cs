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
	public Character(int id = 0, string name = "")
	{
		_id = id;
		_name = name;
	}
};

public class QuestEvent : MonoBehaviour {

	enum ParseMode
	{
		None,
		Character,
		Dialog,
		Choice,
	}

	[SerializeField]UILabel			nameLabel;
	[SerializeField]UILabel			textLabel;

	[SerializeField]UIGrid		choiceRoot;

	ParseMode 			parseMode;

	int 				charaInSceneCount;

	Character[]			charactersInScene;
	EventBase			openingEvent;

	EventBase			currentEvent;

	List<QuestChoiceOption>  choiceList;
	List<Character> characterList;

	// Use this for initialization
	void Start () {
		choiceList = new List<QuestChoiceOption> ();
		parseMode = ParseMode.None;
		charaInSceneCount = 0;
		LoadEvent ("TextData/Dialog2");

		currentEvent = openingEvent;
		ShowCurrentDialog ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadEvent(string filename)
	{
		TextAsset asset = Resources.Load (filename) as TextAsset;
		if (asset != null) {
			LoadEvent (asset.text.Split ('\n'));
		}
	}

	void LoadEvent(string[] questData){

		EventBase currentEvent = null;
		characterList = new List<Character>();

		int currentBranch = 0;
		int maxBranch = currentBranch;

		List<int> branchQueue = new List<int> ();
		List<int> priorityQueue = new List<int> ();
		List<int> finishedQueue = new List<int> (); //for pending branches to be merged later

		bool mergePending = false;
		priorityQueue.Add (currentBranch);

		foreach (string dataLine in questData) {
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
			return;
		if (currentEvent.eventType == EventBase.EventType.Dialog) {
			EventDialog dialog = currentEvent as EventDialog;

			textLabel.text = dialog.dialogLine;
			nameLabel.text = characterList[dialog.characterID]._name;
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
		}
	}

	void OnNext()
	{
		Debug.Log ("Dialog pressed");
		currentEvent = currentEvent.nextEvent;
		ShowCurrentDialog ();
	}

	void OnChoiceSelected(int selected){
		EventChoice choiceEvent = currentEvent as EventChoice;

		if (choiceEvent.nextEvent != null) {
			currentEvent = choiceEvent.nextEvent;
		} else if (choiceEvent.nextEvents.Count > 0){
			currentEvent = choiceEvent.nextEvents [selected];
		}

		foreach (QuestChoiceOption option in choiceList) {
			Destroy(option.gameObject);
		}

		choiceList.Clear ();

		ShowCurrentDialog ();
	}
}
