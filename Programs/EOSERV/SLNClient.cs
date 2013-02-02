using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace EOHax.Programs.EOSERV
{
	class SLNClient
	{
		private Timer timer;

		public SLNClient()
		{
			timer = new Timer(new TimerCallback(this.CheckIn), new AutoResetEvent(false), 0, (long)Properties.Settings.Default.SLN_Time.TotalMilliseconds);
		}

		private void CheckIn(Object stateInfo)
		{
			Program.Logger.LogInfo("SLN checkin");
			String url = Properties.Settings.Default.SLN_URL + "check?software=Beemu&v=" + "1.0";
			url += String.Format("&retry={0}", HttpUtility.UrlEncode(Convert.ToString((long)Properties.Settings.Default.SLN_Time.TotalSeconds)));
			// TODO: host
			if (!String.IsNullOrEmpty(Properties.Settings.Default.Host))
				url += String.Format("&host={0}", HttpUtility.UrlEncode(Properties.Settings.Default.Name));
			url += String.Format("&port={0}&name={1}", HttpUtility.UrlEncode(Convert.ToString(Properties.Settings.Default.Port)),
				HttpUtility.UrlEncode(Properties.Settings.Default.Name));
			if (!String.IsNullOrEmpty(Properties.Settings.Default.Site))
				url += String.Format("&url={0}", HttpUtility.UrlEncode(Properties.Settings.Default.Site));
			if (!String.IsNullOrEmpty(Properties.Settings.Default.Zone))
				url += String.Format("&zone={0}", HttpUtility.UrlEncode(Properties.Settings.Default.Zone));
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "GET";
			request.UserAgent = "BeemuSharp";
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			StreamReader stream = new StreamReader(response.GetResponseStream());
			String ret = stream.ReadToEnd();
			Program.Logger.LogInfo(ret);
		}
	}
}
