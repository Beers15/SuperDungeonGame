using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.Dir;
using static MapUtils.Type;
using static MapUtils.EnumUtils;

namespace LoopExtractor
{
	/* Creates a "turtle" that walks around the perimeter of each region
	 * and generates commands that describe the geometry of that perimeter.
	 * This allows detailed 3D walls to be created in-game.
	 *
	 * Since this turtle generates a loop, UV coordinates can be continuous
	 * around the entirety of the perimeter. This also opens the perimeter
	 * walls to more sophisticated effects, like fractal or perlin deformations
	 * that stay continuous around the perimeter.
	 * 
	 * It's important that all loops are closed in the given map, otherwise
	 * there will probably be an IndexOutOfBounds exception, because no bounds
	 * checking occurs within the turtle
	 */
	public class LoopTurtle
	{
		private int [,] map;
		private Pos pos;
		private Dir dir;
		private bool ccw;
		private int BEDROCK;
		private int width;
		private int height;
		
		public List<Cmd> cmdlist;
		
		public LoopTurtle(int[,] map, Pos pos, int BEDROCK, bool ccw)
		{
			this.map = map;
			this.pos = pos;
			this.BEDROCK = BEDROCK; // the tile to avoid running into
			this.width = map.GetLength(0);
			this.height = map.GetLength(1);
			this.ccw = ccw;
			
			if      (map[pos.x-1, pos.y] == BEDROCK) {
				dir = UP;
				if (ccw)
					this.pos.x -= 1; // ccw turtles want to swim in the bedrock, whereas non-ccw turtles want to avoid it
			}
			else if (map[pos.x, pos.y-1] == BEDROCK) {
				dir = RIGHT;
				if (ccw)
					this.pos.y -= 1;
			}
			else if (map[pos.x+1, pos.y] == BEDROCK) {
				dir = DOWN;
				if (ccw)
					this.pos.x += 1;
			}
			else if (map[pos.x, pos.y+1] == BEDROCK) {
				dir = LEFT;
				if (ccw)
					this.pos.y += 1;
			}
			
			cmdlist = new List<Cmd>();
			if (ccw)
				cmdlist.Add(new Cmd(dir.left(2), this.pos, LINE));
			else
				cmdlist.Add(new Cmd(dir, this.pos, LINE));
			
			advance_until_loop();
			collapse_cmds();
		}
		
		private void advance_until_loop()
		{
			Cmd top = cmdlist[0];
			if (ccw)
				advance_CCW();
			else
				advance_CW();
			while (cmdlist[0] != top) {
				if (ccw)
					advance_CCW();
				else
					advance_CW();
			}
			cmdlist.RemoveAt(0);
		}
		
		private void collapse_cmds()
		{
			int len = cmdlist.Count;
			
			// if the turtle is run counter-clockwise, they need to be collapsed in the opposite order
			if (ccw)
				cmdlist.Reverse();
			
			for (int i = len - 1; i >= 0; i--) {
				
				int j = 1;
				while (is_ccw(cmdlist[i].dir, cmdlist[(i+j)%len].dir, j)) {
					cmdlist[i].type = (Type)((int)cmdlist[i].type + 1);
					cmdlist[(i+j)%len].type = INVALID;
					j++;
				}
			}
			
			List<Cmd> new_cmds = new List<Cmd>();
			for (int i = len - 1; i >= 0; i--) {
				if (cmdlist[i].type != INVALID) {
					new_cmds.Add(cmdlist[i]);
				}
			}
			cmdlist.Clear();
			
			// if the commands aren't reversed again, the walls will be rendered the wrong way around, making them un-seeable
			if (ccw)
				new_cmds.Reverse();
			
			cmdlist = new_cmds;
		}
		
		// checks if d2 is amt*90 degrees counter-clockwise from d1
		private bool is_ccw(Dir d1, Dir d2, int amt)
		{
			return d2.right(amt) == d1;
		}
		
		private void advance_CW()
		{
			if (detect_forward()) { // if tile is in front of turtle, turn clockwise
				if (detect_left() && !detect_forward_left()) { // when advancing, the turtle can traverse across pinching points if we don't check for them
					pos += forward_dict(dir) + left_dict(dir); 
					turn_left();
				}
				else {
					turn_right();
				}
			}
			else if (detect_forward_left()) { // else, if no tile is in front of turtle, and there is a tile to the forward left, move forward and maintain direction
				pos += forward_dict(dir);
			}
			else if (detect_left()) { // else, if there is only a tile to the left, move diagonally and turn counter-clockwise
				pos += forward_dict(dir) + left_dict(dir); 
				turn_left();
			}
			cmdlist.Insert(0, new Cmd(dir, pos, LINE));
		}
		
		private void advance_CCW()
		{
			if (detect_forward()) { // if tile is in front of turtle, turn counter-clockwise
				if (detect_right() && !detect_forward_right()) { // when advancing, the turtle can traverse across pinching points if we don't check for them
					pos += forward_dict(dir) + right_dict(dir); 
					turn_right();
				}
				else {
					turn_left();
				}
			}
			else if (detect_forward_right()) { // else, if no tile is in front of turtle, and there is a tile to the forward right, move forward and maintain direction
				pos += forward_dict(dir);
			}
			else if (detect_right()) { // else, if there is only a tile to the right, move diagonally and turn clockwise
				pos += forward_dict(dir) + right_dict(dir); 
				turn_right();
			}
			cmdlist.Insert(0, new Cmd(dir.left(2), pos, LINE));
		}
		
		private void turn_right()
		{
			dir = dir.right();
		}
		
		private void turn_left()
		{
			dir = dir.left();
		}
		
		private Pos forward_dict(Dir dir)
		{
			switch (dir)
			{
				case LEFT  : return Pos.LEFT;
				case UP    : return Pos.UP;
				case RIGHT : return Pos.RIGHT;
				case DOWN  : return Pos.DOWN;
				
				// this shouldn't happen
				default : return null;
			}
		}
		
		private Pos left_dict(Dir dir)
		{
			switch (dir)
			{
				case LEFT  : return Pos.DOWN;
				case UP    : return Pos.LEFT;
				case RIGHT : return Pos.UP;
				case DOWN  : return Pos.RIGHT;
				
				// this shouldn't happen
				default : return null;
			}
		}
		
		private Pos right_dict(Dir dir)
		{
			switch (dir)
			{
				case LEFT  : return Pos.UP;
				case UP    : return Pos.RIGHT;
				case RIGHT : return Pos.DOWN;
				case DOWN  : return Pos.LEFT;
				
				// this shouldn't happen
				default : return null;
			}
		}
		
		private bool detect_forward()
		{
			Pos check_pos = pos + forward_dict(dir);
			if (map[check_pos.x, check_pos.y] == BEDROCK) {
				return !ccw;  // ccw turtles "swim" in BEDROCK, whereas non-ccw turtles avoid BEDROCK
			}
			return ccw;
		}
		
		private bool detect_forward_left()
		{
			Pos check_pos = pos + forward_dict(dir) + left_dict(dir);
			if (map[check_pos.x, check_pos.y] == BEDROCK) {
				return !ccw;
			}
			return ccw;
		}
		
		private bool detect_forward_right()
		{
			Pos check_pos = pos + forward_dict(dir) + right_dict(dir);
			if (map[check_pos.x, check_pos.y] == BEDROCK) {
				return !ccw;
			}
			return ccw;
		}
		
		private bool detect_left()
		{
			Pos check_pos = pos + left_dict(dir);
			if (map[check_pos.x, check_pos.y] == BEDROCK) {
				return !ccw;
			}
			return ccw;
		}
		
		private bool detect_right()
		{
			Pos check_pos = pos + right_dict(dir);
			if (map[check_pos.x, check_pos.y] == BEDROCK) {
				return !ccw;
			}
			return ccw;
		}
		
	}
}
