using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Transition
{
	NULL = 0,	// represent no transitions in FSM
	E_FAILGESTURE,
	E_LOSTHP,
	E_FINISHATTACK,
	E_NOHP
};

public enum StateID
{
	NULL = 0,  // represent no state in system
	E_IDLE,
	E_ATTACK,
	E_DEATH,
	E_DAMAGED
}

public abstract class FSMState
{
	// Map container to map transitions to state
	protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();

	protected StateID stateID;
	public StateID STATE_ID
	{
		get{return stateID;}
	}

	// Get State resulting from transition
	public StateID GetStateFromTrans(Transition trans)
	{
		if(map.ContainsKey(trans))
			return map[trans];
		else 
			return StateID.NULL;
	}

	// Add Transition
	public void AddTransition(Transition trans, StateID id)
	{
		//check if transition exist
		if(trans == Transition.NULL)
		{
			Debug.LogError("Trying to add a null transition");
			return;
		}
		if(id == StateID.NULL)
		{
			Debug.LogError("Trying to add a null state");
			return;
		}

		// check if current map already contains key
		if(map.ContainsKey(trans))
		{
			Debug.LogError(trans.ToString() + " transition Exist, unable to have duplicate transitions for State: " + id.ToString());
			return;
		}

		map.Add(trans, id);
	}
	// Delete Transition
	public void DeleteTransition(Transition trans)
	{
		//check if transition exist
		if(trans == Transition.NULL)
		{
			Debug.LogError("Trying to add a null transition");
			return;
		}
		// check if current map contains key
		if(map.ContainsKey(trans))
		{
			map.Remove(trans);
		}
	}

	// Init method before entering state
	public virtual void OnEnter()
	{

	}
	// Exit method before leaving state
	public virtual void OnExit()
	{
		
	}
	// Update method
	public abstract void Update();

	// Transition Rule
	public abstract void Transit();

}
