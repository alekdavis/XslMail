# What is XslMail?
XslMail is a Windows console application (XslMail.exe) that can help application developers localize HTML email templates.

## Why use XslMail? 
To support localization of HTML email templates, a developer must address the following questions:

- How to implement common look and feel across various templates without code duplication?
- How to define language- or country-specific elements (images, links, styles)?
- How to support both left-to-right and right-to-left text directions?
- How to make email templates look nice across various platforms and clients?
- How to test email templates with minimal effort?
- How to pass data to templates at run time and perform transformations?
- How to make the application find appropriate localized version of a template at run-time?

While there are existing solutions -- such as [Mailr.NET](https://github.com/alekdavis/Mailr) -- for application-specific aspects (like loading proper localized versions, performing data transformations, testing email), the design phase (i.e. the process of building the templates), involves mostly manual steps. XslMail can automate some of them.

## What is the workflow that XslMail support?

Here are the recommended steps for building localized HTML email templates using XslMail:

1. Define common CSS styles and elements.
2. Define language-specific CSS styles and elements (such as default text alignment for left-to-right and right-to-left languages).
3. Build two master files: one for left-to-right, another for right-to-left languages.
4. Build the custom template files for each supported email notification in the base language (such as English).
5. Send the master and template files to the translators.
6. Build the directory structure holding the localized master and template files.
7. Run ```XslMail.exe``` to generate final HTML template files from the localized master and template files.

## What does XslMail do?

XslMail performs several operations. First, it meges localized template files with the corresponding master files and common files to generate the source HTML files. You can test these HTML files in the browsers, but you cannot use them as email templates, because they need additional processing. At next transformation, XslMail converts all CSS definitions to inline styles so that they can be recognized by email client applications. Finally, it validates and cleans up the templates producing the files that an application can use as is. 

## How does XslMail do it?

XslMail gets a list of subfolders from the specified input directory. The program assumes that the subfolders correspond to email templates (it can ignore certain subfolders). It iterates through each input subfolder and processes every file with the name matching the name of the parent folder and the expected extension. It retrieves the language suffix by stripping off the extension and the common name from the file name and appends this suffix to the name of the master file to identified the proper mster file localization. It then merges the template file with the master file to produce and HTML document.

Then XslMail converts CSS styles to inline with the help of [PreMailer.NET](https://github.com/milkshakesoftware/PreMailer.Net), clean up HTML via [HTML Tidy](http://tidy.sourceforge.net/), and saves the result in the specified output folder (the structure of the output folder will resemble the structure of the input folder, except that the output files can have a different extension). The CSS inline and HTML cleanup steps are optional and can be skipped. The resulting output files can be used as HTML email templates.

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
All master files must reside in the same folder. 

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
You can view [samples online](../../tree/master/Samples) or download them from:

- [XslMail Latest Release Downloads](../../releases)

## Application settings

By default, XslMail assumes the following:

- Master files start with "Master", have extension ".xslt" and reside in the working directory.
- Template files reside in the "Input" subfolder under the working directory and have ".xml" extension.
- Output files will  go to the "Output" folder under the working directory.
- Intermediate files will not be saved.
- HTML files will be inlined and tidyfied.

There are other less essential default that you can check by launching XslMail with the ```/?``` command-line switch. 

You can override the default settings using command-line or the configuration (```.config```) file (if XslMail runs with at least one command-line switch it will ignore the configuration file; otherwise, it will use it, if the file is present).

## Command line

Here are some examples of command-line switches:

  ```XslMail /i:"..\Templates" /o:"C:\Result" /l:Log.txt /stopOnError```
  ```XslMail /masterFileName:Base /masterFileExtension:xsl /keepComments```
  ```XslMail /saveTempFiles /TempFolder:Temp /q /nowarn```

To see a list and description of all command-line options, run the XslMail with the ```/?``` switch.

## Dependencies

XslMail depends on the following packages:

- [PreMailer.NET](https://www.nuget.org/packages/PreMailer.Net) (performs CSS inlining; see [project site](https://github.com/milkshakesoftware/PreMailer.Net))
- [TidyManaged](https://www.nuget.org/packages/TidyManaged/) (fixes issues caused by PreMailer.NET and reformats HTML; see [project site](https://github.com/markbeaton/TidyManaged))

All other third-party .NET dependencies are merged into the application assembly.

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
