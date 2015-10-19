using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using TidyManaged;
using Plossum.CommandLine;

namespace XslMail
{
	/// <summary>
	/// Generates localized HTML email template files from the 
	/// master (XSLT) and custom template (XML) files.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The application is built as a 32-bit assembly because
	/// it calls a 32-bit native DLL (libtidy.dll).
	/// </para>
	/// </remarks>
	class Program
	{
		// Optional log file.
		private static StreamWriter _logFile = null;

		// Optional error log file.
		private static StreamWriter _errFile = null;

		// Command line options.
		private static Options		_options= null;

		/// <summary>
		/// Generates HTML file from XSLT and XML documents,
		/// converts CSS styles to the inline definitions, and 
		/// cleans up HTML code and formats the file 
		/// (the last two steps are optional).
		/// </summary>
		/// <param name="args">
		/// Command-line arguments.
		/// </param>
		/// <returns>
		/// <c>0</c> (zero) on success, <c>-1</c> on failure.
		/// </returns>
		static int Main
		(
			string[] args
		)
		{
			// Indicates program success or failure.
			bool success = true;

			// Used to parse command line.
			CommandLineParser parser = null;

			try
			{
				// Parse command line.
				_options = new Options(); 
				parser = new CommandLineParser(_options);

				parser.Parse();

				// If help switch (/h|/?|/help) is provided
				if (_options.Help)
				{
					// Display help usage info.
					Console.WriteLine(parser.UsageInfo.ToString(
						Console.WindowWidth, false));

					Console.WriteLine("For additional information, see:");
					Console.WriteLine("https://github.com/alekdavis/XslMail");

					return 0;
				}

				// If there are problems with provided switches
				if (parser.HasErrors)
				{
					// Display error info.
					Console.WriteLine(parser.UsageInfo.GetErrorsAsString(
						Console.WindowWidth));
					return -1;
				}
			
				// Finalize initialization of the application options.
				// This call will check if the program is invoked
				// without the command-line switches and if so
 				// read settings from the .config file.
				// It will also adjust the default settings for the
				// application runtime context (working folder, etc).
				_options.Initialize(args);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Cannot initialize.");
				Console.Error.WriteLine(GetErrorMessage(ex));

				return -1;
			}

			// If optional error log file is specified, open it.
			try
			{
				if (!String.IsNullOrEmpty(_options.ErrorFile))
					_errFile = OpenFile(_options.ErrorFile);
			}
			catch(Exception ex)
			{
				Console.Error.WriteLine(
					"Cannot initialize error file '" + 
					_options.ErrorFile + "'.");
				Console.Error.WriteLine(GetErrorMessage(ex));

				if (_logFile != null)
					_logFile.Close();

				return -1;
			}

			// If optional log file is specified, open it.
			try
			{
				if (!String.IsNullOrEmpty(_options.LogFile))
				{
					_logFile = OpenFile(_options.LogFile);
				}
			}
			catch(Exception ex)
			{
				Console.Error.WriteLine(
					"Cannot initialize log file '" + 
					_options.LogFile + "'.");
				Console.Error.WriteLine(GetErrorMessage(ex));

				if (_errFile != null)
					_errFile.Close();

				return -1;
			}

			// Log application options, if needed.
			if (_options.EchoSettings)
			{
				JsonSerializerSettings jsonConfig = 
					new JsonSerializerSettings();
				
				jsonConfig.NullValueHandling = NullValueHandling.Include;

				Log(LogType.Log, JsonConvert.SerializeObject(
					_options, Newtonsoft.Json.Formatting.Indented, 
					jsonConfig));
			}

			try
			{
				// Get the list of folders holding template files.
				List<string> inputFolders = GetInputFolders();			

				foreach (string inputFolder in inputFolders)
				{
					try
					{
						// Process all supported files in folder.
						ProcessFolder(inputFolder);
					}
					catch (Exception ex)
					{
						if (_options.StopOnError)
							throw;

						Log(LogType.Error, GetErrorMessage(ex));
					}
				}

				success = true;

				Log(LogType.Log, "Done.");
			}
			catch (Exception ex)
			{
				Log(LogType.Error, GetErrorMessage(ex));
			}

			return success ? 0 : -1;
		}

		/// <summary>
		/// Generates HTML email templates for all supported files 
		/// in the specified folder.
		/// </summary>
		/// <param name="templateFolderPath">
		/// Path to the template folder (can be absolute or relative).
		/// </param>
		private static void ProcessFolder
		(
			string templateFolderPath
		)
		{
			// Name of the folder serves as the template identifier.
			string templateId = Path.GetFileName(templateFolderPath);

			Log(LogType.Log, "Processing {0}.", templateFolderPath);
	
			// Names of all template files in the folder must 
			// follow this convention: <template id><language><extension>
			// Example:
			//
			// Hello
			//   Hello.xml
			//   Hello-en_us.xml
			//   Hello.ru.xml
			//   Hellozh_tw.xml
			// Goodbye
			//   Goodbye_en-us.xml
			//   Goodbye es.xml
			//   Goodbye-zh_cn.xml
			// 
			string[] files = Directory.GetFiles(templateFolderPath, 
				String.Format("{0}*{1}", templateId,
					_options.TemplateFileExtension));

			foreach (string file in files)
			{
				try
				{
					ProcessFile(templateId, file);
				}
				catch (Exception ex)
				{
					string errMsg = String.Format(
						"Cannot process file: {0}.", file);

					if (_options.StopOnError)
						throw new Exception(errMsg, ex);

					Log(LogType.Error, errMsg);
					Log(LogType.Error, GetErrorMessage(ex));
				}
			}
		}

		/// <summary>
		/// Generates a localized HTML email template using 
		/// the localized master file and template customizations
		/// defined in the localized XML file.
		/// </summary>
		/// <param name="templateId">
		/// Template identifier 
		/// (must match the name of the immediate subfolder).
		/// </param>
		/// <param name="templateFilePath">
		/// Path to the localized template file.
		/// </param>
		private static void ProcessFile
		(
			string templateId,
			string templateFilePath
		)
		{
			// Extract name of the file from path.
			// Name will be used to generate temp and output file names.
			string inputFileName  = Path.GetFileNameWithoutExtension(
				templateFilePath);

			// Output file will be named after the template XML file
			// but with a different extension,
			// If language suffix is present in the name,
			// it will be preserved.
			string outputFileName = String.Format("{0}{1}", 
				inputFileName, _options.OutputFileExtension);

			// Since the file name must start with the template 
			// identifier (name of the immediate subfolder), 
			// subtract it from the file name to get the language 
			// suffix. If language suffix is omitted, it's okay.
			string langSuffix = inputFileName.Remove(0, templateId.Length);

			// Build master file path for the given language (if any).
			string masterFilePath = GetMasterFilePath(langSuffix);

			// Get name of the master file for display purposes.
			string masterFileName = Path.GetFileName(masterFilePath);

			Log(LogType.Log, "Merging {0} and {1} into {2}.", 
				Path.GetFileName(templateFilePath), 
				masterFileName, outputFileName);

			int		step				= 0;
			string	tempFilePath		= null;
			string	tempFilePathFormat	= null;

			// Generate name template for the temporary files 
			// in case we need to save those. The name will include
			// the number of the step producing the file appended
			// before the extension.
			tempFilePathFormat = Path.Combine(
				_options.TempFolder,
				templateId,
				String.Format("{0}.{{0}}{1}", 
					inputFileName, _options.OutputFileExtension));

			// This will be the final file.
			string outputFilePath= Path.Combine(
				_options.OutputFolder, templateId, outputFileName);

			// Email template text.
			string html	= null;

			// STEP 1 (REQUIRED): Transform XSL.
			try
			{
				html = TransformXsl(masterFilePath, templateFilePath);
			}
			catch (Exception ex)
			{
				throw new Exception(
					String.Format("Cannot transform files " +
						"{0} and {1}.", masterFilePath, templateFilePath),
					ex);
			}

			// Save result of XSL transformation (if needed).
			if (_options.SaveTempFiles)
			{
				tempFilePath = String.Format(tempFilePathFormat, step++);

				try
				{
					SaveFile(tempFilePath, html);
				}
				catch (Exception ex)
				{
					throw new Exception(
						String.Format("Cannot save temporary file {0}.", 
							tempFilePath),
						ex);
				}
			}

			// STEP 2 (OPTIONAL): Convert CSS styles to inline styles.
			if (!_options.NoInlineCss)
			{
				try
				{
					html = InlineCss(html);
				}
				catch (Exception ex)
				{
					throw new Exception(
						String.Format(
							"Cannot move CSS styles inline for {0}.", 
							inputFileName),
						ex);
				}

				// If we're saving temp files, save this one only
				// if the tidy step will follow.
				if (_options.SaveTempFiles && (!_options.NoTidy))
				{
					tempFilePath = String.Format(
						tempFilePathFormat, step++);

					try
					{
						SaveFile(tempFilePath, html);
					}
					catch (Exception ex)
					{
						throw new Exception(
							String.Format(
								"Cannot save temporary file {0}.", 
								tempFilePath),
							ex);
					}
				}
			}

			// STEP 3 (OPTIONAL): Tidy HTML.
			if (!_options.NoTidy)
			{
				try
				{
					html = TidyHtml(html);
				}
				catch (Exception ex)
				{
					throw new Exception(
						String.Format("Cannot tidy HTML for {0}.", 
							inputFileName),
						ex);
				}
			}

			// STEP 4 (FINAL): Save result.
			try
			{
				// Save final HTML file.
				if (!_options.NoOutput)
					SaveFile(outputFilePath, html);
			}
			catch (Exception ex)
			{
				throw new Exception(
					String.Format("Cannot save output file {0}.", 
						outputFilePath),
					ex);
			}
		}

		/// <summary>
		/// Transformation for the localized master (XSLT) and 
		/// template (XML) files into HTML string.
		/// </summary>
		/// <param name="masterFilePath">
		/// Path to the master file.
		/// </param>
		/// <param name="templateFilePath">
		/// Path to the template file.
		/// </param>
		/// <returns>
		/// HTML string generated by XSL transformation.
		/// </returns>
		private static string TransformXsl
		(
			string	masterFilePath,
			string	templateFilePath
		)
		{
			// Resulting HTML string.
			string html = null;

			// Localized master template file.
			XslCompiledTransform xslDoc = new XslCompiledTransform();
			xslDoc.Load(masterFilePath);
	
			// Localized notification template file.
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(templateFilePath);

			// Use memory stream to generate the UTF-8 encoding 
			// directive; otherwise, the transformation may result 
			// in the UTF-16 encoding directive.
			using (MemoryStream stream = new MemoryStream())
			{
				// Merge XSL (master) document with XML (template) 
				// document.
				xslDoc.Transform(xmlDoc, null, stream);

				// Move cursor to the beginning.
				stream.Seek(0, SeekOrigin.Begin);
				
				// Read text from memory string into a string.
				using (StreamReader reader = new 
					StreamReader(stream, true))
				{
					html = reader.ReadToEnd();
				}
			}

			return html;
		}

		/// <summary>
		/// Move CSS definitions from the style tag to the inline style
		/// attributes.
		/// </summary>
		/// <param name="html">
		/// The HTML string.
		/// </param>
		/// <returns>
		/// The modified HTML string.
		/// </returns>
		/// <remarks>
		/// This operation is performed with the help of 
		/// <a href="https://www.nuget.org/packages/PreMailer.Net"/>PreMailer.Net</a>.
		/// </remarks>
		private static string InlineCss
		(
			string html
		)
		{
			var result = PreMailer.Net.PreMailer.MoveCssInline(
				html,
				ignoreElements:	_options.IgnoreStyleSelector,
				stripIdAndClassAttributes: true,
				removeStyleElements:true, 
				removeComments:	true);

			if (!_options.NoWarnings)
			{
				foreach (string warning in result.Warnings)
					Log(LogType.Log, warning);
			}

			return result.Html;
		}

		/// <summary>
		/// Tidies the HTML string.
		/// </summary>
		/// <param name="html">
		/// The original HTML string.
		/// </param>
		/// <returns>
		/// The modified HTML string.
		/// </returns>
		/// <remarks>
		/// This operation is performed with the help of 
		/// <a href="https://www.nuget.org/packages/TidyManaged/"/>TidyManaged</a>.
		/// </remarks>
		private static string TidyHtml
		( 
			string html
		)
		{
			// Must use memory stream to support Unicode characters.
			// When using a regular string, Tidy will convert characters
			// to ASCII (must be a bug).
			using (MemoryStream stream = 
				new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
			{ 
				using (TidyManaged.Document doc =
					TidyManaged.Document.FromStream(stream))
				{
					// Do not add META tag with Tidy version info.
					doc.AddTidyMetaElement = false;

					// Specify if NCRs (numeric character references should 
					// be allowed). NCRs format looks like this:  &#...; 
					// For example, the Euro sign (U+20AC) looks like:
					// - &#8364; (decimal)
					// - &#x20AC; (hexadecimal) in Unicode
					// - &#128; (in Cp1252 code page),
					// - &#164; (Euro in ISO/IEC 8859-15 ) 
					// REMARK: It looks like this setting governs input data,
					// i.e. it will convert numeric character references in
					// the input string/stream to the corresponding Unicode 
					// characters (given appropriate output configuration).
 					// We need this because PreMailer generates converts
					// Unicode characters to NCRs, so this option converts
					// them back.
					doc.AllowNumericCharacterReferences = true; 

					// CharacterEncoding does not seem to do anything,
					// but set it just in case.
					doc.CharacterEncoding = TidyManaged.EncodingType.Utf8;
	
					// Leave the DOCTYPE element as is.
					doc.DocType	= DocTypeMode.User;	

					// Always use UTF-8 for character encoding.
					doc.InputCharacterEncoding	= TidyManaged.EncodingType.Utf8;

					// Produce HTML output.
					doc.OutputHtml = true;

					// Specify if Tidy should output entities other than
					// the built-in HTML entities (&amp;, &lt;, &gt; and &quot;) 
					// in the numeric rather than the named entity form.
					// Only entities compatible with the DOCTYPE declaration 
					// generated are used.  Entities that can be represented 
					// in the output encoding are translated correspondingly.
					doc.OutputNumericEntities = false;
					
					// Specify whether to print messages to console.
					doc.Quiet = !_options.Quiet;

					// Output unadorned & characters as &amp;.
					doc.QuoteAmpersands = true;

					// Always use UTF-8 for character encoding.
					doc.OutputCharacterEncoding = TidyManaged.EncodingType.Utf8;

					// Convert NCR (see above) entities (such as &#8364;) 
					// to characters (such as Euro sign).
					doc.PreserveEntities = false;

					// Specify whether to display warnings.
					doc.ShowWarnings = !_options.NoWarnings;

					// Do not stop on errors.
					doc.MaximumErrors = int.MaxValue;

					// Always produce output even if there is an error in page 
					// that Tidy did not fix.
					doc.ForceOutput = true;

					// Okay, prettify it.
					doc.CleanAndRepair();

					// Get prettified result.
					return doc.Save();
				}
			}
		}

		/// <summary>
		/// Saves the specified text in a file.
		/// </summary>
		/// <param name="path">
		/// File path.
		/// </param>
		/// <param name="text">
		/// Text to be saved.
		/// </param>
		/// <remarks>
		/// <para>
		/// Text will be saved in the UTF-8 format.
		/// </para>
		/// <para>
		/// If the folder structure specified in the file
		/// path does not exist, it will be created.
		/// </para>
		/// <para>
		/// If a file in the specified path already exists,
		/// it will be deleted.
		/// </para>
		/// </remarks>
		private static void SaveFile
		(
			string path, 
			string text
		)
		{
			// Delete old file.
			if (File.Exists(path))
				File.Delete(path);

			string folder = Path.GetDirectoryName(path);

			// Make sure directory exists.
			if (!String.IsNullOrEmpty(folder))
				if (!Directory.Exists(folder))
					Directory.CreateDirectory(folder);

			File.WriteAllText(path, text, Encoding.UTF8);
		}

		/// <summary>
		/// Gets path to the master file for the specified language.
		/// </summary>
		/// <param name="langSuffix">
		/// The language suffix.
		/// </param>
		/// <returns>
		/// Path to the master file.
		/// </returns>
		private static string GetMasterFilePath
		(
			string langSuffix
		)
		{
			string masterFileName =
				String.Format("{0}{1}{2}", 
					_options.MasterFileName, 
					langSuffix, 
					_options.MasterFileExtension);

			return Path.Combine(_options.MasterFolder, masterFileName);
		}

		/// <summary>
		/// Gets the list of folders holding localized templates.
		/// </summary>
		/// <returns>
		/// List of template folders.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The folders matching the ignore list 
		/// (see <see cref="XslMail.Options.IgnoreFolderPattern"/>)
		/// will be removed from the result.
		/// </para>
		/// </remarks>
		private static List<string> GetInputFolders()
		{
			string baseFolder = _options.InputFolder;

			if (!Directory.Exists(baseFolder))
				throw new DirectoryDoesNotExistException(baseFolder);

			string[] folders = Directory.GetDirectories(baseFolder);

			if (folders == null || folders.Length == 0)
				throw new DirectoryIsEmptyException(baseFolder);

			List<string> inputFolders = new List<string>();

			foreach (string folder in folders)
			{
				// Make sure this folder is not in the list of
				// names to be ignored.
				if (!IgnoreInputFolder(folder))
					inputFolders.Add(folder);
			}

			return inputFolders;
		}

		/// <summary>
		/// Checks if the specified input folder must be ignored.
		/// </summary>
		/// <param name="path">
		/// Folder path.
		/// </param>
		/// <returns>
		/// <c>True</c> fi folder must be ignored; otherwise, <c>false</c>.
		/// </returns>
		private static bool IgnoreInputFolder
		(
			string	path
		)
		{
			string subFolder = Path.GetFileName(path);

			if (String.IsNullOrEmpty(subFolder))
				return true;

			if (String.IsNullOrEmpty(_options.IgnoreFolderPattern))
				return false;

			Regex regex = new Regex(_options.IgnoreFolderPattern,
				RegexOptions.IgnoreCase);

			if (regex.IsMatch(subFolder))
				return true;

			return false;
		}

		/// <summary>
		/// Builds and writes error logMessage to console.
		/// </summary>
		/// <param name="errMsg">
		/// The error logMessage (or format string).
		/// </param>
		/// <param name="args">
		/// Optional arguments.
		/// </param>
		private static void ShowError
		(
			string errMsg,
			params object[] args
		)
		{
			if (String.IsNullOrEmpty(errMsg))
				return;

			if (args != null)
				errMsg = String.Format(errMsg, args);

			Console.WriteLine(errMsg);
		}

		/// <summary>
		/// Writes error messages from immediate and inner exceptions 
		/// to the console window.
		/// </summary>
		/// <param name="ex">
		/// Exception object.
		/// </param>
		private static void ShowError
		(
			Exception ex
		)
		{
			while (ex != null)
			{
				Console.WriteLine(ex.Message);
				ex = ex.InnerException;
			}
		}

		/// <summary>
		/// Opens a log file for writing.
		/// </summary>
		/// <param name="path">
		/// The file path.
		/// </param>
		/// <returns>
		/// Opened file stream writer object.
		/// </returns>
		private static StreamWriter OpenFile
		(
			string path
		)
		{
			StreamWriter file = new StreamWriter(
				File.Open(path, FileMode.Create, FileAccess.Write, 
					FileShare.Read));
			file.AutoFlush = true;

			return file;
		}

		/// <summary>
		/// Gets the combined error logMessage from immediate 
		/// and all inner exceptions.
		/// </summary>
		/// <param name="ex">
		/// The exception object.
		/// </param>
		/// <returns>
		/// Complete error logMessage.
		/// </returns>
		private static string GetErrorMessage
		(
			Exception ex
		)
		{
			if (ex == null)
				return "";

			StringBuilder errMsg = new StringBuilder();

			while (ex != null)
			{
				if (errMsg.Length != 0)
					errMsg.Append(" ");

				// Remove all new line characters from the string;
				errMsg.Append(Regex.Replace(ex.Message, @"\r\n?|\n", " "));

				ex = ex.InnerException;
			}

			return errMsg.ToString();
		}

		/// <summary>
		/// Logs a logMessage of the specified type.
		/// </summary>
		/// <param name="type">
		/// The type of log entry.
		/// </param>
		/// <param name="options">
		/// The command-line options.
		/// </param>
		/// <param name="message">
		/// The logMessage (or logMessage format).
		/// </param>
		/// <param name="args">
		/// The optional logMessage arguments.
		/// </param>
		/// <remarks>
		/// The basic rules of logging:
		/// - errors go to STDERR (and log/error file(s), 
		///   if these are specified);
		/// - verbose messages go to STDOUT (and log file, if it's 
		///   specified) in verbose mode only;
		/// - log entries go to STDOUT (and log file, if it's specified)
		/// - the quiet mode would suppress all non-error messages from 
		///   being written to STDOUT.
		/// </remarks>
		private static void Log
		(
			LogType			type,
			string			message,
			params object[]	args
		)
		{
			if (message == null)
				return;

			string logMessage = null;
		
			// Format message if needed.
			if (args != null && args.Length > 0)
				logMessage = String.Format(message, args);
			else
				logMessage = message;

			switch (type)
			{
				case LogType.Verbose:
				{
					if (!_options.Verbose)
						return;
				
					if (!_options.Quiet)
						Console.WriteLine(logMessage);

					if (_logFile != null)
					{
						_logFile.WriteLine(logMessage);
						_logFile.Flush();
					}
					break;
				}
				case LogType.Log:
				{
					if (!_options.Quiet)
						Console.WriteLine(logMessage);

					if (_logFile != null)
					{
						_logFile.WriteLine(logMessage);
						_logFile.Flush();
					}
					break;
				}
				case LogType.Error:
				{
					Console.Error.WriteLine(logMessage);

					if (_logFile != null)
					{
						_logFile.WriteLine(logMessage);
						_logFile.Flush();
					}

					if (_errFile != null)
					{
						_errFile.WriteLine(logMessage);
						_errFile.Flush();
					}
					break;
				}
			}
		}
	}
}
