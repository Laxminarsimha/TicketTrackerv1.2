using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MVC_Sample.Models;
using System.Data;
using System.Collections;

namespace MVC_Sample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        List<TicketData> Last24HrsData;
        List<TicketData> MyDefectsData;
        public ActionResult GetLast24HrsDeftecsList()
        {
            ForceLoginController forceLogin = new ForceLoginController();
            if (forceLogin.SaleForceLoginAuthentication())
            {
                DataTable dt = GetLast24HrsData();
                //List<TicketData> ticketdata = LoadData(dt);
                Last24HrsData = LoadData(dt);

                //List<TicketData> ticketdata = LoadData(Server.MapPath("~/Source/TodaysDefects.csv"));
                var resultdata = Last24HrsData.GroupBy(t => t.EscalationStatus).Select(k => new { name = k.Key, y = k.Count() });
                return this.Json(resultdata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }

        public ActionResult GetMyDefectsList()
        {
            ForceLoginController forceLogin = new ForceLoginController();
            if (forceLogin.SaleForceLoginAuthentication())
            {
                DataTable dt = GetMyDefectsData();
                List<TicketData> ticketdata = LoadData(dt);

                //List<TicketData> ticketdata = LoadData(Server.MapPath("~/Source/Tickets.csv"));
                var resultdata = ticketdata.GroupBy(t => t.EscalationStatus).Select(k => new { name = k.Key, y = k.Count() });
                return this.Json(resultdata, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return null;

            }
        }

        public DataTable GetMyDefectsData()
        {
            string soqlQuery = "Select Date_Closed__c, Date_Escalated__c, Date_Resolved_by_Dev__c, Description__c, Dev_Escalation_Date__c,   " +
            " Escalation_ID__c,    Name, OwnerId, Pending_Problem_Mgmt_Action__c, Pending_Support_Action__c, " +
            " PMC_ID__c, PMC_Name__c, Priority__c, Problem_Mgmt_Action_Pending_Date__c, Related_PME__c, " +
            "Site_ID__c, Site_Name__c, Escalation_Status__c, Summary__c, Support_Product__c " +
            "from Problem_Management_Escalation__c  where Support_Product__c in ('OneSite Affordable', 'OneSite Tax Credits', 'OneSite Leasing and Rents Rural Housing') " +
            "and OwnerId ='00537000002yoU5AAI' order by Name desc";

            try
            {

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
                }
            }
            catch (Exception ex)
            {
                return null;
                //throw ex;
            }
        }


        private DataTable GetLast24HrsData()
        {
            //Cursor = Cursors.WaitCursor;
            try
            {
                string soqlQuery = "Select Date_Closed__c, Date_Escalated__c, Date_Resolved_by_Dev__c, Description__c, Dev_Escalation_Date__c,   " +
            " Escalation_ID__c,    Name, OwnerId, Pending_Problem_Mgmt_Action__c, Pending_Support_Action__c, " +
            " PMC_ID__c, PMC_Name__c, Priority__c, Problem_Mgmt_Action_Pending_Date__c, Related_PME__c, " +
            "Site_ID__c, Site_Name__c, Escalation_Status__c, Summary__c, Support_Product__c " +
            "from Problem_Management_Escalation__c  where Support_Product__c in ('OneSite Affordable', 'OneSite Tax Credits', 'OneSite Leasing and Rents Rural Housing') " +
            "and DAY_ONLY(Date_Escalated__c)=LAST_N_DAYS:1 order by Name desc";
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


        public ActionResult GetLast24HrsStatusData(string status)
        {

            DataTable dt = GetLast24HrsData();            
            Last24HrsData = LoadData(dt);
            if (Last24HrsData.Count > 0)
            {
                ViewBag.FilteredData = Last24HrsData.Where(t => t.EscalationStatus == status).ToList();
                return PartialView("~/Views/Home/_FilteredView.cshtml");
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetMyDefectsStatusData(string status)
        {

            DataTable dt = GetMyDefectsData();
            MyDefectsData = LoadData(dt);
            if (MyDefectsData.Count > 0)
            {
                ViewBag.FilteredData = MyDefectsData.Where(t => t.EscalationStatus == status).ToList();
                return PartialView("~/Views/Home/_FilteredView.cshtml");
            }
            else
            {
                return null;
            }
        }
        public List<TicketData> LoadData(string Filepath)
        {
            List<TicketData> ticketdata = new List<TicketData>();
            if (System.IO.File.Exists(Filepath))
            {
                var data = System.IO.File.ReadAllLines(Filepath);
                for (int i = 1; i < data.Length; i++)
                {
                    var ticket = data[i];
                    TicketData currentData = new TicketData();
                    currentData.EscalationId = ticket.Split(',')[0];
                    currentData.Summary = ticket.Split(',')[1];
                    currentData.Priority = ticket.Split(',')[2];
                    currentData.EscalationStatus = ticket.Split(',')[3];
                    ticketdata.Add(currentData);
                }
            }
            return ticketdata;
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

        public ActionResult Search()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult SearchDefects(string search)
        {
            //List<TicketData> ticketdata = LoadData(Server.MapPath("~/Source/Tickets.csv"));
            ForceLoginController forceLogin = new ForceLoginController();
            if (forceLogin.SaleForceLoginAuthentication())
            {
                DataTable dt = GetSearchedQueryData(search);
                List<TicketData> searchedQuery = LoadData(dt);
                //Last24HrsData = LoadData(dt);

                //List<TicketData> ticketdata = LoadData(Server.MapPath("~/Source/TodaysDefects.csv"));
                ViewBag.FilteredData = searchedQuery;
                return PartialView("~/Views/Home/_FilteredView.cshtml");
            }
            else
            {
                return null;
            }


        }

        private DataTable GetSearchedQueryData(string search)
        {
            //Cursor = Cursors.WaitCursor;
            try
            {
                string soqlQuery = "Select Date_Closed__c, Date_Escalated__c, Date_Resolved_by_Dev__c, Description__c, Dev_Escalation_Date__c,   " +
                " Escalation_ID__c,    Name, OwnerId, Pending_Problem_Mgmt_Action__c, Pending_Support_Action__c, " +
                " PMC_ID__c, PMC_Name__c, Priority__c, Problem_Mgmt_Action_Pending_Date__c, Related_PME__c, " +
                "Site_ID__c, Site_Name__c, Escalation_Status__c, Summary__c, Support_Product__c " +
                "from Problem_Management_Escalation__c  where Support_Product__c in ('OneSite Affordable', 'OneSite Tax Credits', 'OneSite Leasing and Rents Rural Housing') " +
                "order by Name desc";

                sforce.QueryResult qr = SalesForcePartner.binding.query(
                    soqlQuery);
                if (qr.size > 0)
                {
                    DataTable dt = CreateDataTable(ParseFieldList(soqlQuery), qr);
                    string[] words = search.Split(' ');
                    DataTable dtResult = new DataTable();
                    DataTable dtDescriptionQuery = new DataTable();
                    DataTable dtSummaryQuery = dt.Select("Summary__c LIKE '%" + search + "%'").CopyToDataTable();
                    if (dtSummaryQuery.Rows.Count >= 1)
                    {
                        dtResult.Merge(dtSummaryQuery);
                    }

                    for (int i = 0; i <= words.Length - 1; i++)
                    {
                        if (words[i].Length > 3)
                        {
                             dtDescriptionQuery = dt.Select("Description__c LIKE '%" + words[i] + "%'").CopyToDataTable();
                            if (words.Length > 1)
                            {
                                if (dtDescriptionQuery.Rows.Count > 1 && words[i + 1].Length > 3)
                                {
                                    dtDescriptionQuery = dtDescriptionQuery.Select("Description__c LIKE '%" + words[i + 1] + "%'").CopyToDataTable();
                                    if (words.Length > 2)
                                    {
                                        if (dtDescriptionQuery.Rows.Count > 1 && i + 2 <= words.Length && words[i + 2].Length > 3)
                                        {
                                            dtDescriptionQuery = dtDescriptionQuery.Select("Description__c LIKE '%" + words[i + 2] + "%'").CopyToDataTable();


                                            if (words.Length > 3)
                                            {
                                                if (dtDescriptionQuery.Rows.Count > 1 && i + 3 <= words.Length && words[i + 3].Length > 3)
                                                {
                                                    dtDescriptionQuery = dtDescriptionQuery.Select("Description__c LIKE '%" + words[i + 3] + "%'").CopyToDataTable();
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        break;
                    }
                    dtResult.Merge(dtDescriptionQuery);
                    DataTable dtResultsData = new DataTable();
                    if (dtResult.Rows.Count>1)
                    {   
                        dtResult.DefaultView.Sort = "Name desc";
                        dtResultsData = dtResult.DefaultView.ToTable();                        
                    }
                    return dtResultsData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ActionResult GetPossibleDataFixes(string search)
        {
            List<TicketData> ticketdata = LoadData(Server.MapPath("~/Source/Tickets.csv"));
            ViewBag.FilteredData = ticketdata.Where(t => t.Summary.ToLower().Contains(search.ToLower())).ToList();
            return PartialView("~/Views/Home/_FilteredView.cshtml");
        }


        // New Implementation

        public ActionResult GetTodaysEscalations()
        {
            List<TicketData> ticketdata = LoadData(Server.MapPath("~/Source/Tickets.csv"));
            var resultdata = ticketdata.GroupBy(t => t.EscalationStatus).Select(k => new { name = k.Key, y = k.Count() });
            return this.Json(resultdata, JsonRequestBehavior.AllowGet);
        }


    }
}