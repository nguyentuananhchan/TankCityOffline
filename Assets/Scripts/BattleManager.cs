using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class BattleManager : MonoBehaviour
{
    public Transform[] enemySpawnPos;
    public Transform[] playerSpawnPos;
    [HideInInspector] public int liveEnemy;
    [HideInInspector] public int livePlayer;
    [HideInInspector] public float bulletTime;
    OurTank[] ourTank = new OurTank[2];
    [SerializeField] TextMeshProUGUI resultGame;
    const int totalEnemy = 20;
    int[] enemyTanks = { 5, 5, 5, 5 };
    int[] enemyQueue = new int[totalEnemy];

    int prizePerBattle = 3;
    bool[] prizeQueue = new bool[totalEnemy];
    [HideInInspector] int enemyBorn;
    GameManager gm;
    [HideInInspector] public enum BattleState { Null,Running,End}
    [HideInInspector] public BattleState battleState;
    private static BattleManager instance;

    public static BattleManager GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<BattleManager>();
        }
        return instance;
    }

    private void Awake()
    {
        gm = GameManager.GetInstance();
        ObjectPool.GetInstance().Clear();
        ourTank[0] = GameObject.Find("player1").GetComponent<OurTank>();
    }

    // Start is called before the first frame update
    void Start()
    {
        print("New battle");
        battleState = BattleState.Running;
        resultGame.text = "";
        enemyBorn = 0;
        FormEnemyQueue();
        SpawnEnemyTank();
        for (int i = 0; i < GameManager.GetInstance().player; i++)
        {
            if (gm.playerLife > 0)
                ourTank[i].Born(true);
            else
                ourTank[i].gameObject.SetActive(false);

        }
        System.Array.Clear(gm.kill, 0, gm.kill.Length);
    }

    private void Update()
    {
        bulletTime -= Time.deltaTime;
    }

    public IEnumerator Loss()
    {
        battleState = BattleState.End;
        print("Game over!");
        resultGame.text = "Thua";
        ourTank[0].m_Dead = true;
        gm.battleResult = GameManager.BattleResult.LOSE;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("ScoreScene");
    }
    public IEnumerator Win()
    {
        battleState = BattleState.End;
        print("Win!");
        resultGame.text = "Thắng";
        ourTank[0].m_Dead = true;
        gm.battleResult = GameManager.BattleResult.LOSE;
        yield return new WaitForSeconds(1.5f);
    }
    public void SpawnEnemyTank()
    {
        while (liveEnemy < 4 && enemyBorn < totalEnemy)
        {
            //print("Tank No " + enemyBorn + " type " + enemyQueue[enemyBorn] + " born at " + enemyBorn % 3);
            GameObject tankInstance = ObjectPool.GetInstance().GetObject("EnemyTank");
            EnemyTank tank = tankInstance.GetComponent<EnemyTank>();
            StartCoroutine(tank.Born(enemyQueue[enemyBorn], enemyBorn, prizeQueue[enemyBorn]));
            liveEnemy++;
            enemyBorn++;
        }

        if (liveEnemy == 0 && enemyBorn == totalEnemy)
        {
            gm.battleResult = GameManager.BattleResult.WIN;
        }
    }


    void FormEnemyQueue()
    {
        int cursor = 0;
        for (int i = 0; i < enemyTanks.Length; i++)
        {
            for (int j = 0; j < enemyTanks[i]; j++)
                enemyQueue[cursor++] = i;
        }
        RandomElement.Shuffle(enemyQueue);

        for (int i = 0; i < prizePerBattle; i++)
            prizeQueue[i] = true;
        for (int i = prizePerBattle; i < totalEnemy; i++)
            prizeQueue[i] = false;
        RandomElement.Shuffle(prizeQueue);
    }


    public void OurTankDie(int player)
    {
        gm.playerLife--;
        if (gm.playerLife == 0 )
        {
            StartCoroutine(Loss());
        }
        else if (gm.playerLife > 0)
        {
            ourTank[player - 1].level = 0;
            ourTank[player - 1].Born(true);
            
        }
    }

}
