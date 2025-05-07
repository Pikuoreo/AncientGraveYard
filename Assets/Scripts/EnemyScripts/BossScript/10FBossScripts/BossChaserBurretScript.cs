using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChaserBurretScript : MonoBehaviour
{
    private GameObject _chaserTarget = default;//’Ç”ö‚·‚éƒ^[ƒQƒbƒg

    private bool _isChaseStart = false;//true‚Å’Ç”öŠJn
    private bool _isAttack = false;//true‚ÅUŒ‚ŠJn

    private float _moveSpeed = 0.009f;//ˆÚ“®‚Ì‘¬‚³

    private int _attackPower = 20;//UŒ‚—Í

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
        //‚P•b‘Ò‚Á‚ÄUŒ‚ŠJn
        float _animationTime = 1f;
        yield return new WaitForSeconds(_animationTime);
        _isChaseStart = true;

        //‚T•b‘Ò‚Á‚Ä’e‚ğ™X‚ÉÁ‚µ‚Ä‚¢‚­
        float AttackEndTime = 5f;
        yield return new WaitForSeconds(AttackEndTime);

        string animationName = "AttackEnd";
        _burretAnimation.SetBool(animationName,true);

    }

    public void Attackend()
    {
        //ƒAƒjƒ[ƒVƒ‡ƒ“‚ªI‚í‚Á‚½‚çÁ‚·
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string _playerTag = "Player";

        if (!_isAttack&&collision.gameObject.CompareTag(_playerTag))
        {
            _isAttack = true;
            //æ“¾‚Íˆê“x‚¾‚¯
            if (_playerAttackScript == null)
            {
                _playerAttackScript = collision.gameObject.GetComponent<PlayerAttack>();
                _playerAttackScript.TakeDamage(_attackPower, this.gameObject, false, false);
            }
            else
            {
                //ƒ_ƒ[ƒW‚ğ—^‚¦‚é
                _playerAttackScript.TakeDamage(_attackPower, this.gameObject, false, false);
            }
            //”•bŠÔ©•ª‚ªUŒ‚‚Å‚«‚È‚­‚·‚é
            StartCoroutine(AttackCoolDown());
        }
    }

    private IEnumerator AttackCoolDown()
    {
        //UŒ‚‚ğ“–‚Ä‚½‚ ‚ÆA1.2•bŠÔ‚ÍUŒ‚‚ğ“–‚Ä‚È‚­‚·‚é
        float waitTime = 1.2f;

        yield return new WaitForSeconds(waitTime);
        _isAttack = false;
    }
}
