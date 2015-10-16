<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet 
  version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
  exclude-result-prefixes="msxsl">
<xsl:template name="Style">
body {
  margin: 0; 
  padding: 0; 
  background-color: #fff;
}

table {
  background-color: #fff;
  border-spacing: 0;
}

img {
  border: 0;
  outline: none;
  text-decoration: none;
}

a {
  text-decoration: none;
  color: #0062ac;
}

a.brand-font-button {
  color: #fff !important;
}

.brand-whitespace {
  padding: 20px 5px 0px 15px;
}

.brand-whitespace-header {
  padding: 10px 5px 0px 15px;
}

.brand-whitespace-footer {
  padding: 10px 5px 0px 15px;
}

.brand-whitespace-none {
  padding: 0px 5px 0px 15px;
}

.brand-font-header {
  font-size: 150%;
  font-weight: bold;
  color: #0062ac;
}

.brand-font-body {
  font-size: 100%;
  color: #000;
}

.brand-font-body-light {
  font-size: 100%;
  color: #959595;
}

.brand-font-body-smaller {
  font-size: 85%;
  color: #000;

}

.brand-font-body-smaller-light {
  font-size: 85%;
  color: #959595;
}

.brand-font-footer {
  font-size: 85%;
}

.brand-font-footer-link {
  font-size: 100%;
}

.brand-font-button {
  color: #fff !important;
}

.brand-button {
  font-size: 100%;
  font-weight: normal;
  text-decoration: none !important;
  background-color: #0062ac;
  border-top: 10px solid #0062ac;
  border-bottom: 10px solid #0062ac;
  border-left: 20px solid #0062ac;
  border-right: 20px solid #0062ac;
  border-radius: 10px;
  -webkit-border-radius: 10px;
  -moz-border-radius: 10px;
  display: inline-block;
}

.brand-separator {
  padding: 5px 0px 0px 0px;
  border-top: solid #b1babf 1px;
  height: 0;
  line-height: 0;
}

.brand-page {
	width: 100%;
	table-layout: fixed;
}

.brand-page-width {
	width: 600px;
	background-color: yellow;
}
}
</xsl:template>
</xsl:stylesheet>
