using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts
{
    public class GameManager : Singleton<GameManager>
    {

        [SerializeField] private Player player;

        [SerializeField] private Chest[] _chests = {};


        private void Start()
        {
            _chests[0].SetupChest("A", new Item("Sword", 10));
        }

        public void GiveItemToPlayer(Item item)
        {
            player.Inventory.Add(item);
        }
    }
}
