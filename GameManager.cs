using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject coinBox;
    public GameObject Enemy;
    public GameObject uiController;
    public int score;
    public float playerMaxHP;
    public float playerCurHP;
    public float playerDamagedHP;
    public float playerMaxCharging;
    public float playerCurCharging;
    public float playerMaxMP;
    public float playerCurMP;
    public float playerMaxExp;
    public float playerCurExp;
    public float expByEnemy;



    private void Update() {
        SetUICont();
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


    //UI 컨트롤//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void SetUICont(){
        SetUICont_HP();
        SetUICont_MP();
        SetUICont_Charging();
        SetUICont_Exp();
    }
    public void SetUICont_HP(){
        uiController.GetComponent<UIController>().playerMaxHP = playerMaxHP;
        uiController.GetComponent<UIController>().playerCurHP = playerCurHP;
    }
    public void SetUICont_MP(){
        uiController.GetComponent<UIController>().playerMaxMP = playerMaxMP;
        uiController.GetComponent<UIController>().playerCurMP = playerCurMP;
    }
    public void SetUICont_Charging(){
        uiController.GetComponent<UIController>().playerMaxCharging = playerMaxCharging;
        uiController.GetComponent<UIController>().playerCurCharging = playerCurCharging;
    }
    public void SetUICont_Exp(){
        uiController.GetComponent<UIController>().playerMaxExp = playerMaxExp;
        uiController.GetComponent<UIController>().playerCurExp = playerCurExp;
    }
    public void NotEnoughMP(){
        uiController.GetComponent<UIController>().NotEnoughMPAni();
    }


    //적 사망시 이벤트////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void EnemyDeadEvent(GameObject enemy){
        ExpToPlayer(enemy.GetComponent<Enemy>().enemyLevel);
    }
        public void ExpToPlayer(float exp){
        player.GetComponent<Player>().GetExp(exp);
    }
}
