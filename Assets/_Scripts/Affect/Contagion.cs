using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum InfectionStatus {
    Susceptible,
    Wounded,
    Infected
}

[System.Serializable]
public class Contagion {
    
    private  float _doseMean = 1f;  
    public float DoseMean {
        get {
            return _doseMean;
        }
        set {
            _doseMean = value;
            UpdateStatus();
        }
    }

    
    private int _immunity = 0; // 0 = not yet infected, 1 = infected, 2 = healed
    private InfectionStatus _status = InfectionStatus.Susceptible;
    private int _k = 10; //number of previous k doses
    public int K {
        get {
            return _k;
        }
        set {
            _k = value;
            UpdateStatus();
        }
    }
    private List<float> _doseHistory = new List<float>();

    public float Dose = 0.0f;

    public InfectionStatus Status {
        get {
            UpdateStatus();
            return _status;
        }
    }

    private float _doseThreshold;

    public float DoseThreshold {
        get {

            return _doseThreshold;
        }
        set {
            _doseThreshold = MathDefs.GaussianDist(value, value / 10f);           
          //  _doseThreshold = value;
            if (_doseThreshold < 0f)
                _doseThreshold = 0f;
            else if(_doseThreshold > 1f)
                _doseThreshold = 1f;
        }
    }


    private float _woundThreshold;

    public float WoundThreshold {
        get {

            return _woundThreshold;
        }
        set {
            //woundThreshold = MathDefs.GaussianDist(value, DoseVariance);
            _woundThreshold = MathDefs.GaussianDist(value, value / 10f);
        }
    }

    public Contagion() {
         Restart();
    }

    public void Restart() {
        _doseHistory.Clear();
        Dose = 0f;
        _immunity = 0;
    }
    public void UpdateStatus() {

        if (Math.Abs(Dose) > DoseThreshold) {
            _status = InfectionStatus.Infected;
            _immunity = 1;
        }
        else{
            if (_immunity == 1) { // now become immune
                _immunity = 2;
            }

   if (_status == InfectionStatus.Infected)  //now becomes susceptible again, but dose threshold is increased
                _doseThreshold += _doseThreshold* 0.5f;
         

                if (Math.Abs(Dose) > WoundThreshold)
                    _status = InfectionStatus.Wounded;
                else
                    _status = InfectionStatus.Susceptible;
            
        }
           
        
    }

    public bool IsSusceptible() {
        return Status != InfectionStatus.Infected; //&& _immunity != 2; //not infected and not immune
    }

    public void AddDose(float sus, float coef) {        
    //    if (Status == InfectionStatus.Susceptible || Status == InfectionStatus.Wounded) { 
            float d = MathDefs.GaussianDist(DoseMean, DoseMean / 10f);

            if(_doseHistory.Count >= K) //keep previous K doses
                _doseHistory.RemoveAt(0);
            
    
         //   _doseHistory.Add(d * sus * Time.deltaTime); //FUNDA

            _doseHistory.Add(d* sus);

            Dose = 0;
            foreach (float t in _doseHistory) 
                Dose += t;

          //funda sonra ac???  Dose /= coef; //coef makes sure we normalize dose to range [0 1]
            
            UpdateStatus();
     //   }


    }



    public void DecayDose() {

        float d;

    
        d = MathDefs.GaussianDist(DoseMean, DoseMean / 10f);

        //take some percentage of the normal dose depending on susceptibility
   //     Dose -= d * 0.01f * Time.deltaTime;

        Dose -= Dose * 0.01f; //* Time.deltaTime; //funda

        if (Dose < 0f)
            Dose = 0f;

        UpdateStatus();


    }



}
