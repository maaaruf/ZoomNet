using System.Text.Json.Serialization;

namespace ZoomNet.Models
{
	/// <summary>
	/// Eligibility requirements for app.
	/// </summary>
	public class AppEligibilityRequirements
	{
		/// <summary>Gets or sets the account types.</summary>
		[JsonPropertyName("account_types")]
		public string AccountTypes { get; set; }

		/// <summary>Gets or sets the premium events.</summary>
		[JsonPropertyName("premiumEvents")]
		public PremiumEvent[] PremiumEvents { get; set; }
	}
}
