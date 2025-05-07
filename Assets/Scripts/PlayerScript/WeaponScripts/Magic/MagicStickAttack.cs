using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicStickAttack : MonoBehaviour
{
    [SerializeField] GameObject _magicBullet = default;//魔法弾

    private GameObject _player = default;

    private int _weaponLevel = 0;//魔法武器の進化ナンバー


    private float _magicAttackBonusPower1 = 1f;//進化ボーナス攻撃力
    private float _criticalMultiple = default;//クリティカル倍率

    private bool _isAttack = false;//trueで攻撃開始

    private SpriteRenderer _stickSprite = default;

    private AudioSource _seAudio = default;

    private PlayerAttack _playerAttack = default;
    private PlayerStatusChange _playerStatusChange =default;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _playerAttack = _player.GetComponent<PlayerAttack>();

        _playerStatusChange=_player.GetComponent<PlayerStatusChange>();

        _stickSprite = this.gameObject.GetComponent<SpriteRenderer>();

        _seAudio = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_isAttack)
        {
            this.transform.position = _player.transform.position;
        }
    }
    public void AttackStart()
    {
        this.transform.position = _player.transform.position;
        _isAttack = true;

        //魔法武器１の可視化
        _stickSprite.enabled = true;


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

    private void PowerCalculation(bool isCriticalJudge)
    {
        float AttackValue = _playerAttack.GiveStickMagicAttackPower();

        float FinalAttackValue = Mathf.Floor(AttackValue * _criticalMultiple);

        StraightMagic(FinalAttackValue, isCriticalJudge);
    }

    private void StraightMagic(float power, bool isCritical)
    {
        StartCoroutine(WeaponInvisible());
        _isAttack = true;

        //武器の進化形態
        switch (_weaponLevel)
        {
            case 0: //第一段階の時

                WeaponLevel0(power, isCritical);
                break;

            case 1:

                WeaponLevel1(power, isCritical);
                break;
        }
    }

    private void WeaponLevel0(float attackPower, bool isCriticalJudge)
    {
        //クリックしたポジションの取得
        Vector3 clickposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickposition.z += 10;

        //武器をクリックした方向に向かせる
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position);

        GameObject burret = default;

        //杖から見て少し上に弾生成
        burret = Instantiate(_magicBullet, this.transform.position + this.transform.up, Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position));

        //当たった時のダメージを代入させる
        burret.GetComponent<MagicBurretScript>().GetPower(attackPower * _magicAttackBonusPower1, this.GetComponent<MagicStickAttack>(), isCriticalJudge, _weaponLevel);
    }

    private void WeaponLevel1(float attackPower, bool isCriticalJudge)
    {
        //クリックしたポジションの取得
        Vector3 clickposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickposition.z += 10;

        //武器をクリックした方向に向かせる
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position);

        GameObject burret = default;

        //杖から見て少し上に弾生成
        burret = Instantiate(_magicBullet, this.transform.position + this.transform.up, Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position));

        //当たった時のダメージを代入させる
        burret.GetComponent<MagicBurretScript>().GetPower(attackPower * _magicAttackBonusPower1, this.GetComponent<MagicStickAttack>(), isCriticalJudge, _weaponLevel);
    }

    //再生するSEを受け取り、流す
    public void ReProductionSE(AudioClip seMaterial)
    {
        _seAudio.PlayOneShot(seMaterial);
    }

    public void WeaponLevelUp()
    {
        _weaponLevel++;
     
        //進化ボーナスを付与
        switch(_weaponLevel)
        {
            case 1:
                //魔法攻撃力を上げる
                int upValue = 30;
                _playerStatusChange.MagicAttackPowerUp(upValue);
                break;
        }
    }

    public int GiveWeaponLevel()
    {
        return _weaponLevel;
    }

    public float GiveMagicpointConsumptionValue()
    {
        float consumptionValue = default;
        switch (_weaponLevel)
        {
            case 0:

                //攻撃したときの消費魔力量
                float consumptionValueLv_1 = 5;
                consumptionValue = consumptionValueLv_1;

                break;

            case 1:

                //攻撃したときの消費魔力量
                float consumptionValueLv_2 = 8;
                consumptionValue = consumptionValueLv_2;
                break;
        }

        return consumptionValue;
    }




    private IEnumerator WeaponInvisible()
    {
        yield return new WaitForSeconds(0.5f);
        _stickSprite.enabled = false;
        _isAttack = false;
    }
}
