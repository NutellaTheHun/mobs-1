using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static ShopperBehavior;

public class DetermineAlternatePath : MonoBehaviour
{
    [SerializeField] SphereCollider LeftColliderArea;
    [SerializeField] SphereCollider RightColliderArea;
    

    public bool LeftOk;
    public bool RightOk;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public Vector3 MoveOutOfTheWay(ShopperBehavior.ShoppingState state)
    {
        if (state == ShoppingState.ShelfChanging)
        {
            //move left, or right, or pause
            if (LeftOk && RightOk) 
            {
                //Go left or right with some preference?
                int rand = UnityEngine.Random.Range(0, 1);
                if (rand == 1)
                {
                    return LeftColliderArea.transform.position;
                }
                else
                {
                    return RightColliderArea.transform.position;
                }
            }
            else if(LeftOk)
            {
                //go left
                return LeftColliderArea.transform.position;
            }
            else if(RightOk)
            {
                //go right
                return RightColliderArea.transform.position;
            }
            else
            {
                //pause
                return Vector3.zero;
            }
        }
        if (state == ShoppingState.GoingToObject)
        {
            //go to center of isle, go ahead or switch side
            if(LeftOk && RightOk)
            {
                int rand = UnityEngine.Random.Range(0, 1);
                //Go left or right with some preference? most likely rarely triggered
                if (rand == 1)
                {
                    return LeftColliderArea.transform.position;
                }
                else
                {
                    return RightColliderArea.transform.position;
                }
            }
            else if (LeftOk)
            {
                //go left
                return LeftColliderArea.transform.position;
            }
            else if (RightOk)
            {
                //go right
                return RightColliderArea.transform.position;
            }
            else
            {
                //pause
                return Vector3.zero;
            }
        }
        if (state == ShoppingState.GoingToLine)
        {
            //move left, or right, or pause
            if(LeftOk && RightOk)
            {
                //Go left or right with some preference?
                int rand = UnityEngine.Random.Range(0, 1);
                //Go left or right with some preference? most likely rarely triggered
                if (rand == 1)
                {
                    return LeftColliderArea.transform.position;
                }
                else
                {
                    return RightColliderArea.transform.position;
                }
            }
            else if (LeftOk)
            {
                //go left
                return LeftColliderArea.transform.position;
            }
            else if (RightOk)
            {
                //go right
                return RightColliderArea.transform.position;
            }
            else
            {
                //pause
                return Vector3.zero;
            }
        }
        return Vector3.zero;
    }
}
