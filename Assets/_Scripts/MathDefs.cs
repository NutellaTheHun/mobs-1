using UnityEngine;
using System.Collections;
using System;

public static class MathDefs  {
	
	private static System.Random rand = new System.Random();

    public static float Truncate(this float value, int digits) {
        double mult = Math.Pow(10.0, digits);
        double result = Math.Truncate(mult * value) / mult;
        return (float)result;
    }

    /// <summary>
    /// LogN Distribution with Box-Muller transform
    /// </summary>
    public static float LogNDist(float mean, float std) {

        float val = GaussianDist(mean, std);
        return Mathf.Exp(val);

    }

	/// <summary>
	/// Gaussian Distribution with Box-Muller transform
	/// </summary>
	public static float GaussianDist(float mean, float std) 
	{
		
		double u1 = rand.NextDouble(); //uniform(0,1) random doubles
		double u2 = rand.NextDouble();
		float randStdNormal = (float)Math.Sqrt(-2.0 * Math.Log(u1)) * (float)Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
		float randNormal = mean + std * randStdNormal; //random normal(mean,stdDev^2)						
		return randNormal;
			
	}
		
	public static int GetRandomNumber(int max) {		
		return rand.Next (max);		
	}

    public static float GetRandomNumber(float max) {
        return (float)rand.NextDouble() * max;
    }
    public static int GetRandomNumber(int min, int max) {
        return rand.Next(min, max);
    }
    public static float GetRandomNumber(float min, float max) {
        return min + (float)rand.NextDouble() * (max - min);
    }

    public static Vector3 ProjectOnPlane(Vector3 v, Vector3 normal) {
        return v - Vector3.Project(v, normal);
    }

    public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f) {
            return 1f;
        }
        else if (dir < 0f) {
            return -1f;
        }
        else {
            return 0f;
        }
    }

    /*
     * Returns an angle between -180 and 180 degrees
     * the vector that we want to measure an angle from
     * Vector3 referenceForward = // some vector that is not Vector3.up 
     * Vector3 newDirection =  some vector that we're interested in 
     */ 
    public static float VectorAngle(Vector3 referenceForward, Vector3 newDirection) {
        // the vector perpendicular to referenceForward (90 degrees clockwise)
        // (used to determine if angle is positive or negative)
        
        Vector3 referenceRight= Vector3.Cross(Vector3.up, referenceForward);
        // Get the angle in degrees between 0 and 180
        float angle = Vector3.Angle(newDirection, referenceForward);
 
        // Determine if the degree value should be negative.  Here, a positive value
        // from the dot product means that our vector is the right of the reference vector   
        // whereas a negative value means we're on the left.
        float sign = (Vector3.Dot(newDirection, referenceRight) > 0.0f) ? -1.0f: 1.0f;
 
        float finalAngle = sign * angle;

        return finalAngle;
    
    }

	public static float GetLength(float[] v)
	{
	    float[] vec = {v[0], v[1], v[2], 0};
	
		vec[0] *= vec[0];
		vec[1] *= vec[1];
		vec[2] *= vec[2];
	
		return (float) Math.Sqrt(vec[0] + vec[1] + vec[2]);
	}
	
	/// <summary>
	/// Normalize all elements of the array arr to [min, max]
	/// </summary>
	public static void NormalizeElements(float[] arr, float min, float max)
	{
		int i;
		float arrMin, arrMax;
		float diff;
		
		arrMin = arrMax = arr[0];		
		for(i=0; i<arr.Length; i++) {		
			if(arr[i] < arrMin)
				arrMin = arr[i];
			if(arr[i] > arrMax)
				arrMax = arr[i];
		}
	
		
		diff = (arrMax - arrMin);
		
		if(arrMax <= max && arrMin >= min) //if they are already in the range [min max] do not change the array
			return;
		
		if(arrMax == arrMin) {
			if(arrMax == min)
				return; //all = min = max
			else if(arrMax > min && arrMax < max ) //[0 1] region
				return;
			else if(arrMax > max) { //clamp to 1
				for(i=0; i<arr.Length; i++)	
					arr[i] = max;
			}
			else if(arrMax < min) {  //clamp to 0
				for(i=0; i<arr.Length; i++)	
					arr[i] = min;
			}
			
		}
				
				
		
		for(i=0; i<arr.Length; i++)		
			arr[i] = (arr[i] - arrMin) / diff;
	}
	
	/// Returns the array length
	public static float  Length(float [] arr)
	{
		float len = 0;
		
		for(int i = 0 ; i < arr.Length; i++)
			len += arr[i] * arr[i];
		
		
		return Mathf.Sqrt (len);
	}

    //Get an average (mean) from more then two quaternions (with two, slerp would be used).
    //Note: this only works if all the quaternions are relatively close together.
    //Usage:
    //-Cumulative is an external Vector4 which holds all the added x y z and w components.
    //-newRotation is the next rotation to be added to the average pool
    //-firstRotation is the first quaternion of the array to be averaged
    //-addAmount holds the total amount of quaternions which are currently added
    //This function returns the current average quaternion
    public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount) {

        float w = 0.0f;
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        //Before we add the new rotation to the average (mean), we have to check whether the quaternion has to be inverted. Because
        //q and -q are the same rotation, but cannot be averaged, we have to make sure they are all the same.
        if (!AreQuaternionsClose(newRotation, firstRotation)) {

            newRotation = InverseSignQuaternion(newRotation);
        }

        //Average the values
        float addDet = 1f / (float)addAmount;
        cumulative.w += newRotation.w;
        w = cumulative.w * addDet;
        cumulative.x += newRotation.x;
        x = cumulative.x * addDet;
        cumulative.y += newRotation.y;
        y = cumulative.y * addDet;
        cumulative.z += newRotation.z;
        z = cumulative.z * addDet;

        //note: if speed is an issue, you can skip the normalization step
        return NormalizeQuaternion(x, y, z, w);
    }

    public static Quaternion NormalizeQuaternion(float x, float y, float z, float w) {

        float lengthD = 1.0f / (w * w + x * x + y * y + z * z);
        w *= lengthD;
        x *= lengthD;
        y *= lengthD;
        z *= lengthD;

        return new Quaternion(x, y, z, w);
    }

    //Changes the sign of the quaternion components. This is not the same as the inverse.
    public static Quaternion InverseSignQuaternion(Quaternion q) {

        return new Quaternion(-q.x, -q.y, -q.z, -q.w);
    }

    //Returns true if the two input quaternions are close to each other. This can
    //be used to check whether or not one of two quaternions which are supposed to
    //be very similar but has its component signs reversed (q has the same rotation as
    //-q)
    public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2) {

        float dot = Quaternion.Dot(q1, q2);

        if (dot < 0.0f) {

            return false;
        }

        else {

            return true;
        }
    }
 
}
