using System;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace _Scripts
{
    public class Chest : MonoBehaviour
    {
        private Item _item;
        private bool _isOpen;
        private bool _canBeOpened;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite spriteChestOpened;
        [SerializeField] private GameObject[] uis;

        private readonly Chest[] _chestsForOpen;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public Chest(String name, Item item, Chest[] chestsForOpen = null)
        {
            gameObject.name = name;
            _item = item;
            _chestsForOpen = chestsForOpen;
            _canBeOpened = chestsForOpen != null;
        }

        [Button("toggle")]
        public void ToggleMessage()
        {
            int index = 1;
            (bool, string) checkValues = CheckForPredecessors();
            
            if (checkValues.Item1)
            {
                index = 0;
            }
            uis[index].gameObject.SetActive(!uis[index].gameObject.activeSelf);
            TMP_Text tmpText = uis[index].GetComponent<TMP_Text>();

            tmpText.text = "Open" + checkValues.Item2;

        }

        private (bool,string) CheckForPredecessors()
        {
            string notOpenedChests = "";
            if (_chestsForOpen == null)
            {
                return (true,notOpenedChests);
            }
            
            foreach (var chest in _chestsForOpen)
            {
                if (!chest._isOpen)
                {
                    notOpenedChests += chest.gameObject.name.ToUpper();
                }
            }

            if (notOpenedChests != "") 
            {
                return (true,notOpenedChests);
            }

            return (false,notOpenedChests);
        }

        public void OpenChest()
        {
            _animator.enabled = true;
        }


        public void ChestOpened()
        {
            _animator.enabled = false;

            _spriteRenderer.sprite = spriteChestOpened;

            GameManager.instance.GiveItemToPlayer(_item);
        }
    }
}
