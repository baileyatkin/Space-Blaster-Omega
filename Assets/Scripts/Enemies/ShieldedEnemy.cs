using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedEnemy : Enemy
{
    public SpriteRenderer spriteRenderer;
    public Sprite unshieldSprite;
    public GameObject deathEffect;
    public bool shielded;
    public float speed = 1.0f;

    private Player player;

    // Start is called before the first frame update
    public override void Start()
    {
        shielded = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    public override void Update()
    {
        //this.transform.position += Vector3.down * this.speed * Time.deltaTime;
    }

    void ChangeSprite()
    {
        spriteRenderer.sprite = unshieldSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            if (shielded)
            {
                shielded = false;
                ChangeSprite();
            }
            else
            {
                player.AddScore(enemyValue);
                this.killed.Invoke();
                GameObject deathExplosion = Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            player.TakeDamage();
            this.killed.Invoke();
            GameObject deathExplosion = Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
