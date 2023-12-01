using UnityEngine;

[CreateAssetMenu(fileName ="Game", menuName ="Game/GameSetting", order = 1)]
public class GameSetting : ScriptableObject
{
    [Header("ƽ��Ƕ�����")] public float angleUpperLimit = 15f;
    [Header("���ƴ洢����")] public int cardStorageUpperLimit = 5;
    [Header("������Χʱ������")] public float outOfRangeTimeLimit = 5f;

    [Header("��������ʱ����")] public float spawnTimeSpacing = 1f;

    [Header("��ұ�ʯ�����ӳ�")] public float playerJewelSpawnDelay = 1f;
    [Header("���Ա�ʯ�����ӳ�")] public float enemyJewelSpawnDelay = 5f;
}
