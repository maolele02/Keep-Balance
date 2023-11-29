using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(Tags.Pointer))
        {
            (Balance.Instance as Balance).BeginRemoveBalanceJewels();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Pointer))
        {
            (Balance.Instance as Balance).StopRemoveJewels();
        }
    }
}
