using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public class Guardian : Enemy
	{
		//declarations
		private Vector3 position, velocity, vectorPos;
		private float shipHeight, shipWidth, speed, rotation;
		private List<Enemy> enemyList;
		private PlayerShip playership;
		
		public Guardian (GraphicsContext g, Texture2D tex, PlayerShip pShip, List<Enemy> eList, int hp, int st, int x, int y):base(g, tex, hp, st)
		{
			shipWidth = sprite.Width;
			shipHeight = sprite.Height; // this may need to be killed later. 
			position = new Vector3(x,y,0); // you are going to adjust here to move it off screen.
			sprite.Position = position;
			sprite.Center = new Vector2 (.5f,.5f);
			speed = 1f;
			velocity = new Vector3 (0,0,0);
			playership = pShip;
			enemyList = eList;
			rotation =0;
		}
		
		public override void Move ()
		{
			//I want this to follow the MotherShip, but I am not sure of any good way to do this. 
			
			vectorPos = playership.Position;
			// Step 1 - avoid your neighbors
			avoidNeighbors(enemyList);
            
			// Step 2 - move towards the player by calculating the vector between the Boid and the player
			Vector3 diff = Vector3.Subtract (vectorPos, Pos);
			if (diff.Length () > 1) 
			{
				velocity += Vector3.Normalize (diff) / 10;
			}
            
			// Calculate the rotation angle of this Boid (for drawing) using Atan2
			if ((velocity.X != 0) || (velocity.Y != 0)) 
			{
				rotation = FMath.Atan2 (velocity.Y, velocity.X);
			}
			// Slow this Boid down if it's going too fast
			float velLength = velocity.Length ();
			if (velLength > speed) 
			{
				velocity = velocity.Normalize ();
				velocity *= speed;
			}
			//stop the motion of the ship.
			if(Vector3.DistanceSquared (Pos, vectorPos) < 5000)
			{
				velocity *= 0;
			}
			
			// Update the position of the Boid based on its velocity
			position += velocity;
			sprite.Position = position;
			sprite.Rotation = rotation;
		}
		
		public override bool DoAttack()
		{
			Vector3 targetPos = playership.Position;
			bool doShoot = false;
			
			if(Vector3.DistanceSquared (Pos, targetPos) < 80000)
			{
				doShoot = true;
			}
			
			return doShoot;
		}
		
		public override void Render ()
		{
			sprite.Render ();
		}
		
		//Help get out of the way of friends. 
		private void avoidNeighbors (List<Enemy> bList)
		{
			//declarations
			Vector3 avoidanceVector = new Vector3(0,0,0);
			int neighborCount = 0;
			
			
			// Loop through each boid in boidList remembering to exclude this boid
			for (int i = 0; i < bList.Count; i++)
			{
				if (this != bList[i])
				{
					float distBetween = Vector3.Distance(bList[i].Pos,this.Pos);
					if (distBetween<100) //if too close then...
					{
						neighborCount +=1;
						avoidanceVector += Vector3.Subtract(this.Pos,bList[i].Pos);
					}
				}
				if(neighborCount>0)
					velocity += avoidanceVector/5000;
			}
		}
	}
}

