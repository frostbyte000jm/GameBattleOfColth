using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public class DropOff : Weapon
	{
	//future update. when Ammo = 0 (after pickup) gun will fly off to random location on the screen. Pilot will need to go pick it up.	
		
		public DropOff (GraphicsContext g, Texture2D tex, Vector3 p, float rot, int uid): base(g, tex, p, rot, uid)
		{
			Ammo = 0;
		}
		
		public DropOff (GraphicsContext g, Texture2D tex, PlayerShip pl, int uid): base(g, tex, pl, uid)
		{
			Ammo = 2; 
			Damage = 100;
		}
		
		public override void Reload()
		{
			Ammo += 2;
		}
		
		public override void Update()
		{
			sprite.Position = player.Position;
			sprite.Rotation = player.Rotation;
		}
		
		public override void Render()
		{
			sprite.Render();
		}
	}
}

