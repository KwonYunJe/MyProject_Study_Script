using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Attack : MonoBehaviour
{
    public static Weapon_Attack waInstance;  //싱글톤 인스턴스

    private void Awake() {
        if(Weapon_Attack.waInstance == null){        //인스턴스가 생성되어 있지 않다면 
            Weapon_Attack.waInstance = this;         //이 스크립트를 대상으로 인스턴스를 생성한다
        }    
    }
    // Start is called before the first frame update
    void Start()
    {

        // atkPos = player.atkPos;
        // atkBoxSize = player.atkBoxSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        public void Attack(){

        Rigidbody2D rigid = Player.playerInstance.rigid;
        Transform atkPos = Player.playerInstance.atkPos;
        Vector2 atkBoxSize = Player.playerInstance.atkBoxSize;
        GameObject bullet = Player.playerInstance.bullet;
        GameObject chargingBullet = Player.playerInstance.chargingBullet;
        bool isCharginAtk = Player.playerInstance.isCharginAtk;
        bool failCharging = Player.playerInstance.failCharging;
        float face = Player.playerInstance.face;
        float attackRange = Player.playerInstance.attackRange;
        float atkCurTime = Player.playerInstance.atkCurTime;
        float atkMaxTime = Player.playerInstance.atkMaxTime;       
        float charging = Player.playerInstance.charging;       
        float weaponDmgClose = Player.playerInstance.weaponDmgClose;
        float weaponDmgAway = Player.playerInstance.weaponDmgAway;        
        float curMP = Player.playerInstance.curMP;
        float chargingCost = Player.playerInstance.chargingCost;        
        float chargingAtkTime = Player.playerInstance.chargingAtkTime;

        //Range
        RaycastHit2D closeEnemy = Physics2D.Raycast(rigid.position, Vector2.right * face, attackRange, LayerMask.GetMask("Enemy"));
        Debug.DrawRay(rigid.position, Vector2.right * face * attackRange, Color.magenta);

        if(Input.GetKeyUp("z") && atkCurTime > atkMaxTime && !isCharginAtk){
            if(charging < 3 || failCharging){
                Debug.Log("일반공격 감지 : " + charging);
                if(closeEnemy){
                    //인스턴스 생성, 함수에 인자 전달(아래 원거리도 동일 적용)
                    Weapon_Attack.waInstance.AttackNear(atkPos, atkBoxSize, weaponDmgClose);
                }else{
                    if(failCharging == false){
                        Weapon_Attack.waInstance.AttackShoot(bullet, face, weaponDmgAway);
                    }else{
                        failCharging = false;
                    }
                }
                atkCurTime = 0;
                charging = 0;
            }else{
                Debug.Log("차징샷 감지 : " + charging);
                Weapon_Attack.waInstance.AttackChargingShoot(curMP, chargingCost, face, chargingBullet, charging, weaponDmgAway, chargingAtkTime);
            }
        }

    }

    public void AttackTime(){      //공격시간 연산
        Player.playerInstance.atkCurTime += Time.deltaTime;
    }

    //근거리 공격

    public void AttackNear(Transform atkPos, Vector2 atkBoxSize, float weaponDmgClose){          //공격
        AttackingAnime();   //공격 애니메이션
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(atkPos.position, atkBoxSize, 0); //OverlapBoxAll(position, size, angle) 배열을 생성하고 감지된 오브젝트를 모두 담는다
        foreach(Collider2D collider in collider2Ds){                                        
            if(collider.tag == "Enemy"){
                GameManager.gmInstance.AtkToEnemy(weaponDmgClose, collider);    //감지된 적과 공격력을 매개변수로 gameManager의 AtkToEnemy함수를 호출
                
            }else if(collider.tag != "Enemy"){
                
            }
        }
    }


    void AttackingAnime(){          //공격 애니메이션 시작
        Player.playerInstance.ATKAreaView.SetActive(true);
        Invoke("AttackEndAnime", 0.4f);
    }

    void AttackEndAnime(){          //공격 애니메이션 종료
        Player.playerInstance.ATKAreaView.SetActive(false);
    }


    //원거리 공격
    public void AttackShoot(GameObject bullet, float face, float weaponDmgAway){         //원거리 공격
        GameObject shootBullet = Instantiate(bullet, transform.position, transform.rotation);   //탄막 생성
        shootBullet.GetComponent<Bullet>().dir = face;      //탄막 방향
        shootBullet.GetComponent<Bullet>().bulletDamage = weaponDmgAway;    //탄막 데미지
    }

    public void AttackChargingShoot(float curMP, float chargingCost, float face, GameObject chargingBullet, float charging, float weaponDmgAway, float chargingAtkTime){
        Debug.Log("ChargShoot!");
        Player.playerInstance.isCharging = false;     //충전 상태 off
        Player.playerInstance.isCharginAtk = true;    //공격 상태 on
        curMP = curMP - chargingCost;
        float bulletPositionX = transform.position.x + face * chargingBullet.transform.localScale.x/2 ; //샷 생성 위치 조정
        GameObject chargingShootBullet = Instantiate(chargingBullet, new Vector2(bulletPositionX, transform.position.y), transform.rotation);   //탄막 생성
        chargingShootBullet.GetComponent<Bullet_Charging>().charging = charging * 0.05f;            //차징샷 크기 
        chargingShootBullet.GetComponent<Bullet_Charging>().bulletDamage = weaponDmgAway;           //탄막 데미지
        chargingShootBullet.GetComponent<Bullet_Charging>().chargingAtkTime = chargingAtkTime;      //탄막 유지시간
        Invoke("EndAttackChargingShoot", chargingAtkTime);                                          //탄막 유지시간이 다 되면 공격상태 off
    }

    public void EndAttackChargingShoot(){
        Player.playerInstance.isCharginAtk = false;
        Player.playerInstance.gameManager.playerCurCharging = 0;
        Player.playerInstance.atkCurTime = 0;
        Player.playerInstance.charging = 0;
    }

    public void DetectCharging(){
        if(Input.GetKey("z") && ! Player.playerInstance.isCharginAtk){
            Player.playerInstance.charging = Player.playerInstance.charging + 0.05f;
            if(Player.playerInstance.charging >= 3){
                if(Player.playerInstance.curMP - Player.playerInstance.chargingCost >= 0){          //마나가 충분할 때
                    Player.playerInstance.isCharging = true;  //충전 상태 on
                    if(Player.playerInstance.charging > Player.playerInstance.chargingMax){
                        Player.playerInstance.charging = Player.playerInstance.chargingMax;
                    }
                }else if(Player.playerInstance.failCharging == false && Player.playerInstance.curMP - Player.playerInstance.chargingCost < 0){  //공격에 필요한 마나가 부족할 때 
                    Player.playerInstance.failCharging = true;
                    Player.playerInstance.charging = 0;
                    Debug.Log("Not enough MP!");
                    GameManager.gmInstance.NotEnoughMP();
                }
            }
           //Debug.Log(charging);
        }
    }
}
