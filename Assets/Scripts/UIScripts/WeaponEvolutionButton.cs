using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEvolutionButton : MonoBehaviour
{
    [SerializeField] WeaponEvolution _parentScript = default;

    public void SordEvolution()
    {
        string findTag = "SordWeapon";
        //���̃��x�����グ��
        GameObject.FindGameObjectWithTag(findTag).GetComponent<SordAttack>().WeaponLevelUp();
        _parentScript.EndEvolutionChoice();
    }
    public void ShieldEvolution()
    {
        string findTag = "ShieldWeapon";
        //���̃��x�����グ��
        GameObject.FindGameObjectWithTag(findTag).GetComponent<ShieldWeapon>().WeaponLevelUp();
        _parentScript.EndEvolutionChoice();
    }

    public void MagicStickEvolution()
    {
        string findTag = "MagicStickWeapon";

        //���i���@�̃��x�����グ��
        GameObject.FindGameObjectWithTag(findTag).GetComponent<MagicStickAttack>().WeaponLevelUp();
        _parentScript.EndEvolutionChoice();
    }

    public void ChaserCaneEvolution()
    {
        string findTag = "ChaserCaneWeapon";

        //�ǐՖ��@�̃��x�����グ��
        GameObject.FindGameObjectWithTag(findTag).GetComponent<ChaserCaneAttack>().WeaponLevelUp();
        _parentScript.EndEvolutionChoice();
    }
}
