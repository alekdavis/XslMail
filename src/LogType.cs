using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XslMail
{
	/// <summary>
	/// Type of a log entry.
	/// </summary>
	enum LogType
	{
		/// <summary>
		/// The normal log entry going to STDOUT and log file.
		/// </summary>
		Log,
		/// <summary>
		/// The verbose log entry with less meaningful info going to STDOUT and log file.
		/// </summary>
		Verbose,
		/// <summary>
		/// The error log entry going to STDERR, log and error log files.
		/// </summary>
		Error
	}
}
