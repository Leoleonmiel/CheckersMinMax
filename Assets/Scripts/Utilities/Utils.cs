using System;
using UnityEngine;

public static class Utils
{
	public enum Type
	{
		Black,
		White
	}

	public enum PlayerType
	{
		Human,
		AI
	}

	public enum PlayerID
	{
		Player1,
		Player2
	}

    public enum Direction
    {
        Left = -1,  
        Right = 1,   
        Up = 1,      
        Down = -1   
    }

	[Serializable]
	public enum AIDifficulty
	{
		Easy,
		Medium,
		Hard
	}
}
