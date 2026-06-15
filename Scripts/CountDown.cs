using UnityEngine;
using UnityEngine.UI;
using TMPro;//TextMeshPro用
using System.Collections;

public class CountDown : MonoBehaviour
{
    [Header("参照オブジェクト")]
    [SerializeField] private TextMeshProUGUI CountDownText; //TextMeshProを使う場合はこれを有効化
    [SerializeField] private int CountDownTime = 3; //カウントダウンの秒数

    public IEnumerator StartCountDown(){
        int CurrentTime = CountDownTime;

        while (CurrentTime>0){
            //テキストに現在の時間を表示
            CountDownText.text = CurrentTime.ToString();
            //1秒待機
            yield return new WaitForSeconds(1.0f);
            //カウントを減らす
            CurrentTime--;
        }

        this.gameObject.SetActive(false);
    }
}
