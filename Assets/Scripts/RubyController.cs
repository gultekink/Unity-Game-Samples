using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class RubyController : MonoBehaviour
{
    public AudioClip damageZoneAudioClip;
    public AudioClip Clip;
    private NonPlayer nonPlayer;
    private UIHealthBar _healthBar;
    public GameObject projectilePrefab;
    private float speed = 3.0f;
    public int maxHealth = 5;
    private Animator _animator;
    public float timeInvincible = 2.0f;
    public Vector2 lookDirection = new Vector2(1,0);
    private bool isInvincible;
    private float invincibleTimer;
    private AudioSource _audiSource;
    private Rigidbody2D _rigidbody2D;
    private float _horizontal;
    private float _vertical;
    public int health { get{return currentHealth ; }}
    public int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
       _rigidbody2D = GetComponent<Rigidbody2D>();
       _animator = GetComponent<Animator>();
       currentHealth = maxHealth;
       _audiSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(_horizontal, _vertical);
        
        
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x,move.y);
            lookDirection.Normalize();
            
            
        }

        _animator.SetFloat("Look X",lookDirection.x);
        _animator.SetFloat("Look Y",lookDirection.y);
        _animator.SetFloat("Speed",move.magnitude);
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 5.0f;
        }
        else
        {
            speed = 3.0f;
        }


        if (isInvincible)
        {
            
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0) 
                isInvincible = false;

        }

    if (Input.GetKeyDown(KeyCode.C))
    {
        Launch();
    }

    if (Input.GetKey(KeyCode.X))
    {
        RaycastHit2D rayCast = Physics2D.Raycast(_rigidbody2D.position + Vector2.up * 0.2f, lookDirection , 1.5f,
            LayerMask.GetMask("NPC"));
        if (rayCast.collider != null)
        {
            nonPlayer = rayCast.collider.GetComponent<NonPlayer>();
            if (nonPlayer != null)
            {
                nonPlayer.DisplayDialog();
            }

            }
        }
    }

    public void PlayShot(AudioClip clip)
    {
        _audiSource.PlayOneShot(clip);
    }

    void FixedUpdate()
    {
        Vector2 position = _rigidbody2D.position;
        position.y = position.y + speed * _vertical * Time.deltaTime;
        position.x = position.x + speed * _horizontal * Time.deltaTime;

        _rigidbody2D.MovePosition(position);
    }

    public void Damage()
    {
        _audiSource.PlayOneShot(damageZoneAudioClip);
    }
    
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            _animator.SetTrigger("Hit");
            if (isInvincible)
                return;
            isInvincible = true;
                invincibleTimer = timeInvincible;
            Damage();
                
            
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float) maxHealth);
        
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, _rigidbody2D.position + Vector2.up * 0.5f, Quaternion.identity);
        
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        PlayShot(Clip);
        _animator.SetTrigger("Launch");
    }

}
