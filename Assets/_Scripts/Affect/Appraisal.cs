using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class Attitude {
    public bool liking;
    public float weight;	 //appealingness
    public GameObject subject = null;
    public bool selected; //For editing purposes
}

[System.Serializable]
public class Standard {
    public bool focusingOnSelf; //if false focusing on other is true
    public bool approving;
    public float weight; //praiseworthiness
    public GameObject subject = null;
    public bool selected; //For editing purposes		
}
[System.Serializable]
public class Goal {
    public bool consequenceForSelf; //if false consequence for other is true
    public bool desirableForOther;
    public bool prospectRelevant;
    public int confirmed;
    public bool pleased;
    public float weight; //desirability
    public string about; //event name
    public GameObject subject = null;
    public bool selected; //For editing purposes

}

//Appraisal definitions
public static class AppDef {
    //About goals
    public const bool Pleased = true;
    public const bool Displeased = false;
    public const bool ConsequenceForSelf = true;
    public const bool ConsequenceForOther = false;
    public const bool ProspectRelevant = true;
    public const bool ProspectIrrelevant = false;
    public const bool DesirableForOther = true;
    public const bool UndesirableForOther = false;
    public const int Unconfirmed = 0;
    public const int Confirmed = 1;
    public const int Disconfirmed = 2;

    //About standards
    public const bool FocusingOnSelf = true;
    public const bool FocusingOnOther = false;
    public const bool Approving = true;
    public const bool Disapproving = false;

    //About attitudes
    public const bool Liking = true;
    public const bool Disliking = false;

}

[System.Serializable]
public class Appraisal : MonoBehaviour {
    [SerializeField]
    public List<Goal> Goals = new List<Goal>();
    [SerializeField]
    public List<Standard> Standards = new List<Standard>();
    [SerializeField]
    public List<Attitude> Attitudes = new List<Attitude>();

    float[] _eventFactor = new float[22];


    private AffectComponent _affectComponent;

    void Start() {
        _affectComponent = GetComponent<AffectComponent>();
    }

    public void Restart() {
        Goals.Clear();
        Standards.Clear();
        Attitudes.Clear();
    }

    public float[] ComputeEventFactor() {
        int i;

        for (i = 0; i < _eventFactor.Length; i++)
            _eventFactor[i] = 0;

        foreach (Goal g in Goals) {

            if (g.consequenceForSelf) {

                if (g.prospectRelevant) {

                    switch (g.confirmed) {
                        case AppDef.Unconfirmed:
                            if (g.pleased)
                                _eventFactor[(int)EType.Hope] += g.weight * _affectComponent.EmotionWeight[(int)EType.Hope];//(1.7 * sqrt(g.expectation)) + (-0.7*g.weight);
                            else
                                _eventFactor[(int)EType.Fear] += g.weight * _affectComponent.EmotionWeight[(int)EType.Fear];//0.4;//2 *g.expectation * g.expectation - g.weight;
                            break;
                        case AppDef.Confirmed:
                            if (g.pleased)
                                _eventFactor[(int)EType.Satisfaction] += g.weight * _affectComponent.EmotionWeight[(int)EType.Satisfaction];
                            else
                                _eventFactor[(int)EType.FearsConfirmed] += g.weight * _affectComponent.EmotionWeight[(int)EType.FearsConfirmed];
                            break;
                        default:
                            if (g.pleased)
                                _eventFactor[(int)EType.Disappointment] += g.weight * _affectComponent.EmotionWeight[(int)EType.Disappointment];//(1.7 * sqrt(g.expectation)) + (-0.7*g.weight) * g.weight;  //hope x weight
                            else
                                _eventFactor[(int)EType.Relief] += g.weight * _affectComponent.EmotionWeight[(int)EType.Relief];//(2 * g.expectation *g.expectation -g.weight)*g.weight; //fear x weight
                            break;
                    }
                }
                else { //prospect irrelevant

                    if (g.pleased)
                        _eventFactor[(int)EType.Joy] += g.weight * _affectComponent.EmotionWeight[(int)EType.Joy];//(1.7 * sqrt(g.expectation)) + (-0.7*g.weight);
                    else
                        _eventFactor[(int)EType.Distress] += g.weight * _affectComponent.EmotionWeight[(int)EType.Distress];// (2 * g.expectation * g.expectation) -g.weight;



                }

            }
            else { //consequences for others

                if (g.desirableForOther) {

                    if (g.pleased)
                        _eventFactor[(int)EType.HappyFor] += g.weight * _affectComponent.EmotionWeight[(int)EType.HappyFor];
                    else
                        _eventFactor[(int)EType.Resentment] += g.weight * _affectComponent.EmotionWeight[(int)EType.Resentment];

                }
                else { //undesirable for other

                    if (g.pleased)
                        _eventFactor[(int)EType.Gloating] += g.weight * _affectComponent.EmotionWeight[(int)EType.Gloating];
                    else
                        _eventFactor[(int)EType.Pity] += g.weight * _affectComponent.EmotionWeight[(int)EType.Pity];
                }

            }



        }


        foreach (Standard s in Standards) {

            if (s.focusingOnSelf) {

                if (s.approving)
                    _eventFactor[(int)EType.Pride] += s.weight * _affectComponent.EmotionWeight[(int)EType.Pride];
                else
                    _eventFactor[(int)EType.Shame] += s.weight * _affectComponent.EmotionWeight[(int)EType.Shame];
            }
            else { //focusing on others		
                if (s.approving)
                    _eventFactor[(int)EType.Admiration] += s.weight * _affectComponent.EmotionWeight[(int)EType.Admiration];
                else {

                    _eventFactor[(int)EType.Reproach] += s.weight * _affectComponent.EmotionWeight[(int)EType.Reproach];
                }
            }

        }

        foreach (Attitude a in Attitudes) {
            if (a.liking)
                _eventFactor[(int)EType.Love] += a.weight * _affectComponent.EmotionWeight[(int)EType.Love];
            else
                _eventFactor[(int)EType.Hate] += a.weight * _affectComponent.EmotionWeight[(int)EType.Hate];

        }



        //compound emotions
        if (_eventFactor[(int)EType.Admiration] > 0 && _eventFactor[(int)EType.Joy] > 0)
            _eventFactor[(int)EType.Gratification] = (_eventFactor[(int)EType.Admiration] + _eventFactor[(int)EType.Joy]) / 2.0f;

        if (_eventFactor[(int)EType.Pride] > 0 && _eventFactor[(int)EType.Joy] > 0)
            _eventFactor[(int)EType.Gratitude] = (_eventFactor[(int)EType.Pride] + _eventFactor[(int)EType.Joy]) / 2.0f;

        if (_eventFactor[(int)EType.Shame] > 0 && _eventFactor[(int)EType.Distress] > 0)
            _eventFactor[(int)EType.Remorse] = (_eventFactor[(int)EType.Shame] + _eventFactor[(int)EType.Distress]) / 2.0f;

        if (_eventFactor[(int)EType.Reproach] > 0 && _eventFactor[(int)EType.Distress] > 0) {

            _eventFactor[(int)EType.Anger] = (_eventFactor[(int)EType.Reproach] + _eventFactor[(int)EType.Distress]) / 2.0f;
        }


        //Normalize eventFactor 
        MathDefs.NormalizeElements(_eventFactor, 0f, 1f);

        return _eventFactor;
    }


    //Order: pleased, consequenceForself, prospectRelevant, {confirmed}
    //pleased, consequenceForOther, subject, desirableForOther
    public void AddGoal(string relEvent, float weight, params object[] values) {

        if (values.Length < 3) {
            Debug.Log("Missing parameter in AddGoal");
            return;
        }

        Goal g = new Goal();

        g.about = relEvent;
        g.weight = weight;

        g.pleased = (bool)values[0];


        g.consequenceForSelf = (bool)values[1];
        if (values[1].Equals(AppDef.ConsequenceForOther)) {
            if (values.Length < 4) {
                Debug.Log("Missing parameter in AddGoal");
                return;
            }
            g.subject = (GameObject)values[2];
            g.desirableForOther = (bool)values[3];

        }
        else { //conseqForSelf

            g.prospectRelevant = (bool)values[2];

            if (values[2].Equals(AppDef.ProspectRelevant)) {
                if (values.Length < 4) {
                    Debug.Log("Missing parameter in AddGoal");
                    return;

                }
                else {
                    g.confirmed = (int)values[3];
                }
            }

        }

        Goals.Add(g);

    }

    //order approving, focusingonSelf, {subject}
    public void AddStandard(float weight, bool approving, params object[] values) {
        Standard s = new Standard();
        s.weight = weight;
        s.approving = approving;

        if (values.Length < 1) {
            Debug.Log("Missing parameter in AddStandard");
            return;
        }

        s.focusingOnSelf = (bool)values[0];

        if (values[0].Equals(AppDef.FocusingOnOther)) {

            if (values.Length < 1) {
                Debug.Log("Missing parameter \"subject\" in AddStandard");
                return;
            }
            else {
                s.subject = (GameObject)values[1];
            }

        }
        Standards.Add(s);
    }


    public void AddAttitude(GameObject subject, float weight, bool liking) {
        Attitude a = new Attitude();
        a.subject = subject;
        a.weight = weight;
        a.liking = liking;
        Attitudes.Add(a);
    }


    //Remove goal about the related event with the indicated values
    //Order: pleased, consequenceForself, prospectRelevant, confirmed
    //Returns weight value !=0 if goal is found
    public float RemoveGoal(string relEvent, params object[] values) {
        float wt = 0f;
        int i = 0;
        while (i < Goals.Count) {
            if (relEvent.Equals("") || Goals[i].about.Equals(relEvent)) {
                if (values.Length == 0) {
                    wt = Goals[i].weight;
                    Goals.Remove(Goals[i]);                    
                }
                else if (values[0].Equals(Goals[i].pleased)) {
                    if (values.Length == 1) {
                        wt = Goals[i].weight;
                        Goals.Remove(Goals[i]);
                        
                    }
                    else if (values[1].Equals(Goals[i].consequenceForSelf)) {
                        if (values.Length == 2) {
                            wt = Goals[i].weight;
                            Goals.Remove(Goals[i]);
                            
                        }
                        else if (values[1].Equals(true) && values[2].Equals(Goals[i].prospectRelevant)) {
                            if (values[2].Equals(true)) { //prospect relevant
                                if (values.Length == 3) {
                                    wt = Goals[i].weight;
                                    Goals.Remove(Goals[i]);
                                    
                                }
                                else if (values[3].Equals(Goals[i].confirmed)) {
                                    wt = Goals[i].weight;
                                    Goals.Remove(Goals[i]);                                    
                                }
                                else
                                    i++;
                            }
                            else {//prospect irrelevant
                                wt = Goals[i].weight;
                                Goals.Remove(Goals[i]);                                
                            }
                        }
                        else if (values[1].Equals(false) && values[2].Equals(Goals[i].desirableForOther)) {
                            wt = Goals[i].weight;
                            Goals.Remove(Goals[i]);
                            
                        }
                        else
                            i++;

                    }
                    else
                        i++;
                }
                else
                    i++;
            }
            else
                i++;
        }


        return wt;
    }

    //Remove standard about the related subject
    //Order: Approvingm focusingonself
    //Returns weight!=0 if standard is found
    public float RemoveStandard(GameObject sub, params object[] values) {
        float wt = 0f;
        int i = 0;
        while (i < Standards.Count) {
            if (sub == null || Standards[i].subject.Equals(sub)) {
                if (values.Length == 0) {
                    wt = Standards[i].weight;
                    Standards.Remove(Standards[i]);
                    
                }
                else if (values[0].Equals(Standards[i].approving)) {
                    if (values.Length == 1) {
                        wt = Standards[i].weight;
                        Standards.Remove(Standards[i]);
                        
                    }
                    else if (values[1].Equals(Standards[i].focusingOnSelf)) {
                        wt = Standards[i].weight;
                        Standards.Remove(Standards[i]);                        
                    }
                    else
                        i++;
                }
                else
                    i++;

            }

            else
                i++;
        }
        return wt;
    }

    //Returns weight!=0 if attitude is found
    public float RemoveAttitude(GameObject sub, params object[] values) {
        float wt = 0f;
        int i = 0;
        while (i < Attitudes.Count) {
            if (sub == null || Attitudes[i].subject.Equals(sub)) {
                if (values.Length == 0) {
                    wt = Attitudes[i].weight;
                    Attitudes.Remove(Attitudes[i]);                    
                }
                else
                    i++;
            }
            else if (values[0].Equals(Attitudes[i].liking)) {
                wt = Attitudes[i].weight;             
                Attitudes.Remove(Attitudes[i]);
            }
            else
                i++;
        }

        return wt;

    }

    //Return whether a standard with the approvalStatus about a subject exists
    public bool DoesStandardExist(GameObject sub, bool approvalStatus) {
        foreach (Standard s in Standards) {
            if (s.subject == sub) {
                if (s.approving == approvalStatus)
                    return true;
            }
        }
        return false; //no standard exits about the subject or the approvalStatus does not match
    }

    //Return whether a goal with the approvalStatus about a subject exists
    public bool DoesGoalExist(string type, GameObject sub) {
        foreach (Goal g in Goals) {
            if (g.about.Equals(type) && g.subject && g.subject.Equals(sub))               
                    return true;
            
        }
        return false; //no standard exits about the subject or the approvalStatus does not match
    }

}
