using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   [SerializeField] private TMP_InputField text;
   
   public void play()
   {
      if (text.text.Length > 0)
      {
         PlayerPrefs.SetInt("Seed", int.Parse(text.text));
      }
      else
      {
         PlayerPrefs.DeleteKey("Seed");
      }
      
      SceneManager.LoadScene(1);
   }

   public void playDemo()
   {
      if (text.text.Length > 0)
      {
         PlayerPrefs.SetInt("Seed", int.Parse(text.text));
      }
      else
      {
         PlayerPrefs.DeleteKey("Seed");
      }
      
      SceneManager.LoadScene(2);
   }
}
