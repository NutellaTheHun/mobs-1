using UnityEngine;

[CreateAssetMenu(fileName = "IsleCountData", menuName = "ScriptableObject/IsleCountData")]
public class IsleCountData : ScriptableObject
{
    public int IslePerShopperMax;
    public int[] ShopperCountInIsle = new int[11];

    private void OnEnable() => ShopperCountInIsle = new int[11];
}

