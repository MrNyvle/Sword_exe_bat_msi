using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public class GameManager : Singleton<GameManager>
    {

        [SerializeField] private Player player;

        [SerializeField] private Chest[] _chests = {};
        [SerializeField] private int Seed;


        private void Start()
        {
            Random.InitState(Seed);
            
            _chests[0].SetupChest("A", new Item("Sword", 10));
        }

        public void GiveItemToPlayer(Item item)
        {
            player.Inventory.Add(item);
        }
    }
}
