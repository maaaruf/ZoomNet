using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ZoomNet.IntegrationTests.Tests;

namespace ZoomNet.IntegrationTests
{
	internal class TestsRunner
	{
		private const int MAX_ZOOM_API_CONCURRENCY = 5;
		private const int TEST_NAME_MAX_LENGTH = 25;
		private const string SUCCESSFUL_TEST_MESSAGE = "Completed successfully";

		private enum ResultCodes
		{
			Success = 0,
			Exception = 1,
			Cancelled = 1223
		}

		private enum ConnectionMethods
		{
			Jwt = 0,
			OAuth = 1
		}

		private readonly ILoggerFactory _loggerFactory;

		public TestsRunner(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
		}

		public async Task<int> RunAsync()
		{
			// -----------------------------------------------------------------------------
			// Do you want to proxy requests through Fiddler? Can be useful for debugging.
			var useFiddler = true;
			var fiddlerPort = 8888; // By default Fiddler4 uses port 8888 and Fiddler Everywhere uses port 8866

			// Do you want to use JWT or OAuth?
			var connectionMethod = ConnectionMethods.OAuth;
			// -----------------------------------;------------------------------------------

			// Configure ZoomNet client
			IConnectionInfo connectionInfo;
			if (connectionMethod == ConnectionMethods.Jwt)
			{
				var apiKey = Environment.GetEnvironmentVariable("ZOOM_JWT_APIKEY", EnvironmentVariableTarget.User);
				var apiSecret = Environment.GetEnvironmentVariable("ZOOM_JWT_APISECRET", EnvironmentVariableTarget.User);
				connectionInfo = new JwtConnectionInfo(apiKey, apiSecret);
			}
			else
			{
				var clientId = Environment.GetEnvironmentVariable("ZOOM_OAUTH_CLIENTID", EnvironmentVariableTarget.User);
				var clientSecret = Environment.GetEnvironmentVariable("ZOOM_OAUTH_CLIENTSECRET", EnvironmentVariableTarget.User);
				var accountId = Environment.GetEnvironmentVariable("ZOOM_OAUTH_ACCOUNTID", EnvironmentVariableTarget.User);
				var refreshToken = Environment.GetEnvironmentVariable("ZOOM_OAUTH_REFRESHTOKEN", EnvironmentVariableTarget.User);
				var accessToken = Environment.GetEnvironmentVariable("ZOOM_OAUTH_ACCESSTOKEN", EnvironmentVariableTarget.User);

				// Server-to-Server OAuth
				if (!string.IsNullOrEmpty(accountId))
				{
					connectionInfo = new OAuthConnectionInfo(clientId, clientSecret, accountId,
						(_, newAccessToken) =>
						{
							Console.Out.WriteLine($"A new access token was issued: {newAccessToken}");
						});
				}

				// Standard OAuth
				else
				{
					connectionInfo = new OAuthConnectionInfo(clientId, clientSecret, refreshToken, accessToken,
						(newRefreshToken, newAccessToken) =>
						{
							Environment.SetEnvironmentVariable("ZOOM_OAUTH_REFRESHTOKEN", newRefreshToken, EnvironmentVariableTarget.User);
							Environment.SetEnvironmentVariable("ZOOM_OAUTH_ACCESSTOKEN", newAccessToken, EnvironmentVariableTarget.User);
						});

					//var authorizationCode = "<-- the code generated by Zoom when the app is authorized by the user -->";
					//connectionInfo = new OAuthConnectionInfo(clientId, clientSecret, authorizationCode,
					//	(newRefreshToken, newAccessToken) =>
					//	{
					//		Environment.SetEnvironmentVariable("ZOOM_OAUTH_REFRESHTOKEN", newRefreshToken, EnvironmentVariableTarget.User);
					//		Environment.SetEnvironmentVariable("ZOOM_OAUTH_ACCESSTOKEN", newAccessToken, EnvironmentVariableTarget.User);
					//	});
				}
			}

			var proxy = useFiddler ? new WebProxy($"http://localhost:{fiddlerPort}") : null;
			var client = new ZoomClient(connectionInfo, proxy, null, _loggerFactory.CreateLogger<ZoomClient>());

			// Configure Console
			var source = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) =>
			{
				e.Cancel = true;
				source.Cancel();
			};

			// Ensure the Console is tall enough and centered on the screen
			if (OperatingSystem.IsWindows()) Console.WindowHeight = Math.Min(60, Console.LargestWindowHeight);
			ConsoleUtils.CenterConsole();

			// These are the integration tests that we will execute
			var integrationTests = new Type[]
			{
				typeof(Accounts),
				typeof(Chat),
				typeof(CloudRecordings),
				typeof(Contacts),
				typeof(Dashboards),
				typeof(Meetings),
				typeof(Roles),
				typeof(Users),
				typeof(Webinars),
				typeof(Reports)
			};

			// Get my user and permisisons
			var myUser = await client.Users.GetCurrentAsync(source.Token).ConfigureAwait(false);
			var myPermissions = await client.Users.GetCurrentPermissionsAsync(source.Token).ConfigureAwait(false);

			// Execute the async tests in parallel (with max degree of parallelism)
			var results = await integrationTests.ForEachAsync(
				async testType =>
				{
					var log = new StringWriter();

					try
					{
						var integrationTest = (IIntegrationTest)Activator.CreateInstance(testType);
						await integrationTest.RunAsync(myUser, myPermissions, client, log, source.Token).ConfigureAwait(false);
						return (TestName: testType.Name, ResultCode: ResultCodes.Success, Message: SUCCESSFUL_TEST_MESSAGE);
					}
					catch (OperationCanceledException)
					{
						await log.WriteLineAsync($"-----> TASK CANCELLED").ConfigureAwait(false);
						return (TestName: testType.Name, ResultCode: ResultCodes.Cancelled, Message: "Task cancelled");
					}
					catch (Exception e)
					{
						var exceptionMessage = e.GetBaseException().Message;
						await log.WriteLineAsync($"-----> AN EXCEPTION OCCURRED: {exceptionMessage}").ConfigureAwait(false);
						return (TestName: testType.Name, ResultCode: ResultCodes.Exception, Message: exceptionMessage);
					}
					finally
					{
						lock (Console.Out)
						{
							Console.Out.WriteLine(log.ToString());
						}
					}
				}, MAX_ZOOM_API_CONCURRENCY)
			.ConfigureAwait(false);

			// Display summary
			var summary = new StringWriter();
			await summary.WriteLineAsync("\n\n**************************************************").ConfigureAwait(false);
			await summary.WriteLineAsync("******************** SUMMARY *********************").ConfigureAwait(false);
			await summary.WriteLineAsync("**************************************************").ConfigureAwait(false);

			foreach (var (TestName, ResultCode, Message) in results.OrderBy(r => r.TestName).ToArray())
			{
				var name = TestName.Length <= TEST_NAME_MAX_LENGTH ? TestName : TestName.Substring(0, TEST_NAME_MAX_LENGTH - 3) + "...";
				await summary.WriteLineAsync($"{name.PadRight(TEST_NAME_MAX_LENGTH, ' ')} : {Message}").ConfigureAwait(false);
			}

			await summary.WriteLineAsync("**************************************************").ConfigureAwait(false);
			await Console.Out.WriteLineAsync(summary.ToString()).ConfigureAwait(false);

			// Prompt user to press a key in order to allow reading the log in the console
			var promptLog = new StringWriter();
			await promptLog.WriteLineAsync("\n\n**************************************************").ConfigureAwait(false);
			await promptLog.WriteLineAsync("Press any key to exit").ConfigureAwait(false);
			ConsoleUtils.Prompt(promptLog.ToString());

			// Return code indicating success/failure
			var resultCode = (int)ResultCodes.Success;
			if (results.Any(result => result.ResultCode != ResultCodes.Success))
			{
				if (results.Any(result => result.ResultCode == ResultCodes.Exception)) resultCode = (int)ResultCodes.Exception;
				else if (results.Any(result => result.ResultCode == ResultCodes.Cancelled)) resultCode = (int)ResultCodes.Cancelled;
				else resultCode = (int)results.First(result => result.ResultCode != ResultCodes.Success).ResultCode;
			}

			return await Task.FromResult(resultCode);
		}
	}
}
