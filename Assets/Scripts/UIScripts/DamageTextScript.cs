using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextScript : MonoBehaviour
{
    private bool _isMove = false;//trueで動き始める

    private TextMeshProUGUI _DamageText = default;
    private RectTransform _textTransform = default;
    // Start is called before the first frame update
    void Awake()
    {
        _DamageText = this.GetComponent<TextMeshProUGUI>();
        _textTransform = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMove&& _DamageText.color.a>0)
        {
            TextMove();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void TextMove()
    {
        float changeSpeed = 0.01f;
        const int TIME_DELTATIME = 250;

        //動きつつ、透明化していく
        _DamageText.color -= new Color(0, 0, 0, changeSpeed) * Time.deltaTime * TIME_DELTATIME;
        _textTransform.transform.position += new Vector3(0, changeSpeed, 0) * Time.deltaTime * TIME_DELTATIME;
    }

    public void TextDefaultMove()
    {
        _isMove = true;

        //テキストの色を白色にする
        _DamageText.color = Color.white;
    }

    public void TextCriticalMove()
    {
        _isMove = true;
        //テキストの色を赤色にする
        _DamageText.color = Color.red;
    }

    public void TextParryMove()
    {
        _isMove = true;

        //テキストの色を黄色にする
        _DamageText.color = Color.yellow;
    }

    public void TextMagicPointRecoveryMove()
    {
        _isMove = true;

        //テキストの色を黄色にする
        _DamageText.color = Color.blue;
    }
}
