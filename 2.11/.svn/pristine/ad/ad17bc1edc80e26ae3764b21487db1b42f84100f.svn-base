using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class BuildQueueClass
    {
        public BuildQueueClass()
        { }

        private string m_taskID = "Task_ID";
        private string m_description = "Description";
        private string m_queuedDate = "Queued_Date";
        private string m_queuedBy = "Queued_By";
        private string m_startDate = "Start_Date";
        private string m_startDateIsEstimate = "Start_Date_Is_Estimate";
        private string m_finishDate = "Finished_Date";
        private string m_finishDateIsEstimate = "Finish_Date_Is_Estimate";

        public string Task_ID { get; set; }
        public string Description { get; set; }
        public string Queue_Date { get; set; }
        public string Queued_By { get; set; }
        public string Start_Date { get; set; }
        private string StartDateIsEstimateString { get; set; }
        public string Finish_Date { get; set; }
        private string FinishDateIsEstimateString { get; set; }

        public bool Start_Date_Is_Estimated { get; set; }
        public bool Finish_Date_Is_Estimated { get; set; }

        private DateTime m_timeStamp;

        public DateTime GetTimeStamp()
        {
            return m_timeStamp;
        }

        public BuildQueueClass(XElement element)
        {
            Task_ID = element.GetValue<string>(m_taskID);
            Description = element.GetValue<string>(m_description);
            Queue_Date = element.GetValue<string>(m_queuedDate);
            Queued_By = element.GetValue<string>(m_queuedBy);
            Start_Date = element.GetValue<string>(m_startDate);
            StartDateIsEstimateString = element.GetValue<string>(m_startDateIsEstimate);
            Start_Date_Is_Estimated = (StartDateIsEstimateString == "1" ? true : false);
            Finish_Date = element.GetValue<string>(m_finishDate);
            FinishDateIsEstimateString = element.GetValue<string>(m_finishDateIsEstimate);
            Finish_Date_Is_Estimated = (FinishDateIsEstimateString == "1" ? true : false);

            m_timeStamp = new DateTime();
            m_timeStamp = DateTime.Now;
        }

        public string BuildQueueItemToString()
        {
            string stringToReturn;

            List<string> thisList = new List<string>();
            thisList.Add("Account Plan Build");
            thisList.Add("Task ID: " + Task_ID);
            thisList.Add("Descirption: " + Description);
            thisList.Add("Queue Date: " + Queue_Date);
            thisList.Add("Queued by: " + Queued_By);
            thisList.Add("Start Date: " + Start_Date);
            thisList.Add("Start Date Is Estimated: " + Start_Date_Is_Estimated.ToString());
            thisList.Add("Finish Date: " + Finish_Date);
            thisList.Add("Finish Date Is Estimated: " + Finish_Date_Is_Estimated.ToString());
            thisList.Add("_____________________________________________________________________________");


            stringToReturn = string.Join(Environment.NewLine, thisList);
            return stringToReturn;
        }
    }
}
