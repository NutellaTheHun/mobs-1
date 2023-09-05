using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shelves -> ObjectsX -> IpadX_Y
[CreateAssetMenu(fileName = "IpadCountData", menuName = "ScriptableObject/IpadCountData")]
public class IpadCountData : ScriptableObject
{
    public int NumberOfIsles;
    public int[] Isle = new int[1];

    private void OnEnable()
    {
        Isle = new int[NumberOfIsles];
    }
}
