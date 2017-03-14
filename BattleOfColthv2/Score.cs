using System;

namespace BattleOfColthv2
{
	public class Score: IComparable
	{
		//declaration
		private int playerScore;
		private string playerInitials;
		
		public int PlayerScore {get {return playerScore;} set{if(value>0) playerScore = value;}}
		public string PlayerInitials {get {return playerInitials;} set{playerInitials = value;}}
		
		public Score (int s, string pI)
		{
			playerScore = s;
			playerInitials = pI;
		}
		
		public int CompareTo(object obj)
		{
			int compInt = this.PlayerScore.CompareTo((obj as Score).PlayerScore);
			if(compInt ==0)
				compInt = -(this.PlayerInitials.CompareTo((obj as Score).PlayerInitials));
			return compInt;
		}
	}
}

