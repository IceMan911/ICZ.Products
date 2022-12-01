using System;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace ICZ.Products.DB
{
	//=============================================================================================
	/// <summary>
	/// Class that will map the public properties of a .NET class to the columns in a DataReader.
	/// For example mapping Customer class properties to a SQL DataReader column names.
	/// This is achieved using .NET Reflection and the custom class DataFieldAttribute which is used
	/// to decorate the .NET class properties with the corresponding DataReader column name.
	/// If you don't understand Reflection then do NOT amend this code!
	/// </summary>
	/// <example>See the class definition for DebtorEntity and the implementation for DebtorData.GetDebtorInfo() as an example.</example>
	public static class ReflectPropertyInfo
	{
		#region ReflectType
		//-----------------------------------------------------------------------------------------
		public static TEntity ReflectType<TEntity>(IDataRecord aDtaRcrd) where TEntity : class, new()
		{
			TEntity lInstanceToPopulate = new TEntity();
			PropertyInfo[] lPropertyInfos = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			//for each public property on the original
			foreach (PropertyInfo lPrptInf in lPropertyInfos)
			{
				DataFieldAttribute[] lDatafieldAttributeArray = lPrptInf.GetCustomAttributes(typeof(DataFieldAttribute),
																						false) as DataFieldAttribute[];
				//this attribute is marked with AllowMultiple=false
				if (lDatafieldAttributeArray != null && lDatafieldAttributeArray.Length == 1)
				{
					DataFieldAttribute lDtaFldAttr = lDatafieldAttributeArray[0];
					//this will blow up if the datareader does not contain the item keyed dfa.Name
					object dbValue = aDtaRcrd[lDtaFldAttr.Name];
					if (dbValue != null)
					{
						lPrptInf.SetValue(lInstanceToPopulate, Convert.ChangeType(dbValue, lPrptInf.PropertyType, CultureInfo.InvariantCulture), null);
					}
				}
			}
			return lInstanceToPopulate;
		}
		#endregion
		#region ReflectType
		//-----------------------------------------------------------------------------------------
		public static TEntity ReflectType<TEntity>(DataRow aDtaRcrd) where TEntity : class, new()
		{
			TEntity lInstanceToPopulate = new TEntity();
			PropertyInfo[] lPropertyInfos = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			//for each public property on the original
			foreach (PropertyInfo lPrptInf in lPropertyInfos)
			{
				DataFieldAttribute[] lDatafieldAttributeArray = lPrptInf.GetCustomAttributes(typeof(DataFieldAttribute),
																						false) as DataFieldAttribute[];
				//this attribute is marked with AllowMultiple=false
				if (lDatafieldAttributeArray != null && lDatafieldAttributeArray.Length == 1)
				{
					DataFieldAttribute lDtaFldAttr = lDatafieldAttributeArray[0];
					//this will blow up if the datareader does not contain the item keyed dfa.Name
					object dbValue = aDtaRcrd[lDtaFldAttr.Name];
					if (dbValue != null && dbValue != DBNull.Value)
					{
						object value = null;
						if (dbValue == null)
						{
							value = null;
						}
						else
						{
							Type type = Nullable.GetUnderlyingType(lPrptInf.PropertyType) ?? lPrptInf.PropertyType;
							if (type.BaseType == typeof(System.Enum))
								type = typeof(System.Int32);
							value = Convert.ChangeType(dbValue, type, CultureInfo.InvariantCulture);
						}
						lPrptInf.SetValue(lInstanceToPopulate, value, null);
					}
				}
			}
			return lInstanceToPopulate;
		}
		#endregion
	}
}
