using System;
using TreeMapper;
using System.Drawing;

namespace TreeMapperTester
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			BoundingBox boundingBox = new BoundingBox ();
			
			boundingBox.MinimumLatitude = -84.774950;
			boundingBox.MinimumLongitude = 38.264822;
			boundingBox.MaximumLatitude = -84.980892;
			boundingBox.MaximumLongitude = 38.103126;

			Console.WriteLine (boundingBox.MinimumLatitude);
			Console.WriteLine (boundingBox.MaximumLatitude);

			Bitmap bitmap = TreeCoverClient.LoadTreeCover (boundingBox);

			bitmap.Save (@"C:\TreeCoverTest\Imported.png");
			Console.WriteLine ("Hello World!");
		}
	}
}
