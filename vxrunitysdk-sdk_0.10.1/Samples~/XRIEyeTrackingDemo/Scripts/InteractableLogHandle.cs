using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class InteractableLogHandle:MonoBehaviour
{
       [SerializeField]private EyeDemoInteractableLogDisplay logDisplay;
  

       public void OnHoverEnter()
        {
            logDisplay.AddInfo($"{name} OnHoverEnter");

        }
    
        public void OnHoverExit()
        {
            logDisplay.AddInfo($"{name} OnHoverExit");
         
        }
        
        public void OnSelectEnter()
        {
            logDisplay.AddInfo($"{name} OnSelectEnter");
           
        }
        
        public void OnSelectExit()
        {
            logDisplay.AddInfo($"{name} OnSelectExit");
            
        }

        public void OnFirstHoverEnter()
        {
            logDisplay.AddInfo($" {name} First OnHoverEnter");
          
        }

        public void OnFirstHoverExit()
        {
            logDisplay.AddInfo($" {name} First OnHoverExit");
        }

        public void OnFirstSelectEnter()
        {
            logDisplay.AddInfo($"{name} First OnSelectEnter");
        }
        public void OnFirstSelectExit()
        {
            logDisplay.AddInfo($"{name} First OnSelectExit");
        }

        public void Avtivated()
        {
            logDisplay.AddInfo($"{name} Avtivated");
        }
        public void Deactivated()
        {
            logDisplay.AddInfo($"{name} Deactivated");
        }
        
    
        
}