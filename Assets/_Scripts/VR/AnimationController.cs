using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    Vector3 lastPos;
    float distance;

    [SerializeField] float moveMin;  //minimum amount of distance made to start walking animation

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);
    }
    private void FixedUpdate()
    {
        lastPos = transform.position;
    }
    void Update()
    {
        distance = Vector3.Distance(transform.position, lastPos);
        //Debug.Log("Distance " + distance);
        /* distance != 0 required as atleast every other frame calculates 0 distance and spams the animation boolean on and off
        and wrecks the walking animation, when player is standing still there's still camera movement thats 10^-5*/
        if (distance != 0) 
        {
            if (distance > moveMin)
            {
                if (animator.GetBool("isMoving") == false)
                {
                    animator.SetBool("isMoving", true);
                }
            }
            else { animator.SetBool("isMoving", false); }
        }
        
    }
    
}

