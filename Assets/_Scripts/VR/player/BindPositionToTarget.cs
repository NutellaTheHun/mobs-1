using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindPositionToTarget : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] bool offset;
    [SerializeField] float offsetValue;
    void FixedUpdate()
    {
        
        if(offset)
        {
            transform.position = target.transform.position - new Vector3(0,offsetValue,0);
        }
        else
        {
            transform.position = target.transform.position;
        }
    }
}
