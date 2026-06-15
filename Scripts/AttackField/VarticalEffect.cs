using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarticalEffect : MonoBehaviour
{
    private static readonly List<GameObject> ChildObjects = new List<GameObject>();//AttackFieldの子オブジェクトを入れる配列
    private const float TreeNum = 3;
    private const float StartTreeY = -20.0f;//木の初期位置
    private const float EndTreeY = -0.61f;//木の最後の位置
    private const float WaitTreeEffect = 1.95f;//木のエフェクトの待機時間
    private const float TreeMoveTimer = 0.05f;//木の移動時間

    void OnEnable(){
        CollectChildObjects();
        SetPos();
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

    private void SetPos(){
        foreach (var child in ChildObjects){
            for(int i=0; i<TreeNum; i++){
                //それぞれの木を取得
                GameObject tree = child.gameObject.transform.GetChild(i).gameObject;
                Vector3 StartPos = tree.transform.position;
                //木の位置を初期化
                tree.transform.position = new Vector3(StartPos.x, StartTreeY, StartPos.z);
                //木の位置の移動
                StartCoroutine(MoveTree(tree, new Vector3(StartPos.x, EndTreeY, StartPos.z)));
            }
        }
    }
    
    //木のエフェクト
    private IEnumerator MoveTree(GameObject Tree, Vector3 TargetPosition){
        //現在の位置
        Vector3 StartPosition = Tree.transform.position;

        float timer = 0f;

        yield return new WaitForSeconds(WaitTreeEffect);

        while (timer < TreeMoveTimer){
            //移動
            Tree.transform.position = Vector3.Lerp(StartPosition, TargetPosition, timer / TreeMoveTimer);
            timer += Time.deltaTime;
            yield return null;
        }
        
        //微調整
        Tree.transform.position = TargetPosition;
    }
}
