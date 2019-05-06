public static class Game {

	private static int launchAngle, distance, playIndex;
	private static string playerName;

	public static int LaunchAngle
	{
		get
		{
			return launchAngle;
		}
		set
		{
			launchAngle = value;
		}
	}

	public static int PlayIndex
	{
		get
		{
			return playIndex;
		}
		set
		{
			playIndex = value;
		}
	}

	public static int Distance
	{
		get
		{
			return distance;
		}
		set
		{
			distance = value;
		}
	}

	public static string PlayerName
	{
		get
		{
			return playerName;
		}
		set
		{
			playerName = value;
		}
	}
}
