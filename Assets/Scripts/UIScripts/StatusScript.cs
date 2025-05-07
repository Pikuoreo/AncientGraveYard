using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusScript : MonoBehaviour
{
    private Image _thisImage = default;
    private bool _isOpenList = false;//trueならリストを開いている、falseならリストを閉じている

    [SerializeField] private PlayerStatusUI _playerStatus = default;
    [SerializeField] private GameObject _textParent = default;
    [SerializeField] private GameObject _weaponList = null;
    [SerializeField] private GameObject _itemList = default;
    [SerializeField] private ItemDescriptionScript _itemDescription = default;

    [SerializeField] private GameObject _floorText = default;

    [SerializeField] private PlayerMoveControll _moveController;
    [SerializeField] private PlayerAttack _playerAttack = default;

    // Start is called before the first frame update
    void Start()
    {
        _thisImage = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_moveController.AliveJudge())
            {
                if (ListOpenJudge())
                {
                    ListOpen();
                }
                else
                {
                    ListClosed();
                }
            }
        }
    }

    /// <summary>
    /// リストを開くか閉じるかの判定を渡す
    /// </summary>
    /// <returns></returns>
    private bool ListOpenJudge()
    {
        if (_isOpenList)
        {
            _isOpenList = false;
            return _isOpenList;
        }
        else
        {
            _isOpenList = true;
            return _isOpenList;
        }
    }

    /// <summary>
    /// リストを開く
    /// </summary>
    private void ListOpen()
    {
        //ステータス欄を開く
        _floorText.SetActive(false);
        _playerStatus.ListOpen();
        _textParent.SetActive(true);
        _isOpenList = true;
        _thisImage.enabled = true;

        //プレイヤーが攻撃できないようにする
        _playerAttack.ControllSwitch(false);

    }

    /// <summary>
    /// リストを閉じる
    /// </summary>
    private void ListClosed()
    {
        //ステータス欄を閉じる
        _floorText.SetActive(true);
        _playerStatus.ListClosed();
        _textParent.SetActive(false);
        _itemList.SetActive(false);
        _weaponList.SetActive(false);

        //プレイヤーを攻撃できるようにする
        _playerAttack.ControllSwitch(true);

        //説明文が表示されてたらリセットする
        _itemDescription.Textreset();

        _isOpenList = false;
        _thisImage.enabled = false;
    }
}
