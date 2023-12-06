using UnityEngine;

namespace _Scripts
{
    public class Item
    {
        private string _name;
        private int _value;


        public Item(string name, int value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int Value
        {
            get => _value;
            set => _value = value;
        }
    }
}
