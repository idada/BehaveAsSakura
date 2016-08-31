﻿using ProtoBuf;

namespace BehaveAsSakura.Variables
{
	public enum VariableType : byte
	{
		Byte,
		SByte,
		Short,
		UShort,
		Integer,
		UInteger,
		Long,
		ULong,
		Float,
		Double,
		String,
	}

	public enum VariableSource : byte
	{
		GlobalConstant,
		OwnerPropertySet,
		AncestorTaskPropertySet,
		LiteralConstant,
	}

	[ProtoContract]
	public class VariableDesc
	{
		[ProtoMember( 1 )]
		public VariableType Type { get; set; }

		[ProtoMember( 2 )]
		public VariableSource Source { get; set; }

		[ProtoMember( 3 )]
		public string Value { get; set; }
	}
}