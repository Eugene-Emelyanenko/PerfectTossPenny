using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float maxSpeed;
    public float minSpeed;
    public float maxY = 2f;
    public float minY = -2f;

    public SpriteRenderer spriteRenderer;

    private float moveSpeed;
    private bool isMovingUp = true;

    private void Start()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        transform.position = new Vector3(transform.position.x, Random.Range(minY, maxY), transform.position.z);
    }

    private void Update()
    {
        // Определяем текущее положение по оси Y
        float currentY = transform.position.y;

        // Если движемся вверх и достигли или превысили maxY, начинаем двигаться вниз
        if (isMovingUp)
        {
            if (currentY >= maxY)
            {
                isMovingUp = false;
            }
        }
        // Если движемся вниз и достигли или ниже minY, начинаем двигаться вверх
        else
        {
            if (currentY <= minY)
            {
                isMovingUp = true;
            }
        }

        // Обновляем положение объекта в зависимости от направления движения
        if (isMovingUp)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
    }
}
