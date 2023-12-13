using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        //TODO: document qui explique l'algo; section soluce; boutons qui add des coffres; fix locked chest (key in each others chest)

        [SerializeField] private Player player;
        
        [SerializeField] private List<Item> _itemsInLateChest = new List<Item>();
        [SerializeField] private List<Item> _itemsInEarlyChest = new List<Item>();
        [SerializeField] private Item doorKey;
        [SerializeField] private Item endDoorKey;
        [SerializeField] private GameObject chests;
        private List<Chest> _chests = new();
        [SerializeField] private List<Door> doors = new();
        [SerializeField] private Door endDoor ;
        [SerializeField] private List<GameObject> _enemies = new List<GameObject>();
        [SerializeField] private TextMeshProUGUI _seedText;
        [SerializeField] private int keyDropChance;
        

        private Dictionary<string, Item> _earlyLootTable = new Dictionary<string, Item>();
        private Dictionary<string, Item> _lateLootTable = new Dictionary<string, Item>();
        private Dictionary<string, Item> _lootTable = new Dictionary<string, Item>();
        private void Start()
        {
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

            for (int i = 0; i < chests.transform.childCount; i++)
            {
                _chests.Add(chests.transform.GetChild(i).GetComponent<Chest>());
            }
            
            
            //_chests[0].SetupChest("A", new Item("Sword", 10));
            StartCoroutine(SpawnEnemies());
            
            GenerateContent();

            _seedText.text = Random.seed.ToString();
        }

        #region Generation

        private void GenerateContent()
        {
            //Generate lists of early chests and late chests
            List<Chest> earlyChestsList = _chests.Where(x => x.Difficulty == Difficulty.Early).ToList();
            
            List<Chest> lateChestList = _chests.Where(x => x.Difficulty == Difficulty.Late).ToList();
            
            //Generate a key in an early chest to open doors that will lead to the late zone 
            int chestIndex = Random.Range(0, earlyChestsList.Count);
            
            Chest tempchest = earlyChestsList[chestIndex];
            
            //generate the key in a random early chest 
            tempchest.SetupChest(_chests.IndexOf(tempchest)+ 1, doorKey);
            
            //give the chest to the doors so that we can check if chest opened before opening the door 
            foreach (var door in doors)
            {
                door.AddParent(earlyChestsList[chestIndex]);
            }
            
            //Generate the end key to open the end door
            chestIndex = Random.Range(0, lateChestList.Count);
            
            tempchest = lateChestList[chestIndex];
            
            //generate the key in a random late chest 
            tempchest.SetupChest(_chests.IndexOf(tempchest)+ 1, doorKey);
            
            //give the chest to the door so that we can check if chest opened before opening the door 
            endDoor.AddParent(lateChestList[chestIndex]);
            
            //The previous part ensures that every door is openable 
            
            
            //Now we generate random items in the chests
            foreach (var chest in _chests)
            {
                if (!chest.isSetup)
                {
                    chest.SetupChest(_chests.IndexOf(chest)+1,GetRandomItem(chest.Difficulty));
                }
            }

            //for each chest if they have a key we will affect that key to another chest of the same difficulty or above, we also remove itself from the pool of chests 
            //also if that key is for the end door we add it to the end door's check list
            //also if that key is for the other doors we add it to the other doors' check list
            foreach (var chest in _chests)
            {
                // If key and additionalData is set to 1 then it's a key for doors
                // If key and additionalData is set to 2 then it's a key for end door
                if (chest.Item.IsKey && chest.Item.AdditionalData == 0)
                {
                    switch (chest.Item.AdditionalData)
                    { 
                        case 1 :
                            foreach (var door in doors)
                            {
                                door.AddParent(chest);
                            }
                            break;
                        
                        case 2 : 
                            endDoor.AddParent(chest);
                            break;
                        
                        default:
                            if (chest.Difficulty == Difficulty.Late)
                            {
                                lateChestList.Where(x => x != chest).ToList()[Random.Range(0, lateChestList.Count-1)].AddParent(chest);
                            }
                            else
                            {
                                _chests.Where(x => x != chest).ToList()[Random.Range(0,_chests.Count-1)].AddParent(chest);
                            }
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// Returns a random item from the loot table, drop for keys have a %
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns> Item </returns>
        private Item GetRandomItem(Difficulty difficulty)
        {
            float chance = Random.value * 100;
            
            if (difficulty == Difficulty.Early)
            {
                //TODO : add all keys to loot tables
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

        public void CopySeed()
        {
            GUIUtility.systemCopyBuffer = Random.seed.ToString();
        }
    }
}
