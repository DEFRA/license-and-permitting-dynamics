﻿<?xml version="1.0" ?><xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"><xsl:output method="text" indent="no"/><xsl:template match="/data"><![CDATA[<div class=keyboardFocusClass><table style="font-family:Arial;font-size:medium;padding:0px;margin:0px;" width="100%" cellspacing=0 cellpadding=0><tbody><tr><td style="background-color:rgb(0, 0, 0);"><img alt="Gov.uk Logo" src="https://devwptinfsto001.blob.core.windows.net/dynamics-emails-images/gov-uk-logo.png"></td></tr><tr><td><img alt="Environment Agency Logo" src="https://devwptinfsto001.blob.core.windows.net/dynamics-emails-images/ea_logo_sm.png" style="margin-top:10px;margin-bottom:10px;" width=200 height=57></td></tr></tbody></table><div style="font-family:Arial;font-size:medium;max-width:600px;"><div><font face=Arial>Dear ]]><xsl:choose><xsl:when test="defra_application/defra_primarycontactid/@name"><xsl:value-of select="defra_application/defra_primarycontactid/@name" /></xsl:when><xsl:otherwise> Applicant</xsl:otherwise></xsl:choose><![CDATA[,</font></div><div><p dir=ltr style="line-height:1.38;margin-top:10pt;margin-bottom:10pt;"><strong>Your environmental permitting application is duly made</strong></p></div><div class=keyboardFocusClass><div style=""><div><strong>Application reference:</strong>&#160;]]><xsl:choose><xsl:when test="defra_application/defra_applicationnumber"><xsl:value-of select="defra_application/defra_applicationnumber" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[</div><div><strong>Applicant:</strong>&#160;]]><xsl:choose><xsl:when test="defra_application/defra_customerid/@name"><xsl:value-of select="defra_application/defra_customerid/@name" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[</div><div><strong>Facility:</strong>&#160;]]><xsl:choose><xsl:when test="defra_application/defra_site_description"><xsl:value-of select="defra_application/defra_site_description" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[</div></div><div style="margin-top:10pt;margin-bottom:10pt;"><p><font face=Arial>I’m writing to let you know that your application, received on ]]><xsl:choose><xsl:when test="defra_application/defra_submittedon/@date"><xsl:value-of select="defra_application/defra_submittedon/@date" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[, is duly made as of ]]><xsl:choose><xsl:when test="defra_application/defra_dulymadecompletedate/@date"><xsl:value-of select="defra_application/defra_dulymadecompletedate/@date" /></xsl:when><xsl:otherwise> -</xsl:otherwise></xsl:choose><![CDATA[. Duly made means that we have all the information we need to begin determination although at times of high work load there may be a delay whilst we wait for a permitting officer to become available to start the determination. Determination is where we assess your application and decide if we can allow what you’ve asked for. If we have to refuse your application, we’ll explain why.</font></p><p><font face=Arial>We may need to ask you for more information during determination. If we do we’ll write to you to explain what we need and how long you have to reply.</font></p><p><font face=Arial>We want to give you a decision as quickly as possible, but the time it takes depends on what’s in the application and the work load we are experiencing.</font></p><p><font face=Arial>You can expect us to determine your application within the timescales below. This is provided you have sent all the information we need and there are no complicating factors.</font></p></div><table style="border-collapse:collapse;border:none;padding:5.4pt;" width=0 cellspacing=0 cellpadding=0 border=1><thead style="background:rgb(0, 0, 0);"><tr style="background-image:initial;background-position:initial;background-size:initial;background-repeat:initial;background-attachment:initial;background-origin:initial;background-clip:initial;" bgcolor="#000000"><td style="width:400pt;border:1pt solid silver;background-image:initial;background-position:initial;background-size:initial;background-repeat:initial;background-attachment:initial;background-origin:initial;background-clip:initial;padding:5.4pt;color:rgb(238, 238, 238);" width=400 valign=top><p><strong>Permit application type</strong></p></td><td style="width:200pt;border:1pt solid silver;background-image:initial;background-position:initial;background-size:initial;background-repeat:initial;background-attachment:initial;background-origin:initial;background-clip:initial;padding:5.4pt;color:rgb(238, 238, 238);" width=200 valign=top><p><strong>Determination time</strong></p></td></tr></thead><tbody><tr><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Transfer or partial transfer</p></td><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Two months</p></td></tr><tr><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Standard permit (other than installations)</p></td><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Three months</p></td></tr><tr><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Mobile plant permit</p></td><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Three months</p></td></tr><tr><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Variations (without public consultation)</p></td><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Three months</p></td></tr><tr><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Surrender or partial surrender</p></td><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Three months</p></td></tr><tr><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Applications with public consultation:</p><ul><li>standard permit for installations</li><li>bespoke permit; and</li><li>variations</li></ul></td><td style="border-right:1pt solid silver;border-bottom:1pt solid silver;border-left:1pt solid silver;border-top:none;padding:5.4pt;" valign=top><p>Four months</p></td></tr></tbody></table><div class=""><p>We explain more about public consultation in our Public Participation Statement:&#160;<a href="https://www.gov.uk/government/publications/environmental-permitting-public-participation-statement">https://www.gov.uk/government/publications/environmental-permitting-public-participation-statement</a></p><p>We may need a longer timescale to determine your application if:</p><ul><li>the application is complex;</li><li>we need to ask you for more information about the technical aspects of your application; or</li><li>there’s a considerable level of interest from the public or other organisations.</li></ul><p>Additional charges apply if:</p><ul><li>we have to carry out extra assessments due to the proposed activity and location;</li><li>we have to issue three or more information notices relating to the same issue; or</li><li>you want to amend your application in such a way that requires further public consultation.</li></ul><p>You can find further information on charging, including when additional charges apply, at:&#160;<a href="https://www.gov.uk/government/publications/environmental-permitting-ep-charges-scheme-april-2014-to-march-2015">https://www.gov.uk/government/publications/environmental-permitting-ep-charges-scheme</a></p><p>If we haven’t already spoken to you about when to expect our decision when an officer starts the determination they will contact you to explain this.</p><p>If your application contained a request for confidentiality, we will write to you separately about our decision on that.</p><p>If you have any questions in the meantime, please phone our Customer Contact Centre on 03708 506506. Alternatively, please email us at the address below.</p><div style="background-color:transparent;border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;letter-spacing:normal;line-height:normal;margin-bottom:13.33px;margin-left:0px;margin-right:0px;margin-top:13.33px;orphans:2;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;text-align:left;text-decoration:none;text-indent:0px;text-transform:none;vertical-align:baseline;white-space:normal;word-spacing:0px;"><font style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:18.06px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;" size=4 face=Arial><b style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:18.06px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:700;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;">To contact us</b></font></div><div style="background-color:transparent;border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Calibri,Arial,Helvetica,sans-serif;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;letter-spacing:normal;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;orphans:2;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;text-align:left;text-decoration:none;text-indent:0px;text-transform:none;vertical-align:baseline;white-space:normal;word-spacing:0px;"><b style="font-family:Arial;">Medium combustion plant and specified generators standard rules</b></div><div style="background-color:transparent;border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Calibri,Arial,Helvetica,sans-serif;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;letter-spacing:normal;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;orphans:2;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;text-align:left;text-decoration:none;text-indent:0px;text-transform:none;vertical-align:baseline;white-space:normal;word-spacing:0px;" class=keyboardFocusClass><a style="border-bottom-color:rgb(0, 102, 204);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 102, 204);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 102, 204);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 102, 204);border-top-style:none;border-top-width:0px;font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;" href="mailto:mcp-permitting@defra.onmicrosoft.com"><font style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;" face=Arial></font></a><font style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;" face=Arial><a href="mailto:waste-permitting-psc@defra.onmicrosoft.com" style="border-bottom-color:rgb(0, 102, 204);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 102, 204);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 102, 204);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 102, 204);border-top-style:none;border-top-width:0px;font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;">waste-permitting-psc@defra.onmicrosoft.com</a></font></div><div style="background-color:transparent;border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Calibri,Arial,Helvetica,sans-serif;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;letter-spacing:normal;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;orphans:2;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;text-align:left;text-decoration:none;text-indent:0px;text-transform:none;vertical-align:baseline;white-space:normal;word-spacing:0px;"><font style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;" face=Arial><b style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:700;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;"><br style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:700;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;"></b></font></div><div style="background-color:transparent;border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Calibri,Arial,Helvetica,sans-serif;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;letter-spacing:normal;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;orphans:2;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;text-align:left;text-decoration:none;text-indent:0px;text-transform:none;vertical-align:baseline;white-space:normal;word-spacing:0px;"><font style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;" face=Arial><b style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:700;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;">All other standard rules and bespoke applications</b></font></div><div style="background-color:transparent;border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;letter-spacing:normal;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;orphans:2;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;text-align:left;text-decoration:none;text-indent:0px;text-transform:none;vertical-align:baseline;white-space:normal;word-spacing:0px;">Permitting and Support Centre</div><div style="background-color:transparent;border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;letter-spacing:normal;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;orphans:2;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;text-align:left;text-decoration:none;text-indent:0px;text-transform:none;vertical-align:baseline;white-space:normal;word-spacing:0px;"><font style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;" face=Arial><a href="mailto:waste-permitting-psc@defra.onmicrosoft.com" style="border-bottom-color:rgb(0, 102, 204);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 102, 204);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 102, 204);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 102, 204);border-top-style:none;border-top-width:0px;font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;">waste-permitting-psc@defra.onmicrosoft.com</a></font></div><div style="background-color:transparent;border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;letter-spacing:normal;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;orphans:2;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;text-align:left;text-decoration:none;text-indent:0px;text-transform:none;vertical-align:baseline;white-space:normal;word-spacing:0px;"><font style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;" face=Arial>Telephone: 02030 253 898</font></div><div style="background-color:transparent;border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Calibri,Arial,Helvetica,sans-serif;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;letter-spacing:normal;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;orphans:2;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;text-align:left;text-decoration:none;text-indent:0px;text-transform:none;vertical-align:baseline;white-space:normal;word-spacing:0px;"><font style="border-bottom-color:rgb(0, 0, 0);border-bottom-style:none;border-bottom-width:0px;border-left-color:rgb(0, 0, 0);border-left-style:none;border-left-width:0px;border-right-color:rgb(0, 0, 0);border-right-style:none;border-right-width:0px;border-top-color:rgb(0, 0, 0);border-top-style:none;border-top-width:0px;color:rgb(0, 0, 0);font-family:Arial;font-size:16px;font-size-adjust:none;font-style:normal;font-variant:normal;font-weight:400;line-height:normal;margin-bottom:0px;margin-left:0px;margin-right:0px;margin-top:0px;padding-bottom:0px;padding-left:0px;padding-right:0px;padding-top:0px;vertical-align:baseline;" face=Arial>Monday to Friday, 8am to 5pm</font></div></div></div></div></div><font style="display:inline;" size=2 face="Tahoma, Verdana, Arial"></font>]]></xsl:template></xsl:stylesheet>