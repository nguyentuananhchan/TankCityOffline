using UnityEngine;


public class Prize : MonoBehaviour
{
    public enum Type {star, life, helmet};
    public Type type;
    public Sprite[] sprites = new Sprite[3];

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        //type = Type.star;// (Type)Random.Range(0, 6);
        type = (Type)Random.Range(0, 4); ;// 
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[(int)type];
        gameObject.transform.position = new Vector2(Random.Range(-13, 13) * 0.25f, Random.Range(-11, 11) * 0.25f);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag =="Player")
        {
            OurTank myTank = collision.GetComponent<OurTank>();
            if(type == Type.life)
            {
                GameManager.GetInstance().playerLife++;
            }
            else if(type == Type.helmet)
            {
                myTank.OnInvinciblePrize();
            }
            else if(type == Type.star)
            {
                myTank.level++;
            }


            ObjectPool.GetInstance().RecycleObj(gameObject);
        }
        else if (collision.name == "EnemyTank" )
        {
            EnemyTank enemy = collision.GetComponent<EnemyTank>();
            if (type == Type.life)
            {
                GameManager.GetInstance().playerLife++;
            }
            else if (type == Type.helmet)
            {
                enemy.OnInvinciblePrize();
            }
            else if (type == Type.star)
            {
                enemy.Type += 1 ;
                enemy.UpdateType(enemy.Type);
            }

            ObjectPool.GetInstance().RecycleObj(gameObject);
        }
    }

}
