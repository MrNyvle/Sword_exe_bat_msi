using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace _Scripts
{
    public enum Difficulty
    {
        Early,
        Late
    }
    
    public class Chest : MonoBehaviour
    {
        [SerializeField] private Sprite spriteChestOpened; 
        [SerializeField] private GameObject[] uis;
        [SerializeField] private Difficulty difficulty;
        [SerializeField] private TextMeshProUGUI idText;

        public int id;
        private Item _item;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        
        private List<Chest> _chestsForOpen = new ();
        
        public bool isOpen;
        public bool isSetup;
        
        public Difficulty Difficulty => difficulty;

        public Item Item => _item;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void SetupChest(int prmID, Item prmItem)
        {
            id = prmID;
            _item = prmItem;
            isSetup = true;
            idText.text = id.ToString();
        }
        
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

            tmpText.text = "Open" + checkValues.Item2;

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

        public void OpenChest()
        {
            Debug.Log("Openchest");
            if (CheckForParents().Item1 && !isOpen)
            {
                _animator.enabled = true;
            }
        }
        
        public void ChestOpened()
        {
            Debug.Log("Chest opened");
            _animator.enabled = false;

            _spriteRenderer.sprite = spriteChestOpened;

            GameManager.instance.GiveItemToPlayer(_item);

            uis[0].transform.parent.gameObject.SetActive(false);

            isOpen = true;

        }

        public void AddParent(Chest chests)
        {
            _chestsForOpen.Add(chests);
        }
        
    }
}
