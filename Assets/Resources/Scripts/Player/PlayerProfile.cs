using UnityEngine;
using System.Collections;

public class PlayerProfile {

	public string playerName;
	public int playerModelIndex;
	public int playerHairIndex;
	public int playerClothesIndex;

	public int stamina;


	static PlayerProfile playerProfile;


	static public PlayerProfile Get(){
		if (playerProfile == null) {
			playerProfile = new PlayerProfile();
			playerProfile.playerName = "Test";
			playerProfile.playerHairIndex = 0;
			playerProfile.playerClothesIndex = 0;
			playerProfile.stamina = 20;
		}
	
		return playerProfile;
	}
}
