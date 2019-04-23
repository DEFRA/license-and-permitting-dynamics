﻿<?xml version="1.0" ?><xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"><xsl:output method="text" indent="no"/><xsl:template match="/data"><![CDATA[<table style="cursor:default;color:rgb(0, 0, 0);font-style:normal;font-weight:400;letter-spacing:normal;orphans:2;text-align:start;text-indent:0px;text-transform:none;white-space:normal;widows:2;word-spacing:0px;background-color:rgb(255, 255, 255);text-decoration-style:initial;text-decoration-color:initial;font-family:Arial;font-size:medium;padding:0px;margin:0px;" width="100%" cellspacing=0 cellpadding=0><tbody><tr><td style="font-size:11px;background-color:rgb(0, 0, 0);"><img alt="Gov.uk Logo" src="https://devwptinfsto001.blob.core.windows.net/dynamics-emails-images/gov-uk-logo.png"></td></tr><tr class=keyboardFocusClass><td style="font-size:11px;"><img alt="Environment Agency Logo" src="https://devwptinfsto001.blob.core.windows.net/dynamics-emails-images/ea_logo_sm.png" style="margin-top:10px;margin-bottom:10px;" width=200 height=57></td></tr></tbody></table><div style="margin:0px 0px 12px;color:rgb(0, 0, 0);font-style:normal;font-weight:400;letter-spacing:normal;orphans:2;text-align:start;text-indent:0px;text-transform:none;white-space:normal;widows:2;word-spacing:0px;background-color:rgb(255, 255, 255);text-decoration-style:initial;text-decoration-color:initial;font-family:Arial;font-size:medium;max-width:600px;" class=keyboardFocusClass><p dir=ltr style="line-height:1.2;margin-top:10pt;margin-bottom:8pt;" id=docs-internal-guid-16d31cf2-7fff-202f-82a1-f60a2e8008e4 class=keyboardFocusClass><span style="font-size:18pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:700;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">Your medium combustion plant / specified generator (MCP/SG) permit application is ready to process (duly made)</span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:16pt;margin-bottom:8pt;"><span style="font-size:13.999999999999998pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:700;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">Application details</span></p><p dir=ltr style="line-height:1.38;margin-top:8pt;margin-bottom:8pt;" class=keyboardFocusClass><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">Application reference: </span>&#160;]]><xsl:choose><xsl:when test="defra_application/defra_applicationnumber"><xsl:value-of select="defra_application/defra_applicationnumber" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[<span style="font-size:12pt;font-family:Arial;color:#000000;background-color:#fff2cc;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;"></span><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;"><br></span><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">Standard rules permit:&#160; </span><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">]]><xsl:choose><xsl:when test="defra_application/defra_facility_description"><xsl:value-of select="defra_application/defra_facility_description" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[</span><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;"><br></span><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">Operator: </span>]]><xsl:choose><xsl:when test="defra_application/defra_customerid/@name"><xsl:value-of select="defra_application/defra_customerid/@name" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[<span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;"><br></span><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">Date received: </span><font class=keyboardFocusClass face=Arial><span></span>]]><xsl:choose><xsl:when test="defra_application/defra_submittedon/@date"><xsl:value-of select="defra_application/defra_submittedon/@date" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[</font><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:#fff2cc;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;"><br></span><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">Duly made date: </span><font class=keyboardFocusClass face=Arial><span></span>]]><xsl:choose><xsl:when test="defra_application/defra_dulymadecompletedate/@date"><xsl:value-of select="defra_application/defra_dulymadecompletedate/@date" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[</font><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:#fff2cc;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;"></span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:16pt;margin-bottom:0pt;"><span style="font-size:13.999999999999998pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:700;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">What we will do next</span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:8pt;margin-bottom:8pt;"><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">We will decide whether to issue a permit (determination). We will let you know by email if we need any further information.</span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:8pt;margin-bottom:8pt;" class=keyboardFocusClass><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">We usually make a decision about MCP/SG standard rules permits within 12 weeks. If we need more information or there are complications, it can take longer than this.</span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:8pt;margin-bottom:8pt;"><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">If we intend to refuse your application, we will contact you in advance to explain our reasons.</span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:8pt;margin-bottom:8pt;"><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">When we have made our decision we will send you the permit or the refusal notice by email.</span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:8pt;margin-bottom:8pt;"><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">If you asked for your application to be confidential, we will write to you later about our decision on that.</span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:16pt;margin-bottom:0pt;"><span style="font-size:13.999999999999998pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:700;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">To contact us</span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:8pt;margin-bottom:8pt;" class=keyboardFocusClass><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">Reply to this email or email </span><a class="sc-jxgvnK hfYTYd" href="mailto:mcp-sr-permitting@environment-agency.gov.uk" title="mailto:mcp-sr-permitting@environment-agency.gov.uk">mcp-sr-permitting@</a><a class="sc-jxgvnK hfYTYd" href="mailto:psc-bacs@defra.onmicrosoft.com" title="mailto:psc-bacs@defra.onmicrosoft.com">defra.onmicrosoft.com</a><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">. Include your application reference in the email subject line.</span></p><p dir=ltr style="line-height:1.3679999999999999;margin-top:8pt;margin-bottom:8pt;"><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:400;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">We will reply within 10 working days.</span></p><br><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:700;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">MCP Directive Standard Rules Team</span><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:700;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;"><br></span><span style="font-size:12pt;font-family:Arial;color:#000000;background-color:transparent;font-weight:700;font-style:normal;font-variant:normal;text-decoration:none;vertical-align:baseline;white-space:pre;white-space:pre-wrap;">Climate Change Trading and Regulatory Services</span></div>]]></xsl:template></xsl:stylesheet>