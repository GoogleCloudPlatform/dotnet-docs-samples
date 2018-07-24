using System.Reflection;
using System.Configuration;

namespace PetShop.ProfileDALFactory {
	/// <summary>
	/// This class is implemented following the Abstract Factory pattern to create the ProfileDAL implementation
	/// specified from the configuration file
	/// </summary>
	public sealed class DataAccess {

		private static readonly string profilePath = ConfigurationManager.AppSettings["ProfileDAL"];

		public static PetShop.IProfileDAL.IPetShopProfileProvider CreatePetShopProfileProvider() {
			string className = profilePath + ".PetShopProfileProvider";
			return (PetShop.IProfileDAL.IPetShopProfileProvider)Assembly.Load(profilePath).CreateInstance(className);
		}
	}
}
