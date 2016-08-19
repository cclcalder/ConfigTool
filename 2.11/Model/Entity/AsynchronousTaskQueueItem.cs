using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    class AsynchronousTaskQueueItem
    {
        public AsynchronousTaskQueueItem()
        {}

        private string m_taskID = "Task_ID";
        private string m_description = "Description";
        private string m_queuedDate = "Queued_Date";
        private string m_queuedBy = "Queued_By";
        private string m_startDate = "Start_Date";
        private string m_startDateIsEstimate = "Start_Date_Is_Estimate";
        private string m_finishedDate = "Finished_Date";
        private string m_finishedDateIsEstime = "Finished_Date_Is_Estimate";

        public string TaskID { get; set; }
        public string Description { get; set; }
        public string QueuedDate { get; set; }
        public string QueuedBy { get; set; }
        public string StartDate { get; set; }
        public string StartDateIsEstimate { get; set; }
        public string FinishedDate { get; set; }
        public string FinishedDateIsEstimate { get; set; }

        public bool StartDateIsEstimateBool { get; set; }
        public bool FinishDateIsEstimateBool { get; set; }

        public AsynchronousTaskQueueItem(XElement element)
        {
            TaskID = element.GetValue<string>(m_taskID);
            Description = element.GetValue<string>(m_description);
            QueuedDate = element.GetValue<string>(m_queuedDate);
            QueuedBy = element.GetValue<string>(m_queuedBy);
            StartDate = element.GetValue<string>(m_startDate);
            StartDateIsEstimate = element.GetValue<string>(m_startDateIsEstimate);
            StartDateIsEstimateBool = (StartDateIsEstimate == "1" ? true : false);
            FinishedDate = element.GetValue<string>(m_finishedDate);
            FinishedDateIsEstimate = element.GetValue<string>(m_finishedDateIsEstime);
            FinishDateIsEstimateBool = (FinishedDateIsEstimate == "1" ? true: false);

        }

        public string AsynchronousTaskQueueItemString()
        {
            string stringToReturn;

            List<string> thisList = new List<string>();
            thisList.Add("Account Plan Build");
            thisList.Add("Task ID: " + TaskID);
            thisList.Add("Descirption: " + Description);
            thisList.Add("Queue Date: " + QueuedDate);
            thisList.Add("Queued by: " + QueuedBy);
            thisList.Add("Start Date: " + StartDate);
            thisList.Add("Start Date Is Estimated: " + StartDateIsEstimateBool.ToString());
            thisList.Add("Finish Date: " + FinishedDate);
            thisList.Add("Finish Date Is Estimated: " + FinishDateIsEstimateBool.ToString());
            thisList.Add("--------------------------");


            stringToReturn = string.Join(Environment.NewLine, thisList);
            return stringToReturn;
        }
    }
}
