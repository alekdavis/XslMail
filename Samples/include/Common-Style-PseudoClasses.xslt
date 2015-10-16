<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet 
  version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
  exclude-result-prefixes="msxsl">
<xsl:template name="StylePseudoClasses">
a:hover {
  text-decoration: underline;
}

a.brand-font-button:hover {
  text-decoration: none !important;
}
</xsl:template>
</xsl:stylesheet>
