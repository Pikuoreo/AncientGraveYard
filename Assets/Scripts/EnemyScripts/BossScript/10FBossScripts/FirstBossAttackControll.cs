using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class FirstBossAttackControll : BossStatus,IBossStatus
{
    private const int MAX_HEALTH = 7500;//�{�X�̗̑͒�`
    private const int BODY_CONTACT_DAMAGE = 35;//�{�X�̐ڐG�_���[�W��`
    private const int MAX_ATTACK_NUMBER = 6;//�ő�̍U���ԍ�+1

    private int _attackNumber = 0;
    private int _attackObjectNumber = 0;


    private bool _isPositionAdjustment = false;//true�ŃA�j���[�V�����ł��ꂽ�|�W�V��������
    private bool _isFixed = false; //���̌Œ�
    private bool _isLeftMove = false;
    private bool _isRightMove = false;
    private bool _isDeath = false;

    private float _stompposition = default;//�X�g���v����ʒu
    private float _elapsedTime = 0;//�o�ߎ���
    private float _burretAttackWaittime = 2f; //�e�ːi�U���̑҂�����

    private string _animationName = default;

    [SerializeField] private List<GameObject> _wallTentacle = new List<GameObject>();//�U���ԍ��P�̃I�u�W�F�N�g
    [SerializeField] private List<GameObject> _cellingTentacle = new List<GameObject>();//�U���ԍ��Q�̃I�u�W�F�N�g
    [SerializeField] private List<GameObject> _magicBurret = new List<GameObject>(); //�U���ԍ��R�̃I�u�W�F�N�g
    [SerializeField] private GameObject _chaserBossBurret = default;//�U���ԍ��S�̃I�u�W�F�N�g

    [SerializeField] private GameObject _exitHoleObject = default;//�X�e�[�W��̏o���I�u�W�F�N�g
    [SerializeField] private GameObject _healthBarObject = default;//�X�e�[�W��̃{�X�̗̑̓o�[�I�u�W�F�N�g

    [SerializeField] private GameObject _rightWall = default;//�X�e�[�W��̓V��
    [SerializeField] private GameObject _celling = default;//�X�e�[�W��̉E�̕�

    private GameObject _player = default;
    private GameObject _stageflor = default;

   

    private List<GameObject> _attackObject = new List<GameObject>();//�U������I�u�W�F�N�g������
    private List<SpriteRenderer> _attackObjectSprite = new List<SpriteRenderer>();//�U������I�u�W�F�N�g��SpriteRenderer
    private List<BoxCollider2D> _attackObjectBoxCollider = new List<BoxCollider2D>();//�U������I�u�W�F�N�g��BoxCollider
    private List<CircleCollider2D> _attackObjectCircleCollider = new List<CircleCollider2D>();//�U������I�u�W�F�N�g��CircleCollider

    [SerializeField] LayerMask _groundLayer = default;//�n�ʃI�u�W�F�N�g�̃��C���[

    private Animator _bossAnime = default;

    private void Start()
    {
        Health = MAX_HEALTH;
        BodyContactDamage = BODY_CONTACT_DAMAGE;

        string findTag = "Player";
        _player = GameObject.FindGameObjectWithTag(findTag);

        _bossAnime=this.GetComponent<Animator>();

       
      
    }

    private void Update()
    {
        if (!IsAttackCoolDown)
        {
            DecisionAttack();
            IsAttackCoolDown = true;
        }

        switch (_attackNumber)
        {

            //�ǂ���G��U��
            case 1:
                AttackNumber1();
                break;

            //�V�䂩��G��U��
            case 2:
                AttackNumber2();
                break;

            //�e�ːi�U��
            case 3:
                AttackNumber3();
                break;

            //�ǔ��e�U��
            case 4:
                AttackNumber4();
                break;

            //�X�g���v�U��
            case 5:
                AttackNumber5();
                break;
               
        }

        if (Health <= 0&&!_isDeath)
        {
            _isDeath = true;
            Death();
        }
    }

    private void DecisionAttack()
    {
        //���X�g�̏�����
        _attackObject.Clear();
        _attackObjectBoxCollider.Clear();
        _attackObjectCircleCollider.Clear();
        _attackObjectSprite.Clear();

        int chooseAttackNumber = ChooseAttack(MAX_ATTACK_NUMBER);
        switch (chooseAttackNumber)
        {
            //�ǂ���G��U��
            case 1:
                //�L�΂��G��̐����擾
                for (int addAmount = 0; addAmount < _wallTentacle.Count; addAmount++)
                {
                    //�U������I�u�W�F�N�g����ɓ����
                    _attackObject.Add(_wallTentacle[addAmount]);
                    _attackObjectSprite.Add(_wallTentacle[addAmount].GetComponent<SpriteRenderer>());
                    _attackObjectBoxCollider.Add(_wallTentacle[addAmount].GetComponent<BoxCollider2D>());
                }
                _attackNumber = chooseAttackNumber;
                break;

            //�V�䂩��G��U��
            case 2:

                //�L�΂��G��̐����擾
                for (int addAmount = 0; addAmount < _cellingTentacle.Count; addAmount++)
                {
                    _attackObject.Add(_cellingTentacle[addAmount]);
                    _attackObjectSprite.Add(_cellingTentacle[addAmount].GetComponent<SpriteRenderer>());
                    _attackObjectBoxCollider.Add(_cellingTentacle[addAmount].GetComponent<BoxCollider2D>());
                }
                _attackNumber = chooseAttackNumber;

                break;

            //�e�ːi�U��
            case 3:

                _bossAnime.enabled = true;

                for (int addAmount = 0; addAmount < _magicBurret.Count; addAmount++)
                {
                    _attackObject.Add(_magicBurret[addAmount]);
                    _attackObjectSprite.Add(_magicBurret[addAmount].GetComponent<SpriteRenderer>());
                    _attackObjectCircleCollider.Add(_magicBurret[addAmount].GetComponent<CircleCollider2D>());
                }

                _animationName = "BurretAnimation";

                _bossAnime.Play(_animationName, 0, 0f);

                _attackNumber = chooseAttackNumber;

                break;

            //�ǔ��e�U��
            case 4:

                _attackNumber = chooseAttackNumber;
                break;

            //�X�g���v�U��
            case 5:

                _bossAnime.enabled = true;
                _animationName = "StompJumpAnimation";

                //��ɏオ��A�j���[�V�����N��
                _bossAnime.Play(_animationName, 0, 0f);

                //���݂���ʒu����
                _stompposition = _player.transform.position.x;

                _attackNumber = chooseAttackNumber;
                break;
        }
    }

    #region �U�����\�b�h
    private void AttackNumber1()
    {
        if (!_isFixed)
        {
            float objectscaleX = 0.01f;
            float objectscaleY = 0.45f;

            //�X�P�[���̐ݒ�
            _attackObject[_attackObjectNumber].transform.localScale = new Vector3(objectscaleX, objectscaleY);
            //�ŏ��ɐG��̈ʒu���Œ�
            _isFixed = true;
            _attackObject[_attackObjectNumber].transform.position = new Vector3(_rightWall.transform.position.x, _player.transform.position.y);

            //�����ڂ�on�ɂ���
            _attackObjectSprite[_attackObjectNumber].enabled = true;

            //�����蔻���on�ɂ���
            _attackObjectBoxCollider[_attackObjectNumber].enabled = true;
        }

        const float MAX_EXTEND = 0.3f;
        //���[�ɓ��B����܂�
        if (_attackObject[_attackObjectNumber].transform.localScale.x < MAX_EXTEND)
        {
            float extendSpeed = 0.05f;
            //���ɐG���L�΂�
            _attackObject[_attackObjectNumber].transform.localScale += new Vector3(extendSpeed * Time.deltaTime, 0);
        }
        else if (_attackObjectNumber < _attackObject.Count - 1)
        {

            //������A�j���[�V�����𗬂�
            _attackObject[_attackObjectNumber].GetComponent<Animator>().SetBool("isLeave", true);

            //���̐G���L�΂��n�߂�
            _isFixed = false;
            _attackObjectNumber++;
        }
        else
        {
            _attackObject[_attackObjectNumber].GetComponent<Animator>().SetBool("isLeave", true);
            //�U�����~�߂�
            _attackObjectNumber = 0;
            _isFixed = false;

            int attackwaitTime = 2;
            StartCoroutine(AttackCoolDown(attackwaitTime));

            _attackNumber = 0;
        }
       
    }

    private void AttackNumber2()
    {
        if (!_isFixed)
        {
            float objectscaleX = 0.01f;
            float objectscaleY = 0.45f;

            //�X�P�[���̐ݒ�
            _attackObject[_attackObjectNumber].transform.localScale = new Vector3(objectscaleX, objectscaleY);

            //�ŏ��ɐG��̈ʒu���Œ�
            _isFixed = true;
            _attackObject[_attackObjectNumber].transform.position = new Vector3(_player.transform.position.x, _celling.transform.position.y);

            //�����ڂ�on�ɂ���
            _attackObjectSprite[_attackObjectNumber].enabled = true;

            //�����蔻���on�ɂ���
            _attackObjectBoxCollider[_attackObjectNumber].enabled = true;

        }
        const float MAX_EXTEND = 0.16f;

        //��ԉ��ɓ��B����܂�
        if (_attackObject[_attackObjectNumber].transform.localScale.x < MAX_EXTEND)
        {
            float extendSpeed = 0.05f;
            //���ɐG���L�΂�
            _attackObject[_attackObjectNumber].transform.localScale += new Vector3(extendSpeed * Time.deltaTime, 0);
        }
        else if (_attackObjectNumber < _attackObject.Count - 1)
        {

            //������A�j���[�V�����𗬂�
            _attackObject[_attackObjectNumber].GetComponent<Animator>().SetBool("isLeave", true);

            //���̐G���L�΂��n�߂�
            _isFixed = false;
            _attackObjectNumber++;
        }
        else
        {
            _attackObject[_attackObjectNumber].GetComponent<Animator>().SetBool("isLeave", true);
            //�U�����~�߂�
            _attackNumber = 0;
            _attackObjectNumber = 0;
            _isFixed = false;

            //�N�[���_�E���J�n
            int attackwaitTime = 2;
            StartCoroutine(AttackCoolDown(attackwaitTime));
        }

        
    }

    private void AttackNumber3()
    {
        //�ŏ��͍U���܂łQ�b�҂�
        if (_elapsedTime > _burretAttackWaittime)
        {
            //�e���ˊJ�n

            _attackObjectCircleCollider[_attackObjectNumber].enabled = true;
            _attackObjectSprite[_attackObjectNumber].enabled = true;

            _attackObject[_attackObjectNumber].GetComponent<BossBurretScript>().Attack(_player);
            _attackObjectNumber++;
            _elapsedTime = 0;
            //��x�U�����n�߂���P�b�҂悤�ɂ���
            _burretAttackWaittime = 1;
        }
        else if (_attackObjectNumber < _attackObject.Count)
        {
            _elapsedTime += Time.deltaTime;
        }
        else
        {
            //�҂����Ԃ��Q�b�ɖ߂�
            _burretAttackWaittime = 2;

            _elapsedTime = 0;

            //���X�g�Q�ƕϐ����O�ɂ���
            _attackObjectNumber = 0;

            _attackNumber = 0;

            int attackwaitTime = 3;
            StartCoroutine(AttackCoolDown(attackwaitTime));
        }

       
    }

    private void AttackNumber4()
    {
       _attackObject.Add(Instantiate(_chaserBossBurret, this.transform.position, Quaternion.identity));

        int attackwaitTime = 7;
        StartCoroutine(AttackCoolDown(attackwaitTime));
        _attackNumber = 0;
    }

    private void AttackNumber5()
    {
        float moveSpeed = 25f;
        //�v���C���[���������E�ɂ�����
        if (_stompposition >= this.transform.position.x && !_isLeftMove)
        {
            this.transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
            _isRightMove = true;
        }
        else if (_isRightMove && _isFixed)
        {
            //���݂��A�j���[�V�������n�߂�
            StartCoroutine(StompfallAnimation());

            //�U�������I��
            _attackNumber = 0;
            _isRightMove = false;
            _isFixed = false;
        }

        //�v���C���[��������荶�ɂ�����
        if (_stompposition <= this.transform.position.x && !_isRightMove)
        {
            this.transform.position -= new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
            _isLeftMove = true;
        }
        else if (_isLeftMove && _isFixed)
        {
            //���݂��A�j���[�V�������J�n����
            StartCoroutine(StompfallAnimation());

            //�����I��
            _attackNumber = 0;
            _isLeftMove = false;
            _isFixed = false;
        }

        //���̍U���܂ł̃N�[���_�E���̓R���[�`�����\�b�h��StompfallAnimation�ɂĐ���

    }

    public void PositionAdjustment()
    {
        _bossAnime.enabled = false;
        _stageflor = Physics2D.OverlapBox(this.transform.position - new Vector3(0, 2f, 0), new Vector2(0.1f, 2f), 0, _groundLayer).gameObject;
        _isPositionAdjustment = true;

        //���̍U���܂ł̃N�[���_�E��
        int attackwaitTime = 3;

        StartCoroutine(AttackCoolDown(attackwaitTime));

    }
    #endregion

    /// <summary>
    /// �U���ԍ�3�̍U���A�j���[�V�����I����ɌĂяo��
    /// </summary>
    public void EndBurretAttackAnimation()
    {
        for (int FixedAmmount = 0; FixedAmmount < _attackObject.Count; FixedAmmount++)
        {
            _attackObject[FixedAmmount].transform.position = _attackObject[FixedAmmount].transform.position;
        }
        _bossAnime.enabled = false;

    }



    /// <summary>
    /// �U���ԍ��T�̃W�����v�A�j���[�V�����I��������Ăяo��
    /// </summary>
    /// <returns></returns>
    private IEnumerator StompfallAnimation()
    {
        //�҂�����
        float waitTime = 0.35f;
        yield return new WaitForSeconds(waitTime);
        {
            //���݂��A�j���[�V�����J�n
            string animationName = "StompFallAnimation";
            _bossAnime.Play(animationName, 0, 0f);
        }
    }

   

    /// <summary>
    /// �U���ԍ��T�̍U���A�j���[�V�����I���ォ��Ăяo��
    /// </summary>
    public void StompJumpFixed()
    {
        _isFixed = true;
    }

    public override void Death()
    {
        for (int attackObjectAmmount = 0; attackObjectAmmount < _attackObject.Count; attackObjectAmmount++)
        {
            Destroy(_attackObject[attackObjectAmmount]);
        }

        _exitHoleObject.SetActive(true);
        base.Death();
    }

    public void HealthBarSet()
    {
        BossHealthBar = _healthBarObject;

        HealthBarDisplay();
    }

    public override IEnumerator AttackCoolDown(float coolDowntime)
    {
        if (_isPositionAdjustment)
        {
            this.transform.position = new Vector3(this.transform.position.x, _stageflor.transform.position.y + 3, this.transform.position.z);
            _isPositionAdjustment = false;
        }
        return base.AttackCoolDown(coolDowntime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string playerTag = "Player";

        if (collision.gameObject.CompareTag(playerTag) && BodyContactDamage > 0)
        {
            BeAttacked(collision.gameObject);
        }
    }
}
