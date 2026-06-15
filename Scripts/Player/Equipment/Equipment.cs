using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] // シリアライズしてインスペクターで設定可能に


public class Equipment{

    public string Name;//装備の名前
    public float AttackPower;//攻撃力
    public float DefensePower;//防御力
    public float JumpTime;//回避時間
    public Sprite Icon;//装備のアイコン

    public Equipment(string name, float attack, float defense, float jump, Sprite icon){
        Name = name;
        AttackPower = attack;
        DefensePower = defense;
        JumpTime = jump;
        Icon = icon;
    }

    public void Equip(EquipmentData equipmentData){
        Name = equipmentData.Name;
        AttackPower = equipmentData.AttackPower;
        DefensePower = equipmentData.DefensePower;
        JumpTime = equipmentData.JumpTime;
        Icon = equipmentData.Icon;
    }
}
