using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeAttack : MonoBehaviour
{
    [Header("参照オブジェクト")]
    [SerializeField] Player Player;//プレイヤーの攻撃モーションの判定
    [SerializeField] HP HP;//敵のHP
    
    private bool isEnter = false;//一回だけ判定する
    private const string BossTag = "Boss";//攻撃対象のタグ

    //接触している時は攻撃ができるように
    private void OnTriggerStay(Collider other){
        if(Player.GetisAttack() && other.gameObject.tag==BossTag){
                if(!isEnter){
                    isEnter = true;
                    StartCoroutine(HP.TakeDamage(Player.GetAttackPower()));
                }
        }
    }

    //モーションが終わったら判定の復活
    public bool SetisEnter(bool enter){
        isEnter = enter;
        return isEnter;
    }
    
    

}
