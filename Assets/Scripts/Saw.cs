using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class Saw : MonoBehaviour
{
    public Touch touch;
    public Material mat;
    public LayerMask mask;
    public CameraShake cameraShake;
    public GameObject effect;
    public GameObject[] wood;
    public float rotationSpeed;
    public AudioSource hit;
    private bool isCutting=false;
    int i = 0;
   
    // Start is called before the first frame update
    void Start()
    {
        hit = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isCutting == false)
        transform.Rotate(new Vector3(0f, 1500f * Time.deltaTime, 0f));
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x + 5f * Time.deltaTime, -1, 1),
                    transform.position.y, transform.position.z);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x - 5f * Time.deltaTime, -1, 1),
                    transform.position.y, transform.position.z);
            }

            int x = 1;
        if (Time.time > i)
        {
            i += 2;
            int random = Random.Range(0, 5); // to spawn random woods
            int random2 = Random.Range(1,3); // to spawn woods at random locations
            Instantiate(wood[random], new Vector3(-1.5f+random2, 7f, 10f), Quaternion.Euler(0, 0, 90));
        }
        Collider[] cuttingObjects = Physics.OverlapBox(transform.position,new Vector3(0.1f,0.1f,0f),transform.rotation,mask);
        foreach(Collider nesne in cuttingObjects) // cutting
        {
            transform.Rotate(new Vector3(0f, 500f * Time.deltaTime, 0f));
            isCutting = true;
            StartCoroutine(isCuttingWood());        
            
            hit.Play();

            x++;
            Debug.Log(x);
            SlicedHull cuttingObject = Cut(nesne.gameObject, mat);
            GameObject kesilmisUst = cuttingObject.CreateUpperHull(nesne.gameObject, mat);
            GameObject kesilmisAlt = cuttingObject.CreateLowerHull(nesne.gameObject, mat);
            Instantiate(effect, new Vector3(transform.position.x, transform.position.y - 0.4f,transform.position.z+ 3f), Quaternion.Euler(-40f, 180f, 0f));
            CameraShake.Shake(0.4f, 0.3f);
            
            AddBody(kesilmisUst);
            AddBody(kesilmisAlt);
            
            Destroy(nesne.gameObject);
            
        }
        
    }
    public SlicedHull Cut(GameObject obj, Material mat = null)
    {
        return obj.Slice(transform.position, transform.up, mat);
    }

    void AddBody(GameObject obj)
    {
        obj.AddComponent<MeshCollider>().convex = true;
        obj.AddComponent<Rigidbody>();
        obj.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        obj.GetComponent<Rigidbody>().AddExplosionForce(350, obj.transform.position, 15);
        obj.GetComponent<Rigidbody>().velocity = obj.GetComponent<Rigidbody>().velocity / 2;
        StartCoroutine(DestroyAfterwards(obj, 2));
    }
    IEnumerator DestroyAfterwards(GameObject obj,float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(obj);
        
    }

    IEnumerator isCuttingWood()
    {
        
        yield return new WaitForSeconds(0.1f);
        isCutting = !isCutting;
    }
}
