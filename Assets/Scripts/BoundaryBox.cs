using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryBox : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision");
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.TakeDamage();
    }
}