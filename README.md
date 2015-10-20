# What is XslMail?
XslMail is a Windows console application (```XslMail.exe```) that can help application developers localize HTML email templates (the rest of the document will refer to *HTML email* templates as *email* templates or *HTML* templates).

## Why use XslMail? 
When building localized email templates, a developer must address the following questions:

- How to implement common look and feel across various templates without code duplication?
- How to define language- or country-specific elements (images, links, styles)?
- How to support both left-to-right and right-to-left text directions?
- How to make email templates look nice across various platforms and clients (Gmail, Outlook, Yahoo!, iPhone, Android, etc)?
- How to test email templates with minimal effort?
- How to pass data to templates at run time and perform transformations?
- How to make the application find appropriate localized version of a template at run-time?

While there are existing solutions -- such as [Mailr.NET](https://github.com/alekdavis/Mailr) -- for application-specific aspects (like loading proper localized versions, performing data transformations, testing email), the design phase (i.e. the process of building the templates), mostly involves manual steps. XslMail can automate some of them.

## How do you build email templates using XslMail?

Here is a brief outline of the workflow that you can use to design, build and localize email templates using XslMail:

1. Define common CSS styles and elements.
2. Define language-specific CSS styles and elements (such as default text alignment for left-to-right and right-to-left languages).
3. Build master files for each unique language-specific layout. Some languages may reuse the existing layout, while other may not (for example, language versions supporting the right-to-left text direction may not look nice when applied to a left-to-right layout).
4. Build the custom template files for each supported email notification in the base language (such as English).
5. Send the master and template files to the translators.
6. Build the directory structure holding the localized master and template files.
7. Run ```XslMail.exe``` to generate final HTML template files from the localized master and template files.

## What does XslMail do?

XslMail performs several operations. First, it merges localized template files with the corresponding master files and common files to generate the original version of the HTML template files. You can test these templates directly in the browsers, but you should not  use them in an application because the external CSS styles defined in the HTML code will not be recognized by many email clients (such as Gmail). You can use an online tool (such as [CSS Inliner](http://inliner.cm/)), to convert external styles to inline CSS, but XslMail will do it for you automatically. In addition, it will remove the unneeded external styles and class definitions, clean up and validate the HTML code resulting in the template files that your application can use without further adjustments.

## How does XslMail do it?

XslMail gets a list of subfolders from the specified input directory (skipping the ones that must be ignored). The program assumes that each subfolder holds localized versions of a specific email template. It iterates through every file found in the subfolder that matches a specific pattern (the file name must start with the name of the subfolder and end with the specified extension) and retrieves the language suffix from the file name (by stripping off the name of the folder at the beginning and extension at the end of the file name). XslMail then merges a master file with the same language suffix with the template file to produce a localized HTML template.

Then XslMail calls [PreMailer.NET](https://github.com/milkshakesoftware/PreMailer.Net) to convert external styles to inline CSS and cleans up HTML code with the help of [HTML Tidy](http://tidy.sourceforge.net/). It saves the result in the specified output folder (the structure of the output folder will resemble the structure of the input folder, except that the output files may have a different extension). The resulting output files can now be used as localized email templates.

## Which does a developer need to know to use XslMail?
To build HTML email templates for XslMail, a developer must be familiar with the following languages and technologies:

- HTML
- XML
- XSLT
- CSS
- .NET Framework

## What does XslMail require?

XslMail runtime requirements include:

- .NetFramework 4.5

## What does XslMail expect?

XslMail expects the following:

### Master files

Master files define the overall look and feel of HTML email templates. They contain common footers, headers, placeholders for headings (and other customizable elements) and other customized section of the email messages.

Master files are built as XSL templates. They support standard XSLT features. Master files can include or import other XSL files. For example, you can define common CSS styles in a stand-alone XSL file and include it in the style section of a master file.

Each supported language must have its own master file. All master files must have the same name, a unique language-specific suffix, and the same extension. The format of the language suffix is not important, although it must match the suffix of the localized template files. Here are some examples of master file names:
```
Master\
- Master.xslt
- Master-EN_US.xslt
- Master (RU).xslt
- Master_ZH-TW.xslt
```
All master files must reside in the same folder. The location of the master file folder can be customized. 

### Template files

Template files define customized sections that will be merged with the master files to generate specific email templates. Template files must be implemented as XML documents.

All translations of the same template must reside under the same folder. Template file names must be named after the template folder and end with the language suffix matching the suffix of the corresponding master file. Here is an example of the template file names and folder structure corresponding the master files listed above:
```
EmailVerification\
- EmailVerification.xml
- EmailVerification-EN_US.xml
- EmailVerification (RU).xml
- EmailVerification_ZH-TW.xml
WeeklyPromo\
- WeeklyPromo.xml
- WeeklyPromo-EN_US.xml
- WeeklyPromo (RU).xml
- WeeklyPromo_ZH-CN.xml
```

## Samples
You can view [samples online](../../tree/master/Samples) or download them from:

- [XslMail Latest Release Downloads](../../releases)

## Application settings

By default, XslMail assumes the following:

- Master files start with *Master*, have the *.xslt* extension.
- Mater files reside in the working directory.
- Template files reside in the *Input* subfolder under the working directory and have the *.xml* extension.
- Output files will be saved in the *Output* folder under the working directory.
- Intermediate files will not be saved.
- HTML files will be inlined, cleaned up, and verified.

There are other less essential default that you can check by launching XslMail with the ```/?``` command-line switch. 

You can override the default settings using the command-line switches or an application configuration (```.config```) file (if XslMail runs with at least one command-line switch it will ignore the configuration file; otherwise, it will use it, if the file is present).

## Command line

Here are some examples of command-line switches:

  ```XslMail /i:"..\Templates" /o:"C:\Result" /l:Log.txt /stopOnError```

  ```XslMail /masterFileName:Base /masterFileExtension:xsl /keepComments```

  ```XslMail /saveTempFiles /TempFolder:Temp /q /nowarn```

To see the list and description of all command-line options, run XslMail with the ```/?``` switch.

## Dependencies

XslMail depends on the following packages:

- [PreMailer.NET](https://www.nuget.org/packages/PreMailer.Net) (performs CSS inlining; see [project site](https://github.com/milkshakesoftware/PreMailer.Net))
- [TidyManaged](https://www.nuget.org/packages/TidyManaged/) (fixes issues caused by PreMailer.NET and cleans up HTML; see [project site](https://github.com/markbeaton/TidyManaged))

All third-party .NET dependencies are merged into the application assembly.

## Distribution files

XslMail distribution includes the following components:

- XslMail.exe
- libtidy.dll

You can download the application binaries from:

- [XslMail Latest Release Downloads](../../releases)

Keep in mind that during run time ```libtidy.dll``` must be located in the application directory.

## Limitations

XslMail is a 32-bit assembly due to dependency on the 32-bit DLL (```libtidy.dll```).

Output files will be encoded in UTF-8.

## Project site

https://github.com/alekdavis/XslMail

## See also

[Mailr.NET](https://github.com/alekdavis/Mailr)
