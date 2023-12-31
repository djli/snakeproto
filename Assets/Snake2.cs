using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake2 : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.left;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 6;
    public bool moveThroughWalls = false;
    public int counter = 0;
    Vector3 startPos = new Vector3(20, 0, 0);
    public int playerTwoLives = 4;
    public GameObject scramble;
    private float lifeCD = 1.0f;
    bool canDie = true;

    private List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;
    public GameObject player1;
    private bool isScrambled = false;
    private float scrambleTimer = 0f;
    private GameObject[] lives;

    private void Start()
    {
        ResetState();
        lives = GameObject.FindGameObjectsWithTag("PlayerTwoLives");
    }

    private void Update()
    {
        if (playerTwoLives < 0f)
        {
            Destroy(this.gameObject);
        }
        scrambleTimer -= Time.deltaTime;
        lifeCD -= Time.deltaTime;
        if (lifeCD < 0)
        {
            canDie = true;
        }
        if (scrambleTimer < 0f)
        {
            isScrambled = false;
            scramble.GetComponent<SpriteRenderer>().enabled = false;
        }

        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                input = Vector2Int.up;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                input = Vector2Int.down;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                input = Vector2Int.right;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                input = Vector2Int.left;
            }
        }
    }

    public void Scramble()
    {
        isScrambled = true;
        scrambleTimer = 5.0f;
        scramble.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void FixedUpdate()
    {
        // Wait until the next update before proceeding
        if (Time.time < nextUpdate)
        {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2Int.zero)
        {
            if (!isScrambled)
            {
                direction = input;
            }
            else
            {
                direction = -input;
            }
        }

        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
    {
        counter++;
        canDie = false;
        lifeCD = 0.5f;
        GameObject[] gos = GameObject.FindGameObjectsWithTag("SpeedUp");
        foreach (GameObject go in gos)
            Destroy(go);
        gos = GameObject.FindGameObjectsWithTag("PlusThree");
        foreach (GameObject go in gos)
            Destroy(go);
        gos = GameObject.FindGameObjectsWithTag("Scramble");
        foreach (GameObject go in gos)
            Destroy(go);
        input = Vector2Int.zero;
        direction = Vector2Int.left;
        transform.position = startPos;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++)
        {
            Grow();
        }
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Grow();
        }
        else if (other.gameObject.CompareTag("PlusThree"))
        {
            Grow();
            Grow();
            Grow();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Scramble"))
        {
            player1.GetComponent<Snake>().Scramble();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("SpeedUp"))
        {
            if (speed < 25f)
            {
                speed += 5f;
            }
            else
            {
                Grow();
                Grow();
            }
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            if (canDie)
            {
                Destroy(lives[playerTwoLives]);
                playerTwoLives--;
                canDie = false;
                lifeCD = 1.0f;
            }
            player1.GetComponent<Snake>().ResetState();
            ResetState();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls)
            {
                Traverse(other.transform);
            }
            else
            {
                if (canDie)
                {
                    Destroy(lives[playerTwoLives]);
                    playerTwoLives--;
                    canDie = false;
                    lifeCD = 1.0f;
                }
                player1.GetComponent<Snake>().ResetState();
                ResetState();
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f)
        {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        }
        else if (direction.y != 0f)
        {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }
}
