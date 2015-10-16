<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet 
  version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
  exclude-result-prefixes="msxsl">
	
<xsl:include href="Include/Common-Meta.xslt"/> 
<xsl:include href="Include/Common-Style.xslt"/> 
<xsl:include href="Include/Common-Style-DirectionLTR.xslt"/>
<xsl:include href="Include/Common-Style-PseudoClasses.xslt"/>
	
<xsl:output method="html" encoding="utf-8" indent="no"/>
	
<xsl:template match="/">
<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;
</xsl:text>
<html lang="zh-Hant" dir="ltr">
<head>
<title>
<xsl:value-of select="/template/subject"/>
</title>
<xsl:call-template name="Meta"/>
<style type="text/css">
<xsl:call-template name="StyleLTR"/>
<xsl:call-template name="Style"/>
</style>
<style type="text/css" id="IgnoreInline">
<xsl:call-template name="StylePseudoClasses"/>	
</style>
</head>
	
<body>
	<table summary="" class="brand-page-width">
		<tr>
			<!-- Left cell with message body. -->
			<td class="brand-page-width">
				
				<!-- Message body. -->
				<table summary="" class="brand-page-width">
					<!-- Header. -->
					<tr>
						<td class="brand-whitespace-none brand-font-body-smaller-light brand-align-donotreply">
							自动生成的消息。
						</td>
					</tr>
					
					<!-- Message heading. -->
					<tr>
						<td class="brand-whitespace-header brand-font-header">
						<xsl:value-of select="/template/heading"/>
						</td>
					</tr>
					<tr>
						<td class="brand-whitespace brand-font-body">
						<xsl:value-of select="/template/greeting"/>
						</td>
					</tr>
					
					<!-- Message body. -->
					<xsl:copy-of select="/template/body/node()"/>
				
					<!-- Help section. -->
					<tr>
						<td class="brand-whitespace brand-font-body">
							<b>需要帮忙</b>
						</td>
					</tr>
					<tr>
						<td class="brand-whitespace brand-font-body">
							对于技术帮助，请联系 
							<a href="@Model.HelpUrl"
							   target="_blank">Initech</a>.
						</td>
					</tr>
					
					<!-- Footer. -->
					<tr>
						<td class="brand-whitespace-footer">
							<table summary="" class="brand-page">
								<tr>
									<td class="brand-separator"><xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text><!-- DO NOT REMOVE! --></td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td class="brand-whitespace-footer brand-font-footer">
							Initech的承诺保护您的隐私。
							有关Initech的隐私政策的信息，请参阅
							<a href="#" class="brand-font-footer-link">这</a>.
						</td>
					</tr>
					<tr>
						<td class="brand-whitespace-footer brand-font-footer">
							<xsl:text disable-output-escaping="yes"><![CDATA[&copy;]]></xsl:text>
							Initech的公司。 
							版权所有。
						</td>
					</tr>
				</table>
			</td>
			<!-- Right cell used for width adjustment. -->
			<td><xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text><!-- DO NOT REMOVE! --></td>
		</tr>
	</table>
</body>
</html> 
</xsl:template>
	
</xsl:stylesheet>

