using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurrentBurretScript : MonoBehaviour
{
    private const float TIME_DELTATIME= 500f;
    private float _moveSpeed = 0.025f;//弾の速度

    private float _minBurretDamage = default;//ダメージの最低値
    private float _maxBurretDamage = default;//ダメージの最高値

    private bool _isAttaked = false;
    // Start is called before the first frame update
    void Start()
    {
        //発生した３秒後、消える
        StartCoroutine(DestroyObject());
    }

    // Update is called once per frame
    void Update()
    {
        //弾の移動
        this.transform.position += (-this.transform.up*_moveSpeed)*Time.deltaTime*TIME_DELTATIME;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string playerTag = "Player";
        //プレイヤーに当たったらダメージを与える
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
        //３秒たったらこの弾を消す
        float destroyTime = 3f;
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }
}
