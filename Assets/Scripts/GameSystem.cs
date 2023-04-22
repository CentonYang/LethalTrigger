using UnityEngine;
using System.IO;

public class GameSystem : MonoBehaviour
{
    static public int p1Char, p2Char, p1Color, p2Color, gamemode, life = 2, timer = 90;
    static public bool p1Comp, p2Comp;
    [System.Serializable]
    public class PlayerData
    {
        public int language;
        public double bgmVol, sfxVol, voiceVol;
    }
    static public PlayerData playerData = new PlayerData { language = 0, bgmVol = .5, sfxVol = .7, voiceVol = .7 };
    [RuntimeInitializeOnLoadMethod]
    static void OnStart()
    {
        if (!File.Exists(Application.dataPath + "/Save.dat"))
            File.WriteAllText(Application.dataPath + "/Save.dat", JsonUtility.ToJson(playerData));
        else
            playerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(Application.dataPath + "/Save.dat"));
    }

    public static void SaveGame(PlayerData pd)
    {
        if (pd != null)
            playerData = pd;
        File.WriteAllText(Application.dataPath + "/Save.dat", JsonUtility.ToJson(playerData));
    }
}