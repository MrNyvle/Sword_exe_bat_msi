using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "Item", menuName = "Create Item So")]
    public class Item : ScriptableObject
    {
        public string Name;
        public int AdditionalData;


        public Item(string name, int value)
        {
            Name = name;
            AdditionalData = value;
        }
    }
}
