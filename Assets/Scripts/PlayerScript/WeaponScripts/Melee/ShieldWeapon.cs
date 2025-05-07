using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldWeapon : MonoBehaviour
{
    private int _weaponLevel = 0;//����̐i���`��

    private PlayerStatusChange _playerStatus = default;
    private PlayerMoveControll _playerMove = default;

    private float _knockBackPower = 6.5f;//�m�b�N�o�b�N��

    private void Start()
    {
        _playerStatus = GetComponentInParent<PlayerStatusChange>();
        _playerMove = GetComponentInParent<PlayerMoveControll>();
    }

    public int GiveWeaponLevel()
    {
        return _weaponLevel;
    }

    public void WeaponLevelUp()
    {
        _weaponLevel++;

        //�i���{�[�i�X�t�^
        switch (_weaponLevel)
        {
            case 1:
                //�h��͂��グ��
                int defenceUpValue = 20;
                _playerStatus.DefencePowerUP(defenceUpValue);
                
                //�v���C���[���^�b�N���U���ł���悤�ɂ���
                _playerMove.TacklePossibleJudge(true);
                break;
        }

    }

    public void ShieldBash(GameObject bashSubject)
    {
        string normalEnemyTag = "NormalEnemy";
        string chaserEnemyTag = "ChaserEnemy";
        string flyEnemyTag = "FlyEnemy";

        float AttackPower = _playerStatus.GiveMeleeAttackPower();

        if (bashSubject.gameObject.CompareTag(normalEnemyTag))
        {
            bashSubject.GetComponent<NormalEnemyScript>().BeAttacked(AttackPower, _knockBackPower, true);
        }
        else if (bashSubject.gameObject.CompareTag(chaserEnemyTag))
        {
            bashSubject.GetComponent<ChaserEnemyScript>().BeAttacked(AttackPower, _knockBackPower, true);
        }
        else if (bashSubject.gameObject.CompareTag(flyEnemyTag))
        {
            bashSubject.GetComponent<FlyEnemyScript>().BeAttacked(AttackPower, true);
        }
        else if (bashSubject.gameObject.CompareTag("Boss"))
        {
           
           bashSubject.gameObject.GetComponent<IBossStatus>().TakeDamage(AttackPower);
        }
    }
}
