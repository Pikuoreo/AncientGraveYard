using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyScript : MonoBehaviour
{
    [Header("�G�̍U���͂̍Œ�l"), SerializeField] private float _attackMinPower = 0f;
    [Header("�G�̍U���͂̍ō��l"), SerializeField] private float _attackMaxPower = 0f;
    [Header("�̗�"), SerializeField] private float _hitPoint = default;
    [Header("�m�b�N�o�b�N�̑傫��"), SerializeField] private float _knockBackValue = 0f;
    [Header("�ς��Ăق����U����"), SerializeField] private float NumberOfAttacks = default;

    [Header("Mp�񕜃A�C�e��"), SerializeField] private GameObject _magicPointStar = default;

    private GameObject _player = default;

    private PlayerStatusChange _playerStatusScript = default;

    private Rigidbody2D _flyEnemyRb = default;//���g�̃��W�b�g�{�f�B

    private float _horizontalSpeed = 0.1f;//���̃X�s�[�h
    private float _varticalSpeed = 0.1f;//�c�̃X�s�[�h

    private bool _isLeftRight = true;//false�͉E�ړ��Atrue�͍��ړ�
    private bool _isUpDown = true;//false�͏�ړ��Atrue�͉��ړ�
    private bool _isStun = false;
    private bool _isRightKnockBack = false;//�E�m�b�N�o�b�N����
    private bool _isLeftKnockBack = false;//���m�b�N�o�b�N����

    private const float TIME_DELTATIME = 500f;
    private const float MAX_HORIZONTAL_SPEED = 5f;//���̍ő�X�s�[�h
    private const float MAX_VARTICAL_SPEED = 1.5f;//�c�̍ő�X�s�[�h

    // Start is called before the first frame update
    void Start()
    {
        _flyEnemyRb = this.GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        #region �ړ�����
        if (_isStun)
        {
            NotMoveProcess();
        }
        else if (_player != null)
        {
            //�ʏ�̈ړ�
            DefaultMoveProcess();
        }

        #endregion

        //�̗͂��O�ɂȂ����玀�S����
        if (_hitPoint <= 0)
        {
            Death();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string _playerTag = "Player";
        //�v���C���[�ɓ���������
        if (collision.gameObject.CompareTag(_playerTag))
        {
            //�����_���[�W��^����
            float attackPower = Random.Range(_attackMinPower, _attackMaxPower);
            collision.gameObject.GetComponent<PlayerAttack>().TakeDamage(attackPower, this.gameObject, true, true);
        }

        int StagePartsLayer = 12;
        int FloorLayer = 18;

        //�X�e�[�W�̘g�g�݂ɓ���������
        if(collision.gameObject.layer==StagePartsLayer|| collision.gameObject.layer == FloorLayer)
        {
            StagePartsCollision(collision.gameObject);
        }
      
    }

    /// <summary>
    /// �㉺���E�ړ����\�b�h
    /// </summary>
    private void DefaultMoveProcess()
    {
        #region ���E�ړ�
        //���������E�ɂ�����
        if (_player.transform.position.x >= this.transform.position.x)
        {
            //�ϐ����E�����p�ɂ���
            if (_isLeftRight)
            {
                _isLeftRight = false;
                _horizontalSpeed /= 2;
                this.transform.rotation = new Quaternion(0, 180, 0, 0);
            }


            //�ړ��̑��������̑����܂ŒB���Ă�����
            if (_flyEnemyRb.velocity.x >= MAX_HORIZONTAL_SPEED)
            {
                //���̑��x�ŉE�Ɉړ�
                _flyEnemyRb.velocity = new Vector2(MAX_HORIZONTAL_SPEED, _flyEnemyRb.velocity.y);
            }
            else
            {
                //���X�ɉE�ړ��̃X�s�[�h���グ��
                _flyEnemyRb.velocity = new Vector2(_horizontalSpeed, _flyEnemyRb.velocity.y);
                _horizontalSpeed += Time.deltaTime * 7.5f;
            }
        }
        //������荶�ɂ�����
        else
        {
            //�ϐ����������p�ɂ���
            if (!_isLeftRight)
            {
                _horizontalSpeed /= 2;
                _isLeftRight = true;
                this.transform.rotation = new Quaternion(0, 0, 0, 0);
            }

            //�ړ��̑��������̑����܂ŒB���Ă�����
            if (_flyEnemyRb.velocity.x <= -MAX_HORIZONTAL_SPEED)
            {
                //���̑��x�ō��Ɉړ�
                _flyEnemyRb.velocity = new Vector2(-MAX_HORIZONTAL_SPEED, _flyEnemyRb.velocity.y);
            }
            else
            {
                //���X�ɍ��ړ��̃X�s�[�h���グ��
                _flyEnemyRb.velocity = new Vector2(_horizontalSpeed, _flyEnemyRb.velocity.y);
                _horizontalSpeed -= Time.deltaTime * 7.5f;
            }
        }
        #endregion

        #region �㉺�ړ�
        //����������ɂ�����
        if (_player.transform.position.y >= this.transform.position.y)
        {
            //�ϐ���������p�ɂ���
            if (_isUpDown)
            {
                _isUpDown = false;
                _varticalSpeed /= 1.5f;
            }

            //�ړ��̑��������̑����܂ŒB���Ă�����
            if (_flyEnemyRb.velocity.y >= MAX_VARTICAL_SPEED)
            {
                //���̑��x�ŏ�Ɉړ�
                _flyEnemyRb.velocity = new Vector2(_flyEnemyRb.velocity.x, MAX_VARTICAL_SPEED);
            }
            else
            {

                //���X�ɏ�ړ��̃X�s�[�h���グ��
                _flyEnemyRb.velocity = new Vector2(_flyEnemyRb.velocity.x, _varticalSpeed);
                _varticalSpeed += Time.deltaTime * 3.5f;
            }
        }
        //������艺�ɂ�����
        else
        {
            //�ϐ����������p�ɂ���
            if (!_isUpDown)
            {
                _varticalSpeed /= 1.5f;
                _isUpDown = true;
            }

            //�ړ��̑��������̑����܂ŒB���Ă�����
            if (_flyEnemyRb.velocity.y <= -MAX_VARTICAL_SPEED)
            {
                //���̑��x�ŏ�Ɉړ�
                _flyEnemyRb.velocity = new Vector2(_flyEnemyRb.velocity.x, -MAX_VARTICAL_SPEED);
            }
            else
            {
                //���X�ɏ�ړ��̃X�s�[�h���グ��
                _flyEnemyRb.velocity = new Vector2(_flyEnemyRb.velocity.x, _varticalSpeed);
                _varticalSpeed -= Time.deltaTime * 3.5f;
            }
        }
        #endregion
    }

    private void NotMoveProcess()
    {
        //�ړ����Ă��Ȃ����A�E�Ƀm�b�N�o�b�N��
        if (_isRightKnockBack)
        {
            if (_flyEnemyRb.velocity.x > 0)
            {
                //���x�����X�ɖ߂�
                _flyEnemyRb.AddForce(new Vector2(MAX_HORIZONTAL_SPEED * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //�m�b�N�o�b�N�I��
                _isRightKnockBack = false;
            }
        }
        //�ړ����Ă��Ȃ����A���Ƀm�b�N�o�b�N��
        else if (_isLeftKnockBack)
        {
            if (_flyEnemyRb.velocity.x < 0)
            {
                //���x�����X�ɖ߂�
                _flyEnemyRb.AddForce(new Vector2(MAX_HORIZONTAL_SPEED * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //�m�b�N�o�b�N�I��
                _isLeftKnockBack = false;
            }
        }
    }

    /// <summary>
    /// ���炩�̃X�e�[�W�p�[�c�ɓ��������ۂ̈ړ����]
    /// </summary>
    /// <param name="collisionParts"></param>
    private void StagePartsCollision(GameObject collisionParts)
    {
        float BoundValue = -0.75f;

        //�ǂ̃I�u�W�F�N�g�ɓ���������
        string wallTag = "Wall";
        if (collisionParts.CompareTag(wallTag))
        {
            //���Ɍ����Ă�����̔��Α��Ƀo�E���h������
            _horizontalSpeed *= BoundValue;
        }

        //���A�V��̃I�u�W�F�N�g�ɓ���������
        string cellingTag = "Celling";
        string floorTag = "Flor";
        if (collisionParts.CompareTag(cellingTag) || collisionParts.CompareTag(floorTag))
        {
            //���Ɍ����Ă�����̔��Α��Ƀo�E���h������
            _varticalSpeed *= BoundValue;
        }
    }

    /// <summary>
    /// ��_���[�W����
    /// </summary>
    /// <param name="Damage">��炤�_���[�W��</param>
    /// <param name="isKnockBackAttack">�m�b�N�o�b�N����U����</param>
    public void BeAttacked(float Damage , bool isKnockBackAttack)
    {
        //hp�����炷
        _hitPoint -= Damage;

        if (isKnockBackAttack)
        {
            //���Ɍ����Ă�����̔��Α��Ƀo�E���h������
            _flyEnemyRb.velocity = Vector2.zero;
            float BoundValue = -0.75f;
            _horizontalSpeed *= BoundValue;
        }
       
    }

    /// <summary>
    /// ���S�����Ƃ��̏���
    /// </summary>
    private void Death()
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

    /// <summary>
    /// �C��f�o�t���t�^���ꂽ���̏���
    /// </summary>
    public void Stun()
    {
        //��莞�ԍs���s�\
        _isStun = true;
        _flyEnemyRb.velocity = Vector2.zero;
        _flyEnemyRb.AddForce(transform.right * _knockBackValue, ForceMode2D.Impulse);
        StartCoroutine(EndStun());

        //�v���C���[�ɓ�����Ȃ�����
        int DontHitEnemyLayer = 20;
        this.gameObject.layer = DontHitEnemyLayer;
    }


    /// <summary>
    /// �K�w�ɂ��X�e�[�^�X�̏㏸
    /// </summary>
    /// <param name="hitPointincreasedValue">�̗͂�������{��</param>
    /// <param name="attackPowerIncreasedValue">�U���͂�������{��</param>
    public void StatusIncrease(float hitPointincreasedValue, float attackPowerIncreasedValue)
    {
        _hitPoint *= hitPointincreasedValue;
        _attackMinPower *= attackPowerIncreasedValue;
        _attackMaxPower *= attackPowerIncreasedValue;

        _player = GameObject.FindGameObjectWithTag("Player");

        _playerStatusScript = _player.GetComponent<PlayerStatusChange>();

        StatusAdjustment();
    }


    /// <summary>
    /// �v���C���[�Ƃ̃X�e�[�^�X�ɍ����������Ƃ��A���g�̃X�e�[�^�X���኱������
    /// </summary>
    private void StatusAdjustment()
    {
        #region �U���͒���
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
        #endregion

        #region�@�̗͒���
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
        #endregion
    }


    /// <summary>
    /// �X�^���I���̏���
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndStun()
    {
        _horizontalSpeed = 0;
        _varticalSpeed = 0;
        float stunTime = 2f;
        yield return new WaitForSeconds(stunTime);
        _isStun = false;

        //�v���C���[�ɓ�����悤�ɂ���
        int EnemyLayer = 22;
        this.gameObject.layer = EnemyLayer;

    }
}
