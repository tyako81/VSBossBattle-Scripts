using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePos : MonoBehaviour
{
    private static List<GameObject> childObjects = new List<GameObject>(); // AttackFieldの子オブジェクトを入れるリスト
    private Vector3 Position = Vector3.zero; //円の座標
    private Vector3 previousPosition = Vector3.zero; //直前の座標
    private const float PositionThreshold = 5.0f; //判定する閾値
    private const int RandomPosX_min = -20; //X座標の最小値
    private const int RandomPosX_max = 20;  //X座標の最大値
    private const int RandomPosZ_min = -10; //Z座標の最小値
    private const int RandomPosZ_max = 10;  //Z座標の最大値

    private const float TreeStartY = -20.0f;   //木の初期Y座標
    private const float TreeTargetY = -0.61f;  //木の目標Y座標
    private const float TreeMoveDelay = 1.95f; //木の移動開始までの待機時間
    private const float TreeMoveDuration = 0.05f; //木の移動時間

    void OnEnable(){
        CollectChildObjects();
        SetPos();
    }

    // 子オブジェクトをリストに追加する関数
    private void CollectChildObjects(){
        //現在のリストをクリア
        childObjects.Clear();

        //子オブジェクトを全て取得
        foreach (Transform child in transform){
            childObjects.Add(child.gameObject);
        }
    }

    private void SetPos(){
        //座標リスト
        List<Vector3> UsedPositions = new List<Vector3>();

        //各オブジェクトにランダムな座標を設定
        foreach (var child in childObjects){
            //座標
            int PosX, PosZ;
            Vector3 NewPos;

            //これまでの座標と近いともう一回
            do {
                PosX = Random.Range(RandomPosX_min, RandomPosX_max);
                PosZ = Random.Range(RandomPosZ_min, RandomPosZ_max);
                NewPos = new Vector3(PosX, 0, PosZ);
            } while (CheckPos(NewPos, UsedPositions));

            //取得した座標をセット
            Position = NewPos;
            child.transform.position = Position;

            //使った座標を記録
            UsedPositions.Add(Position);

            //子オブジェクトの木を取得
            GameObject Tree = child.gameObject.transform.GetChild(0).gameObject;
            // 木の位置を初期化
            Tree.transform.position = new Vector3(PosX, TreeStartY, PosZ);
            // 木の位置の移動
            StartCoroutine(MoveTree(Tree, new Vector3(PosX, TreeTargetY, PosZ)));
        }
    }

    // 既存の座標と近すぎるかチェックする関数
    private bool CheckPos(Vector3 pos, List<Vector3> UsedPositions){
        foreach (var UsedPos in UsedPositions){
            //近いとき
            if (Mathf.Abs(pos.x - UsedPos.x) < PositionThreshold && Mathf.Abs(pos.z - UsedPos.z) < PositionThreshold){
                return true;
            }
        }
        //OKの時
        return false;
    }

    
    //木のエフェクト
    private IEnumerator MoveTree(GameObject Tree, Vector3 TargetPosition){
        //現在の位置
        Vector3 StartPosition = Tree.transform.position;
        float timer = 0f;

        //待機
        yield return new WaitForSeconds(TreeMoveDelay);

        //移動
        while (timer < TreeMoveDuration){
            Tree.transform.position = Vector3.Lerp(StartPosition, TargetPosition, timer / TreeMoveDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        //位置の微調整
        Tree.transform.position = TargetPosition;
    }
}
