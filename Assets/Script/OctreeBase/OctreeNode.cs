using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeNode<TType>
{
    #region props
    private List<Player<TType>> _players = new List<Player<TType>>();
    public List<Player<TType>> Players
    {
        get { return _players; }
        set { _players = value; }
    }

    private List<KeyValuePair<string, dynamic>> _blackboard = new List<KeyValuePair<string, dynamic>>();
    public List<KeyValuePair<string, dynamic>> Blackboard
    {
        get { return _blackboard; }
        set { _blackboard = value; }
    }

    private int _depth;
    public int Depth
    {
        get { return _depth; }
        set { _depth = value; }
    }

    private OctreeNode<TType>[] _subNodes;
    public OctreeNode<TType>[] SubNodes
    {
        get { return _subNodes; }
    }

    private Vector3 _position;
    public Vector3 Position
    {
        get { return _position; }
    }

    private float _size;
    public float Size
    {
        get { return _size; }
    }

    private ShapeType _shapeType;

    public ShapeType ShapeType
    {
        get { return _shapeType; }
        set { _shapeType = value; }
    }

    private OctreeNode<TType> _parentNode;
    public OctreeNode<TType> ParentNode
    {
        get { return _parentNode; }
        set { _parentNode = value; }
    }

    public List<OctreeNode<TType>> TestNodes = new List<OctreeNode<TType>>();
    #endregion

    #region init
    public OctreeNode(Vector3 pos, float size, ShapeType shape)
    {
        _position = pos;
        _size = size;
        _shapeType = shape;
    }

    ///<summary>
    ///Subdivides each cube into 8 more cubes recursively
    ///</summary>
    public void Subdivide(OctreeNode<TType> parentNode, int depth = 0)
    {
        _subNodes = new OctreeNode<TType>[8];
        for (int i = 0; i < _subNodes.Length; ++i)
        {
            Vector3 newPos = _position;
            if ((i & 4) == 4)
            {
                newPos.y += _size * 0.25f;
            }
            else
            {
                newPos.y -= _size * 0.25f;
            }

            if ((i & 2) == 2)
            {
                newPos.x += _size * 0.25f;
            }
            else
            {
                newPos.x -= _size * 0.25f;
            }

            if ((i & 1) == 1)
            {
                newPos.z += _size * 0.25f;
            }
            else
            {
                newPos.z -= _size * 0.25f;
            }

            _subNodes[i] = new OctreeNode<TType>(newPos, _size * 0.5f, ShapeType);
            _subNodes[i].Depth = depth;
            _subNodes[i].ParentNode = parentNode;
            if (depth > 0)
            {
                _subNodes[i].Subdivide(_subNodes[i], depth - 1);
            }
        }
    }

    //generate based on joints
    public void MakeHitpoints(Vector3[] verts)
    {
        _subNodes = new OctreeNode<TType>[(verts.Length/100)+10];
        int i2 = 0;
        for (int i = 0; i < verts.Length; i++)
        {
            if(i2 == 100)
            {
                Vector3 vect = Position;
                vect.x -= verts[i].x;
                vect.y += verts[i].y;
                TestNodes.Add(new OctreeNode<TType>(vect, 0.001f, ShapeType));
                i2 = 0;
            }
            else
            {
                i2++;
            }
        }
    }
    #endregion

    ///<summary>
    ///Gets the blackboard data from your <paramref name="key"/>
    ///</summary>
    public dynamic GetData(string key) => Blackboard.Find(x => x.Key == key).Value;

    /// <summary>
    /// Gets the blackboard data in a new keyvalue pair using you <paramref name="key"/> and <paramref name="value"/>
    /// </summary>
    public void SetData(string key, dynamic value) => Blackboard.Add(new KeyValuePair<string, dynamic>(key, value));

    ///<summary>
    ///return true if the node is the deepest node from the tree
    ///</summary>
    public bool IsLeaf() => _subNodes == null;
}
