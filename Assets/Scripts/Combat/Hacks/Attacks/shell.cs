using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class shell : Attack {

    public float speed = 20f;
    public AudioClip explosion;

    private bool dying = false;

    void Start()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;
    }

	// Update is called once per frame
	void Update () {
        if (!dying)
        {
            transform.Translate(new Vector3(0, -1 * speed * Time.deltaTime, 0));

            if (transform.position.y <= 0)
            {
                createExplosion();
                gameObject.GetComponent<AudioSource>().PlayOneShot(explosion);

                Destroy(this.gameObject, 0.5f);
                dying = true;
                transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                gameObject.GetComponent<SphereCollider>().enabled = true;
            }
        }
	}

    private void createExplosion()
    {
        Debug.Log("EXPLODE!");
    }
}
