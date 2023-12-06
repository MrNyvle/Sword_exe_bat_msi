using System;
using System.Collections.Generic;
using System.Collections;
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
        
        [SerializeField] private List<Item> _itemsInLateChest = new List<Item>();
        [SerializeField] private List<Item> _itemsInEarlyChest = new List<Item>();
        [SerializeField] private List<Chest> _chests = new();
        [SerializeField] private List<GameObject> _enemies = new List<GameObject>();
        [SerializeField] private int seed;
        [SerializeField] private int keyDropChance;
        

        private Dictionary<string, Item> _earlyLootTable = new Dictionary<string, Item>();
        private Dictionary<string, Item> _lateLootTable = new Dictionary<string, Item>();
        private Dictionary<string, Item> _lootTable = new Dictionary<string, Item>();
        private void Start()
        {
            Random.InitState(seed);
            _earlyLootTable = _itemsInEarlyChest.ToDictionary(x => x.Name, x => x);
            _lateLootTable = _itemsInLateChest.ToDictionary(x => x.Name, x => x);
            
            _lootTable = _earlyLootTable;
            foreach (var pair in _lateLootTable)
            {
                if (!_lootTable.Contains(pair))
                {
                    _lootTable.Add(pair.Key, pair.Value);
                }
            }
            
            
            //_chests[0].SetupChest("A", new Item("Sword", 10));
            StartCoroutine(SpawnEnemies());
            
            GenerateContent();
        }

        #region Generation

        private void GenerateContent()
        {
            foreach (var chest in _chests)
            {
                chest.SetupChest(_chests.IndexOf(chest)+1,GetRandomItem(chest.Difficulty));
            }

            foreach (var chest in _chests)
            {
                if (chest.Item.Name == "Key")
                {
                    if (chest.Difficulty == Difficulty.Late)
                    {
                        IEnumerable<Chest> lateChest = _chests.Where(x => x.Difficulty == Difficulty.Late);

                        List<Chest> enumerable = lateChest.ToList();
                        enumerable.ToList()[Random.Range(0,enumerable.Count())].AddParent(chest);
                    }
                    else
                    {
                        _chests[Random.Range(0,_chests.Count())].AddParent(chest);
                    }
                }
            }
        }


        private Item GetRandomItem(Difficulty difficulty)
        {
            float chance = Random.value * 100;
            
            if (difficulty == Difficulty.Early)
            {
                return chance > keyDropChance ? _earlyLootTable.Values.ToList()[Random.Range(0, _earlyLootTable.Count)] : _earlyLootTable["Key"] ;
            }
                            
            return chance > keyDropChance ? _lateLootTable.Values.ToList()[Random.Range(0, _lateLootTable.Count)] : _lateLootTable["Key"];
        }

        #endregion
        
        

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
