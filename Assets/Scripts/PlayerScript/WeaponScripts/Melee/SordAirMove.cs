using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SordAirMove : MonoBehaviour
{
    private List<GameObject> _attackedenemy = new List<GameObject>();//�U�������������G���i�[

    [Header("�G�̃_���[�W��"), SerializeField] private List<AudioClip> _enemydamageSEs = new List<AudioClip>();

    [SerializeField] private TextMeshProUGUI _damageText = default;


    private bool _isHit = false;//�U�����Ȃ����ɓ���������
    private bool _isReturn = false;//���������G�͂��łɍU����H����Ă��邩

    private float _playerMeleeAttackPower = default;//�v���C���[�̋ߐڍU����

    private int _enemyJudge = default;//���X�g�̌���ԍ�
    private int _enemyDamageSeValue = default;//�G���U�������Ƃ��ɗ���Se�̃i���o�[

    private GameObject _worldSpaceCanvas = default;

    private SpriteRenderer _sordAirSprite = default;
    private BoxCollider2D _sordAirCollider = default;
    private Animator _sordAirMoveAnime = default;

    private PlayerAttack _playerAttack = default;

    private AudioSource _gameSE = default;

    // Start is called before the first frame update
    void Awake()
    {
        _gameSE = this.GetComponent<AudioSource>();

        _sordAirSprite = this.GetComponentInParent<SpriteRenderer>();
        _sordAirCollider = this.GetComponent<BoxCollider2D>();
        _sordAirMoveAnime = this.GetComponent<Animator>();

        string _findtag = "Player";

        _playerAttack=GameObject.FindGameObjectWithTag(_findtag).GetComponent<PlayerAttack>();

        _findtag = "WorldSpaceCanvas";

        _worldSpaceCanvas = GameObject.FindGameObjectWithTag(_findtag);
    }

    /// <summary>
    /// ���C�𓮂����n�߂�
    /// </summary>
    /// <param name="MoveDirection">false�͍������Atrue�͉E����</param>
    public void MoveStart(bool MoveDirection)
    {
        //�����ځA�����蔻��\��
        _sordAirSprite.enabled = true;
        _sordAirCollider.enabled = true;

        //���݂̃v���C���[�̋ߐڍU���͎Q��
        _playerMeleeAttackPower = _playerAttack.GiveMeleeAttackPower();

        //�����E�U���Ȃ�
        if (MoveDirection)
        {
            //�E����������
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        //�������U���Ȃ�
        else
        {
            int flipValue = 180;
            //������������
            this.transform.rotation = new Quaternion(0, flipValue, 0, 0);
        }


        string animationName = "SordAirMove";
        _sordAirMoveAnime.PlayInFixedTime(animationName, 0, 0f);

    }

    public void endAnimation()
    {
        //�ړ��A�j���[�V�����I��
        _sordAirSprite.enabled = false;
        _sordAirCollider.enabled = false;
        _attackedenemy.Clear();
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
                while (_enemyJudge < _attackedenemy.Count)
                {
                    //���łɂ��̓G�ɍU�����������Ă�����
                    if (_attackedenemy[_enemyJudge] == collision.gameObject)
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
            _attackedenemy.Add(collision.gameObject);

            //�U������������
            _isHit = true;

            //�g���K�[�����������ʒu�ɐ�������
            TextMeshProUGUI _damageTextDummy = Instantiate(_damageText, collision.transform.position + collision.bounds.extents, Quaternion.identity);

            //�L�����o�X��e�ɂ���
            _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

            //�_���[�W�l���e�L�X�g�ɓ����
            _damageTextDummy.text = _playerMeleeAttackPower.ToString();

            //�_���[�W�e�L�X�g�\��
            _damageTextDummy.GetComponent<DamageTextScript>().TextDefaultMove();

            //�G�Ƀ_���[�W��^����
            if (collision.gameObject.CompareTag("NormalEnemy"))
            {
                _enemyDamageSeValue = 0;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<NormalEnemyScript>().BeAttacked(_playerMeleeAttackPower,0, false);
            }
            else if (collision.gameObject.CompareTag("TurretEnemy"))
            {
                _enemyDamageSeValue = 1;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);

                collision.gameObject.GetComponent<TurretEnemyScript>().BeAttacked(_playerMeleeAttackPower);
            }
            else if (collision.gameObject.CompareTag("ChaserEnemy"))
            {

                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<ChaserEnemyScript>().BeAttacked(_playerMeleeAttackPower,0,false);
            }
            else if (collision.gameObject.CompareTag("FlyEnemy"))
            {
                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<FlyEnemyScript>().BeAttacked(_playerMeleeAttackPower, false);
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);

                IBossStatus bossInterface=collision.gameObject.GetComponent<IBossStatus>();

                bossInterface.TakeDamage(_playerMeleeAttackPower);

                //collision.gameObject.GetComponent<FirstBossAttackControll>().TakeDamage(_playerMeleeAttackPower);
            }

        }
    }
}
