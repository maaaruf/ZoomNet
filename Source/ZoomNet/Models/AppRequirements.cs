using System.Text.Json.Serialization;

namespace ZoomNet.Models
{
	/// <summary>
	/// App requirements.
	/// </summary>
	public class AppRequirements
	{
		/// <summary>Gets or sets the user roles required to authorize or add the app.</summary>
		[JsonPropertyName("userRole")]
		public string UserRole { get; set; }

		/// <summary>Gets or sets the minimum client version required for the app.</summary>
		[JsonPropertyName("min_client_version")]
		public string MinimumClientVersion { get; set; }

		/// <summary>Gets or sets the eligibility requirements for app.</summary>
		[JsonPropertyName("accountEligibility")]
		public AppEligibilityRequirements EligibilityRequirements { get; set; }
	}
}
