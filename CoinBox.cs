using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinBox : MonoBehaviour
{
    public bool isTouch;    //플레이어와 맞닿았는지를 저장하는 변수
    public float upPower;   //박스가 튕기는 정도
    
    public GameObject GoldCoin;
    public GameObject SilverCoin;

    private int ranCoin;            //코인오브젝트를 담을 배열 size
    private GameObject[] coinArr;   //코인 오브젝트를 담을 배열(size는 추후 결정)
    

    Rigidbody2D rigid;
    
    private void Start() {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Awake() {
        ranCoin = Random.Range(3,11);               //랜덤하게 코인의 개수를 정한다
        coinArr = new GameObject[ranCoin];              //위에서 정한 개수만큼 오브젝트를 담을 배열을 생성
        for(int i = 0; i < ranCoin; i++){               //골드 1 : 실버 4 비율로 랜덤하게 배열에 오브젝트를 push
            int ranCoinType = Random.Range(0,5);
            if(ranCoinType == 0){
                coinArr[i] = GoldCoin;
            }else{
                coinArr[i] = SilverCoin;
            }
        }
    }
    void Update()
    {
        if(isTouch){
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        BoxUp();
        PopCoin();
        isTouch = false;
    }
    void CrashBox(){
        gameObject.SetActive(false);
    }
    void BoxUp(){
        if(isTouch == true){
            rigid.AddForce(Vector2.up * upPower,ForceMode2D.Impulse);
            Invoke("CrashBox", 0.2f);
        }
    }

    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.name == "Player"){
            isTouch = true;
        }
    }

    void PopCoin(){
        if(isTouch){
            for(int i = 0; i < ranCoin; i++){
                Debug.Log(transform.position);
                GameObject coin = Instantiate(coinArr[i], transform.position /*Vector2.up * 1.5f*/, transform.rotation);
                float coinDirectionW = Random.Range(-1f,2f);
                float coinDirectionH = Random.Range(3,6);
                Rigidbody2D rigid = coin.GetComponent<Rigidbody2D>();
                rigid.AddForce(new Vector3(coinDirectionW,  coinDirectionH, 0), ForceMode2D.Impulse);
            }
            
        }

    }
}
