using UnityEngine;

public class Item : MonoBehaviour
{
    private Score scoreRef;
    public LevelData levelRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelRef = transform.parent.parent.GetComponent<LevelData>();
        scoreRef = levelRef.scoreRef;
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreRef == null)
        {
            scoreRef = levelRef.scoreRef;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            scoreRef.incrementScore(250);
            levelRef.decrementItem(1);
        }
    }
}
