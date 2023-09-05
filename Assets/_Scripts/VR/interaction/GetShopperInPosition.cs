using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShopperBehavior;

public class GetShopperInPosition : MonoBehaviour
{
    public LineHandler lineHandler;
    [SerializeField] private State state;
    private ShopperBehavior shopper;
    private List<ShopperBehavior> ExitingShoppers = new List<ShopperBehavior>();
    enum State
    {
        PayingPosition,
        WaitingPosition
    }
   
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LinePositionCollision"))
        {
            //Debug.Log(other.name);
            shopper = other.transform.parent.GetComponent<ShopperBehavior>();
            if (shopper.State == (int)ShoppingState.GoingToLine || shopper.State == (int)ShoppingState.WaitingInLine || shopper.State == (int)ShoppingState.Paying)
                switch (state)
                {
                case State.PayingPosition:
                    if(!ExitingShoppers.Contains(shopper))
                    {
                        if (lineHandler.PayingShopper == null)
                        {
                            lineHandler.PayingShopper = shopper;
                        }
                    }
                    break;

                case State.WaitingPosition:
                    if (lineHandler.ShopperAtFrontOfLine == null)
                    {
                        lineHandler.ShopperAtFrontOfLine = shopper;
                    }
                    break;

                default:

                break;
                }
        }
        /*else
        {
            shopper = null;
            switch (state)
            {
                case State.PayingPosition:
                    lineHandler.PayingShopper = null;
                    break;

                case State.WaitingPosition:
                    lineHandler.ShopperAtFrontOfLine = null;
                    break;

                default:

                    break;
            }
        }*/
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LinePositionCollision"))
        {
            /*if (shopper.State == (int)ShoppingState.Paying)
            {
                ExitingShoppers.Add(other.transform.parent.GetComponent<ShopperBehavior>());
            }*/
            switch (state)
            {
                case State.PayingPosition:

                    if (lineHandler.PayingShopper == other.transform.parent.GetComponent<ShopperBehavior>())
                    {
                        lineHandler.PayingShopper = null;
                    }
                    break;

                case State.WaitingPosition:

                    if (lineHandler.ShopperAtFrontOfLine == other.transform.parent.GetComponent<ShopperBehavior>())
                    {
                        lineHandler.ShopperAtFrontOfLine = null;
                    }
                    break;

                default:

                    break;
            }
        }
    }
}
