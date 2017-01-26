using System;

public class DatabaseAttribute : Attribute
{
    public string TableName { get; set; }
    public string IdFieldName { get; set; }

    public DatabaseAttribute(string tableName, string idFieldName)
    {
        this.TableName = tableName;
        this.IdFieldName = idFieldName;
    }
}