using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Model.Entity.Generic;

namespace Model.Entity.Demand
{
    [Serializable]
    public class DemandFilterObject : FilterObject
    {
        public string TrialIdx { get; set; }
        public string TabIdx { get; set; }

        public new static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }    
}