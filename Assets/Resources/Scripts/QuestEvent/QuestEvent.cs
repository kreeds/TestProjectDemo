﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class QuestEvent : MonoBehaviour {

	enum ParseMode
	{
		None,
		CharaSetup,
		PlayerSetup,
		DramaLocation,
		filename,

		Quest,
		QuestAction,

		Path,

		Character,
		Dialog,
		Choice,
		ChoiceOptions,
	}
	[SerializeField]UILabel			playerNameLabel;
	[SerializeField]UILabel			otherNameLabel;

	[SerializeField]UILabel			playerText;
	[SerializeField]UILabel			otherText;

	[SerializeField]UITexture		backgroundTexture;

	[SerializeField]TweenPosition	choiceTweenPos;
	[SerializeField]TweenAlpha		choiceTweenAlpha;
	
	[SerializeField]UIGrid			choiceRoot;

	[SerializeField]Collider		textCollider;

	[SerializeField]GameObject		dialogBase;
	[SerializeField]GameObject		bubbleGroup;
	[SerializeField]GameObject		actionGroup;

	[SerializeField]GameObject		academyArrow;
	[SerializeField]GameObject		normalArrow;

	[SerializeField]GameObject		playerTextGroup;
	[SerializeField]GameObject		otherTextGroup;

//	[SerializeField]LAppModelProxy[] eventCharas;

	[SerializeField]SceneFadeInOut fader;

	[SerializeField]UIPanel			scenePanel;

	[SerializeField]LAppModelProxy	playerChara;

	[SerializeField]string[]		sceneFiles;
	[SerializeField]GameObject[]	sceneFXContainer;
	[SerializeField]int				firstQuest;

	[SerializeField]TweenScale		otherTextTweenIn;
	[SerializeField]TweenScale		playerTextTweenIn;

	[SerializeField]TweenAlpha		otherTextTweenOut;
	[SerializeField]TweenAlpha		playerTextTweenOut;

	QuestProgress					questProgress;
	
	HUDService m_hudService;
	SoundService	m_soundService;
	AudioClip		m_bgm;

	List<LAppModelProxy>	sceneCharas;

	int 				charaInSceneCount;

	Character[]			charactersInScene;
	DialogBase			openingDialogEvent;

	DialogBase			currentDialogEvent;

	Quest				currentQuest;

	bool 				displayingChoice;

	bool				isDisplayingBattleStart;

	List<QuestChoiceOption>  choiceList;

	List<AreaNode>		nodeList;
	List<ActionEvent> 	actionList;

	RelationshipUp					relationShipPanel;
	QuestComplete questComplete;

	static List<Character> characterList;

	static Scene currentScene;

	static public int nextSceneID = -1;

//	static Quest currentQuest; //multiple sceneFiles in one scene later

	// Use this for initialization
	void Start () {
		Service.Init();	
		m_hudService = Service.Get<HUDService>();
		m_hudService.StartScene();


		m_hudService.ShowBottom (true);


		choiceList = new List<QuestChoiceOption> ();
		charaInSceneCount = 0;
		
		playerChara.PlayIdleAnim ();
//		LoadScene ("TextData/Quest2");
		if (nextSceneID == -1) {
			LoadScene (sceneFiles [firstQuest]);
			nextSceneID = firstQuest;
		}
		else
			LoadScene (sceneFiles [nextSceneID]);

		foreach (GameObject obj in sceneFXContainer)
			obj.SetActive (false);

		sceneFXContainer [nextSceneID].SetActive (true);

		switch (nextSceneID){
		case 0:
		{
			m_bgm = Resources.Load("Music/room") as AudioClip;
		}
			break;
		case 1:
			m_bgm = Resources.Load ("Music/outside") as AudioClip;
			break;
		case 2:
		{
			m_bgm = Resources.Load("Music/shopping") as AudioClip;
		}
			break;
			
		}
		m_soundService = Service.Get<SoundService>();
		m_soundService.PlayMusic(m_bgm, true);
		InitializeScene ();

		isDisplayingBattleStart = false;

		m_hudService.HUDControl.EnableRanking (true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitializeScene()
	{
		playerChara.LoadProfile ();
		playerChara.GetModel ().StopBasicMotion (true);
		playerChara.PlayIdleAnim ();

		Vector3 lscale = playerChara.transform.localScale;
		lscale.x = -lscale.x;
		playerChara.transform.localScale = lscale;

		dialogBase.SetActive (false);

		if (currentScene == null)
			return;

		if (actionList == null) {
			actionList = new List<ActionEvent> ();
		} else {
		}

		if (nodeList == null) {
			nodeList = new List<AreaNode> ();
		} else {

		}

//		int i = 0;
		List<int> reqCharas = new List<int> ();

		foreach (EventBase currentEvent in currentScene.eventList) {
			if (currentEvent.eventType != SceneEventType.Exit){
			if (PlayerProfile.Get().IsEventCleared (currentEvent.id) && currentEvent.isRepeat == false)
				continue;

			if (PlayerProfile.Get ().IsEventCleared(currentEvent.prereq) == false)
				continue;

			if (currentEvent.startChain == false && PlayerProfile.Get ().currentEventSet != currentEvent.chain)
				continue;
			}

			bool isBubble = !(currentEvent.eventType == SceneEventType.Exit);

			GameObject obj = NGUITools.AddChild(bubbleGroup, Resources.Load("Prefabs/AreaNode") as GameObject);

			AreaNode an =  obj.GetComponent<AreaNode>();

			int id = isBubble? currentEvent.id : ((Exit)currentEvent).nextScene;
			an.Init(id, false, isBubble, "Bubble", currentEvent.loc, currentEvent.charaid != -1, currentEvent.isMajorEvent);
			an.receiver = gameObject;
			an.callback = isBubble? "OnBubbleClicked" : "OnDoorClicked";

			Vector3 scale = obj.transform.localScale;
			scale.x *= currentEvent.direction;
			obj.transform.localScale = scale;

			nodeList.Add(an);

			if (!reqCharas.Contains (currentEvent.charaid))
				reqCharas.Add (currentEvent.charaid);
		}
		bubbleGroup.SetActive (true);

		backgroundTexture.mainTexture = Resources.Load ("Texture/bg0" + currentScene.bgID) as Texture;

		if (currentScene.bgID == 2) {
			backgroundTexture.mainTexture = Resources.Load ("Texture/shopping") as Texture;
		}

		Vector3 bgScale = backgroundTexture.transform.localScale;
		bgScale.x = currentScene.bgWidth;
		backgroundTexture.transform.localScale = bgScale;
		
		sceneCharas = new List<LAppModelProxy> ();
		foreach (Character chara in currentScene.characterList) {

			if (!reqCharas.Contains(chara._id))
				continue;

			GameObject obj = GameObject.Instantiate(Resources.Load("Live2DAssets/EventLive2DModel")) as GameObject;
			obj.transform.SetParent (scenePanel.transform);
			obj.transform.localScale = new Vector3 (45f*chara.side, 1, 45f);
			obj.transform.localPosition = new Vector3(chara.xpos, chara.ypos, -5);
			
			LAppModelProxy l2dModel = obj.GetComponent<LAppModelProxy>();
			//			eventCharas[i].transform.localPosition = new Vector3(chara.xpos, chara.ypos, -7);
			//
			//			eventCharas[i].SetHair(chara.hairId);
			//			eventCharas[i].SetClothes(chara.clothesId);
			l2dModel.SetCostume(chara.clothesId, chara.hairId);
			//			l2dModel.SetHair(chara.hairId);
			//			l2dModel.SetClothes(chara.clothesId);
			
			sceneCharas.Add(l2dModel);
			
			l2dModel.GetModel ().StopBasicMotion (true);
			if (chara.startAnimation == null){
				l2dModel.PlayIdleAnim ();
			}
			else
				l2dModel.PlayAnimation (chara.startAnimation);
			//			++i;
		}
		UIDraggablePanel dragPanel = scenePanel.GetComponent<UIDraggablePanel> ();
		if (dragPanel != null)
			dragPanel.enabled = true;


		playerTextGroup.SetActive (true);

		m_hudService.HUDControl.RemoveBattleHUD ();
	}

	void ClearScene()
	{
		foreach (AreaNode areaNode in nodeList){
			Destroy(areaNode.gameObject);
		}
		nodeList.Clear ();

		foreach(ActionEvent action in actionList){
			if (action != null)
			Destroy (action.gameObject);
		}
		actionList.Clear ();

		if (questProgress != null)
			Destroy (questProgress.gameObject);

		foreach (LAppModelProxy l2d in sceneCharas) {
			if (l2d != null)
			Destroy (l2d.gameObject);
		}
	}

	static public void LoadScene(string filename)
	{
		TextAsset asset = Resources.Load (filename) as TextAsset;
		if (asset != null) {
			LoadScene (asset.text.Split ('\n'));
		}
	}

	static void LoadScene(string[] sceneData)
	{
		currentScene = new Scene ();
		ParseMode parseMode = ParseMode.None;

		Character currentCharacter = null;

//		Quest currentQuest = null;

		EventBase currentEventBase = null;
//		Drama currentDrama = null;
		QuestAction currentQuestAction = null;
//		Quest currentQuest = null;

		foreach (string line in sceneData) {
			if (line.Contains("BG:")){
				string[] parts = line.Split(':');
				currentScene.bgID = int.Parse(parts[1]);
			}

			if (line.Contains ("BGWidth")){
				string[] parts = line.Split(':');
				currentScene.bgWidth = int.Parse(parts[1]);
			}

			if (line.Contains("NextScene:")){
				string[] parts = line.Split(':');
				currentScene.nextScene = int.Parse(parts[1]);
			}
			
			if (line.Contains("ChangeSequence:")){
				string[] parts = line.Split(':');
				currentScene.nextSequence = parts[1];
			}

			if (line.Contains("<CharaSetup>")){
				parseMode = ParseMode.CharaSetup;
				continue;
			}
			if (line.Contains("<Quest>")){
				parseMode = ParseMode.Quest;
				currentEventBase = new Quest();
//				currentQuest = new Quest();
//				currentQuest.actionList = new List<QuestAction>();
				continue;
			}
			if (line.Contains("<Exit>")){
				parseMode = ParseMode.Path;
				currentEventBase = new Exit();
				//				currentQuest = new Quest();
				//				currentQuest.actionList = new List<QuestAction>();
				continue;
			}

			if (line.Contains("<PlayerSetup>")){
				parseMode = ParseMode.PlayerSetup;
				continue;
			}
			if (line.Contains("<Drama>")){
				parseMode = ParseMode.DramaLocation;
//				currentDrama = new Drama();
				currentEventBase = new Drama();
				continue;
			}
			if (line.Contains("<DramaEnd>") || line.Contains("<QuestEnd>") || line.Contains("<ExitEnd>")){
//				currentScene.dialogList.Add(currentDrama);
				currentScene.eventList.Add (currentEventBase);
			}
//			if (line.Contains("<QuestEnd>")){
//				if (currentQuest != null && currentQuestAction != null){
//					currentQuest.actionList.Add(currentQuestAction);
//				}
//				currentScene.eventList.Add (currentQuest);
//			}
//			if (line.Contains("<QuestEvent>")){
//				parseMode = ParseMode.QuestAction;
//				if (currentQuest != null && currentQuestAction != null){
//					currentQuest.actionList.Add(currentQuestAction);
//				}
//				currentQuestAction = new QuestAction();
//
//			}
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
					if (line.Contains("x:")){
						currentCharacter.xpos = int.Parse(parts[1]);
					}
					if (line.Contains("y:")){
						currentCharacter.ypos = int.Parse(parts[1]);
					}
					if (line.Contains("hair:")){
						currentCharacter.hairId = int.Parse(parts[1]);
					}
					if (line.Contains("clothes:")){
						currentCharacter.clothesId = int.Parse(parts[1]);
					}
					if (line.Contains("name:")){
						currentCharacter._name = parts[1];
					}
					if (line.Contains ("anim:")){
						currentCharacter.startAnimation = parts[1];
					}
					if (line.Contains("side:")){
						currentCharacter.side = int.Parse(parts[1]);
					}
					if (line.Contains ("id:")){
						currentCharacter._id = int.Parse (parts[1]);
					}
				}
				break;

			case ParseMode.DramaLocation:
			case ParseMode.Quest:
			case ParseMode.Path:
				if (line.Contains(":")){
					string[] parts = line.Split(':');
					if (line.Contains("x:")){
						currentEventBase.loc.x = int.Parse(parts[1]); 
//						currentDrama.loc.x = int.Parse(parts[1]);
					}
					if (line.Contains("y:")){
						currentEventBase.loc.y = int.Parse(parts[1]);
//						currentDrama.loc.y = int.Parse(parts[1]);
					}
					if (line.Contains("filename")){
						currentEventBase.file = parts[1];
						if (parseMode == ParseMode.Quest){
							Quest quest = currentEventBase as Quest;
							LoadQuest(ref quest );
						}
//						currentDrama.file = parts[1];
					}
					if (line.Contains("next")){
						currentEventBase.nextEvent = int.Parse(parts[1]);
					}
					if (line.Contains("side")){
						currentEventBase.direction = int.Parse(parts[1]);
//						currentDrama.direction = int.Parse(parts[1]);
					}
					if (line.Contains("prereq:")){
						currentEventBase.prereq = int.Parse (parts[1]);
//						currentDrama.prereq = int.Parse (parts[1]);
					}
					if (line.Contains ("reqchara:")){
						currentEventBase.charaid = int.Parse (parts[1]);
					}
					if (line.Contains("id:")){
						currentEventBase.id = int.Parse(parts[1]);
//						currentDrama.id = int.Parse (parts[1]);
					}
					if (line.Contains ("bondup:")){
						int bond = int.Parse (parts[1]);
						Drama currentDrama = currentEventBase as Drama;
						currentDrama.showBond = (bond == 1);
					}
					if (line.Contains ("major:")){
						int isMajorEvent = int.Parse (parts[1]);
						currentEventBase.isMajorEvent = (isMajorEvent == 1);
					}
					if (line.Contains("AddNews:")){
//						Drama currentDrama = currentEventBase as Drama;
						currentEventBase.addnews = parts[1];
					}
					if (line.Contains("AddNewsIcon:")){
//						Drama currentDrama = currentEventBase as Drama;
						currentEventBase.addnewsIcon = parts[1];
					}
					if (line.Contains("AddNewsImage:")){
//						Drama currentDrama = currentEventBase as Drama;
						currentEventBase.addnewsImage = parts[1];
					}

					if (line.Contains ("gobattle")){
						currentEventBase.isBattle = true;
					}

					if (line.Contains ("monstername:")){
						currentEventBase.monstername = parts[1];
					}

					if (line.Contains ("monsterType:")){
						currentEventBase.enemyType = int.Parse (parts[1]);
					}

					if (line.Contains ("repeat:")){
						currentEventBase.isRepeat = (int.Parse (parts[1]) == 1);
					}

					if (line.Contains ("nextscene:") && parseMode == ParseMode.Path){
						Exit exit = currentEventBase as Exit;
						exit.nextScene = int.Parse (parts[1]);
					}
					if (line.Contains ("chainstart:")){
						currentEventBase.startChain = true;
						currentEventBase.chain = int.Parse(parts[1]);
					}
					else if (line.Contains ("chainend:")){
						currentEventBase.endChain = true;
					}
					else if (line.Contains ("chain:")){
						currentEventBase.chain = int.Parse (parts[1]);
					}

				}
				break;

//			case ParseMode.Quest:
//				if (line.Contains(":")){
//					string[] parts = line.Split(':');
//					if (line.Contains("x:")){
//						currentQuest.loc.x = int.Parse(parts[1]);
//					}
//					if (line.Contains("y:")){
//						currentQuest.loc.y = int.Parse(parts[1]);
//					}
//					if (line.Contains("side")){
//						currentQuest.direction = int.Parse(parts[1]);
//					}
//					if (line.Contains("prereq:")){
//						currentQuest.prereq = int.Parse (parts[1]);
//					}
//					if (line.Contains("id:")){
//						currentQuest.id = int.Parse (parts[1]);
//					}
//					if (line.Contains ("filename:")){
//						currentQuest.file = parts[1];
//						LoadQuest (ref currentQuest);
//					}
//					if (line.Contains("id:")){
//						currentQuest.id = int.Parse (parts[1]);
//					}
//					if (line.Contains ("completion")){
//						currentQuest.requiredAmount = int.Parse (parts[1]);
//					}
//					if (line.Contains("name")){
//						currentQuest.questName = parts[1];
//					}
//					if (line.Contains ("startdesc")){
//						int start = line.IndexOf('"') + 1;
//						string desc = line.Substring (start, line.LastIndexOf('"') - start).Replace("\\n", System.Environment.NewLine) ;
//						currentQuest.questDesc = desc;
//					}
//					if (line.Contains ("finishdesc")){
//						int start = line.IndexOf('"') + 1;
//						string desc = line.Substring (start, line.LastIndexOf('"') - start).Replace("\\n", System.Environment.NewLine) ;
//						currentQuest.finishDesc = desc;
//					}
//					if (line.Contains("nextscene")){
//						currentQuest.nextEvent = parts[1];
//					}
//				}
				break;

//			case ParseMode.QuestAction:
//				if (line.Contains(":")){
//					string[] parts = line.Split(':');
//					if (line.Contains("x:")){
//						currentQuestAction.loc.x = int.Parse(parts[1]);
//					}
//					if (line.Contains("y:")){
//						currentQuestAction.loc.y = int.Parse(parts[1]);
//					}
//					if (line.Contains("desc")){
//						currentQuestAction.desc = parts[1];
//					}
//					if (line.Contains ("completion")){
//						currentQuestAction.completionAmount = int.Parse (parts[1]);
//					}
//					if (line.Contains("cost")){
//						currentQuestAction.staminaCost = int.Parse (parts[1]);
//					}
//				}
//				
//				break;
			}
		}
	}

	
	static void LoadQuest(ref Quest currentQuest)
	{
		ParseMode parseMode = ParseMode.None;

		QuestAction currentQuestAction = null;
		TextAsset asset = Resources.Load (currentQuest.file) as TextAsset;
		string[] questData = asset.text.Split ('\n');

		parseMode = ParseMode.Quest;
		
		foreach (string line in questData) {
			if (line.Contains("<QuestEvent>")){
				parseMode = ParseMode.QuestAction;
				if (currentQuest != null && currentQuestAction != null){
					currentQuest.actionList.Add(currentQuestAction);
				}
				currentQuestAction = new QuestAction();
				
			}
			if (line.Contains("<End>")){
				if (currentQuest != null && currentQuestAction != null){
					currentQuest.actionList.Add(currentQuestAction);
				}
			}
			switch (parseMode){
				
			case ParseMode.Quest:
				if (line.Contains(":")){
					string[] parts = line.Split(':');
					if (line.Contains ("completion")){
						currentQuest.requiredAmount = int.Parse (parts[1]);
					}
					if (line.Contains("name")){
						currentQuest.questName = parts[1];
					}
					if (line.Contains ("startdesc")){
						int start = line.IndexOf('"') + 1;
						string desc = line.Substring (start, line.LastIndexOf('"') - start).Replace("\\n", System.Environment.NewLine) ;
						currentQuest.questDesc = desc;
					}
					if (line.Contains ("finishdesc")){
						int start = line.IndexOf('"') + 1;
						string desc = line.Substring (start, line.LastIndexOf('"') - start).Replace("\\n", System.Environment.NewLine) ;
						currentQuest.finishDesc = desc;
					}

					if (line.Contains ("questarea")){
						currentQuest.questArea = int.Parse (parts[1]);
					}

					if (line.Contains ("rewards:")){
						int start = line.IndexOf('"') + 1;
						string desc = line.Substring (start, line.LastIndexOf('"') - start).Replace("\\n", System.Environment.NewLine) ;
						currentQuest.rewards = desc;
					}
				}
				break;
				
			case ParseMode.QuestAction:
				if (line.Contains(":")){
					string[] parts = line.Split(':');
					if (line.Contains("x:")){
						currentQuestAction.loc.x = int.Parse(parts[1]);
					}
					if (line.Contains("y:")){
						currentQuestAction.loc.y = int.Parse(parts[1]);
					}
					if (line.Contains("desc")){
						currentQuestAction.desc = parts[1];
					}
					if (line.Contains ("completion")){
						currentQuestAction.completionAmount = int.Parse (parts[1]);
					}
					if (line.Contains("cost")){
						currentQuestAction.staminaCost = int.Parse (parts[1]);
					}
				}
				
				break;
			}
		}
	}

	void LoadDialog(string filename)
	{
		TextAsset asset = Resources.Load (filename) as TextAsset;
		if (asset != null) {
			LoadDialog (asset.text.Split ('\n'));
		}
	}

	void LoadDialog(string[] questDialog){
		
		ParseMode parseMode = ParseMode.None;

		DialogBase currentDialogEvent = null;
		characterList = new List<Character>();

		int currentBranch = 0;

		int currentChoiceIndex = 0;
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
			} else if (dataLine == "<ChoiceOptions>"){
				parseMode = ParseMode.ChoiceOptions;
				continue;
			}else if (dataLine == "<End>"){
				break;
			} else if (dataLine == "<ChoiceEnd>"){
				finishedQueue.Insert (0, currentBranch);
				priorityQueue.RemoveAt (0);
				currentBranch = priorityQueue[0];

				if (currentBranch == branchQueue[0])
					mergePending = true;

				continue;
			}
			
			DialogBase nextEvent = null;
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

				DialogLine newDialogLine = new DialogLine();
				newDialogLine.eventGroup = currentBranch;

				newDialogLine.characterID = int.Parse(parts[0]);
				newDialogLine.dialogLine = parts[1].Replace("<playername>", PlayerProfile.Get ().playerName).Replace("\\n", System.Environment.NewLine);
				newDialogLine.anims = null;

				if (parts.Length > 2){
					string[] anims = parts[2].Split (new char[]{','});
					newDialogLine.anims = new string[anims.Length];
					for (int i = 0; i < anims.Length; ++i){
						newDialogLine.anims[i] = anims[i];
					}

					if (parts.Length > 3){
						newDialogLine.isLoopAnim = (parts[3] == "L");
					}
				}

				nextEvent = newDialogLine;
			}
				break;

			case ParseMode.Choice:
			{
				string[] parts = dataLine.Split(new char[] {'^'});

				DialogChoice newChoice = new DialogChoice();

				int branchCount = int.Parse(parts[1]);
				newChoice.eventGroup = currentBranch;

				newChoice.choiceOptions = new string[branchCount];
				newChoice.choiceCost = new int[branchCount];
				newChoice.choiceReward = new int[branchCount];
				newChoice.choiceEnergyReward = new int[branchCount];
				newChoice.choiceMoneyReward = new int[branchCount];

				parseMode = ParseMode.ChoiceOptions;

				currentChoiceIndex = 0;

//				newChoice.eventGroup = currentBranch;
//				newChoice.choiceOptions = new string[parts.Length];
//				newChoice.choiceCost = new int[parts.Length];
//				newChoice.choiceReward = new int[parts.Length];
//
//				int i = 0;
//
//				foreach (string choiceLine in parts){
//					string[] lineParts = choiceLine.Split(new string[]{"::"}, System.StringSplitOptions.None);
//					newChoice.choiceCost[i] = int.Parse(lineParts[0].Split (':')[1]);
//					newChoice.choiceReward[i] = int.Parse(lineParts[2].Split (':')[1]);
//					newChoice.choiceOptions[i++] = lineParts[1];
//					maxBranch = maxBranch + i;
//					priorityQueue.Insert(0, maxBranch);
//				}
//
//				branchQueue.Insert(0, currentBranch); //store last branch node
//				currentBranch = priorityQueue[0]; //remove first node from pq
//
//				parseMode = ParseMode.Dialog;
//				branchQueue.Add (currentBranch);

				nextEvent = newChoice;
			}
				break;

			case ParseMode.ChoiceOptions:
			{
				DialogChoice currentChoice = currentDialogEvent as DialogChoice;
				
				string[] parts = dataLine.Split(new char[] {'^'});

				currentChoice.choiceCost[currentChoiceIndex] = int.Parse(parts[0]);
				currentChoice.choiceOptions[currentChoiceIndex] = parts[1];
				currentChoice.choiceReward[currentChoiceIndex] = int.Parse (parts[2]);

				currentChoice.choiceMoneyReward[currentChoiceIndex] = int.Parse (parts[3]);
				currentChoice.choiceEnergyReward[currentChoiceIndex] = int.Parse (parts[4]);

				currentChoiceIndex++;
				maxBranch = maxBranch + currentChoiceIndex;
				priorityQueue.Insert(0, maxBranch);

				if (currentChoiceIndex >= currentChoice.choiceCost.Length){

					branchQueue.Insert(0, currentBranch); //store last branch node
					currentBranch = priorityQueue[0]; //remove first node from pq

					parseMode = ParseMode.Dialog;
				}
				continue;

			}
				break;
			}

			if (currentDialogEvent == null){
					openingDialogEvent = nextEvent;
				}
			else{
				if (currentDialogEvent.eventGroup == nextEvent.eventGroup)
					currentDialogEvent.nextEvent = nextEvent;
				else{

					int lastBranch = branchQueue[0];
					if (lastBranch != priorityQueue[0]) //branch splitting
					{
						currentDialogEvent = openingDialogEvent.FindBranch(lastBranch);
//						while (currentDialogEvent.eventGroup != lastBranch && currentDialogEvent.eventType != DialogBase.EventType.Choice){
//							currentDialogEvent = currentDialogEvent.nextEvent;
//						}
						DialogChoice branchEvent = (DialogChoice)currentDialogEvent;
						branchEvent.nextEvents.Add (nextEvent);
					}else{ //branch merging
						
						foreach (int finishedIndex in finishedQueue){
							currentDialogEvent = openingDialogEvent.FindLeaf(finishedIndex);
							currentDialogEvent.nextEvent = nextEvent;
						}
						finishedQueue.Clear ();
						branchQueue.RemoveAt(0);
					}
				}

			}
			currentDialogEvent = nextEvent;
		}

		charactersInScene = characterList.ToArray ();

		currentDialogEvent = openingDialogEvent;
		currentDialogEvent.DumpContents ();
//		while (currentDialogEvent != null) {
//			DialogLine dialog = currentDialogEvent as DialogLine;
//			Debug.Log(dialog.dialogLine + "\n");
//
//			currentDialogEvent = currentDialogEvent.nextEvent;
//		}
	}

	void ShowDialogBoxes(bool showPlayerBox, bool showOtherBox)
	{
		if (showPlayerBox) {
			playerTextTweenIn.Play (true);
			playerTextTweenOut.alpha = 0.5f;
			playerTextTweenOut.Play(true);
		} else {
			playerTextTweenOut.from = 0;
			playerTextTweenOut.Play(false);
			playerTextTweenIn.Play(false);
		}

		if (showOtherBox) {
			otherTextTweenIn.Play (true);
			otherTextTweenOut.from = 0.5f;
			otherTextTweenOut.Play (true);
		} else {
			otherTextTweenOut.from = 0;
			otherTextTweenOut.Play (false);
			otherTextTweenIn.Play (false);
		}
	}

	void PlayEmoteFX(Vector3 pos, string[] emoteName)
	{
		foreach (string emote in emoteName) 
		{
			float lifetime = 0.2f;
			GameObject fxObject;
			string fxName = "";

			switch (emote) {
			case "EMOTE_HAPPY":
			case "EMOTE_HAPPY2":
			case "EMOTE_OK":
				fxName = "Prefabs/FX/Emoji_Happy2_Fx";
				break;
			case "EMOTE_WORRIED":
			case "EMOTE_SAD":
			case "EMOTE_CRY":
				fxName = "Prefabs/FX/Emoji_Gloomy_Fx";
				break;
			case "EMOTE_SHOCK":
				fxName = "Prefabs/FX/Emoji_Shock_Fx";
				break;

			}

			if (fxName != "") {
				fxObject = NGUITools.AddChild (scenePanel.gameObject, Resources.Load (fxName) as GameObject);
				Vector3 offset = new Vector3(0, 300f, 5f);
				fxObject.transform.localPosition = pos + offset;
				lifetime = fxObject.particleSystem.duration;
				Destroy (fxObject, lifetime);
			}
		}
	}

	void ShowCurrentDialog()
	{
		if (currentDialogEvent == null) {
//			GoToNext ();
			
//			if (otherTextGroup.transform.localScale.x >= 1)
//				otherTextTweenOut.Play(true);
//			if (playerTextGroup.transform.localScale.x >= 1)
//				playerTextTweenOut.Play (true);
			ShowDialogBoxes(false, false);

			Drama currentDrama = currentScene.getCurrentEvent () as Drama;

			if (currentDrama.showBond){
				GameObject obj = Instantiate(Resources.Load("Prefabs/Event/RelationUpObject")) as GameObject;
				m_hudService.HUDControl.AttachMid(ref obj);
				BondUp bondupObject = obj.GetComponent<BondUp>();
				bondupObject.Initialize (gameObject, currentScene.characterList[0]._name);
				obj.transform.localPosition = new Vector3(0, 0, -12f);
				obj.transform.localScale = Vector3.one;
//				if (relationShipPanel == null){
//					GameObject obj = Instantiate(Resources.Load("Prefabs/Event/RelationUpObject")) as GameObject;
//					m_hudService.HUDControl.AttachMid(ref obj);
//	//				GameObject obj = NGUITools.AddChild(scenePanel.gameObject, Resources.Load("Prefabs/Event/RelationUpObject") as GameObject);
//					relationShipPanel = obj.GetComponent<RelationshipUp>();
//					obj.transform.localPosition = new Vector3(0, 0, -12f);
//					obj.transform.localScale = Vector3.one;
//				}
//				relationShipPanel.Initialize(gameObject, currentScene.characterList[0].hairId, currentScene.characterList[0].clothesId,
//				                             6, 6+currentDrama.relationshipBonus, currentScene.characterList[0]._name);
				dialogBase.SetActive(false);
			}else if (currentDrama.addnews != null){
				ShowNewsNotification ();
			}
			else{
				GoToNext();
			}
			return;
		}
		if (currentDialogEvent.eventType == DialogBase.EventType.Dialog) {
			DialogLine dialog = currentDialogEvent as DialogLine;

			if (dialog.characterID > 0){
				otherText.text = dialog.dialogLine;
				otherNameLabel.text = characterList[dialog.characterID]._name;
				playerText.text = "";

				if (dialog.anims != null){
					sceneCharas[0].PlayAnimationSequence(dialog.anims, dialog.isLoopAnim);
					PlayEmoteFX(sceneCharas[0].transform.localPosition, dialog.anims);
				}

				ShowDialogBoxes(false, true);


//				if (playerTextGroup.transform.localScale.x >= 1)
//					playerTextTweenIn.Play(false);
//				if (otherTextGroup.transform.localScale.x < 1)
//					otherTextTweenIn.Play(true);
			}else{
				ShowDialogBoxes(true, false);
//				if (playerTextGroup.transform.localScale.x < 1)
//					playerTextTweenIn.Play(true);
//
//				if (otherTextGroup.transform.localScale.x >= 1)
//					otherTextTweenIn.Play(false);
				
				playerText.text = dialog.dialogLine;
				bool isAcademy = (dialog.characterID < 0);
				academyArrow.SetActive(isAcademy);
				normalArrow.SetActive(!isAcademy);
				
				if (dialog.anims != null){
					playerChara.PlayAnimationSequence (dialog.anims, dialog.isLoopAnim);
					PlayEmoteFX(playerChara.transform.localPosition, dialog.anims);
				}

				if (!isAcademy)
					playerNameLabel.text = PlayerProfile.Get ().playerName;
				else
					playerNameLabel.text = "Academy";
			}

			displayingChoice = false;
		} else if (currentDialogEvent.eventType == DialogBase.EventType.Choice) {
			DialogChoice choiceEvent = currentDialogEvent as DialogChoice;

			int i = 0;
			foreach (string str in choiceEvent.choiceOptions){
				GameObject obj = NGUITools.AddChild(choiceRoot.gameObject, Resources.Load("Prefabs/Event/QuestChoice") as GameObject);
				QuestChoiceOption choiceOption = obj.GetComponent<QuestChoiceOption>();
				choiceOption.Initialize(gameObject, i, choiceEvent.choiceCost[i], str);
				++i;
				choiceList.Add(choiceOption);
			}

			choiceRoot.repositionNow = true;

//			UITweener tween = choiceRoot.GetComponent<UITweener>();
//			tween.Play (true);
			choiceTweenPos.Play (true);
			choiceTweenAlpha.Play (true);

			textCollider.enabled = false;

//			playerTextGroup.SetActive (false);
		}
	}

	void OnNext()
	{
		if (displayingChoice == false && currentDialogEvent != null) {
			currentDialogEvent = currentDialogEvent.nextEvent;
			ShowCurrentDialog ();
		} else {
			ShowCurrentDialog ();
		}
	}

	void OnChoiceSelected(int selected){
		displayingChoice = true;

		DialogChoice choiceEvent = currentDialogEvent as DialogChoice;
		int cost = choiceEvent.choiceCost [selected];
		if (!PlayerProfile.Get ().IsActionAvailable(cost)) {
			return;
		}

		PlayerProfile.Get ().StartAction (cost);

		if (cost < 0) {

			GameObject obj = NGUITools.AddChild (scenePanel.gameObject, Resources.Load ("Prefabs/Emitter") as GameObject);

			Vector3 newPos = obj.transform.localPosition;
			newPos.x = -scenePanel.transform.localPosition.x;
			newPos.y -=  (170 + 90 * selected);
			obj.transform.localPosition = newPos;
		
			Emitter emitter = obj.GetComponent<Emitter> ();
			emitter.Init (new ItemType[]{ItemType.ENERGY}, new int[]{choiceEvent.choiceEnergyReward [selected]}, -20f, 20f, 60f, 60f, Emitter.EmitType.Flow);
			emitter.Init (new ItemType[]{ItemType.GOLD}, new int[]{choiceEvent.choiceMoneyReward [selected]}, -20f, 20f, 60f, 60f, Emitter.EmitType.Flow);
		}

		if (choiceEvent.nextEvents.Count > 0){
			currentDialogEvent = choiceEvent.nextEvents [selected];
		}
		else {
			currentDialogEvent = choiceEvent.nextEvent;
		} 

		foreach (QuestChoiceOption option in choiceList) {
			Destroy(option.gameObject);
		}
		
//		UITweener tween = choiceRoot.GetComponent<UITweener>();
//		tween.Play (false);
		choiceTweenAlpha.Play (false);
		choiceTweenPos.Play (false);

		choiceList.Clear ();

		//		ShowCurrentDialog ();
//		textLabel.text = choiceEvent.choiceOptions [selected];
		playerText.text = choiceEvent.choiceOptions [selected];
		playerNameLabel.text = PlayerProfile.Get ().playerName;
		textCollider.enabled = true;

//		playerTextTweenIn.Play (true);
//		otherTextTween.Play (false);
//		playerTextGroup.SetActive (true);
//		otherTextGroup.SetActive (false);

		ShowDialogBoxes (true, false);
		
//		Drama currentDrama = currentScene.eventList [currentScene.currentEvent] as Drama;
		Drama currentDrama = currentScene.getCurrentEvent () as Drama;
		currentDrama.relationshipBonus += choiceEvent.choiceReward [selected];
	}

	void OnDoorClicked(int selected){
		QuestEvent.nextSceneID = selected;
		Service.Get<HUDService> ().ChangeScene ("EventScene");
		ClearScene ();
	}

	void OnBubbleClicked(int selected){
		bubbleGroup.SetActive (false);

		currentScene.currentEvent = selected;
//		Drama selectedDrama = currentScene.dialogList [selected];
		EventBase currentEvent = null;

//		foreach (EventBase eventItem in currentScene.eventList){
//			if (eventItem.id != selected)
//				continue;
//
//			currentEvent = eventItem;
//			break;
//		}

		currentEvent = currentScene.getCurrentEvent ();

		if (currentEvent == null) {
			Debug.Log ("INVALID EVENT!");
			return;
		}


		if (currentEvent.eventType == SceneEventType.Drama) {
//			StartDrama (selectedDrama);
			if (currentEvent.isRepeat){
				foreach (AreaNode node in nodeList) {
					if (node.m_areaid == selected)
						node.SetToReadBubble ();
				}
			}

			StartDrama (currentEvent as Drama);
		} else if (currentEvent.eventType == SceneEventType.Quest) {
			currentQuest = currentEvent as Quest;
			CreateQuestDialog ();
		} else if (currentEvent.eventType == SceneEventType.Exit) {
			Exit exit = currentEvent as Exit;
			QuestEvent.nextSceneID = exit.nextScene;
			Service.Get<HUDService> ().ChangeScene ("EventScene");
		}

	}

	void CreateQuestDialog(){
		GameObject obj = Instantiate( Resources.Load ("Prefabs/Event/QuestStart")) as GameObject;
		m_hudService.HUDControl.AttachMid(ref obj);
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = new Vector3(0, 0, -5);
		QuestStart questWindow = obj.GetComponent<QuestStart>();
		questWindow.Initialize(gameObject, currentQuest.questName, currentQuest.questDesc, currentQuest.rewards);
	}

	void ShowNewsNotification(){
		
		GameObject obj = Instantiate( Resources.Load ("Prefabs/Event/NewsNotification")) as GameObject;
		m_hudService.HUDControl.AttachMid(ref obj);
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = new Vector3(0, 0, -5);

		NewsNotificationItem newsItem = obj.GetComponent<NewsNotificationItem> ();

		string headline = currentScene.getCurrentEvent ().addnews.Replace ("<playername>", PlayerProfile.Get ().playerName);
		string icon = currentScene.getCurrentEvent ().addnewsIcon;
		string newsImage = currentScene.getCurrentEvent ().addnewsImage;

		NewsDataItem item = new NewsDataItem (headline, icon, newsImage, false, "Emi was in the newspapersEmi was " +
			"in the newspapersEmi was in the newspapersEmi was in the newspapersEmi was " +
			"in the newspapersEmi was in the newspapersEmi was in the newspapers");

		newsItem.Initialize (gameObject, item._iconTextureName, item._headLine);

		if (currentScene.getCurrentEvent ().endChain == true)
			PlayerProfile.Get ().currentEventSet = -1;

		PlayerProfile.Get ().AddNews (item);

		m_soundService.PlaySound(Resources.Load("Sound/newsnotification03") as AudioClip, false);
	}

		//start quest or start drama
	void GoToNext()
	{
		EventBase current = currentScene.getCurrentEvent ();
		PlayerProfile.Get ().AddClearedEvent (current.id);

		//set enemy type for battle or link to next event
		if (current.isBattle) {
			if (isDisplayingBattleStart == false) {
				textCollider.enabled = false;
				
				GameObject obj = Instantiate (Resources.Load ("Prefabs/Event/BattleStart")) as GameObject;
				m_hudService.HUDControl.AttachMid (ref obj);
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = new Vector3 (0, 0, -5);
				
				m_soundService.StopMusic (m_bgm);
				BattleStart battleDialog = obj.GetComponent<BattleStart> ();

				BattleManager.nextBattleArea = nextSceneID;
				BattleManager.nextBattleMonster = current.enemyType;

				battleDialog.Initialize (gameObject, current.monstername);
				
				isDisplayingBattleStart = true;
			}
		} else if (current.nextEvent != -1) {
			
			foreach (EventBase eventListItem in currentScene.eventList) {
				if (eventListItem.id == current.nextEvent) {
					currentScene.currentEvent = eventListItem.id;
					if (eventListItem.eventType == SceneEventType.Drama) {
						StartDrama (eventListItem as Drama);
					} else {
						dialogBase.SetActive (false);
						currentQuest = eventListItem as Quest;
						CreateQuestDialog ();
					}
					return;
				}
			}
		} else if (!current.isRepeat) {
			ClearScene ();
			if (current.addnews != null)
				Service.Get<HUDService> ().ChangeScene ("FeedScene");
			else
				Service.Get<HUDService> ().ChangeScene ("EventScene");
		} else {
			bubbleGroup.SetActive (true);
			dialogBase.SetActive (false);
			EnableMapScroll (true);
			actionList.Clear ();
			m_hudService.HUDControl.EnableRanking (true);
		}

////		LoadScene(sceneFiles[currentScene.nextScene]);
////		InitializeScene ();
//
//		if (currentScene.nextScene > 0 && currentScene.nextScene < sceneFiles.Length) {
//			nextSceneID = currentScene.nextScene;
//			Service.Get<HUDService> ().ChangeScene ("EventScene");
//		} else if (currentScene.nextSequence != null) {
//			if (currentScene.nextSequence == "BattleScene"){
//
//				if (isDisplayingBattleStart == false){
//					textCollider.enabled = false;
//
//					GameObject obj = Instantiate( Resources.Load ("Prefabs/Event/BattleStart")) as GameObject;
//					m_hudService.HUDControl.AttachMid(ref obj);
//					obj.transform.localScale = Vector3.one;
//					obj.transform.localPosition = new Vector3(0, 0, -5);
//
//					m_soundService.StopMusic(m_bgm);
//					BattleStart battleDialog = obj.GetComponent<BattleStart>();
//					battleDialog.Initialize(gameObject, "Mirror Monster");
//
//					isDisplayingBattleStart = true;
//				}
//			}
//			else
//				Service.Get<HUDService> ().ChangeScene (currentScene.nextSequence);
//		}

//		fader.ChangeScene ("EventScene");
	}

	void OnBeginQuest()
	{
		StartQuest (currentQuest);
	}

	void OnDidNotBeginQuest()
	{
		bubbleGroup.SetActive (true);
		EnableMapScroll (true);
	}

	void StartQuest(Quest quest) {

		if (quest == null)
			return;

		m_hudService.HUDControl.EnableRanking (false);

		currentQuest = quest;
		currentQuest.completedAmount = 0;
//			LoadQuest (ref currentQuest);
		if (questProgress == null){
			GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Event/QuestProgress")) as GameObject;
			m_hudService.HUDControl.AttachTop(ref obj);

			//obj.transform.localPosition = new Vector3(0, -280f, 0);
			obj.transform.localScale = new Vector3(0.35f, 0.35f, 1);

			questProgress = obj.GetComponent<QuestProgress>();
			questProgress.Initialize(currentQuest.questName, "");
		}
		questProgress.gameObject.SetActive (true);
		questProgress.SetProgress (0);
		for (int i = 0; i < currentQuest.actionList.Count; ++i) {
			QuestAction currentAction = currentQuest.actionList [i];
			GameObject actionObj = NGUITools.AddChild (actionGroup, Resources.Load ("Prefabs/Event/ActionEvent") as GameObject);

			Vector3 pos = new Vector3 (currentAction.loc.x, currentAction.loc.y, -7);
			actionObj.transform.localPosition = pos;
			actionObj.transform.localScale = new Vector3(0.75f, 0.75f, 1);

			ActionEvent actionEvent = actionObj.GetComponent<ActionEvent> ();
			actionEvent.Initialize (i, currentAction.staminaCost, gameObject, currentAction.desc);

			actionList.Add(actionEvent);
		}

		if (currentQuest.startChain)
			PlayerProfile.Get ().currentEventSet = currentQuest.chain;

		EnableMapScroll (true);
	}

	void StartDrama(Drama selectedDrama) {
		
		m_hudService.HUDControl.EnableRanking (false);

		float dialogLoc = selectedDrama.loc.x - 50f;

		Vector3 pos = scenePanel.transform.localPosition;
//		float diff = pos.x - dialogLoc;
		pos.x = -dialogLoc;
		SpringPanel.Begin (scenePanel.gameObject, pos, 8f);
//		scenePanel.transform.localPosition = pos;
//
//		Vector4 clipPos = scenePanel.clipRange;
//		clipPos.x = dialogLoc;
//		scenePanel.clipRange = clipPos;

		LoadDialog (selectedDrama.file);

//		pos = dialogBase.transform.localPosition;
//		pos.x = dialogLoc;
//		dialogBase.transform.localPosition = pos;
		dialogBase.SetActive (true);

		pos = playerChara.transform.localPosition;
		pos.x = currentScene.playerPos.x;

		playerChara.transform.localPosition = pos;

		Vector3 scale = playerChara.transform.localScale;
		scale.x *= selectedDrama.direction;
		playerChara.transform.localScale = scale;

		currentDialogEvent = openingDialogEvent; //move it to after the event dialog is loaded
		ShowCurrentDialog ();
		displayingChoice = false;

		EnableMapScroll (false);

		if (questProgress != null) {
			questProgress.gameObject.SetActive (false);
		}

		if (selectedDrama.startChain)
			PlayerProfile.Get ().currentEventSet = selectedDrama.chain;
	}

	void OnAction(int actionID){

		m_soundService.PlaySound(Resources.Load("Sound/tapfinishobjective") as AudioClip, false);
		QuestAction action = currentQuest.actionList [actionID];
		
		GameObject obj = NGUITools.AddChild (scenePanel.gameObject, Resources.Load ("Prefabs/Emitter") as GameObject);
		Vector3 newPos = actionList [actionID].transform.localPosition;
		newPos.x = actionList [actionID].transform.localPosition.x;
		obj.transform.localPosition = newPos;

		Emitter emitter = obj.GetComponent<Emitter> ();
		emitter.Init (new ItemType[]{ItemType.STAR}, new int[]{action.completionAmount}, -20f, 20f, 60f, 60f, Emitter.EmitType.Flow);
	}

	void OnExpandAction(int actionID){

		for (int i = 0; i < actionList.Count; ++i) {
			if (i == actionID || actionList[i] == null)
				continue;

			actionList[i].Close ();
		}
		float newPosX = actionList [actionID].expandedCenter.x;
		Vector3 pos = scenePanel.transform.localPosition;
//
		pos.x = -newPosX;
		SpringPanel.Begin (scenePanel.gameObject, pos, 8f);
	}

	void OnBeginBattle()
	{
		Service.Get<HUDService> ().ChangeScene ("TransformScene");
//		GameObject obj = Instantiate( Resources.Load ("Prefabs/Event/TransformAnim")) as GameObject;
//		m_hudService.HUDControl.AttachMid(ref obj);
//		obj.transform.localScale = Vector3.one;
//		obj.transform.localPosition = new Vector3(0, 0, -5);
//
//		m_hudService.HUDControl.ShowBottom (false);
	}

	void OnStarCollected(){
		if (currentQuest.completedAmount > currentQuest.requiredAmount) {
			questProgress.SetProgress (1);
			return;
		}

		currentQuest.completedAmount ++;
		float progressRatio = 0f;

		if (currentQuest.completedAmount >= currentQuest.requiredAmount && questComplete == null) {
			progressRatio = 1f;

			m_soundService.PlaySound(Resources.Load("Sound/goalcomplete-1") as AudioClip, false);
			GameObject obj = Instantiate( Resources.Load ("Prefabs/Event/QuestFinish")) as GameObject;
			m_hudService.HUDControl.AttachMid(ref obj);
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = new Vector3(0, 0, -5);

			questComplete = obj.GetComponent<QuestComplete>();
			questComplete.Initialize(gameObject, currentQuest.finishDesc, currentQuest.rewards);
			
			questProgress.SetProgress (0);
			questProgress.gameObject.SetActive (false);
		}
		else
			progressRatio = currentQuest.completedAmount / (float)currentQuest.requiredAmount;

		questProgress.SetProgress (progressRatio);

//		Vector3 localScale = progressSprite.transform.localScale;
//		localScale.x = (232)*(5f*progressRatio);
//		progressSprite.transform.localScale = localScale;

	}

	void OnQuestCompleteOk()
	{
		if (currentScene.getCurrentEvent ().eventType == SceneEventType.Quest)
			Destroy (questComplete.gameObject);
		if (currentScene.getCurrentEvent ().addnews != null)
			ShowNewsNotification ();
		else
			GoToNext ();
	}

	void EnableMapScroll(bool enable){
		UIDraggablePanel dragPanel = scenePanel.GetComponent<UIDraggablePanel> ();
		if (dragPanel != null)
			dragPanel.enabled = enable;
		m_hudService.HUDControl.ShowScrollBar (enable);
	}
}
