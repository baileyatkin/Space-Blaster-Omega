using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public Enemy[] row1Prefabs;
    public Enemy[] row2Prefabs;
    public Enemy[] row3Prefabs;
    public Enemy[] row4Prefabs;

    public int rows;
    public int columns;
    public int amountKilled { get; private set; }
    public int totalEnemies;
    public float percentageKilled;
    public float speederChance;
    public string currentDifficulty;

    public AnimationCurve speed;

    public System.Action waveKilled;
    public System.Action spawnSpeeder;

    private List<GunnerEnemy> gunnerList;
    private Vector3 enemyDirection = Vector2.right;
    private int currentGunner;
    private float shootCooldown;

    private void Awake()
    {
        this.totalEnemies = 0;
        this.percentageKilled = 0;
        this.currentGunner = 0;
        speederChance = 999.9f;
        gunnerList = new List<GunnerEnemy>();
        shootCooldown = Random.Range(1.0f, 5.0f);
        Enemy[][] prefabs = new Enemy[][]
        {
            row1Prefabs,
            row2Prefabs,
            row3Prefabs,
            row4Prefabs
        };

        for (int row = 0; row < rows; row++)
        {
            float width = 2.5f * (this.columns - 1);
            float height = -4.0f * (this.rows - 1);
            Vector3 centering = new Vector2(-width / 2, -height / 2);
            Vector3 rowPos = new Vector3(centering.x, centering.y + (row * 2.5f), 0.0f);
            for (int col = 0; col < columns; col++)
            {
                if (prefabs[row][col] != null)
                {
                    this.totalEnemies++;
                    Enemy enemy = Instantiate(prefabs[row][col], this.transform);
                    enemy.killed += EnemyKilled;
                    Vector3 position = rowPos;
                    position.x += col * 2.5f;
                    enemy.transform.position = position;

                    if (enemy.GetComponent<GunnerEnemy>())
                    {
                        GunnerEnemy gunner = enemy.gameObject.GetComponent<GunnerEnemy>();
                        gunnerList.Add(gunner);
                    }

                    if (row <= 1)
                    {
                        enemy.setScore(10);
                    }
                    else if (row == 2)
                    {
                        enemy.setScore(20);
                    }
                    else if (row == 3)
                    {
                        enemy.setScore(30);
                    }
                }
            }
        }
        
        if (gunnerList.Count == 0)
        {
            for (int i = 0; i < gunnerList.Count; i++)
            {
                GunnerEnemy temp = gunnerList[i];
                int randomIndex = Random.Range(i, gunnerList.Count);
                gunnerList[i] = gunnerList[randomIndex];
                gunnerList[randomIndex] = temp;
            }
        }
    }

    private void Update()
    {
        this.percentageKilled = (float)this.amountKilled / (float)this.totalEnemies;

        if (speederChance <= percentageKilled) 
        {
            this.spawnSpeeder.Invoke();
        }
        if (transform.childCount == 0)
        {
            this.waveKilled.Invoke();
            Destroy(this.gameObject);
        }

        if (shootCooldown > 0.0f)
        {
            shootCooldown -= Time.deltaTime;
        }

        this.transform.position += enemyDirection * this.speed.Evaluate(this.percentageKilled) * Time.deltaTime;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        foreach (Transform enemy in this.transform)
        {
            if (enemy == null)
            {
                continue;
            }

            if (enemyDirection == Vector3.right && enemy.position.x >= (rightEdge.x - 1.0f))
            {
                MoveRowDown();
            }
            else if (enemyDirection == Vector3.left && enemy.position.x <= (leftEdge.x + 1.0f))
            {
                MoveRowDown();
            }
        }

        if(shootCooldown <= 0.0f)
        {
            if (currentGunner >= gunnerList.Count)
            {
                currentGunner = 0;
            }
            else
            {
                if (gunnerList[currentGunner] == null)
                {
                    gunnerList.RemoveAt(currentGunner);
                }
                else
                {
                    gunnerList[currentGunner].Shoot();
                    shootCooldown = Random.Range(0.5f, 4.0f);
                }
                currentGunner += 1;
            }
        }
    }

    private void MoveRowDown()
    {
        enemyDirection.x *= -1.0f;

        Vector3 pos = this.transform.position;
        pos.y -= 1.0f;
        this.transform.position = pos;
    }

    private void EnemyKilled()
    {
        this.amountKilled += 1;
        speederChance = Random.Range(0.0f, 100.0f) / 100;
    }
}
