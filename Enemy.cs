using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameManager gameManager;
    public Rigidbody2D rigid;
    SpriteRenderer sprender;
    public float hp;
    public float atkPower;

    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
        sprender = GetComponent<SpriteRenderer>();
    }

    public void Damaged(float weaponDmg){
        DamagedAnime();
        if(hp > weaponDmg){
            hp = hp-weaponDmg;
        }else{
            Destroy();
        }
    }

    private void DamagedAnime(){
        sprender.color = new Color(1, 1, 1, 1);
        Invoke("DamagedAnimeEnd",0.5f);
    }

    private void DamagedAnimeEnd(){
        sprender.color = new Color(78f / 255f, 174f / 255f ,255f / 255f,1);
    }

    public void Destroy(){
        gameObject.SetActive(false);
    }
}
