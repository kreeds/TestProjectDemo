using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class QuestEvent : MonoBehaviour {

	enum ParseMode
	{
		None,
		CharaSetup,
		PlayerSetup,
		DramaLocation,
		DramaFilename,

		Quest,
		QuestAction,

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

	[SerializeField]GameObject		dialogBase;
	[SerializeField]GameObject		bubbleGroup;
	[SerializeField]GameObject		actionGroup;
	[SerializeField]GameObject		playerTextGroup;	

//	[SerializeField]LAppModelProxy[] eventCharas;

	[SerializeField]SceneFadeInOut fader;

	[SerializeField]UIPanel			scenePanel;

	[SerializeField]LAppModelProxy	playerChara;

	[SerializeField]string[]		sceneFiles;
	[SerializeField]int				firstQuest;

	QuestProgress					questProgress;
	
	HUDService m_hudService;

	List<LAppModelProxy>	sceneCharas;

	int 				charaInSceneCount;

	Character[]			charactersInScene;
	DialogBase			openingDialogEvent;

	DialogBase			currentDialogEvent;

	Quest				currentQuest;

	bool 				displayingChoice;

	List<QuestChoiceOption>  choiceList;

	List<AreaNode>		nodeList;
	List<ActionEvent> 	actionList;

	RelationshipUp					relationShipPanel;

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
		InitializeScene ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitializeScene()
	{
		playerChara.PlayIdleAnim ();
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
		sceneCharas = new List<LAppModelProxy> ();
		foreach (Character chara in currentScene.characterList) {
			GameObject obj = GameObject.Instantiate(Resources.Load("Live2DAssets/live2DObject")) as GameObject;
			obj.transform.SetParent (scenePanel.transform);
			obj.transform.localScale = new Vector3 (30f, 30f, 30f);
			obj.transform.localPosition = new Vector3(chara.xpos, chara.ypos, -5);

			LAppModelProxy l2dModel = obj.GetComponent<LAppModelProxy>();
//			eventCharas[i].transform.localPosition = new Vector3(chara.xpos, chara.ypos, -7);
//
//			eventCharas[i].SetHair(chara.hairId);
//			eventCharas[i].SetClothes(chara.clothesId);
			l2dModel.SetHair(chara.hairId);
			l2dModel.SetClothes(chara.clothesId);

			sceneCharas.Add(l2dModel);

//			++i;
		}

		foreach (EventBase currentEvent in currentScene.eventList) {
			if (currentEvent.prereq != -1){
				continue;
			}
			
			GameObject obj = NGUITools.AddChild(bubbleGroup, Resources.Load("Prefabs/AreaNode") as GameObject);

			AreaNode an =  obj.GetComponent<AreaNode>();
			an.Init(currentEvent.id, false, true, "Bubble", currentEvent.loc);
			an.receiver = gameObject;
			an.callback = "OnBubbleClicked";

			Vector3 scale = obj.transform.localScale;
			scale.x *= currentEvent.direction;
			obj.transform.localScale = scale;

			nodeList.Add(an);
		}
		bubbleGroup.SetActive (true);

		backgroundTexture.mainTexture = Resources.Load ("Texture/bg0" + currentScene.bgID) as Texture;

		UIDraggablePanel dragPanel = scenePanel.GetComponent<UIDraggablePanel> ();
		if (dragPanel != null)
			dragPanel.enabled = true;

		playerTextGroup.SetActive (true);
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

		Drama currentDrama = null;
		QuestAction currentQuestAction = null;
		Quest currentQuest = null;

		foreach (string line in sceneData) {
			if (line.Contains("BG")){
				string[] parts = line.Split(':');
				currentScene.bgID = int.Parse(parts[1]);
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
				currentQuest = new Quest();
				currentQuest.actionList = new List<QuestAction>();
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
//				currentScene.dialogList.Add(currentDrama);
				currentScene.eventList.Add (currentDrama);
			}
			if (line.Contains("<QuestEnd>")){
				currentScene.eventList.Add (currentQuest);
			}
			if (line.Contains("<QuestEvent>")){
				parseMode = ParseMode.QuestAction;
				if (currentQuest != null && currentQuestAction != null){
					currentQuest.actionList.Add(currentQuestAction);
				}
				currentQuestAction = new QuestAction();

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
//					if (line.Contains("NextEvent")){
//						currentDrama.nextEvent = parts[1];
//					}
					if (line.Contains("side")){
						currentDrama.direction = int.Parse(parts[1]);
					}
					if (line.Contains("prereq:")){
						currentDrama.prereq = int.Parse (parts[1]);
					}
					if (line.Contains("id:")){
						currentDrama.id = int.Parse (parts[1]);
					}
					if (line.Contains ("showBond")){
						int bond = int.Parse (parts[1]);
						currentDrama.showBond = bond != 0;
					}
				}
				break;

			case ParseMode.Quest:
				if (line.Contains(":")){
					string[] parts = line.Split(':');
					if (line.Contains("x:")){
						currentQuest.loc.x = int.Parse(parts[1]);
					}
					if (line.Contains("y:")){
						currentQuest.loc.y = int.Parse(parts[1]);
					}
					if (line.Contains("side")){
//						currentQuest.direction = int.Parse(parts[1]);
					}
					if (line.Contains ("completion")){
						currentQuest.requiredAmount = int.Parse (parts[1]);
					}
					if (line.Contains("prereq:")){
						currentQuest.prereq = int.Parse (parts[1]);
					}
					if (line.Contains("id:")){
						currentQuest.id = int.Parse (parts[1]);
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
//					if (line.Contains("nextscene")){
//						currentQuest.nextEvent = parts[1];
//					}
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
				newDialogLine.dialogLine = parts[1];

				nextEvent = newDialogLine;
			}
				break;

			case ParseMode.Choice:
			{
				string[] parts = dataLine.Split(new char[] {'^'});

				DialogChoice newChoice = new DialogChoice();
				newChoice.eventGroup = currentBranch;
				newChoice.choiceOptions = new string[parts.Length];
				newChoice.choiceCost = new int[parts.Length];
				newChoice.choiceReward = new int[parts.Length];

				int i = 0;

				foreach (string choiceLine in parts){
					string[] lineParts = choiceLine.Split(new string[]{"::"}, System.StringSplitOptions.None);
					newChoice.choiceCost[i] = int.Parse(lineParts[0].Split (':')[1]);
					newChoice.choiceReward[i] = int.Parse(lineParts[2].Split (':')[1]);
					newChoice.choiceOptions[i++] = lineParts[1];
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

	void ShowCurrentDialog()
	{
		if (currentDialogEvent == null) {
//			GoToNext ();
			
			Drama currentDrama = currentScene.getCurrentEvent () as Drama;

			if (currentDrama.showBond){
				if (relationShipPanel == null){
					GameObject obj = Instantiate(Resources.Load("Prefabs/Event/RelationUpObject")) as GameObject;
					m_hudService.HUDControl.AttachMid(ref obj);
	//				GameObject obj = NGUITools.AddChild(scenePanel.gameObject, Resources.Load("Prefabs/Event/RelationUpObject") as GameObject);
					relationShipPanel = obj.GetComponent<RelationshipUp>();
					obj.transform.localPosition = new Vector3(0, 0, -12f);
					obj.transform.localScale = Vector3.one;
				}
				relationShipPanel.Initialize(gameObject, currentScene.characterList[0].hairId, currentScene.characterList[0].clothesId,
				                             6, 6+currentDrama.relationshipBonus, currentScene.characterList[0]._name);
				dialogBase.SetActive(false);
			}else{
				GoToNext();
			}
			return;
		}
		if (currentDialogEvent.eventType == DialogBase.EventType.Dialog) {
			DialogLine dialog = currentDialogEvent as DialogLine;

			if (dialog.characterID != 0){
				otherText.text = dialog.dialogLine;
				nameLabel.text = characterList[dialog.characterID]._name;
				playerText.text = "";
			}else{
				playerText.text = dialog.dialogLine;
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

			textCollider.enabled = false;

			playerTextGroup.SetActive (false);
		}
	}

	void OnNext()
	{
		if (displayingChoice == false) {
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

		if (choiceEvent.nextEvents.Count > 0){
			currentDialogEvent = choiceEvent.nextEvents [selected];
		}
		else {
			currentDialogEvent = choiceEvent.nextEvent;
		} 

		foreach (QuestChoiceOption option in choiceList) {
			Destroy(option.gameObject);
		}

		choiceList.Clear ();

//		ShowCurrentDialog ();
		textLabel.text = choiceEvent.choiceOptions [selected];
		nameLabel.text = characterList [0]._name;
		textCollider.enabled = true;

		playerTextGroup.SetActive (true);

		
		Drama currentDrama = currentScene.eventList [currentScene.currentEvent] as Drama;
		currentDrama.relationshipBonus += choiceEvent.choiceReward [selected];
	}

	void OnBubbleClicked(int selected){
		bubbleGroup.SetActive (false);

		currentScene.currentEvent = selected;
//		Drama selectedDrama = currentScene.dialogList [selected];
		EventBase currentEvent = currentScene.eventList [selected];

		if (currentEvent.eventType == SceneEventType.Drama) {
//			StartDrama (selectedDrama);
			StartDrama (currentEvent as Drama);
		} else if (currentEvent.eventType == SceneEventType.Quest) {
			currentQuest = currentEvent as Quest;
//			GameObject obj = NGUITools.AddChild (scenePanel.gameObject, Resources.Load ("Prefabs/Event/QuestStart") as GameObject);
			GameObject obj = Instantiate( Resources.Load ("Prefabs/Event/QuestStart")) as GameObject;
			m_hudService.HUDControl.AttachMid(ref obj);
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = new Vector3(0, 0, -5);
			QuestStart questWindow = obj.GetComponent<QuestStart>();
			questWindow.Initialize(gameObject, currentQuest.questName, currentQuest.questDesc);
//			StartQuest (currentEvent as Quest);
		}

	}
		//start quest or start drama

	void GoToNext()
	{
		foreach (EventBase eventListItem in currentScene.eventList){
			if (eventListItem.id == currentScene.currentEvent)
				continue;
			
			if (eventListItem.prereq == currentScene.currentEvent){
				currentScene.currentEvent = eventListItem.id;
				if (eventListItem.eventType == SceneEventType.Drama){
					StartDrama(eventListItem as Drama);
				}else{
					StartQuest (eventListItem as Quest);
				}
				return;
			}
		}
		
		ClearScene ();
//		LoadScene(sceneFiles[currentScene.nextScene]);
//		InitializeScene ();

		if (currentScene.nextScene > 0 && currentScene.nextScene < sceneFiles.Length) {
			nextSceneID = currentScene.nextScene;
			Service.Get<HUDService> ().ChangeScene ("EventScene");
		} else if (currentScene.nextSequence != null) {
			Service.Get<HUDService> ().ChangeScene (currentScene.nextSequence);
		}

//		fader.ChangeScene ("EventScene");
	}

	void BeginQuest()
	{
		StartQuest (currentQuest);
	}


	void StartQuest(Quest quest) {

		if (quest != null) {
			currentQuest = quest;
			if (questProgress == null){
				GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Event/QuestProgress")) as GameObject;
				m_hudService.HUDControl.AttachMid(ref obj);

				obj.transform.localPosition = new Vector3(0, -280f, 0);
				obj.transform.localScale = new Vector3(1, 1, 1);

				questProgress = obj.GetComponent<QuestProgress>();
			}
			questProgress.gameObject.SetActive (true);
			questProgress.SetProgress (0);
			for (int i = 0; i < currentQuest.actionList.Count; ++i) {
				QuestAction currentAction = currentQuest.actionList [i];
				GameObject actionObj = NGUITools.AddChild (actionGroup, Resources.Load ("Prefabs/Event/ActionEvent") as GameObject);

				Vector3 pos = new Vector3 (currentAction.loc.x, currentAction.loc.y, -7);
				actionObj.transform.localPosition = pos;

				ActionEvent actionEvent = actionObj.GetComponent<ActionEvent> ();
				actionEvent.Initialize (i, currentAction.staminaCost, gameObject, currentAction.desc);

				actionList.Add(actionEvent);
			}
		}
	}

	void StartDrama(Drama selectedDrama) {
		float dialogLoc = selectedDrama.loc.x;

		Vector3 pos = scenePanel.transform.localPosition;
		float diff = pos.x - dialogLoc;
		pos.x = -dialogLoc;
		scenePanel.transform.localPosition = pos;

		Vector4 clipPos = scenePanel.clipRange;
		clipPos.x = dialogLoc;
		scenePanel.clipRange = clipPos;

		LoadDialog (selectedDrama.dramaFile);

		pos = dialogBase.transform.localPosition;
		pos.x = dialogLoc;
		dialogBase.transform.localPosition = pos;
		dialogBase.SetActive (true);

		pos = playerChara.transform.localPosition;
		pos.x = currentScene.playerPos.x;

		playerChara.transform.localPosition = pos;

		currentDialogEvent = openingDialogEvent; //move it to after the event dialog is loaded
		ShowCurrentDialog ();
		displayingChoice = false;

		UIDraggablePanel dragPanel = scenePanel.GetComponent<UIDraggablePanel> ();
		if (dragPanel != null)
			dragPanel.enabled = false;

		if (questProgress != null) {
			questProgress.gameObject.SetActive (false);
		}
	}

	void OnAction(int actionID){
		QuestAction action = currentQuest.actionList [actionID];
		
		GameObject obj = NGUITools.AddChild (scenePanel.gameObject, Resources.Load ("Prefabs/Emitter") as GameObject);
		Emitter emitter = obj.GetComponent<Emitter> ();
		emitter.Init (new ItemType[]{ItemType.STAR}, new int[]{action.completionAmount}, 0, 3f, -1f, 1f);

	}

	void OnStarCollected(){

		currentQuest.completedAmount ++;
		float progressRatio = 0f;

		if (currentQuest.completedAmount >= currentQuest.requiredAmount) {
			progressRatio = 1f;
			
			GameObject obj = Instantiate( Resources.Load ("Prefabs/Event/QuestFinish")) as GameObject;
			m_hudService.HUDControl.AttachMid(ref obj);
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = new Vector3(0, 0, -5);

			QuestComplete questComplete = obj.GetComponent<QuestComplete>();
			questComplete.Initialize(gameObject, currentQuest.finishDesc);
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
		GoToNext ();
	}
}
