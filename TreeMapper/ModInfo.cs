#if !(UNITY_4 || UNITY_5)
using ICities;

namespace TreeMapper {
	
	public class ModInfo : IUserMod {
		
		public string Name {
			get
			{ 
				return "Tree Mapper";
			}
		}
		
		public string Description {
			get 
			{
				return "Import Tree Cover into the map editor.";
			}
		}
	}
}
#endif