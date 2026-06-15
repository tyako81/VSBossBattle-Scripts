using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldManager : MonoBehaviour
{
    private static List<GameObject> ChildObjects = new List<GameObject>(); //AttackFieldの子オブジェクトを入れる配列
    private static List<GameObject> SelectedObjects = new List<GameObject>(); //AttackFieldのオブジェクトを入れる配列
    private bool getRandom = false; //抽選中の状態を管理
    private bool isCollision = false; //当たり判定を管理
    private bool isCounting = false; //カウントダウン中かどうかを管理
    
    
    private const float AttackDuration = 1.5f;//攻撃範囲の表示時間
    private const float AttackSpeed = 1.5f;//攻撃範囲の点滅速度
    private const float AttackCenterSize = 0.8f;//攻撃範囲の基本サイズ
    private const float AttackDeltaSize = 0.3f;//攻撃範囲のサイズ変動量
    private const float AttackWarningDuration = 0.5f;//攻撃警告時間
    private const float AttackDelay = 0.2f;//判定のディレイ
    private const int HPThreshold = 7;//HPの閾値
    private int AttackCount = 1;//敵の攻撃回数

    [Header("ランダム抽選設定")]
    [SerializeField] private float minWaitTime = 1.0f;//抽選の最小待機時間
    [SerializeField] private float maxWaitTime = 1.5f;//抽選の最大待機時間
    [SerializeField] private float minNextWaitTime = 0.0f;//次の抽選の最小待機時間
    [SerializeField] private float maxNextWaitTime = 1.0f;//次の抽選の最大待機時間
    
    [Header("参照オブジェクト")]
    [SerializeField] private GameObject EnemyAttackMotion;//アニメーションを再生を判定するオブジェクト
    [SerializeField] private Material Material;//攻撃範囲のマテリアル
    [SerializeField] private Enemy Enemy;//モーションオブジェクトの関数を持ってくるため
    [SerializeField] private Slider hpSlider;//敵のスライダー参照
    [SerializeField] private CountDown CountDown;//カウントダウン管理
    [SerializeField] private AudioSource BossAttackSound;//ボスの攻撃音
    [SerializeField] private AudioSource ChargeSound;//チャージ音
    

    void Start(){
        StartCoroutine(StartSequence()); 
    }

    //攻撃の抽選およびループ
    private IEnumerator StartSequence(){
        // 子オブジェクトの収集
        CollectChildObject();
        // 子オブジェクトの非アクティブ化
        foreach (var child in ChildObjects){
            child.SetActive(false);
        }
        //CountDownコルーチンが終了するまで待機
        yield return StartCoroutine(CountDown.StartCountDown());
        isCounting = true;
        yield return StartCoroutine(RandomizeSelection());
    }

    void Update(){
        //敵のHPが０になったら攻撃範囲表示の停止
        //敵の動きも止まる
        //一回だけ通るようにする
        if (hpSlider.value == 0 && getRandom){
            //コルーチンの停止
            //例外処理
            try{
                StopAllCoroutines();
            }catch (System.ArgumentNullException e){
                Debug.Log($"No coroutines to stop: {e.Message}");
            }
            getRandom = false;
            foreach (var child in ChildObjects){
                child.SetActive(false);
            }
        }
    }

    //子オブジェクトをリストに追加する関数
    public void CollectChildObject(){
        //現在のリストをクリア
        ChildObjects.Clear();
        //子オブジェクトを全て取得
        foreach (Transform child in transform){
            ChildObjects.Add(child.gameObject);
        }
    }

    //ランダムに抽選するコルーチン
    private IEnumerator RandomizeSelection(){
        getRandom = true;
        while (getRandom){
            //リストの削除
            SelectedObjects.Clear();

            //秒でランダムな待機時間を設定
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            //攻撃回数を決定 HPが7以下なら2回
            if(hpSlider.value <= HPThreshold){
                AttackCount = 2;
            }
            for (int i=0;i<AttackCount;i++){
                //ランダム選択(２回選択するときは同じ値を取らないようにする)
                GameObject selectedObject;
                do{
                    int RandomIndex = Random.Range(0, ChildObjects.Count);
                    selectedObject = ChildObjects[RandomIndex];
                }while(SelectedObjects.Contains(selectedObject));

                SelectedObjects.Add(selectedObject);
                //攻撃範囲表示
                selectedObject.SetActive(true);
            }

            float timer = 0f;
            //チャージ音の再生
            ChargeSound.Play();
            ChargeSound.loop = true;

            //攻撃範囲の点滅
            while (timer < AttackDuration){
                //余弦波でマテリアルの変更
                float alpha = AttackCenterSize + Mathf.Cos(Time.time * Mathf.PI * 2.0f * AttackSpeed) * AttackDeltaSize;
                //赤色
                Material.SetColor("_BaseColor", new Color(1.0f, 0.3f, 0.3f, alpha));
                timer += Time.deltaTime;
                //次のフレームまで待機
                yield return null;                
            }

            //攻撃が来る瞬間に攻撃範囲の色を赤に変える
            Material.color = Color.red;
            //攻撃モーションの再生
            EnemyAttackMotion.SetActive(true);
            //チャージ音ループ停止
            ChargeSound.loop = false;
            //攻撃音の再生
            BossAttackSound.Play();
            yield return new WaitForSeconds(AttackWarningDuration);
            //他スクリプトのモーション再生待機時間
            yield return new WaitForSeconds(AttackDelay);
            //判定の付与
            isCollision = true;
            //判定するために少しディレイをかける
            yield return new WaitForSeconds(AttackDelay);
            //判定の剥奪
            isCollision = false;
            yield return new WaitForSeconds(AttackDelay);
            //攻撃範囲削除
            foreach (var obj in SelectedObjects){
                obj.SetActive(false);
            }
            //攻撃モーションのオブジェクト削除,リセット
            Enemy.SetisActive(false);
            EnemyAttackMotion.SetActive(false);

            //ランダムな待機時間を設定
            waitTime = Random.Range(minNextWaitTime, maxNextWaitTime);
            //残り時間待機
            yield return new WaitForSeconds(waitTime);
        }
    }

    //Playerで当たり判定をするため
    //判定が付与された時に敵の攻撃モーション
    public bool GetisCollision(){
        return isCollision;
    }

    //クリア時に攻撃のループを終了するため
    public bool SetgetRandom(bool setbool){
        getRandom = setbool;
        return getRandom;
    }

    //カウントダウン中かどうか判断
    public bool GetisCounting(){
        return isCounting;
    }

    //音楽のループ再生
    public bool SetChargeSoundloop(bool setbool){
        ChargeSound.loop = setbool;
        return ChargeSound.loop;
    }
}
