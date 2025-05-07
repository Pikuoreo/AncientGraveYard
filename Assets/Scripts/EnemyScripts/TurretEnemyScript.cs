using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TurretEnemyScript : MonoBehaviour
{
    [Header("���˂���e"),SerializeField] private GameObject _burret = default;
    [Header("Mp�񕜃A�C�e��"), SerializeField] private GameObject _magicPointStar = default;
    [SerializeField] private GameObject _player = default;
  
    private GameObject _burretClone = default;//���˂����e

    private float _fireTimeValue = 0; //���b��������
    private float _fireTime = 5;//���˂܂ł̎���
    private float _burretRange = 15f;//�˒�����

    [Header("�G��HP"),SerializeField] private float _hitPoint = 210;
    [Header("�G�̍U���͂̍Œ�l"), SerializeField] private float _attackMinPower = 0f;
    [Header("�G�̍U���͂̍ō��l"), SerializeField] private float _attackMaxPower = 0f;
    [Header("�ς��Ăق����U����"), SerializeField] private float NumberOfAttacks = default;

    [SerializeField] private AudioSource _turretAudio = default;
    [SerializeField] private AudioClip _burretSE = default;

    private PlayerStatusChange _playerStatusScript = default;

    private void Start()
    {
        _turretAudio=this.GetComponent<AudioSource>();
    }
    void Update()
    {
        float playerdistanceX = Mathf.Abs(_player.transform.position.x - this.transform.position.x);
        float playerdistanceY = Mathf.Abs(_player.transform.position.y - this.transform.position.y);

        if (_fireTimeValue >= _fireTime&&playerdistanceX<_burretRange&& playerdistanceY < _burretRange)
        {
            Fire();
        }
        else if(_fireTimeValue < _fireTime)
        {
            _fireTimeValue += Time.deltaTime;

        }

        if (_hitPoint <= 0)
        {
            Death();
        }
    }

    private void Fire()
    {
        _turretAudio.PlayOneShot(_burretSE);
        _fireTimeValue = 0;
        _burretClone = Instantiate(_burret, this.transform.position, Quaternion.FromToRotation(Vector3.up, this.transform.position - _player.transform.position));
        _burretClone.GetComponent<TurrentBurretScript>().GetAttackPower(_attackMinPower, _attackMaxPower);
    }

    public void BeAttacked(float Damage)
    {
        _hitPoint -= Damage;
    }

    public void StatusIncrease(float hitPointincreasedValue, float attackPowerIncreasedValue)
    {
        //�[�x�ɂ��킹�ăX�e�[�^�X������
        _hitPoint *= hitPointincreasedValue;
        _attackMinPower *= attackPowerIncreasedValue;
        _attackMaxPower *= attackPowerIncreasedValue;

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerStatusScript = _player.GetComponent<PlayerStatusChange>();
        StatusAdjustment();
    }

    private void StatusAdjustment()
    {
        //�v���C���[�̎���HP 
        float _playerHP = _playerStatusScript.EnemyAttackPowerAdjustment();

        if (_playerHP < _attackMaxPower * NumberOfAttacks)
        {
            //�ő�U���͂̓v���C���[�̗̑͂̉�������
            float firstCalculation = _attackMaxPower / _playerHP;

            //�ő�U���͂�firstCalculation�ŏo�����l���|����
            float secondCalculation = _attackMaxPower * firstCalculation;

            //secondCalculation�ŏo�����l��ς��Ăق����U���񐔂Ŋ���
            float thirdCalculation = Mathf.Floor(secondCalculation / NumberOfAttacks);

            //���g�̍U���͂�thirdCalculation�ŏo����������
            _attackMinPower -= thirdCalculation;
            _attackMaxPower -= thirdCalculation;

        }


        //�v���C���[�̍U���͂��擾
        float playerAttackvalue = _playerStatusScript.EnemyHitPointAdjustment().Item1;

        //�v���C���[�̍��̐E�Ƃ��擾
        int playerProfession = _playerStatusScript.EnemyHitPointAdjustment().Item2;

        switch (playerProfession)
        {
            //���m�̎�
            case 1:

                //�v���C���[���U��U�����Ă��|���Ȃ�HP�Ȃ�
                if (playerAttackvalue * 6 < _hitPoint)
                {

                    float lestHitPoint = default;

                    //�U��U���������̎c��HP���v�Z
                    lestHitPoint = _hitPoint - playerAttackvalue * 6;

                    //�v�Z���ʂ̒l���Q����
                    _hitPoint -= Mathf.Floor(lestHitPoint / 2);
                }

                break;

            //���@�g���̎�
            case 2:

                //�v���C���[��8��U�����Ă��|���Ȃ�HP�Ȃ�
                if (playerAttackvalue * 8 < _hitPoint)
                {
                    float lestHitPoint = default;

                    //�U��U���������̎c��HP���v�Z
                    lestHitPoint = _hitPoint - playerAttackvalue * 6;

                    //�v�Z���ʂ̒l���Q����
                    _hitPoint -= Mathf.Floor(lestHitPoint / 2);
                }

                break;
        }

    }
    public void Death()
    {
        float minRandomValue = 0;
        float maxRandomValue = 101;
        float halfValue = 3;

        float randomValue = Random.Range(minRandomValue, maxRandomValue);

        //3���̈��Mp�񕜃A�C�e�����o��
        if (randomValue <= maxRandomValue / halfValue)
        {
            Instantiate(_magicPointStar, this.transform.transform.position, Quaternion.identity);
        }

        //���񂾂��Ƃ��X�e�[�W�R�A�ɓ`����
        this.GetComponentInParent<SummonEnemyScript>().BeDefeated();
        Destroy(this.gameObject);
    }
}
