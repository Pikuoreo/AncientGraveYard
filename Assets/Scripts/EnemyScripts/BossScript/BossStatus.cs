using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossStatus : MonoBehaviour
{
    private float _health = default;//体力

    private float _maxHealth=default;
   
    private int _bodyContactDamage = default;//体接触時の攻撃力

    private bool _isAttackCoolDown = true;//攻撃のクールダウン

    private GameObject _bossHealthBar;

    private TextMeshProUGUI _healthBarText = default;//HPテキスト

    private Scrollbar _healthBar = default;//ボスのHPバーのスクロール部分

    #region プロパティ
    //体力の公開プロパティ作成
    public float Health
    {
        get { return _health; }

        set
        {
            _health = value;
        }
    }
    //体接触攻撃力の公開プロパティ作成
    public int BodyContactDamage
    {
        get { return _bodyContactDamage; }
        set { _bodyContactDamage = value; }
    }

    public bool IsAttackCoolDown
    {
        get { return _isAttackCoolDown; }
        set { _isAttackCoolDown = value; }
    }

    public GameObject BossHealthBar
    {
        get { return _bossHealthBar; }
        set { _bossHealthBar = value; }
    }

    #endregion

    public void HealthBarDisplay()
    {
        //HPバーを表示する
        BossHealthBar.SetActive(true);

        _maxHealth = Health;

        //スクロールコンポーネント取得
        _healthBar = BossHealthBar.GetComponent<Scrollbar>();

        _healthBarText = BossHealthBar.GetComponentInChildren<TextMeshProUGUI>();

        //HPバーの更新
        _healthBar.size = Health / _maxHealth;

        //体力テキストの更新
        _healthBarText.text = Health + "/" + _maxHealth;

        int _coolDownTime = 1;

        if (this.GetComponent<BoxCollider2D>())
        {
            this.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if(this.GetComponent<CapsuleCollider2D>())
        {
            this.GetComponent<CapsuleCollider2D>().enabled = true;
        }

        

        StartCoroutine(AttackCoolDown(_coolDownTime));
    }

    public virtual void Death()
    {
        GetComponentInParent<StageCoreScript>().ButtleEnd();

       BossHealthBar.SetActive(false);

        Destroy(this.gameObject);
    }

    public virtual void TakeDamage(float ReduseValue)
    {
        _health -= ReduseValue;

        //HPバーの更新
        _healthBar.size = Health / _maxHealth;

        //体力テキストの更新
        _healthBarText.text = Health + "/" + _maxHealth;
    }

    /// <summary>
    /// ボスの攻撃番号を出す
    /// </summary>
    /// <param name="maxAttackNumber">各ボス攻撃の種類の最大値</param>
    /// <returns></returns>
    public int ChooseAttack(int maxAttackNumber)
    {
        int minAttackNumber = 1;
        int AttackNumber = Random.Range(minAttackNumber, maxAttackNumber);
        return AttackNumber;
    }

    public virtual IEnumerator AttackCoolDown(float coolDowntime)
    {
        yield return new WaitForSeconds(coolDowntime);
        IsAttackCoolDown = false;
    }

    public void BeAttacked(GameObject player)
    {
        player.GetComponent<PlayerAttack>().TakeDamage(BodyContactDamage, this.gameObject, false, true);
    }
}
