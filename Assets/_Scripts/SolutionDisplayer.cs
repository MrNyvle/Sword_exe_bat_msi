using _Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SolutionDisplayer : MonoBehaviour
{
    public string idstring;


    private Door _endDoor;
    private Dictionary<int, List<Chest>> _chestToOpenRecursive = new Dictionary<int, List<Chest>>();
    private List<Chest> _chestList = new List<Chest>();

    private int _i = 0;

    public void StartSolving()
    {
        _endDoor = GameManager.instance.EndDoor;
        _chestToOpenRecursive.Clear();
        _chestList = _endDoor.ChestsForOpen;
        ClearDupe(ref _chestList);
        Solve();
        StringAppendDisplay();
    }
    private void ClearDupe(ref List<Chest> ListToClearDupe)
    {
        ListToClearDupe = ListToClearDupe.Distinct().ToList();
    }

    private void Solve()
    {
        _chestToOpenRecursive[_i] = new List<Chest>(_chestList);
        _chestList.Clear();
        bool continueRecursive = false;
        foreach (Chest chest in _chestToOpenRecursive[_i])
        {
            continueRecursive = chest.ChestsForOpen.Count > 0 || continueRecursive;
            _chestList.AddRange(chest.ChestsForOpen);
            if (chest.DoorForOpen)
            {
                _chestList.AddRange(chest.DoorForOpen.ChestsForOpen);
            }
        }
        
        ClearDupe(ref _chestList);
        _i++;
        if (continueRecursive)
        {
            Solve();
        }
    }

    private void StringAppendDisplay()
    {
        for (int i = 0; i < _i; i++)
        {
            foreach (Chest chest in _chestToOpenRecursive[i])
            {
                Debug.Log("Recursivity number: " + i);
                Debug.Log("Chest ID: " + chest.id);
                idstring += chest.id;
            }
        }
    }
}
