using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PDollarGestureRecognizer;
using System;
using System.Text;

public class InputManager : MonoBehaviour {

	public delegate void GestureDelegates(string param1, string param2);
	public delegate void StringGestureDelegates(string param1);

	public static GestureDelegates Save;
	public static StringGestureDelegates Load;

	public static InputManager _instance;

    public DataGestures mgData;
    public GenerateGesture	mgGesture;
    public Transform parent;

    BattleManager m_battleMgr;
    PlayerManager m_playerMgr;
 
 	public float timetoEnd = 3.0f;

 	float currtime;

 	GameObject particleObj;

    bool IsTouchInProgress, IsMovementInTouch, WasMovementInTouch;
    Vector2 positionTouchStart;
    bool IsTrailQueuedForDestroy;
    bool IsMouseMovementStarted;
    bool IsGestureRecognizingNeeded;
    bool IsStartTrailSpawned;

    List<GameObject> trailList;
    GameObject guide;

    int strokeID = 0;
    int strokeCount = 0;


    List<Point> 		currentGesturePoints;
    List<Point>			pointsContainer;


    bool disableGesture = false;
    public bool DisableGesture
    {
    	get{return disableGesture;}
    	set{
    		disableGesture = value;
    	}
    }

    GameObject trail;

	public static InputManager Get()
	{
		return _instance;
	}

	void Awake()
	{
		if(_instance == null)
			_instance = this;
	}
	void Start () {

		currtime = 0.0f;
		trailList = new List<GameObject>();
		currentGesturePoints = new List<Point>();
        IsTouchInProgress = IsTrailQueuedForDestroy = IsMouseMovementStarted = IsStartTrailSpawned = false;

		Save = GestureSave;
		Load = GestureLoad;

		m_battleMgr = BattleManager.Get();
		m_playerMgr = PlayerManager.Get();
	}

    void StartGesture(Vector3 pos)
    {
		positionTouchStart = pos;
        IsTouchInProgress = true;
        IsMovementInTouch = WasMovementInTouch = false;
        IsTrailQueuedForDestroy = false;
        IsGestureRecognizingNeeded = true;
		strokeID++;

		Service.Get<HUDService>().HUDControl.ShowTip(false);

		currentGesturePoints.Add(new Point(pos.x, -pos.y, strokeID));
    }

    void MoveGesture(Vector3 pos)
    {
        IsMovementInTouch = WasMovementInTouch = true;
		currentGesturePoints.Add(new Point(pos.x, -pos.y, strokeID));
    }

    void FinishGesture(Vector3 pos)
    {
        if (IsTouchInProgress)
        {
            IsTouchInProgress = false;
            if (pos != Vector3.zero)
				currentGesturePoints.Add(new Point(pos.x, -pos.y, strokeID));

        }
    }

    // Delegate Functions declarations
    void GestureSave(string filename, string gesturename)
    {
		DataGestures.Save(pointsContainer, strokeCount, filename, gesturename);
    }

    void GestureLoad(string path)
    {
    	Debug.Log("Path: " + path); 
    }

    /// <summary>
    /// Special Attack Phase Check
    /// </summary>
    void SpecialAttackPhaseCheck()
    {
	 	if(m_battleMgr.CurBattlePhase != BattleManager.BattlePhase.SPECIAL)
	 		return;
	 		
		if (!IsTouchInProgress)//end
        {
            if ((trail != null)&&(!IsTrailQueuedForDestroy))
            {	
                IsTrailQueuedForDestroy = true;

                // Clear the list
       			foreach(GameObject obj in trailList)
       			{
       				Destroy(obj);
       			}

                trailList.Clear();

#if UNITY_IPHONE && !UNITY_EDITOR
				if (pointsContainer != null)
					pointsContainer.Clear ();
				
				pointsContainer = new List<Point>();
				pointsContainer.AddRange(currentGesturePoints.ToArray ());
#else
				pointsContainer = GenericCopier<List<Point>>.DeepCopy(currentGesturePoints);
#endif

                IsStartTrailSpawned = false;
                if (IsGestureRecognizingNeeded) //need more than 2 points for gesture recognition
                {
                    string gestureName = mgData.RecognizeGesture(currentGesturePoints);
					if(mgData.IsRequiredGestureRecognized(	gestureName, mgGesture.getGestureIndex)
						 && m_battleMgr.currentGestureState != BattleManager.GestureState.END )
					{
					 
						if(m_battleMgr != null)
							m_battleMgr.CorrectGesture();

						// Call Event
						if(m_playerMgr != null)
							m_playerMgr.AddSpecialCount();


					}
					else // Incorrect Gesture
					{
						GameObject obj = Instantiate(Resources.Load("Prefabs/Battle/MISS")) as GameObject;
						Service.Get<HUDService>().HUDControl.AttachMid(ref obj);

					}
                    IsGestureRecognizingNeeded = false;
					currentGesturePoints.Clear();
                }

				strokeCount = strokeID;
            }
            strokeID = 0;
			currtime = 0;
        }
    }

	void AttackPhaseCheck()
    {
		if(m_battleMgr.CurBattlePhase != BattleManager.BattlePhase.ATTACK)
	 		return;
	 	 
		if (!IsTouchInProgress)//end
        {
            if ((trail != null)&&(!IsTrailQueuedForDestroy))
            {	
                IsTrailQueuedForDestroy = true;

//                // Clear the list
//       			foreach(GameObject obj in trailList)
//       			{
//       				Destroy(obj);
//       			}

                trailList.Clear();

#if UNITY_IPHONE && !UNITY_EDITOR
				if (pointsContainer != null)
					pointsContainer.Clear ();

				pointsContainer = new List<Point>();
				pointsContainer.AddRange(currentGesturePoints.ToArray ());
#else
				pointsContainer = GenericCopier<List<Point>>.DeepCopy(currentGesturePoints);
#endif

                IsStartTrailSpawned = false;
                if (IsGestureRecognizingNeeded && currentGesturePoints.Count > 0) //need more than 2 points for gesture recognition
                {
                    string gestureName = mgData.RecognizeGesture(currentGesturePoints);
					if(mgData.IsRequiredGestureRecognized(	gestureName, mgGesture.getGestureIndex)
						 && m_battleMgr.currentGestureState != BattleManager.GestureState.END )
					{
						// Call Event
						if(m_playerMgr != null)
							m_playerMgr.NormalAttack(null);

						// Particle Effect
						if(particleObj == null)
							particleObj = Instantiate(Resources.Load("Prefabs/PowerEffect"), Vector3.zero, Quaternion.identity) as GameObject;
						else
						{
							particleObj.GetComponent<ParticleSystem>().Play();
						}


					}
                    IsGestureRecognizingNeeded = false;
					currentGesturePoints.Clear();
                }

				strokeCount = strokeID;
            }
            strokeID = 0;
			currtime = 0;
        }
    }



   	/// <summary>
   	/// Draws the player input line ( Debug/Editor Mode only)
   	/// </summary>
    void DrawLine()
    {
		// Draw Line
		if(pointsContainer != null)
		{
			// Draw Debug Lines for current points
			for(int i = 0; i < pointsContainer.Count; ++i)
			{
				if(pointsContainer.Count > (i+1) && pointsContainer[i].StrokeID == pointsContainer[i+1].StrokeID)
				{
					Debug.DrawLine(Camera.main.ScreenToWorldPoint(new Vector3(pointsContainer[i].X, -pointsContainer[i].Y)),
									Camera.main.ScreenToWorldPoint(new Vector3(pointsContainer[i+1].X, -pointsContainer[i+1].Y)), Color.red);
				} 
			}
		}
    }
	
	// Update is called once per frameguide
	void Update ()
    {
    	if(disableGesture)
    		return;

        Vector2 cursorPosition = new Vector2();
        if ((Input.touchCount > 0))
        {
            Touch touch = Input.GetTouch(0);
            cursorPosition = touch.position;
			{
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        StartGesture(cursorPosition);
                        break;
					case TouchPhase.Stationary:
                        IsMovementInTouch = false;
                        break;
                    case TouchPhase.Moved:
                        MoveGesture(cursorPosition);
                        break;
                    case TouchPhase.Ended:
                        FinishGesture(cursorPosition);
                        break;
                    case TouchPhase.Canceled:
                        IsTouchInProgress = false;
                        break;
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            cursorPosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(0)) //touch begin
			{
				Debug.Log("Mouse clicked");
                if (!IsMouseMovementStarted)
                {
                    IsMouseMovementStarted = true;
                    StartGesture(cursorPosition);
                }
            }
            else //touchInProgress
            {
                if ((Mathf.Abs(Input.GetAxis("Mouse X")) > 0) || (Mathf.Abs(Input.GetAxis("Mouse Y")) > 0))//mouse moved
                    MoveGesture(cursorPosition);
                else //stationary
                    IsMovementInTouch = false;
            }
        }
        else //no input
        {
            if (IsMouseMovementStarted)
            {
                FinishGesture(Vector3.zero);
                IsMouseMovementStarted = false;
            }
        }

		if(IsStartTrailSpawned)
			currtime += Time.deltaTime;

        if (IsTouchInProgress)
        {
            if (!WasMovementInTouch) //begin
            {
				Vector3 pos = Camera.main.ScreenToWorldPoint(cursorPosition);
                pos.z = -1; // Make sure the trail is visible
               	trail = SpecialEffects.MakeTrail(pos);
               	trail.transform.parent = Camera.main.transform;
				trailList.Add(trail);
				IsStartTrailSpawned = true;
            }
			else if (IsMovementInTouch) //move
            {
				Vector3 position = Camera.main.ScreenToWorldPoint(cursorPosition);
				position.z = -1; // Make sure the trail is visible
                trail.transform.position = position;
            }
        }

		AttackPhaseCheck();
		SpecialAttackPhaseCheck();

#if UNITY_EDITOR
		DrawLine();
#endif

    }

    public void DestroyTrails()
    {
    	foreach(GameObject trail in trailList)
    		Destroy(trail);
    	trailList.Clear();
    }

}
