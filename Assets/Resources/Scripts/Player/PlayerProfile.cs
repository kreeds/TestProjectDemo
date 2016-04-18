using UnityEngine;
using System.Collections;

public class PlayerProfile {

	public string playerName;
	public int playerModelIndex;
	public int playerHairIndex;
	public int playerClothesIndex;

	public int stamina;
	public int maxStamina;

	public int gold;
	public int gems;

	public int level;

	public const float recoverTime = 300f;

	public float recoverAmount;


	static PlayerProfile playerProfile;

	PlayerProfile()
	{
		playerName = "Ellie";
		playerHairIndex = 0;
		playerClothesIndex = 0;
		
		maxStamina = stamina = 30;
		gems = 500;
		gold = 500000;
		level = 1;

		recoverAmount = 0;
	}

	static public PlayerProfile Get(){
		if (playerProfile == null) {
			playerProfile = new PlayerProfile();

		}
	
		return playerProfile;
	}

	public bool IsActionAvailable(int cost){
		return stamina > cost;
	}

	public void StartAction(int cost){
		stamina -= cost;
		if (recoverAmount <= 0)
			recoverAmount = recoverTime;
	}

	public void UpdateTime(float deltaTime){
		if (stamina < maxStamina) {
			recoverAmount -= Time.fixedDeltaTime;
			if (recoverAmount <= 0){
				stamina++;
			}
		}
	}
}
