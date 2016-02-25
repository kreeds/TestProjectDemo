using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FiniteStateMachine{

	// List of states in the FSM
	private List<FSMState> states;

	// Current state ID
	private StateID curStateID;
	public StateID CurrentStateID
	{
		get{return curStateID;}
	}

	// Current State
	private FSMState curState;
	public FSMState currentState
	{
		get{return curState;}
	}

	// Constructor - Feels weird not destroying
	public FiniteStateMachine()
	{
		states = new List<FSMState>();
	}

	// Add State
	public void AddState(FSMState tstate)
	{
		if(tstate == null)
		{
			Debug.LogError("Null reference when adding State");
			return;
		}


		// Initial State
		if(states.Count == 0)
		{
			states.Add(tstate);
			curState = tstate;
			curStateID = tstate.STATE_ID;
			return;
		}


		// Check for duplicate State before adding
		foreach(FSMState s in states)
		{
			if(s.STATE_ID == tstate.STATE_ID)
			{
				Debug.LogError("Trying to add Duplicate state: " + tstate.STATE_ID.ToString());
				return;
			}
		}
		states.Add(tstate);

	}

	// Delete State

	public void DeleteState(StateID id)
	{
		if(id == StateID.NULL)
		{
			Debug.LogError("Unable to Delete Null State");
			return;
		}

		foreach(FSMState s in states)
		{
			if(s.STATE_ID == id)
			{
				states.Remove(s);
				return;
			}
		}

		// Unable to locate State
		Debug.LogError("Unable to locate state with StateID: " + id.ToString());
	}

	// Transit State
	public void Transit(Transition trans)
	{
		// Check for null
		if(trans == Transition.NULL)
		{
			Debug.LogError("Null Transition not allowed");
			return;
		}

		StateID id = currentState.GetStateFromTrans(trans);
		if(id == StateID.NULL)
		{
			Debug.LogError("Unable to get State trans: " + trans.ToString());
			return;
		}

		curStateID = id;
		foreach(FSMState state in states)
		{
			if(state.STATE_ID  == curStateID)
			{
				curState.OnExit();
				curState = state;
				curState.OnEnter();
				break;
			}
		}
	}
}
