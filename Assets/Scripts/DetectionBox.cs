using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Purpose of this is to detect player projectiles that are approaching the ship and dodge accordingly, difficulty level determines the size of the detection range and how fast the enemy ship will move to dodge the player projectile
//This will make the player have to aim towards the center of the enemy ships to ensure that their shots will kill the enemy

public class DetectionBox : MonoBehaviour
{
    public float detectionRange;
    private bool isSafe;
    private string difficulty;
    public int dodgeSpeed;

    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ShieldEnemy"))
        {
            isSafe = true;

        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Laser") && !isSafe)
        {
            float distance = Vector2.Distance(transform.parent.position, other.transform.position);
            if (distance <= detectionRange)
            {
                Vector3 direction = (transform.parent.position - other.transform.position).normalized;
                transform.parent.position += new Vector3(direction.x, 0, 0) * (dodgeSpeed) * Time.deltaTime;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ShieldEnemy"))
        {
            isSafe = false;
        }
    }

}
