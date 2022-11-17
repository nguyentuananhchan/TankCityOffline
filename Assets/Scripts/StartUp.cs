using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartUp : MonoBehaviour
{
    private Toggle onePlayer, twoPlayers, construction;
    GameManager gm;


    // Start is called before the first frame update
    void Start()
    {
        onePlayer = GameObject.Find("Toggle1Player").GetComponent<Toggle>();
        twoPlayers = GameObject.Find("Toggle2Players").GetComponent<Toggle>();
        construction = GameObject.Find("ToggleConstruction").GetComponent<Toggle>();
        onePlayer.Select();
        gm = GameManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {


    }
}
