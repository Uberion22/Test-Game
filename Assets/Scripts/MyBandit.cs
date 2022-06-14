using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyBandit : MonoBehaviour
{
    [SerializeField] private float m_speed = 0.40f;
    [SerializeField] private float health = 4.0f;
    [SerializeField] private float min_X_Pos = 3;
    [SerializeField] private float start_X_Pos = 9;
    [SerializeField] GameObject playerMarker;
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] private AudioSource audioSource;

    private bool isAttacking;
    private bool isEnemy = false;
    private Animator m_animator;
    private bool movingForward = false;
    private bool movingBack = false;
    private bool attack = false;
    private bool isPlayerStep = false;
    private bool isDead;
    private Color myYellow = new Color(1,0.92f,0.016f, 0.4f);
    private Color myBlue = new Color(0, 0, 1, 0.45f);
    private Color myRed = new Color(1, 0, 0, 0.4f);
    private Color myWhite = new Color(1, 1, 1, 0.0f);
    private SpriteRenderer playerMarkerSpriteRenderer;
    private MyBandit enemyAnimator;
    private const string attackTrigger = "Attack";
    private const string hitTrigger = "Hurt";
    private const string deathTrigger = "Death";

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
        playerMarkerSpriteRenderer = playerMarker.GetComponent<SpriteRenderer>();
        ClearColor();
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
            transform.Translate(new Vector2(m_speed * Time.deltaTime, 0));
        }
        else if(movingBack)
        {
            transform.Translate(new Vector2(-m_speed * Time.deltaTime, 0));
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
        m_animator.SetTrigger(attackTrigger);
        PlaySound(attackTrigger);
    }

    public void GetDamage(int damage)
    {
        ClearColor();
        isAttacking = false;
        isEnemy = true;
        health -= damage;
        if (health <= 0)
        {
            m_animator.SetTrigger(deathTrigger);
            PlaySound(deathTrigger);
            gameObject.tag = "Dead";
            isDead = true;
        }
        else
        {
            m_animator.SetTrigger(hitTrigger);
            PlaySound(hitTrigger);
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
            playerMarkerSpriteRenderer.color = myYellow;
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
            playerMarkerSpriteRenderer.color = myRed;
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

    private void SetPlayerStep(bool playerStep)
    {
        this.isPlayerStep = playerStep;
    }

    private void ClearColor()
    {
        playerMarkerSpriteRenderer.color = myWhite;
    }

    private void SetBlueColor()
    {
        playerMarkerSpriteRenderer.color = myBlue;
    }

    private bool HideCorps()
    {
        return isDead && movingBack;
    }

    private void PlaySound(string soundType)
    {
        switch (soundType)
        {
            case hitTrigger:
            {
                audioSource.clip = audioClips[0];
                audioSource.Play();
                break;
            }
            case deathTrigger:
            {
                audioSource.clip = audioClips[1];
                audioSource.Play();
                break;
            }
            case attackTrigger:
            {
                audioSource.clip = audioClips[2];
                audioSource.Play();
                break;
            }
        }
    }
}
