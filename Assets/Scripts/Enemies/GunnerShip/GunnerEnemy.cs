using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script that handles everything for the enemy ships that shoot at the player, on creation, the Director will pass on the path of the ship and the difficulty that the game is currently set to.
//The difficulty affects how the AI will aim and fire their projectiles as well as the size of the Projectile detection area

public class GunnerEnemy : Enemy
{
    //Setup variables
    public GameObject deathEffect;
    public EnemyMissile missilePrefab;
    public float speed = 5.0f;

    public string difficulty;

    private Player player;

    // Start is called before the first frame update
    public override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        difficulty = transform.parent.GetComponent<Enemies>().currentDifficulty;

        if (difficulty == "Easy")
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public void Shoot()
    {
        if (difficulty == "Hard")
        {
            Vector3 targetPos = player.transform.position + player.GetComponent<SpriteRenderer>().bounds.size.y / 2.0f * player.transform.up;
            // Calculate the player's future position based on their current position and velocity
            Vector3 targetVelocity = new Vector3(player.GetComponent<Player>().movementVelocity, 0, 0);
            float timeToReachTarget = Vector3.Distance(transform.position, targetPos) / missilePrefab.speed;
            Vector3 futurePosition = targetPos + targetVelocity * timeToReachTarget;

            // Calculate the direction towards the player's future position
            Vector3 direction = (futurePosition - transform.position).normalized;
            direction.y = -direction.y;
            direction.x = -direction.x;

            // Calculate the rotation based on the adjusted direction
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);

            // Instantiate the missile at the current position with the adjusted rotation
            EnemyMissile missile = Instantiate(missilePrefab, transform.position, rotation);
        }

        else
        {
            EnemyMissile missile = Instantiate(this.missilePrefab, this.transform.position, Quaternion.identity);

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            player.AddScore(enemyValue);
            GameObject deathExplosion = Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else
        {
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            player.TakeDamage();
            GameObject deathExplosion = Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
