using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserCaneAttack : MonoBehaviour
{

    [SerializeField] private GameObject _chaserBullet = default;//魔法弾

    private int _weaponLevel = 0;//魔法武器の進化ナンバー

    private float _magicAttackBonusPower = 1f;//進化ボーナス攻撃力
    private float _criticalMultiple = default;//クリティカル倍率

    private GameObject _player = default;

    private bool _isAttack = false;//攻撃をしているか

    private SpriteRenderer _caneSprite = default;//武器のスプライト

    private AudioSource _seAudio = default;

    private PlayerAttack _playerAttack = default;
    private PlayerStatusChange _playerStatusChange;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _playerAttack = _player.GetComponent<PlayerAttack>();

        _playerStatusChange= _player.GetComponent<PlayerStatusChange>();

        _caneSprite = this.gameObject.GetComponent<SpriteRenderer>();

        _seAudio = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_isAttack)
        {
            //攻撃中は武器をプレイヤーに追尾させる
            this.transform.position = _player.transform.position;
        }
    }

    public void AttackStart()
    {
        this.transform.position = _player.transform.position;
        _isAttack = true;

        //魔法武器の可視化
        _caneSprite.enabled = true;

        //クリティカル判定とクリティカル倍率を取得
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
        //攻撃力を代入
        float AttackValue = _playerAttack.GiveCaneMagicAttackPower();

        //クリティカル倍率の計算（デフォルトで１）
        float FinalAttackValue = Mathf.Floor(AttackValue * _criticalMultiple);

        ChaserMagic(FinalAttackValue,isCriticalJudge);
    }


    public void ChaserMagic(float power,bool isCritical)
    {

        StartCoroutine(WeaponInvisible());
        _isAttack = true;

        switch (_weaponLevel)
        {
            case 0://第一段階の時
                WeaponLevel0(power, isCritical);
                break;

            case 1:
                WeaponLevel1(power, isCritical);
                break;
        }
    }

    public void WeaponLevel0(float attackPower, bool isCriticalJudge)
    {
        //クリックしたポジションの取得
        Vector3 clickposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int mouse2DClickPositionZ = 10;
        clickposition.z += mouse2DClickPositionZ;

        //武器をクリックした方向に向かせる
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position);

        GameObject burret = default;

        burret = Instantiate(_chaserBullet, this.transform.position, Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position));
        burret.GetComponent<ChaserBurretScript>().GetPower(attackPower * _magicAttackBonusPower, this.GetComponent<ChaserCaneAttack>(), isCriticalJudge,_weaponLevel);
    }

    public void WeaponLevel1(float attackPower, bool isCriticalJudge)
    {
        //クリックしたポジションの取得
        Vector3 clickposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int mouse2DClickPositionZ = 10;
        clickposition.z += mouse2DClickPositionZ;

        //武器をクリックした方向に向かせる
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position);

        GameObject burret = default;

        burret = Instantiate(_chaserBullet, this.transform.position, Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position));
        burret.GetComponent<ChaserBurretScript>().GetPower(attackPower * _magicAttackBonusPower, this.GetComponent<ChaserCaneAttack>(), isCriticalJudge, _weaponLevel);
    }

    public void ReProductionSE(AudioClip seMaterial)
    {
        //print(seMaterial.name);

        _seAudio.PlayOneShot(seMaterial);
    }

    public void WeaponLevelUp()
    {

        _weaponLevel++;

        //進化ボーナスを渡す
        switch (_weaponLevel)
        {
            case 1:
               //MPを上げる
                float upValue = 20;
                _playerStatusChange.MagicPointUp(upValue);
                break;
        }
    }

    public int GiveWeaponLevel()
    {
        return _weaponLevel;
    }

    private IEnumerator WeaponInvisible()
    {
        yield return new WaitForSeconds(0.5f);
        _caneSprite.enabled = false;
        _isAttack = false;

    }
}
