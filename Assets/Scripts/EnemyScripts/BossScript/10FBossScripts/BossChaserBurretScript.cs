using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChaserBurretScript : MonoBehaviour
{
    private GameObject _chaserTarget = default;//追尾するターゲット

    private bool _isChaseStart = false;//trueで追尾開始
    private bool _isAttack = false;//trueで攻撃開始

    private float _moveSpeed = 0.009f;//移動の速さ

    private int _attackPower = 20;//攻撃力

    private const int TIME_DELTATIME = 500;

    private Animator _burretAnimation = default;

    private PlayerAttack _playerAttackScript = default;
    // Start is called before the first frame update
    void Start()
    {
        string findtag = "Player";
        _chaserTarget = GameObject.FindGameObjectWithTag(findtag);
        _burretAnimation = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isChaseStart)
        {
            Chase();
        }
        
    }

   private void Chase()
    {

        Vector2 targetDistance = (_chaserTarget.transform.position - this.transform.position).normalized;

        Vector2 movement = targetDistance * _moveSpeed * Time.deltaTime * TIME_DELTATIME;

        this.transform.position += (Vector3)movement;
    }

    public IEnumerator AttackStart()
    {
        //１秒待って攻撃開始
        float _animationTime = 1f;
        yield return new WaitForSeconds(_animationTime);
        _isChaseStart = true;

        //５秒待って弾を徐々に消していく
        float AttackEndTime = 5f;
        yield return new WaitForSeconds(AttackEndTime);

        string animationName = "AttackEnd";
        _burretAnimation.SetBool(animationName,true);

    }

    public void Attackend()
    {
        //アニメーションが終わったら消す
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string _playerTag = "Player";

        if (!_isAttack&&collision.gameObject.CompareTag(_playerTag))
        {
            _isAttack = true;
            //取得は一度だけ
            if (_playerAttackScript == null)
            {
                _playerAttackScript = collision.gameObject.GetComponent<PlayerAttack>();
                _playerAttackScript.TakeDamage(_attackPower, this.gameObject, false, false);
            }
            else
            {
                //ダメージを与える
                _playerAttackScript.TakeDamage(_attackPower, this.gameObject, false, false);
            }
            //数秒間自分が攻撃できなくする
            StartCoroutine(AttackCoolDown());
        }
    }

    private IEnumerator AttackCoolDown()
    {
        //攻撃を当てたあと、1.2秒間は攻撃を当てなくする
        float waitTime = 1.2f;

        yield return new WaitForSeconds(waitTime);
        _isAttack = false;
    }
}
