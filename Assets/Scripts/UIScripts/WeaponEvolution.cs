using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEvolution : MonoBehaviour
{
    [SerializeField] private GameObject _evolutionImage=default;

    [SerializeField] private GameObject _meleeChoice = default;
    [SerializeField] private GameObject _magicChoice = default;

    [SerializeField] private Image _meleeLeftClickAttackButton = default;
    [SerializeField] private Image _meleeRightClickAttackButton = default;

    [SerializeField] private Image _magicLeftClickAttackButton = default;
    [SerializeField] private Image _magicRightClickAttackButton = default;

    [Header("剣のボタンイメージ"),SerializeField] private List<Sprite> _sordButtonImages = new List<Sprite>();
    [Header("盾のボタンイメージ"),SerializeField] private List<Sprite> _shieldButtonImages = new List<Sprite>();
    [Header("直進魔法武器のボタンイメージ"),SerializeField] private List<Sprite> _magicStickButtonImages = new List<Sprite>();
    [Header("追尾魔法武器のボタンイメージ"),SerializeField] private List<Sprite> _chaserCaneButtonImages = new List<Sprite>();

    private PlayerMoveControll playerMoveControll = default;

    private bool _isEvent = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //イベント実行判定がfalseなら
        if (!_isEvent)
        {
            string playerTag = "Player";

            if (collision.gameObject.CompareTag(playerTag))
            {
                //プレイヤーを動かなくする
                playerMoveControll = collision.gameObject.GetComponent<PlayerMoveControll>();
                playerMoveControll.ControllOff();

                //プレイヤーの職業を代入
                int playerProfession = collision.gameObject.GetComponent<PlayerAttack>().NowProfession();

                //職業別
                switch (playerProfession)
                {
                    //剣士だったら
                    case 1:


                        //剣士用の選択肢を出す

                        _meleeLeftClickAttackButton.sprite = _sordButtonImages[GameObject.FindGameObjectWithTag("SordWeapon").
                                                         GetComponent<SordAttack>().
                                                         GiveWeaponLevel()];

                        _meleeRightClickAttackButton.sprite = _shieldButtonImages[GameObject.FindGameObjectWithTag("ShieldWeapon").
                                                          GetComponent<ShieldWeapon>().
                                                          GiveWeaponLevel()];
                        _meleeChoice.SetActive(true);
                        break;

                    //魔法使いだったら
                    case 2:
                        _magicLeftClickAttackButton.sprite = _magicStickButtonImages[GameObject.FindGameObjectWithTag("MagicStickWeapon").
                                                        GetComponent<MagicStickAttack>().
                                                        GiveWeaponLevel()];

                        _magicRightClickAttackButton.sprite = _chaserCaneButtonImages[GameObject.FindGameObjectWithTag("ChaserCaneWeapon").
                                                         GetComponent<ChaserCaneAttack>().
                                                         GiveWeaponLevel()];
                        //魔法使い用の選択肢を出す
                        _magicChoice.SetActive(true);
                        break;
                }

                //UI全体を出す
                _evolutionImage.SetActive(true);
                _isEvent = true;

            }

            //イベント実行判定をtrueに
           
        }
       
    }

    public void EndEvolutionChoice()
    {
        playerMoveControll.ControllOn();

        //選択UIをすべて見えなくする
        _meleeChoice.SetActive(false);
        _magicChoice.SetActive(false);
        _evolutionImage.SetActive(false);
    }
}
