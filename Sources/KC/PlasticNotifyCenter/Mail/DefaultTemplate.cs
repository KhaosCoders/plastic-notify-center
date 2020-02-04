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
					<div style=""display: block; width:150px; height:150px; background-repeat: no-repeat; background-size: 150px 150px; background-image: " +
                    @"url('data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+CjxzdmcKICAgeG1sbnM6ZGM9Imh0dHA6Ly9wdXJsLm9yZy9kYy9lbG" +
                    @"VtZW50cy8xLjEvIgogICB4bWxuczpjYz0iaHR0cDovL2NyZWF0aXZlY29tbW9ucy5vcmcvbnMjIgogICB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiCiAg" +
                    @"IHhtbG5zOnN2Zz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciCiAgIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIKICAgeG1sbnM6c29kaXBvZGk9Imh0dHA6Ly9zb2RpcG9kaS5zb3VyY2" +
                    @"Vmb3JnZS5uZXQvRFREL3NvZGlwb2RpLTAuZHRkIgogICB4bWxuczppbmtzY2FwZT0iaHR0cDovL3d3dy5pbmtzY2FwZS5vcmcvbmFtZXNwYWNlcy9pbmtzY2FwZSIKICAgd2lkdGg9IjQ4IgogICBoZWlnaHQ9I" +
                    @"jQ4IgogICB2aWV3Qm94PSIwIDAgNDggNDgiCiAgIHZlcnNpb249IjEuMSIKICAgaWQ9InN2ZzE2IgogICBzb2RpcG9kaTpkb2NuYW1lPSJsb2dvLXBsYXN0aXMtbm90aWZ5LWNlbnRlci5zdmciCiAgIGlua3Nj" +
                    @"YXBlOnZlcnNpb249IjAuOTIuNCAoNWRhNjg5YzMxMywgMjAxOS0wMS0xNCkiCiAgIGlua3NjYXBlOmV4cG9ydC1maWxlbmFtZT0iQzpcVXNlcnNcZGV2aWxcRG93bmxvYWRzXGxvZ28tcGxhc3Rpcy1ub3RpZnk" +
                    @"tY2VudGVyLnBuZyIKICAgaW5rc2NhcGU6ZXhwb3J0LXhkcGk9IjEzMi45Mjk5OSIKICAgaW5rc2NhcGU6ZXhwb3J0LXlkcGk9IjEzMi45Mjk5OSI+CiAgPG1ldGFkYXRhCiAgICAgaWQ9Im1ldGFkYXRhMjIiPg" +
                    @"ogICAgPHJkZjpSREY+CiAgICAgIDxjYzpXb3JrCiAgICAgICAgIHJkZjphYm91dD0iIj4KICAgICAgICA8ZGM6Zm9ybWF0PmltYWdlL3N2Zyt4bWw8L2RjOmZvcm1hdD4KICAgICAgICA8ZGM6dHlwZQogICAgI" +
                    @"CAgICAgIHJkZjpyZXNvdXJjZT0iaHR0cDovL3B1cmwub3JnL2RjL2RjbWl0eXBlL1N0aWxsSW1hZ2UiIC8+CiAgICAgICAgPGRjOnRpdGxlPjwvZGM6dGl0bGU+CiAgICAgIDwvY2M6V29yaz4KICAgIDwvcmRm" +
                    @"OlJERj4KICA8L21ldGFkYXRhPgogIDxkZWZzCiAgICAgaWQ9ImRlZnMyMCI+CiAgICA8ZmlsdGVyCiAgICAgICBzdHlsZT0iY29sb3ItaW50ZXJwb2xhdGlvbi1maWx0ZXJzOnNSR0IiCiAgICAgICBpbmtzY2F" +
                    @"wZTpsYWJlbD0iQ29sb3IgU2hpZnQiCiAgICAgICBpZD0iZmlsdGVyNDIzMS03Ij4KICAgICAgPGZlQ29sb3JNYXRyaXgKICAgICAgICAgdHlwZT0iaHVlUm90YXRlIgogICAgICAgICB2YWx1ZXM9IjIzOSIKIC" +
                    @"AgICAgICAgcmVzdWx0PSJjb2xvcjEiCiAgICAgICAgIGlkPSJmZUNvbG9yTWF0cml4NDIyNy0wIiAvPgogICAgICA8ZmVDb2xvck1hdHJpeAogICAgICAgICB0eXBlPSJzYXR1cmF0ZSIKICAgICAgICAgdmFsd" +
                    @"WVzPSIxIgogICAgICAgICByZXN1bHQ9ImNvbG9yMiIKICAgICAgICAgaWQ9ImZlQ29sb3JNYXRyaXg0MjI5LTciIC8+CiAgICA8L2ZpbHRlcj4KICAgIDxmaWx0ZXIKICAgICAgIHN0eWxlPSJjb2xvci1pbnRl" +
                    @"cnBvbGF0aW9uLWZpbHRlcnM6c1JHQiIKICAgICAgIGlua3NjYXBlOmxhYmVsPSJDb2xvciBTaGlmdCIKICAgICAgIGlkPSJmaWx0ZXI0MjI1LTUiPgogICAgICA8ZmVDb2xvck1hdHJpeAogICAgICAgICB0eXB" +
                    @"lPSJodWVSb3RhdGUiCiAgICAgICAgIHZhbHVlcz0iMjM5IgogICAgICAgICByZXN1bHQ9ImNvbG9yMSIKICAgICAgICAgaWQ9ImZlQ29sb3JNYXRyaXg0MjIxLTIiIC8+CiAgICAgIDxmZUNvbG9yTWF0cml4Ci" +
                    @"AgICAgICAgIHR5cGU9InNhdHVyYXRlIgogICAgICAgICB2YWx1ZXM9IjEiCiAgICAgICAgIHJlc3VsdD0iY29sb3IyIgogICAgICAgICBpZD0iZmVDb2xvck1hdHJpeDQyMjMtOSIgLz4KICAgIDwvZmlsdGVyP" +
                    @"gogICAgPGZpbHRlcgogICAgICAgc3R5bGU9ImNvbG9yLWludGVycG9sYXRpb24tZmlsdGVyczpzUkdCIgogICAgICAgaW5rc2NhcGU6bGFiZWw9IkNvbG9yIFNoaWZ0IgogICAgICAgaWQ9ImZpbHRlcjQyMTkt" +
                    @"NiI+CiAgICAgIDxmZUNvbG9yTWF0cml4CiAgICAgICAgIHR5cGU9Imh1ZVJvdGF0ZSIKICAgICAgICAgdmFsdWVzPSIyMzkiCiAgICAgICAgIHJlc3VsdD0iY29sb3IxIgogICAgICAgICBpZD0iZmVDb2xvck1" +
                    @"hdHJpeDQyMTUtNiIgLz4KICAgICAgPGZlQ29sb3JNYXRyaXgKICAgICAgICAgdHlwZT0ic2F0dXJhdGUiCiAgICAgICAgIHZhbHVlcz0iMSIKICAgICAgICAgcmVzdWx0PSJjb2xvcjIiCiAgICAgICAgIGlkPS" +
                    @"JmZUNvbG9yTWF0cml4NDIxNy03IiAvPgogICAgPC9maWx0ZXI+CiAgICA8ZmlsdGVyCiAgICAgICBzdHlsZT0iY29sb3ItaW50ZXJwb2xhdGlvbi1maWx0ZXJzOnNSR0IiCiAgICAgICBpbmtzY2FwZTpsYWJlb" +
                    @"D0iQ29sb3IgU2hpZnQiCiAgICAgICBpZD0iZmlsdGVyNDIxMy00Ij4KICAgICAgPGZlQ29sb3JNYXRyaXgKICAgICAgICAgdHlwZT0iaHVlUm90YXRlIgogICAgICAgICB2YWx1ZXM9IjIzOSIKICAgICAgICAg" +
                    @"cmVzdWx0PSJjb2xvcjEiCiAgICAgICAgIGlkPSJmZUNvbG9yTWF0cml4NDIwOS0wIiAvPgogICAgICA8ZmVDb2xvck1hdHJpeAogICAgICAgICB0eXBlPSJzYXR1cmF0ZSIKICAgICAgICAgdmFsdWVzPSIxIgo" +
                    @"gICAgICAgICByZXN1bHQ9ImNvbG9yMiIKICAgICAgICAgaWQ9ImZlQ29sb3JNYXRyaXg0MjExLTkiIC8+CiAgICA8L2ZpbHRlcj4KICA8L2RlZnM+CiAgPHNvZGlwb2RpOm5hbWVkdmlldwogICAgIHBhZ2Vjb2" +
                    @"xvcj0iI2ZmZmZmZiIKICAgICBib3JkZXJjb2xvcj0iIzY2NjY2NiIKICAgICBib3JkZXJvcGFjaXR5PSIxIgogICAgIG9iamVjdHRvbGVyYW5jZT0iMTAiCiAgICAgZ3JpZHRvbGVyYW5jZT0iMTAiCiAgICAgZ" +
                    @"3VpZGV0b2xlcmFuY2U9IjEwIgogICAgIGlua3NjYXBlOnBhZ2VvcGFjaXR5PSIwIgogICAgIGlua3NjYXBlOnBhZ2VzaGFkb3c9IjIiCiAgICAgaW5rc2NhcGU6d2luZG93LXdpZHRoPSIxMTM3IgogICAgIGlu" +
                    @"a3NjYXBlOndpbmRvdy1oZWlnaHQ9IjQ4NiIKICAgICBpZD0ibmFtZWR2aWV3MTgiCiAgICAgc2hvd2dyaWQ9ImZhbHNlIgogICAgIHNob3dndWlkZXM9ImZhbHNlIgogICAgIGlua3NjYXBlOmd1aWRlLWJib3g" +
                    @"9InRydWUiCiAgICAgaW5rc2NhcGU6em9vbT0iMTUuMSIKICAgICBpbmtzY2FwZTpjeD0iODkuNzAxMTg0IgogICAgIGlua3NjYXBlOmN5PSIxNS42MzcyNyIKICAgICBpbmtzY2FwZTp3aW5kb3cteD0iMCIKIC" +
                    @"AgICBpbmtzY2FwZTp3aW5kb3cteT0iMCIKICAgICBpbmtzY2FwZTp3aW5kb3ctbWF4aW1pemVkPSIwIgogICAgIGlua3NjYXBlOmN1cnJlbnQtbGF5ZXI9Imc1NjYyIj4KICAgIDxzb2RpcG9kaTpndWlkZQogI" +
                    @"CAgICAgcG9zaXRpb249IjU1LjAzMzExMyw0OS4wMzk3MzUiCiAgICAgICBvcmllbnRhdGlvbj0iMSwwIgogICAgICAgaWQ9Imd1aWRlNDMyOCIKICAgICAgIGlua3NjYXBlOmxvY2tlZD0iZmFsc2UiIC8+CiAg" +
                    @"ICA8c29kaXBvZGk6Z3VpZGUKICAgICAgIHBvc2l0aW9uPSI0Ny4wODYwOTMsNDQuMTA1OTYiCiAgICAgICBvcmllbnRhdGlvbj0iMSwwIgogICAgICAgaWQ9Imd1aWRlNDMzMCIKICAgICAgIGlua3NjYXBlOmx" +
                    @"vY2tlZD0iZmFsc2UiIC8+CiAgICA8c29kaXBvZGk6Z3VpZGUKICAgICAgIHBvc2l0aW9uPSI2My4wNzk0Nyw0OC45NzM1MSIKICAgICAgIG9yaWVudGF0aW9uPSIxLDAiCiAgICAgICBpZD0iZ3VpZGU0MzMyIg" +
                    @"ogICAgICAgaW5rc2NhcGU6bG9ja2VkPSJmYWxzZSIgLz4KICAgIDxzb2RpcG9kaTpndWlkZQogICAgICAgcG9zaXRpb249IjUzLjY0MjM4NCwxMS42NTU2MjkiCiAgICAgICBvcmllbnRhdGlvbj0iMCwxIgogI" +
                    @"CAgICAgaWQ9Imd1aWRlNDMzNCIKICAgICAgIGlua3NjYXBlOmxvY2tlZD0iZmFsc2UiIC8+CiAgPC9zb2RpcG9kaTpuYW1lZHZpZXc+CiAgPGcKICAgICBpbmtzY2FwZTpncm91cG1vZGU9ImxheWVyIgogICAg" +
                    @"IGlkPSJsYXllcjMiCiAgICAgaW5rc2NhcGU6bGFiZWw9IkN1YmUiCiAgICAgc3R5bGU9ImRpc3BsYXk6aW5saW5lIj4KICAgIDxnCiAgICAgICB0cmFuc2Zvcm09InRyYW5zbGF0ZSgtMjQuNjY0NTQxLDAuNDg" +
                    @"0NTk3ODcpIgogICAgICAgc3R5bGU9ImZpbGw6bm9uZTtmaWxsLXJ1bGU6ZXZlbm9kZCIKICAgICAgIGlkPSJnMTQiPgogICAgICA8cGF0aAogICAgICAgICBkPSJtIDQ4LjY0OTQ3NywyMy40MDY4NDggdiAyMi" +
                    @"4wOTM3ODEgYyAwLjc2OTIyLC0wLjAwMTUgMS40ODQ0NzksLTAuMjI3NDM1IDIuMDkwMzI4LC0wLjYxNjA4MSBMIDY1Ljg0MDYzLDM1Ljk2NzI2MyBjIDEuMjk2OTE5LC0wLjY1NjU3MyAyLjE4NzA4NiwtMi4wM" +
                    @"TU4ODggMi4xODcwODYsLTMuNTg4NjM2IFYgMTQuMTA5NjIgYyAwLC0wLjM0NDM3IC0wLjA0MzE3LC0wLjY3OTY1NyAtMC4xMjMxOCwtMC45OTkwNTEgTCA0OC43MDE5NDksMjMuNDA2ODQ4IDI5LjUxMjc1OCwx" +
                    @"Mi43Mjc2IDQ4LjY0OTQ3NywyMy40MDY4NDgiCiAgICAgICAgIGlkPSJwYXRoNCIKICAgICAgICAgaW5rc2NhcGU6Y29ubmVjdG9yLWN1cnZhdHVyZT0iMCIKICAgICAgICAgc3R5bGU9ImZpbGw6I2ZmNWIwMDt" +
                    @"maWx0ZXI6dXJsKCNmaWx0ZXI0MjMxLTcpIiAvPgogICAgICA8cGF0aAogICAgICAgICBkPSJNIDY2LjA0OTEwMiwxMC43OTA2ODQgNTAuNjkzNTI0LDEuODU2MjMwOSBDIDUwLjExMzMwNywxLjUxOTY3MDcgND" +
                    @"kuNDM5NzQ4LDEuMzI0NTAzMyA0OC43MjE3NTgsMS4zMjQ1MDMzIGMgLTAuNzY4Mzk1LDAgLTEuNDg0MTQ0LDAuMjIxMTE0NSAtMi4wOTAxMjQsMC42MDE2NzIgbCAtMC4wMDUyLC0wLjAwNjc3IC0xNS4yNzIzM" +
                    @"TYsOC44ODM2ODc3IGMgLTAuODQ2NDI5LDAuNDU3NjQ2IC0xLjUwNjU0NywxLjIxNzI1NyAtMS44NDEwODcsMi4xMzY2ODcgTCA0OC43MTI4MjQsMjMuNTQ5ODkgNjguMDI3OTg5LDEzLjMxNjk1NCBjIC0wLjI3" +
                    @"ODUzNCwtMS4wOTQ2NjcgLTEuMDA4MDk5LC0yLjAwODgzMyAtMS45Nzg4NiwtMi41MjYyNzEiCiAgICAgICAgIGlkPSJwYXRoNiIKICAgICAgICAgaW5rc2NhcGU6Y29ubmVjdG9yLWN1cnZhdHVyZT0iMCIKICA" +
                    @"gICAgICAgc3R5bGU9ImZpbGw6I2ZmNDMwMDtmaWx0ZXI6dXJsKCNmaWx0ZXI0MjI1LTUpIiAvPgogICAgICA8cGF0aAogICAgICAgICBkPSJtIDI5LjMwMTgyMSwzMi40Njc0OTQgYyAwLDEuNTU1NDMxIDAuOD" +
                    @"c2Nzc3LDIuOTAzODQ5IDIuMTUzNDgyLDMuNTY4MDI5IDE1LjAzNDc5MSw4Ljg0MzI0NSAxNC45NDY1OTIsOC43NTU1MTIgMTQuOTQ2NTkyLDguNzU1NTEyIDAuNjM4MTY1LDAuNDQ2MTkyIDEuNDEzMzc0LDAuN" +
                    @"zA5NTk0IDIuMjQ3NTgyLDAuNzA5NTk0IGwgMC4wMDg2LC0yMi4wOTUwNTggLTE5LjExMzg5OSwtMTAuNTg4MzQ2IGMgLTAuMTU2ODM0LDAuNDMwNjc3IC0wLjI0MjM0NiwwLjg5NTAzNSAtMC4yNDIzNDYsMS4z" +
                    @"ODAyMDggeiIKICAgICAgICAgaWQ9InBhdGg4IgogICAgICAgICBpbmtzY2FwZTpjb25uZWN0b3ItY3VydmF0dXJlPSIwIgogICAgICAgICBzdHlsZT0iZmlsbDojZGIyZjAwO2ZpbHRlcjp1cmwoI2ZpbHRlcjQ" +
                    @"yMTktNikiIC8+CiAgICAgIDxwYXRoCiAgICAgICAgIHN0eWxlPSJzdHJva2Utd2lkdGg6MC4xMzI0NTAzMztmaWx0ZXI6dXJsKCNmaWx0ZXI0MjEzLTQpIgogICAgICAgICBkPSJNIDQ4LjY1MDE5NiwzNC4zNT" +
                    @"gyMjcgViAyMy4zOTUzODYgTCAzOS4xMTc4NjUsMTguMTE4NTcgMjkuNTg1NTMzLDEyLjg0MTc1MyAyOS43ODcxNiwxMi40NTE4NSBjIDAuMTEwODk1LC0wLjIxNDQ0NyAwLjQyMzU0NiwtMC42MTAyNDMgMC42O" +
                    @"TQ3OCwtMC44Nzk1NDggMC4yOTI4NDksLTAuMjkwNzY2IDMuODAzNDc3LC0yLjQxMjY0NiA4LjY0MzI0NSwtNS4yMjQxMTggOC44OTgzNDUsLTUuMTY5MTQxMyA4LjkyMTczOCwtNS4xODAxODEgMTAuMzM1MjI0" +
                    @"LC00Ljg3NzQwNjIgMC41NDI2MDcsMC4xMTYyMjg2IDIuNTE2OTE4LDEuMjAxMjA5NiA4LjI2MjYzNCw0LjU0MDcyMjQgOC45ODE4MzQsNS4yMjA0MDE4IDkuMDgyMzQ3LDUuMjgyMzA3OCA5LjUzMDM0Niw1Ljg" +
                    @"2OTY2MjggMC43Njc0MjgsMS4wMDYxNTEgMC43MzcxMzksMC41MTU5NyAwLjcwMTMwOSwxMS4zNDkyMzkgbCAtMC4wMzI5OCw5Ljk3MTQ2MSAtMC4zNTg1ODksMC43MzAxMzEgYyAtMC4xOTcyMjQsMC40MDE1Nz" +
                    @"IgLTAuNTU0ODQsMC45MTc3OCAtMC43OTQ3MDIsMS4xNDcxMjggLTAuNTAyMzAyLDAuNDgwMjg2IC0xNi4yMDI2NzMsOS43ODM0NyAtMTYuOTc1OTQ1LDEwLjA1OTAyIC0wLjI4MjMzOCwwLjEwMDYwOSAtMC42N" +
                    @"TQ4NTQsMC4xODI5MjYgLTAuODI3ODE0LDAuMTgyOTI2IGggLTAuMzE0NDczIHoiCiAgICAgICAgIGlkPSJwYXRoMzcyNyIKICAgICAgICAgaW5rc2NhcGU6Y29ubmVjdG9yLWN1cnZhdHVyZT0iMCIgLz4KICAg" +
                    @"IDwvZz4KICA8L2c+CiAgPGcKICAgICBpbmtzY2FwZTpncm91cG1vZGU9ImxheWVyIgogICAgIGlkPSJsYXllcjUiCiAgICAgaW5rc2NhcGU6bGFiZWw9IkxlZnQgU2lkZSIKICAgICBzdHlsZT0iZGlzcGxheTp" +
                    @"pbmxpbmUiPgogICAgPHBhdGgKICAgICAgIGlua3NjYXBlOmNvbm5lY3Rvci1jdXJ2YXR1cmU9IjAiCiAgICAgICBzdHlsZT0iZmlsbDojZmZmZmZmO2ZpbGwtb3BhY2l0eToxIgogICAgICAgZD0ibSA3LjkxMT" +
                    @"E5ODgsMTYuNTkyNjQzIHYgMTQuODU5NDAyIGMgMCwxLjI2NjA0NiAwLjcxNTgwMSwyLjM2MjEzMyAxLjc2MDQyNCwyLjkwMjA5MiBMIDExLjQ0Mzk3NywzNS4zOTMwNyBWIDI5LjgwNTQ3MSAyOC45MzIzNjIgM" +
                    @"TguMjQ5MzcgYyAtMC44ODM1MjEsLTEuNjExNzQxIDAuMjkzMTkxLC00LjA4NzQ5OCAtMy41MzI3NzgyLC0xLjY1NjcyNyB6IgogICAgICAgaWQ9InBhdGgxMCIKICAgICAgIHNvZGlwb2RpOm5vZGV0eXBlcz0i" +
                    @"Y3NjY2NjY2MiIC8+CiAgPC9nPgogIDxnCiAgICAgaW5rc2NhcGU6bGFiZWw9IlJpZ2h0IFNpZGUiCiAgICAgaWQ9Imc1NjU0IgogICAgIGlua3NjYXBlOmdyb3VwbW9kZT0ibGF5ZXIiCiAgICAgc3R5bGU9ImR" +
                    @"pc3BsYXk6aW5saW5lIj4KICAgIDxwYXRoCiAgICAgICBpZD0icGF0aDU2NTIiCiAgICAgICBkPSJtIDM5LjgwNzQzMiwxNi42MDkxOTkgdiAxNC44NTk0MDIgYyAwLDEuMjY2MDQ2IC0wLjcxNTgwMSwyLjM2Mj" +
                    @"EzMyAtMS43NjA0MjQsMi45MDIwOTIgbCAtMS43NzIzNTQsMS4wMzg5MzMgViAyOS44MjIwMjcgMjguOTQ4OTE4IDE4LjI2NTkyNiBjIDAuMzYzODg4LC0xLjQ2MDQwMSAwLjkwNDI5MywtMi4yODcxNCAzLjUzM" +
                    @"jc3OCwtMS42NTY3MjcgeiIKICAgICAgIHN0eWxlPSJmaWxsOiNmZmZmZmY7ZmlsbC1vcGFjaXR5OjEiCiAgICAgICBpbmtzY2FwZTpjb25uZWN0b3ItY3VydmF0dXJlPSIwIgogICAgICAgc29kaXBvZGk6bm9k" +
                    @"ZXR5cGVzPSJjc2NjY2NjYyIgLz4KICA8L2c+CiAgPGcKICAgICBpbmtzY2FwZTpsYWJlbD0iVG9wIgogICAgIGlkPSJnNTY1MCIKICAgICBpbmtzY2FwZTpncm91cG1vZGU9ImxheWVyIgogICAgIHN0eWxlPSJ" +
                    @"kaXNwbGF5OmlubGluZSI+CiAgICA8cGF0aAogICAgICAgaWQ9InBhdGg1NjQ4IgogICAgICAgZD0ibSAyNS40Njk4MTEsMjUuNzUzNzYgYyAtMC40NzQ5NywwLjI3NTI0NCAtMS4wMjUyNCwwLjQzNDI5OSAtMS" +
                    @"42MTMxNjYsMC40MzQyOTkgLTAuNjI3MDcyLDAgLTEuMjEyMDE2LC0wLjE4MDg2NCAtMS43MDc4NTcsLTAuNDkyNTgxIC0wLjAwMjIsMC4wMDM4IC0xMi40ODg0OTYyLC03LjI2NTc1OCAtMTIuNDg4NDk2MiwtN" +
                    @"y4yNjU3NTggLTEuMDEyOTMzLC0wLjU1MDExMSAtMS43NDkwOTMsLTAuNTk0NzIgLTEuNzQ5MDkzLC0xLjgzNzA3NyAwLjAxMzMyLC0xLjE0NzY1MyAwLjM4MTE4MiwtMS43MTg0NjcgMS4wMjAyMjEsLTEuOTU1" +
                    @"NTk0IDEuMTY3MjE5MiwtMC4yOTE5MTcgMS41ODA1ODkyLDAuMTM0NTA5IDMuODgzNjE3MiwxLjQ5Nzg0NSBsIDkuNzA5NTQ2LDUuNjU0OTA2IGMgMC4zODgwOTksMC4yMzgwMTggMC44NDI5MzEsMC4zNzgyNzI" +
                    @"gMS4zMzA1NzEsMC4zNzgyNzIgMC40NTcwNjcsMCAwLjg4NTQyNywtMC4xMjMzMzMgMS4yNTQ1MTcsLTAuMzM4NDE1IGwgOS43NjA5OSwtNS42ODY0OSBjIDUuMzI0NDIzLDAuNjcyNTU0IDAuNzE1NCwzLjEwMj" +
                    @"Q4MSAzLjE0OTUzLDIuMjk2NzA2IHoiCiAgICAgICBzdHlsZT0iZmlsbDojZmZmZmZmO2ZpbGwtb3BhY2l0eToxIgogICAgICAgaW5rc2NhcGU6Y29ubmVjdG9yLWN1cnZhdHVyZT0iMCIKICAgICAgIHNvZGlwb" +
                    @"2RpOm5vZGV0eXBlcz0iY3NjY2NjY2NzY2NjYyIgLz4KICA8L2c+CiAgPGcKICAgICBzdHlsZT0iZGlzcGxheTppbmxpbmUiCiAgICAgaW5rc2NhcGU6Z3JvdXBtb2RlPSJsYXllciIKICAgICBpZD0iZzU2NjIi" +
                    @"CiAgICAgaW5rc2NhcGU6bGFiZWw9IlRvcCBLb3BpZSI+CiAgICA8cGF0aAogICAgICAgc29kaXBvZGk6bm9kZXR5cGVzPSJjc2NjY2NjY3NjY2NjIgogICAgICAgaW5rc2NhcGU6Y29ubmVjdG9yLWN1cnZhdHV" +
                    @"yZT0iMCIKICAgICAgIHN0eWxlPSJmaWxsOiNmZmZmZmY7ZmlsbC1vcGFjaXR5OjEiCiAgICAgICBkPSJtIDIyLjI0ODEyNSwyNS43NTQwOTUgYyAwLjQ3NDk3LDAuMjc1MjQ0IDEuMDI1MjQsMC40MzQyOTkgMS" +
                    @"42MTMxNjYsMC40MzQyOTkgMC42MjcwNzIsMCAxLjIxMjAxNiwtMC4xODA4NjQgMS43MDc4NTcsLTAuNDkyNTgxIDAuMDAyMiwwLjAwMzggMTIuNDg4NDk2LC03LjI2NTc1OCAxMi40ODg0OTYsLTcuMjY1NzU4I" +
                    @"DEuMDEyOTMzLC0wLjU1MDExMSAxLjc0OTA5MywtMC41OTQ3MiAxLjc0OTA5MywtMS44MzcwNzcgLTAuMDEzMzIsLTEuMTQ3NjUzIC0wLjM4MTE4MiwtMS43MTg0NjcgLTEuMDIwMjIxLC0xLjk1NTU5NCAtMS4x" +
                    @"NjcyMTksLTAuMjkxOTE3IC0xLjU4MDU4OSwwLjEzNDUwOSAtMy44ODM2MTcsMS40OTc4NDUgbCAtOS43MDk1NDYsNS42NTQ5MDYgYyAtMC4zODgwOTksMC4yMzgwMTggLTAuODQyOTMxLDAuMzc4MjcyIC0xLjM" +
                    @"zMDU3MSwwLjM3ODI3MiAtMC40NTcwNjcsMCAtMC44ODU0MjcsLTAuMTIzMzMzIC0xLjI1NDUxNywtMC4zMzg0MTUgbCAtOS43NjA5OSwtNS42ODY0OSBjIC01LjMyNDQyMzIsMC42NzI1NTQgLTAuNzE1NCwzLj" +
                    @"EwMjQ4MSAtMy4xNDk1MzAyLDIuMjk2NzA2IHoiCiAgICAgICBpZD0icGF0aDU2NjAiIC8+CiAgPC9nPgo8L3N2Zz4K')"">&nbsp;</div>
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