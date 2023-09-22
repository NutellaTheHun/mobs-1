using UnityEngine;
using static ShopperBehavior;

public class ShopperBehindInLine : MonoBehaviour
{
    [SerializeField] LineHandler lineHandler;
    private ShopperBehavior ParentShopper;
    // Start is called before the first frame update
    void Start()
    {
        lineHandler = GameObject.Find("counter").GetComponent<LineHandler>();
        ParentShopper = transform.parent.GetComponent<ShopperBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ParentShopper.State == (int)ShoppingState.WaitingInLine & ParentShopper.ShopperBehindInLine == null) 
        {
            if (other.CompareTag("Player"))
            {
                ShopperBehavior otherShopper = other.GetComponent<ShopperBehavior>();
                if (otherShopper.State == (int)ShoppingState.GoingToLine || otherShopper.State == (int)ShoppingState.WaitingInLine)
                {
                    ParentShopper.ShopperBehindInLine = otherShopper;
                    otherShopper.ShopperAheadInLine = ParentShopper;
                    otherShopper.isWaitingBehindSomeone = true;
                    //lineHandler.UpdateEndOfLine(otherShopper);
                }
            }
        }
        
    }
}
