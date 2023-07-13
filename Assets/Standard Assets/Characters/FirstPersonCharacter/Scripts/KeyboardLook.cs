using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class KeyboardLook
    {
        private Quaternion m_CharacterTargetRot;
        public float smoothTime = 50f;


        public void Init(Transform character, Transform camera) {
            m_CharacterTargetRot = character.localRotation;
            
        }

        public void LookRotation(Transform character)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Horizontal");

            //m_CharacterTargetRot *= Quaternion.Euler(0f, yRot* smoothTime * Time.deltaTime, 0f);
            //character.localRotation = m_CharacterTargetRot;


            character.Rotate(0, yRot * smoothTime * Time.deltaTime, 0);




        }

        

        
    }
}
