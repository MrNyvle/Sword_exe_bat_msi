using _Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SolutionDisplayer : MonoBehaviour
{
    [SerializeField] private PlayerUI _playerUI;

    private Door _endDoor;
    private Dictionary<int, List<Chest>> _chestToOpenRecursive = new Dictionary<int, List<Chest>>();
    private List<Chest> _chestToOpenEndDoor = new List<Chest>();
    private List<Chest> _chestToOpenSecondZoneDoor = new List<Chest>();
    private List<Chest> _chestList = new List<Chest>();
    private Chest[] chests = null;

    private int _i = 0;
    private bool _doOnce = false;

    [Button("Solve")]
    public void StartSolving()
    {
        _endDoor = GameManager.instance.EndDoor;
        chests = new Chest[GameManager.instance.NumberOfChests + 1];
        _chestToOpenRecursive.Clear();
        _chestToOpenEndDoor = _endDoor.ChestsForOpen;
        foreach (Chest chest in _chestToOpenEndDoor)
        {
            if (chest.DoorForOpen)
            {
                _chestToOpenSecondZoneDoor = new List<Chest>(chest.DoorForOpen.ChestsForOpen);
            }
        }
        ClearDupe(ref _chestToOpenEndDoor);
        ClearDupe(ref _chestToOpenSecondZoneDoor);
        _chestList = new List<Chest>(_chestToOpenEndDoor);
        Solve(_chestList);
        StringAppendDisplay();
    }
    private void ClearDupe(ref List<Chest> ListToClearDupe)
    {
        ListToClearDupe = ListToClearDupe.Distinct().ToList();
    }

    private void Solve(List<Chest> RecursiveForList)
    {
        _chestToOpenRecursive[_i] = new List<Chest>(RecursiveForList);
        _chestList.Clear();
        bool continueRecursive = false;
        foreach (Chest chest in _chestToOpenRecursive[_i])
        {
            if (chest.ChestsForOpen.Count > 0)
            {
                TextMeshProUGUI tmp = _playerUI.SpawnNewLine(chest.id).GetComponent<TextMeshProUGUI>();
                foreach (Chest chestInside in chest.ChestsForOpen)
                {
                    tmp.text += chestInside.id + " ";
                }
            }
            continueRecursive = chest.ChestsForOpen.Count > 0 || continueRecursive;
            _chestList.AddRange(chest.ChestsForOpen);
        }
        ClearDupe(ref _chestList);
        _i++;
        if (continueRecursive)
        {
            Solve(_chestList);
        }
        else
        {
            if (!_doOnce)
            {
                _doOnce = true;
                Solve(_chestToOpenSecondZoneDoor);
            }
        }
    }

    private void StringAppendDisplay()
    {
        foreach (Chest chest in _chestToOpenEndDoor)
        {
            _playerUI.EndDoorRequirement.text += chest.id + " ";
        }

        foreach (Chest chest in _chestToOpenSecondZoneDoor)
        {
            _playerUI.ZoneTwoDoorRequirement.text += chest.id + " ";
        }
    }
}
