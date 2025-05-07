using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool _isTackle = false;//タックル受付時間か
    private bool _isParry = false;//受け流し受付時間か
    private bool _isSordDirection = false;//剣の向き　falseは左向き　trueは右
    private bool _isAttackDirection = default;//攻撃する向き　falseは左向き trueは右向き
    private bool _isInvincibilityTime;//攻撃を食らった後の無敵時間か
    private bool _isMeleeAttackCoolDown = false;//近接のクールダウン
    private bool _isMagicattackCoolDown = false;//魔法のクールダウン
    private bool _isShieldCoolDown = false;//盾をかまえているか
    private bool _isAlive = true;

    private int _profession = 0;//0は無職
                                //1は剣士
                                //２は魔法使い

    private int _damageSeValue = default;

    private const int MIN_CRITICAL_VALUE = 1;
    private const int MAX_CRITICAL_VALUE = 100;

    private const int PLAYER_FLIP_Y = 180;//プレイヤーを反転させるための値

    private float _meleeAttackCoolDownTime = 0.5f;//近接攻撃のクールダウン
    private float _magicattackCoolDownTime = 0.55f;//魔法攻撃のクールダウン

    private AudioSource _playerAudio = default;

    [Header("敵と接触した時の音"), SerializeField] private List<AudioClip> _damageSEs = new List<AudioClip>();

    private PlayerMoveControll _moveControll = default;

    private PlayerStatusUI _statusUI = default;

    private PlayerStatusChange _playerStatusChange = default;

    private PlayerAnimationControll _animationcontroll = default;

    [SerializeField] private PlayerStatusManegement _playerStatus = default;

    #region 武器用変数

    [Header("戦士用武器保管庫"), SerializeField] private GameObject _meleeWeapon1 = default; //戦士の時持たせる武器1
    [Header("戦士用武器第2保管庫"), SerializeField] private GameObject _meleeWeapon2 = default; //戦士の時持たせる武器2

    [Header("魔術師用武器保管庫"), SerializeField] private GameObject _magicWeapon1 = default; //魔術師の時持たせる武器1
    [Header("魔術師用武器第2保管庫"), SerializeField] private GameObject _magicWeapon2 = default; //魔術師の時持たせる武器1

    [Header("現在の持ち武器"), SerializeField] private GameObject _currentWeapon1 = default; //今持っている武器1
    [Header("現在の持ち武器2"), SerializeField] private GameObject _currentWeapon2 = default; //今持っている武器２

    private GameObject _leftClickWeapon = default;//ゲーム上に出ている左クリック武器
    private GameObject _rightClickWeapon = default;//ゲーム上に出ている右クリック武器

    private SpriteRenderer _leftClickWeaponSprite = default;　//持っている武器のスプライトレンダラー　
    private SpriteRenderer _rightClickWeaponSprite = default;　//持っている武器のスプライトレンダラー2

    private SordMove _meleeWeaponMove = default; //剣の動きにかんするスクリプト
    private SordAttack _meleeWeaponAttack = default;//剣の攻撃に関するスクリプト

    private ShieldWeapon _shieldAttack = default;

    private MagicStickAttack _magicStickAttack = default;//直進魔法の攻撃スクリプト
    private ChaserCaneAttack _chaserCaneAttack = default;//追尾魔法の攻撃スクリプト
    #endregion

    void Start()
    {
        _moveControll = this.GetComponent<PlayerMoveControll>();
        _playerAudio = this.GetComponent<AudioSource>();
        _playerStatusChange = this.GetComponent<PlayerStatusChange>();
        _statusUI = this.GetComponent<PlayerStatusUI>();
        _animationcontroll=this.GetComponentInChildren<PlayerAnimationControll>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAlive)
        {
            //職業別攻撃
            switch (_profession)
            {
                //剣士の時
                case 1:

                    WorriorMoveControll();
                    break;

                //魔法使いの時
                case 2:

                    WizardMovecontroll();
                    break;
            }
        }
    }

    #region 近接攻撃メソッド
    private void WorriorMoveControll()
    {
        #region 剣を振る処理

        //剣を振り上げる
        if (Input.GetMouseButton(0) && !_isMeleeAttackCoolDown)
        {
            _animationcontroll.ReadyMeleeAttackAnimation();
            SordFlip();

            if (!_leftClickWeaponSprite.enabled)
            {
                //攻撃準備
                _meleeWeaponAttack.StartPreAttack(true, _isAttackDirection, _playerStatus.MagicPoint);

                //currentWeapon1を可視化する
                _leftClickWeaponSprite.enabled = true;

                //プレイヤーに追尾させる
                _meleeWeaponMove.AttackStart(true, _isSordDirection);
            }
        }

        //剣を振る
        if (Input.GetMouseButtonUp(0) && _leftClickWeaponSprite.enabled)
        {
            _animationcontroll.StartMeleeAttackAnimation();
            //構えをやめる
            _meleeWeaponAttack.StartPreAttack(false, _isAttackDirection, _playerStatus.MagicPoint);

            //攻撃クールダウン開始
            _isMeleeAttackCoolDown = true;
            StartCoroutine(MeleeAttackCoolDown());
        }
        #endregion

        #region 盾を構える処理
        if (Input.GetMouseButtonDown(1) && !_isShieldCoolDown)//シールドを構える
        {
            _rightClickWeaponSprite.enabled = true;
            _rightClickWeaponSprite.color = Color.yellow;
            _moveControll.GetIsShield(true);
            _moveControll.IsGetParry(true);
            _isParry = true;
            _isShieldCoolDown = true;
            StartCoroutine(ParryTime());
        }

        if (_moveControll.GiveIsShield() && Input.GetMouseButtonUp(1))
        {
            _rightClickWeaponSprite.enabled = false;

            _moveControll.GetIsShield(false);

            StartCoroutine(ShieldCoolDown());
        }

        #endregion
    }

    public float GiveAttackPower(bool isCharge)
    {
        //チャージ攻撃の攻撃力計算
        if (isCharge)
        {
            float chargiMultiple = 1.75f;
            return (_playerStatus.MeleeAttackPower + _playerStatus.BasicPower) * chargiMultiple;
        }
        //非チャージ攻撃の攻撃力計算
        else
        {
            return _playerStatus.MeleeAttackPower + _playerStatus.BasicPower;
        }
    }

    public float GiveMeleeAttackPower()
    {
        return _playerStatus.MeleeAttackPower;
    }

    public void MeleeAttackEnd()
    {
        _moveControll.GetDerectionFixed(false);
    }

    private void SordFlip()
    {
        //もしクリックした位置がプレイやーより右だったら
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > this.transform.position.x)
        {
            _isAttackDirection = true;
            //右を向かせる
            _moveControll.PlayerDirectionChange(true);
            _moveControll.GetDerectionFixed(true);
            this.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        //もしクリックした位置がプレイヤーより左だったら
        else
        {
            _isAttackDirection = false;
            //左を向かせる
            _moveControll.PlayerDirectionChange(false);
            _moveControll.GetDerectionFixed(true);
            this.transform.localRotation = new Quaternion(0, PLAYER_FLIP_Y, 0, 0);
        }
    }

    public void TackleAttack()
    {
        _isTackle = true;
        StartCoroutine(ShieldDushtime());
    }

    #endregion

    #region 魔法攻撃メソッド

    private void WizardMovecontroll()
    {
        //減るMpの量
        float consumptionPoint = _magicStickAttack.GiveMagicpointConsumptionValue();


        //HPを削ってMP回復
        if (Input.GetKey(KeyCode.G) && Input.GetMouseButtonDown(0))
        {
            _playerStatusChange.ConvertHptoMp();
        }

        #region 魔法攻撃１

        else if (Input.GetMouseButtonDown(0) && !_isMagicattackCoolDown && _playerStatus.MagicPoint >= consumptionPoint)
        {
            _animationcontroll.MagicAttackAnimation();
            _magicStickAttack.AttackStart();

            _playerStatusChange.ReduseMagicPoint(consumptionPoint);

            //攻撃のクールダウン
            _isMagicattackCoolDown = true;

            StartCoroutine(MagicAttackCoolDown());
        }

        #endregion

        #region 魔法攻撃２

        //減るMPの量
        consumptionPoint = 8;

        if (Input.GetMouseButtonDown(1) && !_isMagicattackCoolDown && _playerStatus.MagicPoint >= consumptionPoint)
        {
            _animationcontroll.MagicAttackAnimation();
            _chaserCaneAttack.AttackStart();

            _playerStatusChange.ReduseMagicPoint(consumptionPoint);

            //攻撃のクールダウン
            _isMagicattackCoolDown = true;
            StartCoroutine(MagicAttackCoolDown());
        }

        #endregion
    }

    public float GiveStickMagicAttackPower()
    {
       
        return _playerStatus.MagicattackPower + _playerStatus.BasicPower;
    }

    public float GiveCaneMagicAttackPower()
    {
        float attackMultiplier = 1.2f;//直進攻撃より少し威力を上げる
        return _playerStatus.BasicPower + _playerStatus.MagicattackPower * attackMultiplier ;
    }

    #endregion

    #region 共通
    public bool GiveCriticalJudge()
    {
        //クリティカル率取得
        int randomValue = Random.Range(MIN_CRITICAL_VALUE, MAX_CRITICAL_VALUE);

        //クリティカル判定
        if (randomValue <= _playerStatus.CriticalChance)
        {
            //クリティカル
            return true;
        }
        else
        {
            //非クリティカル
            return false;
        }
    }
    #endregion

    public float GiveCriticalMultiple()
    {

        //クリティカル倍率取得
        float criticalMultiplier = _playerStatusChange.GiveCriticalMultiple();

        return criticalMultiplier / MAX_CRITICAL_VALUE;

    }

    public void ChangeForProffession(int professionNumber)
    {
        _profession = professionNumber;
        switch (_profession)
        {
            //戦士になった時
            case 1:

                //持っている武器の切り替え
                _currentWeapon1 = _meleeWeapon1;
                _currentWeapon2 = _meleeWeapon2;

                //先にゲーム上に出ている武器があれば消す
                if (_leftClickWeapon != null)
                {
                    Destroy(_leftClickWeapon);
                }

                //先にゲーム上に出ている武器があれば消す
                if (_rightClickWeapon != null)
                {
                    Destroy(_rightClickWeapon);
                }

                //今持っている武器をゲーム上に出す
                _leftClickWeapon = Instantiate(_currentWeapon1);


                int SordFlipZ = 30;
                //プレイヤーが右を向いていたら
                if (_moveControll.GivePlayerDirection())
                {
                    //剣の向きを右向きにする
                    _isSordDirection = true;

                    _leftClickWeapon.transform.localRotation = Quaternion.Euler(0, 0, SordFlipZ);
                }
                //プレイヤーが左を向いていたら
                else
                {
                    //剣の向きを左向きにする
                    _isSordDirection = false;
                    _leftClickWeapon.transform.localRotation = Quaternion.Euler(0, 0, -SordFlipZ);
                }

                _leftClickWeapon.transform.parent = this.transform;

                float shieldPositonX = 0.4f;

                //盾をゲーム上に生成
                _rightClickWeapon = Instantiate(_currentWeapon2, this.transform.position + new Vector3(shieldPositonX, 0, 0), Quaternion.identity);
                _rightClickWeapon.transform.parent = this.transform;

                //スクリプトの取得
                _meleeWeaponMove = _leftClickWeapon.GetComponent<SordMove>();
                _meleeWeaponAttack = _leftClickWeapon.GetComponent<SordAttack>();

                _shieldAttack=_rightClickWeapon.GetComponent<ShieldWeapon>();

                GetRendererAndCollider();
                break;


            //魔術師になった時
            case 2:

                //持っている武器の切り替えs
                _currentWeapon1 = _magicWeapon1;
                _currentWeapon2 = _magicWeapon2;



                //先にゲーム上に出ている武器があれば消す
                if (_leftClickWeapon != null)
                {
                    Destroy(_leftClickWeapon);
                }

                //先にゲーム上に出ている武器があれば消す
                if (_rightClickWeapon != null)
                {
                    Destroy(_rightClickWeapon);
                }

                //今持っている武器をゲーム上に出す
                _leftClickWeapon = Instantiate(_currentWeapon1, this.transform.position, Quaternion.identity);
                //_weaponObject.transform.parent = this.transform;

                _rightClickWeapon = Instantiate(_currentWeapon2, this.transform.position, Quaternion.identity);
                //_weaponObject2.transform.parent = this.transform;

                //スクリプトの取得
                _magicStickAttack = _leftClickWeapon.GetComponent<MagicStickAttack>();
                _chaserCaneAttack = _rightClickWeapon.GetComponent<ChaserCaneAttack>();

                GetRendererAndCollider();
                break;
        }
    }
    
    private void GetRendererAndCollider()
    {
        //持っている武器のスプライトレンダラーを取得
        _leftClickWeaponSprite = _leftClickWeapon.GetComponent<SpriteRenderer>();
        _rightClickWeaponSprite = _rightClickWeapon.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 敵と接触した際の判定
    /// </summary>
    /// <param name="damage">喰らうダメージ量</param>
    /// <param name="attackedEnemy">攻撃を当てられた敵</param>
    /// <param name="isParryPossible">受け流しができる攻撃かどうか trueなら受け流せる、falseなら受け流せない</param>
    /// <param name="isTacklePossible">タックル攻撃が可能か　trueならできる、falseなら出来ない</param>
    public void TakeDamage(float damage, GameObject attackedEnemy, bool isParryPossible,bool isTacklePossible)
    {
        if (_isParry && isParryPossible)//攻撃を受け流す
        {

            //受け流しの時の音を流す
            _damageSeValue = 2;
            _playerAudio.PlayOneShot(_damageSEs[_damageSeValue]);

            _statusUI.ParryDamageTextAppearance();

            _moveControll.ParryKnockBack();

            int enemyTagNumber = 10;
            int flyEnemyTagNumber = 22;
            int enemyBurretLayer = 17;
            //当たった対象がHPをもつ敵オブジェクトだったら
            if (attackedEnemy.gameObject.layer == enemyTagNumber
                ||
                attackedEnemy.gameObject.layer == flyEnemyTagNumber)
            {
                if (attackedEnemy.gameObject.CompareTag("NormalEnemy"))
                {
                    attackedEnemy.GetComponent<NormalEnemyScript>().Stun();
                }
                else if (attackedEnemy.gameObject.CompareTag("ChaserEnemy"))
                {
                    attackedEnemy.GetComponent<ChaserEnemyScript>().Stun();
                }
                else if (attackedEnemy.gameObject.CompareTag("FlyEnemy"))
                {
                    attackedEnemy.GetComponent<FlyEnemyScript>().Stun();
                }
                else
                {
                    print("不明な敵Tag");
                }

            }
            //HPを持たない敵オブジェクトだったら
            else if (attackedEnemy.gameObject.layer == enemyBurretLayer)
            {
                attackedEnemy.GetComponent<TurrentBurretScript>().BurretErase();
            }

        }
        else if (_isTackle && isTacklePossible && !_isInvincibilityTime)
        {
            _isTackle = false;
            //シールド攻撃の音を流す
            //_playerAudio.PlayOneShot(_damageSEs[_damageSeValue]);

            //自分もすこしノックバックする
            _moveControll.TakeDamageKnockBack();

            _statusUI.UsuallydDamageTextAppearance(_playerStatus.MeleeAttackPower);

            int EnemyTagNumber = 10;
            int FlyEnemyTagNumber = 22;

            //当たった対象がHPをもつ敵オブジェクトだったら
            if (attackedEnemy.gameObject.layer == EnemyTagNumber
                ||
                attackedEnemy.gameObject.layer == FlyEnemyTagNumber)
            {
                //盾で攻撃する
                _shieldAttack.ShieldBash(attackedEnemy);
            }
        }
        else if (!_isInvincibilityTime)//無敵時間じゃなかったら
        {
            this.gameObject.layer = 21;
            //無敵時間開始
            _isInvincibilityTime = true;



            //盾を構えていて、受け流し不可攻撃じゃなかったら
            if (_moveControll.GiveIsShield())
            {
                //ガード時の音を流す
                _damageSeValue = 1;
                _playerAudio.PlayOneShot(_damageSEs[_damageSeValue]);


                _playerStatusChange.GuardTakeDamage(damage);

                StartCoroutine(EndInvincible());
            }
            else //ダメージを食らう
            {
                //攻撃を食らった時の音を流す
                _damageSeValue = 0;
                _playerAudio.PlayOneShot(_damageSEs[_damageSeValue]);
                switch (_profession)
                {
                    case 1: //剣士の時

                        _playerStatusChange.ProFessionIsMeleeTakeDamage(damage);


                        _moveControll.TakeDamageKnockBack();


                        StartCoroutine(EndInvincible());
                        break;

                    case 2://魔法使いの時

                        _playerStatusChange.ProfessionIsMagicTakeDamage(damage);

                        _moveControll.TakeDamageKnockBack();

                        StartCoroutine(EndInvincible());
                        break;
                }

            }
        }
    }

    public int NowProfession()
    {
        return _profession;
    }

    public void ControllSwitch(bool aliveSwitch)
    {
        //攻撃できなくする
        _isAlive = aliveSwitch;

        if (aliveSwitch)
        {
            _isInvincibilityTime=false;
        }
    }

    #region コルーチン
    private IEnumerator MeleeAttackCoolDown()
    {
        yield return new WaitForSeconds(_meleeAttackCoolDownTime);
        _isMeleeAttackCoolDown = false;
    }

    private IEnumerator MagicAttackCoolDown()
    {
        yield return new WaitForSeconds(_magicattackCoolDownTime);//0.75秒後に攻撃できるようになる
        _isMagicattackCoolDown = false;
    }

    private IEnumerator ParryTime()
    {
        float _paryyTime = 0.25f;//受け流し受付時間
        yield return new WaitForSeconds(_paryyTime);
        _moveControll.IsGetParry(false);
        _isParry = false;
        _rightClickWeaponSprite.color = Color.white;
    }

    private IEnumerator ShieldCoolDown()
    {
        float _shieldCoolDownTime = 0.5f;
        yield return new WaitForSeconds(_shieldCoolDownTime);
        _isShieldCoolDown = false;
    }

    private IEnumerator EndInvincible()
    {
        float invincibleTime = 1.2f; //無敵時間

        yield return new WaitForSeconds(invincibleTime); //1.2秒後、無敵時間解除

        if (_moveControll.AliveJudge())
        {
            this.gameObject.layer = 11;
            _isInvincibilityTime = false;
        }
    }

    private IEnumerator ShieldDushtime()
    {

        yield return new WaitForSeconds(0.3f);
        {
            _isTackle = false;
        }
    }
    #endregion
}
