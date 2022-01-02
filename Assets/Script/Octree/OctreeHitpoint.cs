using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using OctreeLib = OctreeLibraryFunctions<int>;

public class OctreeHitpoint : MonoBehaviour
{

    #region serialised fields
    [Header("Setup")]
    [SerializeField]
    private int nrOfDivisions = 2;
    [SerializeField]
    private GameObject gameObject;
    [SerializeField]
    private ShapeType shapeType;

    [Header("Debug")]
    [SerializeField]
    private bool CanDrawDebug;
    [SerializeField]
    private bool canDrawGameDebug;
    [SerializeField]
    private bool canDrawGizmo;
    #endregion

    #region props
    private Octree<int> Octree;
    private OctreeNode<int> RootNode;
    private OctreeComponent WorldOctree;
    #endregion

    // Use this for initialization
    void Start()
    {
        #region make the octree
        Octree = OctreeLib.MakeHitpointTree(gameObject, this.gameObject, nrOfDivisions > 3 ? 3 : nrOfDivisions, shapeType);
        if (Octree == null) return;
        #endregion

        #region make the innercollision node list
        RootNode = Octree.GetRoot();
        if (RootNode == null) return;
        #endregion


    }

    #region debug
    void OnDrawGizmos()
    {
        if (canDrawGizmo)
        {
            #region setup
            Octree = OctreeLib.MakeHitpointTree(gameObject, this.gameObject, nrOfDivisions > 3 ? 3 : nrOfDivisions, shapeType);
            if (Octree == null) return;

            RootNode = Octree.GetRoot();
            if (RootNode == null) return;
            #endregion

            if (CanDrawDebug)
            {
                int i = 1;
                DrawNode(Octree.GetRoot(), i);
            }
        }
    }

    private void DrawNode(OctreeNode<int> node, int i)
    {
        if (!node.IsLeaf())
        {
            foreach (var subnode in node.TestNodes)
            {
                DrawNode(subnode, i);
                i++;
            }
        }

        if (i >= 10 && i < 20)
        {
            Gizmos.color = new Color(0, 0, 1, 1f);
        }
        if (i >= 20 && i < 30)
        {
            Gizmos.color = new Color(0, 1, 1, 1f);
        }
        if (i >= 30)
        {
            Gizmos.color = new Color(1, 0, 1, 1f);
        }
        Gizmos.DrawCube(node.Position, new Vector3(0.1f, 0.3f, 0.25f));
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        #region octree update
        Octree = OctreeLib.MakeHitpointTree(gameObject, this.gameObject, nrOfDivisions > 3 ? 3 : nrOfDivisions, shapeType);
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
        #endregion

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
}
