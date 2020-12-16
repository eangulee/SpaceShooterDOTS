using Unity.Entities;
using UnityEngine;

public class Done_Mover : MonoBehaviour, IConvertGameObjectToEntity
{
	public float speed;
    public float lifeTime;

    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        manager.AddComponent(entity, typeof(MoveForward));
        //manager.AddComponent(entity, typeof(EnemyTag));

        MoveSpeed moveSpeed = new MoveSpeed { Value = speed };
        manager.AddComponentData(entity, moveSpeed);

        TimeToLive timeToLive = new TimeToLive { Value = lifeTime };
        manager.AddComponentData(entity, timeToLive);

    }

    void Start ()
	{
		//GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
}
