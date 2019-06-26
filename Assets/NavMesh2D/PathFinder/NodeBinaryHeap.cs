


using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Lockstep.Serialization;
using UnityEngine;

/** @author Nathan Sweet */
public class Node {
	public float value; //节点排序比较值
	public int index;	//节点索引

	public Node(float value) {
		this.value = value;
	}

	public float getValue() {
		return value;
	}

	public String ToString() {
		return value.ToString();
	}
}

/**
 * 二叉堆
 *  @author Nathan Sweet */
public class NodeBinaryHeap<T> where T: Node{
	public int size;

	private Node[] nodes;
	private bool isMaxHeap;

	public NodeBinaryHeap():this(16, false) {
		
	}

	public NodeBinaryHeap(int capacity, bool isMaxHeap) {
		this.isMaxHeap = isMaxHeap;
		nodes = new Node[capacity];
	}

	/**
	 * 添加节点
	 * @param node
	 * @return
	 */
	public T add(T node) {
		// Expand if necessary.
		if (size == nodes.Length) {
			Node[] newNodes = new Node[size << 1];
			Array.Copy(nodes, 0, newNodes, 0, size);
			nodes = newNodes;
		}
		// Insert at end and bubble up.
		node.index = size;
		nodes[size] = node;
		up(size++);
		return node;
	}

	/**
	 * 添加节点，并设置排序比较值
	 * @param node
	 * @param value 排序比较值
	 * @return
	 */
	public T Add(T node, float value) {
		node.value = value;
		return add(node);
	}

	public T peek() {
		if (size == 0)
			throw new Exception("The heap is empty.");
		return (T) nodes[0];
	}

	/**
	 * 获得堆最小值，并移除
	 * @return
	 */
	public T pop() {
		return remove(0);
	}

	public T remove(T node) {
		return remove(node.index);
	}

	/**
	 * 移除元素
	 * @param index
	 * @return
	 */
	private T remove(int index) {
		Node[] nodes = this.nodes;
		Node removed = nodes[index];
		nodes[index] = nodes[--size];
		nodes[size] = null;
		if (size > 0 && index < size)
			down(index);
		return (T) removed;
	}

	public void clear() {
		Node[] nodes = this.nodes;
		for (int i = 0, n = size; i < n; i++)
			nodes[i] = null;
		size = 0;
	}

	/**
	 * 设置节点值，并进行排序
	 * @param node
	 * @param value
	 */
	public void setValue(T node, float value) {
		float oldValue = node.value;
		node.value = value;
		if (value < oldValue ^ isMaxHeap)
			up(node.index);
		else
			down(node.index);
	}

	/**
	 * 如果节点值小于中间节点值，将节点上移，否则放在末尾
	 * @param index
	 */
	private void up(int index) {
		Node[] nodes = this.nodes;
		Node node = nodes[index];
		float value = node.value;
		while (index > 0) {
			int parentIndex = (index - 1) >> 1;
			Node parent = nodes[parentIndex];
			if (value < parent.value ^ isMaxHeap) {
				nodes[index] = parent;
				parent.index = index;
				index = parentIndex;
			} else
				break;
		}
		nodes[index] = node;
		node.index = index;
	}

	/**
	 * 节点后移
	 * @param index
	 */
	private void down(int index) {
		Node[] nodes = this.nodes;
		int size = this.size;

		Node node = nodes[index];
		float value = node.value;

		while (true) {
			int leftIndex = 1 + (index << 1);
			if (leftIndex >= size)
				break;
			int rightIndex = leftIndex + 1;

			// Always have a left child.
			Node leftNode = nodes[leftIndex];
			float leftValue = leftNode.value;

			// May have a right child.
			Node rightNode;
			float rightValue;
			if (rightIndex >= size) {
				rightNode = null;
				rightValue = isMaxHeap ? float.MinValue : float.MaxValue;
			} else {
				rightNode = nodes[rightIndex];
				rightValue = rightNode.value;
			}

			// The smallest of the three values is the parent.
			if (leftValue < rightValue ^ isMaxHeap) {
				if (leftValue == value || (leftValue > value ^ isMaxHeap))
					break;
				nodes[index] = leftNode;
				leftNode.index = index;
				index = leftIndex;
			} else {
				if (rightValue == value || (rightValue > value ^ isMaxHeap))
					break;
				nodes[index] = rightNode;
				rightNode.index = index;
				index = rightIndex;
			}
		}

		nodes[index] = node;
		node.index = index;
	}



	public int GetHashCode() {
		int h = 1;
		for (int i = 0, n = size; i < n; i++)
			h = h * 31 + FloatToIntBits(nodes[i].value);
		return h;
	}

	int FloatToIntBits(float value){
		ConverterHelperFloat ch = new ConverterHelperFloat { Afloat = value };
		return ch.Aint;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct ConverterHelperFloat
	{
		[FieldOffset(0)]
		public int Aint;

		[FieldOffset(0)]
		public float Afloat;
	}
	
	public String ToString() {
		if (size == 0)
			return "[]";
		Node[] nodes = this.nodes;
		StringBuilder buffer = new StringBuilder(32);
		buffer.Append('[');
		buffer.Append(nodes[0].value);
		for (int i = 1; i < size; i++) {
			buffer.Append(", ");
			buffer.Append(nodes[i].value);
		}
		buffer.Append(']');
		return buffer.ToString();
	}


}
