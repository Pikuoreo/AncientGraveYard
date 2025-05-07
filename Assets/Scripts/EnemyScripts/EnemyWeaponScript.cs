using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponScript : MonoBehaviour
{
    [SerializeField] private bool _bossWeapon = default;//ボスが持つ武器かどうか

    [SerializeField] private int _weaponNumber = default;//武器ID

    private float _attackPower = default;//オブジェクトの攻撃力

    private bool _parryPossible = default;//trueは受け流し可能、falseは受け流し不可

    // Start is called before the first frame update
    void Start()
    {
        if (_bossWeapon)
        {
            //ボスが持つ武器だったら
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
            //通常の敵が持つ武器だったら
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
