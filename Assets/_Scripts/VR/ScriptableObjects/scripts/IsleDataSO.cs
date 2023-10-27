using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IsleData", menuName = "ScriptableObject/IsleData")]
public class IsleDataSO : ScriptableObject
{
    public const int NUMOFISLES = 11;

    //when shopper enters isle, subscribe to [isleIndex]List
    public List<ShopperBehavior>[] shoppers = new List<ShopperBehavior>[NUMOFISLES];
    public int[] shoppersCount = new int[NUMOFISLES];
    public List<ObjComponent>[] ipads = new List<ObjComponent>[NUMOFISLES];
    public int[] ipadCount = new int[NUMOFISLES];
    public List<ShopperBehavior> frontLane = new List<ShopperBehavior>();
    public List<ShopperBehavior> backLane = new List<ShopperBehavior>();

    //UnityEvent IsleDataRegisterShopper;
    //UnityEvent IsleDataDeRegisterShopper;

    //UnityEvent IpadConsumed;

    private void OnEnable()
    {
        shoppers = new List<ShopperBehavior>[NUMOFISLES];
        for(int i = 0; i < NUMOFISLES; i++)
        {
            shoppers[i] = new List<ShopperBehavior>();
        }
        shoppersCount = new int[NUMOFISLES];

        ipads = new List<ObjComponent>[NUMOFISLES];
        for (int i = 0; i < NUMOFISLES; i++)
        {
            ipads[i] = new List<ObjComponent>();
        }
        ipadCount = new int[NUMOFISLES];

        frontLane = new List<ShopperBehavior>();
        backLane = new List<ShopperBehavior>();

        //initializeIpads();
    }

    public void initializeIpads()
    {
        ObjComponent[] sceneIpads = FindObjectsOfType<ObjComponent>();
        foreach (ObjComponent ipad in sceneIpads)
        {
            ipads[ipad.isleIndex].Add(ipad);
            ipadCount[ipad.isleIndex]++;
        }
    }

    public void RegisterShopper(ShopperBehavior sb, CountShoppersInLane.LaneType lane)
    {
        switch (lane)
        {
            case CountShoppersInLane.LaneType.front:
                if (!frontLane.Contains(sb)) frontLane.Add(sb);
                break;
            case CountShoppersInLane.LaneType.back:
                if (!frontLane.Contains(sb)) backLane.Add(sb);
                break;
        }
    }
    public void DeRegisterShopper(ShopperBehavior sb, CountShoppersInLane.LaneType lane)
    {
        switch (lane) 
        {
            case CountShoppersInLane.LaneType.front:
                if (frontLane.Contains(sb)) frontLane.Remove(sb);
                break;
            case CountShoppersInLane.LaneType.back:
                if (frontLane.Contains(sb)) backLane.Remove(sb);
                break;
        }
    }
    public void RegisterShopper(ShopperBehavior sb, int index)
    {
        if (!shoppers[index].Contains(sb))
        {
            shoppers[index].Add(sb);
            shoppersCount[index]++;
        }

    }

    public void DeRegisterShopper(ShopperBehavior sb, int index)
    {
        if (shoppers[index].Contains(sb))
        {
            shoppers[index].Remove(sb);
            shoppersCount[index]--;
        }
    }

    public void RemoveIpad(ObjComponent ipad, int index)
    {
        if (ipads[index].Contains(ipad))
        {
            ipads[index].Remove(ipad);
            ipadCount[index]--;
        }
    }
   
}
