using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace BattleOfColthv2
{
	public class Guns : Weapon
	{
	//future update. when Ammo = 0 (after pickup) gun will fly off to random location on the screen. Pilot will need to go pick it up.	
		
		public Guns (GraphicsContext g, Texture2D tex, Vector3 p, float rot, int uid): base(g, tex, p, rot, uid)
		{
			Ammo = 0;
		}
		
		public Guns (GraphicsContext g, Texture2D tex, PlayerShip pl, int uid): base(g, tex, pl, uid)
		{
			Ammo = 10; 
			Damage = 1;
		}
		
		public override void Reload()
		{
			Ammo += 10;
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

