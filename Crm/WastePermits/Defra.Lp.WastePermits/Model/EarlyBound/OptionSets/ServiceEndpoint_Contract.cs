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
	public enum ServiceEndpoint_Contract
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		EventHub = 7,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		OneWay = 1,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Queue = 2,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Queue_Persistent = 6,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Rest = 3,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Topic = 5,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		TwoWay = 4,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Webhook = 8,
	}
}