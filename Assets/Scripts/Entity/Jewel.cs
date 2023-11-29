using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum JewelType
{
    None,
    Player,
    Enemy
}

public class Jewel : MonoBehaviour
{
    [Range(50f, 200f)]
    [SerializeField][Header("初始降落速度")] private float downSpeed = 100f;
    [SerializeField][Header("宝石配置ID文字")] private TextMeshProUGUI cfgIDText;

    private XConfig.Jewel.Data selfConfig;
    private JewelType jewelType;
    private Rigidbody2D rb;

    // jewel在jewel之间的唯一标识
    private int spawnID;
    private static int spawnCount;
    public int SpawnID => spawnID;

    public JewelType JewelType => jewelType;
    public int JewelCfgID => selfConfig.id;

    public void Init(int cfgID, JewelType jewelType)
    {
        if(!XConfig.Jewel.Items.TryGetValue(cfgID, out selfConfig))
        {
            Debug.LogError($"jewel id: {cfgID} error.");
            return;
        }

        this.jewelType = jewelType;

        cfgIDText.text = cfgID.ToString();

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        spawnID = spawnCount++;

        rb.mass = selfConfig.weight;
    }

    public void AddSpawnSpeed()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        rb.velocity = Vector2.down * downSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(Tags.Balance))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            return;
        }

        if (collision.gameObject.CompareTag(Tags.Jewel))
        {
            JewelManager.Instance.AddJewel(collision.gameObject);
        }
    }
}
