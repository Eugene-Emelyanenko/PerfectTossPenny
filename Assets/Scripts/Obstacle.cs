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
        // ���������� ������� ��������� �� ��� Y
        float currentY = transform.position.y;

        // ���� �������� ����� � �������� ��� ��������� maxY, �������� ��������� ����
        if (isMovingUp)
        {
            if (currentY >= maxY)
            {
                isMovingUp = false;
            }
        }
        // ���� �������� ���� � �������� ��� ���� minY, �������� ��������� �����
        else
        {
            if (currentY <= minY)
            {
                isMovingUp = true;
            }
        }

        // ��������� ��������� ������� � ����������� �� ����������� ��������
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
