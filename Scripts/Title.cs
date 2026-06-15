using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [Header("参照オブジェクト")]
    [SerializeField] GameObject HowToImage;//遊び方1ページ目
    [SerializeField] GameObject HowToImage2;//遊び方2ページ目
    [SerializeField] GameObject HowToImage3;//遊び方3ページ目
    [SerializeField] GameObject SelectItemImage;//アイテムページ
    [SerializeField] GameObject SelectCharacterImage;//キャラクターページ
    [SerializeField] GameObject SelectItemBox;//アイテム選択Box
    [SerializeField] GameObject SelectCharacterBox;//キャラクター選択Box
    [SerializeField] AudioSource ClickSound;//クリック音
    
    private const float WaitForGameStart = 0.3f;//シーン移動待機時間
    private static Vector3 SelectItemBoxPos = new Vector3(0, 50, 0);//アイテム選択Boxの座標
    private static Vector3 SelectCharacterBoxPos = new Vector3(0, 50, 0);//キャラクター選択Boxの座標
    private const float CloakPosX = -650;//マントのX座標
    private const float BootsPosX = 0;//ブーツのX座標
    private const float SwordPosX = 650;//剣のX座標
    private const float BrutePosX = -650;//プルートのX座標
    private const float NinjaPosX = 0;//忍者のX座標
    private const float KaratePosX = 650;//空手のX座標
    private static bool isSelectItemBoxActive = false;//SelectItemBoxのアクティブ状態
    private static string EquippedItem = "";//装備アイテム
    private static string SelectedChar = "Ninja";//選択キャラクター＊初期は忍者

    void Start(){
        //初期化
        SelectItemImage.SetActive(false);
        SelectCharacterImage.SetActive(false);
        SelectItemBox.SetActive(isSelectItemBoxActive);
        HowToImage.SetActive(false);
        HowToImage2.SetActive(false);
        HowToImage3.SetActive(false);
        SelectCharacterBox.transform.localPosition = SelectCharacterBoxPos;

        if(SelectItemBox != null){
            //座標を復元
            SelectItemBox.transform.localPosition = SelectItemBoxPos;
        }
    }

    //クリックの音
    public void PlayClickSound(){
        ClickSound.Play();
    }

    //ゲームスタートボタン
    public void ClickStart(){
        StartCoroutine(ClickStartMethod());
    }
    //クリック音が鳴ってからシーン移動
    private IEnumerator ClickStartMethod(){
        PlayClickSound();
        //音が再生するまで待機
        yield return new WaitForSeconds(WaitForGameStart);
        //選んだ装備を保存
        PlayerPrefs.SetString("EquippedItem", EquippedItem);
        PlayerPrefs.SetString("SelectedChar", SelectedChar);
        SceneManager.LoadScene("Battle");
    }
    //アイテム選択画面
    public void ClikSelectItem(){
        SelectItemImage.SetActive(true);
    }
    //アイテム選択Boxの表示
    public void SelectItem(string Name){
        SelectItemBox.SetActive(true);
        isSelectItemBoxActive = true;
        EquippedItem = Name;
        switch(Name){
            //マントの装備
            case "Cloak":
                SelectItemBoxPos.x = CloakPosX;
                break;
            //ブーツの装備
            case "Boots":
                SelectItemBoxPos.x = BootsPosX;
                break;
            //剣の装備
            case "Sword":
                SelectItemBoxPos.x = SwordPosX;
                break;
        }
        SelectItemBox.gameObject.transform.localPosition = SelectItemBoxPos;
    }
    //装備なしをクリック
    public void ClickNoneEquipment(){
        SelectItemBox.SetActive(false);
        EquippedItem = " ";
    } 
    //キャラクター選択画面
    public void ClikSelectChar(){
        SelectCharacterImage.SetActive(true);
    }
    //キャラクター選択Boxの表示
    public void SelectChar(string Name){
        SelectCharacterBox.SetActive(true);
        SelectedChar = Name;
        switch(Name){
            //プルート
            case "Brute":
                SelectCharacterBoxPos.x = BrutePosX;
                break;
            //忍者
            case "Ninja":
                SelectCharacterBoxPos.x = NinjaPosX;
                break;
            //空手家
            case "Karate":
                SelectCharacterBoxPos.x = KaratePosX;
                break;
        }
        SelectCharacterBox.gameObject.transform.localPosition = SelectCharacterBoxPos;
    }
    //操作方法1ページ目表示
    public void ClickHowTo(){
        HowToImage.SetActive(true);
    }
    //アイテム欄からタイトルに戻る
    public void BackTitle_Item(){
        SelectItemImage.SetActive(false);
    }
    //キャラクター欄からタイトルに戻る
    public void BackTitle_Char(){
        SelectCharacterImage.SetActive(false);
    }
    //遊び方からタイトルに戻る
    public void BackTitle_How(){
        HowToImage.SetActive(false);
    }
    //操作方法2ページ目表示
    public void NextPage(){
        HowToImage2.SetActive(true);
    }
    //1ページ目に戻る
    public void BackPage(){
        HowToImage2.SetActive(false);
    }
    //操作方法3ページ目表示
    public void NextPage2(){
        HowToImage3.SetActive(true);
    }
    //2ページ目に戻る
    public void BackPage2(){
        HowToImage3.SetActive(false);
    }
}
