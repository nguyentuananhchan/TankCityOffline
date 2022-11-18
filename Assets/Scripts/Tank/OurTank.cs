using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class OurTank : Tank
{
    public GameObject shield;
    private string m_VerticalAxisName;
    private string m_HorizontalAxisName;
    private string m_FireButton;                // The input axis that is used for launching shells.
    private Animator shieldAnimator;
    private Vector3 position0;      //when tank collides other tank, it restores to the original position
    private float chargeRate;
    string mapName;
    private bool hasPrize;
    [SerializeField] GameObject prizeObj;
    public int level
    {
        set
        {
            if (value > 3)
                value = 3;
            float[] chargeRates = { 1f, 0.9f, 0.8f, 0.7f};
            int[] healths = { 1, 1, 1, 1};
            m_level = value;
            chargeRate = chargeRates[value];
            Health = healths[value];
            animator.SetInteger("level", value);
            animator.SetInteger("health", Health);
        }
        get
        {
            return m_level;
        }
    }
    private int m_level;

    public void Born(bool prize)
    {
        print("Born in our tank");
        Vector2[] ourSpawnPoint = { new Vector2(-1f, -3f), new Vector2(1f, -3f) };
        
        var bm = BattleManager.GetInstance();
        Debug.Log("m_PlayerNumber:" + m_PlayerNumber);
        hasPrize = prize;
        
        if (prize) prizeObj.SetActive(true);
        else prizeObj.SetActive(false);

        transform.position = bm.playerSpawnPos[m_PlayerNumber - 1].position;
        m_Dead = false;
        moveDirection = Vector2.up;
        gameObject.SetActive(true);
        //level = gameManager.playerLevel[m_PlayerNumber - 1];
        LevelTransform(level);
        speed = 0.7f;
        invincibleTime = 3f;
        shieldAnimator.SetBool("invincible", true); 
    }
    private void LevelTransform(int lv) {
        Debug.Log("hit player, lv:" + lv);
        level = lv;
        m_level = lv;
        animator.SetInteger("level", lv);
        animator.Play("Player" + m_PlayerNumber + "Moving" + lv);
    }
    // Start is called before the first frame update
    void Start()
    {
        side = Side.Player;
        if (gameObject.name == "player1")
            m_PlayerNumber = 1;
        else
            m_PlayerNumber = 2;
        m_VerticalAxisName = "Vertical" + m_PlayerNumber;
        m_HorizontalAxisName = "Horizontal" + m_PlayerNumber;
        m_FireButton = "Fire" + m_PlayerNumber;
        shieldAnimator = shield.GetComponent<Animator>();
        level = 3;
    }


    // Update is called once per frame
    void Update()
    {
        if (m_Dead)
            return;
        float distance;

        if (invincibleTime > 0)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime<=0)
            {
                shieldAnimator.SetBool("invincible", false);
            }
        }
        //if (Input.GetKey(KeyCode.Mouse0))
        //if (Input.GetButtonDown(m_FireButton))
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (m_CurrentChargeTime <= 0)
            {
                print("Level " + level);

                Fire(level < 3 ? 1 : 2);

                RaycastHit2D hit = Physics2D.Raycast(fireTransform.position, moveDirection, 7f, LayerMask.GetMask("Tank"));
               distance = hit.distance;
                m_CurrentChargeTime = Mathf.Lerp(0.9f, 1.5f, distance / 7) * chargeRate;

               // Fire(1);
               // m_CurrentChargeTime = 0;
                //Debug.Log("Charge time: " + m_CurrentChargeTime);
            }
        }
        m_CurrentChargeTime -= Time.deltaTime;

        prizeObj.transform.position = this.transform.position;
    }

    private void FixedUpdate()
    {
        if (m_Dead)
            return;
        // Store the player's input and make sure the audio for the engine is playing.
        float vertical = Input.GetAxisRaw(m_VerticalAxisName);
        float horizontal = Input.GetAxis(m_HorizontalAxisName);
        //print("vertical " + vertical + "； horizontal " + horizontal);
        // Adjust the position of the tank based on the player's input.
        Vector2 position = rigidbody2d.position;

        if (Mathf.Approximately(horizontal, 0.0f) && Mathf.Approximately(vertical, 0.0f))
        {
            animator.speed = 0;
        }
        else
        {
            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                moveDirection.Set(horizontal, 0);
                if (horizontal < 0)
                {
                    rigidbody2d.rotation = 90f;
                    moveDirection = Vector2.left;
                }
                else
                {
                    rigidbody2d.rotation = -90f;
                    moveDirection = Vector2.right;
                }
                position.y = Mathf.RoundToInt(position.y / smallestGrid) * smallestGrid;

            }
            else
            {
                moveDirection.Set(0, vertical);
                if (vertical > 0)
                {
                    rigidbody2d.rotation = 0f;
                    moveDirection = Vector2.up;
                }

                else
                {
                    rigidbody2d.rotation = 180f;
                    moveDirection = Vector2.down;
                }
                position.x = Mathf.RoundToInt(position.x / smallestGrid) * smallestGrid;
            }
            animator.speed = (moveDirection * speed).magnitude;

            position0 = position;
            position += moveDirection * speed * Time.fixedDeltaTime;
            rigidbody2d.MovePosition(position);
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.name == "EnemyTank")
        {
            rigidbody2d.velocity = new Vector2(0,0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Shell" && invincibleTime <= 0)
        {
            Shell shell = collision.GetComponent<Shell>();
            if (shell.shooter < 0)
            {
                if (level > 0)
                {
                    level--;
                    LevelTransform(level);
                }
                if (level <= 0)
                {
                    Health--;

                }
                if (hasPrize)
                {
                    hasPrize = false;
                    GameObject prizeInstance = ObjectPool.GetInstance().GetObject("Prize");
                    Prize prize = prizeInstance.GetComponent<Prize>();
                    prizeObj.SetActive(false);
                }
                // If the current health is at or below zero and it has not yet been registered, call OnDeath.
                if (Health <= 0 && !m_Dead)
                {
                    StartCoroutine(OnDeath());
                }

            }
        }
    }

    private IEnumerator OnDeath()
    {
        // Set the flag so that this function is only called once.
        m_Dead = true;
        yield return new WaitForSeconds(7f / 10f);

        gameObject.SetActive(false);
        BattleManager.GetInstance().OurTankDie(m_PlayerNumber);

    }

    public void OnInvinciblePrize()
    {
        invincibleTime = 6f;
        shieldAnimator.SetBool("invincible", true);
    }

}


