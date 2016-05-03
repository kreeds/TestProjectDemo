using UnityEngine;
using System.Collections;

namespace UnityExtensions
{
	/// <summary>
	/// Extensions to track and log delegate events
	/// </summary>
	public static class DelegateExtensions
	{
		// For event with no parameter
		public static void InvokeEvent(this System.Action eventAction)
		{
			if(eventAction == null)
			{
				Debug.LogWarning("Attempt to invoke an event with no subscribers");
				return;	
			}

			var delegateList = eventAction.GetInvocationList();
			
			if(delegateList != null)
			{
				foreach(var delegateAction in delegateList)
				{
					delegateAction.Method.Invoke(delegateAction.Target, null);
					Debug.Log(string.Format("Subscriber {0} of {1} is fired", delegateAction.Method.Name, delegateAction.Method.DeclaringType.FullName));
				}
			}
		}
		
		// For event with one parameter
		public static void InvokeEvent<T>(this System.Action<T> eventAction, T param)
		{
			if(eventAction == null)
			{
				Debug.LogWarning("Attempt to invoke an event with no subscribers");
				return;	
			}

			var delegateList = eventAction.GetInvocationList();
			
			if(delegateList != null)
			{
				foreach(var delegateAction in delegateList)
				{
					var paramArray = new object[] { param };
					delegateAction.Method.Invoke(delegateAction.Target, paramArray);
					Debug.Log(string.Format("Subscriber {0} of {1} is fired", delegateAction.Method.Name, delegateAction.Method.DeclaringType.FullName));
				}
			}
		}
		
		// For event with two parameters
		public static void InvokeEvent<T1, T2>(this System.Action<T1, T2> eventAction, T1 param1, T2 param2)
		{
			if(eventAction == null)
			{
				Debug.LogWarning("Attempt to invoke an event with no subscribers");
				return;	
			}

			var delegateList = eventAction.GetInvocationList();
			
			if(delegateList != null)
			{
				foreach(var delegateAction in delegateList)
				{
					var paramArray = new object[] { param1, param2 };
					delegateAction.Method.Invoke(delegateAction.Target, paramArray);
					Debug.Log(string.Format("Subscriber {0} of {1} is fired", delegateAction.Method.Name, delegateAction.Method.DeclaringType.FullName));
				}
			}
		}
		
		// For event with three parameters
		public static void InvokeEvent<T1, T2, T3>(this System.Action<T1, T2, T3> eventAction, T1 param1, T2 param2, T3 param3)
		{
			if(eventAction == null)
			{
				Debug.LogWarning("Attempt to invoke an event with no subscribers");
				return;	
			}
			
			var delegateList = eventAction.GetInvocationList();
			
			if(delegateList != null)
			{
				foreach(var delegateAction in delegateList)
				{
					var paramArray = new object[] { param1, param2, param3 };
					delegateAction.Method.Invoke(delegateAction.Target, paramArray);
					Debug.Log(string.Format("Subscriber {0} of {1} is fired", delegateAction.Method.Name, delegateAction.Method.DeclaringType.FullName));
				}
			}
		}
		
		// For general action with a param array.
		public static void InvokeEvent(this GenericAction eventAction, params object[] param)
		{
			if(eventAction == null)
			{
				Debug.LogWarning("Attempt to invoke an event with no subscribers");
				return;	
			}
		
			var delegateList = eventAction.GetInvocationList();
			
			if(delegateList != null)
			{
				foreach(var delegateAction in delegateList)
				{
					object[] paramArray = new object[] { param };
					delegateAction.Method.Invoke(delegateAction.Target, paramArray);
					Debug.Log(string.Format("Subscriber {0} of {1} is fired", delegateAction.Method.Name, delegateAction.Method.DeclaringType.FullName));
				}
			}
		}
	}
	
}
