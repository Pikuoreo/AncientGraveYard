using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusChange : MonoBehaviour
{
    private int DefencePowerDivideValu = 4;

    private float _guardValue = 0.5f;//ガード時のダメージ軽減率
    private float _chargeMultiple = 2.5f;//チャージ攻撃時の倍率

    private float _regeneTime = 0f;　//Hpのダメージ食らってからの経過時間
    private float _magicRegeneTime = 0f;//Mpのダメージ食らってからの経過時間
    private float _regeneValue = 0f;//初期回復力、だんだん上がる（HP）
    private float _maxRegeneValue = 3f;//最大回復力（HPの最大回復力毎秒)
    private float _magicPointRegeneValue = 0f;//初期回復力、だんだん上がる(MP)
    private float _magicPointMaxRegeneValue = 10f;//最大回復力（MPの最大回復力毎秒)

    private float _actualDamage = default;//最終的にくらうダメージ量

    private const float START_REGENE_TIME = 2f;//Hpの回復が始まる時間
    private const float START_MAGICPOINT_REGENE_TIME = 0.75f;//Mpの回復が始まる時間

    private bool _isDeath = false;//死亡判定

    private PlayerMoveControll _playerMoveControll = default;
    private PlayerAttack _playerAttack = default;
    private PlayerStatusUI _playerStatusUI = default;
    private PlayerAnimationControll _playerAnimationControll = default;

    private BGMChangeScript _bgmScript = default;

    private GameOverScript _gameOverScript = default;

    [SerializeField] private PlayerStatusManegement _playerStatus = default;


    private Animator _playeDeathAnimation = default;
    void Start()
    {
        //プレイヤーのＨＰに最大ＨＰを代入する
        _playerStatus.HitPoint = _playerStatus.MaxHitPoint;
        //プレイヤーのＭＰに最大ＭＰを代入する
        _playerStatus.MagicPoint = _playerStatus.MaxMagicPoint;

        _playerAnimationControll=this.GetComponentInChildren<PlayerAnimationControll>();
        _playeDeathAnimation = this.GetComponent<Animator>();
        _playerMoveControll = this.GetComponent<PlayerMoveControll>();
        _playerAttack = this.GetComponent<PlayerAttack>();
        _playerStatusUI = this.gameObject.GetComponent<PlayerStatusUI>();

        string findTag = "BackGroundBGM";
        _bgmScript = GameObject.FindGameObjectWithTag(findTag).GetComponent<BGMChangeScript>();

        findTag = "FirstGameOverAnimation";
        _gameOverScript = GameObject.FindGameObjectWithTag(findTag).GetComponent<GameOverScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //チートコマンド
        if (Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.P) && Input.GetKeyDown(KeyCode.Return))
        {
            AttackPowerUP(1000);
        }

        //体力が減っていたら
        if (_playerStatus.HitPoint < _playerStatus.MaxHitPoint)
        {
            Regeneration();
        }

        if (_playerStatus.MagicPoint < _playerStatus.MaxMagicPoint)
        {
            MagicpointRegeneration();
        }

        //体力がなくなったら
        if (_playerStatus.HitPoint <= 0 && !_isDeath)
        {
            _isDeath = true;
            _playerStatus.HitPoint = 0;
            Death();
        }
    }

    private void Death()
    {
        _playerMoveControll.ControllOff();

        _playerAnimationControll.AnimationChange_Death();

        //死亡アニメーションを流す
        _playeDeathAnimation.SetBool("isDeath", true);

        //BGMを徐々に小さくする
        _bgmScript.BGMFadeout();

        //画面を暗くしていく
        _gameOverScript.StartGameOverAnimation();
    }

    public void ConvertHptoMp()
    {
        //減るHpの量（最大Hpの２０分の一）
        int CutDownOnLifeValue = 20;

        if (_playerStatus.MagicPoint < _playerStatus.MaxMagicPoint && Mathf.Floor(_playerStatus.HitPoint) > _playerStatus.MaxHitPoint / CutDownOnLifeValue)
        {
            //回復時間をリセット
            _regeneTime = 0;
            _regeneValue = 0;

            //Hpの２０分の一を減らす
            _playerStatus.HitPoint -= _playerStatus.MaxHitPoint / CutDownOnLifeValue;

            //HPバーを減らす
            _playerStatusUI.HpBarAdaptation();

            //MPを増やす
            _playerStatus.MagicPoint += 20;

            if (_playerStatus.MagicPoint > _playerStatus.MaxMagicPoint)
            {
                _playerStatus.MagicPoint = _playerStatus.MaxMagicPoint;
            }

            //MPバーに値を適応させる
            _playerStatusUI.MpBarAdaptation();

        }
    }

    /// <summary>
    /// プレイヤーのHP自然回復
    /// </summary>
    private void Regeneration()
    {
        float regenePowerUpValue = 4;

        //自然回復

        if (_regeneTime >= START_REGENE_TIME)
        {
            //徐々に回復力を上げる
            if (_regeneValue < _maxRegeneValue)
            {
                //回復
                _playerStatus.HitPoint += _regeneValue * Time.deltaTime;

                //回復力を上げる
                _regeneValue += Time.deltaTime / regenePowerUpValue;

                //体力バーの更新
                _playerStatusUI.HpBarAdaptation();
            }
            else
            {
                //回復
                _playerStatus.HitPoint += _regeneValue * Time.deltaTime;

                //体力バーの更新
                _playerStatusUI.HpBarAdaptation();
            }
        }
        else
        {
            //時間計測
            _regeneTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// プレイヤーのMP自然回復
    /// </summary>
    private void MagicpointRegeneration()
    {
        float regenePowerUpValue = 1.25f;

        if (_magicRegeneTime >= START_MAGICPOINT_REGENE_TIME)
        {
            //徐々に回復力を上げる
            if (_magicPointRegeneValue < _magicPointMaxRegeneValue)
            {
                //MP回復
                _playerStatus.MagicPoint += _magicPointRegeneValue * Time.deltaTime;

                //回復力を上げる
                _magicPointRegeneValue += Time.deltaTime / regenePowerUpValue;

                //MPバーの更新
                _playerStatusUI.MpBarAdaptation();


            }
            else
            {
                //MP回復
                _playerStatus.MagicPoint += _magicPointRegeneValue * Time.deltaTime;

                //MPバーの更新
                _playerStatusUI.MpBarAdaptation();
            }
        }
        else
        {
            //時間計測
            _magicRegeneTime += Time.deltaTime;
        }

    }

    public void GuardTakeDamage(float Damage)
    {
        //実際のダメージ
        _actualDamage = Mathf.Ceil((Damage - _playerStatus.DefensePower / DefencePowerDivideValu) * _guardValue);

        //HPを減らす
        _playerStatus.HitPoint -= _actualDamage;

        //HPバーを減らす
        _playerStatusUI.HpBarAdaptation();

        //回復までの時間を０に
        _regeneTime = 0;
        //自然回復力を初期値にする
        _regeneValue = 0;


        _playerStatusUI.GuardDamageTextAppearance(_actualDamage);
    }

    public void ProFessionIsMeleeTakeDamage(float damage)
    {
        float _meleeDamageReductionRate = 1.5f;
        //剣士によるダメージ軽減率
        float _bonusDefence = _playerStatus.DefensePower * _meleeDamageReductionRate;

        //実際に受けるダメージ
        _actualDamage = Mathf.Ceil(damage - _bonusDefence / DefencePowerDivideValu);

        //HPを減らす
        _playerStatus.HitPoint -= _actualDamage;

        //HPバーを減らす
        _playerStatusUI.HpBarAdaptation();

        //回復までの時間を０に
        _regeneTime = 0;
        //自然回復力を初期値にする
        _regeneValue = 0;

        _playerStatusUI.UsuallydDamageTextAppearance(_actualDamage);
    }

    public void ProfessionIsMagicTakeDamage(float damage)
    {
        //実際のダメージ
        _actualDamage = Mathf.Ceil(damage - _playerStatus.DefensePower / DefencePowerDivideValu);

        //HPを減らす
        _playerStatus.HitPoint -= _actualDamage;

        //HPバーを減らす
        _playerStatusUI.HpBarAdaptation();

        //回復までの時間を０に
        _regeneTime = 0;
        //自然回復力を初期値にする
        _regeneValue = 0;

        _playerStatusUI.UsuallydDamageTextAppearance(_actualDamage);
    }

    public void HitPointAndMagicPointReset()
    {
        _playerStatus.HitPoint = _playerStatus.MaxHitPoint;
        _playerStatusUI.HpBarAdaptation();

        _playerStatus.MagicPoint = _playerStatus.MaxMagicPoint;
        _playerStatusUI.MpBarAdaptation();

        _isDeath = false;

    }

    public void ReduseMagicPoint(float ReduseValue)
    {
        //MPを減らす
        _playerStatus.MagicPoint -= ReduseValue;

        //_playerStatusUI = this.gameObject.GetComponent<PlayerStatusUI>();
        //MPバーに値を適応させる
        _playerStatusUI.MpBarAdaptation();

        //MP回復までの経過時間を０にする
        _magicRegeneTime = 0f;
        //自然回復力を初期値にする
        _magicPointRegeneValue = 0f;


    }

    public void MagicPointRecovery(float recoveryValue)
    {

        //MPを減らす
        _playerStatus.MagicPoint += recoveryValue;

        if (_playerStatus.MagicPoint > _playerStatus.MaxMagicPoint)
        {
            _playerStatus.MagicPoint = _playerStatus.MaxMagicPoint;
        }

        //_playerStatusUI = this.gameObject.GetComponent<PlayerStatusUI>();
        //MPバーに値を適応させる
        _playerStatusUI.MpBarAdaptation();



        _playerStatusUI.MagicPointRecoveryTextAppearance(recoveryValue);
    }

    /// <summary>
    /// プレイヤーのHPを防御力を合わせた実質HPを渡す
    /// </summary>
    /// <returns></returns>
    public float EnemyAttackPowerAdjustment()
    {
        float returnvalue = default;

        returnvalue = _playerStatus.MaxHitPoint + Mathf.Floor(_playerStatus.DefensePower / DefencePowerDivideValu);
        return (returnvalue);
    }


    /// <summary>
    /// プレイヤーの職業別の攻撃力を渡す
    /// </summary>
    /// <returns></returns>
    public (float, int) EnemyHitPointAdjustment()
    {
        float returnValue = default;

        int profession = _playerAttack.NowProfession();

        switch (profession)
        {
            case 1: //剣士だった時
                returnValue = _playerStatus.MeleeAttackPower + _playerStatus.BasicPower;

                break;

            case 2://魔法使いだった時
                returnValue = _playerStatus.MagicattackPower + _playerStatus.BasicPower;
                break;

        }

        return (returnValue, profession);
    }

    public float GiveCriticalMultiple()
    {
        return _playerStatus.CriticalMultiple;
    }

    public float GiveMeleeAttackPower()
    {
        return _playerStatus.MeleeAttackPower;
    }

    #region アイテム取得メソッド全般

    public void AttackPowerUP(float upValue)
    {
        //基礎攻撃力up
        _playerStatus.BasicPower += upValue;
    }

    public void MeleeAttackPowerUp(float upValue)
    {
        //近接攻撃力up
        _playerStatus.MeleeAttackPower += upValue;
    }

    public void MagicAttackPowerUp(float upValue)
    {
        //基礎攻撃力up
        _playerStatus.MagicattackPower += upValue;

    }

    public void ChargeMultipleUP(float upValue)
    {
        //溜め攻撃倍率up
        _chargeMultiple += upValue;
    }

    public void hitPointUP(float upValue)
    {
        //体力up
        _playerStatus.HitPoint += upValue;
        _playerStatus.MaxHitPoint += upValue;

        _playerStatusUI.HpBarAdaptation();
    }

    public void CriticalChanceUP(float upValue)
    {
        //クリティカル率up
        _playerStatus.CriticalChance += upValue;
    }

    public void CriticalMultipleUP(float upValue)
    {
        //クリティカル倍率up
        _playerStatus.CriticalMultiple += upValue;
    }

    public void DefencePowerUP(float upValue)
    {
        //防御力up
        _playerStatus.DefensePower += upValue;
    }

    public void MagicPointUp(float upValue)
    {
        //魔力量up
        _playerStatus.MagicPoint += upValue;
        _playerStatus.MaxMagicPoint += upValue;

        _playerStatusUI.MpBarAdaptation();
    }

    #endregion
}
