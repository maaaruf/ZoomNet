using System.Text.Json.Serialization;

namespace ZoomNet.Models
{
	/// <summary>
	/// App links.
	/// </summary>
	public class AppLinks
	{
		/// <summary>Gets or sets the app's documentation link.</summary>
		[JsonPropertyName("documentation_url")]
		public string Documentation { get; set; }

		/// <summary>Gets or sets the app's privacy policy link.</summary>
		[JsonPropertyName("privacy_policy_url")]
		public string PrivacyPolicy { get; set; }

		/// <summary>Gets or sets the app's support link.</summary>
		[JsonPropertyName("support_url")]
		public string Support { get; set; }

		/// <summary>Gets or sets the app's terms of use link.</summary>
		[JsonPropertyName("terms_of_use_url")]
		public string TermsOfUse { get; set; }
	}
}
