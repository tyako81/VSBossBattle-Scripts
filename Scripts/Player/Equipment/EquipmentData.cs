using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment/Create New Equipment")]
public class EquipmentData : ScriptableObject
{
    public string Name;//名前
    public float AttackPower;//攻撃力
    public float DefensePower;//防御力
    public float JumpTime;//回避時間
    public Sprite Icon;//アイコン
}