using System;
using UnityEngine;
using System.Collections;

public class MyBandit : MonoBehaviour
{
    [SerializeField] private float m_speed = 0.40f;
    [SerializeField] private float health = 4.0f;
    [SerializeField] private float min_X_Pos = 3;
    [SerializeField] private float start_X_Pos = 9;
    
    private bool isAttacking;
    private bool isEnemy = false;
    private Animator m_animator;
    private bool movingForward = false;
    private bool movingBack = false;
    private bool attack = false;
    private bool isPlayerStep = false;
    private bool isDead;
   
    private MyBandit enemyAnimator;
    private SpriteRenderer spriteRenderer;

    public static event Action<string> enemySelected;
    public static event Action startNextStep;


    // Use this for initialization
    void Start()
    {
        MyBandit.enemySelected += SetAsEnemy;
        GameManager.ckeckIsPlayerStep += SetPlayerStep;
        GameManager.cleareColor += ClearColor;
        m_animator = GetComponent<Animator>();
        start_X_Pos = transform.position.x;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movingForward || movingBack)
        {
            m_animator.SetInteger("AnimState", 2);
        }
        //Combat Idle
        else
        {
            m_animator.SetInteger("AnimState", 1);
        }

        if (Math.Abs(transform.position.x) <= Math.Abs(min_X_Pos))
        {
            movingForward = false;
            if (!movingBack && !attack && isAttacking)
            {
                attack = true;
                StartCoroutine(WaitBeforeAttack());
            }
        }
        
        if (Math.Abs(transform.position.x) >= Math.Abs(start_X_Pos) && movingBack)
        {
            movingBack = false;
            gameObject.SetActive(!HideCorps());
        }

        if (movingForward)
        {
            transform.Translate(new Vector2(m_speed * Time.deltaTime, transform.position.y));
        }
        else if(movingBack)
        {
            transform.Translate(new Vector2(-m_speed * Time.deltaTime, transform.position.y));
        }
    }

    public void Attacking()
    {
        StartCoroutine(WaitAfterAttack());
    }

    private IEnumerator WaitAfterAttack(float time = 2)
    {
        
        yield return new WaitForSeconds(time);
        
        movingBack = true;
        gameObject.SetActive(!HideCorps());
        attack = false;
        isEnemy = false;
        if (isAttacking)
        {
            startNextStep?.Invoke();
        }
        isAttacking = false;
    }

    IEnumerator WaitBeforeAttack()
    {
        yield return new WaitForSeconds(2);
        m_animator.SetTrigger("Attack");
    }

    public void GetDamage(int damage)
    {
        spriteRenderer.color = Color.white;
        isAttacking = false;
        isEnemy = true;
        health -= damage;
        if (health <= 0)
        {
            m_animator.SetTrigger("Death");
            gameObject.tag = "Dead";
            isDead = true;
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
        if (isPlayerStep && gameObject.CompareTag("Enemy"))
        {
            enemySelected?.Invoke(gameObject.name);
        }
    }

    void OnMouseEnter()
    {
        if (isPlayerStep && gameObject.CompareTag("Enemy") && !isEnemy)
        {
            spriteRenderer.color = Color.yellow;
        }
    }

    void OnMouseExit()
    {
        if (!isEnemy && !gameObject.CompareTag("Player"))
        {
            ClearColor();
        }
    }

    private void SetAsEnemy(string name)
    {
        if (gameObject.name == name)
        {
            isEnemy = true;
            spriteRenderer.color = Color.red;
        }
        else
        {
            isEnemy = false;
            if (gameObject.CompareTag("Enemy"))
            {
                ClearColor();
            }
        }
    }

    private void SetPlayerStep(bool isPlayerStep)
    {
        this.isPlayerStep = isPlayerStep;
    }

    private void ClearColor()
    {
        spriteRenderer.color = Color.white;
    }

    private bool HideCorps()
    {
        return isDead && movingBack;
    }
}
