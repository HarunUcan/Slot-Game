using UnityEngine;
using System.IO;

public class JsonSaveLoad
{

#if UNITY_EDITOR
    public string path = Application.dataPath + "/SaveFolder/PlayerSave.json";
#else
    public string path = Application.dataPath + "/PlayerSave.json";
#endif

    private CharacterStats characterStats = new CharacterStats();
    public void SaveData()
    {
        string data = JsonUtility.ToJson(characterStats);
        //Debug.Log(data);
        File.WriteAllText(path, data);
    }

    public void LoadData()
    {
        if (File.Exists(path))
        {
            string savedData = File.ReadAllText(path);

            characterStats = JsonUtility.FromJson<CharacterStats>(savedData);

            PlayerStats.AmountOfMoney = characterStats.AmountOfMoney;
        }
        else
        {
            PlayerStats.AmountOfMoney = 1000000;
        }
    }
}