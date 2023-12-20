using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class MainMenu : MonoBehaviour
{
   [SerializeField] private TMP_InputField seedText;
   [SerializeField] private TextMeshProUGUI nbChest;
   [SerializeField] private Slider slider;
   
   public void play()
   {
      if (seedText.text.Length > 0)
      {
         PlayerPrefs.SetInt("Seed", int.Parse(seedText.text));
      }
      else
      {
         PlayerPrefs.DeleteKey("Seed");
      }

      if (nbChest.text.Length > 0)
      {
         PlayerPrefs.SetInt("NbChest", int.Parse(nbChest.text));
      }
      else
      {
         PlayerPrefs.DeleteKey("NbChest");
      }
      
      SceneManager.LoadScene(1);
   }

   public void playDemo()
   {
      if (seedText.text.Length > 0)
      {
         PlayerPrefs.SetInt("Seed", int.Parse(seedText.text));
      }
      else
      {
         PlayerPrefs.DeleteKey("Seed");
      }
      
      if (nbChest.text.Length > 0)
      {
         PlayerPrefs.SetInt("NbChest", int.Parse(nbChest.text));
      }
      else
      {
         PlayerPrefs.DeleteKey("NbChest");
      }
      
      SceneManager.LoadScene(2);
   }

   public void SliderUpdate()
   {
      nbChest.text = slider.value.ToString();
   }
   
}
