<?xml version="1.0" ?><xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"><xsl:output method="text" indent="no"/><xsl:template match="/data"><![CDATA[<font face="Tahoma, Verdana, Arial" size=2 style="display:inline;"></font><div><br></div><div><p class=MsoNormal><span style="font-family:&quot;Calibri&quot;,sans-serif;">&#160;</span></p>

<table class=MsoNormalTable border=1 cellspacing=0 cellpadding=0 style="margin-left:1.0cm;border-collapse:collapse;border:none;">
 <tbody><tr class=keyboardFocusClass>
  <td width=179 valign=top style="width:134.45pt;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt;">
  <p class=MsoNormal><span style="color:windowtext;">Applicant:</span></p>
  </td>
  <td width=404 valign=top style="width:303.3pt;border:solid windowtext 1.0pt;border-left:none;padding:0cm 5.4pt 0cm 5.4pt;">]]><xsl:choose><xsl:when test="defra_application/defra_customerid/@name"><xsl:value-of select="defra_application/defra_customerid/@name" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[</td>
 </tr>
 <tr class=keyboardFocusClass>
  <td width=179 valign=top style="width:134.45pt;border:solid windowtext 1.0pt;border-top:none;padding:0cm 5.4pt 0cm 5.4pt;">
  <p class=MsoNormal><span style="color:windowtext;">Site
  name:</span></p>
  </td>
  <td width=404 valign=top style="width:303.3pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt;">]]><xsl:choose><xsl:when test="defra_application/defra_site_description"><xsl:value-of select="defra_application/defra_site_description" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[</td>
 </tr>
 <tr class=keyboardFocusClass>
  <td width=179 valign=top style="width:134.45pt;border:solid windowtext 1.0pt;border-top:none;padding:0cm 5.4pt 0cm 5.4pt;">
  <p class=MsoNormal><span style="color:windowtext;">Application
  Ref:</span></p>
  </td>
  <td width=404 valign=top style="width:303.3pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt;" class="">
  <p class=MsoNormal>]]><xsl:choose><xsl:when test="defra_application/defra_applicationnumber"><xsl:value-of select="defra_application/defra_applicationnumber" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[</p>
  </td>
 </tr>
 <tr class=keyboardFocusClass>
  <td width=179 valign=top style="width:134.45pt;border:solid windowtext 1.0pt;border-top:none;padding:0cm 5.4pt 0cm 5.4pt;">
  <p class=MsoNormal><span style="color:windowtext;">Application
  Type:</span></p>
  </td>
  <td width=404 valign=top style="width:303.3pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt;">]]><xsl:choose><xsl:when test="defra_application/defra_applicationtype"><xsl:value-of select="defra_application/defra_applicationtype" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[</td>
 </tr>
 <tr class=keyboardFocusClass>
  <td width=179 valign=top style="width:134.45pt;border:solid windowtext 1.0pt;border-top:none;padding:0cm 5.4pt 0cm 5.4pt;">
  <p class=MsoNormal><span style="color:windowtext;">Date
  of 'Sign Off':</span></p>
  </td>
  <td width=404 valign=top style="width:303.3pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt;" class="">]]><xsl:choose><xsl:when test="defra_application/defra_peerreviewedon"><xsl:value-of select="defra_application/defra_peerreviewedon" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[<p class=MsoNormal></p>
  </td>
 </tr>
 <tr style="height:39.1pt;">
  <td width=584 colspan=2 valign=top style="width:437.75pt;border:solid windowtext 1.0pt;border-top:none;padding:0cm 5.4pt 0cm 5.4pt;height:39.1pt;">
  <p class=MsoNormal><span style="color:windowtext;">I
  have reviewed all permitting documents in line with appropriate regime
  specific check lists. See comments and tracked changes. </span></p>
  </td>
 </tr>
 <tr class=keyboardFocusClass>
  <td width=179 valign=top style="width:134.45pt;border:solid windowtext 1.0pt;border-top:none;padding:0cm 5.4pt 0cm 5.4pt;">
  <p class=MsoNormal><span style="color:windowtext;">Peer
  Reviewer Name: </span></p>
  </td>
  <td width=404 valign=top style="width:303.3pt;border-top:none;border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt;" class=keyboardFocusClass>
  <p class=MsoNormal>]]><xsl:choose><xsl:when test="defra_application/defra_reviewerid/@name"><xsl:value-of select="defra_application/defra_reviewerid/@name" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[</p>
  <p class=MsoNormal><span style="color:windowtext;">&#160;</span></p>
  </td>
 </tr>
</tbody></table>

<p class=MsoNormal>&#160;</p>

<p class=MsoNormal><span style="font-family:&quot;Calibri&quot;,sans-serif;">&#160;</span></p>

<p class=MsoNormal>&#160;</p></div>]]></xsl:template></xsl:stylesheet>