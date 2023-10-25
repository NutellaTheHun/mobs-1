using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsleCountData", menuName = "ScriptableObject/IsleCountData")]
public class IsleCountData : ScriptableObject
{
    public int IslePerShopperMax;
    public int[] ShopperCountInIsle = new int[11];
    public List<ShopperBehavior> ShopperBehaviors;

    private void OnEnable() => ShopperCountInIsle = new int[11];
}

