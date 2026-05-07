// using UnityEngine;
// using TMPro; // Required for TextMeshPro

// public class InfoManager : MonoBehaviour
// {
//     public TextMeshProUGUI infoText; 
    
//     // Hardcoded pages so you don't have to type them in Unity
//     private string[] pages = new string[]
//     {
//         "The canine skeleton consists of approximately 320 bones, providing structure and protection for vital organs.",
//         "A dog's skull shape is categorized into three types: Brachycephalic (short-nosed), Mesaticephalic (medium), and Dolichocephalic (long-nosed).",
//         "Unlike humans, dogs lack a collarbone (clavicle), allowing for a greater stride length and flexibility when running.",
//         "The vertebral column is divided into five regions: Cervical, Thoracic, Lumbar, Sacral, and Coccygeal (tail bones)."
//     };

//     private int currentPage = 0;

//     void Start()
//     {
//         // Set the initial text when the scene starts
//         UpdateUI();
//     }
//  public void Next()
//     {
//         if (infoText == null) return;
        
//         index++;
//         if (index >= data.Length) index = 0; // Loop to start
        
//         infoText.text = data[index];
//         Debug.Log("Switched to Page: " + index); // This tells us it's working in the Console
//     }

//     public void Previous()
//     {
//         if (infoText == null) return;

//         index--;
//         if (index < 0) index = data.Length - 1; // Loop to end
        
//         infoText.text = data[index];
//         Debug.Log("Switched to Page: " + index);
//     }

//     void UpdateUI()
//     {
//         if (infoText != null)
//         {
//             infoText.text = pages[currentPage];
//         }
//     }
// }


