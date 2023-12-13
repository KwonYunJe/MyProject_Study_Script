using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    public static Player_Interaction playerInter;

    private void Awake() {
        if(Player_Interaction.playerInter == null){
            Player_Interaction.playerInter = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damaged(Collision2D other){     //적에게 맞았을 때
        SpriteRenderer sprender = Player.playerInstance.sprender;
        float def = Player.playerInstance.def;
        float curHP = Player.playerInstance.curHP;
        float damage = GameManager.gmInstance.DamagedFromEnemy(other);
        gameObject.layer = 9;
        sprender.color = new Color( 120/255f , 120/255f, 120/255f);
        if(damage <= def / 15){
            curHP = curHP - 0;
        }else{
            if(curHP > curHP - (damage - def / 15)){
                curHP = curHP - (damage - def / 15);//플레이어 체력감소
                GameManager.gmInstance.playerDamagedHP = curHP;//감소된 체력을 gamemanger로 전달
            }else{
                //Destroy
            }
        }
        Invoke("OffDamaged", 0.5f); 
    }

    void OffDamaged(){
        Player.playerInstance.gameObject.layer = 3;
        Player.playerInstance.sprender.color = new Color(0,0,0);
    }

    public void GetExp(float exp){      //경험치 획득 - GM에 의해 호출된다. 
        Debug.Log("Sum exp");
        Player.playerInstance.curExp = Player.playerInstance.curExp + exp;
        if(Player.playerInstance.curExp >= Player.playerInstance.maxExp){
            Player.playerInstance.curExp = Player.playerInstance.curExp - Player.playerInstance.maxExp;                                           //경험치 초과분
            Player.playerInstance.maxExp = Player.playerInstance.maxExp + Player.playerInstance.maxExp * 0.3f;                                    //최대 경험치 갱신
            GameManager.gmInstance.playerMaxExp = Player.playerInstance.maxExp;                                  //gm의 최대 경험치 갱신
            Player.playerInstance.playerLevel++;
        }
    }

    public void GetCoin(Collision2D other){    //코인 획득 시
        if(other.gameObject.name == "GoldCoin(Clone)"){
                GameManager.gmInstance.GetGoldCoin();
            }else if(other.gameObject.name == "SilverCoin(Clone)"){
                GameManager.gmInstance.GetSilverCoin();
            }
        Destroy(other.gameObject);
    }
}
