using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagicBurretScript : MonoBehaviour
{
    private float _burretPower = 0;//�U����
    private float _knockBackPower = 6.5f;//�m�b�N�o�b�N�̑傫��
    private float _moveSpeed = 10f;//�ړ�����X�s�[�h

    private int _burretLevel = default;//�e�̃��x���i��̃��x���ƈꏏ�j

    private bool _isCritical = false;

    [SerializeField] private List<AudioClip> enemyDamageSE = new List<AudioClip>();

    private MagicStickAttack _parentWeaponScript = default;

    [SerializeField] private TextMeshProUGUI _damageText = default;//�_���[�W�e�L�X�g

    private GameObject _worldSpaceCanvas = default;//�_���[�W�e�L�X�g�\���̂��߂̃L�����o�X

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeUp());
        _worldSpaceCanvas = GameObject.FindGameObjectWithTag("WorldSpaceCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.up*Time.deltaTime*_moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        int stagePartsLayer = 12;
        int florLayer = 18;
        
        //�ǁA���A�V��̂����ꂩ�ɓ��������灕����̐i���`�Ԃ��O��������
        if (_burretLevel<=0&&(collision.gameObject.layer == stagePartsLayer||collision.gameObject.layer==florLayer))
        {
            //��������
            DestroyObject(0);
        }

        int enemyLayer = 10;
        int donthitEnemyLayer = 20;
        int FlyEnemyLayer = 22;

        //�G�ɓ���������
        if(collision.gameObject.layer == enemyLayer || collision.gameObject.layer == donthitEnemyLayer || 
            collision.gameObject.layer == FlyEnemyLayer)
        {
            DamageTextDisplay(collision.contacts[0].point);
            if (collision.gameObject.CompareTag("NormalEnemy"))
            {
                collision.gameObject.GetComponent<NormalEnemyScript>().BeAttacked(_burretPower, _knockBackPower,true);
                DestroyObject(1);
            }
            else if (collision.gameObject.CompareTag("TurretEnemy"))
            {
                collision.gameObject.GetComponent<TurretEnemyScript>().BeAttacked(_burretPower);
                DestroyObject(2);
            }
            else if (collision.gameObject.CompareTag("ChaserEnemy"))
            {
                collision.gameObject.GetComponent<ChaserEnemyScript>().BeAttacked(_burretPower, _knockBackPower,true);
                DestroyObject(3);
            }
            else if (collision.gameObject.CompareTag("FlyEnemy"))
            {
                collision.gameObject.GetComponent<FlyEnemyScript>().BeAttacked(_burretPower,true);
                DestroyObject(3);
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                collision.gameObject.GetComponent<IBossStatus>().TakeDamage(_burretPower);
                DestroyObject(3);
            }
        }
       
    }
    public void DamageTextDisplay(Vector3 DisplayPoint)
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

    public void GetPower(float AttackPower, MagicStickAttack weaponScript,bool isCriticalJudge,int weaponEvolution)
    {
        //�U���́A�N���e�B�J��������擾
        if (weaponEvolution > 0)
        {
            int DontStagePartsHitLayer = 26;
            this.gameObject.layer = DontStagePartsHitLayer;
        }

        _burretPower = AttackPower;
        _parentWeaponScript = weaponScript;
        _isCritical = isCriticalJudge;
    }

    private void DestroyObject( int DamageSeNumber)
    {
        if(_parentWeaponScript != null)
        {
            _parentWeaponScript.ReProductionSE(enemyDamageSE[DamageSeNumber]);
        }
     
        Destroy(this.gameObject);
    }

    private IEnumerator TimeUp()
    {
        yield return new WaitForSeconds(3);
        DestroyObject(0);
    }
}
