using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsleVolumeManager : MonoBehaviour
{
    public CountShoppersInIsle[] isleVolumes = new CountShoppersInIsle[10];
    // Start is called before the first frame update
    void Start()
    {
        isleVolumes = GetComponentsInChildren<CountShoppersInIsle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public List<ShopperBehavior> GetShoppersInIsle(int index)
    {
        return isleVolumes[index].ShopperList;
    }
    */
}
