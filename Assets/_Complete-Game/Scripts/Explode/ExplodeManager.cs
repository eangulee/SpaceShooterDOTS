using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class ExplodeManager : MonoBehaviour
{
    EntityManager _manager;
    GameObjectConversionSettings _settings;
    BlobAssetStore _blobAssetStore;
    public GameObject cube;
    public int amount = 100;
    public float time = 5f;
    public float cellSize = 0.01f;
    public Vector3 pos;
    public Vector3 velocity;
    private Entity cubeEntity;
    void Start()
    {
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _blobAssetStore = new BlobAssetStore();
        _settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
    }

    public void Explode(GameObject target)
    {
        //简单的校验，但很关键
        if (amount > 5000)
        {
            Debug.LogError("To Many Piece !!! limit is 5000");
            return;
        }
        //...省略1 初始化各种参数...
        //var targetECS = GameObjectConversionUtility.ConvertGameObjectHierarchy(target, _settings);
        //创建数组
        var old = target.transform.localScale;
        target.transform.localScale = old * cellSize;
        cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(target, _settings);
        target.transform.localScale = old;
        NativeArray<Entity> many = new NativeArray<Entity>(amount, Allocator.Temp);
        _manager.Instantiate(cubeEntity, many);

        for (int i = 0; i < amount; i++)
        {
            Entity entity = many[i];
            _manager.SetComponentData(entity, new Translation { Value = pos });
            _manager.AddComponent<PhysicsVelocity>(entity);
            _manager.SetComponentData(entity, new PhysicsVelocity { Linear = velocity });
            _manager.AddComponent<LifeTimeToDestroyData>(entity);
            _manager.SetComponentData(entity, new LifeTimeToDestroyData { restTime = time });
        }

        //...省略2 给每个实体配置位置和速度...
        _manager.DestroyEntity(cubeEntity);
        //销毁数组
        many.Dispose();
    }

    private void OnDestroy()
    {
        _blobAssetStore.Dispose();
    }

    #region Event Handler
    public void OnExplodeBtnClickHandler()
    {
        Explode(this.cube);
    }
    #endregion
}
