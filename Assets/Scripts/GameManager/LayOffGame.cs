using UnityEngine;

public class LayOffGame
{
    public const float ASTEROIDS_SPAWN_TIME = 3.0f;

    public const float PLAYER_RESPAWN_TIME = 4.0f;

    public const string PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

    public static Color GetColor(int colorChoice)
    {
        switch (colorChoice)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
        }

        return Color.black;
    }
}
