using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossStatus : MonoBehaviour
{
    private float _health = default;//�̗�

    private float _maxHealth=default;
   
    private int _bodyContactDamage = default;//�̐ڐG���̍U����

    private bool _isAttackCoolDown = true;//�U���̃N�[���_�E��

    private GameObject _bossHealthBar;

    private TextMeshProUGUI _healthBarText = default;//HP�e�L�X�g

    private Scrollbar _healthBar = default;//�{�X��HP�o�[�̃X�N���[������

    #region �v���p�e�B
    //�̗͂̌��J�v���p�e�B�쐬
    public float Health
    {
        get { return _health; }

        set
        {
            _health = value;
        }
    }
    //�̐ڐG�U���͂̌��J�v���p�e�B�쐬
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
        //HP�o�[��\������
        BossHealthBar.SetActive(true);

        _maxHealth = Health;

        //�X�N���[���R���|�[�l���g�擾
        _healthBar = BossHealthBar.GetComponent<Scrollbar>();

        _healthBarText = BossHealthBar.GetComponentInChildren<TextMeshProUGUI>();

        //HP�o�[�̍X�V
        _healthBar.size = Health / _maxHealth;

        //�̗̓e�L�X�g�̍X�V
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

        //HP�o�[�̍X�V
        _healthBar.size = Health / _maxHealth;

        //�̗̓e�L�X�g�̍X�V
        _healthBarText.text = Health + "/" + _maxHealth;
    }

    /// <summary>
    /// �{�X�̍U���ԍ����o��
    /// </summary>
    /// <param name="maxAttackNumber">�e�{�X�U���̎�ނ̍ő�l</param>
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
