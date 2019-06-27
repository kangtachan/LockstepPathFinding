using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 默认图路径 
 * <br>
 * Default implementation of a {@link GraphPath} that uses an internal
 * {@link List} to store nodes or connections.
 * 
 * @param <N>
 *            Type of node
 * 
 * @author davebaol
 */
public class DefaultGraphPath<N> : GraphPath<N> {
    public List<N> nodes;

    /** Creates a {@code DefaultGraphPath} with no nodes. */
    public DefaultGraphPath() : this(new List<N>()){ }

    /** Creates a {@code DefaultGraphPath} with the given capacity and no nodes. */
    public DefaultGraphPath(int capacity) : this(new List<N>(capacity)){ }

    /** Creates a {@code DefaultGraphPath} with the given nodes. */
    public DefaultGraphPath(List<N> nodes){
        this.nodes = nodes;
    }

    public void Clear(){
        nodes.Clear();
    }

    public int GetCount(){
        return nodes.Count;
    }

    public void Add(N node){
        nodes.Add(node);
    }

    public N Get(int index){
        return nodes[index];
    }

    public void reverse(){
        nodes.Reverse();
    }
}