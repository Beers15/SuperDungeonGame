using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class Path
{
    List<Pos> positions;
	public Path(List<Pos> positions) 
	{
		this.positions = positions;
	}
	public void truncateTo(int length)
	{
		if (positions == null) return;
		if (length >= distance()) return;
		
		int i = 1, remaining = length;
		int next = Pos.abs_dist(positions[1], positions[0]);
		while (remaining - next > 0 && i < positions.Count) {
			i++;
			remaining -= next;
			next = Pos.abs_dist(positions[i], positions[i-1]);
		}
		
		Pos cap = null;
		Pos diff = positions[i] - positions[i-1];
		
		if (diff.y != 0) {
			remaining = diff.y < 0 ? -remaining : remaining;
			cap = positions[i-1] + new Pos(0, remaining);
		}
		else if (diff.x != 0) {
			remaining = diff.x < 0 ? -remaining : remaining;
			cap = positions[i-1] + new Pos(remaining, 0);
		}
		
		List<Pos> newPositions = new List<Pos>();
		for (int j = 0; j < i; j++) newPositions.Add(positions[j]);
		newPositions.Add(cap);
		
		positions = newPositions;
	}
	public Pos startPos() 
	{
		if (positions == null) return null;
		return positions[0];
	}
	public Pos endPos() 
	{
		if (positions == null) return null;
		return positions[positions.Count - 1];
	}
	public int distance()
	{
		if (positions == null) return -1;
		
		int distance = 0;
		for (int i = 1; i < positions.Count; i++)
			distance += Pos.abs_dist(positions[i], positions[i-1]);
		return distance;
	}
	public List<Pos> getPositions()
	{
		return positions;
	}
	public bool empty()
	{
		return positions == null;
	}
}
