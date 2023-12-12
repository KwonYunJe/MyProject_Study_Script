using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Close : MonoBehaviour
{
    public static Weapon_Close wcInstance;  //싱글톤 인스턴스
    public Player player;
    public GameManager gameManager;
    public GameObject ATKAreaView;

    private void Awake() {
        if(Weapon_Close.wcInstance == null){        //인스턴스가 생성되어 있지 않다면 
            Weapon_Close.wcInstance = this;         //이 스크립트를 대상으로 인스턴스를 생성한다
        }    
    }
    // Start is called before the first frame update
    void Start()
    {
        ATKAreaView = player.ATKAreaView;
        // atkPos = player.atkPos;
        // atkBoxSize = player.atkBoxSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackNear(Transform atkPos, Vector2 atkBoxSize, float weaponDmgClose){          //공격
        AttackingAnime();   //공격 애니메이션
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(atkPos.position, atkBoxSize, 0); //OverlapBoxAll(position, size, angle) 배열을 생성하고 감지된 오브젝트를 모두 담는다
        foreach(Collider2D collider in collider2Ds){                                        
            if(collider.tag == "Enemy"){
                gameManager.AtkToEnemy(weaponDmgClose, collider);    //감지된 적과 공격력을 매개변수로 gameManager의 AtkToEnemy함수를 호출
                
            }else if(collider.tag != "Enemy"){
                
            }
        }
    }

    public void DebugLog(){
        Debug.Log("This is Close Attack Massage");
    }

    void AttackingAnime(){          //공격 애니메이션 시작
        ATKAreaView.SetActive(true);
        Invoke("AttackEndAnime", 0.4f);
    }

    void AttackEndAnime(){          //공격 애니메이션 종료
        ATKAreaView.SetActive(false);
    }
}
