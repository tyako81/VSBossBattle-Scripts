using System.Collections.Generic;
using UnityEngine;

public class EquipmentDatabase : MonoBehaviour
{
    private static Dictionary<string, EquipmentData> equipmentDict = new Dictionary<string, EquipmentData>();

    [SerializeField] private List<EquipmentData> equipmentList;//Inspectorで装備データを設定

    private void Awake(){
        //Dictionaryに装備データを登録
        foreach (EquipmentData equipment in equipmentList){
            equipmentDict[equipment.Name] = equipment;
        }
    }

    //名前の取得
    public static EquipmentData GetEquipmentByName(string name){
        if (equipmentDict.TryGetValue(name, out EquipmentData equipment)){
            return equipment;
        }
        Debug.Log($"装備 {name} が見つかりません");
        return null;
    }
}
