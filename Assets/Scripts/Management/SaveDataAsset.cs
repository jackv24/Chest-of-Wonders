using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CreateAssetMenu(menuName = "Data/Save Data Asset")]
public class SaveDataAsset : ScriptableObject
{
    [SerializeField]
    private SaveData data;

    public SaveData Data
    {
        get
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, data);
                ms.Position = 0;

                return (SaveData)formatter.Deserialize(ms);
            }
        }
    }
}
