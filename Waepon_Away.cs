using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waepon_Away : MonoBehaviour
{
    public static Waepon_Away waInstance;  //싱글톤 인스턴스

    private void Awake() {
        if(Waepon_Away.waInstance == null){        //인스턴스가 생성되어 있지 않다면 
            Waepon_Away.waInstance = this;         //이 스크립트를 대상으로 인스턴스를 생성한다
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

    public void AttackShoot(GameObject bullet, float face, float weaponDmgAway){         //원거리 공격
        GameObject shootBullet = Instantiate(bullet, transform.position, transform.rotation);   //탄막 생성
        shootBullet.GetComponent<Bullet>().dir = face;      //탄막 방향
        shootBullet.GetComponent<Bullet>().bulletDamage = weaponDmgAway;    //탄막 데미지
    }

    public void AttackChargingShoot(float curMP, float chargingCost, float face, GameObject chargingBullet, float charging, float weaponDmgAway, float chargingAtkTime){
        Debug.Log("ChargShoot!");
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
    }
}
