using System;
using System.Web.UI.WebControls;

namespace DevelopmentWithADot.AspNetExtendedValidators
{
	public sealed class ConfirmValidator : CustomValidator
	{
		public String ConfirmationMessage
		{
			get;
			set;
		}

		protected override void OnPreRender(EventArgs e)
		{
			if ((this.Enabled == true) && (this.Visible == true))
			{
				String script = String.Concat("function confirmValidation", this.ClientID, "(sender, args) { var message = '", this.ConfirmationMessage, "'.replace('{0}', args.Value); args.IsValid = window.confirm(message); }");

				this.ClientValidationFunction = String.Concat("confirmValidation", this.ClientID);

				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), String.Concat("confirmValidation", this.ClientID), script, true);
			}

			base.OnPreRender(e);
		}
	}
}
