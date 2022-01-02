using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using OctreeLib = OctreeLibraryFunctions<int>;

public class OctreePlayer : MonoBehaviour
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
    private bool canDrawInnerCollisionNodes;
    [SerializeField]
    private bool canDrawOuterCollisionNodes;
    [SerializeField]
    private bool canDrawGameDebug;
    [SerializeField]
    private bool canDrawGizmo;
    #endregion

    #region props
    private List<OctreeNode<int>> InnerCollisionNodesDebug = new List<OctreeNode<int>>() { null, null, null, null, null, null, null, null };
    private List<OctreeNode<int>> InnerCollisionNodes = new List<OctreeNode<int>>() { null, null, null, null, null, null, null, null };
    private List<OctreeNode<int>> IntersectedWorldNodes = new List<OctreeNode<int>>();

    private Color minColor = new Color(1, 1, 1, 1f);
    private Color maxColor = new Color(0, 0.5f, 1, 0.25f);

    private Octree<int> Octree;
    private OctreeNode<int> RootNode;
    private OctreeComponent WorldOctree;
    private Player<int> PlayerObj;
    #endregion

    // Use this for initialization
    void Start()
    {
        #region make the octree
        Octree = OctreeLib.MakeTree(gameObject, this.gameObject, overrideSize, size, startFromObjectYAxis, nrOfDivisions > 3 ? 3 : nrOfDivisions, shapeType);
        if (Octree == null) return;
        #endregion

        #region make the innercollision node list
        RootNode = Octree.GetRoot();
        if (RootNode == null) return;

        FindInnerCollisionNodes(RootNode);
        #endregion

        #region Create the player
        OctreeNode<int> node = OctreeLib.GetNodeAtPosition(RootNode, this.transform.position);
        if (node == null) return;
        PlayerObj = new Player<int>(this.transform.position, node, this.gameObject);
        node.Players.Add(PlayerObj);
        #endregion
    }

    #region debug
    void OnDrawGizmos()
    {
        if (canDrawGizmo)
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

            if (canDrawInnerCollisionNodes)
            {

                for (int i = 0; i < RootNode.SubNodes.Length; i++)
                {
                    OctreeNode<int> nodeAtPosition = OctreeLib.GetNodeAtPosition(RootNode.SubNodes[i], this.transform.position);
                    if (nodeAtPosition != null)
                    {
                        InnerCollisionNodesDebug[i] = nodeAtPosition;
                    }
                }

                for (int i = 0; i < InnerCollisionNodesDebug.Count; i++)
                {
                    OctreeNode<int> node = InnerCollisionNodesDebug[i];
                    Gizmos.color = new Color(1, 0.5f, 0.5f, 1);
                    OctreeLib.DrawCorrectWireType(node);
                }
            }

            if (canDrawOuterCollisionNodes)
            {
                Gizmos.color = new Color(1, 0.2f, 0.7f, 1);
                OctreeLib.DrawCorrectWireType(RootNode);
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
        #region octree update
        Octree = OctreeLib.MakeTree(gameObject, this.gameObject, overrideSize, size, startFromObjectYAxis, nrOfDivisions > 3 ? 3 : nrOfDivisions, shapeType);
        #endregion

        #region get rootnode, world octree, world intersecting nodes, inner collision nodes
        //get the rootNode
        RootNode = Octree.GetRoot();
        if (RootNode == null) return;

        //get the octree of the world
        GameObject floor = GameObject.Find("Floor");
        if (floor == null) return;

        WorldOctree = floor.GetComponent<OctreeComponent>();
        if (WorldOctree == null) return;

        FindInnerCollisionNodes(RootNode);

        //get all world nodes the player is colliding with (can be used for spatial partitioning, check neighbours, ...)
        OctreeLib.FindIntersectingNodes(WorldOctree, InnerCollisionNodes, IntersectedWorldNodes, canDrawGameDebug);
        #endregion

        # region Update the Player list in the nodes
        if (this.transform.tag == "Friendly" || this.transform.tag == "Enemy")
        {
            //checking against the world nodes
            OctreeNode<int> node = OctreeLib.GetNodeAtPosition(WorldOctree.RootNode, this.transform.position);
            //if(node.Players == null || node.Players.FirstOrDefault(x => x.CurrentNode == node) == null) 
            //{             
            //}
            if(PlayerObj.CurrentNode != node)
            {
                if (!node.Players.Contains(PlayerObj))
                {
                    node.Players.Add(PlayerObj);
                }
                PlayerObj.CurrentNode.Players.Remove(PlayerObj);
                PlayerObj.CurrentNode = node;
            }
        }
        #endregion

        //Change post processing based on territory claim
        if (this.transform.tag == "Friendly")
        {
            AreaObjects<int> obj = WorldOctree.AreaObjects.Find(x => x.AreaName == (string)PlayerObj.CurrentNode.GetData("AreaName"));
            if (obj.AreaName != null)
            {
                if (obj.IsClaimed)
                {
                    GameObject.Find("PostProcessiongGO").GetComponent<PostProcessVolume>().enabled = true;
                }
                else
                {
                    GameObject.Find("PostProcessiongGO").GetComponent<PostProcessVolume>().enabled = false;
                }
            }
            else
            {
                GameObject.Find("PostProcessiongGO").GetComponent<PostProcessVolume>().enabled = false;
            }
        }

        //find in which area you're in
        //AreaObjects<int> obj2 = WorldOctree.AreaObjects.Find(x => x.IsInBuildingNodeAtPos(WorldOctree.RootNode, transform.position));
        //print("Obj2, Currently in: " + obj2.AreaName);
        OctreeNode<int> foundNode = OctreeLib.GetNodeAtPosition(WorldOctree.Octree.GetRoot(), transform.position);
        if (foundNode != null)
        {
            if (foundNode.Blackboard != null && foundNode.Blackboard.Count != 0)
            {
                var areaName = foundNode.GetData("AreaName");
                print("Currently in: " + areaName);
            }
        }

        //Get the amount of players in your surrounding nodes
        if (this.transform.tag == "Friendly")
        {
            List<Player<int>> players = new List<Player<int>>();
            foreach (OctreeNode<int> node in IntersectedWorldNodes)
            {
                for (int i = 0; i < node.Players.Count; i++)
                {
                    players.Add(node.Players[i]);
                }
            }
            print("Amount of nearby players: " + players.Count);
        }


        if (canDrawGameDebug)
        {
            // Get all own nodes the player object is colliding with
            foreach (var subnode in RootNode.SubNodes)
            {
                OctreeNode<int> nodeAtPosition = OctreeLib.GetNodeAtPosition(subnode, this.transform.position);
                if (nodeAtPosition != null)
                {
                    OctreeLib.DrawBounds(nodeAtPosition, new Color(1, 0.5f, 0.5f, 1));
                }
            }
        }
    }

    private void FindInnerCollisionNodes(OctreeNode<int> rootNode)
    {
        for (int i = 0; i < rootNode.SubNodes.Length; i++) // Get all nodes the player object is colliding with
        {
            OctreeNode<int> nodeAtPosition = OctreeLib.GetNodeAtPosition(rootNode.SubNodes[i], this.transform.position);
            if (nodeAtPosition != null)
            {
                InnerCollisionNodes[i] = nodeAtPosition;
            }
        }
    }
}