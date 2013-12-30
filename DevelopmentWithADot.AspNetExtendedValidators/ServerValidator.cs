using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DevelopmentWithADot.AspNetExtendedValidators
{
	public sealed class ServerValidator : CustomValidator, IPostBackEventHandler
	{
		protected override void OnPreRender(EventArgs e)
		{
			if ((this.Visible == true) && (this.Enabled == true))
			{
				String script = "\nfunction ValidateServerValidator(sender, args)\n" +
								"{\n" +
								"	var xhr = null;\n" +
								"	if (window.XMLHttpRequest)\n" +
								"	{\n" +
								"		xhr = new XMLHttpRequest();\n" +
								"	}\n" +
								"	else\n" +
								"	{\n" +
								"		xhr = new ActiveXObject('Microsoft.XMLHTTP');\n" +
								"	}\n" +
								"	xhr.open('POST', document.location.href, false);\n" +
								"	xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');\n" +
								"	xhr.setRequestHeader('x-microsoftajax', 'Delta=true');\n" +
								"	xhr.setRequestHeader('x-requested-with', 'XMLHttpRequest');\n" +
								"	xhr.send('__ASYNCPOST=true&__EVENTARGUMENT=' + escape(args.Value) + '&__EVENTTARGET=' + sender.id.replace('_', '%24'));\n" +
								"	var array = xhr.responseText.split('|');\n" +
								"	args.IsValid = (array[0] == 'true');\n" +
								"}\n";

				this.ClientValidationFunction = "ValidateServerValidator";

				if (this.Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "ValidateServerValidator") == false)
				{
					this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ValidateServerValidator", script, true);
				}
			}

			base.OnPreRender(e);
		}

		protected override void OnInit(EventArgs e)
		{
			if ((this.Visible == true) && (this.Enabled == true))
			{
				this.Page.RegisterRequiresRaiseEvent(this);
			}

			base.OnInit(e);
		}

		private void OnValidate()
		{
			this.Context.Response.Clear();
			this.Context.Response.Write(String.Concat(this.OnServerValidate(this.Context.Request.Form [ "__EVENTARGUMENT" ]).ToString().ToLower(), "|"));
		}

		#region IPostBackEventHandler Members

		void IPostBackEventHandler.RaisePostBackEvent(String eventArgument)
		{
			this.OnValidate();
		}

		#endregion
	}
}