using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Service : MonoBehaviour {

	// Map to contain each service type
	static Dictionary<System.Type, object> 	m_serviceMap = new Dictionary<System.Type, object>();
	static GameObject 						m_obj;
	protected static Service				m_instance;


	static void Instantiate()
	{
		if(m_instance != null)
			return;

		// Search current Scene and see if object exists
		m_instance = FindObjectOfType(typeof(Service)) as Service;

		// if Unable to find serivce create new
		if(m_instance == null)
		{
			// Create empty object
			m_obj = new GameObject("Service");
			// Add Service component
			m_instance = m_obj.AddComponent<Service>();
		}
	}

	#region Mono Methods
	void Awake()
	{
		// Singleton service should never be destroyed until end of application
		DontDestroyOnLoad(this);
	}

	/// <summary>
	/// Destroy method when application exits
	/// </summary>
	void OnApplicationExit()
	{
		// there is no need to destroy dictionary objects since it will be destroyed at the end of the application
		m_serviceMap.Clear();
		Destroy(m_obj);
	}
	#endregion

	#region public

	/// <summary>
	/// Method to Init Service class
	/// </summary>
	public static void Init()
	{
		Instantiate();

		// Adding HUD Service
		Add<HUDService>();
	}


	public static T Get<T>() where T:CSingleton
	{
		Instantiate();
		Add<T>();

		return m_serviceMap[typeof(T)] as T;
	}
	public static void Add<T>() where T:CSingleton
	{
		if(!m_serviceMap.ContainsKey(typeof(T)))
		{
			// Make sure service adds the new Service Type
			T singleton = m_obj.AddComponent<T>();
			m_serviceMap.Add(typeof(T), singleton);
		}	
	}
	public static void Remove<T>() where T:CSingleton
	{
		if(m_serviceMap.ContainsKey(typeof(T)))
		{
			var singleton = m_serviceMap[typeof(T)];
			Destroy(singleton as T);
			m_serviceMap.Remove(typeof(T));
		}
	}

	#endregion
}
