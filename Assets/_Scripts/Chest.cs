using System.Collections.Generic;
using System.Linq;
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
        
        public List<Chest> _chestsForOpen = new ();
        [SerializeField] private Door doorForOpen;
        
        public bool isOpen;
        public bool isSetup;
        public bool isChecked = false;
        
        public Difficulty Difficulty
        {
            get => difficulty;
            set => difficulty = value;
        }

        public Item Item
        {
            get => _item;
            set => _item = value;
        }

        public List<Chest> ChestsForOpen { get => _chestsForOpen; set => _chestsForOpen = value; }
        public Door DoorForOpen { get => doorForOpen; set => doorForOpen = value; }

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

            if (difficulty == Difficulty.Late)
            {
                doorForOpen = GameManager.instance.Doors.First();
            }
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
