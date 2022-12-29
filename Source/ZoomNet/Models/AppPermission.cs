using System.Text.Json.Serialization;
using ZoomNet.Json;

namespace ZoomNet.Models
{
	/// <summary>
	/// An app's permission.
	/// </summary>
	public class AppPermission
	{
		/// <summary>Gets or sets the app's permission group..</summary>
		[JsonPropertyName("group")]
		public string Group { get; set; }

		/// <summary>Gets or sets the app's permission group message.</summary>
		[JsonPropertyName("groupMessage")]
		public string GroupMessage { get; set; }

		/// <summary>Gets or sets the app's group title.</summary>
		[JsonPropertyName("title")]
		public string Title { get; set; }

		/// <summary>Gets or sets the type of the app.</summary>
		[JsonPropertyName("permissions")]
		[JsonConverter(typeof(AppPermissionsConverter))]
		public string[] Permissions { get; set; }
	}
}
