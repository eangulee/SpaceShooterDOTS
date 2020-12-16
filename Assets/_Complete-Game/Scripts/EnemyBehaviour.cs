using Unity.Entities;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{

	public float speed = 2f;
	public float enemyHealth = 1f;


	void Start()
	{
		//rigidBody = GetComponent<Rigidbody>();
	}

	public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
	{
        manager.AddComponent(entity, typeof(EnemyTag));
        manager.AddComponent(entity, typeof(MoveForward));

        MoveSpeed moveSpeed = new MoveSpeed { Value = speed };
        manager.AddComponentData(entity, moveSpeed);

        Health health = new Health { Value = enemyHealth };
		manager.AddComponentData(entity, health);
    }
}
