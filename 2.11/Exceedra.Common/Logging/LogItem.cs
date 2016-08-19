using System;
 
using System.Collections.Generic;
using System.IO;
using System.Linq;
 
using System.Threading;
 
namespace Exceedra.Common.Logging
{

    public class StorageBase
    {
        private static string _messageTemplate
        {
            get
            {
                return
                    "______________________________START {0}_______________________________________" +
                    Environment.NewLine +
                    "Method: {1} - At: @" + DateTime.Now +
                    Environment.NewLine +
                    "Argument: " +
                    Environment.NewLine + 
                    "{2}" +
                    Environment.NewLine +
                    "________________________________END {0}________________________________________" +
                    Environment.NewLine + Environment.NewLine + Environment.NewLine;
            }
        }

        private static string _runnerTemplate
        {
            get
            {
                return
                    "insert into scripts (method) values ('{0}||{1}')" +
                    Environment.NewLine;
            }
        }    

        private static readonly string LogFileName = AppDomain.CurrentDomain.GetPath() + "Log.txt";

        private static readonly object LoggingSyncObject = new object();

        public static void LogMessageToFile(string type, string method, string body, string u)
        {
           
            StorageBase.Add(new LogItem(type, method, body, u));
            // #if (DEBUG)
            lock (LoggingSyncObject)
                Retry(() => File.AppendAllText(LogFileName, string.Format(_messageTemplate,type,method,body)), 3, true);
            // #endif
        }


        public static void LogCallToFile(string method, string body)
        {            
            // #if (DEBUG)
          //lock (LoggingSyncObject)
          //      Retry(() => File.AppendAllText(AppDomain.CurrentDomain.GetPath() + "runner.txt", string.Format(_runnerTemplate, method, body)), 3, true);
            // #endif
        }

        private static void Retry(Action action, int retryCount, bool failSilently = false, int delay = 10)
        {
            while (retryCount-- > 0)
            {
                try
                {
                    action();
                    break;
                }
                catch (Exception)
                {
                    if (retryCount == 0 && !failSilently) throw;
                    Thread.Sleep(delay);
                }
            }
        }

        public static List<LogItem> Items
        {
            get { return _items.OrderByDescending(r => r.DateStamp).ToList(); }
        }

        private static List<LogItem>  _items { get; set; }

        public static void Add(LogItem l)
        {
            if (_items == null)
            {
                _items = new List<LogItem>();
            }

            _items.Add(l);
            l.ID = _items.Count + 1;
        }

        public static void Clear()
        {
            if (_items != null)
            {
                _items.Clear();
            }
        }
    }

    public class LogItem
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public string Response { get; set; }
        public string ResponseShort
        {
            get
            {
                if (Response == null) return string.Empty;

                if (Response.Length >= 100) return Response.Substring(0, 100) + " ... ";
                return Response;
            }
            set
            {
                // should be blank - just to make xml cells editable in the app trace
            }
        }
        public string UserID { get; set; }
        public DateTime DateStamp { get; set; }
        public string error { get { return Type.ToLower().Contains("error") ? "1" : "0"; } }

         
        public LogItem(string t, string m, string r, string u )
        {
            ID = 0;
            Type = t;
            Method = m;
            Response = r; 
            UserID = (u);
            DateStamp = DateTime.Now;

          
        }
    }
}
