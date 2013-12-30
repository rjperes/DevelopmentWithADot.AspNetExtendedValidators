using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;

namespace DevelopmentWithADot.AspNetExtendedValidators
{
	public sealed class DataAnnotationsValidator : BaseValidator
	{
		#region Protected override methods
		protected override void OnInit(EventArgs e)
		{
			if ((this.Enabled == true) && (this.Visible == true))
			{
				this.Page.RegisterRequiresControlState(this);
			}

			base.OnInit(e);
		}

		protected override void LoadControlState(Object savedState)
		{
			Object [] state = savedState as Object [];
			base.LoadControlState(state [ 0 ]);
			this.DisplayMode = (ValidationSummaryDisplayMode) state [ 1 ];
			this.PropertyName = (String) state [ 2 ];
			this.SourceTypeName = (String) state [ 3 ];
			this.FirstErrorOnly = (Boolean) state [ 4 ];
		}

		protected override Object SaveControlState()
		{
			Object [] state = new Object [ 5 ];
			state [ 0 ] = base.SaveControlState();
			state [ 1 ] = this.DisplayMode;
			state [ 2 ] = this.PropertyName;
			state [ 3 ] = this.SourceTypeName;
			state [ 4 ] = this.FirstErrorOnly;
			return (state);
		}

		protected override Boolean EvaluateIsValid()
		{
			if ((String.IsNullOrEmpty(this.SourceTypeName) == true) || (String.IsNullOrEmpty(this.PropertyName) == true))
			{
				return (true);
			}

			Type type = Type.GetType(this.SourceTypeName, false);

			if (type != null)
			{
				PropertyInfo prop = type.GetProperty(this.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

				if (prop != null)
				{
					ValidationAttribute [] attrs = prop.GetCustomAttributes(typeof(ValidationAttribute), true) as ValidationAttribute [];
					List<ValidationException> errors = new List<ValidationException>();
					String value = this.GetControlValidationValue(this.ControlToValidate);

					if (attrs.Length == 0)
					{
						MetadataTypeAttribute [] metadata = type.GetCustomAttributes(typeof(MetadataTypeAttribute), true) as MetadataTypeAttribute [];

						if (metadata.Length != 0)
						{
							prop = metadata [ 0 ].MetadataClassType.GetProperty(this.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

							if (prop != null)
							{
								attrs = prop.GetCustomAttributes(typeof(ValidationAttribute), true) as ValidationAttribute [];
							}
						}
					}

					for (Int32 i = 0; i < attrs.Length; ++i)
					{
						try
						{
							attrs [ i ].Validate(value, prop.Name);
						}
						catch (ValidationException ve)
						{
							errors.Add(ve);
						}
						catch
						{
						}

						if ((this.FirstErrorOnly == true) && (errors.Count != 0))
						{
							break;
						}
					}

					if (String.IsNullOrEmpty(this.ErrorMessage) == true)
					{
						this.ErrorMessage = this.formatErrorMessage(errors);
					}

					return (errors.Count == 0);
				}
			}

			return (true);
		}
		#endregion

		#region Private methods
		private String formatErrorMessage(IList<ValidationException> results)
		{
			String str = String.Empty;
			String str2 = String.Empty;
			String str3 = String.Empty;
			String str4 = String.Empty;
			StringBuilder builder = new StringBuilder();

			switch (this.DisplayMode)
			{
				case ValidationSummaryDisplayMode.List:
					str3 = "<br/>";
					break;

				case ValidationSummaryDisplayMode.SingleParagraph:
					str3 = " ";
					str4 = "<br/>";
					break;

				default:
					str = "<ul>";
					str2 = "<li>";
					str3 = "</li>";
					str4 = "</ul>";
					break;
			}

			if (results.Count != 0)
			{
				builder.Append(str);

				foreach (ValidationException result in results)
				{
					builder.Append(str2);
					builder.Append(result.Message);
					builder.Append(str3);
				}

				builder.Append(str4);
			}

			return (builder.ToString());
		}
		#endregion

		#region Public properties
		[Browsable(true)]
		[DefaultValue(false)]
		public Boolean FirstErrorOnly
		{
			get;
			set;
		}

		[Browsable(true)]
		[DefaultValue(ValidationSummaryDisplayMode.List)]
		public ValidationSummaryDisplayMode DisplayMode
		{
			get;
			set;
		}

		[Browsable(true)]
		[DefaultValue("")]
		public String PropertyName
		{
			get;
			set;
		}

		[Browsable(true)]
		[DefaultValue("")]
		public String SourceTypeName
		{
			get;
			set;
		}
		#endregion
	}
}
