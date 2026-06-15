using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackTitle : MonoBehaviour
{
    [Header("参照オブジェクト")]
    [SerializeField] Slider hpSlider; //敵のスライダーの参照
    [SerializeField] GameObject hpSlider_P; //プレイヤーのスライダーの参照
    [SerializeField] Image WinfadeImage; //フェード用のUIイメージを設定
    [SerializeField] Image LosefadeImage; //フェード用のUIイメージを設定
    [SerializeField] GameObject ClearUI;//Clearの削除

    private bool isFading = false; //フェード中の判定
    private const float FadeDuration = 1.5f;  //フェードにかかる時間
    private const float WinWaitTime = 0.5f;  //勝利後の待機時間
    private const float LoseWaitTime = 2.0f; //敗北後の待機時間
    private static readonly Color WinFadeColor = Color.white; //勝利時のフェード色
    private static readonly Color LoseFadeColor = Color.black; //敗北時のフェード色

    void Start(){
        //敵を倒していない最初は非アクティブ
        if (hpSlider.value != 0){
            this.gameObject.SetActive(false);
        }
    }

    //接触したらタイトルへ
    private void OnTriggerEnter(Collider other){
        //一回だけ実行
        if(!isFading){
            StartCoroutine(WinFadeScene());
        }
    }

    //勝った時のフェード
    public IEnumerator WinFadeScene(){
        //HPを非表示にする
        hpSlider_P.SetActive(false);
        //クリアの非表示
        ClearUI.SetActive(false);

        //判定の削除
        isFading = true;

        //フェード開始
        float timer = 0f;
        while (timer < FadeDuration){
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / FadeDuration);
            WinfadeImage.color = new Color(WinFadeColor.r, WinFadeColor.g, WinFadeColor.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(WinWaitTime);
        SceneManager.LoadScene("Title");
    }

    //負けた時のフェード
    public IEnumerator LoseFadeScene(){
        //HPを非表示にする
        hpSlider_P.SetActive(false);

        //フェード開始
        float timer = 0f;
        while (timer < FadeDuration){
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / FadeDuration);
            LosefadeImage.color = new Color(LoseFadeColor.r, LoseFadeColor.g, LoseFadeColor.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(LoseWaitTime);
        SceneManager.LoadScene("Title");
    }
}
