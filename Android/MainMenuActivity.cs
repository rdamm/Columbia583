using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;

using Xamarin.Auth;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Columbia583.Android
{
	[Activity (Label = "Columbia583.Android_Main_Menu", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainMenuActivity : AndroidActivity
	{
		protected Button searchTrailsButton = null;
		protected Button debugAndTestsButton = null;
		protected Button loginButton = null;
		protected Button logoutButton = null;
		protected TextView loggedInUsersUsernameText = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.Main);

			// Get the controls.
			searchTrailsButton = FindViewById<Button> (Resource.Id.button_searchTrails);
			debugAndTestsButton = FindViewById<Button> (Resource.Id.button_debugAndTests);
			loginButton = FindViewById<Button> (Resource.Id.button_login);
			logoutButton = FindViewById<Button> (Resource.Id.button_logout);
			loggedInUsersUsernameText = FindViewById<TextView> (Resource.Id.txtLoggedInUsersUsername);

			// Get the logged in user and show their username.
			Account account = getLoggedInUser ();
			if (account != null) {
				// Show the username.
				displayUsername (account);
			} else {
				// Clear the username.
				clearUsername ();
			}
			
			// Assign the event handlers.
			if (searchTrailsButton != null) {
				searchTrailsButton.Click += (sender, e) => {

					// Load the search trails page.
					var intent = new Intent(this, typeof(SearchTrailsActivity));
					StartActivity(intent);

				};
			}
			if (debugAndTestsButton != null) {
				debugAndTestsButton.Click += (sender, e) => {

					// Load the debug and tests page.
					var intent = new Intent(this, typeof(DebugMenuActivity));
					StartActivity(intent);

				};
			}
			if (loginButton != null) {
				loginButton.Click += requestLoginEvent;
			}
			if (logoutButton != null) {
				logoutButton.Click += logoutEvent;
			}
		}


		/// <summary>
		/// Requests a Facebook login page, and calls the response login event after it completes.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void requestLoginEvent(object sender, EventArgs eventArgs)
		{
			// Create an OAuth authentication request.  Add email to the scope to include it in the account request permissions.
			var auth = new OAuth2Authenticator (
				clientId: "370094413194090",
				scope: "email",
				authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
				redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

			// Open the login screen.
			StartActivity (auth.GetUI (this));

			// Once the login screen has completed, run this event.
			auth.Completed += responseLoginEvent;
		}


		/// <summary>
		/// Called when the login request returns to the application.  This will store the account and
		/// display its username if it is authenticated, and destroy the account and clear the username
		/// otherwise.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void responseLoginEvent(object sender, AuthenticatorCompletedEventArgs eventArgs)
		{
			// Get the account from the login response arguments.
			Account account = eventArgs.Account;

			// Destroy all other Facebook logins.  This does not support multiple login storage.
			destroyOauthLogins();

			// If the user was logged in successfully, store their login info and update the username.
			// Otherwise, destroy their login info and clear the username.
			if (eventArgs.IsAuthenticated)
			{
				if (account != null) {
					// Store their login.
					AccountStore accountStore = AccountStore.Create (this);
					accountStore.Save (account, "Facebook");

					// Update the username display.
					displayUsername (eventArgs.Account);
				} else {
					// Clear the username display.
					clearUsername ();
				}
			}
			else
			{
				// Destroy their login.
				if (account != null) {
					AccountStore accountStore = AccountStore.Create (this);
					accountStore.Delete (account, "Facebook");
				}

				// Clear the username display.
				clearUsername ();
			}
		}


		/// <summary>
		/// Logs out the user.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void logoutEvent(object sender, EventArgs eventArgs)
		{
			// Destroy all Facebook logins.
			destroyOauthLogins();

			// Clear the username display.
			clearUsername();
		}


		/// <summary>
		/// Gets the logged in user.
		/// </summary>
		/// <returns>The logged in user.</returns>
		protected Account getLoggedInUser()
		{
			// TODO: Get the username from the local database instead.  OAuth requests cannot be made when out of
			// network range.

			// Get the user's login info and show their username.
			AccountStore accountStore = AccountStore.Create (this);
			IEnumerable<Account> accounts = accountStore.FindAccountsForService ("Facebook");
			Account account = null;
			foreach(Account a in accounts)
			{
				// TODO: Find a way to get the first element.
				account = a;
				break;
			}

			return account;
		}


		/// <summary>
		/// Displays the username of the given account.  Will clear the username if the account is null, or if
		/// the username response request fails.
		/// </summary>
		/// <param name="account">Account.</param>
		protected void displayUsername(Account account)
		{
			if (account != null)
			{
				// Get the user's profile info.
				var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, account);
				request.GetResponseAsync().ContinueWith (t => {
					// If the response is invalid, then act as if the login failed.
					if (t.IsFaulted)
					{
						Console.WriteLine ("Error: " + t.Exception.InnerException.Message);
						clearUsername ();
					}
					else
					{
						// Get the user's info from the response.
						string json = t.Result.GetResponseText();
						OAuthUser userProfile = JsonConvert.DeserializeObject<OAuthUser>(json);

						// Get the user's email from the profile.
						string userEmail = userProfile.email;

						// Get the user from the local users.  If the user doesn't exist, create it.
						Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
						User user = dataAccessLayerCommon.getUserByEmail(userEmail);
						if (user == null)
						{
							// Create the user.
							user = new User(0, 0, userEmail, userProfile.name, DateTime.Now, DateTime.Now, true);
							Data_Access_Layer_Upload dataAccessLayerUpload = new Data_Access_Layer_Upload();
							dataAccessLayerUpload.uploadUser(user);
						}

						// Set the active user in the app globals.
						dataAccessLayerCommon.setActiveUser(user.id);

						// Update the username view.
						RunOnUiThread (() => loggedInUsersUsernameText.Text = "Logged in as " + userProfile.name + ".");

						// Show the logout button.
						RunOnUiThread (() => loginButton.Visibility = ViewStates.Invisible);
						RunOnUiThread (() => logoutButton.Visibility = ViewStates.Visible);
					}
				});
			}
			else
			{
				clearUsername ();
			}
		}


		/// <summary>
		/// Clears the username view.
		/// </summary>
		protected void clearUsername()
		{
			// Change the username view.
			RunOnUiThread (() => loggedInUsersUsernameText.Text = "Not logged in.");

			// Show the login button.
			loginButton.Visibility = ViewStates.Visible;
			logoutButton.Visibility = ViewStates.Invisible;
		}


		/// <summary>
		/// Destroys the OAuth logins.
		/// </summary>
		protected void destroyOauthLogins()
		{
			// Destroy all stored OAuth Facebook logins.
			AccountStore accountStore = AccountStore.Create (this);
			IEnumerable<Account> accounts = accountStore.FindAccountsForService ("Facebook");
			foreach (Account a in accounts)
			{
				accountStore.Delete (a, "Facebook");
			}

			// Destroy the active user in the app globals.
			Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
			dataAccessLayerCommon.setActiveUser (0);
		}
	}
}