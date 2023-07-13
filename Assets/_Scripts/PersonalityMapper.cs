using UnityEngine;
using System.Collections;
using UnityEngine.AI;


//[RequireComponent (typeof (AffectComponent))]
public class PersonalityMapper: MonoBehaviour 
{
	public void PersonalityToSteering()
	{
					
		float[] personality = this.GetComponent<AffectComponent>().Personality;
		NavMeshAgent navMeshAgent = transform.GetComponent<NavMeshAgent>();


		//Max speed = [0.3 0.7], E //regular walking speed
		//GetComponent<NavMeshAgent>().speed = GetComponent<AgentComponent>().WalkingSpeed = 0.75f + 0.32f * personality[(int)OCEAN.E]; //Adjusts current speed as well

		navMeshAgent.angularSpeed = 120;
        
//	    float maxSpeed = 0.5f;
	//    float minSpeed = 0.3f;
        float maxSpeed = 1.6f;//1.4f;
	    float minSpeed = 1.0f;// 1.0f;
		navMeshAgent.speed = GetComponent<AgentComponent>().WalkingSpeed = ((0.5f* personality[(int)OCEAN.E] + 0.5f) * (maxSpeed - minSpeed) + minSpeed); //Adjusts current speed as well

		navMeshAgent.acceleration = 1f;

        //Waiting duration [11 21] frames
      	GetComponent<AgentComponent>().WaitDuration = 5f * personality[(int)OCEAN.A] + 16f;

      //High priority agents do NOT change their path -- no or little avoidance. If equal priority, both change paths. !!!!Low number means high priority!!!
      // [98 1] E, 1/A --> 0 and 99 are reserved for fallen and pushing agents resp.
	    int maxAvoidance = 98;
	    int minAvoidance = 1;
		navMeshAgent.avoidancePriority = (int)((float)maxAvoidance - (float)(maxAvoidance - minAvoidance) * ((0.5f * personality[(int)OCEAN.E] + 0.5f) + (-0.5f * personality[(int)OCEAN.A] + 0.5f)));

		navMeshAgent.avoidancePriority = 1;

        //radius [0.2 0.1] //should be small otherwise they oscillate
	    //float minRadius = 0.3f;
	    //float maxRadius = 0.8f;
        float minRadius = 0.3f;//0.4f;
        float maxRadius = 0.6f;//0.7f;
		navMeshAgent.radius = minRadius + (maxRadius - minRadius) * (-0.5f * personality[(int)OCEAN.E] + 0.5f);
        
        //[0.2 0.6] may work better because otherwise they oscillate too much
      
	//	GetComponent<AgentComponent>().PanicThreshold = 0.5f * (-0.5f*personality[(int)OCEAN.N]  + 0.5f) + 0.5f * (0.5f * personality[(int)OCEAN.C] +0.5f);


        //GetComponent<AgentComponent>().WaitingThreshold = 0.5f * (-0.5f * Personality[(int)OCEAN.N] + 0.5f) + 0.5f * (0.5f * Personality[(int)OCEAN.C] + 0.5f);

    
        
		//  Pushiness = [low, med, high], E, 1/A
	    //	float pushVal = Personality[(int)OCEAN.E] + 1f - Personality[(int)OCEAN.A];
			
	    /*
		//Change mesh color according to Personality
		Color persColor = Color.white;
		persColor.r = persColor.g = persColor.b = (Personality[(int)OCEAN.E]+1f)/2.0f;
		transform.GetComponent<MeshRenderer>().material.color = persColor;
	    */	
	}
}
