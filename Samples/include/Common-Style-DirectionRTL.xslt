﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet 
  version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
  exclude-result-prefixes="msxsl">
<xsl:template name="StyleRTL">
td {
  font-family: Verdana, Arial, Helvetica, sans-serif;
	font-size: 13px;
	direction: rtl;
  text-align: right;
  color: #000;
  background-color: #fff;
	padding: 0;
}

.brand-align-donotreply {
	text-align: left;
}
</xsl:template>
</xsl:stylesheet>
