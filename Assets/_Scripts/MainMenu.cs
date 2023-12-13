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
      Random.InitState(int.Parse(text.text));
      SceneManager.LoadScene(1);
   }

   public void playDemo()
   {
      SceneManager.LoadScene(2);
   }
}
