using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float tossForce = 10f;

    private Rigidbody2D rb;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();

        rb.simulated = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Box"))
        {
            Box box = collision.gameObject.GetComponent<Box>();
            gameManager.IncreaseScore(box.boxData.coinMultiplier);
            gameManager.CreatePlusCoinEffect();
            gameManager.arrow.gameObject.SetActive(true);
            gameManager.arrow.enabled = true;
            gameManager.CreateNewCoin();           
            Destroy(gameObject);
        }
        else if(collision.collider.CompareTag("Ground"))
        {
            gameManager.TakeDamage();
            gameManager.CreateTakeDamageEffect(transform.position.x);
            gameManager.arrow.gameObject.SetActive(true);
            gameManager.arrow.enabled = true;
            gameManager.CreateNewCoin();
            Destroy(gameObject);
        }
    }

    public void Toss(float zAngle)
    {
        rb.simulated = true;
        // ��������� ���� � �������
        float radianAngle = zAngle * Mathf.Deg2Rad;

        // ������������ ����������� ������ �� ���� X � Y
        Vector2 direction = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

        // ��������� ���� � Rigidbody2D
        rb.AddForce(direction * tossForce, ForceMode2D.Impulse);

        // ��������� ��������� ��������
        float randomTorque = Random.Range(-2f, 2f); // ��������� �������� �������� �� -10 �� 10
        rb.AddTorque(randomTorque, ForceMode2D.Impulse);
    }
}
