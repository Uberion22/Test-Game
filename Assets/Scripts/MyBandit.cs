using System;
using UnityEngine;
using System.Collections;

public class MyBandit : MonoBehaviour
{
    [SerializeField] private float m_speed = 0.40f;
    [SerializeField] private float health = 4.0f;
    [SerializeField] private float unitPower = 2.0f;
    [SerializeField] private float min_X_Pos = 3;
    [SerializeField] private float start_X_Pos = 9;
    [SerializeField] GameObject playerMarker;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator m_animator;
    [SerializeField] private SpriteRenderer playerMarkerSpriteRenderer;
    
    private bool isAttacking;
    private bool isEnemy;
    private bool movingForward;
    private bool movingBack;
    private bool attack;
    private bool isPlayerStep;
    private bool isDead;
    
    private MyBandit enemyAnimator;

    public static event Action<string> enemySelected;
    public static event Action startNextStep;


    // Use this for initialization
    void Start()
    {
        enemySelected += SetAsEnemy;
        GameManager.ckeckIsPlayerStep += SetPlayerStep;
        GameManager.cleareColor += ClearColor;
        start_X_Pos = transform.position.x;
        ClearColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (movingForward || movingBack)
        {
            m_animator.SetInteger(TriggersNames.AnimationState, 2);
        }
        //Combat Idle
        else
        {
            m_animator.SetInteger(TriggersNames.AnimationState, 1);
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
        var coef = 15.0f / (Math.Abs(start_X_Pos) - Math.Abs(min_X_Pos));
        if (movingForward)
        {
            transform.Translate(GetUnitTransformVector());
        }
        else if(movingBack)
        {
            transform.Translate(-GetUnitTransformVector());
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

    private IEnumerator WaitBeforeAttack()
    {
        yield return new WaitForSeconds(2);
        m_animator.SetTrigger(TriggersNames.AttackTrigger);
    }

    public void GetDamage(float damage)
    {
        ClearColor();
        isAttacking = false;
        isEnemy = true;
        health -= damage;
        if (health <= 0)
        {
            m_animator.SetTrigger(TriggersNames.DeathTrigger);
            gameObject.tag = CharacterTags.Dead;
            isDead = true;
        }
        else
        {
            m_animator.SetTrigger(TriggersNames.HitTrigger);
        }

        StartCoroutine(WaitAfterAttack(2.5f));
    }

    public void SendDamage()
    {
        enemyAnimator.GetDamage(unitPower);
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

    private void OnMouseDown()
    {
        if (isPlayerStep && gameObject.CompareTag(CharacterTags.Enemy))
        {
            enemySelected?.Invoke(gameObject.name);
        }
    }

    private void OnMouseEnter()
    {
        if (isPlayerStep && gameObject.CompareTag(CharacterTags.Enemy) && !isEnemy)
        {
            playerMarkerSpriteRenderer.color = MyColors.MyYellow;
        }
    }

    private void OnMouseExit()
    {
        if (!isEnemy && !gameObject.CompareTag(CharacterTags.Player))
        {
            ClearColor();
        }
    }

    private void SetAsEnemy(string name)
    {
        if (gameObject.name == name)
        {
            isEnemy = true;
            playerMarkerSpriteRenderer.color = MyColors.MyRed;
        }
        else
        {
            isEnemy = false;
            if (gameObject.CompareTag(CharacterTags.Enemy))
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
        playerMarkerSpriteRenderer.color = MyColors.MyWhite;
    }

    private void SetBlueColor()
    {
        playerMarkerSpriteRenderer.color = MyColors.MyBlue;
    }

    private bool HideCorps()
    {
        return isDead && movingBack;
    }

    private Vector2 GetUnitTransformVector()
    {
        var vectorCoef = 15.0f / (Math.Abs(start_X_Pos) - Math.Abs(min_X_Pos));
        
        return new Vector2(m_speed * Time.deltaTime, -Time.deltaTime * vectorCoef);
    }

    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

}
