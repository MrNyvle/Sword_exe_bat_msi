using System.Data;
using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "Item", menuName = "Create Item So")]
    public class Item : ScriptableObject
    {
        public string Name;
        public bool IsKey;
        public int AdditionalData;


        public Item(string name, bool isKey, int value)
        {
            Name = name;
            IsKey = isKey;
            AdditionalData = value;
        }
    }
}
