using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 20.0f;
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private float _attackSpeed = 1.0f;

    private Rigidbody2D _rb;
    private Player_InputActions _inputAction;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _playerDirection = Vector2.zero;
    private Vector2 _shootDirection = Vector2.zero;
    private Animator _animator;
    private bool _isShooting = false;
    private float _nextShot = 0.0f;
    private bool _isBombAvailable = true;
    private List<GameObject> _interractables = new ();

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

        _inputAction.Player.Attack.performed += attack =>
        {
            _shootDirection = attack.ReadValue<Vector2>();
            _isShooting = true;
        };

        _inputAction.Player.Attack.canceled += attack =>
        {
            _shootDirection = attack.ReadValue<Vector2>();
            _isShooting = false;
        };

        _inputAction.Player.Interact.performed += interacting =>
        {
            Interact();
        };

        _inputAction.Player.Bomb.performed += bomb =>
        {

        };
    }

    private void FixedUpdate()
    {
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

        if (_isShooting && Time.time > _nextShot)
        {
            Shoot();
            _nextShot = Time.time + _attackSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Chest"))
        {
            Chest chest = other.GetComponent<Chest>();
            
            chest.ToggleMessage();
            _interractables.Add(chest.gameObject);         
        }
        if (other.tag.Equals("Door"))
        {
            Door door = other.GetComponent<Door>();  
            door.ToggleMessage();
            _interractables.Add(door.gameObject);                
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Chest"))
        {
            Chest chest = other.GetComponent<Chest>();
            chest.ToggleMessage(); 
            if (_interractables.Contains(chest.gameObject)) 
            { 
                _interractables.Remove(chest.gameObject);
            }
        }
        
        if (other.tag.Equals("Door"))
        {
            Door door = other.GetComponent<Door>();
            door.ToggleMessage(); 
            if (_interractables.Contains(door.gameObject)) 
            { 
                _interractables.Remove(door.gameObject);
            }
        }
    }

    private void Interact()
    {
        if (_interractables.Count>0)
        {
            foreach (var chest in _interractables)
            {
                if (chest.gameObject.tag.Equals("Chest"))
                {
                    chest.GetComponent<Chest>().OpenChest();
                }
                
                if (chest.gameObject.tag.Equals("Door"))
                {
                    chest.GetComponent<Door>().OpenDoor();
                }
            }
        }
    }

    private void Shoot()
    {
        if (_inventory.Contains(GameManager.instance.CheckLootTable("Bow")))
        {
            GameObject arrow;
            arrow = Instantiate(_arrowPrefab, transform.position, new Quaternion(0, 0, 0, 0));
            arrow.GetComponent<Rigidbody2D>().velocity = _shootDirection * 5;
            arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-_shootDirection.x, _shootDirection.y) * Mathf.Rad2Deg);
        }
    }

    private void Bomb()
    {
        if (_inventory.Contains(GameManager.instance.CheckLootTable("Bomb")) && _isBombAvailable)
        {
            
        }
    }
    
    public List<Item> Inventory
    {
        get => _inventory;
        set => _inventory = value;
    }
}
