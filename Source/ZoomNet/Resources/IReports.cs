using System;
using System.Threading;
using System.Threading.Tasks;
using ZoomNet.Models;

namespace ZoomNet.Resources
{
	/// <summary>
	/// Allows you to view various metrics.
	/// </summary>
	/// <remarks>
	/// See <a href="https://marketplace.zoom.us/docs/api-reference/zoom-api/reports/">Zoom documentation</a> for more information.
	/// </remarks>
	public interface IReports
	{
		/// <summary>
		/// Get a list of participants from past meetings with two or more participants. To see a list of participants for meetings with one participant use <see cref="IDashboards.GetMeetingParticipantsAsync"/>.
		/// </summary>
		/// <param name="meetingId">The meeting ID or meeting UUID. If given the meeting ID it will take the last meeting instance.</param>
		/// <param name="pageSize">The number of records returned within a single API call.</param>
		/// <param name="pageToken">
		/// The next page token is used to paginate through large result sets.
		/// A next page token will be returned whenever the set of available results exceeds the current page size.
		/// The expiration period for this token is 15 minutes.
		/// </param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="ReportMeetingParticipant">participants</see>.
		/// </returns>
		Task<PaginatedResponseWithToken<ReportMeetingParticipant>> GetMeetingParticipantsAsync(string meetingId, int pageSize = 30, string pageToken = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get a list past meetings and webinars for a specified time period. The time range for the report is limited to a month and the month must fall within the past six months.
		/// </summary>
		/// <param name="userId">The user ID or email address of the user.</param>
		/// <param name="from">Start date.</param>
		/// <param name="to">End date.</param>
		/// <param name="type">The meeting type to query for.</param>
		/// <param name="pageSize">The number of records returned within a single API call.</param>
		/// <param name="pageToken">
		/// The next page token is used to paginate through large result sets.
		/// A next page token will be returned whenever the set of available results exceeds the current page size.
		/// The expiration period for this token is 15 minutes.
		/// </param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="PastMeeting">meetings.</see>.
		/// </returns>
		Task<PaginatedResponseWithToken<PastMeeting>> GetMeetingsAsync(string userId, DateTime from, DateTime to, ReportMeetingType type = ReportMeetingType.Past, int pageSize = 30, string pageToken = null, CancellationToken cancellationToken = default);
	}
}
