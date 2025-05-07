using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject _player = default;
    private GameObject _spotTarget = default;//カメラが寄る対象

    private float _spotTime = 0f;
    private float _elapsedTime = 0f;//経過時間

    private bool _isSpot = false;//trueでカメラが対象に寄る

    [SerializeField] private GameObject _backGroundObject=default;//背景

    [SerializeField] private PlayerMoveControll _playerControllScript = default;

    [SerializeField] private BGMChangeScript _bgmChange = default;

    private int BossBGMValue = 0;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

        //特定のオブジェクトにカメラを寄せたいとき
        if (_isSpot)
        {
            _elapsedTime += Time.deltaTime;

            //spottime分、オブジェクトにカメラを当てる
            if (_elapsedTime < _spotTime)
            {
                SpotCameraControll();
            }
            //時間超過したら、元の状態に戻す
            else
            {
                EndSpotCameraControll();
            }
        }
        //通常はプレイヤーにカメラを追尾させる
        else
        {
            defaultCameraControll();
        }
    }

    public void defaultCameraControll()
    {
        float adjustmentY = 1.75f;
        float adjustmentZ = -10;

        //プレイヤーにカメラを追従させる
        this.transform.position = _player.transform.position + new Vector3(0, adjustmentY, adjustmentZ);
    }

    private void SpotCameraControll()
    {
        float spotSpeed = 0.05f;
        float adjustmentZ = 10;

        //対象にターゲットが寄る
        this.transform.position -= (this.transform.position - _spotTarget.transform.position)*spotSpeed+new Vector3(0,0,adjustmentZ);
    }

    private void EndSpotCameraControll()
    {
        //対象に寄るのをやめる
        _isSpot = false;
        _spotTime = 0;
        _elapsedTime = 0;

        //ボスの攻撃を開始する
        _spotTarget.GetComponent<IBossStatus>().HealthBarSet();


        _spotTarget = default;

        //プレイヤーが動けるようにする
        _playerControllScript.ControllOn();

        //次のボス戦で流れるＢＧＭをあらかじめセットしておく
        _bgmChange.ChangeBossBGM(BossBGMValue);
        BossBGMValue++;
    }

    public void SpotCamera(GameObject spotObject,float spotTime)
    {
        //カメラをspotobjectに寄せる
        _isSpot = true;
        //BGMを止める
        _bgmChange.BGMStop();

        _spotTarget = spotObject;
        _spotTime = spotTime;
        //プレイヤーを動かさなくする
        _playerControllScript.ControllOff();
    }

    public void ReMatchBossBattle()
    {
        //ゲームオーバーになった後、ボスと再選していたら

        //BGMの参照ナンバーを１戻す
        BossBGMValue--;
    }

    public void Backgroundchange()
    {
        //背景を変える
        _backGroundObject.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
