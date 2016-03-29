using System;

namespace TreeMapper
{
	public class BoundingBox
	{
		public double MinimumLatitude{ get; set;}
		public double MinimumLongitude{ get; set;}
		public double MaximumLatitude { get; set;}
		public double MaximumLongitude{ get; set;}
		
		public BoundingBox ()
		{
			MinimumLatitude = 0.0;
			MinimumLongitude = 0.0;
			MaximumLatitude = 0.0;
			MaximumLongitude = 0.0;
		}
		
		public string GetUriString()
		{
			string boundingBoxString = MinimumLatitude + "," + MinimumLongitude + "," + MaximumLatitude + "," + MaximumLongitude;
			return boundingBoxString;
		}
	}
}

