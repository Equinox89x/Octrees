using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OctreeLibraryFunctions<TType>
{
    private static ShapeType _shapeType;
    public static ShapeType ShapeType
    {
        get { return _shapeType; }
        set { _shapeType = value; }
    }

    /// <summary>
    /// Creates the Octree based on available data
    /// </summary>
    public static Octree<TType> MakeTree(GameObject gameObject, GameObject documentRootObject, bool overrideSize, float size, bool startFromObjectYAxis, int nrOfDivisions, ShapeType shape)
    {
        Vector3 pos = gameObject ? gameObject.transform.position : documentRootObject.transform.position;
        if (gameObject && !overrideSize)
        {
            RectTransform rt = (RectTransform)gameObject.transform;
            size = rt.rect.width;
        }
        if (startFromObjectYAxis)
        {
            pos.y += size / 2;
        }

        _shapeType = shape;
        return new Octree<TType>(pos, size, nrOfDivisions > 3 ? 3 : nrOfDivisions, shape);
    }
    
    /// <summary>
    /// Creates the Octree based on available data
    /// </summary>
    public static Octree<TType> MakeHitpointTree(GameObject gameObject, GameObject documentRootObject, int nrOfDivisions, ShapeType shape)
    {
        Vector3 pos = gameObject ? gameObject.transform.position : documentRootObject.transform.position;
        float size = 2;
        //if (gameObject)
        //{
        //    RectTransform rt = (RectTransform)gameObject.transform;
        //    size = rt.rect.width;
        //}

        SkinnedMeshRenderer obj = gameObject.GetComponent<SkinnedMeshRenderer>();
        //obj.sharedMesh.vertices
        //Mesh mesh = gameObject.GetComponent<Mesh>();

        _shapeType = shape;
        if (obj)
        {
            return new Octree<TType>(pos, obj.sharedMesh.vertices, shape);
        }
        return null;
    }

    ///<summary>
    ///Gets the node that is at the <paramref name="position"/>
    ///</summary>
    public static OctreeNode<TType> GetNodeAtPosition<TType>(OctreeNode<TType> node, Vector3 position)
    {
        //if (!node.IsLeaf())
        //{
        if (node.SubNodes != null)
        {
            foreach (var subnode in node.SubNodes)
            {
                Bounds bounds = new Bounds(subnode.Position, new Vector3(subnode.Size, subnode.Size, subnode.Size));
                Bounds targetBounds = new Bounds(position, new Vector3(3, 3, 3));
                if (targetBounds.Intersects(bounds))
                {
                    return GetNodeAtPosition(subnode, position);
                }
                //switch (ShapeType)
                //{
                //    case ShapeType.Cube:
                //        if (targetBounds.Intersects(bounds))
                //        {
                //            return GetNodeAtPosition(subnode, position);
                //        }
                //        break;
                //    case ShapeType.Sphere:
                //        Vector3 bottomLeft = subnode.Position - new Vector3(subnode.Size / 2, subnode.Size / 2, subnode.Size / 2);
                //        Vector3 topRight = subnode.Position + new Vector3(subnode.Size / 2, subnode.Size / 2, subnode.Size / 2);
                //        if (AreSpheresOverlapping(subnode.Position, position, subnode.Size, 15) || Vector3.Distance(position, subnode.Position) < 15 || DoesCubeIntersectSphere(bottomLeft, topRight, position, 15))
                //        {
                //            return GetNodeAtPosition(subnode, position);
                //        }
                //        break;
                //    case ShapeType.Quad:
                //        break;
                //    default:
                //        break;
                //}

            }
        }
        else
        {
            return node;
        }
        return null;
        //}
    }

    /// <summary>
    /// check if nodes in a list intersect with nodes in an array
    /// </summary>
    public static bool IsIntersectingNodes<TType>(List<OctreeNode<TType>> intersectNodes, OctreeNode<TType>[] checkNodes, bool isCluster = false)
    {
        if (isCluster)
        {

            //rather than looking from every node, turn the 4 nodes into 1 bound
            //get the center of the 4 bottom nodes, and increase y with half the size (since there are 4 other nodes on top of it)
            Vector3 result = (intersectNodes[0].Position + intersectNodes[1].Position + intersectNodes[2].Position + intersectNodes[3].Position) / 4.0f;
            result.y += intersectNodes[0].Size / 2;

            float newSize = intersectNodes[0].Size * 2f;
            Bounds intersectBounds = new Bounds(result, new Vector3(newSize, newSize, newSize));
            if (IsIntersectingNode(intersectBounds, checkNodes))
            {
                return true;
            }
            //switch (shape)
            //{
            //    case ShapeType.Cube:
            //        //return IsIntersectingNode(intersectBounds, checkNodes);
            //        if (IsIntersectingNode(intersectBounds, checkNodes))
            //        {
            //            return true;
            //        }
            //        break;

            //    case ShapeType.Sphere:
            //        OctreeNode<TType> node = new OctreeNode<TType>(intersectNodes[0].Position, newSize, intersectNodes[0].ShapeType);
            //        //return IsIntersectingNode(node, checkNodes);
            //        if (IsIntersectingNode(node, checkNodes))
            //        {
            //            return true;
            //        }
            //        break;

            //    case ShapeType.Quad:

            //        break;

            //    default:
            //        //return IsIntersectingNode(intersectBounds, checkNodes);
            //        if (IsIntersectingNode(intersectBounds, checkNodes))
            //        {
            //            return true;
            //        }
            //        break;
            //}
            //DrawBounds(intersectNodes[0], new Color(0, 1, 0, 1));
        }
        else
        {
            foreach (var intersectNode in intersectNodes)
            {
                if (IsIntersectingNode(intersectNode, checkNodes))
                {
                    return true;
                }
                //return IsIntersectingNode(intersectNode, checkNodes);
            }
        }
        return false;
    }

    #region Check intersect of bounds or sphere with node
    ///<summary>
    ///check if bounds intersect with nodes from an array
    ///</summary>
    public static bool IsIntersectingNode<TType>(Bounds intersectBounds, OctreeNode<TType>[] checkNodes)
    {
        foreach (var checkNode in checkNodes)
        {
            Bounds checkBounds = new Bounds(checkNode.Position, new Vector3(checkNode.Size, checkNode.Size, checkNode.Size));
            if (checkBounds.Intersects(intersectBounds)) { 
                return true; 
            }

        }
        return false;
    }

    ///<summary>
    ///check if bounds intersect with nodes from a List
    ///</summary>
    public static bool IsIntersectingNode<TType>(Bounds intersectBounds, List<OctreeNode<TType>> checkNodes)
    {
        foreach (var checkNode in checkNodes)
        {
            Bounds checkBounds = new Bounds(checkNode.Position, new Vector3(checkNode.Size, checkNode.Size, checkNode.Size));
            if (checkBounds.Intersects(intersectBounds)) { 
                return true; 
            }

        }
        return false;
    }

    ///<summary>
    ///check if a node intersect with nodes from a List
    ///</summary>
    public static bool IsIntersectingNode<TType>(OctreeNode<TType> intersectBounds, List<OctreeNode<TType>> checkNodes)
    {
        foreach (var checkNode in checkNodes)
        {
            Bounds checkBounds = new Bounds(checkNode.Position, new Vector3(checkNode.Size, checkNode.Size, checkNode.Size));
            float newSize = intersectBounds.Size * 2f;
            Bounds intersectBound = new Bounds(intersectBounds.Position, new Vector3(newSize, newSize, newSize));
            if (checkBounds.Intersects(intersectBound))
            {
                return true;
            }

            //if (intersectBounds.ShapeType == ShapeType.Sphere && checkNode.ShapeType == ShapeType.Sphere)
            //{
            //    if (AreSpheresOverlapping(checkNode.Position, intersectBounds.Position, checkNode.Size, intersectBounds.Size))
            //    {
            //        return true;
            //    }
            //}
            //else
            //{
            //    Vector3 bottomLeft = checkNode.Position - new Vector3(checkNode.Size / 2, checkNode.Size / 2, checkNode.Size / 2);
            //    Vector3 topRight = checkNode.Position + new Vector3(checkNode.Size / 2, checkNode.Size / 2, checkNode.Size / 2);
            //    //if (DoesCubeIntersectSphere(bottomLeft, topRight, intersectBounds.Position, intersectBounds.Size))
            //    //{
            //    //    return true;
            //    //}
            //    if (Vector3.Distance(intersectBounds.Position, checkNode.Position) < intersectBounds.Size)
            //    {
            //        return true;
            //    }
            //}

        }
        return false;
    }

    ///<summary>
    ///check if a node intersect with nodes from an Array
    ///</summary>
    public static bool IsIntersectingNode<TType>(OctreeNode<TType> intersectBounds, OctreeNode<TType>[] checkNodes)
    {
        foreach (var checkNode in checkNodes)
        {
            Bounds checkBounds = new Bounds(checkNode.Position, new Vector3(checkNode.Size, checkNode.Size, checkNode.Size));
            float newSize = intersectBounds.Size * 2f;
            Bounds intersectBound = new Bounds(intersectBounds.Position, new Vector3(newSize, newSize, newSize));
            if (checkBounds.Intersects(intersectBound))
            {
                return true;
            }

            //if (intersectBounds.ShapeType == ShapeType.Sphere && checkNode.ShapeType == ShapeType.Sphere)
            //{
            //    if (AreSpheresOverlapping(checkNode.Position, intersectBounds.Position, checkNode.Size, intersectBounds.Size))
            //    {
            //        return true;
            //    }
            //}
            //else
            //{
            //    Vector3 bottomLeft = checkNode.Position - new Vector3(checkNode.Size / 2, checkNode.Size / 2, checkNode.Size / 2);
            //    Vector3 topRight = checkNode.Position + new Vector3(checkNode.Size / 2, checkNode.Size / 2, checkNode.Size / 2);
            //    //if (DoesCubeIntersectSphere(bottomLeft, topRight, intersectBounds.Position, intersectBounds.Size))
            //    //{
            //    //    DrawSphereBounds(bottomLeft, new Color(0, 0, 1, 1), checkNode.Size);
            //    //    DrawSphereBounds(topRight, new Color(0, 0, 1, 1), checkNode.Size);
            //    //    return true;
            //    //}

            //    if (Vector3.Distance(intersectBounds.Position, checkNode.Position) < intersectBounds.Size)
            //    {
            //        DrawSphereBounds(bottomLeft, new Color(0, 0, 1, 1), checkNode.Size);
            //        DrawSphereBounds(topRight, new Color(0, 0, 1, 1), checkNode.Size);
            //        return true;
            //    }
            //}
        }
        return false;
    }
    #endregion

    #region Sphere & Sphere/Cube Overlap methods
    /// <summary>
    /// returns true if the given spheres are overlapping
    /// </summary>
    public static bool AreSpheresOverlapping(Vector3 r, Vector3 r2, float radius1, float radius2)
    {
        float distance = (float)Math.Sqrt(Math.Pow((r2.x - r.x), 2) + Math.Pow(r2.y - r.y, 2) + Math.Pow(r2.z - r.z, 2));
        return distance < radius1 + radius2 || distance < Math.Abs(radius1 - radius2);
    }

    /// <summary>
    /// return true if a cube intersects with a sphere
    /// </summary>
    private static bool DoesCubeIntersectSphere(Vector3 C1, Vector3 C2, Vector3 S, float R)
    {
        float dist_squared = R * R;
        /* assume C1 and C2 are element-wise sorted, if not, do that now */
        if (S.x < C1.x) dist_squared -= (float)Math.Pow(S.x - C1.x, 2);
        else if (S.x > C2.x) dist_squared -= (float)Math.Pow(S.x - C2.x, 2);
        if (S.y < C1.y) dist_squared -= (float)Math.Pow(S.y - C1.y,2);
        else if (S.y > C2.y) dist_squared -= (float)Math.Pow(S.y - C2.y,2);
        if (S.z < C1.z) dist_squared -= (float)Math.Pow(S.z - C1.z,2);
        else if (S.z > C2.z) dist_squared -= (float)Math.Pow(S.z - C2.z,2);
        return dist_squared > 0;
    }

    #endregion

    #region debug drawing bounds
    /// <summary>
    /// Debug draws shapes based on the shapetype
    /// </summary>
    public static void DrawBounds<TType>(OctreeNode<TType> nodeAtPosition, Color color, float delay = 0)
    {
        switch (ShapeType)
        {
            case ShapeType.Cube:
                Bounds cube = new Bounds(nodeAtPosition.Position, new Vector3(nodeAtPosition.Size, nodeAtPosition.Size, nodeAtPosition.Size));
                DrawCubeBounds(cube, color, delay);
                break;

            case ShapeType.Sphere:
                DrawSphereBounds(nodeAtPosition.Position, color, delay);
                break;

            case ShapeType.Quad:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Draw the correct Gizmo's based on the type (debug only)
    /// </summary>
    public static void DrawCorrectWireType(OctreeNode<int> node)
    {
        switch (ShapeType)
        {
            case ShapeType.Cube:
                Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
                break;
            case ShapeType.Sphere:
                Gizmos.DrawWireSphere(node.Position, node.Size / 2);
                break;
            case ShapeType.Quad:
                break;
            default:
                break;
        }
    }

    ///<summary>
    ///Drawing a debug cuboid from the Bounds
    ///</summary>
    public static void DrawCubeBounds(Bounds b, Color color, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, color, delay);
        Debug.DrawLine(p2, p3, color, delay);
        Debug.DrawLine(p3, p4, color, delay);
        Debug.DrawLine(p4, p1, color, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, color, delay);
        Debug.DrawLine(p6, p7, color, delay);
        Debug.DrawLine(p7, p8, color, delay);
        Debug.DrawLine(p8, p5, color, delay);

        // sides
        Debug.DrawLine(p1, p5, color, delay);
        Debug.DrawLine(p2, p6, color, delay);
        Debug.DrawLine(p3, p7, color, delay);
        Debug.DrawLine(p4, p8, color, delay);
    }

    ///<summary>
    ///Drawing a debug spheroid from the Bounds
    ///</summary>
    public static void DrawSphereBounds(Vector3 b, Color color, float size, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.x + size / 2, b.y, b.z);
        var p2 = new Vector3(b.x - size / 2, b.y, b.z);
        var p3 = new Vector3(b.x, b.y + size / 2, b.z);
        var p4 = new Vector3(b.x, b.y - size / 2, b.z);
        var p5 = new Vector3(b.x, b.y, b.z + size / 2);
        var p6 = new Vector3(b.x, b.y, b.z - size / 2);
        var p7 = new Vector3(b.x + size / 2, b.y, b.z + size / 2);
        var p8 = new Vector3(b.x - size / 2, b.y, b.z - size / 2);
        var p9 = new Vector3(b.x, b.y + size / 2, b.z + size / 2);
        var p10 = new Vector3(b.x, b.y - size / 2, b.z - size / 2);

        Debug.DrawLine(b, p2, color, delay);
        Debug.DrawLine(b, p3, color, delay);
        Debug.DrawLine(b, p4, color, delay);
        Debug.DrawLine(b, p1, color, delay);

        Debug.DrawLine(b, p6, color, delay);
        Debug.DrawLine(b, p7, color, delay);
        Debug.DrawLine(b, p8, color, delay);
        Debug.DrawLine(b, p5, color, delay);

        Debug.DrawLine(b, p9, color, delay);
        Debug.DrawLine(b, p10, color, delay);
    }
    #endregion

    #region find intersecting nodes
    ///<summary>
    ///Find all the nodes an object is intersecting with
    ///</summary>
    public static void FindIntersectingNodes(OctreeComponent octree, List<OctreeNode<int>> innerCollisionNodes, List<OctreeNode<int>> intersectedWorldNodes, bool canDrawGameDebug)
    {
        intersectedWorldNodes.Clear();
        OctreeNode<int> rootNode = octree.Octree.GetRoot();
        if (rootNode == null) return;
        foreach (var subnode in rootNode.SubNodes) // Get all nodes with an object of this area
        {
            FindIntersectingNodesRecursion(subnode, innerCollisionNodes, intersectedWorldNodes, canDrawGameDebug);
        }
    }

    ///<summary>
    ///Find all the nodes the gameobject is intersecting with recursively
    ///</summary>
    private static void FindIntersectingNodesRecursion(OctreeNode<int> subnode, List<OctreeNode<int>> innerCollisionNodes, List<OctreeNode<int>> intersectedWorldNodes, bool canDrawGameDebug = false)
    {
        if (subnode.SubNodes != null)
        {
            if (IsIntersectingNodes(innerCollisionNodes, subnode.SubNodes, true))
            {
                foreach (var hitNode in subnode.SubNodes)
                {
                    FindIntersectingNodesRecursion(hitNode, innerCollisionNodes, intersectedWorldNodes, canDrawGameDebug);
                }
            }
        }
        else
        {
            Bounds bounds = new Bounds(subnode.Position, new Vector3(subnode.Size, subnode.Size, subnode.Size));
            if (IsIntersectingNode(bounds, innerCollisionNodes))
            {
                intersectedWorldNodes.Add(subnode);
                if (canDrawGameDebug)
                {
                    DrawBounds(subnode, new Color(0.5f, 0.5f, 1f, 1));
                }
            }
        }
    }
    #endregion

    public static void FindInnerCollisionNodes<TType>(OctreeNode<TType> rootNode, List<OctreeNode<TType>> InnerCollisionNodes, Vector3 position)
    {
        for (int i = 0; i < rootNode.SubNodes.Length; i++) // Get all nodes the player object is colliding with
        {
            OctreeNode<TType> nodeAtPosition = GetNodeAtPosition(rootNode.SubNodes[i], position);
            if (nodeAtPosition != null)
            {
                InnerCollisionNodes[i] = nodeAtPosition;
            }
        }
    }
}