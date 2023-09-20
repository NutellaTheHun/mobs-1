
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class FaceAnimator : MonoBehaviour
{

	[HideInInspector] public bool freeze;
	[HideInInspector] public float expressFactor;

	[HideInInspector] public bool talkingNow;

	[HideInInspector] public SkinnedMeshRenderer meshRenderer;
	[HideInInspector] public SkinnedMeshRenderer meshRendererEyes;
	[HideInInspector] public SkinnedMeshRenderer meshRendererEyelashes;


	private int shapeKeyCount;

	private float[] values;
	private float[] valuesDisp;
	[HideInInspector] public float[] targets;
	private float[] speeds;


	//Mixamo characters
	private int blink_left = 0;
	private int blink_right = 1;
	private int browsDown_left = 2;
	private int browsDown_right = 3;
	private int browsIn_left = 4;
	private int browsIn_right;
	private int browsOuterLower_left = 5;
	private int browsOuterLower_right = 6;
	private int browsUp_left = 7;
	private int browsUp_right = 8;
	private int cheekPuff_left = 9;
	private int cheekPuff_right = 10;
	private int eyesWide_left = 11;
	private int eyesWide_right = 12;
	private int frown_left = 13;
	private int frown_right = 14;
	private int jawBackward = 15;
	private int jawForeward = 16;
	private int jawRotateY_left = 17;
	private int jawRotateY_right = 18;	
	private int jawRotateZ_left;
	private int jawRotateZ_right = 19;

	private int jawDown = 20;
	private int jawLeft = 21;
	private int jawRight = 22;
	private int jawUp = 23;
	private int lowerLipDown_left = 24;
	private int lowerLipDown_right = 25;
	private int lowerLipIn = 26;
	private int lowerLipOut = 27;
	private int midmouth_left = 28;
	private int midmouth_right = 29;
	private int mouthDown = 30;
	private int mouthNarrow_left = 31;
	private int mouthNarrow_right = 32;
	private int mouthOpen = 33;
	private int mouthUp = 34;
	private int mouthWhistle_left = 35;
	private int mouthWhistle_right = 36;
	private int noseScrunch_left = 37;
	private int noseScrunch_right = 38;
	private int smileLeft = 39;
	private int smileRight = 40;
	private int squint_left = 41;
	private int squint_right = 42;
	private int toungeUp = 43;
	private int upperLipIn = 44;
	private int upperLipOut = 45;
	private int upperLipUp_left = 46;
	private int upperLipUp_right = 47;

	[Header("Expression Parameters")]
	//Takes a value between 0 and 100
	public float[] expressions; //Order is happy, sad, angry, afraid, neutral, disgust, shock



	public bool blinkOff = false;


	public float blinkTimer;
	public float openTimer;
	public bool blinkInProgress;
	public int blinkState;

	const int NONE = 0;
	const int CLOSING = 1;
	const int OPENING = 2;

	[HideInInspector] public float blinkCloseSpeed;
	[HideInInspector] public float blinkOpenSpeed;


	private void Awake()
	{
		Init();
	}

	private void AssignBlendShapes(){

		if(gameObject.name.Contains("Pass") || gameObject.name.Contains("McPerson") ||  gameObject.name.Contains("Agent")){

			blink_left = 0;
			blink_right = 1;
			browsDown_left = 2;
			browsDown_right = 3;
			browsIn_left = 4;
			browsIn_right = 5;
			browsOuterLower_left = 6;
			browsOuterLower_right = 7;
			browsUp_left = 8;
			browsUp_right = 9;
			cheekPuff_left = 10;
			cheekPuff_right = 11;
			eyesWide_left = 12;
			eyesWide_right = 13;
			frown_left = 14;
			frown_right = 15;
			jawBackward = 16;
			jawForeward = 17;
			jawRotateY_left = 18;
			jawRotateY_right = 19;
			jawRotateZ_left = 20;
			jawRotateZ_right = 21;
			jawDown = 22;
			jawLeft = 23;
			jawRight = 24;
			jawUp = 25;
			lowerLipDown_left = 26;
			lowerLipDown_right = 27;
			lowerLipIn = 28;
			lowerLipOut = 29;
			midmouth_left = 30;
			midmouth_right = 31;
			mouthDown = 32;
			mouthNarrow_left = 33;
			mouthNarrow_right = 34;
			mouthOpen = 35;
			mouthUp = 36;
			mouthWhistle_left = 37;
			mouthWhistle_right = 38;
			noseScrunch_left = 39;
			noseScrunch_right = 40;
			smileLeft = 41;
			smileRight = 42;
			squint_left = 43;
			squint_right = 44;
			toungeUp = 45;
			upperLipIn = 46;
			upperLipOut = 47;
			upperLipUp_left = 48;
			upperLipUp_right = 49;
		}

	}


	public void Init()
	{
		AssignBlendShapes();
		expressions = new float[7];
		blinkInProgress = false;
		blinkState = NONE;

		meshRenderer = transform.Find("Body").GetComponent<SkinnedMeshRenderer>();

		//if(!meshRenderer)//Frank //TODO 
		//        meshRenderer = transform.Find("CC_Base_Body").GetComponent<SkinnedMeshRenderer>();

		//meshRendererEyes = transform.Find("Eyes").GetComponent<SkinnedMeshRenderer>();
		//meshRendererEyelashes = transform.Find("Eyelashes").GetComponent<SkinnedMeshRenderer>();

		values = new float[50];
		for (int i = 0; i < 50; i++)
		{
			values[i] = 0.0f;
		}

		valuesDisp = new float[50];
		for (int i = 0; i < 50; i++)
		{
			valuesDisp[i] = 0.0f;
		}

		targets = new float[50];
		for (int i = 0; i < 50; i++)
		{
			targets[i] = 0.0f;
		}

		speeds = new float[50];
		for (int i = 0; i < 50; i++)
		{
			speeds[i] = 3.0f;
		}

		blinkCloseSpeed = 12f;
		blinkOpenSpeed = 8f;


		expressFactor = 1f;

	}


	private void FixedUpdate()
	{

		UpdateExpressions();

		//Funda -- will be handled by the data
		if (!blinkOff) Blink();
		ExpressionPass();




		StepTargets();
		SetShapeKeys();

	}

	void UpdateExpressions()
	{
		int expressionInd = GetComponent<AffectComponent>().GetExpressionInd();
		float expressionVal = GetComponent<AffectComponent>().GetExpressionValue();

		for (int i = 0; i < 6; i++)
		{
			if (i != expressionInd)
				expressions[i] = 0;
		}
		expressions[expressionInd] = 100 * expressionVal;


	}

	public void UpdateEyelids(float y)
	{

		if (blinkInProgress)
			return;
		/////Sets targets immediately
		//
		//values[blink_left] = 90 - 180*y;
		//values[blink_right] = 90 - 180 * y;

		//targets[blink_left] = 90 - 180 * y;
		//targets[blink_right] = 90 - 180 * y;

		/////Sets targets immediately
		if (y < 0.5)
		{
			values[blink_left] = targets[blink_left] = 30 - 60 * y;
			values[blink_right] = targets[blink_right] = 30 - 60 * y;
		}
		else
		{
			values[blink_left] = targets[blink_left] = 20 - 40 * y;
			values[blink_right] = targets[blink_right] = 20 - 40 * y;
		}


	}


	public void Blink()
	{

		// TODO
		//blinkCloseSpeed = 100f / (0.1f / Time.fixedDeltaTime); //20f; //100/20 = 5 fixed updates --> 5 * 0.02sec = 0.1sec is avg blink ratio // speed = 16.66
		//blinkOpenSpeed = blinkCloseSpeed * 2f;//blinkCloseSpeed/2f;


		//blink_max = 4f;
		//blink_min = 2f;

		float targetPercentage = 90f;

		if (!blinkInProgress)
			return;
		//blinkTimer -= 1f;//Time.deltaTime;
		//openTimer -= 1f;//Time.deltaTime;

		blinkTimer -= Time.fixedDeltaTime;
		openTimer -= Time.fixedDeltaTime;

		// time to blink
		//    if(!blinkingNow && blinkTimer <= 0) {
		if (blinkState == NONE)
		{  //start closing

			targets[blink_left] = targetPercentage;
			targets[blink_right] = targetPercentage;
			speeds[blink_left] = blinkCloseSpeed * targetPercentage;
			speeds[blink_right] = blinkCloseSpeed * targetPercentage;


			//blinkTimer = 100f / blinkCloseSpeed;//0.2f; //100/(12 + 8)
			blinkTimer = 1f / blinkCloseSpeed;//0.2f; //100/(12 + 8)


			blinkState = CLOSING;

		}

		//if(blinkingNow && blinkTimer <= 0) {
		if (blinkState == CLOSING && blinkTimer <= 0)
		{//closing state complete
			targets[blink_left] = 0f;
			targets[blink_right] = 0f;
			speeds[blink_left] = blinkOpenSpeed * targetPercentage;
			speeds[blink_right] = blinkOpenSpeed * targetPercentage;




			//openTimer = 100f / blinkOpenSpeed;
			openTimer = 1f / blinkOpenSpeed;


			blinkState = OPENING;



			//blinkInProgress = false; //opening the eyes continues
			//blinkInProgress = false;

		}

		if (blinkState == OPENING && openTimer <= 0)
		{ //opening state complete;
			blinkState = NONE;
			blinkInProgress = false;


		}



		//if(blinkTimer <= 0)
		//blinkInProgress = false;
		//else {
		//    blinkInProgress = false;
		//    blinkTimer = 0.2f;
		//}
	}




	public void InitShapeKeys()
	{
		shapeKeyCount = meshRenderer.sharedMesh.blendShapeCount;
		Dictionary<string, int> shapeKeyDict_body = new Dictionary<string, int>();
		for (int i = 0; i < shapeKeyCount; i++)
		{
			shapeKeyDict_body.Add(meshRenderer.sharedMesh.GetBlendShapeName(i), i);
		}


		shapeKeyDict_body.Clear();
	}

	void StepTargets()
	{
		for (int i = 0; i < 50; i++)
		{
			if (Mathf.Abs(values[i] - targets[i]) <= speeds[i])
			{
				values[i] = targets[i];
			}
			else
			{
				values[i] -= (speeds[i] * Mathf.Sign(values[i] - targets[i]));
			}
		}

		if (Mathf.Abs(w_happy - expressions[(int)EkmanType.Happy]) <= 3)
		{
			w_happy = expressions[(int)EkmanType.Happy];
		}
		else
		{
			w_happy -= (3 * Mathf.Sign(w_happy - expressions[(int)EkmanType.Happy]));
		}

		if (Mathf.Abs(w_angry - expressions[(int)EkmanType.Angry]) <= 3)
		{
			w_angry = expressions[(int)EkmanType.Angry];
		}
		else
		{
			w_angry -= (3 * Mathf.Sign(w_angry - expressions[(int)EkmanType.Angry]));
		}

		//if (Mathf.Abs(w_shock - expressions[(int)EkmanType.Shock]) <= 3)
		//{
		//    w_shock = expressions[(int)EkmanType.Shock];
		//}
		//else
		//{
		//    w_shock -= (3 * Mathf.Sign(w_shock - expressions[(int)EkmanType.Shock]));
		//}

		if (Mathf.Abs(w_sad - expressions[(int)EkmanType.Sad]) <= 3)
		{
			w_sad = expressions[(int)EkmanType.Sad];
		}
		else
		{
			w_sad -= (3 * Mathf.Sign(w_sad - expressions[(int)EkmanType.Sad]));
		}


		if (Mathf.Abs(w_afraid - expressions[(int)EkmanType.Afraid]) <= 3)
		{
			w_afraid = expressions[(int)EkmanType.Afraid];
		}
		else
		{
			w_afraid -= (3 * Mathf.Sign(w_sad - expressions[(int)EkmanType.Afraid]));
		}
	}

	private void SetShapeKeys()
	{
		meshRenderer.SetBlendShapeWeight(blink_left, values[blink_left] + valuesDisp[blink_left]);
		meshRenderer.SetBlendShapeWeight(blink_right, values[blink_right] + valuesDisp[blink_right]);
		meshRenderer.SetBlendShapeWeight(browsDown_left, values[browsDown_left] + valuesDisp[browsDown_left]);
		meshRenderer.SetBlendShapeWeight(browsDown_right, values[browsDown_right] + valuesDisp[browsDown_right]);
		meshRenderer.SetBlendShapeWeight(browsIn_left, values[browsIn_left] + valuesDisp[browsIn_left]);
		//meshRenderer.SetBlendShapeWeight(browsIn_right, values[browsIn_right] + valuesDisp[browsIn_right]);
		meshRenderer.SetBlendShapeWeight(browsOuterLower_left, values[browsOuterLower_left] + valuesDisp[browsOuterLower_left]);
		meshRenderer.SetBlendShapeWeight(browsOuterLower_right, values[browsOuterLower_right] + valuesDisp[browsOuterLower_right]);
		meshRenderer.SetBlendShapeWeight(browsUp_left, values[browsUp_left] + valuesDisp[browsUp_left]);
		meshRenderer.SetBlendShapeWeight(browsUp_right, values[browsUp_right] + valuesDisp[browsUp_right]);
		meshRenderer.SetBlendShapeWeight(cheekPuff_left, values[cheekPuff_left] + valuesDisp[cheekPuff_left]);
		meshRenderer.SetBlendShapeWeight(cheekPuff_right, values[cheekPuff_right] + valuesDisp[cheekPuff_right]);
		meshRenderer.SetBlendShapeWeight(eyesWide_left, values[eyesWide_left] + valuesDisp[eyesWide_left]);
		meshRenderer.SetBlendShapeWeight(eyesWide_right, values[eyesWide_right] + valuesDisp[eyesWide_right]);
		meshRenderer.SetBlendShapeWeight(frown_left, values[frown_left] + valuesDisp[frown_left]);
		meshRenderer.SetBlendShapeWeight(frown_right, values[frown_right] + valuesDisp[frown_right]);
		meshRenderer.SetBlendShapeWeight(jawBackward, values[jawBackward] + valuesDisp[jawBackward]);
		meshRenderer.SetBlendShapeWeight(jawForeward, values[jawForeward] + valuesDisp[jawForeward]);
		meshRenderer.SetBlendShapeWeight(jawRotateY_left, values[jawRotateY_left] + valuesDisp[jawRotateY_left]);
		meshRenderer.SetBlendShapeWeight(jawRotateY_right, values[jawRotateY_right] + valuesDisp[jawRotateY_right]);
		//meshRenderer.SetBlendShapeWeight(jawRotateZ_left, values[jawRotateZ_left] + valuesDisp[jawRotateZ_left]);
		meshRenderer.SetBlendShapeWeight(jawRotateZ_right, values[jawRotateZ_right] + valuesDisp[jawRotateZ_right]);
		meshRenderer.SetBlendShapeWeight(jawDown, values[jawDown] + valuesDisp[jawDown]);
		meshRenderer.SetBlendShapeWeight(jawLeft, values[jawLeft] + valuesDisp[jawLeft]);
		meshRenderer.SetBlendShapeWeight(jawRight, values[jawRight] + valuesDisp[jawRight]);
		meshRenderer.SetBlendShapeWeight(jawUp, values[jawUp] + valuesDisp[jawUp]);
		meshRenderer.SetBlendShapeWeight(lowerLipDown_left, values[lowerLipDown_left] + valuesDisp[lowerLipDown_left]);
		meshRenderer.SetBlendShapeWeight(lowerLipDown_right, values[lowerLipDown_right] + valuesDisp[lowerLipDown_right]);
		meshRenderer.SetBlendShapeWeight(lowerLipIn, values[lowerLipIn] + valuesDisp[lowerLipIn]);
		meshRenderer.SetBlendShapeWeight(lowerLipOut, values[lowerLipOut] + valuesDisp[lowerLipOut]);
		meshRenderer.SetBlendShapeWeight(midmouth_left, values[midmouth_left] + valuesDisp[midmouth_left]);
		meshRenderer.SetBlendShapeWeight(midmouth_right, values[midmouth_right] + valuesDisp[midmouth_right]);
		meshRenderer.SetBlendShapeWeight(mouthDown, values[mouthDown] + valuesDisp[mouthDown]);
		meshRenderer.SetBlendShapeWeight(mouthNarrow_left, values[mouthNarrow_left] + valuesDisp[mouthNarrow_left]);
		meshRenderer.SetBlendShapeWeight(mouthNarrow_right, values[mouthNarrow_right] + valuesDisp[mouthNarrow_right]);
		meshRenderer.SetBlendShapeWeight(mouthOpen, values[mouthOpen] + valuesDisp[mouthOpen]);
		meshRenderer.SetBlendShapeWeight(mouthUp, values[mouthUp] + valuesDisp[mouthUp]);
		meshRenderer.SetBlendShapeWeight(mouthWhistle_left, values[mouthWhistle_left] + valuesDisp[mouthWhistle_left]);
		meshRenderer.SetBlendShapeWeight(mouthWhistle_right, values[mouthWhistle_right] + valuesDisp[mouthWhistle_right]);
		meshRenderer.SetBlendShapeWeight(noseScrunch_left, values[noseScrunch_left] + valuesDisp[noseScrunch_left]);
		meshRenderer.SetBlendShapeWeight(noseScrunch_right, values[noseScrunch_right] + valuesDisp[noseScrunch_right]);
		meshRenderer.SetBlendShapeWeight(smileLeft, values[smileLeft] + valuesDisp[smileLeft]);
		meshRenderer.SetBlendShapeWeight(smileRight, values[smileRight] + valuesDisp[smileRight]);
		meshRenderer.SetBlendShapeWeight(squint_left, values[squint_left] + valuesDisp[squint_left]);
		meshRenderer.SetBlendShapeWeight(squint_right, values[squint_right] + valuesDisp[squint_right]);
		meshRenderer.SetBlendShapeWeight(toungeUp, values[toungeUp] + valuesDisp[toungeUp]);
		meshRenderer.SetBlendShapeWeight(upperLipIn, values[upperLipIn] + valuesDisp[upperLipIn]);
		meshRenderer.SetBlendShapeWeight(upperLipOut, values[upperLipOut] + valuesDisp[upperLipOut]);
		meshRenderer.SetBlendShapeWeight(upperLipUp_left, values[upperLipUp_left] + valuesDisp[upperLipUp_left]);
		meshRenderer.SetBlendShapeWeight(upperLipUp_right, values[upperLipUp_right] + valuesDisp[upperLipUp_right]);

		if (meshRendererEyelashes != null)
		{
			meshRendererEyelashes.SetBlendShapeWeight(blink_left, values[blink_left] + valuesDisp[blink_left]);
			meshRendererEyelashes.SetBlendShapeWeight(blink_right, values[blink_right] + valuesDisp[blink_right]);
			meshRendererEyelashes.SetBlendShapeWeight(browsDown_left, values[browsDown_left] + valuesDisp[browsDown_left]);
			meshRendererEyelashes.SetBlendShapeWeight(browsDown_right, values[browsDown_right] + valuesDisp[browsDown_right]);
			meshRendererEyelashes.SetBlendShapeWeight(browsIn_left, values[browsIn_left] + valuesDisp[browsIn_left]);
			//meshRendererEyelashes.SetBlendShapeWeight(browsIn_right, values[browsIn_right] + valuesDisp[browsIn_right]);
			meshRendererEyelashes.SetBlendShapeWeight(browsOuterLower_left, values[browsOuterLower_left] + valuesDisp[browsOuterLower_left]);
			meshRendererEyelashes.SetBlendShapeWeight(browsOuterLower_right, values[browsOuterLower_right] + valuesDisp[browsOuterLower_right]);
			meshRendererEyelashes.SetBlendShapeWeight(browsUp_left, values[browsUp_left] + valuesDisp[browsUp_left]);
			meshRendererEyelashes.SetBlendShapeWeight(browsUp_right, values[browsUp_right] + valuesDisp[browsUp_right]);
			meshRendererEyelashes.SetBlendShapeWeight(cheekPuff_left, values[cheekPuff_left] + valuesDisp[cheekPuff_left]);
			meshRendererEyelashes.SetBlendShapeWeight(cheekPuff_right, values[cheekPuff_right] + valuesDisp[cheekPuff_right]);
			meshRendererEyelashes.SetBlendShapeWeight(eyesWide_left, values[eyesWide_left] + valuesDisp[eyesWide_left]);
			meshRendererEyelashes.SetBlendShapeWeight(eyesWide_right, values[eyesWide_right] + valuesDisp[eyesWide_right]);
			meshRendererEyelashes.SetBlendShapeWeight(frown_left, values[frown_left] + valuesDisp[frown_left]);
			meshRendererEyelashes.SetBlendShapeWeight(frown_right, values[frown_right] + valuesDisp[frown_right]);
			meshRendererEyelashes.SetBlendShapeWeight(jawBackward, values[jawBackward] + valuesDisp[jawBackward]);
			meshRendererEyelashes.SetBlendShapeWeight(jawForeward, values[jawForeward] + valuesDisp[jawForeward]);
			meshRendererEyelashes.SetBlendShapeWeight(jawRotateY_left, values[jawRotateY_left] + valuesDisp[jawRotateY_left]);
			meshRendererEyelashes.SetBlendShapeWeight(jawRotateY_right, values[jawRotateY_right] + valuesDisp[jawRotateY_right]);
			//meshRendererEyelashes.SetBlendShapeWeight(jawRotateZ_left, values[jawRotateZ_left] + valuesDisp[jawRotateZ_left]);
			meshRendererEyelashes.SetBlendShapeWeight(jawRotateZ_right, values[jawRotateZ_right] + valuesDisp[jawRotateZ_right]);
			meshRendererEyelashes.SetBlendShapeWeight(jawDown, values[jawDown] + valuesDisp[jawDown]);
			meshRendererEyelashes.SetBlendShapeWeight(jawLeft, values[jawLeft] + valuesDisp[jawLeft]);
			meshRendererEyelashes.SetBlendShapeWeight(jawRight, values[jawRight] + valuesDisp[jawRight]);
			meshRendererEyelashes.SetBlendShapeWeight(jawUp, values[jawUp] + valuesDisp[jawUp]);
			meshRendererEyelashes.SetBlendShapeWeight(lowerLipDown_left, values[lowerLipDown_left] + valuesDisp[lowerLipDown_left]);
			meshRendererEyelashes.SetBlendShapeWeight(lowerLipDown_right, values[lowerLipDown_right] + valuesDisp[lowerLipDown_right]);
			meshRendererEyelashes.SetBlendShapeWeight(lowerLipIn, values[lowerLipIn] + valuesDisp[lowerLipIn]);
			meshRendererEyelashes.SetBlendShapeWeight(lowerLipOut, values[lowerLipOut] + valuesDisp[lowerLipOut]);
			meshRendererEyelashes.SetBlendShapeWeight(midmouth_left, values[midmouth_left] + valuesDisp[midmouth_left]);
			meshRendererEyelashes.SetBlendShapeWeight(midmouth_right, values[midmouth_right] + valuesDisp[midmouth_right]);
			meshRendererEyelashes.SetBlendShapeWeight(mouthDown, values[mouthDown] + valuesDisp[mouthDown]);
			meshRendererEyelashes.SetBlendShapeWeight(mouthNarrow_left, values[mouthNarrow_left] + valuesDisp[mouthNarrow_left]);
			meshRendererEyelashes.SetBlendShapeWeight(mouthNarrow_right, values[mouthNarrow_right] + valuesDisp[mouthNarrow_right]);
			meshRendererEyelashes.SetBlendShapeWeight(mouthOpen, values[mouthOpen] + valuesDisp[mouthOpen]);
			meshRendererEyelashes.SetBlendShapeWeight(mouthUp, values[mouthUp] + valuesDisp[mouthUp]);
			meshRendererEyelashes.SetBlendShapeWeight(mouthWhistle_left, values[mouthWhistle_left] + valuesDisp[mouthWhistle_left]);
			meshRendererEyelashes.SetBlendShapeWeight(mouthWhistle_right, values[mouthWhistle_right] + valuesDisp[mouthWhistle_right]);
			meshRendererEyelashes.SetBlendShapeWeight(noseScrunch_left, values[noseScrunch_left] + valuesDisp[noseScrunch_left]);
			meshRendererEyelashes.SetBlendShapeWeight(noseScrunch_right, values[noseScrunch_right] + valuesDisp[noseScrunch_right]);
			meshRendererEyelashes.SetBlendShapeWeight(smileLeft, values[smileLeft] + valuesDisp[smileLeft]);
			meshRendererEyelashes.SetBlendShapeWeight(smileRight, values[smileRight] + valuesDisp[smileRight]);
			meshRendererEyelashes.SetBlendShapeWeight(squint_left, values[squint_left] + valuesDisp[squint_left]);
			meshRendererEyelashes.SetBlendShapeWeight(squint_right, values[squint_right] + valuesDisp[squint_right]);
			meshRendererEyelashes.SetBlendShapeWeight(toungeUp, values[toungeUp] + valuesDisp[toungeUp]);
			meshRendererEyelashes.SetBlendShapeWeight(upperLipIn, values[upperLipIn] + valuesDisp[upperLipIn]);
			meshRendererEyelashes.SetBlendShapeWeight(upperLipOut, values[upperLipOut] + valuesDisp[upperLipOut]);
			meshRendererEyelashes.SetBlendShapeWeight(upperLipUp_left, values[upperLipUp_left] + valuesDisp[upperLipUp_left]);
			meshRendererEyelashes.SetBlendShapeWeight(upperLipUp_right, values[upperLipUp_right] + valuesDisp[upperLipUp_right]);
		}

		if (meshRendererEyes != null)
		{
			meshRendererEyes.SetBlendShapeWeight(blink_left, values[blink_left] + valuesDisp[blink_left]);
			meshRendererEyes.SetBlendShapeWeight(blink_right, values[blink_right] + valuesDisp[blink_right]);
			meshRendererEyes.SetBlendShapeWeight(browsDown_left, values[browsDown_left] + valuesDisp[browsDown_left]);
			meshRendererEyes.SetBlendShapeWeight(browsDown_right, values[browsDown_right] + valuesDisp[browsDown_right]);
			meshRendererEyes.SetBlendShapeWeight(browsIn_left, values[browsIn_left] + valuesDisp[browsIn_left]);
			//meshRendererEyes.SetBlendShapeWeight(browsIn_right, values[browsIn_right] + valuesDisp[browsIn_right]);
			meshRendererEyes.SetBlendShapeWeight(browsOuterLower_left, values[browsOuterLower_left] + valuesDisp[browsOuterLower_left]);
			meshRendererEyes.SetBlendShapeWeight(browsOuterLower_right, values[browsOuterLower_right] + valuesDisp[browsOuterLower_right]);
			meshRendererEyes.SetBlendShapeWeight(browsUp_left, values[browsUp_left] + valuesDisp[browsUp_left]);
			meshRendererEyes.SetBlendShapeWeight(browsUp_right, values[browsUp_right] + valuesDisp[browsUp_right]);
			meshRendererEyes.SetBlendShapeWeight(cheekPuff_left, values[cheekPuff_left] + valuesDisp[cheekPuff_left]);
			meshRendererEyes.SetBlendShapeWeight(cheekPuff_right, values[cheekPuff_right] + valuesDisp[cheekPuff_right]);
			meshRendererEyes.SetBlendShapeWeight(eyesWide_left, values[eyesWide_left] + valuesDisp[eyesWide_left]);
			meshRendererEyes.SetBlendShapeWeight(eyesWide_right, values[eyesWide_right] + valuesDisp[eyesWide_right]);
			meshRendererEyes.SetBlendShapeWeight(frown_left, values[frown_left] + valuesDisp[frown_left]);
			meshRendererEyes.SetBlendShapeWeight(frown_right, values[frown_right] + valuesDisp[frown_right]);
			meshRendererEyes.SetBlendShapeWeight(jawBackward, values[jawBackward] + valuesDisp[jawBackward]);
			meshRendererEyes.SetBlendShapeWeight(jawForeward, values[jawForeward] + valuesDisp[jawForeward]);
			meshRendererEyes.SetBlendShapeWeight(jawRotateY_left, values[jawRotateY_left] + valuesDisp[jawRotateY_left]);
			meshRendererEyes.SetBlendShapeWeight(jawRotateY_right, values[jawRotateY_right] + valuesDisp[jawRotateY_right]);
			//meshRendererEyes.SetBlendShapeWeight(jawRotateZ_left, values[jawRotateZ_left] + valuesDisp[jawRotateZ_left]);
			meshRendererEyes.SetBlendShapeWeight(jawRotateZ_right, values[jawRotateZ_right] + valuesDisp[jawRotateZ_right]);
			meshRendererEyes.SetBlendShapeWeight(jawDown, values[jawDown] + valuesDisp[jawDown]);
			meshRendererEyes.SetBlendShapeWeight(jawLeft, values[jawLeft] + valuesDisp[jawLeft]);
			meshRendererEyes.SetBlendShapeWeight(jawRight, values[jawRight] + valuesDisp[jawRight]);
			meshRendererEyes.SetBlendShapeWeight(jawUp, values[jawUp] + valuesDisp[jawUp]);
			meshRendererEyes.SetBlendShapeWeight(lowerLipDown_left, values[lowerLipDown_left] + valuesDisp[lowerLipDown_left]);
			meshRendererEyes.SetBlendShapeWeight(lowerLipDown_right, values[lowerLipDown_right] + valuesDisp[lowerLipDown_right]);
			meshRendererEyes.SetBlendShapeWeight(lowerLipIn, values[lowerLipIn] + valuesDisp[lowerLipIn]);
			meshRendererEyes.SetBlendShapeWeight(lowerLipOut, values[lowerLipOut] + valuesDisp[lowerLipOut]);
			meshRendererEyes.SetBlendShapeWeight(midmouth_left, values[midmouth_left] + valuesDisp[midmouth_left]);
			meshRendererEyes.SetBlendShapeWeight(midmouth_right, values[midmouth_right] + valuesDisp[midmouth_right]);
			meshRendererEyes.SetBlendShapeWeight(mouthDown, values[mouthDown] + valuesDisp[mouthDown]);
			meshRendererEyes.SetBlendShapeWeight(mouthNarrow_left, values[mouthNarrow_left] + valuesDisp[mouthNarrow_left]);
			meshRendererEyes.SetBlendShapeWeight(mouthNarrow_right, values[mouthNarrow_right] + valuesDisp[mouthNarrow_right]);
			meshRendererEyes.SetBlendShapeWeight(mouthOpen, values[mouthOpen] + valuesDisp[mouthOpen]);
			meshRendererEyes.SetBlendShapeWeight(mouthUp, values[mouthUp] + valuesDisp[mouthUp]);
			meshRendererEyes.SetBlendShapeWeight(mouthWhistle_left, values[mouthWhistle_left] + valuesDisp[mouthWhistle_left]);
			meshRendererEyes.SetBlendShapeWeight(mouthWhistle_right, values[mouthWhistle_right] + valuesDisp[mouthWhistle_right]);
			meshRendererEyes.SetBlendShapeWeight(noseScrunch_left, values[noseScrunch_left] + valuesDisp[noseScrunch_left]);
			meshRendererEyes.SetBlendShapeWeight(noseScrunch_right, values[noseScrunch_right] + valuesDisp[noseScrunch_right]);
			meshRendererEyes.SetBlendShapeWeight(smileLeft, values[smileLeft] + valuesDisp[smileLeft]);
			meshRendererEyes.SetBlendShapeWeight(smileRight, values[smileRight] + valuesDisp[smileRight]);
			meshRendererEyes.SetBlendShapeWeight(squint_left, values[squint_left] + valuesDisp[squint_left]);
			meshRendererEyes.SetBlendShapeWeight(squint_right, values[squint_right] + valuesDisp[squint_right]);
			meshRendererEyes.SetBlendShapeWeight(toungeUp, values[toungeUp] + valuesDisp[toungeUp]);
			meshRendererEyes.SetBlendShapeWeight(upperLipIn, values[upperLipIn] + valuesDisp[upperLipIn]);
			meshRendererEyes.SetBlendShapeWeight(upperLipOut, values[upperLipOut] + valuesDisp[upperLipOut]);
			meshRendererEyes.SetBlendShapeWeight(upperLipUp_left, values[upperLipUp_left] + valuesDisp[upperLipUp_left]);
			meshRendererEyes.SetBlendShapeWeight(upperLipUp_right, values[upperLipUp_right] + valuesDisp[upperLipUp_right]);
		}


	}

	private void ExpressionPass()
	{
		//targets[browsDown_left] = (expressions[(int)EkmanType.Angry] + expressions[(int)EkmanType.Disgust] * 0.5f) * expressFactor;
		targets[browsDown_left] = (expressions[(int)EkmanType.Angry]) * expressFactor;
		targets[browsDown_right] = targets[browsDown_left];
		targets[cheekPuff_left] = expressions[(int)EkmanType.Angry] * 0.02f * expressFactor;
		targets[cheekPuff_right] = targets[cheekPuff_left];
		targets[frown_left] = (expressions[(int)EkmanType.Angry] * 0.4f + expressions[(int)EkmanType.Sad] * 0.85f + expressions[(int)EkmanType.Disgust] * 0.1f + expressions[(int)EkmanType.Afraid] * 0.6f) * expressFactor;
		targets[frown_right] = targets[frown_left];
		targets[mouthDown] = (expressions[(int)EkmanType.Angry] * 0.1f + expressions[(int)EkmanType.Sad] * 0.1f + expressions[(int)EkmanType.Afraid] * 0.05f) * expressFactor;
		targets[mouthNarrow_left] = (expressions[(int)EkmanType.Angry] * 0.2f + expressions[(int)EkmanType.Shock] * 0.2f + expressions[(int)EkmanType.Afraid] * 0.5f) * expressFactor;
		targets[mouthNarrow_right] = targets[mouthNarrow_left];
		targets[squint_left] = (expressions[(int)EkmanType.Angry] * 0.3f + expressions[(int)EkmanType.Sad] * 0.1f + expressions[(int)EkmanType.Happy] * 0.4f + expressions[(int)EkmanType.Disgust] * 0.3f) * expressFactor;
		targets[squint_right] = targets[squint_left];

		targets[browsUp_left] = (expressions[(int)EkmanType.Happy] * 0.3f + expressions[(int)EkmanType.Shock] + expressions[(int)EkmanType.Afraid] * 0.9f + expressions[(int)EkmanType.Sad] * 0.05f) * expressFactor;
		targets[browsUp_right] = targets[browsUp_left];
		targets[eyesWide_left] = (expressions[(int)EkmanType.Shock] * 0.8f + expressions[(int)EkmanType.Afraid] * 0.9f) * expressFactor;
		targets[eyesWide_right] = targets[eyesWide_left];
		//targets[mouthOpen] = (expressions[(int)EkmanType.Happy] * 0.12f + expressions[(int)EkmanType.Shock] * 0.3f + expressions[(int)EkmanType.Afraid] * 0.5f) * expressFactor;
		targets[mouthOpen] = (expressions[(int)EkmanType.Happy] * 0.12f + expressions[(int)EkmanType.Shock] * 0.3f + expressions[(int)EkmanType.Afraid] * 0.3f) * expressFactor;
		// targets[upperLipIn] = exp_happy * 0.02f * expressFactor;
		// targets[lowerLipIn] = exp_happy * 0.02f * expressFactor;
		targets[smileLeft] = expressions[(int)EkmanType.Happy] * 0.95f * expressFactor;
		targets[smileRight] = targets[smileLeft];

		targets[browsOuterLower_left] = (expressions[(int)EkmanType.Sad] * 0.95f + expressions[(int)EkmanType.Shock] * 0.5f) * expressFactor;
		targets[browsOuterLower_right] = targets[browsOuterLower_left];
		targets[browsIn_left] = (expressions[(int)EkmanType.Sad] * 0.05f + expressions[(int)EkmanType.Afraid] * 0.8f) * expressFactor;
		//targets[browsIn_right] = targets[browsIn_left];
		targets[jawBackward] = expressions[(int)EkmanType.Sad] * 0.05f * expressFactor;

		targets[jawDown] = expressions[(int)EkmanType.Disgust] * 0.2f * expressFactor;
		targets[noseScrunch_left] = expressions[(int)EkmanType.Disgust] * 0.85f * expressFactor;
		targets[noseScrunch_right] = targets[noseScrunch_left];
		targets[mouthUp] = expressions[(int)EkmanType.Disgust] * 0.05f * expressFactor;
		targets[jawForeward] = expressions[(int)EkmanType.Disgust] * 0.1f * expressFactor;
		targets[upperLipOut] = expressions[(int)EkmanType.Disgust] * 0.45f * expressFactor;
		targets[lowerLipIn] = expressions[(int)EkmanType.Disgust] * 0.15f * expressFactor;
		targets[mouthWhistle_left] = (expressions[(int)EkmanType.Disgust] * 0.1f) * expressFactor;
		targets[mouthWhistle_right] = targets[mouthWhistle_left];
		targets[midmouth_left] = expressions[(int)EkmanType.Disgust] * 0.15f * expressFactor;
		targets[midmouth_right] = targets[midmouth_left];

		targets[lowerLipDown_left] = expressions[(int)EkmanType.Afraid] * 0.3f * expressFactor;
		targets[lowerLipDown_right] = targets[lowerLipDown_left];
	}

	private float w_happy;
	private float w_angry;
	private float w_shock;
	private float w_sad;
	private float w_afraid;




	public void SetTargetsImmediate()
	{
		for (int i = 0; i < 50; i++)
		{
			values[i] = targets[i];
		}
		w_angry = expressions[(int)EkmanType.Angry];
		w_happy = expressions[(int)EkmanType.Happy];
		w_sad = expressions[(int)EkmanType.Sad];
		w_shock = expressions[(int)EkmanType.Shock];
		w_afraid = expressions[(int)EkmanType.Afraid];
	}



}
