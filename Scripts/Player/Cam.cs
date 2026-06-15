using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour{

    [Header("参照オブジェクト")]
    [SerializeField] GameObject Player;
    
    private static Vector3 Position;
    private static Vector3 CamPos = Vector3.zero;

    // Start is called before the first frame update
    void Start(){
        //カメラの位置を取得
        CamPos.z = this.gameObject.transform.position.z;
        CamPos.y = this.gameObject.transform.position.y;
        //プレイヤーにカメラを追尾
        Position.x = Player.transform.position.x;
        Position.z = Player.transform.position.z + CamPos.z;
        Position.y = CamPos.y;
    }

    // Update is called once per frame
    void Update(){
        //常に追尾
        Position.x = Player.transform.position.x;
        Position.z = Player.transform.position.z + CamPos.z;
        Position.y = CamPos.y;
        
        this.transform.position = Position;
    }
}
