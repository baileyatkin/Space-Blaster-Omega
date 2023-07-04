using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public System.Action killed;

    public int enemyValue;

    // Start is called before the first frame update
    public abstract void Start();

    // Update is called once per frame
    public abstract void Update();

    public void setScore(int score)
    {
        this.enemyValue = score;
    }
}
