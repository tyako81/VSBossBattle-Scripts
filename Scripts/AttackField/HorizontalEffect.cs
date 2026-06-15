using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalEffect : MonoBehaviour
{
    private static readonly List<GameObject> ChildObjects = new List<GameObject>(); //AttackFieldの子オブジェクトを入れる配列

    private const float RockMoveDelay = 1.87f; //石の移動開始までの待機時間
    private const float RockMoveDuration = 0.25f;//石の移動時間
    private const float NextMoveDuration = 0.25f;//２回目の石の移動時間

    void OnEnable(){
        CollectChildObjects();
        StartCoroutine(SetPos());
    }

    //子オブジェクトをリストに追加する関数
    private void CollectChildObjects(){
        //現在のリストをクリア
        ChildObjects.Clear();

        //子オブジェクトを全て取得
        foreach (Transform child in transform){
            ChildObjects.Add(child.gameObject);
        }
    }

    //円の設置
    private IEnumerator SetPos(){
        //石の初期化
        foreach (var child in ChildObjects){
            Vector3 StartPos = child.transform.position;            

            //石を非表示にする
            child.SetActive(false);
            //石の移動
            StartCoroutine(MoveRock(child, StartPos));
            yield return new WaitForSeconds(NextMoveDuration);
            StartCoroutine(MoveRock(child, StartPos));
        }
    }

    //石のエフェクト
    private IEnumerator MoveRock(GameObject Rock, Vector3 StartPos){
        //移動位置
        Vector3 TargetPos = new Vector3(-StartPos.x, StartPos.y, StartPos.z);
            
        float timer = 0f;

        //移動まで待機
        yield return new WaitForSeconds(RockMoveDelay);

        while (timer < RockMoveDuration){
            //石の出現
            Rock.SetActive(true);
            //移動
            Rock.transform.position = Vector3.Lerp(StartPos, TargetPos, timer / RockMoveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        //石の位置のリセット
        Rock.transform.position = StartPos;
    }
}
