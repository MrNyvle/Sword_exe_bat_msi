using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts
{
    public class CooldownManager : Singleton<CooldownManager>
    {
        private Dictionary<string, float> _cooldown = new Dictionary<string, float>();

        public void StartCooldown(string nameOfCooldown, float cooldownDuration)
        {
            
        }
    }
}