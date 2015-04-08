using System;
using System.Threading;
using Android.App;
using Android.Content;

namespace Columbia583.Android
{
	[Service]
	public class DatabaseUpdateService : Service
	{
		protected Timer updateCheckTimer = null;

		// Update constants.
		protected const uint updateTimerPeriod = 3600000;		// 1 hour
		protected const int updatePeriod = 1;					// 1 day
		protected const int acceptableUpdatePeriodStart = 1;	// 1:00 AM
		protected const int acceptableUpdatePeriodEnd = 4;		// 4:00 AM

		public DatabaseUpdateService ()
		{
		}


		public override void OnStart (global::Android.Content.Intent intent, int startId)
		{
			base.OnStart (intent, startId);

			// Run the timer.
			DoStuff ();
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();

			// Destroy the timer.
			updateCheckTimer.Dispose ();
		}

		public void DoStuff ()
		{
			updateCheckTimer = new Timer ((o) => {
				TimerCheck();
			}, null, 0, updateTimerPeriod);
		}

		public override global::Android.OS.IBinder OnBind (global::Android.Content.Intent intent)
		{
			throw new NotImplementedException ();
		}

		public void TimerCheck()
		{
			// Get the time the database was last updated.
			Data_Layer_App_Globals dataLayerAppGlobals = new Data_Layer_App_Globals();
			DateTime databaseLastUpdated = dataLayerAppGlobals.getDatabaseLastUpdated ();

			// Determine the number of days since the database was last updated.
			DateTime currentTime = DateTime.Now;
			int daysSinceLastUpdate = (int)Math.Floor((currentTime - databaseLastUpdated).TotalDays);

			// If the database is sufficiently out of date, and the current time is within the acceptable
			// update window, update the database.
			if (daysSinceLastUpdate >= 1 && currentTime.Hour > acceptableUpdatePeriodStart && currentTime.Hour < acceptableUpdatePeriodEnd)
			{
				// TODO: Check if WiFi is available.

				// TODO: Update the database.

			}
		}
	}
}

