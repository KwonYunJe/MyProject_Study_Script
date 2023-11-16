using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Bullet : MonoBehaviour
{
    public GameManager gameManager;
    public float bulletDamage;
    public float dir;
    public float bulletSpeed;
    Rigidbody2D rigid;
    SpriteRenderer spRender;

    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
        Direction();
    }

    private void Update() {
        Move();
    }

    public void Move(){
        transform.Translate(Vector3.right * bulletSpeed * dir);
    }
    public void Direction(){
        if(dir ==  -1){
            //spRender.flipX = true;
            gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "groundMask"){         //탄막 제거
            Destroy(gameObject);       //게임 오브젝트 삭제
        }

        if(other.gameObject.tag == "Enemy"){                //적에게 닿을 시
            other.GetComponent<Enemy>().Damaged(bulletDamage);
            Destroy(gameObject);
        }
    }
}
