using System.Collections.Generic;
using UnityEngine;

public class CharacterDatabase : MonoBehaviour
{
    private static Dictionary<string, CharacterData> characterDict = new Dictionary<string, CharacterData>();

    [SerializeField] private List<CharacterData> characterList;
    [SerializeField] private GameObject Player;
    private GameObject currentCharacter;//現在のキャラクター

    private void Awake(){
        //Dictionaryにキャラクターデータを登録
        foreach (CharacterData character in characterList){
            characterDict[character.characterName] = character;
        }

        //選択キャラクターの名前を取得
        string selectedCharacterName = PlayerPrefs.GetString("SelectedChar", "");
        //データの取得
        CharacterData selectedCharacter = GetCharacterByName(selectedCharacterName);
        if (selectedCharacter != null){
            ChangingCharacter(selectedCharacter);
        }
    }

    public static CharacterData GetCharacterByName(string name){
        if (characterDict.TryGetValue(name, out CharacterData character)){
            return character;
        }
        Debug.Log($"キャラクター {name} が見つかりません");
        return null;
    }

    //キャラ変更
    public void ChangingCharacter(CharacterData newCharacter){
        //前回のkyラクター削除
        if (currentCharacter != null){
            Destroy(currentCharacter);
        }
        Debug.Log(newCharacter.characterPrefab);
        currentCharacter = Instantiate(newCharacter.characterPrefab, new Vector3(0, 0, 0), transform.rotation);
        currentCharacter.transform.parent = Player.transform;
        PlayerPrefs.SetString("SelectedCharacter", newCharacter.characterName);
    }
}
