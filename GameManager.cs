using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject coinBox;
    public GameObject Enemy;
    public int score;

    private void Update() {
        
    }

    public void GetSilverCoin(){
        score = score + 10;
        Debug.Log("Get SilverCoin");
    }

    public void GetGoldCoin(){
        score = score + 30;
        Debug.Log("Get GoldCoin");
    }

    public void AtkToEnemy(float weaponDmg, Collider2D enemy){  //Player로 부터 공격력과 감지된 적의 collider를 매개변수로 받음
        enemy.GetComponent<Enemy>().Damaged(weaponDmg);         //공격력을 매개변수로하여 감지된 적의 Damaged를 호출
    }

    public float DamagedFromEnemy(Collision2D enemy){
        return enemy.collider.GetComponent<Enemy>().atkPower;
    }
}
