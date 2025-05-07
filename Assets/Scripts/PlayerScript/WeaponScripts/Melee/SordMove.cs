using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SordMove : MonoBehaviour
{
    private bool _isAttack = false;�@//�U����������
    private bool _isPreAttack = false;//�U�������߂Ă�Œ���
    private bool _isHit = false;//�U�����Ȃ����ɓ���������
    private bool _isReturn = false;//���������G�͂��łɍU����H����Ă��邩
    private bool _isCritical = false;//�N���e�B�J���������ǂ���
    private bool _isPlayAnimation = false;//true�ŃA�j���[�V�����Đ�

    private int _enemyJudge = default;//���X�g�̌���ԍ�
   

    private float _playerAttackPower = 0;//�v���C���[�̍U����
    private float _knockBackPower = 7.5f;

    private GameObject _player = default;�@//�v���C���[�Q��
    private GameObject _worldSpaceCanvas = default;

    private List<GameObject> _attackedEnemy = new List<GameObject>();//�U�������������G���i�[

    private SpriteRenderer WeaponSprite = default;�@//���g�̌�����
    private BoxCollider2D WeaponCollider = default;�@//���g�̓����蔻��
    private Animator _weaponAnim = default;�@//���g�̃A�j���[�V����

    [SerializeField] private TextMeshProUGUI _damageText = default;

    private PlayerAttack _playerAttackScript = default;

    private AudioSource _gameSE = default;

    private int _enemyDamageSeValue = default;

    [Header("����U�������̉�"), SerializeField] private AudioClip _sordSE = default;
    [Header("�G�̃_���[�W��"), SerializeField] private List<AudioClip> _enemyDamageSEs = new List<AudioClip>(); 

    // Start is called before the first frame update
    void Start()
    {
        _gameSE=this.GetComponent<AudioSource>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerAttackScript = _player.GetComponent<PlayerAttack>();
        WeaponCollider = this.GetComponent<BoxCollider2D>();
        WeaponSprite = this.GetComponent<SpriteRenderer>();

        _weaponAnim = this.GetComponent<Animator>();

        _worldSpaceCanvas = GameObject.FindGameObjectWithTag("WorldSpaceCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPreAttack)
        {
            //�����̃|�W�V��������Ƀv���C���[�̏������ɍ��킹��
            this.transform.position = _player.transform.position;
        }

        if (_isAttack&&!_isPlayAnimation)
        {
           
            if (this.transform.localRotation.z >=0)
            {
                StartSordAnimation();
            }
            else
            {
                StartSordAnimation();
            } 
        }
    }

    private void StartSordAnimation()
    {
        string animationName = "isAttack";
        //����U��A�j���[�V�����𗬂�
        _weaponAnim.SetBool(animationName, true);
        _isPlayAnimation = true;
    }

    public void AttackStart(bool isStart , bool isAnimationDirection)
    {
        _isPreAttack = isStart;

        //true��������E�����̃A�j���[�V����
        if (isAnimationDirection)
        {
            _weaponAnim.SetBool("isRight", true);
        }
        //false�������獶�����̃A�j���[�V����
        else
        {
            _weaponAnim.SetBool("isLeft", true);
        }
    }

    /// <summary>
    /// �A�j���[�V�������I�������ɗ���
    /// </summary>
    public void AttackEnd()
    {
        //�U���I����A���ׂčŏ��ɖ߂�
        _isHit = false;
        _attackedEnemy.Clear();

        string animationName = "isAttack";

        _weaponAnim.SetBool(animationName, false);
        _isAttack = false;
        _isPlayAnimation = false;

        //�����ڂ������Ȃ�����
        WeaponCollider.enabled = false;
        WeaponSprite.enabled = false;

        //�U���I��
        _isPreAttack = false;
        _playerAttackScript.MeleeAttackEnd();
    }

    public void GetAttackPower(float playerPower, bool isCritical)
    {
        //�U���́A�N���e�B�J��������擾
        _playerAttackPower = playerPower;

        //�U������ON
        WeaponCollider.enabled = true;
        _isAttack = true;
        _isCritical = isCritical;
        _gameSE.PlayOneShot(_sordSE);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        int enemyLayer = 10;
        int donthitEnemyLayer = 20;
        int FlyEnemyLayer = 22;

        if (collision.gameObject.layer == enemyLayer || 
            collision.gameObject.layer == donthitEnemyLayer || 
            collision.gameObject.layer == FlyEnemyLayer)
        {
            //��������ɍU�����������Ă�����
            if (_isHit)
            {
                while (_enemyJudge < _attackedEnemy.Count)
                {
                    //���łɂ��̓G�ɍU�����������Ă�����
                    if (_attackedEnemy[_enemyJudge] == collision.gameObject)
                    {
                        _isReturn = true;
                    }
                    _enemyJudge++;
                }
                _enemyJudge = 0;
            }

            //���łɍU�����������Ă���G�Ȃ�Ȃɂ����Ȃ�
            if (_isReturn)
            {
                _isReturn = false;
                return;

            }

            //�U�������������G�����X�g�ɓ����
            _attackedEnemy.Add(collision.gameObject);

            //�U������������
            _isHit = true;

            //�g���K�[�����������ʒu�ɐ�������
            TextMeshProUGUI _damageTextDummy = Instantiate(_damageText, collision.transform.position+collision.bounds.extents, Quaternion.identity);

            //�L�����o�X��e�ɂ���
            _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

            //�_���[�W�l���e�L�X�g�ɓ����
            _damageTextDummy.text = _playerAttackPower.ToString();
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

            //�G�Ƀ_���[�W��^����
            if (collision.gameObject.CompareTag("NormalEnemy"))
            {
                _enemyDamageSeValue = 0;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);

                collision.gameObject.GetComponent<NormalEnemyScript>().BeAttacked(_playerAttackPower, _knockBackPower,true);
            }
            else if (collision.gameObject.CompareTag("TurretEnemy"))
            {
                _enemyDamageSeValue = 1;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);

                collision.gameObject.GetComponent<TurretEnemyScript>().BeAttacked( _playerAttackPower);
            }
            else if (collision.gameObject.CompareTag("ChaserEnemy"))
            {

                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<ChaserEnemyScript>().BeAttacked(_playerAttackPower, _knockBackPower,true);
            }
            else if (collision.gameObject.CompareTag("FlyEnemy"))
            {
                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<FlyEnemyScript>().BeAttacked(_playerAttackPower,true);
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<IBossStatus>().TakeDamage(_playerAttackPower);
            }

        }
    }

    

   
}
