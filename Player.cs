using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.XR;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprender;
    public Transform checkPos;
    public GameManager gameManager;
    public GameObject ATKAreaView;
    public GameObject groundCheckBox1;
    //public GameObject groundCheckBox2;
    public GameObject Head;
    public GameObject Body;
    public GameObject Leg;

    Collider2D thisCol;

    public float moveSpeed;      //움직임 속도   
    public float checkRadius;   //감지 반경
    public float distance;      //감지거리
    public float jumpPower;     //점프 파워
    public float HP;            //체력
    public float def;           //방어력
    public float weaponDmg;     //무기 공격력

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




    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
        thisCol = GetComponent<Collider2D>();
        sprender = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        Face();
        Attack();
        AttackTime();
        DetectSlope();
        GroundCheck1();
        OnOffDetectGround();
        //BodySense();
    }

    private void FixedUpdate() {
        Move();
        Jump();
    }
    void Move(){
        inputX = Input.GetAxisRaw("Horizontal");
        if(isSlope && isGround1 && !isJump){       //isGround는 합쳐서 isLanding으로 묶을 것 
            rigid.velocity = perp * moveSpeed * inputX * -1;
        }else if(!isSlope && isGround1){
            rigid.velocity = new Vector2(inputX * moveSpeed, 0);
        }else if(!isGround1){
            rigid.velocity = new Vector2(inputX * moveSpeed, rigid.velocity.y);
        }
        if(isJump){
            rigid.velocity = new Vector2(inputX * moveSpeed / 1.5f, rigid.velocity.y);
        }else{
            rigid.velocity = new Vector2(inputX * moveSpeed, rigid.velocity.y);
        }
        
        rigid.velocity = new Vector2(inputX * moveSpeed, rigid.velocity.y);
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

    /*
    void BodySense(){
        RaycastHit2D isHeadSense = Physics2D.CircleCast(Head.transform.position, Head.transform.localScale.x, Vector2.zero, 0, LayerMask.GetMask("groundMask") );
        RaycastHit2D isBodySense = Physics2D.BoxCast(Body.transform.position, Head.transform.localScale, 0, Vector2.zero, 0, LayerMask.GetMask("groundMask") );
        RaycastHit2D isLegSense = Physics2D.CircleCast(Leg.transform.position, Leg.transform.localScale.x, Vector2.zero, 0, LayerMask.GetMask("groundMask") );
        if(isHeadSense){
            headSense = true;
        }
        if(isBodySense){
            bodySense = true;
        }
        if(isLegSense){
            legSense = true;
        }

        if(headSense){
            if(bodySense){
                if(legSense){
                    
                }else{
                    headSense = false;
                    bodySense = false;
                }
            }else{
                headSense = false;
            }
        }else if(bodySense){
            if(legSense){

            }else{
                bodySense = false;
            }
        }
    }
    */
    

    private void Jump()
    {
        if (isGround1 == true){
            if (Input.GetAxis("Jump") != 0)
            {
                isJump = true;
                rigid.velocity = new Vector2(rigid.velocity.x, 0);
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            }
        }
        if(isGround1 && rigid.velocity.y <= 0){
            isJump = false;
        }
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
            GetCoin(other);
        }
        if(other.gameObject.tag == "Enemy"){
            Damaged(other);
        }

    }

    void Attack(){          //공격
        if(Input.GetKeyDown("z") && atkCurTime > atkMaxTime){
            AttackingAnime();   //공격 애니메이션
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(atkPos.position, atkBoxSize, 0); //OverlapBoxAll(position, size, angle) 배열을 생성하고 감지된 오브젝트를 모두 담는다
            foreach(Collider2D collider in collider2Ds){                                        
                if(collider.tag == "Enemy"){
                    gameManager.AtkToEnemy(weaponDmg, collider);    //감지된 적과 공격력을 매개변수로 gameManager의 AtkToEnemy함수를 호출
                }
            }
            atkCurTime = 0;
        }
    }
    void AttackTime(){      //공격시간 연산
        atkCurTime += Time.deltaTime;
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

        //플레이어 바디 범위
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Head.transform.position, Head.transform.localScale.x);//머리
        Gizmos.DrawWireCube(Body.transform.position, Body.transform.localScale);//몸통
        Gizmos.DrawWireSphere(Leg.transform.position, Leg.transform.localScale.x);//다리
    }

    void AttackingAnime(){          //공격 애니메이션
        ATKAreaView.SetActive(true);
        Invoke("AttackEndAnime", 0.4f);
    }
    void AttackEndAnime(){          //공격 애니메이션
        ATKAreaView.SetActive(false);
    }



    void Face(){
        if((atkPos.localPosition.x < 0 && inputX ==1) || (atkPos.localPosition.x > 0 && inputX == -1)){
            //Debug.Log(atkPos.localPosition.x + " <<기존|새로>> " + new Vector3(-atkPos.position.x, atkPos.position.y, atkPos.position.z));
            atkPos.localPosition = new Vector3(-atkPos.localPosition.x, atkPos.localPosition.y, atkPos.localPosition.z);
            //Debug.Log(atkPos.position.x);
        }
        else{
            return;
        }
    }

    void Damaged(Collision2D other){     //적에게 맞았을 때
        float damage = gameManager.DamagedFromEnemy(other);
        rigid.velocity = Vector2.zero;  //이전 연산되고 있던 속도를 무효
        gameObject.layer = 9;
        sprender.color = new Color( 120/255f , 120/255f, 120/255f);
        if(damage <= def / 15){
            HP = HP - 0;
        }else{
            if(HP > HP - (damage - def / 15)){
                HP = HP - (damage - def / 15);//플레이어 체력감소
            }else{
                //Destroy
            }
        }
        
        int dirc = transform.position.x - other.gameObject.GetComponent<Enemy>().rigid.position.x > 0 ? 1 : -1;     //(현재 플레이어의 좌표 - 부딪힌 오브젝트의 좌표) 가 양수일때는 1, 음수일때는 -1
        //Debug.Log(dirc);
        rigid.AddForce(new Vector2(dirc * 100000, 1) * damageShock ,ForceMode2D.Impulse);      //위의 방향(dirc)대로 힘을 가함 
        Debug.Log(new Vector2(dirc * 100000, 1));
        Invoke("OffDamaged", 0.5f); 
    }

    void OffDamaged(){
        gameObject.layer = 3;
        sprender.color = new Color(0,0,0);
    }

    void GetCoin(Collision2D other){    //코인 획득 시
        if(other.gameObject.name == "GoldCoin(Clone)"){
                gameManager.GetGoldCoin();
            }else if(other.gameObject.name == "SilverCoin(Clone)"){
                gameManager.GetSilverCoin();
            }
            Destroy(other.gameObject);
    }
}
