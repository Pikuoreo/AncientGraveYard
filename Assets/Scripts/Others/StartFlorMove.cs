using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFlorMove : MonoBehaviour
{

     private Vector3 _originalCameraPosition;

    private float _shakeMagnitude = 0.1f;// êUìÆÇÃã≠Ç≥Çêßå‰

    private bool _isMove = false;
    private bool _isLeftMoveStop = false;
    private bool _isRightMoveStop = false;

    [SerializeField] private GameObject _leftMoveFlor = default;
    [SerializeField] private GameObject _rightMoveFlor = default;

    [SerializeField] private GameObject _startChangeProfessionMagic = default;
    [SerializeField] private GameObject _startChangeProfessionWarrior = default;

    private CircleCollider2D _magicItemCollider = default;
    private CircleCollider2D _warriorItemCollider = default;

    [SerializeField] GameObject _camera = default;
    // Start is called before the first frame update
    void Start()
    {
        _magicItemCollider = _startChangeProfessionMagic.GetComponent<CircleCollider2D>();
        _warriorItemCollider = _startChangeProfessionWarrior.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!_magicItemCollider.enabled && !_warriorItemCollider.enabled)
        {
            _isMove = true;
        }
        if (_isLeftMoveStop && _isRightMoveStop)
        {
            _isMove = false;
        }
        if (_isMove)
        {

            float x = Random.Range(-1f, 1f) * _shakeMagnitude;
            float y = Random.Range(-1f, 1f) * _shakeMagnitude;
            // êUìÆÇÉJÉÅÉâÇ…ìKóp
            _camera.transform.localPosition += new Vector3(x, y, 0f);
            if (_leftMoveFlor != null)
            {
                if (_leftMoveFlor.transform.position.x>=this.transform.position.x - 4f )
                {
                    _leftMoveFlor.transform.position -= new Vector3(0.001f, 0) * Time.deltaTime * 1000;
                }
                else
                {
                    _isLeftMoveStop = true;
                }
            }

             if (_rightMoveFlor != null)
            {
                if (_rightMoveFlor.transform.position.x <= this.transform.position.x + 4f)
                {
                    _rightMoveFlor.transform.position+=new Vector3(0.001f, 0) * Time.deltaTime * 1000;
                }
                else
                {
                    _isRightMoveStop = true;
                }
            }

        }
    }


    public void FlorMove(bool isGameStart)
    {
        _isMove = isGameStart;
    }

   
}
