using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAttackScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ChangeWizard")|| collision.gameObject.CompareTag("ChangeWarrior"))
        {
            collision.gameObject.GetComponent<SpecialItemScript>().BreakItem();
        }

       
    }
}
