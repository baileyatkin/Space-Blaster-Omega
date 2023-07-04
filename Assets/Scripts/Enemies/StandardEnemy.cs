using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemy : Enemy
{
    public GameObject deathEffect;

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
        else if (difficulty == "Standard")
        {
            transform.GetChild(0).gameObject.GetComponent<DetectionBox>().dodgeSpeed = 2;
        }
        else if (difficulty == "Hard")
        {
            transform.GetChild(0).gameObject.GetComponent<DetectionBox>().dodgeSpeed = 5;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            player.AddScore(enemyValue);
            this.killed.Invoke();
            GameObject deathExplosion = Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else
        {
            player.TakeDamage();
            this.killed.Invoke();
            GameObject deathExplosion = Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
