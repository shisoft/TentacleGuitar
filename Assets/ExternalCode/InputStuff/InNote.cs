using System;

namespace AssemblyCSharp
{
	public class InNote
	{
		public InNote (int id, double amp)
		{
			this.Id = id;
			this.Amp = amp;
		}

		public int Id { get; set;}
		public double Amp { get; set;}
		public float Time { get; set;}

	}
}

