using UnityEngine;
using System.Collections;

public class GameService : CSingleton {

	PlayerProfile m_playerProfile;

	void Awake()
	{
		m_playerProfile = PlayerProfile.Get();
		Load();
	}

	public void Save()
	{
		PlayerPrefs.SetString(GameConstants.PLAYERNAME, m_playerProfile.playerName);
		PlayerPrefs.SetInt(GameConstants.MODELINDEX, m_playerProfile.playerModelIndex);
		PlayerPrefs.SetInt(GameConstants.HAIRINDEX, m_playerProfile.playerHairIndex);
		PlayerPrefs.SetInt(GameConstants.CLOTHESINDEX, m_playerProfile.playerClothesIndex);

		PlayerPrefs.SetInt(GameConstants.STAMINA, m_playerProfile.stamina);
		PlayerPrefs.SetInt(GameConstants.MAXSTAMINA, m_playerProfile.maxStamina);
		PlayerPrefs.SetInt(GameConstants.GOLD, m_playerProfile.gold);
		PlayerPrefs.SetInt(GameConstants.GEMS, m_playerProfile.gems);
		PlayerPrefs.SetInt(GameConstants.LEVELS, m_playerProfile.level);

		PlayerPrefs.SetInt(GameConstants.UNREADNEWSCNT, m_playerProfile.unreadNewsCnt);

		PlayerPrefs.Save();

		Debug.Log(string.Format("Saving values... - MaxStamina:{0}, stamina: {1}" , m_playerProfile.maxStamina, m_playerProfile.stamina));
	}

	public void Load()
	{
		Debug.Log("*******LOADING");
		if(PlayerPrefs.GetInt(GameConstants.MAXSTAMINA) == 0)
			return;

		m_playerProfile.playerName = PlayerPrefs.GetString(GameConstants.PLAYERNAME);
		m_playerProfile.playerModelIndex = PlayerPrefs.GetInt(GameConstants.MODELINDEX);
		m_playerProfile.playerHairIndex = PlayerPrefs.GetInt(GameConstants.HAIRINDEX);
		m_playerProfile.playerClothesIndex = PlayerPrefs.GetInt(GameConstants.CLOTHESINDEX);
		m_playerProfile.stamina = PlayerPrefs.GetInt(GameConstants.STAMINA);
		m_playerProfile.maxStamina = PlayerPrefs.GetInt(GameConstants.MAXSTAMINA);
		m_playerProfile.gold = PlayerPrefs.GetInt(GameConstants.GOLD);
		m_playerProfile.gems = PlayerPrefs.GetInt(GameConstants.GEMS);
		m_playerProfile.level = PlayerPrefs.GetInt(GameConstants.LEVELS);
		m_playerProfile.unreadNewsCnt = PlayerPrefs.GetInt(GameConstants.UNREADNEWSCNT);

		Debug.Log(string.Format("Loading values... - MaxStamina:{0}, stamina: {1}" , m_playerProfile.maxStamina, m_playerProfile.stamina));


	}

}
