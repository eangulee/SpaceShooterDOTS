using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[AlwaysSynchronizeSystem]
public class LifeTimeToDestroySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities.WithoutBurst().ForEach((Entity entity, ref LifeTimeToDestroyData life) =>
        {
            life.restTime -= Time.DeltaTime;
            if (life.restTime < 0)
            {
                ecb.DestroyEntity(entity);
            }
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
        return default;
    }
}