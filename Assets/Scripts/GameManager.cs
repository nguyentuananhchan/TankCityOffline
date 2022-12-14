using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public int player;
    public int stage;
    public string currentMapName;
    public Tilemap currentMap;
    [HideInInspector] public int[,] kill = new int[2, 4];
    [HideInInspector] public int playerLife = 1;
    [HideInInspector] public int[] playerLevel = { 1, 1 };
    public enum BattleResult {WIN, LOSE};
    [HideInInspector] public BattleResult battleResult;

    int MaxStage = 4;
    private static GameManager instance;

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<GameManager>();
        }
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (this != instance)
        {
            Debug.LogError("singleton:" + this.ToString() + " exists, remove it");
            Destroy(this);
        }
       // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerLevel[0] = playerLevel[1] = 1;
        print("player level " + playerLevel[0] + " " + playerLevel[1]);
    }

    public void NextStage()
    {
        stage++;
        if (stage > MaxStage)
            stage = 1;
    }
}
