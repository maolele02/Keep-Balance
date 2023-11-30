using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoSingleton<Balance>
{
    [SerializeField][Header("平衡时宝石消除的时间间隔")] private float removeDurationTime = 2.5f;
    private float angleUpperLimit;
    private float angleLowerLimit;

    public float AngleUpperLimit
    {
        get { return angleUpperLimit;}
        set { angleUpperLimit = value; }
    }

    public float AngleLowerLimit
    {
        get { return angleLowerLimit;}
        set { angleLowerLimit = value; }
    }

    private float angle;
    private Action<float> onRangeChange;

    public void AddRangeListener(Action<float> listener)
    {
        onRangeChange = listener;
    }

    private void Update()
    {
        float tempAngle = transform.localEulerAngles.z;

        if(tempAngle > 180f)
        {
            tempAngle -= 360f;
        }
        else if(tempAngle < -180f)
        {
            tempAngle += 360f;
        }

        tempAngle = tempAngle > angleUpperLimit ? angleUpperLimit : tempAngle;
        tempAngle = tempAngle < angleLowerLimit ? angleLowerLimit : tempAngle;

        transform.localEulerAngles = Vector3.forward * tempAngle;
        
        if (angle != tempAngle)
        {
            angle = tempAngle;
            onRangeChange?.Invoke(angle);
        }
    }


    private IEnumerator _BeginRemoveBalanceJewels()
    {
        while (true)
        {
            JewelManager.Instance.RemoveBalanceJewels();
            yield return new WaitForSeconds(removeDurationTime);
        }
    }

    public void BeginRemoveBalanceJewels()
    {
        StopAllCoroutines();
        StartCoroutine(_BeginRemoveBalanceJewels());
    }

    public void StopRemoveJewels()
    {
        StopAllCoroutines();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Jewel))
        {
            JewelManager.Instance.AddJewel(collision.gameObject);
        }
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    GameObject go = collision.gameObject;
    //    if (go.CompareTag(Tags.Jewel))
    //    {
    //        Jewel jewel = go.GetComponent<Jewel>();
    //        if (jewel.JewelType == JewelType.Player)
    //        {
    //            if (playerJewels.ContainsKey(jewel.JewelCfgID))
    //            {
    //                if (playerJewels[jewel.JewelCfgID] != null)
    //                {
    //                    playerJewels[jewel.JewelCfgID].Remove(go);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (enemyJewels.ContainsKey(jewel.JewelCfgID))
    //            {
    //                if (enemyJewels[jewel.JewelCfgID] != null)
    //                {
    //                    enemyJewels[jewel.JewelCfgID].Remove(go);
    //                }
    //            }
    //        }
    //    }
    //}
}
