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
    Rigidbody2D rigid;
    SpriteRenderer spRender;

    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("Destroy", 1.5f);
        transform.localScale = new Vector2(transform.localScale.x, charging);
        firstBulletHeight = transform.localScale.y;
    }

    private void Update() {
        anim();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {

        if(other.gameObject.tag == "Enemy"){                //적에게 닿을 시
            other.GetComponent<Enemy>().Damaged(bulletDamage);
        }
    }

    void anim(){
        transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - i * 0.1f);
        if(lastBulletHeight > transform.localScale.y - i || firstBulletHeight < transform.localScale.y - i){ 
            i = -i;
        }
    }

    private void Destroy(){
        Destroy(gameObject);
    }
}
