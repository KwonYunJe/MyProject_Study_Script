using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.XR;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public static Player playerInstance;    //싱글톤 인스턴스
    //public GameObject weapon_Close;
    //public Player_Interaction pInter = new Player_Interaction();
    //public Weapon_Attack wAttack = new Weapon_Attack();
    public Rigidbody2D rigid;
    public SpriteRenderer sprender;
    public Transform checkPos;
    public GameManager gameManager;
    public GameObject bullet;
    public GameObject chargingBullet;
    public GameObject ATKAreaView;
    public GameObject groundCheckBox1;
    //public GameObject groundCheckBox2;
    public GameObject Head;
    public GameObject Body;
    public GameObject Leg;
    public GameObject Face;
    public GameObject Psprite;
    SpriteRenderer pSprender;

    Collider2D thisCol;

    public float moveSpeed;      //움직임 속도   
    public float checkRadius;   //감지 반경
    public float distance;      //감지거리
    public float jumpPower;     //점프 파워
    public float dashPower;     //대쉬 파워
    public bool isDash;         //대쉬 중 방향 조작 불가
    public float dashDir;       //대쉬 방향
    public float maxHP;         //최대체력
    public float curHP;         //현재체력
    public float maxMP;         //최대마나
    public float curMP;         //현재마나
    public float def;           //방어력
    public float playerLevel;   //레벨
    public float maxExp;        //최대 경험치
    public float curExp;        //현재 경험치
    public float weaponDmgClose;     //무기 공격력(근접)
    public float weaponDmgAway;     //무기 공격력(원거리)
    public float attackRange;       //근접, 원거리 공격 여부를 결정하는 거리
    public float chargingMax;       //차징 최대치
    public float charging;          //차징 공격(게이지)
    public bool isCharging;         //차징 중
    public bool failCharging;       //차징 실패
    public bool isCharginAtk;       //차징 공격 중
    public float chargingAtkTime;   //차징공격 지속 시간
    public float chargingCost;      //차징공격 소모 마나

    public Vector2 perp;

    public bool isSlope;
    public bool isJump;
    public bool isGround1;
    public bool headSense;
    public bool bodySense;
    public bool legSense;

    private float inputX;
    private float inputY;
    private float angle;    //맞닿은 표면과 수직선과의 각도를 저장
    public float atkCurTime; //공격한 순간부터 시간이 저장됨
    public float atkMaxTime;  //다음 공격까지 공격을 할 수 없는 시간을 저장
    public Transform atkPos;    //공격이 실행될 위치
    public Vector2 atkBoxSize;  //공격이 실행될 크기
    public float face;    //공격 방향
    public float damageShock;   //피격시 튕겨나가는 힘

    public RaycastHit2D isGroundBox;

    private void Awake() {
    }


    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
        thisCol = GetComponent<Collider2D>();
        sprender = GetComponent<SpriteRenderer>();
        pSprender = Psprite.GetComponent<SpriteRenderer>();
        face = 1;
        FirstSend();
        curMP = maxMP;
    }

    private void Update() {
        FaceDir();
        GetComponent<Weapon_Attack>().Attack();
        GetComponent<Weapon_Attack>().AttackTime();
        Dash();
        DetectSlope();
        GetComponent<Weapon_Attack>().DetectCharging();
        GroundCheck1();
        OnOffDetectGround();
        FlipSP();
        SendGM();
        //BodySense();
    }

    private void FixedUpdate() {
        Move();
        Jump();
        
    }

    //rigidbody -> translate로 전환 필요
    //움직임/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       void Move(){
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////움직임 제어 조건
        if(isDash == true){     // 대쉬상태일 경우
            inputX = dashDir;   // 대쉬를 할 방향으로 고정
        }
        
        else if(isCharginAtk || isCharging){   //차징샷을 충전중이거나 공격을 실행중일 시
            inputX = 0;                         //좌우 입력을 무시한다.
        }
        
        else{                  // 대쉬상태가 아닐 경우
            inputX = Input.GetAxisRaw("Horizontal");    //수평계 입력을 방향으로 설정
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////슬로프 조건 
        if(isSlope && isGround1 && !isJump){       
            rigid.velocity = perp * moveSpeed * inputX * -1;
            //transform.Translate(perp * moveSpeed * inputX);
        }else if(!isSlope && isGround1){
            //rigid.velocity = new Vector2(inputX * moveSpeed, 0);
            //transform.Translate(new Vector2(inputX * moveSpeed,0));
        }else if(!isGround1){
            rigid.velocity = new Vector2(inputX * moveSpeed, rigid.velocity.y);
        }
        if(isJump){
            rigid.velocity = new Vector2(inputX * moveSpeed / 1.5f, rigid.velocity.y);
            //transform.Translate(Vector2.right * inputX * moveSpeed * 0.5f);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////움직임 연산

        rigid.velocity = new Vector2(inputX * moveSpeed, rigid.velocity.y);
        //transform.Translate(Vector2.right * inputX * moveSpeed * Time.deltaTime);
    }

    void DetectSlope(){     //경사로 감지 및 경사로상의 움직임
        
        if(isSlope && inputX == 0){//x축 입력이 없을 때
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; //x축의 이동과 회전을 잠금
        }else{          //이외의 경우
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;  //회전만 잠금(x축의 이동잠금은 해제)
        }

        RaycastHit2D rayHit = Physics2D.Raycast(checkPos.position, Vector2.down, distance, LayerMask.GetMask("groundMask")); //checkPos의 위치로부터 아래방향으로 distance의 거리만큼 groundMask만 스캔
        Debug.DrawRay(checkPos.position, Vector2.down * distance, Color.green);//위에서 그려진 ray를 가시화. 색은 Green.

        if(rayHit){

            angle = Vector2.Angle(rayHit.normal, Vector2.up);   //rayHit.normal : 스캔된 오브젝트의 표면에서 수직방향을 반환, Angle : 두 지점의 각도를 구함
            Debug.DrawLine(rayHit.point, rayHit.point + rayHit.normal, Color.red);  //Ray가 닿은 부분부터 닿은 부분으 수직으로 선을 그린다. 색은 Red.

            perp = Vector2.Perpendicular(rayHit.normal).normalized;        //위에서 그려진 법선을 90도 회전한 각도를 저장한다.
            Debug.DrawLine(rayHit.point, rayHit.point + perp, Color.blue);  //Ray가 닿은 부분부터 닿은부분의 수직 법선을 90도 회전한 값으로 그린다. 색은 blue.

            //Debug.Log(perp);

            if(angle != 0){
                isSlope = true;
            }else{
                isSlope = false;
            }
        }
    }

    void GroundCheck1(){     //지면 감지
        if(rigid.velocity.y > 0.1f && !isSlope){
            isGroundBox = Physics2D.BoxCast(Vector2.zero,Vector2.zero,0,Vector2.zero);
        }else{
            //isGround = Physics2D.OverlapCircle(checkPos.position, checkRadius, LayerMask.GetMask("groundMask"));
            int layerMask = (1 << LayerMask.NameToLayer("Player")); //플레이어 레이어 제외
            layerMask  = ~layerMask ;
            isGroundBox = Physics2D.BoxCast(groundCheckBox1.transform.position, groundCheckBox1.transform.localScale, 0, Vector2.down, 0, layerMask);
        }

        if(isGroundBox && isGroundBox.collider.tag == "Land"){
            isGround1 = true;
        }else{
            isGround1 = false;
        }
    }
    

    private void Jump()
    {
        if (isGround1 == true){
            if (Input.GetAxis("Jump") != 0 && !(isCharginAtk || isCharging))    //공격을 차징중이거나 차징공격을 실행중일 때는 점프를 할 수 없다.
            {
                isJump = true;
                //rigid.velocity = new Vector2(rigid.velocity.x, 0);
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            }
        }
        if(isGround1 && rigid.velocity.y <= 0){
            isJump = false;
        }
    }

    private void Dash(){
        if((inputX != 0)&&(Input.GetKeyDown(KeyCode.LeftShift))){
            Debug.Log("DashKey Down");
            dashDir = Input.GetAxisRaw("Horizontal");       //누르고 있는 방향을 대쉬 방향으로 고정
            isDash = true;                                  //대쉬 상태 on
            moveSpeed = moveSpeed * dashPower;              //이동속도를 증가
            Invoke("EndDash", 0.1f);                        //0.1초 후 감속 함수를 실행
        }
    }

    private void EndDash(){
        moveSpeed = moveSpeed / dashPower;                  //증가했던 속도만큼 다시 감소
        isDash = false;                                     //대쉬 상태 off
    }

    void OnOffDetectGround(){
        if(rigid.velocity.y > 0){
            groundCheckBox1.SetActive(false);
        }else{
            groundCheckBox1.SetActive(true);
        }
    }

    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag == "Coin"){
            //Player_Interaction.playerInter.GetCoin(other);
            GetComponent<Player_Interaction>().GetCoin(other);
        }
        if(other.gameObject.tag == "Enemy"){
            //Player_Interaction.playerInter.Damaged(other);
            GetComponent<Player_Interaction>().Damaged(other);
        }

    }

    void FaceDir(){
        //캐릭터 x좌표 - 공격박스 x좌표 
        float dir = gameObject.transform.position.x - atkPos.transform.position.x;
        if((atkPos.localPosition.x < 0 && inputX ==1) || (atkPos.localPosition.x > 0 && inputX == -1)){
            //Debug.Log(atkPos.localPosition.x + " <<기존|새로>> " + new Vector3(-atkPos.position.x, atkPos.position.y, atkPos.position.z));
            atkPos.localPosition = new Vector3(-atkPos.localPosition.x, atkPos.localPosition.y, atkPos.localPosition.z);
            //Debug.Log(atkPos.position.x);
            if(dir < 0){
                face = -1;
            }else if(dir > 0){
                face = 1;
            }
        }
        else{
            return;
        }
    }

    void FlipSP(){
        if(face == -1){
            pSprender.flipX = true;
            pSprender.transform.localPosition = new Vector2(0.06f,pSprender.transform.localPosition.y);
        }else{
            pSprender.flipX = false;
            pSprender.transform.localPosition = new Vector2(-0.06f,pSprender.transform.localPosition.y);
        }
        
    }
    // 상호작용///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void FirstSend(){
        gameManager.playerMaxHP = maxHP;
        gameManager.playerMaxCharging = chargingMax;
        gameManager.playerMaxMP = maxMP;
        gameManager.playerMaxExp = maxExp;
    }
    void SendGM(){
        gameManager.playerCurHP = curHP;
        gameManager.playerCurMP = curMP;
        gameManager.playerCurExp = curExp;
        gameManager.playerLevel = playerLevel;
        if(isCharging){
            gameManager.playerCurCharging = charging;
        }
    }
    // void GetCoin(Collision2D other){    //코인 획득 시
    //     if(other.gameObject.name == "GoldCoin(Clone)"){
    //             gameManager.GetGoldCoin();
    //         }else if(other.gameObject.name == "SilverCoin(Clone)"){
    //             gameManager.GetSilverCoin();
    //         }
    //         Destroy(other.gameObject);
    // }

    // public void GetExp(float exp){
    //     curExp = curExp + exp;
    //     if(curExp >= maxExp){
    //         curExp = curExp - maxExp;                                           //경험치 초과분
    //         maxExp = maxExp + maxExp * 0.3f;                                    //최대 경험치 갱신
    //         gameManager.playerMaxExp = maxExp;                                  //gm의 최대 경험치 갱신
    //         playerLevel++;
    //     }
    // }
    public void GetExp(float expByGM){
        Debug.Log("Recive exp and send to pInteraction");
        GetComponent<Player_Interaction>().GetExp(expByGM);
    }

    private void OnDrawGizmos() {   
        //공격 범위 그리기
        Gizmos.color = Color.red;                               
        Gizmos.DrawWireCube(atkPos.position, atkBoxSize);

        //지면 감지 범위1
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckBox1.transform.position, groundCheckBox1.transform.localScale);
        
        //지면 감지 범위2
        // Gizmos.color = Color.green;
        // Gizmos.DrawWireCube(groundCheckBox2.transform.position, groundCheckBox2.transform.localScale);

        // //플레이어 바디 범위
        // Gizmos.color = Color.cyan;
        // Gizmos.DrawWireSphere(Head.transform.position, Head.transform.localScale.x);//머리
        // Gizmos.DrawWireCube(Body.transform.position, Body.transform.localScale);//몸통
        // Gizmos.DrawWireSphere(Leg.transform.position, Leg.transform.localScale.x);//다리
        // Gizmos.DrawWireCube(Face.transform.position, Face.transform.localScale);//얼굴
    }
}
