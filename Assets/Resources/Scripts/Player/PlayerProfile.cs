using UnityEngine;
using System.Collections;

public class PlayerProfile {

	public string playerName;
	public int playerModelIndex;
	public int playerHairIndex;
	public int playerClothesIndex;


	static PlayerProfile playerProfile;


	static public PlayerProfile Get(){
		if (playerProfile == null) {
			playerProfile = new PlayerProfile();
			playerProfile.playerName = "Test";
			playerProfile.playerHairIndex = 0;
			playerProfile.playerClothesIndex = 0;
		}
	
		return playerProfile;
	}
}
