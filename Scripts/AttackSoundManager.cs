using UnityEngine;

public class AttackSoundManager : MonoBehaviour
{

    string SelectedCharName;//選択キャラクター
    private AudioSource PlayerAttackSound;//プレイヤーの攻撃音


    void Awake(){
        //初期化
        SelectedCharName = PlayerPrefs.GetString("SelectedChar", "");
        PlayerAttackSound = GameObject.Find(SelectedCharName+"AttackSound").GetComponent<AudioSource>();
    }

    //HPに与えるための関数
    public AudioSource GetAttackSound(){
        return PlayerAttackSound;
    }

}
