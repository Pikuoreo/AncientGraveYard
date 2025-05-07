using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class magicPointStarMove : MonoBehaviour
{
    private Rigidbody2D _starRb = default;
    [SerializeField] float _recoveryValue = default;//MpâÒïúó 
    // Start is called before the first frame update
    void Start()
    {
        _starRb=this.GetComponent<Rigidbody2D>();

        float minMoveX = -0.0015F;
        float maxMoveX = 0.0015f;

        float moveY = 0.0015f;

        float moveX = Random.Range(minMoveX, maxMoveX);

        StartCoroutine(DestroyObject());

        Vector2 moveDirection= new Vector2(moveX,moveY);

        _starRb.AddForce(moveDirection ,ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string PlayerTag = "Player";
        if (collision.gameObject.CompareTag(PlayerTag))
        {
            //MpÇâÒïúÇ≥ÇπÇÈ
            collision.gameObject.GetComponent<PlayerStatusChange>().MagicPointRecovery(_recoveryValue);
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DestroyObject()
    {
        int waitTime=5;
        yield return new WaitForSeconds(waitTime);
        //5ïbå„Ç…è¡Ç∑
        Destroy(this.gameObject); 
    }
}
