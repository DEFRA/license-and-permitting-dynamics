//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WastePermits.Model.EarlyBound
{
	
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.0.0.9369")]
	public enum defra_dulymadechecklistState
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
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("defra_dulymadechecklist")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "9.0.0.9369")]
	public partial class defra_dulymadechecklist : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		public static class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string defra_adequatesiteconditionreport = "defra_adequatesiteconditionreport";
			public const string defra_adequatesiteplan = "defra_adequatesiteplan";
			public const string defra_adequatetechnicalability = "defra_adequatetechnicalability";
			public const string defra_appendix1completed = "defra_appendix1completed";
			public const string defra_applicationId = "defra_applicationid";
			public const string defra_areaofficer = "defra_areaofficer";
			public const string defra_baselinereportacceptable = "defra_baselinereportacceptable";
			public const string defra_bespokepermitchecklist = "defra_bespokepermitchecklist";
			public const string defra_completedbyid = "defra_completedbyid";
			public const string defra_completedon = "defra_completedon";
			public const string defra_confirmedassrp = "defra_confirmedassrp";
			public const string defra_dulymadechecklistId = "defra_dulymadechecklistid";
			public const string Id = "defra_dulymadechecklistid";
			public const string defra_fireplanadequate = "defra_fireplanadequate";
			public const string defra_general_notes = "defra_general_notes";
			public const string defra_miningwastemanagementplanacceptable = "defra_miningwastemanagementplanacceptable";
			public const string defra_name = "defra_name";
			public const string defra_refusalconsidered = "defra_refusalconsidered";
			public const string defra_rejectedstatusreason = "defra_rejectedstatusreason";
			public const string defra_relevantdocumentation = "defra_relevantdocumentation";
			public const string defra_rvd_team_decision = "defra_rvd_team_decision";
			public const string defra_samplingreport = "defra_samplingreport";
			public const string defra_standardpermitchecklist = "defra_standardpermitchecklist";
			public const string defra_wasterecoveryplanacceptable = "defra_wasterecoveryplanacceptable";
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
			public const string defra_application_defra_dulymadechecklist = "defra_application_defra_dulymadechecklist";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public defra_dulymadechecklist() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "defra_dulymadechecklist";
		
		public const string PrimaryIdAttribute = "defra_dulymadechecklistid";
		
		public const string PrimaryNameAttribute = "defra_name";
		
		public const int EntityTypeCode = 10020;
		
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_adequatesiteconditionreport")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_adequatesiteconditionreport
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_adequatesiteconditionreport");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_adequatesiteconditionreport");
				this.SetAttributeValue("defra_adequatesiteconditionreport", value);
				this.OnPropertyChanged("defra_adequatesiteconditionreport");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_adequatesiteplan")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_adequatesiteplan
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_adequatesiteplan");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_adequatesiteplan");
				this.SetAttributeValue("defra_adequatesiteplan", value);
				this.OnPropertyChanged("defra_adequatesiteplan");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_adequatetechnicalability")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_adequatetechnicalability
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_adequatetechnicalability");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_adequatetechnicalability");
				this.SetAttributeValue("defra_adequatetechnicalability", value);
				this.OnPropertyChanged("defra_adequatetechnicalability");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_appendix1completed")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_appendix1completed
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_appendix1completed");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_appendix1completed");
				this.SetAttributeValue("defra_appendix1completed", value);
				this.OnPropertyChanged("defra_appendix1completed");
			}
		}
		
		/// <summary>
		/// Unique identifier for Application associated with Duly Made Checklist.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_applicationid")]
		public Microsoft.Xrm.Sdk.EntityReference defra_applicationId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("defra_applicationid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_applicationId");
				this.SetAttributeValue("defra_applicationid", value);
				this.OnPropertyChanged("defra_applicationId");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_areaofficer")]
		public Microsoft.Xrm.Sdk.EntityReference defra_areaofficer
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("defra_areaofficer");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_areaofficer");
				this.SetAttributeValue("defra_areaofficer", value);
				this.OnPropertyChanged("defra_areaofficer");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_baselinereportacceptable")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_baselinereportacceptable
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_baselinereportacceptable");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_baselinereportacceptable");
				this.SetAttributeValue("defra_baselinereportacceptable", value);
				this.OnPropertyChanged("defra_baselinereportacceptable");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_bespokepermitchecklist")]
		public System.Nullable<bool> defra_bespokepermitchecklist
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("defra_bespokepermitchecklist");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_bespokepermitchecklist");
				this.SetAttributeValue("defra_bespokepermitchecklist", value);
				this.OnPropertyChanged("defra_bespokepermitchecklist");
			}
		}
		
		/// <summary>
		/// Unique identifier for User associated with Duly Made Checklist.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_completedbyid")]
		public Microsoft.Xrm.Sdk.EntityReference defra_completedbyid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("defra_completedbyid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_completedbyid");
				this.SetAttributeValue("defra_completedbyid", value);
				this.OnPropertyChanged("defra_completedbyid");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_completedon")]
		public System.Nullable<System.DateTime> defra_completedon
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("defra_completedon");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_completedon");
				this.SetAttributeValue("defra_completedon", value);
				this.OnPropertyChanged("defra_completedon");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_confirmedassrp")]
		public System.Nullable<bool> defra_confirmedassrp
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("defra_confirmedassrp");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_confirmedassrp");
				this.SetAttributeValue("defra_confirmedassrp", value);
				this.OnPropertyChanged("defra_confirmedassrp");
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_dulymadechecklistid")]
		public System.Nullable<System.Guid> defra_dulymadechecklistId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("defra_dulymadechecklistid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_dulymadechecklistId");
				this.SetAttributeValue("defra_dulymadechecklistid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("defra_dulymadechecklistId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_dulymadechecklistid")]
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
				this.defra_dulymadechecklistId = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_fireplanadequate")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_fireplanadequate
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_fireplanadequate");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_fireplanadequate");
				this.SetAttributeValue("defra_fireplanadequate", value);
				this.OnPropertyChanged("defra_fireplanadequate");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_general_notes")]
		public string defra_general_notes
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("defra_general_notes");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_general_notes");
				this.SetAttributeValue("defra_general_notes", value);
				this.OnPropertyChanged("defra_general_notes");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_miningwastemanagementplanacceptable")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_miningwastemanagementplanacceptable
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_miningwastemanagementplanacceptable");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_miningwastemanagementplanacceptable");
				this.SetAttributeValue("defra_miningwastemanagementplanacceptable", value);
				this.OnPropertyChanged("defra_miningwastemanagementplanacceptable");
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
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_refusalconsidered")]
		public System.Nullable<bool> defra_refusalconsidered
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("defra_refusalconsidered");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_refusalconsidered");
				this.SetAttributeValue("defra_refusalconsidered", value);
				this.OnPropertyChanged("defra_refusalconsidered");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_rejectedstatusreason")]
		public string defra_rejectedstatusreason
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("defra_rejectedstatusreason");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_rejectedstatusreason");
				this.SetAttributeValue("defra_rejectedstatusreason", value);
				this.OnPropertyChanged("defra_rejectedstatusreason");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_relevantdocumentation")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_relevantdocumentation
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_relevantdocumentation");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_relevantdocumentation");
				this.SetAttributeValue("defra_relevantdocumentation", value);
				this.OnPropertyChanged("defra_relevantdocumentation");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_rvd_team_decision")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_rvd_team_decision
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_rvd_team_decision");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_rvd_team_decision");
				this.SetAttributeValue("defra_rvd_team_decision", value);
				this.OnPropertyChanged("defra_rvd_team_decision");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_samplingreport")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_samplingreport
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_samplingreport");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_samplingreport");
				this.SetAttributeValue("defra_samplingreport", value);
				this.OnPropertyChanged("defra_samplingreport");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_standardpermitchecklist")]
		public System.Nullable<bool> defra_standardpermitchecklist
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("defra_standardpermitchecklist");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_standardpermitchecklist");
				this.SetAttributeValue("defra_standardpermitchecklist", value);
				this.OnPropertyChanged("defra_standardpermitchecklist");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_wasterecoveryplanacceptable")]
		public Microsoft.Xrm.Sdk.OptionSetValue defra_wasterecoveryplanacceptable
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("defra_wasterecoveryplanacceptable");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_wasterecoveryplanacceptable");
				this.SetAttributeValue("defra_wasterecoveryplanacceptable", value);
				this.OnPropertyChanged("defra_wasterecoveryplanacceptable");
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
		/// Status of the Duly Made Checklist
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public System.Nullable<WastePermits.Model.EarlyBound.defra_dulymadechecklistState> StateCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((WastePermits.Model.EarlyBound.defra_dulymadechecklistState)(System.Enum.ToObject(typeof(WastePermits.Model.EarlyBound.defra_dulymadechecklistState), optionSet.Value)));
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
		/// Reason for the status of the Duly Made Checklist
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
		/// 1:N defra_dulymadechecklist_defra_application_dulymadechecklistid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_dulymadechecklist_defra_application_dulymadechecklistid")]
		public System.Collections.Generic.IEnumerable<WastePermits.Model.EarlyBound.defra_application> defra_dulymadechecklist_defra_application_dulymadechecklistid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<WastePermits.Model.EarlyBound.defra_application>("defra_dulymadechecklist_defra_application_dulymadechecklistid", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_dulymadechecklist_defra_application_dulymadechecklistid");
				this.SetRelatedEntities<WastePermits.Model.EarlyBound.defra_application>("defra_dulymadechecklist_defra_application_dulymadechecklistid", null, value);
				this.OnPropertyChanged("defra_dulymadechecklist_defra_application_dulymadechecklistid");
			}
		}
		
		/// <summary>
		/// N:1 defra_application_defra_dulymadechecklist
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_applicationid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("defra_application_defra_dulymadechecklist")]
		public WastePermits.Model.EarlyBound.defra_application defra_application_defra_dulymadechecklist
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<WastePermits.Model.EarlyBound.defra_application>("defra_application_defra_dulymadechecklist", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.OnPropertyChanging("defra_application_defra_dulymadechecklist");
				this.SetRelatedEntity<WastePermits.Model.EarlyBound.defra_application>("defra_application_defra_dulymadechecklist", null, value);
				this.OnPropertyChanged("defra_application_defra_dulymadechecklist");
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public defra_dulymadechecklist(object anonymousType) : 
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
                        Attributes["defra_dulymadechecklistid"] = base.Id;
                        break;
                    case "defra_dulymadechecklistid":
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
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_adequatesiteconditionreport")]
		public virtual defra_dulymadechecklist2? defra_adequatesiteconditionreportEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist2?)(EntityOptionSetEnum.GetEnum(this, "defra_adequatesiteconditionreport")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_adequatesiteconditionreport = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_adequatesiteplan")]
		public virtual defra_dulymadechecklist2? defra_adequatesiteplanEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist2?)(EntityOptionSetEnum.GetEnum(this, "defra_adequatesiteplan")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_adequatesiteplan = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_adequatetechnicalability")]
		public virtual defra_dulymadechecklist2? defra_adequatetechnicalabilityEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist2?)(EntityOptionSetEnum.GetEnum(this, "defra_adequatetechnicalability")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_adequatetechnicalability = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_appendix1completed")]
		public virtual defra_dulymadechecklist1? defra_appendix1completedEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist1?)(EntityOptionSetEnum.GetEnum(this, "defra_appendix1completed")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_appendix1completed = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_baselinereportacceptable")]
		public virtual defra_dulymadechecklist2? defra_baselinereportacceptableEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist2?)(EntityOptionSetEnum.GetEnum(this, "defra_baselinereportacceptable")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_baselinereportacceptable = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_fireplanadequate")]
		public virtual defra_dulymadechecklist2? defra_fireplanadequateEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist2?)(EntityOptionSetEnum.GetEnum(this, "defra_fireplanadequate")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_fireplanadequate = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_miningwastemanagementplanacceptable")]
		public virtual defra_dulymadechecklist2? defra_miningwastemanagementplanacceptableEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist2?)(EntityOptionSetEnum.GetEnum(this, "defra_miningwastemanagementplanacceptable")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_miningwastemanagementplanacceptable = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_relevantdocumentation")]
		public virtual defra_dulymadechecklist2? defra_relevantdocumentationEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist2?)(EntityOptionSetEnum.GetEnum(this, "defra_relevantdocumentation")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_relevantdocumentation = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_rvd_team_decision")]
		public virtual defra_dulymadechecklist_defra_rvd_team_decision? defra_rvd_team_decisionEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist_defra_rvd_team_decision?)(EntityOptionSetEnum.GetEnum(this, "defra_rvd_team_decision")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_rvd_team_decision = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_samplingreport")]
		public virtual defra_dulymadechecklist2? defra_samplingreportEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist2?)(EntityOptionSetEnum.GetEnum(this, "defra_samplingreport")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_samplingreport = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defra_wasterecoveryplanacceptable")]
		public virtual defra_dulymadechecklist2? defra_wasterecoveryplanacceptableEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist2?)(EntityOptionSetEnum.GetEnum(this, "defra_wasterecoveryplanacceptable")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				defra_wasterecoveryplanacceptable = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual defra_dulymadechecklist_StatusCode? StatusCodeEnum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((defra_dulymadechecklist_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				StatusCode = value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null;
			}
		}
	}
}