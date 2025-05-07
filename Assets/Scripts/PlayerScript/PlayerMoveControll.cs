using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveControll : MonoBehaviour
{
    #region ����
    private bool _isDeceleration = false;//�������Ă��邩
    private bool _isJump = false;//�W�����v�����Ă��邩
    private bool _isDoubleJump = false;//�_�u���W�����v��������
    private bool _isLeftRight = true; //false�͍��Atrue�͉E
    private bool _isParry = false;//�󂯗�����t���Ԃ�
    private bool _isAlive = true;//�v���C���[�������Ă��邩
    private bool _isRightKnockBack = false;//�E�m�b�N�o�b�N����
    private bool _isLeftKnockBack = false;//���m�b�N�o�b�N����
    private bool _isShield = false;//�������܂��Ă��邩
    private bool _isTacklePossible = false;//�^�b�N���U�����\��
    private bool _isLeftTackle = false;//���^�b�N����
    private bool _isRightTackle = false;//�E�^�b�N����

    private bool _isDerectionFixed = false;

    private bool _isGround = default; //�n�ʂɑ������Ă��邩�ǂ���
    [SerializeField] LayerMask _groundLayer = default;//�n�ʃI�u�W�F�N�g�̃��C���[

    private const int COMMON_ITEM_LAYER = 6; //�R�����A�C�e���̃��C���[
    private const int RARE_ITEM_LAYER = 7;�@//���A�A�C�e���̃��C���[
    private const int LEGEND_ITEM_LAYER = 8;�@//���W�F���h�A�C�e���̃��C���[
    private const int SPECIAL_ITEM_LAYER = 9;�@//����A�C�e���̃��C���[

    #endregion

    #region �X�e�[�^�X

    private float _guardMovementSpeed = 2f;//�����\���Ă���Ƃ��̃X�s�[�h
    private float _knockBackValue = 0.2f;//�m�b�N�o�b�N��
  
    private float _tackleSpeed = 0.3f;//�^�b�N���̑��x

    private const float JUMP_POEWR = 14.5f;//�v���C���[�̃W�����v��
    private const float FRICTION = 1f;//�U��������ۂ̖��C
    private const float DECELERATION_SPEED = 0.01f;//���X�Ɍ�������X�s�[�h
    private const float MINI_JUMP = 3f;

    #endregion

    #region ���^�@

    private Rigidbody2D _playerRB = default;

    [SerializeField] private AudioClip _jumpSE = default;
    [SerializeField] private AudioClip _dubbleJumpSE = default;
    
    private AudioSource _playerAudio = default;

    private const int PLAYER_FLIP_Y = 180;//�v���C���[�𔽓]�����邽�߂̒l

    private float _movementSpeed = 0.1f;//�v���C���[�̈ړ����x
    private float _tackleElapsedTime = 0f;//�^�b�N���\����̌o�ߎ���

    private Animator _playerAnim = default;

    private const float TIME_DELTATIME = 500f; //Time.Deltatime�Ɋ|����l

    private PlayerAttack _attackController = default;

    private PlayerStatusChange _playerStatusChange = default;

    [SerializeField] private PlayerStatusManegement _playerStatus =default;

    [SerializeField] private PlayerAnimationControll _playerAnimation= default;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _playerRB = this.GetComponent<Rigidbody2D>();
        _playerAnim = this.GetComponent<Animator>();
        _playerAudio = this.GetComponent<AudioSource>();
        _attackController = this.GetComponent<PlayerAttack>();
        _playerStatusChange = this.GetComponent<PlayerStatusChange>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAlive)
        {
            #region �ړ�

            if (!_isShield) //�V�[���h���\���Ă��Ȃ�������
            {
                DefaultMove();
            }
            else if (_isShield) //�V�[���h���\���Ă�����
            {
                if (!_isParry)//�󂯗������ԈȊO�ŏ����\���Ă���Ƃ� 
                {
                    ShieldMove();
                }
                //�󂯗�����t���Ԃ͈ړ��s��
            }

            int playerLayer = 11;
            if (Input.GetKey(KeyCode.S)&&this.gameObject.layer==playerLayer)
            {
                int fallPlayerLayer = 14;
                //������ォ�牺�ւ��蔲����
                SlipThroughPlatForm(fallPlayerLayer);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                //������ォ�牺�ւ��蔲�����Ȃ�����
                SlipThroughPlatForm(playerLayer);
            }

            #endregion

            //�n�ʂɂ��Ă��邩���Ă��Ȃ���
            if (_isGround)
            {
                AnimationChange();

                JumpJudge();
            }
            else
            {
                _playerAnimation.AnimationChange_Jump();
            }
        }  
    }
    #region oncollision,ontrigger
    private void OnCollisionStay2D(Collision2D collision)
    {
        string floorTag = "Flor";
        if (collision.gameObject.CompareTag(floorTag))
        {
            _isGround = Physics2D.OverlapBox(this.transform.position - new Vector3(0, 1.5f, 0), new Vector2(0.1f, 1f), 0, _groundLayer);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        string floorTag = "Flor";
        if (collision.gameObject.CompareTag(floorTag))
        {
            _isGround = false;
            _isJump = true;
        }
    }
    #endregion
   

    private void AnimationChange()
    {
        if (_playerRB.velocity.x != 0)
        {
            _playerAnimation.AnimationChange_Run();
        }
        else
        {
            _playerAnimation.AnimationChange_Idle();
        }
    }
    private void KnockBackDirection()
    {
        //�E�Ɉړ���
        if (_playerRB.velocity.x > 0)
        {
            _isRightKnockBack = true;
        }
        //���Ɉړ���
        else
        {
            _isLeftKnockBack = true;
        }
    }

    /// <summary>
    /// �ʏ�̈ړ����\�b�h
    /// </summary>
    private void DefaultMove()
    {
        
        #region ���E�ړ�����
        //���ړ�
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            
            //������������
            if (_isLeftRight && !_isDerectionFixed)
            {
                this.transform.rotation = new Quaternion(0, PLAYER_FLIP_Y, 0, 0);

                _isLeftRight = false;

                if (_playerRB.velocity.x > 0)
                {
                    //�u���[�L��������
                    _playerRB.velocity = new Vector2(FRICTION, this._playerRB.velocity.y);
                }

            }
            else if (_isLeftRight)
            {
                if (_playerRB.velocity.x > 0)
                {
                    //�u���[�L��������
                    _playerRB.velocity = new Vector2(FRICTION, this._playerRB.velocity.y);
                }
            }

            //�����̒��f
            if (_isDeceleration)
            {
                _isDeceleration = false;
            }
            //�v���C���[�̈ړ����x��MAXMOVEMENTSPEED�܂łɐ�������
            if (_playerRB.velocity.x > -_playerStatus.MaxMovementSpeed)
            {
                _playerRB.AddForce(new Vector2(-_movementSpeed * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else if(_playerRB.velocity.x < -_playerStatus.MaxMovementSpeed)
            {
                _playerRB.AddForce(new Vector2(_movementSpeed * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                _playerRB.velocity = new Vector2(-_playerStatus.MaxMovementSpeed, this._playerRB.velocity.y);
            }
        }
        //�E�ړ�
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            //�E����������
            if (!_isLeftRight && !_isDerectionFixed)
            {
                this.transform.rotation = new Quaternion(0, 0, 0, 0);

                _isLeftRight = true;

                if (_playerRB.velocity.x < 0)
                {
                    //�u���[�L��������
                    _playerRB.velocity = new Vector2(-FRICTION, this._playerRB.velocity.y);
                }
            }

            //�����̒��f
            if (_isDeceleration)
            {
                _isDeceleration = false;
            }

            //�v���C���[�̈ړ����x��MAXMOVEMENTSPEED�܂łɐ�������
            if (_playerRB.velocity.x < _playerStatus.MaxMovementSpeed)
            {
                _playerRB.AddForce(new Vector2(_movementSpeed * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else if(_playerRB.velocity.x > _playerStatus.MaxMovementSpeed)
            {
                _playerRB.AddForce(new Vector2(-_movementSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                
            }
            else
            {
                _playerRB.velocity = new Vector2(_playerStatus.MaxMovementSpeed, this._playerRB.velocity.y);
            }
        }
        //�ړ����Ă��Ȃ����A�E�Ƀm�b�N�o�b�N��
        else if (_isRightKnockBack)
        {
            if (_playerRB.velocity.x > 0)
            {
                //���x�����X�ɖ߂�
                _playerRB.AddForce(new Vector2(-_movementSpeed * Time.deltaTime * TIME_DELTATIME, 0));
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
            if (_playerRB.velocity.x < 0)
            {
                //���x�����X�ɖ߂�
                _playerRB.AddForce(new Vector2(_movementSpeed * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //�m�b�N�o�b�N�I��
                _isLeftKnockBack = false;
            }
        }
        #endregion

        #region �W�����v����
        //�W�����v
        if (Input.GetKeyDown(KeyCode.Space) && !_isDoubleJump)
        {
            if (_isJump)
            {
                //���ʂ̃W�����v�̉���炷
                _playerAudio.PlayOneShot(_dubbleJumpSE);
            }
            else
            {

                //�_�u���W�����v�̉���炷
                _playerAudio.PlayOneShot(_jumpSE);
            }

            if (_isJump)
            {
                _isDoubleJump = true;
            }
            _isJump = true;
            _playerRB.velocity = new Vector2(this._playerRB.velocity.x, JUMP_POEWR);


        }

        //���W�����v
        if (_playerRB.velocity.y >= 0 && Input.GetKeyUp(KeyCode.Space))
        {
            _playerRB.velocity = new Vector2(this._playerRB.velocity.x, this._playerRB.velocity.y / MINI_JUMP);
        }
        #endregion



        //�ړ��L�[�𗣂����猸���J�n
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) ||
           Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            _playerAnimation.AnimationChange_Idle();
            _isDeceleration = true;
        }

        #region ���X�Ɍ��������鏈��
        //���Ɉړ����Ă�����
        if ((!_isLeftRight && !_isDerectionFixed) || (!_isLeftRight && _isDerectionFixed && _playerRB.velocity.x < 0) || (_isLeftRight && _isDerectionFixed && _playerRB.velocity.x < 0))
        {
            //���X�Ɍ���������
            if (_isDeceleration && this._playerRB.velocity.x < 0)
            {
                this._playerRB.velocity -= new Vector2(this._playerRB.velocity.x * DECELERATION_SPEED, 0);
            }
            else
            {
                _isDeceleration = false;
            }

        }
        //�E�Ɉړ����Ă�����
        else if ((_isLeftRight && !_isDerectionFixed) || (_isLeftRight && _isDerectionFixed) || (!_isLeftRight && _isDerectionFixed))
        {
            //���X�Ɍ���������
            if (_isDeceleration && this._playerRB.velocity.x > 0)
            {
                this._playerRB.velocity -= new Vector2(this._playerRB.velocity.x * DECELERATION_SPEED, 0);
            }
            else
            {
                _isDeceleration = false;
            }
        }
        #endregion

        #region �^�b�N������
        if (_isTacklePossible)
        {
            //A��f������񓯎������ō��^�b�N��
            if (Input.GetKeyDown(KeyCode.A) && _isLeftTackle)
            {
                //�^�b�N������
                TackleMove();
                _attackController.TackleAttack();

                //�^�b�N�����o���Ȃ�����
                TacklePossibleJudge(false);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                //A����񉟂��������true
                _tackleElapsedTime = 0f;
                _isLeftTackle = true;
                _isRightTackle = false;
            }

            //D��f������񓯎������ŉE�^�b�N��
            if (Input.GetKeyDown(KeyCode.D) && _isRightTackle)
            {
                //�^�b�N������
                TackleMove();
                _attackController.TackleAttack();

                //�^�b�N�����o���Ȃ�����
                TacklePossibleJudge(false);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                //D����񉟂��������true
                _tackleElapsedTime = 0f;
                _isLeftTackle = false;
                _isRightTackle = true;
            }

            //A��D����񉟂���Ă�����
            if (_isLeftTackle || _isRightTackle)
            {
                //�^�b�N����t���Ԃ��v��
                _tackleElapsedTime += Time.deltaTime;
            }

            //�^�b�N����t����
            float tackleReceptionTime = 0.3f;

            //�o�ߎ��Ԃ���t���Ԃ��߂�����
            if (_tackleElapsedTime > tackleReceptionTime)
            {
                //�^�b�N�����s
                _isLeftTackle = false;
                _isRightTackle = false;
            }
        }
       
        #endregion
    }

    /// <summary>
    /// �����\���Ă��鎞�̈ړ����\�b�h
    /// </summary>
    private void ShieldMove()
    {
        //���ړ�
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            //������������
            if (_isLeftRight && !_isDerectionFixed)
            {
                this.transform.rotation = new Quaternion(0, PLAYER_FLIP_Y, 0, 0);

                _isLeftRight = false;
            }
            _playerRB.velocity = new Vector2(-_guardMovementSpeed, _playerRB.velocity.y);
        }

        //�E�ړ�
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (!_isLeftRight && !_isDerectionFixed)
            {
                this.transform.rotation = new Quaternion(0, 0, 0, 0);

                _isLeftRight = true;
            }
            _playerRB.velocity = new Vector2(_guardMovementSpeed, _playerRB.velocity.y);
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) ||
          Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            _playerRB.velocity = Vector2.zero;
        }
    }

    private void TackleMove()
    {
        //�U��������Œ肳��Ă�����
        if (_isDerectionFixed)
        {
            //�������猩�ĉE�������Ă��O�ɐi��ł�����
            if (_isLeftRight&&_playerRB.velocity.x > 0)
            {
                //�������猩�đO�Ƀ^�b�N��
                _playerRB.AddForce(transform.right * _tackleSpeed, ForceMode2D.Impulse);
            }

            //�������猩�č��������Ă��Ă��O�ɐi��ł�����
            if(!_isLeftRight && _playerRB.velocity.x < 0)
            {
                //�������猩�đO�Ƀ^�b�N��
                _playerRB.AddForce(transform.right * _tackleSpeed, ForceMode2D.Impulse);
            }
        }
        //�Œ肳��Ă��Ȃ�������
        else
        {
            //�������猩�đO�Ƀ^�b�N��
            _playerRB.AddForce(transform.right * _tackleSpeed, ForceMode2D.Impulse);
        }
           
    }

    public void GetItem(GameObject Item)
    {
        Item.GetComponent<SpriteRenderer>().enabled = false;
        Item.GetComponent<CircleCollider2D>().enabled = false;

        //�擾�����A�C�e����
        if (Item.layer == COMMON_ITEM_LAYER)//�R�����A�C�e����������
        {
            Item.GetComponent<CommonItemScript>().GiveToPlayer(_playerStatusChange);

        }
        else if (Item.layer == RARE_ITEM_LAYER)//���A�A�C�e����������
        {

        }
        else if (Item.layer == LEGEND_ITEM_LAYER)//���W�F���h�A�C�e����������
        {

        }
        else if (Item.layer == SPECIAL_ITEM_LAYER)//����A�C�e����������
        {
            Item.GetComponent<SpecialItemScript>().GiveToPlayer(_attackController);
        }
    }

    public bool AliveJudge()
    {
        return _isAlive;
    }

    public void TacklePossibleJudge(bool isJudge)
    {
        _isTacklePossible = isJudge ;

        if (!isJudge)
        {
            StartCoroutine(TackleCoolDown());
        }
    }

    private void SlipThroughPlatForm(int playerLayer)
    {
        this.gameObject.layer = playerLayer;
    }

    private void JumpJudge()
    {
        //�W�����v�\�ɂ���
        if (_isJump)
        {
            _isJump = false;
        }
        if (_isDoubleJump)
        {
            _isDoubleJump = false;
        }
    }


    public void ControllOn()
    {
        //������悤�ɂ���
        _isAlive = true;
        _attackController.ControllSwitch(true);
    }

    /// <summary>
    /// �v���C���[�𓮂��Ȃ�����
    /// </summary>
    public void ControllOff()
    {
        _playerRB.velocity = Vector2.zero;
        _isAlive = false;
        _attackController.ControllSwitch(false);
    }

    public void Retry()
    {
       
        ControllOn();

        //�X�e�[�^�X�����ɖ߂�
        _playerStatusChange.HitPointAndMagicPointReset();

        //�G�ɓ�����Ȃ����C���[���瓖���郌�C���[�ɐ؂�ւ���
        int playerLayer = 11;
        this.gameObject.layer = playerLayer;
        _playerAnim.SetBool("isDeath", false);
    }
   
    public void ParryKnockBack()
    {
        //�������������ɉ�����
        float stepBack = 0.05f;

        //�኱�m�b�N�o�b�N
        _playerRB.velocity = Vector2.zero;
        _playerRB.AddForce(-transform.right * stepBack, ForceMode2D.Impulse);
        KnockBackDirection();
    }

    public void TakeDamageKnockBack()
    {
        //�m�b�N�o�b�N
        _playerRB.velocity = Vector2.zero;
        _playerRB.AddForce(-transform.right * _knockBackValue, ForceMode2D.Impulse);
        KnockBackDirection();
    }

    public void PlayerDirectionChange(bool ChangeDirection)
    {
        //�v���C���[�̕����ɑ������
        _isLeftRight = ChangeDirection;
    }

    public void GetDerectionFixed(bool isJudge)
    {
        //�v���C���[�̕������Œ艻������
        _isDerectionFixed = isJudge;
    }

    public void GetIsShield(bool isJudge)
    {
        //�����\�������̈ړ��ɐ؂�ւ���
        _isShield = isJudge;
    }

    public void IsGetParry(bool isJudge)
    {
        _isParry = isJudge;
    }

    public bool GivePlayerDirection()
    {
        //false�͍��Atrue�͉E
        return _isLeftRight;
    }

   public bool GiveIsShield()
    {
        return _isShield;
    }

    private IEnumerator TackleCoolDown()
    {
        int cooldownTime = 1;
        //�P�b��A�^�b�N�����ł���悤�ɂ���
        yield return new WaitForSeconds(cooldownTime);
        {
            TacklePossibleJudge(true);
        }
    }

}
