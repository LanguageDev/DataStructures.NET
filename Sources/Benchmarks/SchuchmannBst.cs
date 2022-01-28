// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

// Source: https://github.com/Sebastian-Schuchmann/Binary-Search-Tree

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchuchmannBst;

internal enum TraversalType
{
    PreOrder,
    InOrder,
    OutOrder,
    PostOrder
}

internal sealed class Node<T> where T : IComparable<T>, IEquatable<T>
{
    public T Value;
    public Node<T>? Left;
    public Node<T>? Right;

    public Node(T val)
    {
        this.Value = val;
    }

    public bool HasTwoChildren()
    {
        if (this.Left != null && this.Right != null)
            return true;
        return false;
    }

    public bool HasOneChild()
    {
        if (this.Left != null ^ this.Right != null)
            return true;
        return false;
    }

    public bool HasNoChildren()
    {
        if (!this.HasTwoChildren() && !this.HasOneChild())
            return true;
        return false;
    }

    //Return limited to only childs
    public Node<T>? Child()
    {
        if (this.HasOneChild())
        {
            if (this.Left != null) return this.Left;
            if (this.Right != null) return this.Right;
        }
        return null;
    }
}

internal sealed class BinarySearchTree<T> where T : IComparable<T>, IEquatable<T>
{
    public Node<T>? Root;

    public void Insert(T Value)
    {
        if (this.Root == null)
        {
            this.Root = new Node<T>(Value);
        }
        else
        {
            this.InsertHelper(Value, this.Root);
        }
    }

    public void DeleteNode(Node<T> NodeToDelete)
    {
        //No children -> Just Delete
        if (NodeToDelete.HasNoChildren())
        {
            if (NodeToDelete.Equals(this.Root))
            {
                this.Root = null;
                return;
            }

            this.Traverse((Node<T> node) =>
            {
                if (node.Left != null)
                {
                    if (node.Left.Equals(NodeToDelete))
                        node.Left = null;
                }
                if (node.Right != null)
                {
                    if (node.Right.Equals(NodeToDelete))
                        node.Right = null;
                }
            }, TraversalType.PostOrder);
            return;
        }

        //Exactly one child -> Swap 
        if (NodeToDelete.HasOneChild())
        {
            if (NodeToDelete.Equals(this.Root))
            {
                this.Root = this.Root.Child();
                return;
            }

            this.Traverse((Node<T> node) =>
            {
                if (node.Left != null)
                {
                    if (node.Left.Equals(NodeToDelete))
                        node.Left = NodeToDelete.Child();
                }
                if (node.Right != null)
                {
                    if (node.Right.Equals(NodeToDelete))
                        node.Right = NodeToDelete.Child();
                }
            }, TraversalType.PostOrder);

            return;
        }

        //Find Minimum of Right sub-Tree, Copy Value, Delete Minimum 
        if (NodeToDelete.HasTwoChildren())
        {
            Node<T> SuccessorNode = this.MinValue(NodeToDelete.Right!);
            NodeToDelete.Value = SuccessorNode.Value;
            this.DeleteNode(SuccessorNode);

            return;
        }

    }

    public void Insert(T[] Values)
    {
        if (Values.Length == 0)
            return;

        int index = 0;

        if (this.Root == null)
        {
            this.Root = new Node<T>(Values[0]);
            index++;
        }

        for (int i = index; i < Values.Length; i++)
            this.InsertHelper(Values[i], this.Root);
    }

    public Node<T>? Search(T Value)
    {
        if (this.Root!.Value.Equals(Value))
            return this.Root;

        else
        {
            return this.SearchHelper(Value, this.Root);
        }
    }

    private Node<T>? SearchHelper(T Value, Node<T>? node)
    {
        if (node == null)
            return node;

        if (Value.Equals(node.Value))
            return node;

        if (Value.CompareTo(node.Value) == -1) //-1 = "<"
        {
            node = this.SearchHelper(Value, node.Left);
        }
        else
        {
            node = this.SearchHelper(Value, node.Right);
        }

        return node;
    }

    private Node<T> InsertHelper(T Value, Node<T>? node)
    {
        if (node == null)
        {
            node = new Node<T>(Value);
            return node;
        }
        if (Value.CompareTo(node.Value) == -1) //-1 = "<"
        {
            node.Left = this.InsertHelper(Value, node.Left);
        }
        else
        {
            node.Right = this.InsertHelper(Value, node.Right);
        }

        return node;
    }

    private Node<T> MinValue(Node<T> startingNode)
    {
        T minVal = startingNode.Value;
        while (startingNode.Left != null)
        {
            minVal = startingNode.Left.Value;
            startingNode = startingNode.Left;
        }
        return startingNode;
    }

    public void Traverse(Action<Node<T>> Process, TraversalType TravType)
    {
        if (this.Root == null)
        {
            return;
        }

        switch (TravType)
        {
        case TraversalType.InOrder: this.TraverseHelperInOrder(this.Root, Process); break;
        case TraversalType.OutOrder: this.TraverseHelperOutOrder(this.Root, Process); break;
        case TraversalType.PostOrder: this.TraverseHelperPostOrder(this.Root, Process); break;
        case TraversalType.PreOrder: this.TraverseHelperPreOrder(this.Root, Process); break;
        }
    }

    private void TraverseHelperPreOrder(Node<T>? node, Action<Node<T>> Process)
    {
        if (node == null)
        {
            return;
        }

        Process(node);
        this.TraverseHelperPreOrder(node.Left, Process);
        this.TraverseHelperPreOrder(node.Right, Process);
    }
    private void TraverseHelperInOrder(Node<T>? node, Action<Node<T>> Process)
    {
        if (node == null)
        {
            return;
        }

        this.TraverseHelperInOrder(node.Left, Process);
        Process(node);
        this.TraverseHelperInOrder(node.Right, Process);
    }
    private void TraverseHelperPostOrder(Node<T>? node, Action<Node<T>> Process)
    {
        if (node == null)
        {
            return;
        }

        this.TraverseHelperPostOrder(node.Left, Process);
        this.TraverseHelperPostOrder(node.Right, Process);
        Process(node);
    }
    private void TraverseHelperOutOrder(Node<T>? node, Action<Node<T>> Process)
    {
        if (node == null)
        {
            return;
        }

        this.TraverseHelperOutOrder(node.Right, Process);
        Process(node);
        this.TraverseHelperOutOrder(node.Left, Process);
    }
}
