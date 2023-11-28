using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Bullet_Charging : MonoBehaviour
{
    public GameManager gameManager;
    public float bulletDamage;  //데미지
    public float dir;           //방향
    public float bulletHeight;  //공격 크기(y축)
    public float charging;      //충전 정도 - 플레이어로부터 받는다
    public bool endAtk;         //공격 종료 애니메이션을 시작시키는 트리거
    public float chargingAtkTime;   //차징공격 지속 시간
    public float blinkTime;     //공격 연타 간격(시간)     
    
    Rigidbody2D rigid;
    SpriteRenderer spRender;
    BoxCollider2D boxCol;

    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        Invoke("EndAtkStart", chargingAtkTime);
        RigidBodyBlink();
        transform.localScale = new Vector2(transform.localScale.x, charging);
        bulletHeight = transform.localScale.y;
    }

    private void Update() {
        DestroyAnim();
    }   
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Enemy"){                //적에게 닿을 시
            other.GetComponent<Enemy>().Damaged(bulletDamage);
        }
    }

    void RigidBodyBlink(){
        if(boxCol.enabled == true){
            boxCol.enabled = false;
        }else{
            boxCol.enabled = true;
        }
        Invoke("RigidBodyBlink", blinkTime);
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////Destroy

    void EndAtkStart(){
        endAtk = true;
    }

    void DestroyAnim(){
        if(endAtk == true){
            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - 0.05f);
            if(transform.localScale.y <= 0){ 
                Destroy();
            }
        }
    }

    private void Destroy(){
        Destroy(gameObject);
    }
}
