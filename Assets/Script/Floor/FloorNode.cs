using UnityEngine;

[System.Serializable]
public class FloorNode
{
    private Vector3Int privateCellPos;
    private bool privateIsAnimalArea; // 울타리 안에 포함되었는지 여부

    public FloorNode(Vector3Int pos)
    {
        privateCellPos = pos;
        PrivateIsAnimalArea = false;
    }

    public bool PrivateIsAnimalArea { get => privateIsAnimalArea; set => privateIsAnimalArea = value; }
}