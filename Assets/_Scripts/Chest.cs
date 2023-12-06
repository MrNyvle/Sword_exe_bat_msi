using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

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

        private int _id;
        private Item _item;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        
        private List<Chest> _chestsForOpen = new ();
        
        private bool _isOpen;
        
        public Difficulty Difficulty => difficulty;

        public Item Item => _item;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void SetupChest(int prmID, Item prmItem)
        {
            _id = prmID;
            _item = prmItem;
        }
        
        public void ToggleMessage()
        {
            int index = 1;
            (bool, string) checkValues = CheckForParent();
            
            if (checkValues.Item1)
            {
                index = 0;
            }
            uis[index].gameObject.SetActive(!uis[index].gameObject.activeSelf);
            TMP_Text tmpText = uis[index].GetComponentInChildren<TextMeshProUGUI>();

            tmpText.text = "Open" + checkValues.Item2;

        }

        private (bool,string) CheckForParent()
        {
            string notOpenedChests = " ";
            if (_chestsForOpen == null)
            {
                return (true,notOpenedChests);
            }
            
            foreach (var chest in _chestsForOpen)
            {
                if (!chest._isOpen)
                {
                    notOpenedChests += "(" + chest._id.ToString().ToUpper() + ")";
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
            if (CheckForParent().Item1 && !_isOpen)
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

            _isOpen = true;

        }

        public void AddParent(Chest chests)
        {
            _chestsForOpen.Add(chests);
        }
        
    }
}
