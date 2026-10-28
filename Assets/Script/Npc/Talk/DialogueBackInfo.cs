using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueBackInfo", menuName = "Dialogue/BackInfo")]
public class DialogueBackInfo : ScriptableObject
{
    [SerializeField] private BackInfoType backInfoType;

    [SerializeField] private GameObject skyObject;
    [SerializeField] private GameObject wallObject;
    [SerializeField] private GameObject frontObject;

    public BackInfoType Id => backInfoType;
    public GameObject SkyObject => skyObject;
    public GameObject WallObject => wallObject;
    public GameObject FrontObject => frontObject;
}

[System.Serializable]
public enum BackInfoType
{
    DayRestaurant,
    NightRestaurant,
    Town,
    Lodging,
}
