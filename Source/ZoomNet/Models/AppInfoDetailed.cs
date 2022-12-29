using System.Text.Json.Serialization;

namespace ZoomNet.Models
{
	/// <summary>
	/// Detailed information about a marketplace app.
	/// </summary>
	public class AppInfoDetailed : AppInfo
	{
		/// <summary>Gets or sets the app's description.</summary>
		[JsonPropertyName("app_description")]
		public string Description { get; set; }

		/// <summary>Gets or sets the app scopes.</summary>
		[JsonPropertyName("app_scopes")]
		public string[] Scopes { get; set; }

		/// <summary>Gets or sets the app requirements.</summary>
		[JsonPropertyName("app_requirements")]
		public AppRequirements Requirements { get; set; }

		/// <summary>Gets or sets the app permissions.</summary>
		[JsonPropertyName("app_permissions")]
		public AppPermission[] Permissions { get; set; }

		/// <summary>Gets or sets the app links.</summary>
		[JsonPropertyName("app_links")]
		public AppLinks Links { get; set; }
	}
}
