using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDescription : MonoBehaviour
{
    [SerializeField] private GameObject _weaponList = default;

    [Header("左クリック攻撃のイメージ"), SerializeField] private Image _leftClickAttackImage = default;
    [Header("右クリック攻撃のイメージ"), SerializeField] private Image _rightClickAttackImage = default;
    [Header("説明欄のイメージ"), SerializeField] private Image _descriptionTextImage = default;

    [SerializeField] private PlayerAttack _playerProffesion = default;

    private int _sordLevel = default;
    private int _shieldLevel = default;
    private int _magicStickLevel = default;
    private int _chaserCaneLevel = default;

    private int _playerProfessionNumber = default;

    [Header("剣のリストアイテム画像"), SerializeField] List<Sprite> _sordItemImages=new List<Sprite>();
    [Header("剣の説明テキスト画像"), SerializeField] List<Sprite> _sordDescriptionTextImages = new List<Sprite>();


    [Header("盾のリストアイテム画像"), SerializeField] List<Sprite> _shieldItemImages=new List<Sprite>();
    [Header("盾の説明テキスト画像"), SerializeField] List<Sprite> _shieldDescriptionTextImages = new List<Sprite>();

    [Header("直進魔法武器のリストアイテム画像"), SerializeField] List<Sprite> _magicStickItemImages=new List<Sprite>();
    [Header("直進魔法武器の説明テキスト画像"), SerializeField] List<Sprite> _magicStickDescriptionTextImages = new List<Sprite>();

    [Header("追尾魔法武器のリストアイテム画像"), SerializeField] List<Sprite> _chaserCaneItemImages=new List<Sprite>();
    [Header("追尾魔法武器の説明テキスト画像"), SerializeField] List<Sprite> _chaserCaneDescriptionTextImages = new List<Sprite>();
    //剣士の識別番号
    const int MELEE_PROFESSION_NUMBER = 1;
    //魔法使いの識別番号
    const int MAGIC_PROFESSION_NUMBER = 2;


    /// <summary>
    /// 左クリック攻撃の説明
    /// </summary>
    public void LeftClickAttackDescription()
    {
        //剣士だったら
        if (_playerProfessionNumber == MELEE_PROFESSION_NUMBER)
        {
            _descriptionTextImage.sprite = _sordDescriptionTextImages[_sordLevel];
        }
        //魔法使いだったら
        else if (_playerProfessionNumber == MAGIC_PROFESSION_NUMBER)
        {
            _descriptionTextImage.sprite = _magicStickDescriptionTextImages[_magicStickLevel];
        }
    }

    /// <summary>
    /// 右クリック攻撃の説明
    /// </summary>
    public void RightClickAttackDescription()
    {
        //剣士だったら
        if (_playerProfessionNumber == MELEE_PROFESSION_NUMBER)
        {
            _descriptionTextImage.sprite = _shieldDescriptionTextImages[_shieldLevel];
        }
        //魔法使いだったら
        else if (_playerProfessionNumber == MAGIC_PROFESSION_NUMBER)
        {
            _descriptionTextImage.sprite = _chaserCaneDescriptionTextImages[_chaserCaneLevel];
        }
    }

    /// <summary>
    /// 職業別のボタン表示&職業別の現在の武器レベル取得
    /// </summary>
    public void ProfessionDescriptionDisplay()
    {
        //プレイヤーの今の職業を参照
         _playerProfessionNumber = _playerProffesion.NowProfession();

        //剣士だったら
        if (_playerProfessionNumber == MELEE_PROFESSION_NUMBER)
        {
            //剣の現在のレベルを取得
            string findName = "SordWeapon";
            _sordLevel = GameObject.FindGameObjectWithTag(findName).GetComponent<SordAttack>().GiveWeaponLevel();

            //左クリック攻撃のイメージを現在の剣のレベルに合わせたアイテム画像に変更
            _leftClickAttackImage.sprite = _sordItemImages[_sordLevel];


            //盾の現在のレベルを取得
            findName = "ShieldWeapon";
            _shieldLevel = GameObject.FindGameObjectWithTag(findName).GetComponent<ShieldWeapon>().GiveWeaponLevel();

            //右クリック攻撃のイメージを現在の盾のレベルに合わせたアイテム画像に変更
            _rightClickAttackImage.sprite = _shieldItemImages[_shieldLevel];

            //装備のリストをオン
            _weaponList.SetActive(true);
        }
        //魔法使いだったら
        else if (_playerProfessionNumber == MAGIC_PROFESSION_NUMBER)
        {
            

            //直進魔法武器の現在のレベルを取得
            string findName = "MagicStickWeapon";
            _magicStickLevel = GameObject.FindGameObjectWithTag(findName).GetComponent<MagicStickAttack>().GiveWeaponLevel();

            //左クリック攻撃のイメージを現在の直進魔法武器のレベルに合わせたアイテム画像に変更
            _leftClickAttackImage.sprite = _magicStickItemImages[_magicStickLevel];


            //追尾魔法武器の現在のレベルを取得
            findName = "ChaserCaneWeapon";
            _chaserCaneLevel = GameObject.FindGameObjectWithTag(findName).GetComponent<ChaserCaneAttack>().GiveWeaponLevel();

            //右クリック攻撃のイメージを現在の追尾魔法武器のレベルに合わせたアイテム画像に変更
            _rightClickAttackImage.sprite = _chaserCaneItemImages[_chaserCaneLevel];

            //装備のリストをオン
            _weaponList.SetActive(true);
        }

        DescriptionTextImageReset();
    }

    public void DescriptionTextImageReset()
    {
        _descriptionTextImage.sprite = default;
    }
}
