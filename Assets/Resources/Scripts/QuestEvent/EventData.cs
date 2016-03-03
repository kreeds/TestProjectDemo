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
	public Character(int id = 0, string name = "")
	{
		_id = id;
		_name = name;
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

	public Scene()
	{
		eventList = new List<EventBase> ();
		characterList = new List<Character> ();
	}
}

public class Drama:EventBase
{
	public string dramaFile;
//	public Vector2 loc;
//	public Vector2 playerPos; //position of player at start of convo
//	public int direction;
//	
//	public string nextEvent; //maybe replace with a id that indicates the next event?

	public Drama(): base()
	{
		eventType = SceneEventType.Drama;
	}
}

public class QuestAction
{
	public Vector2 loc;
	public int staminaCost;
	public int completionAmount;
	public string desc;
}

public class Quest:EventBase
{
	public List<QuestAction> actionList;
	
	public int requiredAmount;
	public int completedAmount;

	public string questName;
	public string questDesc;

	public Quest(): base()
	{
		eventType = SceneEventType.Quest;
	}
//	public string nextEvent;
}

public enum SceneEventType
{
	Drama,
	Quest
}

public class EventBase
{
	public SceneEventType eventType;

	public int id;
	public Vector2 loc;
	public Vector2 playerPos; //position of player at start of convo
	public int direction;

	public int nextEventID;

	public int exp;

	public int prereq;

	public EventBase()
	{
		id = 0;
		loc = Vector2.zero;
		playerPos = loc;
		direction = 1;
		nextEventID = -1;
		prereq = -1;
	}
}