using UnityEngine;
using Zenject;

public class UnitFactory
{
    private readonly DiContainer _container;

    [Inject]
    public UnitFactory(DiContainer container)
    {
        _container = container;
    }

    public UnitView Create(UnitData data, Vector3 position, Quaternion rotation)
    {
        var prefab = data.ShapeConfig.SHAPE_PREFAB;
        
        var instance = _container.InstantiatePrefabForComponent<UnitView>( prefab, position, rotation, null);
        instance.Initialize(data);
        
        return instance;
    }
}