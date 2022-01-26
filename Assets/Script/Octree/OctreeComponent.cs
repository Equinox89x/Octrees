using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using OctreeLib = OctreeLibraryFunctions<int>;


public class OctreeComponent : MonoBehaviour
{
    #region serialised fields
    [Header("Setup")]
    [SerializeField]
    private float size = 50;
    [SerializeField]
    private bool overrideSize;
    [SerializeField]
    private int nrOfDivisions = 2;
    [SerializeField]
    private GameObject gameObject;
    [SerializeField]
    private bool startFromObjectYAxis;
    [SerializeField]
    private ShapeType shapeType;
    
    [Header("Debug")]
    [SerializeField]
    private bool CanDrawDebug;
    [SerializeField]
    private bool CanDrawBuildingAreas;
    #endregion

    #region props
    private List<List<OctreeNode<int>>> Areas;    //debug
    public List<AreaObjects<int>> AreaObjects;   //game

    private Color minColor = new Color(1, 1, 1, 1f);
    private Color maxColor = new Color(0, 0.5f, 1, 0.25f);

    public Octree<int> Octree;
    public OctreeNode<int> RootNode;
    public GameObject Player;
    #endregion

    // Use this for initialization
    void Start()
    {
        #region make the octree
        Octree = OctreeLib.MakeTree(gameObject, this.gameObject, overrideSize, size, startFromObjectYAxis, nrOfDivisions, shapeType);
        if (Octree == null) return;
        #endregion

        #region make the areas
        AreaObjects = new List<AreaObjects<int>>();
        RootNode = Octree.GetRoot();
        if (RootNode == null) return;
        CreateAreas(RootNode);
        #endregion
    }

    /// <summary>
    ///creating the areas based on the items within the "static items" gameobject 
    /// </summary>
    private void CreateAreas(OctreeNode<int> rootNode)
    {
        GameObject DetectableAreas = GameObject.Find("StaticItems");
        if (!DetectableAreas || DetectableAreas.transform.childCount == 0) return;
        for (int i = 0; i < DetectableAreas.transform.childCount; i++) //loop through all the areas in the static items
        {
            List<Building<int>> BuildingAreaObjects = new List<Building<int>>();

            Transform buildingsGroup = DetectableAreas.transform.GetChild(i);
            if (!buildingsGroup || buildingsGroup.transform.childCount == 0) return;
            for (int i2 = 0; i2 < buildingsGroup.transform.childCount; i2++) //loop through all the objects in an area
            {
                Transform child = buildingsGroup.transform.GetChild(i2);
                if (!child) return;
                foreach (var subnode in rootNode.SubNodes) // Get all nodes with an object of this area
                {
                    OctreeNode<int> nodeAtPosition = OctreeLib.GetNodeAtPosition(subnode, child.position);
                    if (nodeAtPosition != null)
                    {
                        BuildingType building = (BuildingType)Enum.Parse(typeof(BuildingType), child.tag);

                        List<KeyValuePair<string, dynamic>> obj = new List<KeyValuePair<string, dynamic>>();
                        nodeAtPosition.Blackboard = obj;
                        nodeAtPosition.SetData("Node", nodeAtPosition);
                        nodeAtPosition.SetData("BuildingType", building);
                        nodeAtPosition.SetData("Building", child.gameObject);
                        nodeAtPosition.SetData("AreaName", buildingsGroup.name);

                        BuildingAreaObjects.Add(new Building<int>(child.name, i2, nodeAtPosition, building, child.gameObject));

                    }
                }
            }
            AreaObjects.Add(new AreaObjects<int>(buildingsGroup.name, i, true, BuildingAreaObjects));
        }
    }

    #region debug
    void OnDrawGizmos()
    {
        #region setup
        Octree = OctreeLib.MakeTree(gameObject, this.gameObject, overrideSize, size, startFromObjectYAxis, nrOfDivisions > 3 ? 3 : nrOfDivisions, shapeType);
        if (Octree == null) return;

        RootNode = Octree.GetRoot();
        if (RootNode == null) return;
        #endregion

        if (CanDrawDebug)
        {
            DrawNode(Octree.GetRoot());
        }

        if (CanDrawBuildingAreas)
        {
            Areas = new List<List<OctreeNode<int>>>();
            List<Color> colors = new List<Color>() { new Color(1, 0, 0, 1f), new Color(0, 1, 0, 1f), new Color(0, 0, 1, 1f), new Color(1, 1, 0, 1), new Color(1, 0, 1, 1) };

            GameObject DetectableAreas = GameObject.Find("StaticItems");
            if (!DetectableAreas || DetectableAreas.transform.childCount == 0) return;
            for (int i = 0; i < DetectableAreas.transform.childCount; i++) //loop through all the areas in the static items
            {
                List<OctreeNode<int>> BuildingAreas = new List<OctreeNode<int>>();

                Transform buildingsGroup = DetectableAreas.transform.GetChild(i);
                if (!buildingsGroup || buildingsGroup.transform.childCount == 0) return;
                for (int i2 = 0; i2 < buildingsGroup.transform.childCount; i2++) //loop through all the objects in an area
                {
                    Transform child = buildingsGroup.transform.GetChild(i2);
                    if (!child) return;
                    foreach (var subnode in RootNode.SubNodes) // Get all nodes with an object of this area
                    {
                        OctreeNode<int> nodeAtPosition = OctreeLib.GetNodeAtPosition(subnode, child.position);
                        if (nodeAtPosition != null)
                        {
                            BuildingAreas.Add(nodeAtPosition);
                        }
                    }
                }
                Areas.Add(BuildingAreas);
            }

            for (int i = 0; i < Areas.Count; i++)
            {
                foreach (var node in Areas[i])
                {
                    Gizmos.color = colors[i];
                    OctreeLib.DrawCorrectWireType(node);
                }
            }
        }
    }

    private void DrawNode(OctreeNode<int> node, int nodeDepth = 0)
    {
        if (!node.IsLeaf())
        {
            foreach (var subnode in node.SubNodes)
            {
                DrawNode(subnode, nodeDepth + 1);
            }
        }

        Gizmos.color = Color.Lerp(minColor, maxColor, nodeDepth / (float)nrOfDivisions);
        OctreeLib.DrawCorrectWireType(node);

        //Cube nodePosition = new Cube(node.Position, node.Size, node.Size, node.Size);
        Bounds nodePosition = new Bounds(node.Position, new Vector3(node.Size, node.Size, node.Size));
        Gizmos.color = new Color(1, 0, 1, 1f);
        Vector3 bottomLeft = nodePosition.center - (nodePosition.size / 2);
        Gizmos.DrawSphere(bottomLeft, 1);
        Gizmos.color = new Color(0, 0, 1, 1f);
        Gizmos.DrawSphere(nodePosition.center, 0.3f);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Octree == null) return;
        
        Player = GameObject.Find("Player");
        if (!Player) return;

        RootNode = Octree.GetRoot();
        if (RootNode == null) return;

        //OctreeNode<int> node = OctreeLibraryFunctions.GetNodeAtPosition(rootNode, player.transform.position);
        //AreaObjects<int> obj = AreaObjects.Find(x => x.Buildings.Where(p => p.Node.Position == node.Position).Equals(true));

        //if (node != null)
        //{
        //var areaName = node.GetData("AreaName");
        //print(areaName);
        //print(obj.AreaName);
        //}
    }

}