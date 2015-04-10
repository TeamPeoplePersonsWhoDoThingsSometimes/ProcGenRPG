using UnityEngine;
using System.Collections;

public class smokeGrenade : Attack {

    private float timePassed = 0f;
    private int spawned = 0;
    private Vector3 launchPos;
    private float cooldown = 0f;


    public float timeTillLaunch = 3f;
    public int bombsToSpawn = 15;
    public GameObject bomb;
    public float timeToDestroy = 25f;
    public float spawnTime = 0.3f;


	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Rigidbody>().AddForce((Player.playerPos.forward.normalized * 1200) + Player.playerPos.up.normalized * 400);
        gameObject.GetComponent<Rigidbody>().AddTorque(Player.playerPos.forward.normalized * 50);
	}
	
	// Update is called once per frame
	void Update () {
        timePassed += Time.deltaTime;
        cooldown += Time.deltaTime;
        if (timePassed > timeTillLaunch)
        {
            if (spawned < bombsToSpawn)
            {
                if (cooldown > spawnTime)
                {
                    if (launchPos == Vector3.zero)
                    {
                        launchPos = transform.position;
                    }
                    spawnBomb();
                    spawned++;
                    cooldown = 0;
                }
            }
        }
        if (timePassed > timeToDestroy)
        {
            Destroy(this.gameObject);
        }
	}

    private void spawnBomb()
    {
        if (bomb != null)
        {
            System.Random rand = new System.Random();
            Vector3 randPos = launchPos + new Vector3((float) rand.Next(-6, 6), 50f, (float) rand.Next(-6, 6));
            GameObject temp = (GameObject) GameObject.Instantiate(bomb, randPos, Quaternion.identity);
            temp.GetComponent<Attack>().SetDamage(thisDamage);
            temp.GetComponent<Attack>().SetCrit(critChance);
        }
    }
}
