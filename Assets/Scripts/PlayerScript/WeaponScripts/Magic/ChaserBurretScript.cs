using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ChaserBurretScript : MonoBehaviour
{
    private float _burretPower = 0f;//�e�̈З�
    private float _burretAliveTime = 3f;//�e�̎�������
    private float _moveSpeed = 10f;

    private int _weaponEvolution = default;//����̐i���`��
    private int DamageSeNumber = default;
    private int NumberOfAttacks = 0;

    private List<GameObject> _chaseEnemys = new List<GameObject>();//�ǐՑΏ�
    private GameObject _worldSpaceCanvas = default;//�_���[�W�e�L�X�g�\���̂��߂̃L�����o�X

    private bool _isChase = false;//�ǐՒ�����������Ȃ���
    private bool _isCritical = false;//�N���e�B�J�����ǂ���

    private Vector3 _comparisonPosition = new Vector3();

    [SerializeField] private TextMeshProUGUI _damageText = default;//�_���[�W�e�L�X�g

    [SerializeField] private List<AudioClip> enemyDamageSE = new List<AudioClip>();//�U���������̃T�E���h

    private ChaserCaneAttack ParentWeaponscript = default;//��ɂ��Ă�X�N���v�g

    private const int ADDITIONAL_ATTACK_VALUE = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeUp());

        string findTag = "WorldSpaceCanvas";
        _worldSpaceCanvas = GameObject.FindGameObjectWithTag(findTag);

        //�e�o���ʒu����
        _comparisonPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isChase && _chaseEnemys[0] !=null)
        {
            Chase();
        }
        else
        {
            NotChase();
        }
       
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
        int stagePartsLayer = 12;
        int florLayer = 18;

        //�X�e�[�W�ɓ���������
        if (collision.gameObject.layer == stagePartsLayer || collision.gameObject.layer == florLayer)
        {
            //�e������
            AdditionalAttackCheck(false,collision.gameObject.tag);
        }

        int enemyLayer = 10;
        int donthitEnemyLayer = 20;
        int FlyEnemyLayer = 22;

        //�G�ɍU��������������
        if(collision.gameObject.layer==enemyLayer ||
            collision.gameObject.layer==donthitEnemyLayer||
            collision.gameObject.layer == FlyEnemyLayer)
        {
            _isChase = false;
            //�_���[�W���e�L�X�g�\��
            DamageTextDisplay(collision.contacts[0].point);

            if (collision.gameObject.CompareTag("NormalEnemy"))
            {
                collision.gameObject.GetComponent<NormalEnemyScript>().BeAttacked(_burretPower, 0,false);
                DamageSeNumber = 1;
            }
            else if (collision.gameObject.CompareTag("TurretEnemy"))
            {
                collision.gameObject.GetComponent<TurretEnemyScript>().BeAttacked(_burretPower);
                DamageSeNumber = 2;
            }
            else if (collision.gameObject.CompareTag("ChaserEnemy"))
            {
                collision.gameObject.GetComponent<ChaserEnemyScript>().BeAttacked(_burretPower, 0, false);
                DamageSeNumber = 3;
            }
            else if (collision.gameObject.CompareTag("FlyEnemy"))
            {
                collision.gameObject.GetComponent<FlyEnemyScript>().BeAttacked(_burretPower, true);
                DamageSeNumber = 3;
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                collision.gameObject.GetComponent<IBossStatus>().TakeDamage(_burretPower);
                DamageSeNumber = 3;
            }
            ParentWeaponscript.ReProductionSE(enemyDamageSE[DamageSeNumber]);
            //���킪�i�����Ă�����
            AdditionalAttackCheck(true,collision.gameObject.tag);
        }
    }

    private void Chase()
    {
        //�G��ǔ�
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, _chaseEnemys[0].transform.position - this.transform.position);
        this.transform.position += transform.up * Time.deltaTime * _moveSpeed;
    }

    private void NotChase()
    {
        //�܂��������ł���
        this.transform.position += transform.up * Time.deltaTime * _moveSpeed;
    }

    /// <summary>
    /// �e�̒ǉ��U��
    /// </summary>
    /// <param name="CollisionObjectIsEnemy">�G���X�e�[�W�p�[�c���@true�̓X�e�[�W�p�[�c�Afalse�̓X�e�[�W�p�[�c</param>
    private void AdditionalAttackCheck(bool CollisionObjectIsEnemy,string collisionObjectTag)
    {
        if (CollisionObjectIsEnemy)
        {
            //���킪��x�ł��i���ς݂��A�U���񐔂��O��������
            if (_weaponEvolution >= ADDITIONAL_ATTACK_VALUE&&NumberOfAttacks==0)
            {
                AdditionalAttack();
            }
            //�i�����Ă��Ȃ�������
            else
            {
                DestroyObject();
            }
        }
        else
        {
            //���킪��x�ł��i���ς݂�������
            if (_weaponEvolution >= ADDITIONAL_ATTACK_VALUE)
            {
                CollisionStageParts(collisionObjectTag);
            }
            //�i�����Ă��Ȃ�������
            else
            {
                DestroyObject();
            }
        }
    }

    private void AdditionalAttack()
    {
        switch (_weaponEvolution)
        {
            //�i�����x���P�̎�
            case 1:
                WeaponEvolution1AdditionalAttack();
                break;
        }
    }

   
    private void WeaponEvolution1AdditionalAttack()
    {
        NumberOfAttacks++;
        this.transform.Rotate(0, 0, 180);
        _comparisonPosition=this.transform.position;

        //�ǔ�����G�����Z�b�g
        _chaseEnemys.Clear();
    }

    private void CollisionStageParts(string stagePartsTag)
    {
        string cellingTag = "Celling";
        string florTag = "Flor";
        string wallTag = "Wall";
        //���A�V��ɓ������Ă�����
        if (stagePartsTag == cellingTag || stagePartsTag == florTag)
        {
            FlorOrCellingCollision();
        }
        //�ǂɓ������Ă�����
        else if (stagePartsTag == wallTag)
        {
            WallCollision();
        }
    }

    private void FlorOrCellingCollision()
    {
        int clockWise = -90;//���v���
        int counterClockWise = 90;//�����v���

        //�O��Ȃ������ꏊ����i�񂾕������v�Z
        Vector3 _burretDerectionFacing = this.transform.position - _comparisonPosition;
        //�Ȃ������ꏊ�̍X�V
        _comparisonPosition = this.transform.position;
        //�E��ړ�
        if (_burretDerectionFacing.x > 0 && _burretDerectionFacing.y > 0)
        {
            //���v���ɂX�O�x��]
            this.transform.Rotate(0, 0, clockWise);
        }
        //�E���ړ�
        else if (_burretDerectionFacing.x > 0 && _burretDerectionFacing.y < 0)
        {
            //�����v���ɂX�O�x��]
            this.transform.Rotate(0, 0, counterClockWise);
        }
        //����ړ�
        else if (_burretDerectionFacing.x < 0 && _burretDerectionFacing.y > 0)
        {
            //�����v���ɂX�O�x��]
            this.transform.Rotate(0, 0, counterClockWise);
        }
        //�����ړ�
        else
        {
            //���v���ɂX�O�x��]
            this.transform.Rotate(0, 0, clockWise);
        }
    }

    private void WallCollision()
    {
        int clockWise = -90;//���v���
        int counterClockWise = 90;//�����v���

        //�O��Ȃ������ꏊ����i�񂾕������v�Z
        Vector3 _burretDerectionFacing = this.transform.position - _comparisonPosition;

        //�Ȃ������ꏊ�̍X�V
        _comparisonPosition = this.transform.position;
        //�E��ړ�
        if (_burretDerectionFacing.x > 0 && _burretDerectionFacing.y > 0)
        {
            //�����v���ɂX�O�x��]
            this.transform.Rotate(0, 0, counterClockWise);
        }
        //�E���ړ�
        else if (_burretDerectionFacing.x > 0 && _burretDerectionFacing.y < 0)
        {
            //���v���ɂX�O�x��]
            this.transform.Rotate(0, 0, clockWise);
        }
        //����ړ�
        else if (_burretDerectionFacing.x < 0 && _burretDerectionFacing.y > 0)
        {
            //���v���ɂX�O�x��]
            this.transform.Rotate(0, 0, clockWise);
        }
        //�����ړ�
        else
        {
            //�����v���ɂX�O�x��]
            this.transform.Rotate(0, 0, counterClockWise);
        }
    }

  
    private void DamageTextDisplay(Vector3 DisplayPoint)
    {
        //�g���K�[�����������ʒu�ɐ�������
        TextMeshProUGUI _damageTextDummy = Instantiate(_damageText, DisplayPoint, Quaternion.identity);
        //�L�����o�X��e�ɂ���
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        //�_���[�W�l���e�L�X�g�ɓ����
        _damageTextDummy.text = (_burretPower).ToString();

        //�N���e�B�J��������������
        if (_isCritical)
        {
            //�e�L�X�g��Ԃɂ���
            _damageTextDummy.GetComponent<DamageTextScript>().TextCriticalMove();
        }
        //���Ȃ�������
        else
        {
            //�e�L�X�g�𔒂ɂ���
            _damageTextDummy.GetComponent<DamageTextScript>().TextDefaultMove();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int enemyLayer = 10;
        int dontHitEnemyLayer = 20;
        int flyEnemyLayer = 22;

        //�g���K�[�������̂��G��������
        if (collision.gameObject.layer == enemyLayer|| 
            collision.gameObject.layer == dontHitEnemyLayer||
            collision.gameObject.layer == flyEnemyLayer)
        {
            //�ǔ�����G���X�g�ɒǉ�
            _chaseEnemys.Add(collision.gameObject);
            //�ǔ��J�n
            _isChase = true;
        }
    }

    public void GetPower(float AttackPower, ChaserCaneAttack weaponScript,bool isCriticalJudge, int weaponEvolutionNumber)
    {
        _burretPower = AttackPower;
        ParentWeaponscript = weaponScript;
        _isCritical = isCriticalJudge;

        _weaponEvolution = weaponEvolutionNumber;
        
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator TimeUp()
    {
        yield return new WaitForSeconds(_burretAliveTime);
        DestroyObject();
        DamageSeNumber = 0;
        ParentWeaponscript.ReProductionSE(enemyDamageSE[DamageSeNumber]);
    }
}
