using UnityEngine;
using System.Collections;

public enum TransitionType
{
	Fade,
	FadeWait,
	None
};


public static class SceneManager
{
    //private static LoadingTransition _loadingTransition = null;

    private static AsyncOperation _asyncOpp = null;
    private static bool _transitionFlag = false;
    private static string _lastSceneName = "";
    private static string _nextSceneName = "";
    private static bool _onNonBlock;
    private static bool _onBlock;
	
	private static bool _androidBackFlag = false;

	private static string _blankTransitionScene = "BlankTransition";
	private static bool _loadingBlankTransitionScene = false;

    /// <summary>
    /// Initializes the <see cref="SequenceManager"/> class.
    /// </summary>
    static SceneManager()
    {
        _asyncOpp = null;

        _lastSceneName = "";
        _nextSceneName = "";

        _onBlock = false;
        _onNonBlock = false;

//        GameObject obj = Object.Instantiate(Resources.Load("2D/Prefab/Common/LoadingTransition")) as GameObject;
//        obj.name = "LoadingTransition";
//		Object.DontDestroyOnLoad(obj);
//		_loadingTransition = obj.GetComponent<LoadingTransition>();

    }
    public static void ChangeScene(string sceneName)
    {
		ChangeScene(sceneName, TransitionType.Fade);
    }
    public static void ChangeScene(string sceneName, TransitionType type)
    {

		if(sceneName.Length == 0){
            Debug.Log("SceneName is Null");
            return;
        }
        if (_nextSceneName.Length > 0){
        	Debug.Log("Current Loading some other scene");
            return;
        }
        //ロード中
        if (_asyncOpp != null && !_asyncOpp.isDone){
            return;
        }
        
        string curScene = Application.loadedLevelName;  //_nextSeqName;


        _lastSceneName = curScene;
        _nextSceneName = sceneName;

		if (type == TransitionType.None)
		{
			SceneManager.LoadNextLevelCore();
		}
		else
		{
			_transitionFlag = false;
			// Commence Transition Effects
//			_loadingTransition.ShowTransition(type, () => {
//				SceneManager.LoadNextLevel();
//			});
		}
    }

    private static bool _onLoadNextLevel = false;
    private static void LoadNextLevel()
    {
        _onLoadNextLevel = false;
    }

	public static void StartTransition(TransitionType type, System.Action act)
	{
		if (type == TransitionType.None)
		{
			act();
		}
		else
		{
			_transitionFlag = false;
		}
	}

	public static void FinishTransition()
	{
		_transitionFlag = true;
	}

    public static void LoadNextLevelCore()
    {

		_asyncOpp = Application.LoadLevelAsync (_nextSceneName);
		_nextSceneName = "";
        _onLoadNextLevel = true;
    }

	public static bool IsFinishTransition(TransitionType type)
	{
		bool ret = true;

        if (!_onLoadNextLevel) {
            LoadNextLevelCore();
        }

		if (_loadingBlankTransitionScene) {
			if(_asyncOpp.isDone)
			{
				//start loading the actual scene 
				_loadingBlankTransitionScene = false;
				LoadNextLevelCore();
			}
			ret = false; //return false as we are not transitiong out of the loading state
		}

		if (type == TransitionType.FadeWait)
		{
			ret = _transitionFlag;
		}
		else if (_asyncOpp != null)
		{
			ret = _asyncOpp.isDone;
		}
		
		return ret;
	}


    public static string GetLastSceneName()
    {
        return _lastSceneName;
    }

    public static void ShowLoadingIcon(bool show)
    {
    }

	public static void ShowProgressIconWhileConnecting(bool show)
	{
	}

    public static void ShowProgressIcon(bool show)
    {
        _onBlock = show;
        if (!show) {
        }
        else {
        }
    }

    public static void ShowProgressIconNonBlock(bool show)
    {
        //  出す場合、ブロック中ならスルー
        _onNonBlock = show;
        if (!_onBlock) {
        }
    }

    
    /// <summary>
    /// OS show
    /// </summary>
    /// <param name="show"></param>
    public static void ShowOSProgressIcon(bool show)
    {
#if !UNITY_STANDALONE_WIN && !UNITY_STANDALONE_OSX
        if (show) {
#if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.WhiteLarge);
#elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
#endif
            Handheld.StartActivityIndicator();
        }
        else {
            Handheld.StopActivityIndicator();
        }
#endif
    }

	public static bool IsAndroidBack()
	{
		return _androidBackFlag;
	}

	public static void SetAndroidBack(bool flag)
	{
		_androidBackFlag = flag;
	}


}