using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class Door : MonoBehaviour
    {
        
        [SerializeField] private SpriteRenderer spriteToToggle;
        [SerializeField] private GameObject[] uis;
        private List<Chest> _chestsForOpen = new ();
        [SerializeField] private Difficulty difficulty;
        
        public Difficulty Difficulty => difficulty;

        public bool isOpen = false;
        
        public void ToggleMessage()
        {
            int index = 1;
            (bool, string) checkValues = CheckForParents();
            
            if (checkValues.Item1)
            {
                index = 0;
            }
            uis[index].gameObject.SetActive(!uis[index].gameObject.activeSelf);
            TMP_Text tmpText = uis[index].GetComponentInChildren<TextMeshProUGUI>();

            tmpText.text = isOpen ? "Close" : "Open" + checkValues.Item2;
        }
        
        
        public void ToggleMessageRefresh()
        {
            int index = 1;
            (bool, string) checkValues = CheckForParents();
            
            if (checkValues.Item1)
            {
                index = 0;
            }
            TMP_Text tmpText = uis[index].GetComponentInChildren<TextMeshProUGUI>();

            tmpText.text = isOpen ? "Close" : "Open" + checkValues.Item2;
        }
        
        
        private (bool,string) CheckForParents()
        {
            string notOpenedChests = " ";
            if (_chestsForOpen == null)
            {
                return (true,notOpenedChests);
            }
            
            foreach (var chest in _chestsForOpen)
            {
                if (!chest.isOpen)
                {
                    notOpenedChests += "(" + chest.id.ToString().ToUpper() + ")";
                }
            }

            if (notOpenedChests == " ") 
            {
                return (true,notOpenedChests);
            }

            return (false,notOpenedChests);
        }


        public void OpenDoor()
        {
            if (CheckForParents().Item1)
            {
                isOpen = !isOpen;
                spriteToToggle.gameObject.SetActive(!spriteToToggle.gameObject.activeSelf);
                ToggleMessageRefresh();
            }
        }
        
        public void AddParent(Chest chests)
        {
            _chestsForOpen.Add(chests);
        }
    }
}