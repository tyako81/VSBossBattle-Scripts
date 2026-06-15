using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class Player : MonoBehaviour {

    string SelectedCharName;//選択キャラクター
    CharacterData selectedChar;//キャラクターデータ
    string EquippedItemName;//装備名
    EquipmentData equippedItem;//装備データ
    private float AttackPower;//プレイヤーの攻撃力
    private float Damage;//被ダメージ
    private float isAvoidTime = 0.15f;//回避中の無敵時間
    private float Speed;//移動速度
    private const float Tolerance = 0.3f;//Rayの判定距離
    private const float JumpMoveTimer = 0.4f;//ジャンプの移動時間
    private const float JumpWaitTimer = 0.7f;//ジャンプのクールタイム
    private const float JumpDistance = 1.5f;//ジャンプの距離
    private const float FirstAttackDelay = 0.2f;//攻撃の開始までの待機時間
    private float LastAttackDelay;//攻撃後の硬直
    private const float KnockBackDistance = 0.1f;//ノックバックの距離
    private const float KnockbackTime = 0.2f;//ノックバック時間
    private const float StunTime = 1.5f;//ヒット後の硬直時間
    private const float CameraShakeDuration = 0.1f;//カメラシェイクの時間
    private const float CameraShakeIntensity = 1.0f;//カメラシェイクの強さ
    private const float SceneChangeTime = 0.15f;//画面色変化の時間
    private const float SceneAlpha = 0.7f;//画面エフェクトの透明度
    private const float RayOffset = 0.1f;//rayの距離
    private const float DelayAssignisAvoidTime = 0.15f;//無敵付与までの待ち時間
    private bool isJumping = false;  // ジャンプフラグ
    private bool isAttack = false;//攻撃フラグ
    private bool isHit = false;//当たり判定フラグ
    private bool isAvoid = false;//回避フラグ
    private CharacterController CharacterController;//プレイヤーの操作
    private Animator Animator;//アニメーション
    private static Vector3 MoveDirection = Vector3.zero;//キャラ座標
    //キャラクターの動き
    private int Animatornum = 0;//どのアニメーションをしているか0アイドル1走り
    private float Velocity = 0;//アニメーションブレンド変数


    [Header("参照オブジェクト")]
    [SerializeField] private Equipment equipment;
    [SerializeField] HP HP;//HPから関数を持ってくるため
    [SerializeField] Enemy Enemy;//Enemyから敵の攻撃力を持ってくるため
    [SerializeField] FieldManager FieldManager;//当たり判定・カウントダウンの関数を持ってくるため
    [SerializeField] JudgeAttack JudgeAttack;//モーションが終わったら攻撃判定の復活
    [SerializeField] GameObject hpSlider_P;//HPの表示
    [SerializeField] Image SceneImage;//画面全体のシーン
    [SerializeField] AudioSource BeforeAttackSound;//攻撃音
    [SerializeField] AudioSource AvoidSound;//避ける音
    [SerializeField] Image EquipmentImage;//装備のイメージ
    [SerializeField] Image EquipmentBackImage;//装備表示の背景
    
   void Awake(){
        
   }
	// Use this for initialization
	void Start(){
        //初期化
        CharacterController = GetComponent<CharacterController>();
        //キャラクターの初期化
        SelectedCharName = PlayerPrefs.GetString("SelectedChar", "");
        Animator = transform.Find(SelectedCharName + "(Clone)").GetComponent<Animator>();
        selectedChar = CharacterDatabase.GetCharacterByName(SelectedCharName);
        LastAttackDelay = selectedChar.LastAttackDelay;
        AttackPower += selectedChar.Power;
        Speed = selectedChar.Speed;
        //Debug.Log(Animator.gameObject.name);
        hpSlider_P.SetActive(true);

        //基本ダメージ量
        Damage = Enemy.GetAttackPower();
        //装備の反映
        EquippedItemName = PlayerPrefs.GetString("EquippedItem", "");
        if(!string.IsNullOrEmpty(EquippedItemName)){
            equippedItem = EquipmentDatabase.GetEquipmentByName(EquippedItemName);
            if(equippedItem != null){
                //装備をもとにステータスの計算
                AttackPower = AttackPower + equippedItem.AttackPower;
                Damage -= equippedItem.DefensePower;
                isAvoidTime = isAvoidTime + equippedItem.JumpTime;
                SetEquipmentImage(equippedItem.Icon);
            }
        }else{
            EquipmentImage.gameObject.SetActive(false);
            EquipmentBackImage.gameObject.SetActive(false);
        }
	}

	
	// Update is called once per frame
	void Update(){		
        //地面判定
        if(CheckGrounded()){
            Run();
            Jump();
            Attack();
            Collision();
        }
	}
 
    //地面判定
    private bool CheckGrounded(){
        //初期位置と向き
        var ray = new Ray(transform.position + Vector3.up * RayOffset , Vector3.down);
        //rayのHit判定
        //第一引数：飛ばすRay
        //第二引数：Rayの最大距離
        return Physics.Raycast(ray,Tolerance);
    }

    //敵の攻撃にヒットしているか
    private void Collision(){
        //初期位置と向き
        var ray = new Ray(transform.position + Vector3.up * RayOffset , Vector3.down);
        //rayのHit判定
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Tolerance)){
            // 攻撃範囲が消える瞬間にHitしているか確認
            if (hit.collider.gameObject.tag == "AttackField" && FieldManager.GetisCollision()){
                // まだHitしていない場合のみログを表示
                if (!isHit && !isAvoid){
                    Debug.Log("Hit");
                    isHit = true;
                    //攻撃が当たったらノックバクする
                    //すべてのコルーチンを停止
                    //例外処理
                    try{
                        StopAllCoroutines();
                    }catch(System.ArgumentNullException e) {
                        Debug.Log($"No coroutines to stop: {e.Message}");
                    }
                    StartCoroutine(KnockBack());
                    StartCoroutine(HP.TakeDamage(Damage));
                    //攻撃途中にノックバックすると判定が一回消えてしまうので
                    //攻撃フラグオフ
                    isAttack = false;
                    //攻撃判定ON
                    JudgeAttack.SetisEnter(false);
                //攻撃を回避したら
                }else if(!isHit && isAvoid){
                    isHit = true;
                    isAvoid = false;
                    StartCoroutine(SceneColorChangeBlue());
                }
            }
        }else{
            //Rayがヒットしなくなったらフラグをリセット
            isHit = false;
        }
        //次の判定のためにリセット
        if(!FieldManager.GetisCollision()){
            isHit = false;
        }
    }

    //移動
     private void Run(){
        //ジャンプ・攻撃中は動かない
        if(isJumping || isAttack || !FieldManager.GetisCounting()){

        }else{
            // 入力に基づいて移動方向を取得
            float MoveX = Input.GetAxis("Horizontal");  // 横方向
            float MoveZ = Input.GetAxis("Vertical");    // 前後方向
        
            // キャラクターの向きに基づいて移動方向を決定
            MoveDirection = new Vector3(MoveX, 0, MoveZ);
            MoveDirection *= Speed; // 移動速度を掛ける
            if (MoveDirection != Vector3.zero) {
                transform.rotation = Quaternion.LookRotation(MoveDirection);
                Animatornum = 1;
            } else {
                Animatornum = 0;
            }
            //走りアニメーションの変更
            if(Animatornum==1){
                //Animator Controllerのパラメータを変更Run
                Animator.SetBool("Moving", true);
                Velocity+=0.02f;
                Animator.SetFloat("Velocity", Velocity);
            }else{
                //Animator Controllerのパラメータを変更Idle
                Animator.SetBool("Moving", false);
                Animator.SetFloat("Velocity", 0);
                Velocity = 0;
            }
            //移動の実行
            CharacterController.Move(MoveDirection * Time.deltaTime);
        
        }
    }


    //ジャンプ関数
    private void Jump(){
        // スペースキーが押されたとき、ジャンプ可能なら処理を開始
        if (Input.GetKeyDown("space") && !isJumping && !Animator.GetBool("Attack") && FieldManager.GetisCounting()){
            StartCoroutine(Jumping());
        }
    }
    private IEnumerator Jumping(){
        //ジャンプフラグオン
        isJumping = true;
        //今の移動方向を取得
        Vector3 JumpDirection = MoveDirection;
        //ジャンプ中はRUNモーションにならないように
        Animator.SetBool("Jump", true);
        //アニメーション
        Animator.Play("Jumping", 0, 0);
        //音の再生
        AvoidSound.Play();
        //回避中に無敵を付与
        StartCoroutine(AssagnisAvoidTime());

        //ジャンプ移動処理
        float timer = 0f;
        while (timer < JumpMoveTimer){
            CharacterController.Move(JumpDirection * Time.deltaTime * JumpDistance);
            timer += Time.deltaTime;
            //次のフレームまで待機
            yield return null;  
        }

        //残りのクールタイム待機
        yield return new WaitForSeconds(JumpWaitTimer - JumpMoveTimer);
        //ジャンプ終了
        Animator.SetBool("Jump", false);
        //ジャンプフラグオフ
        isJumping = false;
    }

    private IEnumerator AssagnisAvoidTime(){
        yield return new WaitForSeconds(DelayAssignisAvoidTime);
        isAvoid = true;
        yield return new WaitForSeconds(isAvoidTime);
        isAvoid = false;
    }

    //回避すると画面を青くする
    private IEnumerator SceneColorChangeBlue(){
        //青色
        SceneImage.color = new Color(0, 0.9f, 1.0f, SceneAlpha);
        yield return new WaitForSeconds(SceneChangeTime);
        //透明にする
        SceneImage.color = new Color(0, 0, 0, 0.0f);
    }

    //攻撃関数
    private void Attack(){
        // スペースキーが押されたとき、ジャンプ可能なら処理を開始
        if (Input.GetMouseButtonDown(0) && !isJumping && !isAttack && FieldManager.GetisCounting()){
            StartCoroutine(Attacking());
        }
    }
    private IEnumerator Attacking(){
        //音の再生
        BeforeAttackSound.Play();
        //攻撃フラグオン
        isAttack = true;
        //攻撃中はRUNモーションにならないように
        Animator.SetBool("Attack", true);
        yield return new WaitForSeconds(FirstAttackDelay);
        //アニメーション
        Animator.Play("Attack", 0, 0);
        //残りのクールタイム待機
        yield return new WaitForSeconds(LastAttackDelay);
        //攻撃終了
        Animator.SetBool("Attack", false);
        //攻撃フラグオフ
        isAttack = false;
        //攻撃判定ON
        JudgeAttack.SetisEnter(false);
    }

    // ノックバック処理
    private IEnumerator KnockBack(){
        //全て中断
        isJumping = true;
        isAttack = true;

        //ノックバックする時はIdleにする
        Animator.Play("Idle", 0, 0);
        Animator.SetBool("Moving", false);
        Velocity = 0;

        // カメラシェイク開始
        StartCoroutine(CameraShake(CameraShakeDuration, CameraShakeIntensity));

        //後方へのノックバック処理
        //ノックバック距離
        Vector3 knockbackDistance =  -transform.forward * KnockBackDistance; 
        
        //ノックバック時間
        float timer = 0f;
        while (timer < KnockbackTime){
            CharacterController.Move(knockbackDistance);
            timer += Time.deltaTime;
            //次のフレームまで待機
            yield return null;             
        }

        //敵の攻撃がヒットした後の硬直
        yield return new WaitForSeconds(StunTime);

        //操作を再開
        Animator.SetBool("Jump", false);
        Animator.SetBool("Attack", false);
        isJumping = false;
        isAttack = false;
    }

    //プレイヤーの攻撃判定をするための関数
    public bool GetisAttack(){
        return isAttack;
    }

    //プレイヤーの攻撃力をJudgeAttackに与えるため
    public float GetAttackPower(){
        return AttackPower;
    }
 
    // カメラを揺らす処理(時間　強さ)
    private IEnumerator CameraShake(float duration, float magnitude){
        // カメラの初期位置を保存
        Transform cameraTransform = Camera.main.transform;
        Vector3 originalPosition = cameraTransform.localPosition;

        float timer = 0.0f;
        while (timer < duration){
            // カメラのランダムな揺れを生成
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            cameraTransform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            timer += Time.deltaTime;

            // 次のフレームまで待機
            yield return null;
        }
        // カメラの位置を元に戻す
        cameraTransform.localPosition = originalPosition;
    }

    //装備の表示
    private void SetEquipmentImage(Sprite EquipmentPicture){
        //Debug.Log(EquipmentPicture);
        EquipmentImage.sprite = EquipmentPicture;
    }
}