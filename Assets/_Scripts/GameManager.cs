using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private List<Item> _itemsInChest = new List<Item>();
        [SerializeField] private int Seed;
        [SerializeField] private List<GameObject> _enemies = new List<GameObject>();

        private Dictionary<string, Item> _lootTable = new Dictionary<string, Item>();
        private void Start()
        {
            Random.InitState(Seed);
            _lootTable = _itemsInChest.ToDictionary(x => x.Name, x => x);
            //_chests[0].SetupChest("A", new Item("Sword", 10));
            StartCoroutine(SpawnEnemies());
        }

        public void GiveItemToPlayer(Item item)
        {
            player.Inventory.Add(item);
        }

        public Item CheckLootTable(String itemName)
        {
            return _lootTable[itemName];
        }


        private IEnumerator SpawnEnemies()
        {
            foreach (var enemy in _enemies) 
            {
                enemy.SetActive(true);
                yield return new WaitForSeconds(Random.value / 2);
            }
        }
    }
}
