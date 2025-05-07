using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveControll : MonoBehaviour
{
    #region 判定
    private bool _isDeceleration = false;//減速しているか
    private bool _isJump = false;//ジャンプをしているか
    private bool _isDoubleJump = false;//ダブルジャンプをしたか
    private bool _isLeftRight = true; //falseは左、trueは右
    private bool _isParry = false;//受け流し受付時間か
    private bool _isAlive = true;//プレイヤーが生きているか
    private bool _isRightKnockBack = false;//右ノックバック中か
    private bool _isLeftKnockBack = false;//左ノックバック中か
    private bool _isShield = false;//盾をかまえているか
    private bool _isTacklePossible = false;//タックル攻撃が可能か
    private bool _isLeftTackle = false;//左タックルか
    private bool _isRightTackle = false;//右タックルか

    private bool _isDerectionFixed = false;

    private bool _isGround = default; //地面に足がついているかどうか
    [SerializeField] LayerMask _groundLayer = default;//地面オブジェクトのレイヤー

    private const int COMMON_ITEM_LAYER = 6; //コモンアイテムのレイヤー
    private const int RARE_ITEM_LAYER = 7;　//レアアイテムのレイヤー
    private const int LEGEND_ITEM_LAYER = 8;　//レジェンドアイテムのレイヤー
    private const int SPECIAL_ITEM_LAYER = 9;　//特殊アイテムのレイヤー

    #endregion

    #region ステータス

    private float _guardMovementSpeed = 2f;//盾を構えているときのスピード
    private float _knockBackValue = 0.2f;//ノックバック量
  
    private float _tackleSpeed = 0.3f;//タックルの速度

    private const float JUMP_POEWR = 14.5f;//プレイヤーのジャンプ力
    private const float FRICTION = 1f;//振り向いた際の摩擦
    private const float DECELERATION_SPEED = 0.01f;//徐々に減速するスピード
    private const float MINI_JUMP = 3f;

    #endregion

    #region メタ　

    private Rigidbody2D _playerRB = default;

    [SerializeField] private AudioClip _jumpSE = default;
    [SerializeField] private AudioClip _dubbleJumpSE = default;
    
    private AudioSource _playerAudio = default;

    private const int PLAYER_FLIP_Y = 180;//プレイヤーを反転させるための値

    private float _movementSpeed = 0.1f;//プレイヤーの移動速度
    private float _tackleElapsedTime = 0f;//タックル可能からの経過時間

    private Animator _playerAnim = default;

    private const float TIME_DELTATIME = 500f; //Time.Deltatimeに掛ける値

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
            #region 移動

            if (!_isShield) //シールドを構えていなかったら
            {
                DefaultMove();
            }
            else if (_isShield) //シールドを構えていたら
            {
                if (!_isParry)//受け流し時間以外で盾を構えているとき 
                {
                    ShieldMove();
                }
                //受け流し受付時間は移動不可
            }

            int playerLayer = 11;
            if (Input.GetKey(KeyCode.S)&&this.gameObject.layer==playerLayer)
            {
                int fallPlayerLayer = 14;
                //足場を上から下へすり抜ける
                SlipThroughPlatForm(fallPlayerLayer);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                //足場を上から下へすり抜けられなくする
                SlipThroughPlatForm(playerLayer);
            }

            #endregion

            //地面についているかついていないか
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
        //右に移動中
        if (_playerRB.velocity.x > 0)
        {
            _isRightKnockBack = true;
        }
        //左に移動中
        else
        {
            _isLeftKnockBack = true;
        }
    }

    /// <summary>
    /// 通常の移動メソッド
    /// </summary>
    private void DefaultMove()
    {
        
        #region 左右移動処理
        //左移動
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            
            //左を向かせる
            if (_isLeftRight && !_isDerectionFixed)
            {
                this.transform.rotation = new Quaternion(0, PLAYER_FLIP_Y, 0, 0);

                _isLeftRight = false;

                if (_playerRB.velocity.x > 0)
                {
                    //ブレーキをかける
                    _playerRB.velocity = new Vector2(FRICTION, this._playerRB.velocity.y);
                }

            }
            else if (_isLeftRight)
            {
                if (_playerRB.velocity.x > 0)
                {
                    //ブレーキをかける
                    _playerRB.velocity = new Vector2(FRICTION, this._playerRB.velocity.y);
                }
            }

            //減速の中断
            if (_isDeceleration)
            {
                _isDeceleration = false;
            }
            //プレイヤーの移動速度をMAXMOVEMENTSPEEDまでに制限する
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
        //右移動
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            //右を向かせる
            if (!_isLeftRight && !_isDerectionFixed)
            {
                this.transform.rotation = new Quaternion(0, 0, 0, 0);

                _isLeftRight = true;

                if (_playerRB.velocity.x < 0)
                {
                    //ブレーキをかける
                    _playerRB.velocity = new Vector2(-FRICTION, this._playerRB.velocity.y);
                }
            }

            //減速の中断
            if (_isDeceleration)
            {
                _isDeceleration = false;
            }

            //プレイヤーの移動速度をMAXMOVEMENTSPEEDまでに制限する
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
        //移動していないかつ、右にノックバック中
        else if (_isRightKnockBack)
        {
            if (_playerRB.velocity.x > 0)
            {
                //速度を徐々に戻す
                _playerRB.AddForce(new Vector2(-_movementSpeed * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //ノックバック終了
                _isRightKnockBack = false;
            }
        }
        //移動していないかつ、左にノックバック中
        else if (_isLeftKnockBack)
        {
            if (_playerRB.velocity.x < 0)
            {
                //速度を徐々に戻す
                _playerRB.AddForce(new Vector2(_movementSpeed * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //ノックバック終了
                _isLeftKnockBack = false;
            }
        }
        #endregion

        #region ジャンプ処理
        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && !_isDoubleJump)
        {
            if (_isJump)
            {
                //普通のジャンプの音を鳴らす
                _playerAudio.PlayOneShot(_dubbleJumpSE);
            }
            else
            {

                //ダブルジャンプの音を鳴らす
                _playerAudio.PlayOneShot(_jumpSE);
            }

            if (_isJump)
            {
                _isDoubleJump = true;
            }
            _isJump = true;
            _playerRB.velocity = new Vector2(this._playerRB.velocity.x, JUMP_POEWR);


        }

        //小ジャンプ
        if (_playerRB.velocity.y >= 0 && Input.GetKeyUp(KeyCode.Space))
        {
            _playerRB.velocity = new Vector2(this._playerRB.velocity.x, this._playerRB.velocity.y / MINI_JUMP);
        }
        #endregion



        //移動キーを離したら減速開始
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) ||
           Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            _playerAnimation.AnimationChange_Idle();
            _isDeceleration = true;
        }

        #region 徐々に減速させる処理
        //左に移動していたら
        if ((!_isLeftRight && !_isDerectionFixed) || (!_isLeftRight && _isDerectionFixed && _playerRB.velocity.x < 0) || (_isLeftRight && _isDerectionFixed && _playerRB.velocity.x < 0))
        {
            //徐々に減速させる
            if (_isDeceleration && this._playerRB.velocity.x < 0)
            {
                this._playerRB.velocity -= new Vector2(this._playerRB.velocity.x * DECELERATION_SPEED, 0);
            }
            else
            {
                _isDeceleration = false;
            }

        }
        //右に移動していたら
        else if ((_isLeftRight && !_isDerectionFixed) || (_isLeftRight && _isDerectionFixed) || (!_isLeftRight && _isDerectionFixed))
        {
            //徐々に減速させる
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

        #region タックル処理
        if (_isTacklePossible)
        {
            //Aを素早く二回同時押しで左タックル
            if (Input.GetKeyDown(KeyCode.A) && _isLeftTackle)
            {
                //タックル成功
                TackleMove();
                _attackController.TackleAttack();

                //タックルを出来なくする
                TacklePossibleJudge(false);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                //Aを一回押した判定をtrue
                _tackleElapsedTime = 0f;
                _isLeftTackle = true;
                _isRightTackle = false;
            }

            //Dを素早く二回同時押しで右タックル
            if (Input.GetKeyDown(KeyCode.D) && _isRightTackle)
            {
                //タックル成功
                TackleMove();
                _attackController.TackleAttack();

                //タックルを出来なくする
                TacklePossibleJudge(false);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                //Dを一回押した判定をtrue
                _tackleElapsedTime = 0f;
                _isLeftTackle = false;
                _isRightTackle = true;
            }

            //AかDが一回押されていたら
            if (_isLeftTackle || _isRightTackle)
            {
                //タックル受付時間を計測
                _tackleElapsedTime += Time.deltaTime;
            }

            //タックル受付時間
            float tackleReceptionTime = 0.3f;

            //経過時間が受付時間を過ぎたら
            if (_tackleElapsedTime > tackleReceptionTime)
            {
                //タックル失敗
                _isLeftTackle = false;
                _isRightTackle = false;
            }
        }
       
        #endregion
    }

    /// <summary>
    /// 盾を構えている時の移動メソッド
    /// </summary>
    private void ShieldMove()
    {
        //左移動
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            //左を向かせる
            if (_isLeftRight && !_isDerectionFixed)
            {
                this.transform.rotation = new Quaternion(0, PLAYER_FLIP_Y, 0, 0);

                _isLeftRight = false;
            }
            _playerRB.velocity = new Vector2(-_guardMovementSpeed, _playerRB.velocity.y);
        }

        //右移動
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
        //振り向きを固定されていたら
        if (_isDerectionFixed)
        {
            //自分から見て右を向いてかつ前に進んでいたら
            if (_isLeftRight&&_playerRB.velocity.x > 0)
            {
                //自分から見て前にタックル
                _playerRB.AddForce(transform.right * _tackleSpeed, ForceMode2D.Impulse);
            }

            //自分から見て左を向いていてかつ前に進んでいたら
            if(!_isLeftRight && _playerRB.velocity.x < 0)
            {
                //自分から見て前にタックル
                _playerRB.AddForce(transform.right * _tackleSpeed, ForceMode2D.Impulse);
            }
        }
        //固定されていなかったら
        else
        {
            //自分から見て前にタックル
            _playerRB.AddForce(transform.right * _tackleSpeed, ForceMode2D.Impulse);
        }
           
    }

    public void GetItem(GameObject Item)
    {
        Item.GetComponent<SpriteRenderer>().enabled = false;
        Item.GetComponent<CircleCollider2D>().enabled = false;

        //取得したアイテムが
        if (Item.layer == COMMON_ITEM_LAYER)//コモンアイテムだったら
        {
            Item.GetComponent<CommonItemScript>().GiveToPlayer(_playerStatusChange);

        }
        else if (Item.layer == RARE_ITEM_LAYER)//レアアイテムだったら
        {

        }
        else if (Item.layer == LEGEND_ITEM_LAYER)//レジェンドアイテムだったら
        {

        }
        else if (Item.layer == SPECIAL_ITEM_LAYER)//特殊アイテムだったら
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
        //ジャンプ可能にする
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
        //動けるようにする
        _isAlive = true;
        _attackController.ControllSwitch(true);
    }

    /// <summary>
    /// プレイヤーを動けなくする
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

        //ステータスを元に戻す
        _playerStatusChange.HitPointAndMagicPointReset();

        //敵に当たらないレイヤーから当たるレイヤーに切り替える
        int playerLayer = 11;
        this.gameObject.layer = playerLayer;
        _playerAnim.SetBool("isDeath", false);
    }
   
    public void ParryKnockBack()
    {
        //自分も少し後ろに下がる
        float stepBack = 0.05f;

        //若干ノックバック
        _playerRB.velocity = Vector2.zero;
        _playerRB.AddForce(-transform.right * stepBack, ForceMode2D.Impulse);
        KnockBackDirection();
    }

    public void TakeDamageKnockBack()
    {
        //ノックバック
        _playerRB.velocity = Vector2.zero;
        _playerRB.AddForce(-transform.right * _knockBackValue, ForceMode2D.Impulse);
        KnockBackDirection();
    }

    public void PlayerDirectionChange(bool ChangeDirection)
    {
        //プレイヤーの方向に代入する
        _isLeftRight = ChangeDirection;
    }

    public void GetDerectionFixed(bool isJudge)
    {
        //プレイヤーの方向を固定化させる
        _isDerectionFixed = isJudge;
    }

    public void GetIsShield(bool isJudge)
    {
        //盾を構えた時の移動に切り替える
        _isShield = isJudge;
    }

    public void IsGetParry(bool isJudge)
    {
        _isParry = isJudge;
    }

    public bool GivePlayerDirection()
    {
        //falseは左、trueは右
        return _isLeftRight;
    }

   public bool GiveIsShield()
    {
        return _isShield;
    }

    private IEnumerator TackleCoolDown()
    {
        int cooldownTime = 1;
        //１秒後、タックルができるようにする
        yield return new WaitForSeconds(cooldownTime);
        {
            TacklePossibleJudge(true);
        }
    }

}
