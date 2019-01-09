//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lp.Model.EarlyBound
{
	
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.0.0.9369")]
	public enum defra_permitState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// 
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("defra_permit")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.0.0.9369")]
	public partial class defra_permit : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string defra_area_team = "defra_area_team";
			public const string defra_currentnumber = "defra_currentnumber";
			public const string defra_customerid = "defra_customerid";
			public const string defra_eawmlnumber = "defra_eawmlnumber";
			public const string defra_locked = "defra_locked";
			public const string defra_name = "defra_name";
			public const string defra_permitdocumentlocation = "defra_permitdocumentlocation";
			public const string defra_permitId = "defra_permitid";
			public const string Id = "defra_permitid";
			public const string defra_permitnumber = "defra_permitnumber";
			public const string defra_primarycontactid = "defra_primarycontactid";
			public const string defra_updatesharepointmetadata = "defra_updatesharepointmetadata";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string OwnerId = "ownerid";
			public const string OwningBusinessUnit = "owningbusinessunit";
			public const string OwningTeam = "owningteam";
			public const string OwningUser = "owninguser";
			public const string StateCode = "statecode";
			public const string StatusCode = "statuscode";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
			public const string defra_account_defra_permit_Customer = "defra_account_defra_permit_Customer";
			public const string defra_contact_defra_permit_Customer = "defra_contact_defra_permit_Customer";
			public const string defra_contact_defra_permit_primarycontactid = "defra_contact_defra_permit_primarycontactid";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public defra_permit() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "defra_permit";
		
		public const string PrimaryIdAttribute = "defra_permitid";
		
		public const string PrimaryNameAttribute = "defra_name";
		
		public const int EntityTypeCode = 10028;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		[System.Diagnostics.DebuggerNonUserCode()]
		private void OnPropertyChanged(string propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		private void OnPropertyChanging(string propertyName)
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdby");
			}
		}
		
		/// <summary>
		/// Date and time when the record was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdon")]
		public System.Nullable<System.DateTime> CreatedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("createdon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who created the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedOnBehalfBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdonbehalfby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("CreatedOnBehalfBy");
				this.SetAttributeValue("createdonbehalfby", value);
				this.OnPropertyChanged("CreatedOnBehalfBy");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_area_team")]
		public Microsoft.Xrm.Sdk.EntityReference defra_area_team
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("defra_area_team");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_area_team");
				this.SetAttributeValue("defra_area_team", value);
				this.OnPropertyChanged("defra_area_team");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_currentnumber")]
		public System.Nullable<int> defra_currentnumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("defra_currentnumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_currentnumber");
				this.SetAttributeValue("defra_currentnumber", value);
				this.OnPropertyChanged("defra_currentnumber");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_customerid")]
		public Microsoft.Xrm.Sdk.EntityReference defra_customerid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("defra_customerid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_customerid");
				this.SetAttributeValue("defra_customerid", value);
				this.OnPropertyChanged("defra_customerid");
			}
		}
		
		/// <summary>
		/// Will be used by downstream systems
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_eawmlnumber")]
		public string defra_eawmlnumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("defra_eawmlnumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_eawmlnumber");
				this.SetAttributeValue("defra_eawmlnumber", value);
				this.OnPropertyChanged("defra_eawmlnumber");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_locked")]
		public System.Nullable<bool> defra_locked
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("defra_locked");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_locked");
				this.SetAttributeValue("defra_locked", value);
				this.OnPropertyChanged("defra_locked");
			}
		}
		
		/// <summary>
		/// The name of the custom entity.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_name")]
		public string defra_name
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("defra_name");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_name");
				this.SetAttributeValue("defra_name", value);
				this.OnPropertyChanged("defra_name");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_permitdocumentlocation")]
		public Microsoft.Xrm.Sdk.EntityReference defra_permitdocumentlocation
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("defra_permitdocumentlocation");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_permitdocumentlocation");
				this.SetAttributeValue("defra_permitdocumentlocation", value);
				this.OnPropertyChanged("defra_permitdocumentlocation");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_permitid")]
		public System.Nullable<System.Guid> defra_permitId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("defra_permitid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_permitId");
				this.SetAttributeValue("defra_permitid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("defra_permitId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_permitid")]
		public override System.Guid Id
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return base.Id;
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.defra_permitId = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_permitnumber")]
		public string defra_permitnumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("defra_permitnumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_permitnumber");
				this.SetAttributeValue("defra_permitnumber", value);
				this.OnPropertyChanged("defra_permitnumber");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_primarycontactid")]
		public Microsoft.Xrm.Sdk.EntityReference defra_primarycontactid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("defra_primarycontactid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_primarycontactid");
				this.SetAttributeValue("defra_primarycontactid", value);
				this.OnPropertyChanged("defra_primarycontactid");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_updatesharepointmetadata")]
		public System.Nullable<bool> defra_updatesharepointmetadata
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("defra_updatesharepointmetadata");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_updatesharepointmetadata");
				this.SetAttributeValue("defra_updatesharepointmetadata", value);
				this.OnPropertyChanged("defra_updatesharepointmetadata");
			}
		}
		
		/// <summary>
		/// Sequence number of the import that created this record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("importsequencenumber")]
		public System.Nullable<int> ImportSequenceNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("importsequencenumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("ImportSequenceNumber");
				this.SetAttributeValue("importsequencenumber", value);
				this.OnPropertyChanged("ImportSequenceNumber");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who modified the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedby");
			}
		}
		
		/// <summary>
		/// Date and time when the record was modified.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedon")]
		public System.Nullable<System.DateTime> ModifiedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("modifiedon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who modified the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedOnBehalfBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedonbehalfby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("ModifiedOnBehalfBy");
				this.SetAttributeValue("modifiedonbehalfby", value);
				this.OnPropertyChanged("ModifiedOnBehalfBy");
			}
		}
		
		/// <summary>
		/// Date and time that the record was migrated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("overriddencreatedon")]
		public System.Nullable<System.DateTime> OverriddenCreatedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("overriddencreatedon");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("OverriddenCreatedOn");
				this.SetAttributeValue("overriddencreatedon", value);
				this.OnPropertyChanged("OverriddenCreatedOn");
			}
		}
		
		/// <summary>
		/// Owner Id
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ownerid")]
		public Microsoft.Xrm.Sdk.EntityReference OwnerId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ownerid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("OwnerId");
				this.SetAttributeValue("ownerid", value);
				this.OnPropertyChanged("OwnerId");
			}
		}
		
		/// <summary>
		/// Unique identifier for the business unit that owns the record
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		public Microsoft.Xrm.Sdk.EntityReference OwningBusinessUnit
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owningbusinessunit");
			}
		}
		
		/// <summary>
		/// Unique identifier for the team that owns the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		public Microsoft.Xrm.Sdk.EntityReference OwningTeam
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owningteam");
			}
		}
		
		/// <summary>
		/// Unique identifier for the user that owns the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		public Microsoft.Xrm.Sdk.EntityReference OwningUser
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owninguser");
			}
		}
		
		/// <summary>
		/// Status of the Permit
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<Lp.Model.EarlyBound.defra_permitState> StateCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((Lp.Model.EarlyBound.defra_permitState)(System.Enum.ToObject(typeof(Lp.Model.EarlyBound.defra_permitState), optionSet.Value)));
				}
				else
				{
					return null;
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("StateCode");
				if ((value == null))
				{
					this.SetAttributeValue("statecode", null);
				}
				else
				{
					this.SetAttributeValue("statecode", new Microsoft.Xrm.Sdk.OptionSetValue(((int)(value))));
				}
				this.OnPropertyChanged("StateCode");
			}
		}
		
		/// <summary>
		/// Reason for the status of the Permit
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public Microsoft.Xrm.Sdk.OptionSetValue StatusCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statuscode");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("StatusCode");
				this.SetAttributeValue("statuscode", value);
				this.OnPropertyChanged("StatusCode");
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("timezoneruleversionnumber")]
		public System.Nullable<int> TimeZoneRuleVersionNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("timezoneruleversionnumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("TimeZoneRuleVersionNumber");
				this.SetAttributeValue("timezoneruleversionnumber", value);
				this.OnPropertyChanged("TimeZoneRuleVersionNumber");
			}
		}
		
		/// <summary>
		/// Time zone code that was in use when the record was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("utcconversiontimezonecode")]
		public System.Nullable<int> UTCConversionTimeZoneCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("utcconversiontimezonecode");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("UTCConversionTimeZoneCode");
				this.SetAttributeValue("utcconversiontimezonecode", value);
				this.OnPropertyChanged("UTCConversionTimeZoneCode");
			}
		}
		
		/// <summary>
		/// Version Number
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("versionnumber")]
		public System.Nullable<long> VersionNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<long>>("versionnumber");
			}
		}
		
		/// <summary>
		/// 1:N defra_defra_permit_defra_location_permitid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_defra_permit_defra_location_permitid")]
		public System.Collections.Generic.IEnumerable<Lp.Model.EarlyBound.defra_location> defra_defra_permit_defra_location_permitid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<Lp.Model.EarlyBound.defra_location>("defra_defra_permit_defra_location_permitid", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_defra_permit_defra_location_permitid");
				this.SetRelatedEntities<Lp.Model.EarlyBound.defra_location>("defra_defra_permit_defra_location_permitid", null, value);
				this.OnPropertyChanged("defra_defra_permit_defra_location_permitid");
			}
		}
		
		/// <summary>
		/// 1:N defra_defra_permit_defra_permit_lines_permitid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_defra_permit_defra_permit_lines_permitid")]
		public System.Collections.Generic.IEnumerable<Lp.Model.EarlyBound.defra_permit_lines> defra_defra_permit_defra_permit_lines_permitid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<Lp.Model.EarlyBound.defra_permit_lines>("defra_defra_permit_defra_permit_lines_permitid", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_defra_permit_defra_permit_lines_permitid");
				this.SetRelatedEntities<Lp.Model.EarlyBound.defra_permit_lines>("defra_defra_permit_defra_permit_lines_permitid", null, value);
				this.OnPropertyChanged("defra_defra_permit_defra_permit_lines_permitid");
			}
		}
		
		/// <summary>
		/// 1:N defra_permit_Annotations
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_permit_Annotations")]
		public System.Collections.Generic.IEnumerable<Lp.Model.EarlyBound.Annotation> defra_permit_Annotations
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<Lp.Model.EarlyBound.Annotation>("defra_permit_Annotations", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_permit_Annotations");
				this.SetRelatedEntities<Lp.Model.EarlyBound.Annotation>("defra_permit_Annotations", null, value);
				this.OnPropertyChanged("defra_permit_Annotations");
			}
		}
		
		/// <summary>
		/// 1:N defra_permit_defra_application
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_permit_defra_application")]
		public System.Collections.Generic.IEnumerable<Lp.Model.EarlyBound.defra_application> defra_permit_defra_application
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<Lp.Model.EarlyBound.defra_application>("defra_permit_defra_application", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_permit_defra_application");
				this.SetRelatedEntities<Lp.Model.EarlyBound.defra_application>("defra_permit_defra_application", null, value);
				this.OnPropertyChanged("defra_permit_defra_application");
			}
		}
		
		/// <summary>
		/// 1:N defra_permit_defra_areacomments
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_permit_defra_areacomments")]
		public System.Collections.Generic.IEnumerable<Lp.Model.EarlyBound.defra_areacomment> defra_permit_defra_areacomments
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<Lp.Model.EarlyBound.defra_areacomment>("defra_permit_defra_areacomments", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_permit_defra_areacomments");
				this.SetRelatedEntities<Lp.Model.EarlyBound.defra_areacomment>("defra_permit_defra_areacomments", null, value);
				this.OnPropertyChanged("defra_permit_defra_areacomments");
			}
		}
		
		/// <summary>
		/// 1:N defra_permit_defra_notifications
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_permit_defra_notifications")]
		public System.Collections.Generic.IEnumerable<Lp.Model.EarlyBound.defra_notification> defra_permit_defra_notifications
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<Lp.Model.EarlyBound.defra_notification>("defra_permit_defra_notifications", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_permit_defra_notifications");
				this.SetRelatedEntities<Lp.Model.EarlyBound.defra_notification>("defra_permit_defra_notifications", null, value);
				this.OnPropertyChanged("defra_permit_defra_notifications");
			}
		}
		
		/// <summary>
		/// N:1 defra_account_defra_permit_Customer
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_customerid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_account_defra_permit_Customer")]
		public Lp.Model.EarlyBound.Account defra_account_defra_permit_Customer
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<Lp.Model.EarlyBound.Account>("defra_account_defra_permit_Customer", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_account_defra_permit_Customer");
				this.SetRelatedEntity<Lp.Model.EarlyBound.Account>("defra_account_defra_permit_Customer", null, value);
				this.OnPropertyChanged("defra_account_defra_permit_Customer");
			}
		}
		
		/// <summary>
		/// N:1 defra_contact_defra_permit_Customer
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_customerid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_contact_defra_permit_Customer")]
		public Lp.Model.EarlyBound.Contact defra_contact_defra_permit_Customer
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<Lp.Model.EarlyBound.Contact>("defra_contact_defra_permit_Customer", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_contact_defra_permit_Customer");
				this.SetRelatedEntity<Lp.Model.EarlyBound.Contact>("defra_contact_defra_permit_Customer", null, value);
				this.OnPropertyChanged("defra_contact_defra_permit_Customer");
			}
		}
		
		/// <summary>
		/// N:1 defra_contact_defra_permit_primarycontactid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_primarycontactid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_contact_defra_permit_primarycontactid")]
		public Lp.Model.EarlyBound.Contact defra_contact_defra_permit_primarycontactid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<Lp.Model.EarlyBound.Contact>("defra_contact_defra_permit_primarycontactid", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_contact_defra_permit_primarycontactid");
				this.SetRelatedEntity<Lp.Model.EarlyBound.Contact>("defra_contact_defra_permit_primarycontactid", null, value);
				this.OnPropertyChanged("defra_contact_defra_permit_primarycontactid");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public defra_permit(object anonymousType) : 
				this()
		{
            foreach (var p in anonymousType.GetType().GetProperties())
            {
                var value = p.GetValue(anonymousType, null);
                var name = p.Name.ToLower();
            
                if (name.EndsWith("enum") && value.GetType().BaseType == typeof(System.Enum))
                {
                    value = new Microsoft.Xrm.Sdk.OptionSetValue((int) value);
                    name = name.Remove(name.Length - "enum".Length);
                }
            
                switch (name)
                {
                    case "id":
                        base.Id = (System.Guid)value;
                        Attributes["defra_permitid"] = base.Id;
                        break;
                    case "defra_permitid":
                        var id = (System.Nullable<System.Guid>) value;
                        if(id == null){ continue; }
                        base.Id = id.Value;
                        Attributes[name] = base.Id;
                        break;
                    case "formattedvalues":
                        // Add Support for FormattedValues
                        FormattedValues.AddRange((Microsoft.Xrm.Sdk.FormattedValueCollection)value);
                        break;
                    default:
                        Attributes[name] = value;
                        break;
                }
            }
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual defra_permit_StatusCode? StatusCodeEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_permit_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				StatusCode = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
	}
}