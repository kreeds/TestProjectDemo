using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogBase
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
	public DialogBase nextEvent;
	
	public virtual void DumpContents()
	{
	}
	
	public virtual DialogBase FindLeaf(int groupIndex)
	{
		if (nextEvent != null)
			return nextEvent.FindLeaf (groupIndex);
		else if (eventGroup == groupIndex) {
			return this;
		}
		return null;
	}
	
	public virtual DialogBase FindBranch(int groupIndex)
	{
		if (nextEvent != null)
			return nextEvent.FindBranch (groupIndex);
		
		return null;
	}
};

public class DialogLine : DialogBase
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

public class DialogChoice : DialogBase
{
	public string[] choiceOptions;
	public int[] choiceCost;
	public int[] choiceReward;
	public int[] choiceMoneyReward;
	public int[] choiceEnergyReward;
	public List<DialogBase> nextEvents;
	
	public DialogChoice()
	{
		nextEvents = new List<DialogBase> ();
		eventType = EventType.Choice;
	}
	
	
	public override void DumpContents()
	{
		if (nextEvent != null)
			nextEvent.DumpContents ();
		else {
			int i = 0;
			foreach (DialogBase childNodes in nextEvents){
				Debug.Log(i + ") " + choiceOptions[i++]);
				childNodes.DumpContents ();
			}
		}
	}
	
	public override DialogBase FindLeaf(int groupIndex)
	{
		DialogBase leafEvent = null;
		foreach (DialogBase childNode in nextEvents) {
			DialogBase temp = childNode.FindLeaf(groupIndex);
			if (temp != null){
				return temp;
			}
		}
		return null;
	}
	
	public override DialogBase FindBranch(int groupIndex)
	{
		foreach (DialogBase child in nextEvents) {
			DialogBase leafBranch = child.FindBranch (groupIndex);
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
	public int side;
	public Character(int id = 0, string name = "")
	{
		_id = id;
		_name = name;
		side = 1;
	}
};

public class Scene
{
	public List<Character> characterList;

	public List<EventBase> eventList;
	
	public Vector2 playerPos; //position of player at start of scene
	
	public int bgID;

	public int nextScene;

	public int currentEvent;

	public string nextSequence;

	public EventBase getCurrentEvent()
	{
		foreach (EventBase eventItem in eventList) {
			if (eventItem.id == currentEvent)
				return eventItem;
		}
		return null;
	}

	public Scene()
	{
		eventList = new List<EventBase> ();
		characterList = new List<Character> ();
		nextScene = -1;
		nextSequence = null;
	}
}

public class Drama:EventBase
{
//	public Vector2 loc;
//	public Vector2 playerPos; //position of player at start of convo
//	public int direction;
//	
//	public string nextEvent; //maybe replace with a id that indicates the next event?
	public int relationshipBonus; //needs to be associated with a character next time

	public Drama(): base()
	{
		relationshipBonus = 0;
		eventType = SceneEventType.Drama;
		showBond = false;
	}
}

public class QuestAction
{
	public Vector2 loc;
	public int staminaCost;
	public int completionAmount;
	public string desc;
}

public enum SceneEventType
{
	Drama,
	Quest,
	Exit
}

public class Exit:EventBase
{
	public int nextScene;
	public Exit(): base()
	{
		isRepeat = true;
		eventType = SceneEventType.Exit;
	}
}

public class Quest:EventBase
{
	public List<QuestAction> actionList;
	
	public int requiredAmount;
	public int completedAmount;

	public int questArea;

	public string questName;
	public string questDesc;

	public string finishDesc;


	public Quest(): base()
	{
		eventType = SceneEventType.Quest;
		actionList = new List<QuestAction> ();
	}
//	public string nextEvent;
}

public class EventBase
{
	public bool showBond;
	public bool startChain;
	public bool endChain;
	public SceneEventType eventType;
	public string file;

	public int id;

	public int charaid;
	public Vector2 loc;
	public Vector2 playerPos; //position of player at start of convo
	public int direction;

	public int nextEvent;

	public int exp;

	public int prereq;
	public bool isBattle;
	public bool isRepeat;
	public int enemyType;
	public string addnews;
	public string addnewsIcon;
	public string addnewsImage;
	public string monstername;

	public int chain;

	public EventBase()
	{
		id = 0;
		loc = Vector2.zero;
		playerPos = loc;
		direction = 1;
		nextEvent = -1;
		prereq = -1;
		charaid = -1;
		chain = -1;

		isBattle = false;

		isRepeat = false;

		addnews = null;

		startChain = false;

		monstername = "A Monster";

		enemyType = 0;
	}
}