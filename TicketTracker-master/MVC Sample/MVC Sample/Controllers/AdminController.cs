using MVC_Sample.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Sample.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        List<TicketData> Last24HrsData;
        List<TicketData> MyDefectsData;

        [HttpPost]
        public ActionResult GetCenterData(string type)
        {
            DataTable dt = Get24HrsDataSOQL();
            if (dt != null)
            {
                Last24HrsData = LoadData(dt);
            }
            if (Last24HrsData.Count > 0)
            {
                ViewBag.FilteredData = Last24HrsData.Where(t => t.EscalationStatus == type).ToList();
            }
            else
            {
                return null;
            }
            return PartialView();
        }

        private DataTable Get24HrsDataSOQL()
        {
            //Cursor = Cursors.WaitCursor;
            try
            {
                string soqlQuery = "Select  Description__c,    " +
            " Escalation_ID__c,    Name, OwnerId,  " +
            "  Priority__c,  " +
            "  Escalation_Status__c, Summary__c, Support_Product__c " +
            "from Problem_Management_Escalation__c  where Support_Product__c in ('OneSite Affordable', 'OneSite Tax Credits', 'OneSite Leasing and Rents Rural Housing') " +
            "and DAY_ONLY(Date_Escalated__c)=LAST_N_DAYS:3 order by Name desc";
                //this.btnMore.Visible = false;
                //PartnerSample.binding.QueryOptionsValue = new sforce.QueryOptions();
                //PartnerSample.binding.QueryOptionsValue.batchSize = Convert.ToInt16(Application.UserAppDataRegistry.GetValue("batchSize", "500"));
                //PartnerSample.binding.QueryOptionsValue.batchSizeSpecified = true;

                sforce.QueryResult qr = SalesForcePartner.binding.query(
                    soqlQuery);
                if (qr.size > 0)
                {

                    DataTable dt = CreateDataTable(ParseFieldList(soqlQuery), qr);
                    return dt;
                }
                else
                {
                    return null;
                    //System.Windows.Forms.MessageBox.Show("No records matched query.", "Patner Sample", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                return null;
                //System.Windows.Forms.MessageBox.Show("Query failed: " + ex.Message, "Partner Sample", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            //Cursor = Cursors.Default;
        }

        public List<TicketData> LoadData(DataTable dtRecords)
        {
            List<TicketData> ticketdata = new List<TicketData>();
            if (dtRecords.Columns.Count > 0)
            {

                for (int i = 0; i <= dtRecords.Rows.Count - 1; i++)
                {
                    var ticket = dtRecords.Rows[i];
                    TicketData currentData = new TicketData();
                    currentData.EscalationId = ticket["Name"].ToString();
                    currentData.Summary = ticket["Summary__c"].ToString();
                    currentData.Priority = ticket["Priority__c"].ToString();
                    currentData.EscalationStatus = ticket["Escalation_Status__c"].ToString();
                    ticketdata.Add(currentData);
                }
            }
            return ticketdata;
        }

        private DataTable CreateDataTable(ArrayList fieldList, sforce.QueryResult qr)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < fieldList.Count; i++)
            {
                dt.Columns.Add(fieldList[i].ToString());
            }
            for (int i = 0; i < qr.records.Length; i++)
            {
                DataRow dr = dt.NewRow();
                sforce.sObject record = qr.records[i];
                if (dr.Table.Columns.Contains("ID"))
                    dr["Id"] = record.Id;
                for (int j = 0; j < record.Any.Length; j++)
                {
                    System.Xml.XmlElement xEl = (System.Xml.XmlElement)record.Any[j];
                    dr[xEl.LocalName] = xEl.InnerText;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private ArrayList ParseFieldList(String soqlText)
        {
            //remove the select statement
            String temp = soqlText.Substring(soqlText.ToLower().IndexOf(" ")).Trim();
            //remove the everything after the field list
            temp = temp.Substring(0, temp.ToLower().IndexOf("from") - 1);
            //remove all the spaces
            temp = temp.Replace(" ", "");
            //return the tokenized array
            String[] temparray = temp.Split(",".ToCharArray());
            ArrayList al = new ArrayList();
            for (int i = 0; i < temparray.Length; i++)
            {
                al.Add(temparray[i]);
            }
            return al;
        }
    }
}