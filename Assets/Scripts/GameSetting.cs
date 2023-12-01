using UnityEngine;

[CreateAssetMenu(fileName ="Game", menuName ="Game/GameSetting", order = 1)]
public class GameSetting : ScriptableObject
{
    [Header("平衡角度上限")] public float angleUpperLimit = 15f;
    [Header("卡牌存储上限")] public int cardStorageUpperLimit = 5;
    [Header("超出范围时间上限")] public float outOfRangeTimeLimit = 5f;

    [Header("卡牌生成时间间隔")] public float spawnTimeSpacing = 1f;

    [Header("玩家宝石生成延迟")] public float playerJewelSpawnDelay = 1f;
    [Header("电脑宝石生成延迟")] public float enemyJewelSpawnDelay = 5f;
}
