using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public Collider2D gridArea;
    const float CD = 10f;
    private float cooldown = CD;
    public GameObject plusThree;
    public GameObject scramble;
    public GameObject speed;

    private Snake snake;

    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown < 0f)
        {
            cooldown = CD;
            switch (Random.Range(0, 3))
            {
                case 0:
                    Instantiate(plusThree, RandomizePosition(), Quaternion.identity);
                    break;
                case 1:
                    Instantiate(scramble, RandomizePosition(), Quaternion.identity);
                    break;
                case 2:
                    Instantiate(plusThree, RandomizePosition(), Quaternion.identity);
                    break;
            }
        }
    }

    public Vector2 RandomizePosition()
    {
        Bounds bounds = gridArea.bounds;

        // Pick a random position inside the bounds
        // Round the values to ensure it aligns with the grid
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Prevent the food from spawning on the snake
        while (snake.Occupies(x, y))
        {
            x++;

            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y)
                {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        Vector2 spawnPos = new Vector2(x, y);
        return spawnPos;
    }

}
