using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI EndDoorRequirement;
    public TextMeshProUGUI ZoneTwoDoorRequirement;
    public GameObject ChestRequirement;

    private List<GameObject> _chestRequirementsHolder = new List<GameObject>();

    public List<GameObject> ChestRequirementsHolder { get => _chestRequirementsHolder; set => _chestRequirementsHolder = value; }

    public GameObject SpawnNewLine(int chestID)
    {
        var go = Instantiate(ChestRequirement, this.transform);
        _chestRequirementsHolder.Add(go);
        go.GetComponent<TextMeshProUGUI>().text = "Chest required to Open Chest " + chestID + ": ";
        return go;
    }
}
