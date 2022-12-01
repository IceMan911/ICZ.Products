using System;

namespace ICZ.Products.DB
{
    //=============================================================================================
    /// <summary>
    /// Provides the mapping between the name of the source and target entity properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DataFieldAttribute : Attribute
    {
        #region Fields
        //-----------------------------------------------------------------------------------------
        private readonly string mName;
        #endregion
        #region Constructor
        //---------------------------------------------------------------------------------------------
        public DataFieldAttribute(string aName)
        {
            mName = aName;
        }
        #endregion
        #region DataFieldAttribute
        //---------------------------------------------------------------------------------------------
        public string Name
        {
            get { return mName; }
        }
        #endregion
    }
}
