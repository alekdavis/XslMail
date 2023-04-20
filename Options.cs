using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

using TidyManaged;

using Plossum.CommandLine;
using C5;
using System.Diagnostics;

namespace XslMail
{
	/// <summary>
	/// Defines command-line options.
	/// </summary>
	/// <remarks>
	/// This implementation of command-line handling requires
	/// <a href="https://www.nuget.org/packages/Plossum.CommandLine/">Plossum CommandLine</a>
	/// Nuget package (see 
	/// <a href="http://www.codeproject.com/Articles/19869/Powerful-and-simple-command-line-parsing-in-C">documentation</a>).
	/// </remarks>
	[CommandLineManager(EnabledOptionStyles=OptionStyles.Windows)]
	[CommandLineOptionGroup(
		"options", 
		Name="OPTIONS")]
	public class Options
	{
		[CommandLineOption(
			Name="echoSetting", 
			Description="Log application settings. " +
				"By default, application settings will not be logged.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public bool EchoSettings { get; set; }

		[CommandLineOption(
			Name="errorFile", 
			Description="Path to the optional error log file. " +
				"By default, all error messages will be written to the standard " +
				"error stream (stderr).",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string ErrorFile { get; set; }

		[CommandLineOption(
			Name="h", 
			Aliases="help,?", 
			Description="Shows this help text.", 
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public bool Help { get; set; }

		[CommandLineOption(
			Name="ignoreFolderPattern", 
			Description="Regular expression pattern defining the list of " +
				"input folders that must be ignored. " +
				"The default value is '^\\W|^_|^common$|^include$|^master$|^shared$', " +
				"i.e. subfolder names that start with non-alphanumeric characters and " +
				"matching specific names (common, include, master, shared).",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string IgnoreFolderPattern { get; set; }

		[CommandLineOption(
			Name="ignoreStyleSelector", 
			Description="Selector for the CSS <style> section elements " +
				"of which will not be inlined. " +
				"The default value is '#IgnoreInline', i.e. " +
				"the CSS elements defined in the section with  the ID value of " +
				"'IgnoreInline' (<style type='text/css' id='IgnoreInline'>) " +
				"will not be inlined.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string IgnoreStyleSelector { get; set; }

		[CommandLineOption(
			Name="i", 
			Aliases="in,input,inputFolder", 
			Description="Path to the root folder containing the data. " +
				"By default, it is assumed to be 'Input' in the " +
				"working folder.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string InputFolder { get; set; }

		[CommandLineOption(
			Name="keepComments", 
			Description="Keeps comment elements. " +
				"By default, comments will be stripped.",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool KeepComments { get; set; }

		[CommandLineOption(
			Name="keepIdAndClassAttributes", 
			Description="Keeps ID and class attributes after the CSS inlining step. " +
				"By default, ID and class attributes will be stripped.",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool KeepIdAndClassAttributes { get; set; }

		[CommandLineOption(
			Name="keepStyleElements", 
			Description="Keeps the external style elements. " +
				"By default, the external style elements will be stripped.",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool KeepStyleElements { get; set; }

		[CommandLineOption(
			Name="l", 
			Aliases="log,logFile", 
			Description="Path to the optional log file. " +
				"By default, all log messages will be written to the " +
				"standard output stream (stdout).",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string LogFile { get; set; }

		[CommandLineOption(
			Name="masterFileExtension",
			Description="Extension of the master email template file. " +
				"The default value is '.xslt'.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string MasterFileExtension	{ get; set; }

		[CommandLineOption(
			Name="masterFileName",
			Description="Name of the master email template file " +
				"(not including the language suffix). " +
				"The default value is 'Master'.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string MasterFileName { get; set; }

		[CommandLineOption(
			Name="masterFolder", 
			Description="Path to the root folder containing the " +
				"master files. " +
				"By default, master files are assumed to be in " +
				"the root of the input folder.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string MasterFolder { get; set; }

		[CommandLineOption(
			Name="noInlineCss", 
			Description="Do not convert CSS styles inline. " +
				"By default, the CSS styles will be changed to " +
				"inline with the help of PreMailer.NET.",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool NoInlineCss { get; set; }

		[CommandLineOption(
			Name="noOutput", 
			Description="Do not generate the output files. " +
				"This setting overrides the /saveTempFiles switch. " +
				"By default, the output files will be generated.",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool NoOutput { get; set; }

		[CommandLineOption(
			Name="noTidy", 
			Description="Do not reformat the resulting HTML. " +
				"By default, the generated output files will be " +
				"reformatted using Tidy HTML " +
				"(libtidy.dll must be in the working folder).",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool NoTidy { get; set; }

		[CommandLineOption(
			Name="nowarn", 
			Aliases="noWarnings", 
			Description="Do not display warning messages. " +
				"By default, all warnings will be displayed.",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool NoWarnings { get; set; }

		[CommandLineOption(
			Name="outputFileExtension",
			Description="Extension of the output email template file. " +
				"The default value is '.html'.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string OutputFileExtension { get; set; }

		[CommandLineOption(
			Name="o", 
			Aliases="out,output,outputFolder", 
			Description="Path to the root folder that will hold output. " +
				"By default, this will be be 'Output' in the " +
				"working folder.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string OutputFolder { get; set; }

		[CommandLineOption(
			Name="q", 
			Aliases="quiet", 
			Description="Suppresses all log messages directed to the standard " +
				"output stream (stdout).",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool Quiet { get; set; }

		[CommandLineOption(
			Name="saveTempFiles",
			Description="Saves intermediate files for each step of the workflow. " +
				"By default, the intermediate step files will not be saved.",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool SaveTempFiles { get; set; }

		[CommandLineOption(
			Name="stopOnError", 
			Description="Stops execution on first encountered error. " +
				"By default, execution will continue on error.",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool StopOnError	{ get; set; }

		[CommandLineOption(
			Name="temp", 
			Aliases="tempFolder", 
			Description="Path to the folder where the intermediate " +
				"files will be saved. " +
				"By default, this will be folder 'Temp' in the " +
				"working folder.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string TempFolder { get; set; }

		[CommandLineOption(
			Name="templateFileExtension",
			Description="Extension of the file holding email template " +
				"customization info. " +
				"The default value is '.xml'.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string TemplateFileExtension	{ get; set; }

		[CommandLineOption(
			Name="v", 
			Aliases="verbose", 
			Description="Outputs verbose information about the program " +
				"execution. " +
				"By default, the verbose messages will not be generated.",
			MinOccurs=0,
			MaxOccurs=1, 
			GroupId="options")]
		public bool Verbose { get; set; }

		[CommandLineOption(
			Name="sub",
			Aliases="substitutions",
			Description="List of character and string substitutions" +
				"that will be performed on the final HTML text. " +
				"The default value is '©=&copy;|®=&reg;|™=&trade;'.",
			MinOccurs=0, 
			MaxOccurs=1, 
			GroupId="options")]
		public string Substitutions { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Options"/> class
		/// and sets the command-line defaults.
		/// </summary>
		public Options()
		{
			EchoSettings			= false;
			ErrorFile				= null;
			IgnoreFolderPattern		= "\\W|^_|^common$|^include$|^master$|^shared$'";
			IgnoreStyleSelector		= "#IgnoreInline";
			InputFolder				= @"Input";
			KeepComments			= false;
			KeepIdAndClassAttributes= false;
			KeepStyleElements		= false;
			LogFile					= null;
			MasterFileExtension		= ".xslt";
			MasterFileName			= "Master";
			MasterFolder			= "";
			NoInlineCss				= false;
			NoOutput				= false;
			NoTidy					= false;
			NoWarnings				= false;
			OutputFileExtension		= ".html";
			OutputFolder			= @"Output";
			Quiet					= false;
			SaveTempFiles			= false;
			StopOnError				= true;
			TempFolder				= "Temp";
			TemplateFileExtension   = ".xml";
            Verbose                 = false;
            Substitutions			= "©=&copy;|®=&reg;|™=&trade;";
        }

        /// <summary>
        /// Initializes the settings.
        /// </summary>
        /// <param name="args">
        /// Command-line arguments.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method checks if the program was invoked without 
        /// command-line switches and, if so, sets the option
        /// values using the application configuration file
        /// (if the configuration file exists).
        /// If the program was invoked with command-line switches,
        /// the configuration file will be ignored.
        /// </para>
        /// <para>
        /// This method also sets appropriate defaults and resolves
        /// conflicting settings.
        /// </para>
        /// </remarks>
		public void Initialize
		(
			string[] args
		)
		{
			if (args.Length == 0)
			{
				ReadConfigFile();
            }

			// Use root of input folder if master folder
			// is not specified.
			if (String.IsNullOrEmpty(MasterFolder))
				MasterFolder = InputFolder;

			// If no output must be generated,
			// do not save intermediate (temp) files.
			if (NoOutput)
				SaveTempFiles = false;
		}

		/// <summary>
		/// Sets program option using the application settings read
		/// from the application configuration (.config) file.
		/// </summary>
		private void ReadConfigFile()
		{
			var appSettings = ConfigurationManager.AppSettings;

			foreach (string key in appSettings.AllKeys)
			{
				string value = appSettings[key];
				
				if (key.Equals("EchoSettings", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					EchoSettings = bool.Parse(value.ToLower());				
				}
				else if (key.Equals("ErrorFile", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					ErrorFile = value;
				}
				else if (key.Equals("IgnoreFolderPattern", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					IgnoreFolderPattern = value;				
				}
				else if (key.Equals("IgnoreStyleSelector", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					IgnoreStyleSelector = value;				
				}
				else if (key.Equals("InputFolder", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					InputFolder = value;				
				}
				else if (key.Equals("KeepComments", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					KeepComments = bool.Parse(value.ToLower());				
				}
				else if (key.Equals("KeepStyleElelments", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					KeepStyleElements = bool.Parse(value.ToLower());				
				}
				else if (key.Equals("KeepIdAndClassAttributes", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					KeepIdAndClassAttributes = bool.Parse(value.ToLower());				
				}
				else if (key.Equals("LogFile", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					LogFile = value;				
				}
				else if (key.Equals("MasterFileExtension", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					MasterFileExtension = value;								
				}
				else if (key.Equals("MasterFileName", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					MasterFileName = value;				
				}
				else if (key.Equals("MasterFolder", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					MasterFolder = value;				
				}
				else if (key.Equals("NoInlineCss", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					NoInlineCss = bool.Parse(value.ToLower());				
				}
				else if (key.Equals("NoOutput", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					NoOutput = bool.Parse(value.ToLower());				
				}
				else if (key.Equals("NoTidy", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					NoTidy = bool.Parse(value.ToLower());								
				}
				else if (key.Equals("NoWarnings", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					NoWarnings = bool.Parse(value.ToLower());
				}
				else if (key.Equals("OutputFileExtension", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					OutputFileExtension = value;				
				}
				else if (key.Equals("OutputFolder", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					OutputFolder = value;				
				}
				else if (key.Equals("Quiet", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					Quiet = bool.Parse(value.ToLower());				
				}
				else if (key.Equals("SaveTempFiles", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					SaveTempFiles = bool.Parse(value.ToLower());
				}
				else if (key.Equals("StopOnError", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					StopOnError = bool.Parse(value.ToLower());
				}
				else if (key.Equals("TempFolder", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					TempFolder = value;				
				}
				else if (key.Equals("TemplateFileExtension", 
					StringComparison.InvariantCultureIgnoreCase))
				{
					TemplateFileExtension = value;				
				}
				else if (key.Equals("Verbose",
					StringComparison.InvariantCultureIgnoreCase))
				{
					Verbose = bool.Parse(value.ToLower());								
				}
				else if (key.Equals("Substitutions",
					StringComparison.InvariantCultureIgnoreCase))
				{
					Substitutions = value;
				}
			}
		}
	}
}

