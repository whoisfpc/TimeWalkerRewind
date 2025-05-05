using UnityEngine;
using System.Collections;

public class MyFallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb2d;

    public float fallDelay;
    public float lifeTime = 3f;
    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if (GetComponent<Collider2D>().isTrigger)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);

        rb2d.isKinematic = false;
        GetComponent<Collider2D>().isTrigger = true;
        yield return 0;
    }
}
