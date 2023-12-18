using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    // public static Player_Interaction playerInter;

    // private void Awake() {
    //     Debug.Log("This is pInteraction Awake");
    //     if(Player_Interaction.playerInter == null){
    //         Debug.Log("pInter instance start");
    //         Player_Interaction.playerInter = this;
    //         Debug.Log("pInter instance end");
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damaged(Collision2D other){     //적에게 맞았을 때
        Debug.Log("Damaged!!");
        SpriteRenderer sprender = GetComponent<Player>().sprender;
        float def = GetComponent<Player>().def;
        float curHP = GetComponent<Player>().curHP;
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
        GetComponent<Player>().gameObject.layer = 3;
        GetComponent<Player>().sprender.color = new Color(0,0,0);
    }

    public void GetExp(float expByGM){      //경험치 획득
        Debug.Log("Sum exp");
        GetComponent<Player>().curExp = GetComponent<Player>().curExp + expByGM;
        if(GetComponent<Player>().curExp >= GetComponent<Player>().maxExp){
            GetComponent<Player>().curExp = GetComponent<Player>().curExp - GetComponent<Player>().maxExp;                                           //경험치 초과분
            GetComponent<Player>().maxExp = GetComponent<Player>().maxExp + GetComponent<Player>().maxExp * 0.3f;                                    //최대 경험치 갱신
            GameManager.gmInstance.playerMaxExp = GetComponent<Player>().maxExp;                                  //gm의 최대 경험치 갱신
            GetComponent<Player>().playerLevel++;
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
