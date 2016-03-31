using UnityEngine;
using System.Collections;

public class SoundService : CSingleton {

//	GameObject m_Bgm
//	GameObject m_bgmObj;
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//
//	public void Play(string soundObjName, 
//            SoundOpt soundOpt = SoundOpt.None, 
//            string soundName = "", 
//            bool restart = true, 
//            float volume = 1, 
//            float pitch = 1, 
//            int priority = 128,
//            float delay = 0f,
//            bool preventDuplicates = false
//            )
//    {
//        AudioSource audioSource = null;
//        SoundProperty soundProperty = null;
//        bool isBGM = (soundOpt & SoundOpt.IsBGM) != 0;
//        
//        if(soundName == string.Empty)
//        {
//            soundName = soundObjName;    
//        }
//        
//        if (preventDuplicates)
//        {
//            GameObject soundObject = GetSoundGameObject(soundObjName);
//            if(soundObject != null)
//            {
//                audioSource = soundObject.GetComponent<AudioSource>();
//            }
//        }
//
//        if (audioSource == null)
//        {
//            AudioClip audio = null;
//            if((soundOpt & SoundOpt.IsDLC) != 0)
//            {
//                D.Log ("=========================== playing DLC sound");
//                Object audioObj = null;
//                if(BH6Service.Get<SoundAssetService>().sound_assets != null)
//                {
//                    audioObj = BH6Service.Get<SoundAssetService> ().getAsset (soundName, true);
////                    BH6Service.Get<SoundAssetService>().sound_assets.TryGetValue(soundName, out audioObj);
//                }
//                audio = audioObj as AudioClip;
//                
//            }
//            else
//            {
//                audio = BH6Service.Get<SoundAssetService>().LoadFromResource("Sounds", soundName) as AudioClip;
//            }
//            
//            if(audio == null)
//            {
//                D.Warn("Cannot find the sound that you want to play - " + soundName);
//                return;
//            }
//            
//            GameObject soundObject;
//            if (isBGM)
//            {
//                if(m_bgmObj != null)
//                {
//                    Destroy(m_bgmObj);
//                }
//
//                soundObject = new GameObject(soundObjName);
//                m_bgmObj = soundObject;
//                soundObject.transform.parent = m_containerBGM.transform;
//            }
//            else
//            {
//                soundObject = new GameObject(soundObjName);
//                soundObject.transform.parent = m_containerSFX.transform;
//            }
//            audioSource         = soundObject.AddComponent<AudioSource>();
//            audioSource.clip    = audio;
//            soundProperty        = new SoundProperty(soundObject, isBGM, volume);
//
//            AudioSourceExt audioSourceExt = soundObject.AddComponent<AudioSourceExt>();
//            audioSourceExt.DoNotDestroy = false;
//            audioSourceExt.IsBGM = isBGM;
//            audioSourceExt.Volume = volume;
//            if(delay > 0f)
//            {
//                audioSourceExt.StartedPlayback = false;
//            }
//            else
//            {
//                audioSourceExt.StartedPlayback = true;
//            }
//        }
//        
//        if(audioSource != null)
//        {
//            if(isBGM)
//            {
//                if(IsIPodMusicPlaying())
//                {
//                    audioSource.volume = 0f;
//                }
//                else
//                {
//                    audioSource.volume = volume * BH6Service.Get<PlayerService>().MusicVolume * m_MuteVolume;
//                }
//            }
//            else
//            {
//                audioSource.volume    = volume * BH6Service.Get<PlayerService>().EffectsVolume * m_MuteVolume;
//            }
//
//            audioSource.pitch        = pitch;
//            audioSource.loop        = (soundOpt & SoundOpt.Loop) != 0;
//            audioSource.priority    = priority;
//
//            if((soundOpt & SoundOpt.FadeOut) != 0)
//            {
//                this.StartTask(PlayAndFadeOut(audioSource));
//            }
//            else
//            {
//                if(restart || !audioSource.isPlaying)
//                {
//                    if(delay > 0f)
//                    {
//                        StartCoroutine(AudioSourcePlayWithDelay(audioSource, delay));
//                    }
//                    else
//                    {
//                        audioSource.Play();
//                    }
//                }
//                else
//                {
//                    D.Log("Already playing source: " + soundName);
//                }
//            }
//        }
//    }
}
