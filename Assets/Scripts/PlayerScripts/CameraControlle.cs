using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlle : MonoBehaviour
{
    public Transform cible;
    public GameObject joueur;
    public float lissage;

    public Vector2 minPosition;
    public Vector2 maxPosition;

  

    // Start is called before the first frame update
    void Start()
    {
        joueur = GameObject.Find("Player");
        cible = joueur.GetComponent<Transform>();
    }
    
    private void LateUpdate() 
    {
         
    if (cible != null)
    {
         if (transform.position != cible.position)
        {
            Vector3 PositionCible = new Vector3(cible.position.x, cible.position.y, transform.position.z);

            PositionCible.x = Mathf.Clamp(PositionCible.x, minPosition.x, maxPosition.x);
            PositionCible.y = Mathf.Clamp(PositionCible.y, minPosition.y, maxPosition.y);

            transform.position = Vector3.Lerp(transform.position, PositionCible, lissage);
        }
    }
       
    }
}
