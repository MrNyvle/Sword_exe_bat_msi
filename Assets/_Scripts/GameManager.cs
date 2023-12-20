using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        //TODO: finir document qui explique l'algo; section soluce; fix locked chest (key in each others chest)

        [SerializeField] private Player player;
        [SerializeField] private SolutionDisplayer solutionDisplayer;

        [SerializeField] private List<Item> _itemsInLateChest = new List<Item>();
        [SerializeField] private List<Item> _itemsInEarlyChest = new List<Item>();
        [SerializeField] private Item doorKey;
        [SerializeField] private Item endDoorKey;

        [SerializeField][Range(4,15)] private int numberOfChests;
        [SerializeField] private GameObject spawnZones;
        [SerializeField] private GameObject chestPrefab;
        [SerializeField] private GameObject chestsParentObject;
        
        private List<Chest> _chests = new();
        [SerializeField] private List<Door> doors = new();
        [SerializeField] private Door endDoor ;
        [SerializeField] private List<GameObject> _enemies = new List<GameObject>();
        [SerializeField] private TextMeshProUGUI _seedText;
        [SerializeField] private int keyDropChance;
        

        private Dictionary<string, Item> _earlyLootTable = new Dictionary<string, Item>();
        private Dictionary<string, Item> _lateLootTable = new Dictionary<string, Item>();
        private Dictionary<string, Item> _lootTable = new Dictionary<string, Item>();
        
        public List<Door> Doors => doors;

        public Door EndDoor { get => endDoor; set => endDoor = value; }
        public int NumberOfChests { get => numberOfChests; set => numberOfChests = value; }

        private void Start()
        {
            if (PlayerPrefs.HasKey("Seed"))
            {
                Random.InitState(PlayerPrefs.GetInt("Seed"));
                _seedText.text = PlayerPrefs.GetInt("Seed").ToString();
            }
            else
            {
                _seedText.text = Random.seed.ToString();
            }

            if (PlayerPrefs.HasKey("NbChest"))
            {
                numberOfChests = PlayerPrefs.GetInt("NbChest");
            }
            
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
            
            StartCoroutine(SpawnEnemies());

            if (spawnZones != null)
            {
                SpawnChests();
            }
            
            GenerateContent();
            solutionDisplayer.StartSolving();
        }
        
        #region Generation
        private void SpawnChests()
        {
            //splits number of chest in half for each zones
            int numberOfLateChests = numberOfChests / 2 ;
            int numberOfEarlyChests = numberOfChests - numberOfLateChests;

            List<(Difficulty, BoxCollider2D)> spawnZonesList = new List<(Difficulty, BoxCollider2D)>();
            
            //fills my array of colliders depending of difficulty

            for (int i = 0; i < spawnZones.transform.childCount; i++)
            {
                String difficultyZone = spawnZones.transform.GetChild(i).name.Split(" - ")[1];

                switch (difficultyZone)
                {
                    case "Early":
                        spawnZonesList.Add((Difficulty.Early,spawnZones.transform.GetChild(i).GetComponent<BoxCollider2D>()));
                        break;
                    case "Late":
                        spawnZonesList.Add((Difficulty.Late,spawnZones.transform.GetChild(i).GetComponent<BoxCollider2D>()));
                        break;
                }
            }
            
            //generate the amount of chests asked in the correct difficulty zone, there are more than 1 zone per difficulty and that is selected at random
            //this part is for the early zone 
            for (int i = 0; i < numberOfEarlyChests; i++)
            {
                List<(Difficulty, BoxCollider2D)> EarlyZones = spawnZonesList.Where(x => x.Item1 == Difficulty.Early).ToList();
                (Difficulty, BoxCollider2D) zone = EarlyZones[Random.Range(0,EarlyZones.Count)];

                (Vector3, Vector3) cornersOfCollider = GetCornersOfCollider(zone.Item2);

                Vector3 spawnPos = GetPointInSquare(cornersOfCollider, zone.Item2);
                
                GameObject chestGameObject = Instantiate(chestPrefab, spawnPos, Quaternion.identity, chestsParentObject.transform);
                Chest chest = chestGameObject.GetComponent<Chest>();
                chest.Difficulty = Difficulty.Early;
                _chests.Add(chest);
            }
            //this part is for the late zone 
            for (int i = 0; i < numberOfLateChests; i++)
            {
                List<(Difficulty, BoxCollider2D)> LateZones = spawnZonesList.Where(x => x.Item1 == Difficulty.Late).ToList();
                (Difficulty, BoxCollider2D) zone = LateZones[Random.Range(0,LateZones.Count)];

                (Vector3, Vector3) cornersOfCollider = GetCornersOfCollider(zone.Item2);

                Vector3 spawnPos = GetPointInSquare(cornersOfCollider, zone.Item2);
                
                GameObject chestGameObject = Instantiate(chestPrefab, spawnPos, Quaternion.identity, chestsParentObject.transform);
                Chest chest = chestGameObject.GetComponent<Chest>();
                chest.Difficulty = Difficulty.Late;
                _chests.Add(chest);
            }
        }

        /// <summary>
        /// Returns a tuple of vector3 which corresponds to the top right corner and then bottom left corner respectively 
        /// </summary>
        /// <param name="collider2D"></param>
        /// <returns>(Vector3, Vector3)</returns>
        private (Vector3, Vector3) GetCornersOfCollider(BoxCollider2D collider2D)
        {
            Vector3 topRight = new Vector2(collider2D.offset.x + collider2D.bounds.extents.x, collider2D.offset.y + collider2D.bounds.extents.y);
            Vector3 bottomLeft = new Vector2(collider2D.offset.x - collider2D.bounds.extents.x, collider2D.offset.y - collider2D.bounds.extents.y);

            topRight += collider2D.transform.position;
            bottomLeft += collider2D.transform.position;

            return (topRight, bottomLeft);
        }

        /// <summary>
        /// Recursive Function that returns a Vector3 that is in a given collider, and no chest is on that position
        /// </summary>
        /// <param name="corners"></param>
        /// <param name="collider2D"></param>
        /// <returns> Vector3 </returns>
        private Vector3 GetPointInSquare((Vector3, Vector3) corners, BoxCollider2D collider2D)
        {
            
            int randX = (int)Random.Range(corners.Item1.x, corners.Item2.x);
            int randY = (int)Random.Range(corners.Item1.y, corners.Item2.y);
            
            Vector3 point = new Vector3(randX, randY, 0);
            
            if (collider2D.bounds.Contains(point) && _chests.All(x => x.gameObject.transform.position != point))
            {
                return point;
            }
            return GetPointInSquare(corners, collider2D);
        }
        
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
            // foreach (var door in doors)
            // {
            //     door.AddParent4(earlyChestsList[chestIndex]);
            // }
            
            //Generate the end key to open the end door
            chestIndex = Random.Range(0, lateChestList.Count);
            
            tempchest = lateChestList[chestIndex];
            
            //generate the key in a random late chest 
            tempchest.SetupChest(_chests.IndexOf(tempchest)+ 1, endDoorKey);
            
            //give the chest to the door so that we can check if chest opened before opening the door 
            // endDoor.AddParent(lateChestList[chestIndex]);
            
            //The previous part ensures that every door is openable 
            
            
            //Now we generate random items in the chests
            foreach (var chest in _chests.Where(x => x.isSetup == false))
            {
                chest.SetupChest(_chests.IndexOf(chest)+1,GetRandomItem(chest.Difficulty));
            }

            //for each chest if they have a key we will affect that key to another chest of the same difficulty or above,
            //also if that key is for the end door we add it to the end door's check list
            //also if that key is for the other doors we add it to the other doors' check list
            foreach (var chest in _chests)
            {
                // If key and additionalData is set to 1 then it's a key for doors
                // If key and additionalData is set to 2 then it's a key for end door
                if (chest.Item.IsKey)
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
            
            // check for loops 
            
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
                return chance > keyDropChance ? _earlyLootTable.Values.ToList()[Random.Range(0, _earlyLootTable.Count)] : _earlyLootTable["Key"] ;
            }
                            
            return chance > keyDropChance ? _lateLootTable.Values.ToList()[Random.Range(0, _lateLootTable.Count)] : _lateLootTable["Key"];
        }

        #endregion

        #region Tools/Other/PlayerInterraction

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
                    if (PlayerPrefs.HasKey("Seed"))
                    {
                        GUIUtility.systemCopyBuffer = PlayerPrefs.GetInt("Seed").ToString();
                    }
                    else
                    {
                       GUIUtility.systemCopyBuffer = Random.seed.ToString(); 
                    }
                }

        #endregion
        
    }
}
