using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountShoppersInIsle : MonoBehaviour
{
    [SerializeField] IsleCountData _isleCountData;
    [SerializeField] private int Index;
    public List<ShopperBehavior> ShopperList;
    // Start is called before the first frame update
    void Start()
    {
        ShopperList = new List<ShopperBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LinePositionCollision")) //Was using "PlayerTag" but some component initialized a sphere with radius 4 of that tag so using an already implemented collider
        {
            if (ShopperList.Contains(other.transform.parent.GetComponent<ShopperBehavior>()) == false)
            {
                ShopperList.Add(other.transform.parent.GetComponent<ShopperBehavior>());
                _isleCountData.ShopperCountInIsle[Index] = ShopperList.Count;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LinePositionCollision"))
        {
            if (ShopperList.Contains(other.transform.parent.GetComponent<ShopperBehavior>()) == true)
            {
                ShopperList.Remove(other.transform.parent.GetComponent<ShopperBehavior>());
                _isleCountData.ShopperCountInIsle[Index] = ShopperList.Count;
            }
        }
    }
}
