using System;
using UnityEngine;
using System.Collections;

public class MyBandit : MonoBehaviour
{

    [SerializeField] float m_speed = 0.40f;
    [SerializeField] float health = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float min_X_Pos = 3;
    [SerializeField] float start_X_Pos = 9;
    [SerializeField] bool isAttacking = true;
    [SerializeField] bool isEnemy = false;
    [SerializeField] GameManager gameManager;


    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private bool m_combatIdle = false;
    private bool m_isDead = false;

    private bool movingForward = false;
    private bool movingBack = false;

    private bool attack = false;
    private MyBandit enemyAnimator;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        //m_body2d = GetComponent<Rigidbody2D>();
        start_X_Pos = transform.position.x;
        enemyAnimator = GameObject.Find("LightBandit_0 (2)").GetComponent<MyBandit>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movingForward || movingBack)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else
            m_animator.SetInteger("AnimState", 1);

        if (Math.Abs(transform.position.x) <= Math.Abs(min_X_Pos))
        {
            movingForward = false;
            if (!movingBack && !attack && isAttacking)
            {
                attack = true;
                StartCoroutine(WaitBeforeAttack());
            }
        }
        
        if (Math.Abs(transform.position.x) >= Math.Abs(start_X_Pos))
        {
            movingBack = false;
        }

        if (movingForward)
        {
            transform.Translate(new Vector2(m_speed * Time.deltaTime, transform.position.y));
            //m_body2d.velocity = new Vector2(m_speed, m_body2d.velocity.y);
        }
        else if(movingBack)
        {
            transform.Translate(new Vector2(-m_speed * Time.deltaTime, transform.position.y));
            //m_body2d.velocity = new Vector2(-m_speed, m_body2d.velocity.y);
        }

    }

    public void Attacking()
    {
        StartCoroutine(WaitAfterAttack());
    }

    IEnumerator WaitAfterAttack(float time = 2)
    {
        
        yield return new WaitForSeconds(time);
        
        movingBack = true;
        attack = false;
        isEnemy = false;
        if (isAttacking) gameManager.StartNextStep();
        isAttacking = false;
        
    }

    IEnumerator WaitBeforeAttack()
    {
        yield return new WaitForSeconds(2);
        m_animator.SetTrigger("Attack");
    }

    public void GetDamage(int damage)
    {
        isAttacking = false;
        isEnemy = true;
        //if(isAttacking) return;
        health -= damage;
        if (health <= 0)
        {
            m_animator.SetTrigger("Death");
            gameObject.tag = "Dead";
        }
        else
        {
            m_animator.SetTrigger("Hurt");
        }

        StartCoroutine(WaitAfterAttack(2.5f));
    }

    public void SendDamage(int damage)
    {
        enemyAnimator.GetDamage(damage);
    }

    public void StartAttack(object enemyName)
    {
        enemyAnimator = GameObject.Find(enemyName.ToString()).GetComponent<MyBandit>();
        enemyAnimator.movingForward = true;
        this.movingForward = true;
        isEnemy = false;
        isAttacking = true;
        Debug.Log($"Attack {enemyName.ToString()}");
    }

    void OnMouseDown()
    {
        if (gameManager.isPlayerStep && gameObject.CompareTag("Enemy"))
        {
            gameManager.PlayerAttack(gameObject.name);
        }
    }
}
