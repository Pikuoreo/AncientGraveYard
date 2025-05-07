using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SordAttack : MonoBehaviour
{
    private bool _isAttack = false;//trueで攻撃開始
    private bool _isCharge = default;//trueでチャージ攻撃
    private bool _isAttackdirection = false;//falseは左向き、trueは右向き

    private int _weaponLevel = 0;//武器の進化形態

    private float _meleeChargeTime = 0f;//近接攻撃のチャージ時間
    private float _criticalMultiple = default;
    private float _playerMagicPoint = default;

    private SpriteRenderer _sordSprite = default;

    private  float _chargeAttackTime = 1f;//チャージまでの時間

    private PlayerAttack _playerAttack=default;
    private PlayerStatusChange _playerStatus = default;
    private SordMove _meleeAttackMove = default;

    [Header("追加攻撃の剣気"),SerializeField] private GameObject _sordAir = default;
    private GameObject _sordAirClone = default;
    private SordAirMove _sordAirmove = default;

    // Start is called before the first frame update
    void Start()
    {
        _sordSprite = this.GetComponent<SpriteRenderer>();
        _playerAttack = this.GetComponentInParent<PlayerAttack>();
        _meleeAttackMove = this.GetComponent<SordMove>();
        _playerStatus= this.GetComponentInParent<PlayerStatusChange>();
    }

    // Update is called once per frame
    void Update()
    {
        //クリック長押し中
        if (_isAttack)
        {
            //攻撃チャージ時間を計測
            _meleeChargeTime += Time.deltaTime;

            if (_meleeChargeTime > _chargeAttackTime&&!_isCharge)
            {
                SpriteChangeCharge();
            }

        }
        //左クリックを離したら
        else if(_meleeChargeTime>0)
        {
            _meleeChargeTime = 0;

            switch (_weaponLevel)
            {
                //未進化
                case 0:
                    WeaponLevel0();

                    break;

                //第進化
                case 1:
                    WeaponLevel1();
                    break;
            }
          
        }
    }


    /// <summary>
    /// すべての武器レベルの攻撃に共通する処理
    /// </summary>
    public void AllWeaponLevelProcess()
    {
        //色を元に戻す
        _sordSprite.color = Color.white;


        bool isCritical = _playerAttack.GiveCriticalJudge();

        //クリティカル攻撃なら
        if (isCritical)
        {
            //クリティカル倍率を代入
            _criticalMultiple = _playerAttack.GiveCriticalMultiple();
        }
        //通常攻撃なら
        else
        {
            //クリティカル倍率を１にする

            float notcritical = 1;
            _criticalMultiple = notcritical;
        }

        PowerCalculation(isCritical);
    }
    public void WeaponLevel0()
    {
        AllWeaponLevelProcess();
    }

    public void WeaponLevel1()
    {
        int reduseMagicPoint = 10;
        if (_isCharge&&_playerMagicPoint>reduseMagicPoint)
        {
            _playerStatus.ReduseMagicPoint(reduseMagicPoint);
            if (_sordAirClone == null)
            {
                //剣気を飛ばす
                _sordAirClone=Instantiate(_sordAir,this.transform.position,Quaternion.identity);
                _sordAirmove = _sordAirClone.GetComponentInParent<SordAirMove>();
            }

            //ポジションを剣の位置にする
            _sordAirClone.transform.position = this.transform.position;
            //動くアニメーションスタート
            _sordAirmove.MoveStart(_isAttackdirection);
        }
        AllWeaponLevelProcess();
    }

    private void PowerCalculation(bool isCriticalJudge)
    {
        //攻撃力、クリティカル判定を取得
        float AttackValue = _playerAttack.GiveAttackPower(_isCharge);

        float FinalAttackValue = Mathf.Floor(AttackValue * _criticalMultiple);

        //攻撃力を渡す
        _meleeAttackMove.GetAttackPower(FinalAttackValue,isCriticalJudge);
        _isCharge = false;
    }

    public void StartPreAttack(bool preAttackJudge,bool isSordDirection,float magicPoint)
    {
        _isAttack = preAttackJudge;
        _isAttackdirection = isSordDirection;
        _playerMagicPoint = magicPoint;
    }

    private void SpriteChangeCharge()
    {
        _sordSprite.color = Color.yellow;
        _isCharge = true;
    }

    public void WeaponLevelUp()
    {
        _weaponLevel++;

        //進化ボーナス付与
        switch (_weaponLevel)
        {
            case 1:
                //攻撃力アップ
                int meleePowerUpValue = 30;
                _playerStatus.MeleeAttackPowerUp(meleePowerUpValue);
                break;
        }
    }

    public int GiveWeaponLevel()
    {
        return _weaponLevel;
    }

   
}
