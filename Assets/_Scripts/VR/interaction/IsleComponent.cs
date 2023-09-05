using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsleComponent : MonoBehaviour
{
    [SerializeField] public int IsleIndex;
    [SerializeField] IpadCountData _IpadCountData;
    private ObjComponent[] IpadList;

    // Start is called before the first frame update
    void Start()
    {
        IpadList = GetComponentsInChildren<ObjComponent>();
        _IpadCountData.Isle[IsleIndex] = GetIpadCount();
    }

    private int GetIpadCount()
    {
        return IpadList.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateIsleCount()
    {
        _IpadCountData.Isle[IsleIndex]--;
    }
}
