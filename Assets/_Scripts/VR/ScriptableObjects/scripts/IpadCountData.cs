using UnityEngine;

//Shelves -> ObjectsX -> IpadX_Y
[CreateAssetMenu(fileName = "IpadCountData", menuName = "ScriptableObject/IpadCountData")]
public class IpadCountData : ScriptableObject
{
    public int NumberOfIsles;
    public IsleData[] Isle;

    public void removeIpad(int isleIndex, ObjComponent.ShelfSide sideOfIsle)
    {
        if(sideOfIsle == ObjComponent.ShelfSide.Left)
        {
            Isle[isleIndex]._left--;
        }
        else
        {
            Isle[isleIndex]._right--;
        }
        Isle[isleIndex]._total--;
    }

    private void OnEnable()
    {
        Isle = new IsleData[NumberOfIsles];
    }
}
