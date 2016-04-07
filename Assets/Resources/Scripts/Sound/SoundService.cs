using UnityEngine;
using System.Collections;

public class SoundService : CSingleton {

	GameObject m_BGMContainer;
	GameObject m_SFXContainer;

	AudioSource[]	m_BGMSource;
	AudioSource[]	m_SFXSounds;

	AudioClip[]		m_preload;

	const int MAX_BGMCIP = 3;
	const int MAX_SFX = 16;

	void Awake()
	{
		// Create the BGM containers;
		m_BGMContainer = GameObject.Find("BGMContainer");
		if(m_BGMContainer == null)
		{
			m_BGMContainer = Instantiate(Resources.Load("Prefabs/SFX/BGMContainer")) as GameObject;
		}

		m_SFXContainer = GameObject.Find("SFXContainer");
		if(m_SFXContainer == null)
		{
			m_SFXContainer = Instantiate(Resources.Load("Prefabs/SFX/SFXContainer")) as GameObject;
		}

		m_BGMSource = new AudioSource[MAX_BGMCIP];
		m_SFXSounds = new AudioSource[MAX_SFX];

		for (int i = 0; i < m_SFXSounds.Length; i++) 
		{
			m_SFXSounds[i] = (AudioSource) m_SFXContainer.AddComponent(typeof(AudioSource));
		}

		for (int i = 0; i < m_BGMSource.Length; i++) 
		{
			m_BGMSource[i] = (AudioSource) m_BGMContainer.AddComponent(typeof(AudioSource));
		}
	}

	public void PreloadSFXResource(string[] name)
	{
		if(name == null || name.Length == 0)
		{
			return;
		}

		m_preload = new AudioClip[name.Length];
		for(int i = 0; i < name.Length; ++i)
		{
			m_preload[i] = Resources.Load("Sound/" + name[i]) as AudioClip;
		}
	}

	public AudioClip GetSFX(string name)
	{
		foreach(AudioClip clip in m_preload)
		{
			if(clip.name == name)
			{
				return clip;
			}
		}

		return null;
	}


	public int PlaySound(AudioClip c, bool loop)
	{
		for(int i = 0; i < m_SFXSounds.Length; i++)
		{
			AudioSource s = m_SFXSounds[i];
			//  check if sound is playing
			if(!s.isPlaying)
			{
				s.clip = c;
				s.loop = loop;
				s.Play();
				SetSoundVolume(i, 1.0f, 1.0f);
				return i;
			}
		}

		return -1;
	}

	public int PlayMusic(AudioClip c, bool loop)
	{
		for(int i = 0; i < m_BGMSource.Length; i++)
		{
			AudioSource s = m_BGMSource[i];
			//  check if sound is playing
			if(!s.isPlaying)
			{
				s.clip = c;
				s.loop = loop;
				s.Play();
				SetMusicVolume(i, 1.0f, 1.0f);
				return i;
			}
		}

		return -1;
	}


	public void StopMusic(AudioClip c)
	{
		foreach(AudioSource s in m_BGMSource)
		{
			if(s.clip == c && s.isPlaying)
			{
				s.Stop();
			}
		}
	}

	public void StopMusic(int channel)
	{
		if(m_BGMSource != null && m_BGMSource.Length > channel)
			m_BGMSource[channel].Stop();	
	}

	/// <summary>
	/// Sets the volume of the audio source
	/// </summary>
	/// <param name="i">The index of the Sound audio source</param>
	/// <param name="newVol">New vol.</param>
	/// <param name="time">Transition time</param>
	public void SetSoundVolume(int i, float newVol, float time)
	{
		float oldVolume = m_SFXSounds[i].volume;
		StartCoroutine(AdjustVolume(m_SFXSounds[i], oldVolume, newVol, time));
	}

	public void SetSoundVolume(int i, float newVol)
	{
		m_SFXSounds[i].volume = newVol;
	}

	/// <summary>
	/// Sets the volume of the audio source
	/// </summary>
	/// <param name="i">The index of the Sound audio source</param>
	/// <param name="newVol">New vol.</param>
	/// <param name="time">Transition time</param>
	public void SetMusicVolume(int i, float newVol, float time)
	{
		float oldVolume = m_BGMSource[i].volume;
		StartCoroutine(AdjustVolume(m_BGMSource[i], oldVolume, newVol, time));
	}

	public void SetMusicVolume(int i, float newVol)
	{
		m_BGMSource[i].volume = newVol;
	}


	IEnumerator AdjustVolume(AudioSource src, float oldVolume, float newVolume, float time)
	{
		//float transitionStart = Time.time;
		float timeElapsed = 0.0f;
		while(timeElapsed < time)
		{
			timeElapsed += Time.deltaTime/time;
			src.volume = Mathf.Lerp(oldVolume, newVolume, timeElapsed);

			yield return null;
		}

		src.volume = newVolume;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
