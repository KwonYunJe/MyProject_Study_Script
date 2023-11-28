using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Enemy : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject groundCheckBox;
    public GameObject cliffCheckBox;
    public float cliffCheckDistance;
    public float cliffCheckStartSpot;
    public RaycastHit2D cliffCheck;

    public Rigidbody2D rigid;
    CircleCollider2D circleCol;
    SpriteRenderer sprender;


    public float hp;
    public float atkPower;
    public float enemyLevel;

    public bool move;
    public int moveDir;
    public float moveTime ;
    public int moveSpeed;
    public int face;
    public float jumpPower;
    public bool jumping;
    public bool startDestroy;
    public float alpha; //파괴 애니메이션을 위한 투명도 값
    
    public bool isSlope;
    public bool isGround;
    public bool isJump;
    public float distance;
    public float angle;    //맞닿은 표면과 수직선과의 각도를 저장
    public Vector2 perp;
    public RaycastHit2D groundCheck;
    
    public bool isCliff;
    public Vector2 befJumpPos;

    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
        sprender = GetComponent<SpriteRenderer>();
        circleCol = GetComponent<CircleCollider2D>();
        move = true;
        Invoke("MonsterMoveStatus", 1f);
        MonsterJumpPer();
        face = 1;
        alpha = 1;
    }
    private void Awake() {
        

    }

    private void Update() {
        if(startDestroy == false){
            MosterMove();
            DetectSlope();
            GroundCheck();
            Face();
            DetectCliff();
        }
        Destory2nd();
    }


///이동관련//////////////////////////////////////////////////////////////////////////////////////////

    void MosterMove(){
        if(move == true){

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
            if(!isJump){
                befJumpPos = rigid.position; //점프하기 전 위치를 기록
            }
            MonsterJump();
        }
        Invoke("MonsterJumpPer", 3);
    }

    void Face(){
        if(moveDir < 0 && face > 0 || moveDir > 0 && face < 0){
            face = -face;
            Debug.Log("face : " + face);
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
        Vector2 start = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);  //ray의 시작 좌표값

        if(isSlope){
            if((face > 0 && perp.y < 0) || (face < 0 && perp.y > 0)){     //우상향 경사, 상승중 / 우하향 경사, 하강중
                //시작좌표값을 접촉중인 경사면의 각도, 길이는 1만큼, 방향은 진행방향과 동일하게 이동시킨다 
                start = new Vector2(gameObject.transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * (cliffCheckStartSpot * face),
                gameObject.transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad));

            }else if((face > 0 && perp.y > 0) || (face < 0 && perp.y < 0)){   //우하향 경사, 하강중 / 우상향 경사, 하강중
                start = new Vector2(gameObject.transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * (cliffCheckStartSpot * face),
                gameObject.transform.position.y - Mathf.Sin(angle * Mathf.Deg2Rad));
            }
        }
        cliffCheck = Physics2D.Raycast(start, Vector2.down, cliffCheckDistance, LayerMask.GetMask("groundMask"));
        Debug.DrawRay(start, Vector2.down * cliffCheckDistance, Color.cyan);//위에서 그려진 ray를 가시화. 색은 Cyan.

        if(!cliffCheck){
            //Debug.Log("Not detected land");
            cliffCheckDistance += 0.1f;
            if(cliffCheckDistance > 10){
                moveDir = 0;
            }
        }else{
            //Debug.Log("Detected land");
            cliffCheckDistance = 1f;
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
            gameManager.EnemyDeadEvent(gameObject);
            Destroy1st();
            startDestroy = true;
        }
    }

    private void DamagedAnime(){
        sprender.color = new Color(1, 1, 1, 1);
        Invoke("DamagedAnimeEnd",0.1f);
    }

    private void DamagedAnimeEnd(){
        sprender.color = new Color(78f / 255f, 174f / 255f ,255f / 255f, 1);
    }

    public void Destroy1st(){
        move = false;
        circleCol.enabled = false;
        Destroy(rigid);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

     private void Destory2nd(){
        if(startDestroy == true){
            alpha = alpha - 0.01f;
            if(alpha <= 0){
                Destroy(gameObject);
            }else{
                sprender.color = new Color(78f / 255f, 174f / 255f ,255f / 255f, alpha);
            }
        }
    }


    private void OnDrawGizmos() {
        //지면 감지 오브젝트 경계선(groundCheckBox)
        Gizmos.color = Color.blue;                               
        Gizmos.DrawWireCube(groundCheckBox.transform.position, groundCheckBox.transform.localScale);
    }


}
