using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;

namespace FinancesSharp
{
	public class ConnectionStringHelper
	{
		public static Dictionary<String, String> Get(string connectionStringName)
		{
			return WebConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString
				.Split(';')
				.Select(x => x.Split('='))
				.ToDictionary(x => x[0], x => x[1]);
		}
	}
}
