using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] private List<GameObject> _keysList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        _keysList[Random.Range(0, _keysList.Count)].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
