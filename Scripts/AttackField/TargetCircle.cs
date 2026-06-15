using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCircle : MonoBehaviour
{
    private static Vector3 Circleposition = Vector3.zero;//円の位置
    private static Vector3 TreePos = Vector3.zero;//木の位置
    private static GameObject Tree;//木のオブジェクト
    private const float StartTreeY = -2000f;//木の初期位置
    private const float EndTreeY = -61f;//木の最後の位置
    private const float WaitTreeEffect = 1.5f;//木のエフェクトの待機時間
    private const float TreeMoveTimer = 0.05f;//木の移動時間
    private const float TrackTime = 0.6f;//追尾する時間
    private bool isMove = true;//追尾フラグ

    [Header("参照オブジェクト")]
    [SerializeField] GameObject Player;//プレイヤーの位置を取得する

    // Start is called before the first frame update
    void OnEnable(){
        //初期化
        isMove = true;
        //現在のプレイヤーの位置に円を出す
        Circleposition = Player.transform.position;

        //プレイヤーを追尾する
        StartCoroutine(Tracking());
        
        //木を取得
        Tree = this.gameObject.transform.GetChild(0).gameObject;
        //木の位置を初期化
        Tree.transform.localPosition = new Vector3(0, StartTreeY, 0);
    }

    void Update(){
        //isMoveがtrueの時は追尾する
        if(isMove){
            //現在のプレイヤーの位置に円と木を追尾
            Circleposition = Player.transform.position;
            this.transform.position = new Vector3(Circleposition.x, 0, Circleposition.z);
        }
    }

    //プレイヤーを追尾する時間待機
    private IEnumerator Tracking(){
        yield return new WaitForSeconds(TrackTime);
        isMove = false;
        StartCoroutine(MoveTree());
    }

    //木のエフェクト
    private IEnumerator MoveTree(){
        float timer = 0f;
        //現在の位置
        Vector3 StartPosition = new Vector3(0, StartTreeY, 0);
        Vector3 TargetPosition = new Vector3(0, EndTreeY, 0);
        //待機
        yield return new WaitForSeconds(WaitTreeEffect);

        while (timer < TreeMoveTimer){
            //移動
            Tree.transform.localPosition = Vector3.Lerp(StartPosition, TargetPosition, timer / TreeMoveTimer);
            timer += Time.deltaTime;
            yield return null;
        }
        
        //微調整
        Tree.transform.localPosition = TargetPosition;
    }
}
