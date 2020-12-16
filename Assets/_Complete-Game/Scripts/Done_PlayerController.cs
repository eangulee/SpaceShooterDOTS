using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;


[System.Serializable]
public class Done_Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class Done_PlayerController : MonoBehaviour
{
    public float speed;
    public float tilt;
    public Done_Boundary boundary;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;

    private float nextFire;

    EntityManager _manager;
    GameObjectConversionSettings _settings;
    BlobAssetStore _blobAssetStore;
    Entity bulletEntityPrefab;

    public int spreadAmount;

    void Start()
    {
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _blobAssetStore = new BlobAssetStore();
        _settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(shot, _settings);
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            Vector3 rotation = shotSpawn.rotation.eulerAngles;
            rotation.x = 0f;

            //Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            SpawnBulletECS(rotation);
            GetComponent<AudioSource>().Play();
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        GetComponent<Rigidbody>().velocity = movement * speed;

        GetComponent<Rigidbody>().position = new Vector3
        (
            Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
        );

        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
    }

    void SpawnBulletECS(Vector3 rotation)
    {
        int max = spreadAmount / 2;
        int min = -max;
        int totalAmount = spreadAmount;

        Vector3 tempRot = rotation;
        int index = 0;

        NativeArray<Entity> bullets = new NativeArray<Entity>(totalAmount, Allocator.TempJob);
        _manager.Instantiate(bulletEntityPrefab, bullets);

        for (int y = min; y < max; y++)
        {
            tempRot.y = (rotation.y + 3 * y) % 360;

            _manager.SetComponentData(bullets[index], new Translation { Value = shotSpawn.position });
            _manager.SetComponentData(bullets[index], new Rotation { Value = Quaternion.Euler(tempRot) });

            index++;
        }

        bullets.Dispose();
    }
}
