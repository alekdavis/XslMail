# What is XslMail?
XslMail is a Windows command-line tool implemented as a 32-bit console application (XslMail.exe). XslMail is intended to help application developers and translators simplify the HTML email template design, localization and preparation workflow.

## Why use XslMail? 

Implementing HTML email template localization is a daunting process with many complexities and multiple manual steps. 

First, a developer needs to build HTML templates. To support common look and feel and minimize maintenance effort, a developer may chose to build templates from master files defining the template structure and common element styles and template-specific layouts. Depending on language support, some templates may need to use right-to-left text direction, language- or country-specific elements (such as links or images), and other customizations.

When the templates are ready, they must be sent to translators. Translated versions must be verified and merged with master files to produce the initial template files. If everything looks normal, additional steps must be taken, including changing HTML code to use the inline style definitions (most email clients do not support external styles).

Once the final template versions are built, they can be used to test email notifications in various clients (desktop, web-based, mobile).

As you can see, there are multiple manual steps in this process, and XslMail can automate some of them.

## What does XslMail require?

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
For additional information about master and template files, see [Samples](#samples).

## Samples

You can view [samples online](../../tree/master/Samples) or download them from:

- [XslMail Latest Release Downloads](../../releases)

## How does XslMail work?

XslMail performs several operations. 

First, it gets a list of subfolders from the specified input directory.The program assumes that the subfolders correspond to email templates (it can ignore certain subfolders). It iterates through each input subfolder and processes every file with the name matching the name of the parent folder and the expected extension. It retrieves the language suffix by stripping off the extension and the common name from the file name and appends this suffix to the name of the master file. It then merges the template file with the master file to produce and HTML document.

Then XslMail converts CSS styles to inline, clean up HTMl via HTML Tidy, and saves the result in the specified output folder (the structure of the output folder will resemble the structure of the input folder, except that the output files can have a different extension). The CSS inline and HTML cleanup steps are optional and can be skipped. The resulting output files can be used as HTML email templates.

## Application settings

By default, XslMail assumes the following:

- Master files start with "Master", have extension ".xslt" and reside in the working directory.
- Template files reside in the "Input" subfolder under the working directory and have ".xml" extension.
- Output files will  go to the "Output" folder under the working directory.
- Intermediate files will not be saved.
- HTML files will be inlined and tidyfied.

There are other less essential default that you can check by launching XslMail with the ```/?``` command-line switch.

Application settings can be specified either via command-line switches or in the application configuration (```.config```) file. When XslMail runs with at least one command-line switch it will ignore the configuration file.

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
