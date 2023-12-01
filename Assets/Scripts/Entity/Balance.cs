using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoSingleton<Balance>
{
    [SerializeField][Header("游戏配置")] private GameSetting gameSetting;

    private Rigidbody2D rb;

    private RectTransform rectTrans;

    private float angle;
    private Action<float> onRangeChange;

    public void AddRangeListener(Action<float> listener)
    {
        onRangeChange = listener;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        rb.mass = gameSetting.balanceMass;

        rectTrans = GetComponent<RectTransform>();
        
    }

    public void ResetAngle()
    {
        if (rb == null)
        {
            return;
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        transform.localEulerAngles = Vector3.zero;
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

        tempAngle = tempAngle > gameSetting.angleUpperLimit ? gameSetting.angleUpperLimit : tempAngle;
        tempAngle = tempAngle < -gameSetting.angleUpperLimit ? -gameSetting.angleUpperLimit : tempAngle;

        transform.localEulerAngles = Vector3.forward * tempAngle;
        
        if (angle != tempAngle)
        {
            angle = tempAngle;
            onRangeChange?.Invoke(angle);
        }

        // 很奇怪，我Freeze Position了物体还是会因为碰撞而改变位置
        // 这里代码强制设置位置不变
        rectTrans.anchoredPosition = Vector2.zero;
    }


    private IEnumerator _BeginRemoveBalanceJewels()
    {
        while (true)
        {
            JewelManager.Instance.RemoveBalanceJewels();
            yield return new WaitForSeconds(gameSetting.removeDurationTime);
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag(Tags.Jewel))
        {
            Jewel jewel = go.GetComponent<Jewel>();
            JewelManager.Instance.RemoveJewel(jewel.JewelType, jewel.SpawnID);
        }
    }
}
