using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeDemoInteractableLogDisplay : MonoBehaviour
{
    [SerializeField]private TextMesh info;
 
   private List<string> infos = new List<string>();

   [SerializeField]private int logCount = 15;
   public void AddInfo(string info)
   {
       infos.Add(info);
       while (infos.Count>logCount)
       {
           infos.RemoveAt(0);
       }

       ShowInfo();
   }
   
   public void ShowInfo()
   {
       string infoStr = "";
       foreach (var info in infos)
       {
           infoStr += info + "\n";
       }
       this.info.text = infoStr;
   }
   
    
    
    
    
}
