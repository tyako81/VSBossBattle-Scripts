using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("参照オブジェクト")]
    [SerializeField] private GameObject EnemyAttackmotion; //アニメーション再生の判定オブジェクト

    private static int AttackPower = 1; 
    private Animator Animator; //アニメーション
    private CharacterController CharacterController; //敵の操作
    private bool isActive = false; //一回だけ攻撃モーションをするための管理フラグ

    private static readonly Vector3 Position_I = new Vector3(0, -14.0f, 20.0f); //初期位置
    private static readonly Vector3 Position_L   = new Vector3(-17.0f, -14.0f, 20.0f); //左の移動位置
    private static readonly Vector3 Position_C = new Vector3(0.0f, -14.0f, 20.0f);  //中央の移動位置
    private static readonly Vector3 Position_R  = new Vector3(17.0f, -14.0f, 20.0f); //右の移動位置

    private const float AttackWaitTime = 1.0f; //攻撃後の待機時間
    private const float MoveDuration = 1.0f; //移動時間
    private const float DownDuration = 2.0f; //撃破時の沈む時間
    private const float DownPosition = -25.0f; //敵の沈むY座標

    void Start(){
        //初期化
        Animator = GetComponent<Animator>();
        EnemyAttackmotion.gameObject.SetActive(false);
        CharacterController = GetComponent<CharacterController>(); 
        transform.position = Position_I;
    }

    void Update(){
        StartCoroutine(Enemy_Attack());
    }   

    //敵の攻撃モーション後に移動
    private IEnumerator Enemy_Attack(){
        //EnemyAttackmotionがアクティブになったら一回だけアニメーションを再生
        if(EnemyAttackmotion.activeSelf && !isActive){
            Animator.Play("Attack", 0, 0);
            isActive = true;
            //-1:左　0:真ん中　1:右
            int RandomIndex = Random.Range(-1, 2);
            yield return new WaitForSeconds(AttackWaitTime);

            Vector3 TargetPosition = Vector3.zero;   
            // 移動
            if(RandomIndex == -1){
                //左
                TargetPosition = Position_L;
            }else if(RandomIndex == 0){
                //真ん中
                TargetPosition = Position_C;
            }else if(RandomIndex == 1){
                //右
                TargetPosition = Position_R;
            }
            //移動開始
            yield return StartCoroutine(HorizontalMove(TargetPosition));
        }
    }

    //水平方向の移動
    private IEnumerator HorizontalMove(Vector3 TargetPosition){
        //現在の位置
        Vector3 StartPosition = transform.position;
        float timer = 0f;
        //移動
        while (timer < MoveDuration){
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, timer / MoveDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        //位置の微調整
        transform.position = TargetPosition;
    }

    //体力が0になったら敵が沈む
    public IEnumerator DefeteEnemy(){
        StopAllCoroutines();
        //現在の位置
        Vector3 StartPosition = transform.position;
        //沈む位置
        Vector3 TargetPosition = new Vector3(StartPosition.x, DownPosition, StartPosition.z); 

        float timer = 0f;
        //沈む
        while (timer < DownDuration){
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, timer / DownDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        this.gameObject.SetActive(false);
    }

    //FieldManagerでisActiveをfalseにするため
    public bool SetisActive(bool active){
        isActive = active;
        return isActive;
    }

    //Playerで被ダメージを計算するため
    public int GetAttackPower(){
        return AttackPower;
    }
}

