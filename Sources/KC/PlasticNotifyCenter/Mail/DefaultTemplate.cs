namespace PlasticNotifyCenter.Mail
{
    /// <summary>
    /// Supplies a default template for emails
    /// </summary>
    public static class DefaultTemplate
    {
        /// <summary>
        /// Gets HTML code for a email template
        /// </summary>
        public static string Html => @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
	<head>
		<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
		<title>Plastic-Notify-Center</title>
		<meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
	</head>
	<body style=""margin: 0; padding: 0; font-family: Arial, Helvetica, sans-serif"">
		<table align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"">
			<tr>
				<td align=""center"" bgcolor=""#ae8aed"" style=""padding: 40px 0 30px 0;"">
					<img alt=""Logo"" width=""131"" height=""150"" src=""https://raw.githubusercontent.com/KhaosCoders/plastic-notify-center/master/Sources/KC/PlasticNotifyCenter/wwwroot/img/mail-logo.png"">
					<div style=""color:#ffffff; font-weight:bold; font-size:2em; margin:10px 4px 10px 4px;"">Plastic-Notify-Center</div>
				</td>
			</tr>
			<tr>
				<td bgcolor=""#ffffff"" style=""padding: 20px 30px 40px 30px;"">
					<h1>%PNC_TITLE%</h1>
					<div>%PNC_BODY%</div>
				</td>
			</tr>
			<tr>
				<td bgcolor=""#ffffff"" style=""padding: 10px 30px 20px 30px; text-align:right;"">
					<small>%PNC_TAGS%</small>
				</td>
			</tr>
			<tr>
				<td bgcolor=""#7D3C98"" style=""padding: 30px 30px 30px 30px;"">
					<table cellpadding=""0"" cellspacing=""0"" width=""100%"">
						<tr>
							<td width=""75%"">
                                <p style=""color: #ffffff;"">
                                    Send by <strong>Plastic-Notify-Center</strong><br/>
                                    <small>
                                        Manage your notification rules <a href=""%PNC_RULESURL%"" style=""color: #ffffff; font-weight: bold;"">here</a><br/>
                                        Or ask your message coordinator
                                    </small>
                                </p>
							</td>
							<td>
							<div style=""background:#FF5733; text-align:center; border-radius: 4px; padding: 8px 4px 8px 4px;"">
								<a href=""https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=SECLTUNN2B776&source=url"" target=""_blank"" style=""font-weight:bold; color: #ffffff; text-decoration:none;"">Support this application</a>
								</div>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</body>
</html>";
    }
}