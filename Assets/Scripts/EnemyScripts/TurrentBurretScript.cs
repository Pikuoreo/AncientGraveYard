using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurrentBurretScript : MonoBehaviour
{
    private const float TIME_DELTATIME= 500f;
    private float _moveSpeed = 0.025f;//�e�̑��x

    private float _minBurretDamage = default;//�_���[�W�̍Œ�l
    private float _maxBurretDamage = default;//�_���[�W�̍ō��l

    private bool _isAttaked = false;
    // Start is called before the first frame update
    void Start()
    {
        //���������R�b��A������
        StartCoroutine(DestroyObject());
    }

    // Update is called once per frame
    void Update()
    {
        //�e�̈ړ�
        this.transform.position += (-this.transform.up*_moveSpeed)*Time.deltaTime*TIME_DELTATIME;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string playerTag = "Player";
        //�v���C���[�ɓ���������_���[�W��^����
        if (collision.gameObject.CompareTag(playerTag) &&!_isAttaked)
        {
            _isAttaked = true;
            float damage = Random.Range(_minBurretDamage, _maxBurretDamage);

            collision.gameObject.GetComponent<PlayerAttack>().TakeDamage(damage, this.gameObject,true, false);
        }
    }

    public void GetAttackPower(float minAttackPower, float maxAttackPower)
    {
        _minBurretDamage = minAttackPower;
        _maxBurretDamage = maxAttackPower;
    }

    public void BurretErase()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator DestroyObject()
    {
        //�R�b�������炱�̒e������
        float destroyTime = 3f;
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }
}
