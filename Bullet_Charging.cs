using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Bullet_Charging : MonoBehaviour
{
    public GameManager gameManager;
    public float bulletDamage;
    public float dir;
    public float firstBulletHeight;
    public float i;
    public float lastBulletHeight;
    public float charging;
    public bool endAtk;
    Rigidbody2D rigid;
    SpriteRenderer spRender;
    BoxCollider2D boxCol;

    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        Invoke("EndAtkStart", 1.5f);
        RigidBodyBlink();
        transform.localScale = new Vector2(transform.localScale.x, charging);
        firstBulletHeight = transform.localScale.y;
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
        Invoke("RigidBodyBlink", 0.2f);
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////Destroy

    void EndAtkStart(){
        endAtk = true;
    }

    void DestroyAnim(){
        if(endAtk == true){
            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - 0.05f);
            if(transform.localScale.y < 0){ 
                Destroy();
            }
        }
    }

    private void Destroy(){
        Destroy(gameObject);
    }
}
