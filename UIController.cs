using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameManager gameManager;
    public Image HpBarCircle;
    public Image chargingBar;
    public Image mpBar;
    public Image mpBarBack;
    public Image expBar;
    public Text hpText;
    public Text mpText;
    public Text chargingText;
    public Text expText;
    public Text playerLevelText;
    public float playerLevel;
    public float playerMaxHP;
    public float playerCurHP;
    public bool playerIsDamaged;
    public float playerMaxCharging;
    public float playerCurCharging;
    public float playerMaxMP;
    public float playerCurMP;
    public float playerMaxExp;
    public float playerCurExp;
    public int mpAniCycle;
    public int mpAniCycleMax;

    private void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        setHP();
        SetMP();
        SetExp();
        setCharging();
        SetLevelText();
    }

    public void setHP(){
        if(playerMaxHP != 0){
            float amount = playerCurHP / playerMaxHP;
            HpBarCircle.fillAmount = amount;
            hpText.text = amount * 100 + "%";
            //Debug.Log(amount);
        }
    }

    public void setCharging(){
        if(playerMaxCharging != 0){
            float amount = playerCurCharging / playerMaxCharging;
            chargingBar.fillAmount = amount;
            chargingText.text = amount * 100 + "%";
            //Debug.Log(amount);
        }
    }

    public void SetMP(){
        if(playerMaxMP != 0){
            float amount = playerCurMP / playerMaxMP;
            mpBar.fillAmount = amount;
            mpText.text = amount * 100 + "%";
            //Debug.Log(amount);
        }
    }

    public void SetExp(){
        if(playerMaxExp != 0){
            float amount = playerCurExp / playerMaxExp;
            expBar.fillAmount = amount;
            expText.text = amount * 100 + "%";
            //Debug.Log(amount);
        }
    }

    public void SetLevelText(){
        playerLevelText.text = "Lv. " + playerLevel;
    }

    public void NotEnoughMPAni(){
        if(mpAniCycle < mpAniCycleMax){
            MPBarAniRed();
        }else{
            mpAniCycle = 0;
        }
    }

    public void MPBarAniRed(){
        mpBarBack.color = new Color(255/255, 125/255, 125/255, 1);
        Invoke("MPBarAniGrey", 0.1f);
    }
    public void MPBarAniGrey(){
        mpBarBack.color = new Color(82/255, 82/255, 82/255, 1);
        mpAniCycle++;
        Invoke("NotEnoughMPAni", 0.1f);
    }
}
