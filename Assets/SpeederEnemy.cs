using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeederEnemy : Enemy
{
    public GameObject deathEffect;
    public float speed;
    public Vector3 direction;
    private Player player;

    // Start is called before the first frame update
    public override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    public override void Update()
    {
        this.transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            player.AddScore(25);
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
