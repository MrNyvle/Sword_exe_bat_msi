using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 20.0f;

    private Rigidbody2D _rb;
    private Player_InputActions _inputAction;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _playerDirection = Vector2.zero;
    private Animator _animator;

    private List<Chest> _chests = new ();

    private List<Item> _inventory = new ();
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _inputAction = new Player_InputActions();
        _inputAction.Enable();
    }

    // Start is called before the first frame update
    void Start()
    { 

        _inputAction.Player.Move.performed += moving =>
        {
            _playerDirection = moving.ReadValue<Vector2>();
        };
        _inputAction.Player.Move.canceled += moving =>
        {
            _playerDirection = moving.ReadValue<Vector2>();
        };
        _inputAction.Player.Interact.performed += interacting =>
        {
            Interact();
        };
    }

    private void FixedUpdate()
    {
        Debug.Log(_playerDirection);
        _rb.velocity = _playerDirection * _speed;
        if (_playerDirection.x > 0) 
        {
            _spriteRenderer.flipX = false;
        }
        if (_playerDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        _animator.SetBool("isMoving", _playerDirection == Vector2.zero ? false : true);
        
        Debug.Log(_inventory.Count);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Chest"))
        {
            Chest chest = other.GetComponent<Chest>();
            
            chest.ToggleMessage();

            _chests.Add(chest);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Chest"))
        {
            Chest chest = other.GetComponent<Chest>();
            
            chest.ToggleMessage();

            if (_chests.Contains(chest))
            {
                _chests.Remove(chest);
            }
        }
    }

    private void Interact()
    {
        Debug.Log("interact");
        if (_chests.Count>0)
        {
            foreach (var chest in _chests)
            {
                chest.OpenChest();
            }
        }
    }
    
    public List<Item> Inventory
    {
        get => _inventory;
        set => _inventory = value;
    }
}
