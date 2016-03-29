using System;
using System.Net;
using System.IO;
using System.Drawing;

namespace TreeMapper
{
	public static class TreeCoverClient
	{
		public const string TREE_COVER_SERVER_ADDRESS = "http://ec2-50-18-182-188.us-west-1.compute.amazonaws.com:6080/arcgis/rest/services/TreeCover2000/ImageServer/exportImage?f=image&bbox=";
		
		public static Bitmap LoadTreeCover(BoundingBox BoundingBox)
		{
			Bitmap image_bitmap;
			
			string uri = GetTreeCoverUri(BoundingBox);
			
			using (Stream image_stream = GetImageStream (uri)) 
			{
				image_bitmap = new Bitmap(image_stream);
			}

			return image_bitmap;
		}
		
		public static Stream GetImageStream(string URI)
		{
			Stream stream = null;
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (URI);
			
			String response_string = string.Empty;
			HttpWebResponse web_response = (HttpWebResponse)request.GetResponse ();
			
			stream = web_response.GetResponseStream ();

			return stream;
		}
		
		public static string GetTreeCoverUri(BoundingBox BoundingBox)
		{
			string uri;
			
			int  imageSR=4326;
			int  bboxSR=4326;
			int size = 1081;
			
			string boundingBoxParameter = BoundingBox.GetUriString ();
			
			uri = string.Format ("{0}{1}&imageSR={2}&bboxSR={3}&size={4},{4}", TREE_COVER_SERVER_ADDRESS, boundingBoxParameter, imageSR, bboxSR, size);
			return uri;
		}
		
	}
}

