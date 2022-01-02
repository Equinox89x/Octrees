using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///<summary>
///Data structure that holds data and a blackboard for each node in the world octree
///</summary>
public struct WorldData<TType>
{
    public WorldData(OctreeNode<TType> node = null, BuildingType building = BuildingType.None, GameObject prefabBuilding = null, bool isClaimed = false, bool hasPointOfInterest = true, int nodeIndex = -1)
    {
        Node = node;
        NodeIndex = nodeIndex;
        Building = building;
        PrefabBuilding = prefabBuilding;
        IsClaimed = isClaimed;
        HasPointOfInterest = hasPointOfInterest;
    }

    private OctreeNode<TType> Node;
    private int NodeIndex;
    private BuildingType Building;
    private GameObject PrefabBuilding;
    private bool IsClaimed;
    private bool HasPointOfInterest;
}

#region an area with it's data and claim status
///<summary>
///Data struct that holds the information of an area of the world
///</summary>
public struct AreaObjects<TType>
{
    public AreaObjects(string name, int id, bool isClaimed = false, List<Building<TType>> buildings = null)
    {
        _areaName = name;
        _areaId = id;
        _isClaimed = isClaimed;
        _buildings = buildings;
    }

    #region props
    private string _areaName;
    public string AreaName
    {
        get { return _areaName; }
        set { _areaName = value; }
    }

    private int _areaId;
    public int AreaId
    {
        get { return _areaId; }
        set { _areaId = value; }
    }

    private List<Building<TType>> _buildings;
    public List<Building<TType>> Buildings
    {
        get { return _buildings; }
        set { _buildings = value; }
    }

    private bool _isClaimed;
    public bool IsClaimed
    {
        get { return _isClaimed; }
        set { _isClaimed = value; }
    }
    #endregion

    //public Building<TType> GetBuildingById(int id)
    //{
    //    return Buildings.Find(x => x.BuildingId == id);
    //}

    //public Building<TType> GetBuildingAtLocation(OctreeNode<TType> rootNode, Vector3 pos)
    //{
    //    OctreeNode<TType> node = OctreeLibraryFunctions.GetNodeAtPosition(rootNode, pos);
    //    Bounds bounds = new Bounds(node.Position, new Vector3(node.Size, node.Size, node.Size));
    //    return Buildings.Find(x => bounds.Intersects(new Bounds(x.Node.Position, new Vector3(x.Node.Size, x.Node.Size, x.Node.Size))));
            
    //}
    //public Building<TType> GetBuildingAtLocation2(OctreeNode<TType> rootNode, Vector3 pos)
    //{
    //    OctreeNode<TType> node = OctreeLibraryFunctions.GetNodeAtPosition(rootNode, pos);
    //    return Buildings.Find(x => x.Node == node);
    //}
    //public bool IsInBuildingNodeAtPos(OctreeNode<TType> rootNode, Vector3 pos)
    //{
    //    OctreeNode<TType> node = OctreeLibraryFunctions.GetNodeAtPosition(rootNode, pos);
    //    return Buildings.Count(x => x.Node == node) >= 1;
    //}
}

///<summary>
///Data structure that holds the info of a building in an area
///</summary>
public struct Building<TType>
{
    public Building(string buildingName, int buildingId, OctreeNode<TType> node, BuildingType buildingType, GameObject prefabBuilding){
        BuildingName = buildingName;
        BuildingId = buildingId;
        Node = node;
        BuildingType = buildingType;
        PrefabBuilding = prefabBuilding;
    }

    public string BuildingName;
    public int BuildingId;
    public OctreeNode<TType> Node;
    public BuildingType BuildingType;
    public GameObject PrefabBuilding;
}

///<summary>
///Type of building
///</summary>
public enum BuildingType
{
    None,
    Base,
    House,
    Vehicle,
    Misc
}
#endregion

/// <summary>
/// Data structure that holds the player info
/// </summary>
/// <typeparam name="TType">Should be the TType the node gets (in this demo, int)</typeparam>
public class Player<TType>
{
    public Player(Vector3 position, OctreeNode<TType> node, GameObject gameObject)
    {
        _position = position;
        _currentNode = node;
        _gameObject = gameObject;
    }

    private Vector3 _position;
    public Vector3 Position
        {
        get { return _position; }
        set { _position = value; }
    }

    private OctreeNode<TType> _currentNode;

    public OctreeNode<TType> CurrentNode
    {
        get { return _currentNode; }
        set { _currentNode = value; }
    }

    private GameObject _gameObject;

    public GameObject GameObject
    {
        get { return _gameObject; }
        set { _gameObject = value; }
    }
}

public enum ShapeType
{
    Cube,
    Sphere,
    Quad
}