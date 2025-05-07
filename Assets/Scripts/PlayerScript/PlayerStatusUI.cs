using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{

    #region ステータス欄用
    [Header("プレイヤーの体力"), SerializeField] private Text _hitPointText = default;
    [Header("プレイヤーのMP"), SerializeField] private Text _magicPointText = default;
    [Header("プレイヤーの基礎攻撃力"), SerializeField] private Text _basicPowerText = default;
    [Header("プレイヤーのクリティカル率"), SerializeField] private Text _criticalChanceText = default;

    [Header("プレイヤーのクリティカル倍率"), SerializeField] private Text _criticalMultipleText = default;
    [Header("プレイヤーの近接攻撃力"), SerializeField] private Text _meleeAttackPowerText = default;
    [Header("プレイヤーの魔法攻撃力"), SerializeField] private Text _magicAttackPowerText = default;
    [Header("プレイヤーの防御力"), SerializeField] private Text _defenceText = default;
    #endregion

    #region Hpバー、Mpバーに関するもの

    [SerializeField] private Scrollbar _hpBar = default;
    [SerializeField] private Scrollbar _mpBar = default;

    [SerializeField] private TextMeshProUGUI _hpText = default;//残りHpのテキスト
    [SerializeField] private TextMeshProUGUI _mpText = default;//残りMpのテキスト
    [SerializeField] private TextMeshProUGUI _damageText = default;//ダメージ値を表示するテキスト

    #endregion

    [SerializeField] private GameObject _worldSpaceCanvas = default;//ワールド依存のキャンバス

    private TextMeshProUGUI _damageTextDummy = default;//ダメージ値を表示するテキストの生成したオブジェクト

    private PlayerMoveControll _playerMoveControll = default;

    [SerializeField] private PlayerStatusManegement _playerStatus = default;

    // Start is called before the first frame update
    void Start()
    {
        _playerMoveControll=this.GetComponent<PlayerMoveControll>();

        HpBarAdaptation();
        MpBarAdaptation();
    }

    /// <summary>
    /// アイテム欄を開く
    /// </summary>
    public void ListOpen()
    {
        Time.timeScale = 0;

        if (_playerMoveControll.AliveJudge())
        {
            //体力ステータス
            string hpPremise = "最大体力：";
            _hitPointText.text = hpPremise + _playerStatus.MaxHitPoint;

            //MPステータス
            string mpPremise = "MP；";
            _magicPointText.text = mpPremise + _playerStatus.MaxMagicPoint;

            //基礎攻撃力ステータス
            string basicPowerPremise = "基礎攻撃力：";
            _basicPowerText.text = basicPowerPremise + _playerStatus.BasicPower;

            //クリティカル率ステータス
            string criticalChancePremise = "クリティカル率：";
            _criticalChanceText.text = criticalChancePremise + _playerStatus.CriticalChance + "%";

            //クリティカル倍率ステータス
            string criticalMultiplePremise = "クリティカル倍率：";
            _criticalMultipleText.text = criticalMultiplePremise + _playerStatus.CriticalMultiple + "%";

            //近接攻撃力ステータス
            string meleePowerPremise = "近接攻撃力：";
            _meleeAttackPowerText.text = meleePowerPremise + _playerStatus.MeleeAttackPower;

            //魔法攻撃力ステータス
            string magicPowerPremise = "魔法攻撃力：";
            _magicAttackPowerText.text = magicPowerPremise + _playerStatus.MagicattackPower;

            //防御力ステータス
            string defencePowerPremise = "防御力：";
            _defenceText.text = defencePowerPremise + _playerStatus.DefensePower;
        }
    }

    /// <summary>
    /// アイテム欄を閉じる
    /// </summary>
    public void ListClosed()
    {
        Time.timeScale = 1;
    }


    public void HpBarAdaptation()
    {
        //hpバーの適応
        _hpBar.size = _playerStatus.HitPoint / _playerStatus.MaxHitPoint;

        HitPointTextchange();
    }

    public void MpBarAdaptation()
    {
        //Mpバーの適応
        _mpBar.size = _playerStatus.MagicPoint / _playerStatus.MaxMagicPoint;

        MagicPointTextchange();
    }

    private void HitPointTextchange()
    {

        string hpText = "Hp:";
        //Hpテキストの適応
        _hpText.text = hpText  + _playerStatus.HitPoint.ToString("f0") + "/" + _playerStatus.MaxHitPoint;

    }

    private void MagicPointTextchange()
    {
        string mpText = "Mp:";
        //Mpテキストの適応
        _mpText.text = mpText  + _playerStatus.MagicPoint.ToString("f0") + "/" + _playerStatus.MaxMagicPoint;
        
    }

    public void ParryDamageTextAppearance()
    {
        string ParryText = "Parry!!";
        //トリガーが当たった位置に生成する
        TextMeshProUGUI _damageTextDummy = Instantiate(_damageText, this.transform.position, Quaternion.identity);

        //キャンバスを親にする
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        //ダメージ値をテキストに入れる
        _damageTextDummy.text = ParryText;

        //テキストを動かす
        _damageTextDummy.GetComponent<DamageTextScript>().TextParryMove();
    }
    public void GuardDamageTextAppearance(float damage)
    {
        //トリガーが当たった位置に生成する
        _damageTextDummy = Instantiate(_damageText, this.gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        //キャンバスを親にする
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        string _guardText = "Guard!!";

        //ダメージ値をテキストに入れる
        _damageTextDummy.text = _guardText  + " " + damage.ToString();

        _damageTextDummy.GetComponent<DamageTextScript>().TextDefaultMove();
    }

    public void UsuallydDamageTextAppearance(float damage)
    {
        //トリガーが当たった位置に生成する
        _damageTextDummy = Instantiate(_damageText, this.gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        //キャンバスを親にする
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        //ダメージ値をテキストに入れる
        _damageTextDummy.text = damage.ToString();

        _damageTextDummy.GetComponent<DamageTextScript>().TextDefaultMove();
    }

    public void MagicPointRecoveryTextAppearance(float recoveryValue)
    {
        //自分の位置に生成する
        _damageTextDummy = Instantiate(_damageText,this.transform.position, Quaternion.identity);

        //キャンバスを親にする
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        //回復値をテキストに入れる
        _damageTextDummy.text = recoveryValue.ToString();

        _damageTextDummy.GetComponent<DamageTextScript>().TextMagicPointRecoveryMove();
    }
}
