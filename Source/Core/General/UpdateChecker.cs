﻿using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace CodeImp.DoomBuilder
{
	internal static class UpdateChecker
	{
		private static BackgroundWorker worker;
		private static string errordesc;
		
		internal static void PerformCheck()
		{
			if(worker != null && worker.IsBusy)
			{
				General.ErrorLogger.Add(ErrorType.Warning, "Update check is already running!");
				General.MainWindow.ShowErrors();
				return;
			}
			
			worker = new BackgroundWorker();
			worker.DoWork += DoWork;
			worker.RunWorkerCompleted += RunWorkerCompleted;
			worker.WorkerSupportsCancellation = true;
			worker.RunWorkerAsync();
		}

		private static void DoWork(object sender, DoWorkEventArgs e)
		{
			string url = GetDownloadUrl(Path.Combine(General.AppPath, "Updater.ini"));
			if(string.IsNullOrEmpty(url))
			{
				errordesc = "Update check failed: failed to get update url from Updater.ini!";
				e.Cancel = true;
				return;
			}

			// Get local revision number
			int localrev = General.ThisAssembly.GetName().Version.Revision;

			// Get remote revision number
			int remoterev;
			using(MemoryStream stream = DownloadWebFile(Path.Combine(url, "Version.txt")))
			{
				if(stream == null)
				{
					errordesc = "Update check failed: failed to retrieve remote revision info.";
					e.Cancel = true;
					return;
				}

				string s = string.Empty;
				using(StreamReader reader = new StreamReader(stream))
				{
					s = reader.ReadToEnd();
				}

				if(!int.TryParse(s, out remoterev))
				{
					errordesc = "Update check failed: failed to retrieve remote revision number.";
					e.Cancel = true;
					return;
				}
			}

			if(remoterev > localrev)
			{
				// Get changelog info
				string changelog = GetChangelog(url, localrev);

				if(string.IsNullOrEmpty(changelog))
				{
					errordesc = "Update check failed: failed to retrieve changelog.";
					e.Cancel = true;
					return;
				}

				// Pass data to MainForm
				General.MainWindow.UpdateAvailable(remoterev, changelog);
			}
		}

		private static void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
		{
			worker = null;
			if(!string.IsNullOrEmpty(errordesc))
			{
				General.ErrorLogger.Add(ErrorType.Warning, errordesc);
				General.MainWindow.ShowErrors();
			}
		}

		private static string GetChangelog(string url, int localrev)
		{
			StringBuilder sb = new StringBuilder(@"{\rtf1");
			
			using(MemoryStream stream = DownloadWebFile(Path.Combine(url, "Changelog.xml")))
			{
				if(stream == null) return string.Empty;
				
				XmlDocument doc = new XmlDocument();
				doc.Load(stream);
				int noderev;

				// Revision infos go in descending order
				if(doc.ChildNodes.Count == 0) return string.Empty;
				foreach(XmlNode log in doc.ChildNodes)
				{
					if(log.ChildNodes.Count == 0) continue;
					foreach(XmlNode logentry in log.ChildNodes)
					{
						if(logentry.Attributes == null || !int.TryParse(logentry.Attributes.GetNamedItem("revision").Value, out noderev)) continue;
						if(noderev <= localrev) break;

						// Add info
						sb.Append(@"{\b R" + noderev + @":}\par ");

						foreach(XmlNode prop in logentry.ChildNodes)
						{
							if(prop.Name == "msg")
							{
								sb.Append(prop.InnerText.Trim()).Append(@"\par\par ");
								break;
							}
						}
					}
				}
			}

			sb.Append("}");
			return sb.ToString();
		}

		private static MemoryStream DownloadWebFile(string url)
		{
			// Open a data stream from the supplied URL
			WebRequest webReq = WebRequest.Create(url);
			WebResponse webResponse;

			try
			{
				webResponse = webReq.GetResponse();
			}
			catch(WebException)
			{
				return null;
			}
			
			Stream dataStream = webResponse.GetResponseStream();

			// Download the data in chuncks
			byte[] dataBuffer = new byte[1024];

			// Download the data
			MemoryStream memoryStream = new MemoryStream();
			while(!General.MainWindow.IsDisposed)
			{
				// Let's try and read the data
				int bytesFromStream = dataStream.Read(dataBuffer, 0, dataBuffer.Length);
				if(bytesFromStream == 0)
				{
					// Download complete
					break;
				}
				else 
				{
					// Write the downloaded data
					memoryStream.Write(dataBuffer, 0, bytesFromStream);
				}
			}

			// Release resources
			dataStream.Close();

			// Rewind and return the stream
			memoryStream.Position = 0;
			return memoryStream;
		}

		private static string GetDownloadUrl(string filename)
		{
			string[] lines = File.ReadAllLines(filename);
			foreach(string line in lines)
			{
				if(line.StartsWith("URL")) return line.Substring(3).Trim();
			}

			return string.Empty;
		}
	}
}
