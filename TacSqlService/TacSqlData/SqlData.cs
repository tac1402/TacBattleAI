// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections.Generic;

namespace Tac.Sql
{

	[Serializable]
	public class LogData
	{
		public string Operation { get; set; }
		public int CommandId { get; set; }
		public string CommandText { get; set; }
		public List<ParameterData> Parameters { get; set; }
	}

	[Serializable]
	public class ParameterData
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public string DbType { get; set; }
	}


	[Serializable]
	public class LogDataTable
	{
		public List<ColumnInfo> Columns { get; set; }
		public List<List<string?>> Rows { get; set; }
	}

	[Serializable]
	public class ColumnInfo
	{
		public string Name { get; set; }
		public string TypeName { get; set; }
	}

	// PipeConstants.cs
	public static class PipeConstants
	{
		public const string PipeName = "SqlDataPipe";
		public const string ServiceName = "TacSqlService";
		public const string ServicePathInstall = @"P:\TacSqlService\Install";
	}

}
