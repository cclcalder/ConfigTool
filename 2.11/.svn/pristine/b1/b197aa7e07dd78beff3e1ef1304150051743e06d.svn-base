using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
 
using System.Xml.Serialization;
using Telerik.Pivot.Core;

namespace Model.Utilities
{
    //[DataContract]
    //public class DataProviderSettings : Model.Utilities.Dynamic.Expando
    //{
    //    //[DataMember]
    //    //public string UserID { get; set; }

    //    //[DataMember]
    //    //public string ViewType { get; set; }
    //    //[DataMember]
    //    //public string ViewName { get; set; }

    //    [DataMember]
    //    public object[] Aggregates { get; set; }

    //    [DataMember]
    //    public object[] Filters { get; set; }

    //    [DataMember]
    //    public object[] Rows { get; set; }

    //    [DataMember]
    //    public object[] Columns { get; set; }

    //    [DataMember]
    //    public int AggregatesLevel { get; set; }

    //    [DataMember]
    //    public PivotAxis AggregatesPosition { get; set; }


    //    public DataProviderSettings()
    //        : base()
    //    { }

    //    public static DataProviderSettings Deserialise(XElement res)
    //    {
    //        var r = new XmlSerializer(typeof(DataProviderSettings));
    //        return (DataProviderSettings)r.Deserialize(res.CreateReader());

    //    }
    //}

    [DataContract]
    public class DataServiceProvider
    {
        [DataMember]
        public object[] Aggregates { get; set; }
        [DataMember]
        public object[] Filters { get; set; }
        [DataMember]
        public object[] Rows { get; set; }

        [DataMember]
        public object[] Columns { get; set; }
        [DataMember]
        public int AggregatesLevel { get; set; }

        [DataMember]
        public PivotAxis AggregatesPosition { get; set; }
    }


    public abstract class DataProviderSerializer
    {
        public abstract IEnumerable<Type> KnownTypes { get; }


        public string Serialize(object context)
        {
            string serialized = string.Empty;

            IDataProvider dataProvider = context as IDataProvider;
            if (dataProvider != null)
            {
                MemoryStream stream = new MemoryStream();

                DataServiceProvider settings = new DataServiceProvider()
                {
                    Aggregates = dataProvider.Settings.AggregateDescriptions.OfType<object>().ToArray(),
                    Filters = dataProvider.Settings.FilterDescriptions.OfType<object>().ToArray(),
                    Rows = dataProvider.Settings.RowGroupDescriptions.OfType<object>().ToArray(),
                    Columns = dataProvider.Settings.ColumnGroupDescriptions.OfType<object>().ToArray(),
                    AggregatesLevel = dataProvider.Settings.AggregatesLevel,
                    AggregatesPosition = dataProvider.Settings.AggregatesPosition
                };

                DataContractSerializer serializer = new DataContractSerializer(typeof(DataServiceProvider), KnownTypes);
                serializer.WriteObject(stream, settings);

                stream.Position = 0;
                var streamReader = new StreamReader(stream);
                serialized += streamReader.ReadToEnd();
            }

            return serialized;
        }

        //public string Serialize( string name,object context)
        //{
        //    string serialized = string.Empty;

        //    IDataProvider dataProvider = context as IDataProvider;
        //    if (dataProvider != null)
        //    {
        //        MemoryStream stream = new MemoryStream();

        //        DataProviderSettings settings = new DataProviderSettings()
        //        {
        //            UserID = User.CurrentUser.ID,
        //            ViewType = "Cube",
        //            ViewName = name,
        //            Aggregates = dataProvider.Settings.AggregateDescriptions.OfType<object>().ToArray(),
        //            Filters = dataProvider.Settings.FilterDescriptions.OfType<object>().ToArray(),
        //            Rows = dataProvider.Settings.RowGroupDescriptions.OfType<object>().ToArray(),
        //            Columns = dataProvider.Settings.ColumnGroupDescriptions.OfType<object>().ToArray(),
        //            AggregatesLevel = dataProvider.Settings.AggregatesLevel,
        //            AggregatesPosition = dataProvider.Settings.AggregatesPosition
        //        };

        //     //serialized = SerializeToString(settings);


        //        DataContractSerializer serializer = new DataContractSerializer(typeof(DataProviderSettings));
        //        serializer.WriteObject(stream, settings);

        //        stream.Position = 0;
        //        var streamReader = new StreamReader(stream);
        //        serialized += streamReader.ReadToEnd();
        //    }

        //    return serialized;
        //}

        public static string SerializeToString(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);

                return writer.ToString();
            }
        }

        public IDataProvider Deserialize(object context, string savedValue)
        {
            IDataProvider dataProvider = context as IDataProvider;
            if (dataProvider != null)
            {
                var stream = new MemoryStream();
                var tw = new StreamWriter(stream);
                tw.Write(savedValue);
                tw.Flush();
                stream.Position = 0;

                DataContractSerializer serializer = new DataContractSerializer(typeof(DataServiceProvider), KnownTypes);
                var result = serializer.ReadObject(stream);



                dataProvider.Settings.AggregateDescriptions.Clear();
                foreach (var aggregateDescription in (result as DataServiceProvider).Aggregates)
                {
                    dataProvider.Settings.AggregateDescriptions.Add(aggregateDescription);
                }

                dataProvider.Settings.FilterDescriptions.Clear();
                foreach (var filterDescription in (result as DataServiceProvider).Filters)
                {
                    dataProvider.Settings.FilterDescriptions.Add(filterDescription);
                }

                dataProvider.Settings.RowGroupDescriptions.Clear();
                foreach (var rowDescription in (result as DataServiceProvider).Rows)
                {
                    dataProvider.Settings.RowGroupDescriptions.Add(rowDescription);
                }

                dataProvider.Settings.ColumnGroupDescriptions.Clear();
                foreach (var columnDescription in (result as DataServiceProvider).Columns)
                {
                    dataProvider.Settings.ColumnGroupDescriptions.Add(columnDescription);
                }

                dataProvider.Settings.AggregatesPosition = (result as DataServiceProvider).AggregatesPosition;
                dataProvider.Settings.AggregatesLevel = (result as DataServiceProvider).AggregatesLevel;

                return dataProvider;
            }
            return null;
        }
    }

}
