using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathMovement : MonoBehaviour
{
    [Header("移動與跳躍參數")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("移動方向向量陣列 (符合作業 Vector 與陣列要求)")]
    // 陣列索引對應：
    // [0] = Vector2.zero  (靜止)
    // [1] = Vector2.right (向右)
    // [2] = Vector2.left  (向左)
    // [3] = Vector2.up    (向上跳躍)
    public Vector2[] moveDirections = new Vector2[]
    {
        Vector2.zero,
        Vector2.right,
        Vector2.left,
        Vector2.up
    };

    private Rigidbody2D rb;
    private bool isGrounded = true;

    void Start()
    {
        // 取得角色身上的 Rigidbody2D 元件以進行物理跳躍
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true; // 鎖定 Z 軸旋轉，避免人物倒下
        }
    }

    void Update()
    {
        // 1. 偵測鍵盤左右輸入，並從 Vector 陣列取得對應的方向
        int dirIndex = 0; // 預設為 0 (靜止 Vector2.zero)

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            dirIndex = 1; // 對應陣列中的 Vector2.right
            // 角色面向右側
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            dirIndex = 2; // 對應陣列中的 Vector2.left
            // 角色水平翻轉面向左側
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // 使用陣列裡的 Vector 計算水平速度
        Vector2 horizontalVelocity = moveDirections[dirIndex] * moveSpeed;
        rb.linearVelocity = new Vector2(horizontalVelocity.x, rb.linearVelocity.y);

        // 2. 偵測空白鍵 (Space) 或上鍵進行跳躍 (使用陣列裡的 Vector2.up)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            Vector2 jumpVector = moveDirections[3] * jumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVector.y);
            isGrounded = false;
        }
    }

    // 碰撞偵測：碰到地面代表落地，允許再次起跳
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 只要碰到底部物件就重置跳躍狀態
        isGrounded = true;
    }
}