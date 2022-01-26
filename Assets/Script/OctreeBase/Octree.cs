using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree<TType>
{
    #region props
    private OctreeNode<TType> node;
    private int nrOfDivisions;
    private ShapeType shapeType;

    private List<OctreeNode<int>> _innerCollisionNodes = new List<OctreeNode<int>>() { null, null, null, null, null, null, null, null };
    public List<OctreeNode<int>> InnerCollisionNodes
    {
        get { return _innerCollisionNodes; }
        set { _innerCollisionNodes = value; }
    }
    #endregion

    #region init
    public Octree(Vector3 position, float size, int nrOfDivisions, ShapeType shape)
    {
        shapeType = shape;
        node = new OctreeNode<TType>(position, size, shape);
        node.Subdivide(node, nrOfDivisions);
    }
    
    public Octree(Vector3 position, Vector3[] verts, ShapeType shape)
    {
        shapeType = shape;
        node = new OctreeNode<TType>(position, 0.01f, shape);
        node.MakeHitpoints(verts);
    }
    #endregion

    ///<summary>
    ///Gets the top level node from the tree
    ///</summary>
    public OctreeNode<TType> GetRoot() => node;
    //{
    //    return node;
    //}
}