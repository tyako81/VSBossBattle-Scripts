using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;//キャラクターの名前
    public GameObject characterPrefab;//オブジェクト
    public int Health;//HP
    public float Power;//攻撃力
    public float Speed;//歩く速さ
    public float LastAttackDelay;//攻撃後の硬直時間
    public float AttackSoundDelay;//攻撃音再生までの時間
}
