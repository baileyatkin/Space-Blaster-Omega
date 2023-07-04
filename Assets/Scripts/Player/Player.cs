using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Projectile laserPrefab;
    public HealthBar healthBar;
    public SpriteRenderer spriteRenderer;
    public Color damageColour = Color.red;
    public AudioSource hitSound;
    public GameObject deathEffect;

    public float speed = 5.0f;
    public float playerInput;

    public int maxHealth = 3;
    public int currentHealth;
    public int score;

    public float flashDuration = 0.1f;
    public float movementVelocity = 0.0f;

    public bool paused;

    private bool laserActive;
    private bool stopMovement;
    private float minX;
    private float maxX;
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = 3;
        //Get boundaries
        Camera gameCamera = Camera.main;
        minX = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        maxX = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        Debug.Log("Player Loaded");
    }

    // Update is called once per frame
    private void Update()
    {
        if (currentHealth > 0)
        {
            //Perform player movement, movement is clamped so that the player does not move outside the boundary of the camera
            playerInput = Input.GetAxisRaw("Horizontal");
            float movement = (playerInput * speed * Time.deltaTime);
            float newPos = Mathf.Clamp(transform.position.x + movement, minX, maxX);
            transform.position = new Vector2(newPos, transform.position.y);

            if (Input.GetKeyDown(KeyCode.D))
            {
                movementVelocity = 5.0f;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                movementVelocity = -5.0f;
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                movementVelocity = 0.0f;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                movementVelocity = 0.0f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!paused)
                {
                    Shoot();
                }
            }
        }
        else
        {

            Die();
        }   
    }

    private void Die()
    {
        FinalScore(score);
        GameObject deathExplosion = Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    private void Shoot()
    {
        if (!laserActive) 
        {
            Projectile projectile = Instantiate(this.laserPrefab, this.transform.position, Quaternion.identity);
            projectile.destroyed += LaserDestroyed;
            laserActive = true;
        }
    }

    private void LaserDestroyed()
    {
        laserActive = false;
    }

    public void TakeDamage()
    {
        currentHealth -= 1;
        uiManager.UpdateHealth(currentHealth);
        StartCoroutine(FlashRed());
    }

    public void AddScore(int points)
    {
        score += points;
        uiManager.UpdateScore(score);
    }

    private void FinalScore(int points)
    {
        uiManager.FinalScore(points);
    }

    private IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = damageColour;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile"))
        {
            hitSound.Play();
            TakeDamage();
        }
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            this.transform.Translate(0.0f, 0.0f, 0.0f);
        }    
    }
}
