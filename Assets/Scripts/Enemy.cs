using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    private int health;
    public bool alive;
    public float range = 10.0f;
    private float targetDist;

    public float fireRate = 0.5f;
    private float fireTimer;

    private Score scoreRef;
    private Gun gunRef;
    public LevelData levelRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireTimer = -10.0f;
        alive = true;
        levelRef = transform.parent.parent.GetComponent<LevelData>();
        scoreRef = levelRef.scoreRef;
        health = levelRef.enemyHealth;
        gunRef = GetComponentInChildren<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreRef == null)
        {
            scoreRef = levelRef.scoreRef;
        }

        findTarget();
    }

    private void findTarget()
    {
        RaycastHit hit;

        Vector3 target = levelRef.gameRef.playerRef.transform.position;

        targetDist = Vector3.Distance(target, transform.position);

        Vector3 direction = target - transform.position;
        direction = Vector3.Normalize(direction);

        fireTimer -= Time.deltaTime;

        if (targetDist <= range && fireTimer <= 0.0f)
        {
            if (fireTimer > -10.0f)
            { 
                fireTimer = fireRate; 
            }
            if (Physics.Raycast(transform.position, transform.forward, out hit, range))
            {
                Player player = hit.transform.GetComponent<Player>();
                if (player != null)
                {
                    fireTimer = fireRate;
                    gunRef.fireTargetPlayer(target, direction);
                    return;
                }
            }
            transform.LookAt(levelRef.gameRef.playerRef.transform);
        }
    }

    public void damage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            health = 0;
            die();
        }
    }

    void die()
    {
        Destroy(gameObject);
        alive = false;
        scoreRef.incrementScore(100);
        levelRef.decrementEnemy(1);
    }
}
