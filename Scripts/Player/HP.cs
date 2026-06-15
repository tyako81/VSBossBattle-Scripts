using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HP : MonoBehaviour
{
    [Header("参照オブジェクト")]
    [SerializeField] Slider hpSlider;//スライダーの参照
    [SerializeField] Slider hpSlider_R;//赤色のスライダーの参照
    [SerializeField] string MyName;//キャラクターによってHP0になった時の判定
    [SerializeField] GameObject BackTitle;//タイトルに戻るためのブロック
    [SerializeField] GameObject ClearUI;//クリアーした時に表示するもの
    [SerializeField] GameObject LoseUI;//負けた時に表示するもの
    [SerializeField] GameObject BossHP;//クリアしたら敵のHP表示を削除
    [SerializeField] FieldManager fieldmanager;//攻撃範囲出現の停止
    [SerializeField] Enemy Enemy;//０になったら敵が沈む関数を持ってくる
    [SerializeField] BackTitle BackTitle_Fun;//フェードアウトする関数を持ってくる
    [SerializeField] Image SceneImage;//画面全体のシーン
    [SerializeField] AttackSoundManager AttackSoundManager;//攻撃音を持ってくる関数
    [SerializeField] AudioSource DamageSound;//攻撃された時の音
    [SerializeField] AudioSource LoseSound;//負けの音
    [SerializeField] AudioSource ClearSound;//勝ちの音
    [SerializeField] AudioSource BGM;//BGM


    private static string SelectedCharName;//選択キャラクター
    private static CharacterData selectedChar;//キャラクターデータ
    private const float maxBossHp = 15.0f;//ボスのHP
    private bool isDead = false;//倒れたかどうか
    private float maxHp;//それぞれのHP
    private float CurrentHP;//今のHP
    private const float SceneAlpha = 0.7f;//画面エフェクトの透明度
    private const float RedEffectDuration = 0.2f;//赤色のエフェクト時間
    private const float WhiteEffectDuration = 0.15f;//白色のエフェクト時間
    private const float HpSliderDecreaseStep = 0.01f;//HPスライダーの減少量
    private static AudioSource MyAttackSound;//攻撃音
    private static float AttackSoundDelay;//攻撃音がするまでの時間


    void Start(){
        // 初期設定
        BossHP.SetActive(true);
        ClearUI.SetActive(false);
        LoseUI.SetActive(false);
        if(MyName=="Player"){
            //HPの取得
            SelectedCharName = PlayerPrefs.GetString("SelectedChar", "");
            selectedChar = CharacterDatabase.GetCharacterByName(SelectedCharName);
            maxHp = selectedChar.Health;
            MyAttackSound = AttackSoundManager.GetAttackSound();
            AttackSoundDelay = selectedChar.AttackSoundDelay;
        }else if(MyName=="Boss"){
            //HPの取得
            maxHp = maxBossHp; 
        }
        //HPを最大値に設定
        CurrentHP = maxHp;
        //スライダーの最大値を設定
        hpSlider.maxValue = maxHp; 
        hpSlider_R.maxValue = maxHp;
        // 現在のHPを反映
        hpSlider.value = CurrentHP;
        hpSlider_R.value = CurrentHP;
        //画面全体の色の初期設定
        SceneImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    public IEnumerator TakeDamage(float damage){
        if(!isDead){
            if(MyName=="Boss"){
                //攻撃音の再生
                StartCoroutine(PlaySound(MyAttackSound, AttackSoundDelay));
            }else if(MyName=="Player"){
                //ダメージ音の再生
                DamageSound.Play();
            }
            // HPを減らす処理
            CurrentHP -= damage;
            if (CurrentHP < 0){
                CurrentHP = 0;
                StopAllCoroutines();
            }

            StartCoroutine(ChangeSceneColor());

            // スライダーに現在のHPを反映
            hpSlider.value = CurrentHP;
            double RedDamageEffectTime = 0;
            while(damage > RedDamageEffectTime){
                hpSlider_R.value -= HpSliderDecreaseStep;
                RedDamageEffectTime += HpSliderDecreaseStep;
                yield return null;
            }
        

            //HPが０になったら
            if(hpSlider.value==0 && !isDead){
                isDead = true;
                //BGMの停止
                BGM.Stop();
                //敵を倒したら出口の表示
                if(MyName=="Boss"){
                    //クリア画面の表示
                    ClearUI.SetActive(true);
                    //音の再生
                    ClearSound.Play();
                    //敵のHPの非表示
                    BossHP.SetActive(false);
                    //敵が沈む
                    StartCoroutine(Enemy.DefeteEnemy());
                    //音のループの停止
                    fieldmanager.SetChargeSoundloop(false);
                    //倒したらコルーチンを停止させて敵を消滅
                    BackTitle.SetActive(true);
                //自分がやられたら強制タイトルへ
                }else if(MyName=="Player"){
                    //攻撃範囲表示の停止
                    fieldmanager.SetgetRandom(false);
                    //負けの表示
                    LoseUI.SetActive(true);
                    //音の再生
                    LoseSound.Play();
                    //フェード開始
                    StartCoroutine(BackTitle_Fun.LoseFadeScene());
                }else{
                    //例外処理
                }
            }
        }
        
    }

    //ダメージを受けると画面の色を変える
    private IEnumerator ChangeSceneColor(){
        if(MyName=="Boss"){
            //ダメージを与えたら画面を白くする
            SceneImage.color = new Color(1.0f, 1.0f, 1.0f, SceneAlpha);
            yield return new WaitForSeconds(WhiteEffectDuration);
            SceneImage.color = new Color(0, 0, 0, 0.0f);
        }else if(MyName=="Player"){
            //ダメージを受けたら画面を赤くする
            SceneImage.color = new Color(1.0f, 0, 0, SceneAlpha);
            yield return new WaitForSeconds(RedEffectDuration);
            SceneImage.color = new Color(0, 0, 0, 0.0f);
        }
    }

    //音を再生する関数
    private IEnumerator PlaySound(AudioSource AttackSound, float duration){
        //キャラごとによって再生時間を調整
        yield return new WaitForSeconds(duration);
        AttackSound.Play();
    }
}
