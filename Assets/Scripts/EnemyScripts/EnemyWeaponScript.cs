using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponScript : MonoBehaviour
{
    [SerializeField] private bool _bossWeapon = default;//�{�X�������킩�ǂ���

    [SerializeField] private int _weaponNumber = default;//����ID

    private float _attackPower = default;//�I�u�W�F�N�g�̍U����

    private bool _parryPossible = default;//true�͎󂯗����\�Afalse�͎󂯗����s��

    // Start is called before the first frame update
    void Start()
    {
        if (_bossWeapon)
        {
            //�{�X�������킾������
            switch (_weaponNumber)
            {
                case 1:
                    _attackPower = 50;
                    _parryPossible = false;
                    break;
            }
        }
        else
        {
            //�ʏ�̓G�������킾������
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string playerTag = "Player";
        if (collision.gameObject.CompareTag(playerTag))
        {
            collision.gameObject.GetComponent<PlayerAttack>().TakeDamage(_attackPower, this.gameObject, _parryPossible,false);
        }
    }
}
