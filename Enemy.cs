using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Enemy : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject groundCheckBox;
    public GameObject cliffCheckBox;
    public Rigidbody2D rigid;
    SpriteRenderer sprender;


    public float hp;
    public float atkPower;

    public int moveDir;
    public float moveTime ;
    public int moveSpeed;
    public int face;
    public float jumpPower;
    public bool jumping;

    
    public bool isSlope;
    public bool isGround;
    public bool isJump;
    public float distance;
    public float angle;    //맞닿은 표면과 수직선과의 각도를 저장
    public Vector2 perp;
    public RaycastHit2D groundCheck;
    public RaycastHit2D cliffCheck;


    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
        sprender = GetComponent<SpriteRenderer>();
        Invoke("MonsterMoveStatus", 1f);
        MonsterJumpPer();
        
    }
    private void Awake() {
        

    }

    private void Update() {
        MosterMove();
        DetectSlope();
        GroundCheck();
        Face();
        DetectCliff();
    }


///이동관련//////////////////////////////////////////////////////////////////////////////////////////

    void MosterMove(){
        if(isSlope && isGround && !isJump){       //isGround는 합쳐서 isLanding으로 묶을 것 
            rigid.velocity = perp * moveSpeed * moveDir * -1;
        }else if(!isSlope && isGround){
            //rigid.velocity = new Vector2(moveDir * moveSpeed, 0);
        }else if(!isGround){
            rigid.velocity = new Vector2(moveDir * moveSpeed, rigid.velocity.y);
        }

        rigid.velocity = new Vector2(moveDir  * moveSpeed, rigid.velocity.y);

        if(isGround && rigid.velocity.y <= 0){
            isJump = false;
        }
    }

    void MonsterMoveStatus(){
        //Debug.Log("MonsterMoveStart");
        moveDir = Random.Range(-1, 2);   //움직일 방향 or 제자리
        moveTime = Random.Range(2, 7);    //행동을 지속할 시간
        moveSpeed = Random.Range(1,3);
        //Debug.Log(" move : " + moveDir + " Time : " + moveTime + " speed : " + moveSpeed);

        Invoke("MonsterMoveStop", moveTime);
    }
    void MonsterMoveStop(){
        //Debug.Log("MonsterStop");
        moveDir = 0;

        Invoke("MonsterMoveStatus", 1f);
    }

    void MonsterJump(){
        if (isGround == true){
            //Debug.Log("jump");
            isJump = true;
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    void MonsterJumpPer(){
        int jumpPer = Random.Range(0,2);
        //Debug.Log("jumpPer : "+ jumpPer);
        if(jumpPer == 1){
            MonsterJump();
        }
        Invoke("MonsterJumpPer", 3);
    }

    void Face(){
        if((moveDir == 1 && cliffCheckBox.transform.localPosition.x < 0) || (moveDir == -1 && cliffCheckBox.transform.localPosition.x > 0)){
            cliffCheckBox.transform.localPosition = new Vector2(-cliffCheckBox.transform.localPosition.x, cliffCheckBox.transform.localPosition.y);
        }else{
            return;
        }
    }


    void DetectSlope(){     //경사로 감지 및 경사로상의 움직임
        
        if(isSlope && moveDir == 0){//x축 입력이 없을 때
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; //x축의 이동과 회전을 잠금
        }else{          //이외의 경우
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;  //회전만 잠금(x축의 이동잠금은 해제)
        }

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.down, distance, LayerMask.GetMask("groundMask")); //checkPos의 위치로부터 아래방향으로 distance의 거리만큼 groundMask만 스캔
        Debug.DrawRay(transform.position, Vector2.down * distance, Color.green);//위에서 그려진 ray를 가시화. 색은 Green.

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

    void DetectCliff(){
        cliffCheck = Physics2D.BoxCast(cliffCheckBox.transform.position, cliffCheckBox.transform.localScale, 0, Vector2.down, 0);
        Debug.Log(cliffCheck.collider.name);/////////////////////////////////////////////////////////////////////////////////////////감지가 안됨 
        if(isGround && !cliffCheck){
            Debug.Log("Cliff Detected!");
            rigid.velocity = new Vector2(0,rigid.velocity.y);
            Invoke("MonsterMoveStatus", 1f);
        }
    }

    void GroundCheck(){     //지면 감지
        if(rigid.velocity.y > 0.1f && !isSlope){
            groundCheck = Physics2D.BoxCast(Vector2.zero,Vector2.zero,0,Vector2.zero);
        }else{
            //isGround = Physics2D.OverlapCircle(checkPos.position, checkRadius, LayerMask.GetMask("groundMask"));
            int layerMask = (1 << LayerMask.NameToLayer("Enemy")); //플레이어 레이어 제외
            layerMask  = ~layerMask ;
            groundCheck = Physics2D.BoxCast(groundCheckBox.transform.position, groundCheckBox.transform.localScale, 0, Vector2.down, 0, layerMask);
        }

        if(groundCheck && groundCheck.collider.tag == "Land"){
            isGround = true;
        }else{
            isGround = false;
        }
    }

    public void Damaged(float weaponDmg){
        DamagedAnime();
        if(hp > weaponDmg){
            hp = hp-weaponDmg;
        }else{
            DestoryAnime();
        }
    }

    private void DamagedAnime(){
        sprender.color = new Color(1, 1, 1, 1);
        Invoke("DamagedAnimeEnd",0.5f);
    }

    private void DamagedAnimeEnd(){
        sprender.color = new Color(78f / 255f, 174f / 255f ,255f / 255f,1);
    }

    private void DestoryAnime(){
        sprender.color = Color.red;
        Destroy();
    }

    public void Destroy(){
        gameObject.SetActive(false);
    }


    private void OnDrawGizmos() {
        //지면 감지 오브젝트 경계선(groundCheckBox)
        Gizmos.color = Color.blue;                               
        Gizmos.DrawWireCube(groundCheckBox.transform.position, groundCheckBox.transform.localScale);

        //절벽 감지 오브젝트 경계선(cliffCheckBox)
        Gizmos.color = Color.green;                               
        Gizmos.DrawWireCube(cliffCheckBox.transform.position, cliffCheckBox.transform.localScale);
    }
}
