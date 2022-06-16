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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator m_animator;
    [SerializeField] private SpriteRenderer playerMarkerSpriteRenderer;
    
    private bool isAttacking;
    private bool isEnemy;
    private MovingDirection movingDirectionModifer;
    private bool isDead;
    private MyBandit selectedEnemy;

    public static event Action<MyBandit> enemySelected;
    public static event Action startNextStep;
    public static bool isPlayerStep;
    private readonly float yPosModifer = 15.0f;

    // Use this for initialization
    void Start()
    {
        enemySelected += SetAsEnemy;
        start_X_Pos = transform.position.x;
        ClearColor();
    }
    
    // Update is called once per frame
    void Update()
    {
        SetCurrentAnimState();
        SetMovementModifer();
        MoveUnit();
    }

    #region Character Actions

    public void Attacking()
    {
        StartCoroutine(WaitAfterAttack());
    }

    private IEnumerator WaitAfterAttack(float time = 2)
    {
        yield return new WaitForSeconds(time);
        
        movingDirectionModifer = MovingDirection.MoveBack;
        gameObject.SetActive(!HideCorps());
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
        selectedEnemy.GetDamage(unitPower);
    }

    public void StartAttack(object currentEnemy)
    {
        ClearColor();
        isPlayerStep = false;
        selectedEnemy = currentEnemy as MyBandit;
        selectedEnemy.movingDirectionModifer = MovingDirection.MoveForward;
        selectedEnemy.ClearColor();
        this.movingDirectionModifer = MovingDirection.MoveForward;
        isEnemy = false;
        isAttacking = true;
    }

    #endregion

    #region Select Actions

    private void OnMouseDown()
    {
        if (isPlayerStep && gameObject.CompareTag(CharacterTags.Enemy))
        {
            enemySelected?.Invoke(gameObject.GetComponent<MyBandit>());
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

    private void SetAsEnemy(MyBandit myBandit)
    {
        if (myBandit.Equals(this))
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

    public void ClearColor()
    {
        playerMarkerSpriteRenderer.color = MyColors.MyWhite;
    }

    private void SetBlueColor()
    {
        playerMarkerSpriteRenderer.color = MyColors.MyBlue;
    }

    #endregion

    #region Movment Metods

    private void MoveUnit()
    {
        transform.Translate((int)movingDirectionModifer * GetUnitTransformVector());
    }

    private void SetMovementModifer()
    {
        if (!UnitOutOfFieldBounds()) return;

        if (isAttacking)
        {
            StartCoroutine(WaitBeforeAttack());
        }
        movingDirectionModifer = MovingDirection.NotMove;
    }

    private bool UnitOutOfFieldBounds()
    {
        var currentXPos = Math.Abs(transform.position.x);
        var outOfMinXPosWhenMoveForward = currentXPos < Math.Abs(min_X_Pos) && movingDirectionModifer == MovingDirection.MoveForward;
        var outOfStartXPosWhenMoveBack = currentXPos > Math.Abs(start_X_Pos) && movingDirectionModifer == MovingDirection.MoveBack;

        return outOfStartXPosWhenMoveBack || outOfMinXPosWhenMoveForward;
    }

    #endregion

    #region Other Methods

    private bool HideCorps()
    {
        return isDead && movingDirectionModifer == MovingDirection.MoveBack;
    }

    private Vector2 GetUnitTransformVector()
    {
        var yComponent = -Time.deltaTime * yPosModifer / (Math.Abs(start_X_Pos) - Math.Abs(min_X_Pos));
        
        return new Vector2(m_speed * Time.deltaTime, yComponent);
    }

    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    private void OnDestroy()
    {
        startNextStep = null;
        enemySelected = null;
        Debug.Log("Object Destroyed!");
    }

    private void SetCurrentAnimState()
    {
        var currentState = movingDirectionModifer != MovingDirection.NotMove
            ? MyAnimationStates.MovingState 
            : MyAnimationStates.IdleState;
        m_animator.SetInteger(TriggersNames.AnimationState, currentState);
    }

    #endregion
}
