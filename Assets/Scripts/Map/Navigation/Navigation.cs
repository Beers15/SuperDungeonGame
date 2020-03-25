using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
// includes definition for tile types, as well as traversable()
using static MapUtils.MapConstants;

public class NavigationHandler
{
	private class Vertex
	{
		public const int MAX_DIST = int.MaxValue / 2;
		
		public int dist;
		public bool visited;
		public Pos pos;
		public List<Vertex> visible;
		public Vertex prev;
		
		public Vertex(int x, int y)
		{
			dist 	 = MAX_DIST; // to avoid overflow errors
			visited  = false;
			pos 	 = new Pos(x, y);
			visible  = new List<Vertex>();
			prev	 = null;
		}
		
		public Vertex(Pos p)
		{
			dist 	 = MAX_DIST;
			visited  = false;
			pos 	 = p;
			visible  = new List<Vertex>();
			prev	 = null;
		}
		
		public void reset()
		{
			dist 	 = MAX_DIST;
			visited  = false;
			prev	 = null;
		}
		
		public override string ToString()
		{
			return pos.ToString() + "|" + dist.ToString() + "|" + visited.ToString() + "|" + visible.Count.ToString();
		}
	}
	
	private Vertex[,] vertex_map;
	private List<Vertex> nav_graph;
	
	private int[,] map;
	private int width;
	private int height;
	
	private static float total_verts = 0.0f;
	private static float total_edges = 0.0f;
	
	public NavigationHandler(int[,] map)
	{
		this.map = map;
		width = map.GetLength(0);
		height = map.GetLength(1);
		vertex_map = new Vertex[width, height];
		nav_graph = new List<Vertex>();
		
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				if (tile_traversable(x, y)) {
					Vertex vert = new Vertex(x, y);
					vertex_map[x, y] = vert;
					nav_graph.Add(vert);
				}
				
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				if (tile_traversable(x, y)) {
					
					Vertex vert = vertex_map[x, y];
					
					if (tile_traversable(x-1, y))
						vert.visible.Add(vertex_map[x-1,y]);
					
					if (tile_traversable(x+1, y))
						vert.visible.Add(vertex_map[x+1,y]);
					
					if (tile_traversable(x, y-1))
						vert.visible.Add(vertex_map[x,y-1]);
					
					if (tile_traversable(x, y+1))
						vert.visible.Add(vertex_map[x,y+1]);
				}
		
		total_verts += nav_graph.Count;
		foreach (Vertex vert in nav_graph) {
			total_edges += vert.visible.Count;
		}
	}
	
	bool tile_traversable(int x, int y)
	{
		if (x < 0 || x >= width || y < 0 || y >= height)
			return false;
		
		else if (traversable(map[x, y]))
			return true;
		
		else return false;
	}
	
	public void removeTraversableTile(Pos tilePos)
	{
		Vertex toDie = vertex_map[tilePos.x, tilePos.y];
		if (toDie == null) return;
		
		foreach (Vertex neighbor in toDie.visible) {
			neighbor.visible.Remove(toDie);
		}
		
		nav_graph.Remove(toDie);
		vertex_map[tilePos.x, tilePos.y] = null;
	}
	
	public void insertTraversableTile(Pos tilePos)
	{
		Vertex newVert = new Vertex(tilePos.x, tilePos.y);
		
		int x = tilePos.x, y = tilePos.y;
		if (vertex_map[x, y] != null) return;
		vertex_map[x, y] = newVert;
		
		if (vertex_map[x-1, y] != null) {
			newVert.visible.Add(vertex_map[x-1,y]);
			vertex_map[x-1, y].visible.Add(newVert);
		}
		
		if (vertex_map[x+1, y] != null) {
			newVert.visible.Add(vertex_map[x+1,y]);
			vertex_map[x+1, y].visible.Add(newVert);
		}
		
		if (vertex_map[x, y-1] != null) {
			newVert.visible.Add(vertex_map[x,y-1]);
			vertex_map[x, y-1].visible.Add(newVert);
		}
		
		if (vertex_map[x, y+1] != null) {
			newVert.visible.Add(vertex_map[x,y+1]);
			vertex_map[x, y+1].visible.Add(newVert);
		}
		
		nav_graph.Add(newVert);
	}
	
	Vertex pop_min_dist_vert(List<Vertex> graph)
	{
		Vertex min = graph[0];
		for (int i = 1; i < graph.Count; i++) {
			if (graph[i].dist < min.dist)
				min = graph[i];
		}
		graph.Remove(min);
		return min;
	}
	
	// finds a single path from point a to point b
	public List<Pos> shortestPath(Pos p_origin, Pos p_target)
	{
		try {
			Vertex source = vertex_map[p_origin.x, p_origin.y];
			Vertex target = vertex_map[p_target.x, p_target.y];
			
			if (source == null || target == null) return null;
			
			// maximum range we will need to search
			int maxDistance = Pos.abs_dist(source.pos, target.pos);
			
			// create a temporary graph of vertices to be pulled from during pathfinding
			List<Vertex> tmp_graph = new List<Vertex>(nav_graph);
			
			source.dist = 0;
			Vertex min_vert = pop_min_dist_vert(tmp_graph);
			
			// uses dijkstra method to find minimum distance from origin to target
			while (tmp_graph.Count > 0 && min_vert != target && min_vert != null) {
				foreach (Vertex neighbor in min_vert.visible) {
					int alt_dist = min_vert.dist + Pos.abs_dist(min_vert.pos, neighbor.pos);
					if (alt_dist < neighbor.dist) {
						neighbor.dist = alt_dist;
						neighbor.prev = min_vert;
					}
				}
				min_vert = pop_min_dist_vert(tmp_graph);
			}
			
			List<Pos> path = null;
			if (target.prev != null) path = constructPath(target, source);
			
			return path;
		}
		finally {
			// post path-finding cleanup
			foreach(Vertex vertex in nav_graph)
				vertex.reset();
		}
	}
	
	// finds an entire batch of paths
	public List<List<Pos>> shortestPathBatched(Pos p_origin, List<Pos> targetPositions)
	{	
		try {
			Vertex source = vertex_map[p_origin.x, p_origin.y];
			List<Vertex> targetVerts = new List<Vertex>();

			foreach (Pos p in targetPositions) {
				if (p != p_origin && vertex_map[p.x, p.y] != null) {
					targetVerts.Add(vertex_map[p.x, p.y]);
				}
			}

			// create a temporary graph of vertices to be pulled from during pathfinding
			List<Vertex> tmp_graph = new List<Vertex>(nav_graph);
			
			source.dist = 0;
			Vertex min_vert = pop_min_dist_vert(tmp_graph);
			
			// uses dijkstra method to find minimum distance from origin to target
			while (tmp_graph.Count > 0) {
				foreach (Vertex neighbor in min_vert.visible) {
					int alt_dist = min_vert.dist + Pos.abs_dist(min_vert.pos, neighbor.pos);
					if (alt_dist < neighbor.dist) {
						neighbor.dist = alt_dist;
						neighbor.prev = min_vert;
					}
				}
				min_vert = pop_min_dist_vert(tmp_graph);
			}
			
			// construct paths leading back to source
			List<List<Pos>> paths = new List<List<Pos>>();
			foreach (Vertex target in targetVerts)
				if (target.prev != null) paths.Add(constructPath(target, source));
			
			return paths;
		}
		finally {
			// post path-finding cleanup
			foreach(Vertex vertex in nav_graph)
				vertex.reset();
		}
	}
	
	Vertex pop_min_dist_vert_in_range(List<Vertex> graph, int maxDistance)
	{
		Vertex min = graph[0];
		for (int i = 1; i < graph.Count; i++) {
			if (graph[i].dist < min.dist && graph[i].dist < maxDistance)
				min = graph[i];
		}
		if (min.dist == Vertex.MAX_DIST)
			min = null;
		graph.Remove(min);
		return min;
	}
	
	// finds an entire batch of paths, but with a specified max distance to search
	// this can be much faster than shortestPathBatched
	public List<List<Pos>> shortestPathBatchedInRange(Pos p_origin, List<Pos> targetPositions, int maxDistance)
	{	
		try {
			Vertex source = vertex_map[p_origin.x, p_origin.y];
			if (source == null) return null;
			List<Vertex> targetVerts = new List<Vertex>();

			foreach (Pos p in targetPositions) {
				if (p != p_origin && vertex_map[p.x, p.y] != null) {
					targetVerts.Add(vertex_map[p.x, p.y]);
				}
			}

			// create a temporary graph of vertices to be pulled from during pathfinding
			List<Vertex> tmp_graph = new List<Vertex>();
			foreach (Vertex vert in nav_graph) {
				if (Pos.abs_dist(vert.pos, p_origin) <= maxDistance)
					tmp_graph.Add(vert);
			}
			
			source.dist = 0;
			Vertex min_vert = pop_min_dist_vert_in_range(tmp_graph, maxDistance);
			
			// uses dijkstra method to find minimum distance from origin to target
			while (tmp_graph.Count > 0 && min_vert != null) {
				foreach (Vertex neighbor in min_vert.visible) {
					int alt_dist = min_vert.dist + Pos.abs_dist(min_vert.pos, neighbor.pos);
					if (alt_dist < neighbor.dist) {
						neighbor.dist = alt_dist;
						neighbor.prev = min_vert;
					}
				}
				min_vert = pop_min_dist_vert_in_range(tmp_graph, maxDistance);
			}
			
			// construct paths leading back to source
			List<List<Pos>> paths = new List<List<Pos>>();
			foreach (Vertex target in targetVerts)
				if (target.prev != null) paths.Add(constructPath(target, source));
			
			return paths;
		}
		finally {
			// post path-finding cleanup
			foreach(Vertex vertex in nav_graph)
				vertex.reset();
		}
	}
	
	List<Pos> constructPath(Vertex target, Vertex source)
	{
		Stack<Pos> path = new Stack<Pos>();
		Vertex curr_vert = target;
		while (curr_vert != null && curr_vert != source) {
			path.Push(curr_vert.pos);
			curr_vert = curr_vert.prev;
		}
		path.Push(source.pos);
		return clean_up_path(path);
	}
	
	List<Pos> clean_up_path(Stack<Pos> path)
	{
		List<Pos> new_path = new List<Pos>();
		Pos p1 = path.Pop(), p2, mp;
		new_path.Add(p1);
		
		while (path.Count > 0) {
			p2 = path.Pop();
			mp = get_valid_midpoint(p1, p2);
			if (mp != null)
				new_path.Add(mp);
			new_path.Add(p2);
			p1 = p2;
		}
		
		return new_path;
	}
	
	Pos get_valid_midpoint(Pos p1, Pos p2)
	{
		if (p1.x == p2.x || p1.y == p2.y)
			return null;
		
		for (int x = p1.x; x != p2.x; x += Math.Sign(p2.x - p1.x))
			if (!traversable(map[x, p1.y]))
				return new Pos(p1.x, p2.y);
		for (int y = p1.y; y != p2.y; y += Math.Sign(p2.y - p1.y))
			if (!traversable(map[p2.x, y]))
				return new Pos(p1.x, p2.y);
			
		return new Pos(p2.x, p1.y);
	}
	
	void display_vertices()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (vertex_map[x, y] != null) {
					Vector3 pos = new Vector3(-width/2 + x + .5f, 0f, -height/2 + y + .5f);
					Debug.DrawLine(pos, pos + Vector3.up, Color.green, 2f, false);
				}
			}
		}
	}
	
	void print_debug_info()
	{
		Stack<Vertex> connected_graph = new Stack<Vertex>();
		connected_graph.Push(nav_graph[0]);
		
		float verts = 0;
		float edges = 0;
		while (connected_graph.Count > 0) {
			verts++;
			Vertex current = connected_graph.Pop();
			draw_lines_to_neighbors(current);
			foreach (Vertex vertex in current.visible) {
				edges++;
				if (!vertex.visited) {
					vertex.visited = true;
					connected_graph.Push(vertex);
				}
			}
			current.visited = true;
		}
		
		foreach(Vertex vertex in nav_graph)
			vertex.reset();
			
		Debug.Log("Verts: " + verts + ", Edges: " + edges);
		if (verts < nav_graph.Count) {
			Debug.Log("Some areas are not connected!");
		}
		
		Debug.Log("Curr E/V ratio: " + (edges / verts));
		Debug.Log("Avg E/V ratio: " + (total_edges / total_verts));
	}
	
	void draw_lines_to_neighbors(Vertex v)
	{
		Vector3 origin = new Vector3(-width/2 + v.pos.x + .5f, 0f, -height/2 + v.pos.y + .5f);
		foreach(Vertex n in v.visible) {
			Vector3 neighbor = new Vector3(-width/2 + n.pos.x + .5f, 0f, -height/2 + n.pos.y + .5f);
			Debug.DrawLine(origin, neighbor, Color.white, 1.5f, false);
		}
	}
}