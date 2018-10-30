using UnityEngine;

[CreateAssetMenu(menuName = "Data/Save Data Asset")]
public class SaveDataAsset : ScriptableObject
{
    [SerializeField]
    private SaveData data;
    public SaveData Data { get { return data; } }
}
