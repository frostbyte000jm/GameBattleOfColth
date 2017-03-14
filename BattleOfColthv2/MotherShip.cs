using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public class MotherShip : Enemy
	{
		//declarations
		private Vector3 position, velocity, powerStnPos;
		private float shipHeight, shipWidth, speed, rotation;
		private List<Enemy> enemyList;
		private PowerStation powerstation;
		private bool point1;
		private int stpDist;
		
		public MotherShip (GraphicsContext g, Texture2D tex, PowerStation pStn, List<Enemy> eList, int hp, int st, int x, int y):base(g, tex, hp, st)
		{
			shipWidth = sprite.Width;
			shipHeight = sprite.Height; // this may need to be killed later. 
			position = new Vector3(x,y,0); // you are going to adjust here to move it off screen.
			sprite.Position = position;
			sprite.Center = new Vector2 (.5f,.5f);
			speed = .5f;
			velocity = new Vector3 (0,0,0);
			powerstation = pStn;
			enemyList = eList;
			rotation =0;
			point1 = false;
			stpDist = 0;
		}
		
		public override void Move ()
		{
			powerStnPos = powerstation.Position;
			if(point1 != true)
				powerStnPos.Y = shipHeight/2;
            
			// Step 2 - move towards the player by calculating the vector between the Boid and the player
			Vector3 diff = Vector3.Subtract (powerStnPos, position);
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
			
			if (point1)
				stpDist = 15000;
			else
				stpDist = 100;
			if(Vector3.DistanceSquared (position, powerStnPos) < stpDist)
			{
				velocity *= 0;
				if(point1 == false)
					point1=true;
			}
			// Update the position of the Boid based on its velocity
			position += velocity;
			sprite.Position = position;
			sprite.Rotation = rotation;
		}
		
		public override bool DoAttack()
		{
			Vector3 targetPos = powerstation.Position;
			bool doShoot = false;
			
			if(Vector3.DistanceSquared (Pos, targetPos) < 50000)
			{
				doShoot = true;
			}
			
			return doShoot;
		}
		
		public override void Render ()
		{
			sprite.Render ();
		}
	}
}

