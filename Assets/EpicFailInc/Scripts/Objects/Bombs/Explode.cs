using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {
    public bool isLit = true;
    public int timeToExplodeInSeconds = 3;

    private float time;
    private GameObject explosion;

    //Explosion scores
    public int DestructableWallPoints = 10;

    //Private
    private GameManager _gameManager;
    private bool _hasManager = false;


	// Use this for initialization
	void Start ()
	{
	    explosion = transform.GetChild(0).gameObject;
	    GameObject manager = GameObject.FindGameObjectWithTag("Manager");
	    if (manager != null)
	    {
	        _gameManager = (GameManager) manager.GetComponent("GameManager");
	        _hasManager = true;
	    }
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (isLit)
        {
            time += Time.deltaTime;
            if (time >= timeToExplodeInSeconds)
            {
                isLit = false;
                transform.GetChild(1).gameObject.active = false;
                executeCollision();
                StartCoroutine(explode());
                transform.localScale = new Vector3(0,0,0);
            }
        }
	}

    IEnumerator explode()
    {

        yield return new WaitForSeconds(1);
        Destroy(transform.gameObject);
    }

    void executeCollision()
    {
        explosion.active = true;
        const int explosionPower = 300;
        const int explosionRadius = 2;

        var colliders = Physics.OverlapSphere( transform.position, explosionRadius );
        foreach (var hit in colliders)
        {
            if (hit.rigidbody && hit.rigidbody != transform.rigidbody)
            {
                switch(hit.gameObject.tag)
                {
                    case "DestructableWall":
                        var wall = (DestructableWalls)hit.gameObject.GetComponent(typeof(DestructableWalls));
                        if (wall != null)
                        {
                            wall.Destruct();
                        }
                        if (_hasManager) _gameManager.Score += DestructableWallPoints;
                        break;
                    default:
                        hit.rigidbody.AddExplosionForce(explosionPower, transform.position, explosionRadius);
                        break;
                }
            }
        }
    }
}

