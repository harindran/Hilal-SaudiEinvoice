﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Common;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Diagnostics;
using EInvoice.Models;
using static EInvoice.Common.clsGlobalMethods;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using SAPbobsCOM;
using iText.Kernel.Pdf;
using iText.Pdfa;
using iText.Layout;
using iText.Kernel.Utils;
using iText.Kernel.Pdf.Filespec;
using System.Threading;



namespace EInvoice.Business_Objects
{
    class ClsARInvoice
    {

        private string strSQL;
        private SAPbobsCOM.Recordset objRs;
        private bool blnRefresh;
        private bool blnprint;
        SAPbouiCOM.Button button;
        private SAPbouiCOM.Form objTempForm;


        #region ITEM EVENT
        public void Item_Event(string oFormUID, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            try
            {
                //if (pVal.InnerEvent) return;
                SAPbouiCOM.Form oForm = clsModule.objaddon.objapplication.Forms.Item(oFormUID);               
                ClsARInvoice.EinvoiceMethod einvoiceMethod = ClsARInvoice.EinvoiceMethod.Default;
                string DocEntry = "";
                string TransType = "";
                string Type = "";
                SAPbouiCOM.Button button = null;
               
                if (pVal.BeforeAction)
                {
                    switch (pVal.EventType)
                    {                       
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:                         
                            Create_Customize_Fields(oForm);
                            break;                      

                    }
                }
                else
                {
                                        
                    switch (pVal.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:                            
                            if (oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                                clsModule.objaddon.Cleartext(oForm);                            
                            break;                      
                        case SAPbouiCOM.BoEventTypes.et_GOT_FOCUS:
                            if(clsModule.objaddon.objglobalmethods.isupdate)
                            {                                
                                clsModule.objaddon.objglobalmethods.isupdate = false;
                                buttonenable(oForm);
                                

                            }
                            break;

                        case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                            {
                                if (pVal.FormTypeEx != "425") return;
                                if (objTempForm!=null)
                                {
                                    clsModule.objaddon.Cleartext(objTempForm);
                                    objTempForm = null;
                                }
                                
                                break;
                            }

                        case SAPbouiCOM.BoEventTypes.et_FORM_DRAW:                            
                            if (pVal.FormTypeEx == "179")
                            {
                                objTempForm = clsModule.objaddon.objapplication.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount);
                            }                           
                            break;

                        case SAPbouiCOM.BoEventTypes.et_CLICK:
                            if (pVal.ItemUID == "einv")
                            {

                                oForm.PaneLevel = 26;
                            }
                            switch (pVal.FormType)
                            {
                                case 133:                                                                       
                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("OINV").GetValue("DocEntry", 0);
                                        TransType = "INV";
                                        button = (SAPbouiCOM.Button)oForm.Items.Item("btneinv").Specific;
                                        if (button.Item.Enabled)
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                    }
                                    break;
                                case 179:                                  
                                   
                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("ORIN").GetValue("DocEntry", 0);
                                        TransType = "CRN";
                                        button = (SAPbouiCOM.Button)oForm.Items.Item("btneinv").Specific;
                                        if (button.Item.Enabled)
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                    }
                                    break;
                                case 65300:

                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("ODPI").GetValue("DocEntry", 0);
                                        TransType = "DPI";
                                        button = (SAPbouiCOM.Button)oForm.Items.Item("btneinv").Specific;
                                        if (button.Item.Enabled)
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                    }
                                    break;

                            }
                            bool docrefresh = false;
                            if (DocEntry != "" && TransType != "" && Type != "")
                            {
                                DataTable dt = new DataTable();
                                Generate_Cancel_IRN(einvoiceMethod, DocEntry, TransType, Type, ref dt, false);
                                button.Caption = "Generate E-invoice";
                                
                                if (dt.Rows.Count > 0)
                                {
                                    if (blnRefresh)
                                    {
                                        docrefresh = true;
                                    }
                                }
                                if (blnprint)
                                {
                                    docrefresh = true;
                                }
                                if (docrefresh)
                                {
                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                                    clsModule.objaddon.objapplication.Menus.Item("1304").Activate();
                                    clsModule.objaddon.objapplication.StatusBar.SetText("Operation completed successfully[Message 200 - 48]", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                }
                            }
                            
                            

                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {

            }
        }
        #endregion

        public void EnabledMenu( SAPbouiCOM.Form oForm, bool Penable = false, string UDFormID = "")
        {
            try
            {


                //   Penable = true;
                oForm.Freeze(true);
                switch (oForm.TypeEx)
                {
                    case "133":
                    case "179":
                    case "65300":
                        oForm.Items.Item("txtPIH").Enabled = Penable;
                        oForm.Items.Item("txtUUID").Enabled = Penable;
                        oForm.Items.Item("txtInvHash").Enabled = Penable;
                        oForm.Items.Item("txtICV").Enabled = Penable;
                        oForm.Items.Item("txtEinvSt").Enabled = Penable;
                        oForm.Items.Item("txtWarn").Enabled = Penable;
                        oForm.Items.Item("txtError").Enabled = Penable;
                        oForm.Items.Item("txtIssueDt").Enabled = Penable;

                        SAPbouiCOM.Form oUDFForm;

                        if (!string.IsNullOrEmpty(oForm.UDFFormUID))
                        {
                            oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);
                            oUDFForm.Items.Item("U_PIHNo").Enabled = Penable;
                            oUDFForm.Items.Item("U_UUIDNo").Enabled = Penable;
                            oUDFForm.Items.Item("U_InvoiceHashNo").Enabled = Penable;
                            oUDFForm.Items.Item("U_ICVNo").Enabled = Penable;
                            oUDFForm.Items.Item("U_EinvStatus").Enabled = Penable;
                            oUDFForm.Items.Item("U_Warn").Enabled = Penable;
                            oUDFForm.Items.Item("U_Error").Enabled = Penable;
                            oUDFForm.Items.Item("U_Issuedt").Enabled = Penable;
                        }

                        break;
                }
            }
            catch (Exception)
            {
                return;

            }
            finally
            {
                oForm.Freeze(false);
            }

        }
        #region FORM DATA EVENT
        public void FormData_Event(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            try
            {

                if (BusinessObjectInfo.BeforeAction)
                {
                    switch (BusinessObjectInfo.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                            break;
                    }
                }
                else
                {
                    SAPbouiCOM.Form activefrm = clsModule.objaddon.objapplication.Forms.Item(BusinessObjectInfo.FormUID);
                    switch (BusinessObjectInfo.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD:
                            if (BusinessObjectInfo.ActionSuccess)
                            {  
                                
                                buttonenable(activefrm);
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE:
                            if (BusinessObjectInfo.ActionSuccess)
                            {
                                switch (activefrm.Type.ToString())
                                {
                                    case "133":                                                                               
                                    case "179":
                                    case "65300":
                                        clsModule.objaddon.objglobalmethods.isupdate = true;
                                        break;
                                    default:
                                        return;
                                }                              
                            }
                            break;
                   

                    }
                }
            }
            catch (Exception Ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(Ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                BubbleEvent = false;
                return;
            }
            finally
            {
                // oForm.Freeze(false);
            }
        }
        #endregion


        public string GetInvoiceData(string DocEntry, string TransType)
        {
            DataTable dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(@"SELECT ""U_SerConfig"" FROM ""@EICON""");

            Querycls qcls = new Querycls();
            if (dt.Rows.Count > 0)
            {

                if (!String.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["U_SERCONFIG"])))
                {
                    qcls.docseries = Convert.ToString(dt.Rows[0]["U_SERCONFIG"]);
                }
            }

            switch (TransType)
            {
                case "INV":
                    strSQL = qcls.InvoiceQuery(DocEntry);
                    break;
                case "CRN":
                    strSQL = qcls.CreditNoteQuery(DocEntry);
                    break;
                case "DPI":
                    strSQL = qcls.ARDownInvoiceQuery(DocEntry);
                    break;


            }
            if (!clsModule.HANA)
            {
                strSQL = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(strSQL);
            }
            return strSQL;
        }

        public string GetDownpayment(string DocEntry, string Transtype)
        {
            string maintb = "";
            string subtb1 = "";
            switch (Transtype)
            {
                case "INV":
                    maintb = "INV9";
                    subtb1 = "INV4";
                    break;
                case "CRN":
                    maintb = "RIN9";
                    subtb1 = "RIN4";
                    break;
                case "DPI":
                    maintb = "DPI9";
                    subtb1 = "DPI4";
                    break;

            }

            strSQL = " SELECT  'Advance' AS \"ItemsellerID\", '1' AS \"Quantity\",'EA' AS \"UomCode\", '0' AS \"Gross\",'0' AS \"taxamt\", ";
            strSQL += " '0' AS \"Linenet\",'Advance' AS \"Dscription\",'Advance' AS \"ItemsellerID\",'Advance' AS \"ItemBuyerID\", 'S' AS \"TaxCat\", ";
            strSQL += " '15' AS \"Taxrate\",'' AS \"Reason\",'' AS \"Reasoncode\",0 AS \"PriceAmt\",0 AS \"DiscAmt\",0 AS \"BaseAmt\", ";
            strSQL += " t2.\"DrawnSum\"  AS \"DPTaxableAmt\", t2.\"Vat\" AS \"DPTaxAmt\",'386' AS \"DPDoctype\",t3.\"DocNum\" AS \"DPID\", ";
            strSQL += " t3.\"DocDate\"  AS \"DPIssudt\",t3.\"DocTime\"  AS \"DPIssutime\",t3.\"U_UUIDNo\" AS \"DPUUID\" ";
            strSQL += " FROM "+ maintb +" t2 ";
            strSQL += " INNER JOIN ODPI t3 ON t2.\"BaseAbs\" = t3.\"DocEntry\" " ;
            strSQL += " WHERE t2.\"DocEntry\" = '" + DocEntry +"'";


            if (!clsModule.HANA)
            {
                strSQL = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(strSQL);
            }

            return strSQL;
        }


        public enum EinvoiceMethod
        {
            Default = 0,
            CreateIRN = 1,
            CancelIRN = 2,
            GetIrnByDocnum = 3,
            GETIRNDetails = 4


        }

        private void Create_Customize_Fields(SAPbouiCOM.Form oForm)
        {                     
            try
            {
                switch (oForm.TypeEx)
                {
                    case "133":
                    case "179":
                    case "65300":
                        break;
                    default:
                        return;
                }

                SAPbouiCOM.Item oItem;
                clsModule.objaddon.objglobalmethods.WriteErrorLog("Customize Field Start");

                try
                {
                    if (oForm.Items.Item("btneinv").UniqueID == "btneinv")
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {

                }
                switch (oForm.TypeEx)
                {
                    case "133":
                    case "179":
                    case "65300":

                        SAPbouiCOM.Folder objfolder;
                        oItem = oForm.Items.Add("einv", SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                        objfolder = (SAPbouiCOM.Folder)oItem.Specific;
                        oItem.AffectsFormMode = false;
                        objfolder.Caption = "E-Invoice Details";
                        objfolder.GroupWith("1320002137");
                        objfolder.Pane = 26;
                        oItem.Width = 125;
                        oItem.Visible = true;
                        // oForm.PaneLevel = 1;
                        oItem.Left = oForm.Items.Item("1320002137").Left + oForm.Items.Item("1320002137").Width;
                        oItem.Enabled = true;
                        break;
                  
                }
                switch (oForm.TypeEx)
                {
                    case "133":
                    case "179":
                    case "65300":


                        oItem = oForm.Items.Add("btneinv", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                        button = (SAPbouiCOM.Button)oItem.Specific;
                        button.Caption = "Generate E-invoice";
                        oItem.Left = oForm.Items.Item("2").Left + oForm.Items.Item("2").Width + 5;
                        oItem.Top = oForm.Items.Item("2").Top;
                        oItem.Height = oForm.Items.Item("2").Height;
                        oItem.LinkTo = "2";
                        Size Fieldsize = System.Windows.Forms.TextRenderer.MeasureText("Generate E-Invoice", new Font("Arial", 12.0f));
                        oItem.Width = Fieldsize.Width;
                        oForm.Items.Item("btneinv").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_All), SAPbouiCOM.BoModeVisualBehavior.mvb_False);                      
                                         
                        break;
                    default:
                        return;
                }





                SAPbouiCOM.Item newTextBox;
                SAPbouiCOM.EditText otxt;
                SAPbouiCOM.StaticText olbl;
                string tablename = "";
                oForm.Freeze(true);

                switch (oForm.TypeEx)
                {
                    case "133":
                        tablename = "OINV";
                        break;
                    case "179":
                        tablename = "ORIN";
                        break;
                    case "65300":
                        tablename = "ODPI";
                        break;
                    default:
                        return;
                }


                int top = oForm.Items.Item("112").Top + 25;
                int space = 15;
                int labelwidth = 200;
                int textboxwidth = 300;
                int textboxheight = 15;


                CreateLabel(oForm, "lblPIH", "PIH No", 26, 26, oForm.Items.Item("112").Left + 20, top, labelwidth);
                CreateTextbox(oForm, "txtPIH", tablename, "U_PIHNo", 26, 26, oForm.Items.Item("lblPIH").Left + 80, top, textboxwidth, textboxheight);
                top = top + space;

                CreateLabel(oForm, "lblUUID", "UUID No", 26, 26, oForm.Items.Item("112").Left + 20, top, labelwidth);
                CreateTextbox(oForm, "txtUUID", tablename, "U_UUIDNo", 26, 26, oForm.Items.Item("lblUUID").Left + 80, top, textboxwidth, textboxheight);
                top = top + space;

                CreateLabel(oForm, "lblInvHash", "InvoiceHash No", 26, 26, oForm.Items.Item("112").Left + 20, top, labelwidth);
                CreateTextbox(oForm, "txtInvHash", tablename, "U_InvoiceHashNo", 26, 26, oForm.Items.Item("lblInvHash").Left + 80, top, textboxwidth, textboxheight);
                top = top + space;

                CreateLabel(oForm, "lblICV", "ICV No", 26, 26, oForm.Items.Item("112").Left + 20, top, labelwidth);
                CreateTextbox(oForm, "txtICV", tablename, "U_ICVNo", 26, 26, oForm.Items.Item("lblICV").Left + 80, top, textboxwidth, textboxheight);
                top = top + space;

                CreateLabel(oForm, "lblEinvSt", "E-Inv Status", 26, 26, oForm.Items.Item("112").Left + 20, top, labelwidth);
                CreateTextbox(oForm, "txtEinvSt", tablename, "U_EinvStatus", 26, 26, oForm.Items.Item("lblEinvSt").Left + 80, top, textboxwidth, textboxheight);
                top = top + space;

                CreateLabel(oForm, "lblIssueDt", "Issue Date", 26, 26, oForm.Items.Item("112").Left + 20, top, labelwidth);
                CreateTextbox(oForm, "txtIssueDt", tablename, "U_Issuedt", 26, 26, oForm.Items.Item("lblIssueDt").Left + 80, top, textboxwidth, textboxheight);
                top = top + space;
                CreateLabel(oForm, "lblWarn", "Warning", 26, 26, oForm.Items.Item("112").Left + 20, top, labelwidth);
                CreateTextbox(oForm, "txtWarn", tablename, "U_Warn", 26, 26, oForm.Items.Item("lblWarn").Left + 80, top, textboxwidth, textboxheight);
                top = top + space;
                CreateLabel(oForm, "lblError", "Error", 26, 26, oForm.Items.Item("112").Left + 20, top, labelwidth);
                CreateTextbox(oForm, "txtError", tablename, "U_Error", 26, 26, oForm.Items.Item("lblError").Left + 80, top, textboxwidth, textboxheight);

                oForm.Freeze(false);

                clsModule.objaddon.objglobalmethods.WriteErrorLog("Customize Field Completed");
            }
            catch (Exception ex)
            {
            }
        }






        private void CreateLabel(SAPbouiCOM.Form oForm, string name, string caption, int fromPane, int toPane, int left, int top, int width)
        {
            SAPbouiCOM.Item newTextBox;
            SAPbouiCOM.StaticText olbl;

            newTextBox = oForm.Items.Add(name, SAPbouiCOM.BoFormItemTypes.it_STATIC);
            newTextBox.FromPane = fromPane;
            newTextBox.ToPane = toPane;
            newTextBox.Left = left;
            newTextBox.Top = top;
            newTextBox.Width = width;
            olbl = (SAPbouiCOM.StaticText)oForm.Items.Item(name).Specific;
            ((SAPbouiCOM.StaticText)(olbl.Item.Specific)).Caption = caption;
        }

        private void CreateTextbox(SAPbouiCOM.Form oForm, string name, string tablename, string Feildname, int fromPane, int toPane, int left, int top, int width, int height)
        {
            SAPbouiCOM.Item newTextBox;
            SAPbouiCOM.EditText olbl;

            newTextBox = oForm.Items.Add(name, SAPbouiCOM.BoFormItemTypes.it_EDIT);
            newTextBox.FromPane = fromPane;
            newTextBox.ToPane = toPane;
            newTextBox.Left = left;
            newTextBox.Top = top;
            newTextBox.Width = width;
            newTextBox.Height = height;
            oForm.Items.Item(name).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_All), SAPbouiCOM.BoModeVisualBehavior.mvb_False);

            olbl = (SAPbouiCOM.EditText)oForm.Items.Item(name).Specific;
            try
            {
                olbl.DataBind.SetBound(true, tablename, Feildname);
            }
            catch (Exception ex)
            {


            }

        }
    
        public bool GetXML(string DocEntry,string TransType,string Accesstkn,string Filename,ref DataTable dt)
        {
            SAPbobsCOM.Recordset invrecordset;
            invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            strSQL = @"Select T0.""U_Live"",T0.""U_UATUrl"",T0.""U_LiveUrl"",T0.""U_AuthKey"",T0.""U_SerConfig"",T1.""U_URLType"",T1.""U_URL"", ";
            strSQL += @"Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LiveUrl"",T1.""U_URL"") End as URL,";
            strSQL += @"T0.""U_DBUser"" ,T0.""U_DBPass"",T0.""U_Cryspath"" ";
            strSQL += @" from ""@EICON"" T0 join ""@EICON1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";
            strSQL += @" and T1.""U_URLType""='XML File' ";

            objRs.DoQuery(strSQL);
            if (objRs.RecordCount == 0)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"XML File\". Please update in E-invoice Configuration... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }

            strSQL = GetInvoiceData(DocEntry, TransType);
            invrecordset.DoQuery(strSQL);
            if (invrecordset.RecordCount > 0)
            {

                clsModule.objaddon.objapplication.StatusBar.SetText("Creating XML File. Please Wait...." + DocEntry, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                Dictionary<string, string> Queryparameter = new Dictionary<string, string>();
                DateTime startDate;

                if (DateTime.TryParseExact(invrecordset.Fields.Item("DocDate").Value.ToString(), CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns(), CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                {
                    Queryparameter.Add("financialyear", startDate.Year.ToString());
                }


                Queryparameter.Add("ref_nm", invrecordset.Fields.Item("DocNum").Value.ToString());
                Queryparameter.Add("invoicetypecode", invrecordset.Fields.Item("TaxType").Value.ToString());

                Dictionary<string, string> head = new Dictionary<string, string>();
                head.Add("authorization", "Bearer " + Accesstkn);
                
                string Xmlpath = Filename;


             dt= Get_API_Response("", objRs.Fields.Item("URL").Value.ToString(),httpMethod:"GET", headers: head, Queryparameter: Queryparameter,filepath: Xmlpath);


            }
            return true;
        }
        private bool GetPDF(string DocEntry, string TransType,string FileName)
        {
            SAPbobsCOM.Recordset invrecordset;
            invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            strSQL = GetInvoiceData(DocEntry, TransType);
            invrecordset.DoQuery(strSQL);
            if (invrecordset.RecordCount > 0)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText("Getting Data from  Crysatl Report. Please Wait...." + DocEntry, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

            
                string crytalpath = "";
                string endpath = "";                
                string files = "";                
                string fileType = "";                
                switch (TransType)
                {
                    case "INV":
                        fileType = (invrecordset.Fields.Item("DocType").Value.ToString() == "I") ? "Item" : "Service";
                        files = $"SELECT \"U_FileNm\" FROM \"@EICON2\" WHERE \"U_DocType\" = 'A/R Invoice' AND \"U_TransType\" = '{fileType}'";
                        endpath = clsModule.objaddon.objglobalmethods.getSingleValue(files);
                        break;
                    case "CRN":
                        fileType = (invrecordset.Fields.Item("DocType").Value.ToString() == "I") ? "Item" : "Service";
                        files = $"SELECT \"U_FileNm\" FROM \"@EICON2\" WHERE \"U_DocType\" = 'A/R Credit Memo' AND \"U_TransType\" = '{fileType}'";
                        endpath = clsModule.objaddon.objglobalmethods.getSingleValue(files);

                        break;
                    case "DPI":
                        fileType = (invrecordset.Fields.Item("DocType").Value.ToString() == "I") ? "Item" : "Service";
                        files = $"SELECT \"U_FileNm\" FROM \"@EICON2\" WHERE \"U_DocType\" = 'A/R Down Payment' AND \"U_TransType\" = '{fileType}'";
                        endpath = clsModule.objaddon.objglobalmethods.getSingleValue(files);

                        break;
                }

                strSQL = @"Select T0.""U_Live"",T0.""U_UATUrl"",T0.""U_LiveUrl"",T0.""U_AuthKey"",T0.""U_SerConfig"",T1.""U_URLType"",T1.""U_URL"", ";
                strSQL += @"Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LiveUrl"",T1.""U_URL"") End as URL,";
                strSQL += @"T0.""U_DBUser"" ,T0.""U_DBPass"",T0.""U_Cryspath"" ";
                strSQL += @" from ""@EICON"" T0 join ""@EICON1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";                

                objRs.DoQuery(strSQL);
                if (objRs.RecordCount == 0)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Kindly Check E-invoice Configuration... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return false;
                }
                crytalpath = objRs.Fields.Item("U_Cryspath").Value.ToString() + endpath + ".rpt";

                clsModule.objaddon.objglobalmethods.Create_RPT_To_PDF(crytalpath, clsModule.objaddon.objcompany.Server,
                clsModule.objaddon.objcompany.CompanyDB, objRs.Fields.Item("U_DBUser").Value.ToString(), objRs.Fields.Item("U_DBPass").Value.ToString(), DocEntry, FileName);

            }
            return true;
        }

        private bool GetPDFA3(string DocEntry,string TransType,string Filename)
        {
            SAPbobsCOM.Recordset invrecordset;
            invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            strSQL = GetInvoiceData(DocEntry, TransType);
            invrecordset.DoQuery(strSQL);
            if (invrecordset.RecordCount > 0)
            {
                strSQL = @"Select T0.""U_Live"",T0.""U_UATUrl"",T0.""U_LiveUrl"",T0.""U_AuthKey"",T0.""U_SerConfig"",T1.""U_URLType"",T1.""U_URL"", ";
                strSQL += @"Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LiveUrl"",T1.""U_URL"") End as URL,";
                strSQL += @"T0.""U_DBUser"" ,T0.""U_DBPass"",T0.""U_Cryspath"" ";
                strSQL += @" from ""@EICON"" T0 join ""@EICON1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";

                objRs.DoQuery(strSQL);
                if (objRs.RecordCount == 0)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Kindly Check E-invoice Configuration... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return false;
                }
                clsModule.objaddon.objapplication.StatusBar.SetText("Creating PDF A3. Please Wait...." + DocEntry, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                string BaseSysPath = Getbasepath();
                string SysPath = BaseSysPath + invrecordset.Fields.Item("DocNum").Value.ToString() + "_";
                SysPath += clsModule.objaddon.objglobalmethods.DateFormat(clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("DocDate").Value.ToString()), "dd/MM/yyyy", "yyyy-MM-dd");

                string existingPdfPath = SysPath + "_PDF.pdf";
                string xmlFilePath = SysPath + "_XML.XMl";
                string outputPdfPath = Filename;
                Stream fileStream1 = new FileStream("sRGB_CS_profile.icm", FileMode.Open, FileAccess.Read);
                PdfReader pdfReader = new PdfReader(existingPdfPath);
                {
                    using (PdfWriter pdfWriter = new PdfWriter(outputPdfPath))
                    {
                        Stream srgbProfileStream = fileStream1;
                        PdfOutputIntent outputIntent = new PdfOutputIntent("sRGB IEC61966-2.1", "", "", "sRGB IEC61966-2.1", srgbProfileStream);
                        using (PdfADocument pdfDoc = new PdfADocument(pdfWriter, PdfAConformanceLevel.PDF_A_3A, outputIntent, new DocumentProperties()))
                        {
                            Document document = new Document(pdfDoc);
                            PdfMerger pdfMerger = new PdfMerger(pdfDoc);
                            {
                                PdfDocument sourcePdf = new PdfDocument(new PdfReader(existingPdfPath));
                                pdfMerger.Merge(sourcePdf, 1, sourcePdf.GetNumberOfPages());
                            }

                            pdfDoc.SetTagged();

                            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
                            pdfDoc.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
                            pdfDoc.GetCatalog().SetPageMode(PdfName.UseOutlines);

                            PdfDictionary catalog = pdfDoc.GetCatalog().GetPdfObject();
                            if (!catalog.ContainsKey(PdfName.MarkInfo))
                            {
                                PdfDictionary markInfo = new PdfDictionary();
                                markInfo.Put(PdfName.Marked, PdfBoolean.TRUE);
                                catalog.Put(PdfName.MarkInfo, markInfo);
                            }

                            string embeddedname = invrecordset.Fields.Item("DocNum").Value.ToString();
                            embeddedname += clsModule.objaddon.objglobalmethods.DateFormat(clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("DocDate").Value.ToString()), "dd/MM/yyyy", "yyyy-MM-dd");
                            embeddedname += "_XML.XMl";

                            PdfFileSpec fileSpec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc,
                                File.ReadAllBytes(xmlFilePath),
                                embeddedname, embeddedname,
                                new PdfName("text/xml"), new PdfDictionary(), PdfName.Data);
                            pdfDoc.AddAssociatedFile(embeddedname, fileSpec);
                        }
                    }
                }
            }
            return true;
        }

       
        public bool Generate_Cancel_IRN(EinvoiceMethod Create_Cancel, string DocEntry, string TransType, string Type, ref DataTable datatable,
            bool frommul)
        {
            string requestParams;            
          
            bool Einvlog =false;
            try
            {


                SAPbobsCOM.Recordset invrecordset, DPMrecset;
                objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                DPMrecset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                Dictionary<string, string> Queryparameter = new Dictionary<string, string>();

                Dictionary<string, string> head = new Dictionary<string, string>();
                bool blnstatus = false;
                if (Create_Cancel == EinvoiceMethod.CreateIRN)
                {
                    GenerateIRN GenerateIRNGetJson = new GenerateIRN();

                    strSQL = GetInvoiceData(DocEntry, TransType);
                    invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);


                    invrecordset.DoQuery(strSQL);
                    if (invrecordset.RecordCount > 0)
                    {

                        string Einvstus = "";
                        Einvstus = invrecordset.Fields.Item("Einvsts").Value.ToString();                        
                        switch (Einvstus)
                        {
                            case "CLEARED":
                            case "REPORTED":
                                Einvstus = "CLEARED";
                                blnstatus = true;
                                break;
                        }
                       

                        
                          
                            strSQL = @"Select T0.""U_Live"",T0.""U_UATUrl"",T0.""U_LiveUrl"",T0.""U_AuthKey"",T0.""U_SerConfig"",T1.""U_URLType"",T1.""U_URL"", ";
                            strSQL += @"Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LiveUrl"",T1.""U_URL"") End as URL,T0.""U_DevID"",T0.""U_Startdate"" ";
                            strSQL += @" from ""@EICON"" T0 join ""@EICON1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";
                            strSQL += @" and T1.""U_URLType""='Token Api' ";


                            DataTable dt1 = new DataTable();
                            dt1 = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);

                            if (dt1.Rows.Count == 0)
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Token API is Missing ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                return false;
                            }

                            var Data = new Dictionary<string, string>
                            {
                                { "grant_type", "client_credentials" },
                                { "client_id", dt1.Rows[0]["U_DevID"].ToString() },
                                { "client_secret", dt1.Rows[0]["U_AuthKey"].ToString() }
                            };

                            //var Data =  new Dictionary<string, string>
                            //{
                            //    { "grant_type", "client_credentials" },
                            //    { "client_id", "hilaluat-68718" },
                            //    { "client_secret", "H@$H$3cr3t$321" }
                            //};
                            string formData1 = string.Join("&", Data.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));


                            string Accesstkn = "";

                            datatable = Get_API_Response(formData1, dt1.Rows[0]["URL"].ToString(), contenttype: "application/x-www-form-urlencoded");
                            if (clsModule.objaddon.objglobalmethods.CheckIfColumnExists(datatable, "access_token"))
                            {
                                Accesstkn = datatable.Rows[0]["access_token"].ToString();

                            }
                            else
                            {
                                if (clsModule.objaddon.objglobalmethods.CheckIfColumnExists(datatable, "msg"))
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText(datatable.Rows[0]["msg"].ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                }
                                else
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("check ci", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                }
                                return false;
                            }
                        
                            strSQL = @"Select T0.""U_Live"",T0.""U_UATUrl"",T0.""U_LiveUrl"",T0.""U_AuthKey"",T0.""U_SerConfig"",T1.""U_URLType"",T1.""U_URL"",T0.""U_TranName"", ";
                            strSQL += @"Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LiveUrl"",T1.""U_URL"") End as URL,T0.""U_DevID"",T0.""U_Startdate"" ";
                            strSQL += @" from ""@EICON"" T0 join ""@EICON1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";
                            strSQL += @" and T1.""U_URLType""='E-invoice -Tax' ";

                            objRs.DoQuery(strSQL);
                            if (objRs.RecordCount == 0)
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Create Invoice\". Please update in E-invoice Configuration... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                return false;
                            }



                            DateTime stdt;
                            DateTime docdt;

                            DateTime.TryParseExact(objRs.Fields.Item("U_Startdate").Value.ToString(), CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns(), CultureInfo.InvariantCulture, DateTimeStyles.None, out stdt);
                            DateTime.TryParseExact(invrecordset.Fields.Item("DocDate").Value.ToString(), CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns(), CultureInfo.InvariantCulture, DateTimeStyles.None, out docdt);
                            if (!string.IsNullOrEmpty(objRs.Fields.Item("U_Startdate").Value.ToString()))
                            {
                                if (!(docdt >= stdt))
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("Cannot Generate E-invoice Before valid Date(" + stdt + ")", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    return false;
                                }
                            }


                            string vatid = invrecordset.Fields.Item("TaxIdNum").Value.ToString();
                            string syscur = invrecordset.Fields.Item("SysCurrncy").Value.ToString();



                            GenerateIRNGetJson.ReferenceNumber = invrecordset.Fields.Item("DocNum").Value.ToString();

                            DateTime startDate;

                            if (DateTime.TryParseExact(invrecordset.Fields.Item("DocDate").Value.ToString(), CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns(), CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                            {
                                GenerateIRNGetJson.FinancialYear = startDate.Year.ToString();
                            }

                            GenerateIRNGetJson.InvTypeCd = invrecordset.Fields.Item("TaxType").Value.ToString();
                            GenerateIRNGetJson.InvoiceNumber = invrecordset.Fields.Item("DocNum").Value.ToString();
                            GenerateIRNGetJson.InvoiceDate = clsModule.objaddon.objglobalmethods.DateFormat(clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("DocDate").Value.ToString()), "dd/MM/yyyy", "yyyy-MM-dd");
                            GenerateIRNGetJson.InvoiceTime = clsModule.objaddon.objglobalmethods.ConverttoTime(invrecordset.Fields.Item("DocTime").Value.ToString());

                            GenerateIRNGetJson.InvSubtype = invrecordset.Fields.Item("U_EType").Value.ToString().Substring(0, 2);
                            GenerateIRNGetJson.ThirdPartyInvoice = invrecordset.Fields.Item("U_EType").Value.ToString().Substring(2, 1);
                            GenerateIRNGetJson.NominalInvoice = invrecordset.Fields.Item("U_EType").Value.ToString().Substring(3, 1);
                            GenerateIRNGetJson.ExportInvoice = invrecordset.Fields.Item("U_EType").Value.ToString().Substring(4, 1);
                            GenerateIRNGetJson.SummaryInvoice = invrecordset.Fields.Item("U_EType").Value.ToString().Substring(5, 1);
                            GenerateIRNGetJson.SelfBilledinvoice = invrecordset.Fields.Item("U_EType").Value.ToString().Substring(6, 1);

                            GenerateIRNGetJson.ConversionRate = invrecordset.Fields.Item("DocRate").Value.ToString();
                            GenerateIRNGetJson.Note = "";
                            GenerateIRNGetJson.OrderRef = "";
                            GenerateIRNGetJson.BlngRef = invrecordset.Fields.Item("BaseDoc").Value.ToString();// invrecordset.Fields.Item("DocNum").Value.ToString(); 
                            GenerateIRNGetJson.BlngRefIssueDt = clsModule.objaddon.objglobalmethods.DateFormat(clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("DocDate").Value.ToString()), "dd/MM/yyyy", "yyyy-MM-dd"); ;
                            GenerateIRNGetJson.ContractDocRef = invrecordset.Fields.Item("NumAtCard").Value.ToString();
                            GenerateIRNGetJson.DocCurrencyCd = invrecordset.Fields.Item("DocCur").Value.ToString();
                            GenerateIRNGetJson.DeliveryActualDeliveryDate = clsModule.objaddon.objglobalmethods.DateFormat(clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("DocDate").Value.ToString()), "dd/MM/yyyy", "yyyy-MM-dd");
                            GenerateIRNGetJson.DeliveryLatestDeliveryDate = "";
                            GenerateIRNGetJson.PymtMeansPymtMeansCode = invrecordset.Fields.Item("Paymeanscode").Value.ToString();
                            GenerateIRNGetJson.PymtMeansInstructionNoteReason = invrecordset.Fields.Item("Comments").Value.ToString();
                            GenerateIRNGetJson.PymtTermsNote = "";
                            GenerateIRNGetJson.PymtTermsPayeeAccountID = "";


                            clsModule.objaddon.objglobalmethods.WriteErrorLog("Document Details Complete");

                            GenerateIRNGetJson.ActngSuplParty.PartyTaxScheme.CompanyID = invrecordset.Fields.Item("TaxIdNum").Value.ToString();
                            GenerateIRNGetJson.ActngSuplParty.PartyTaxScheme.CompanyIDAR = "";
                            GenerateIRNGetJson.ActngSuplParty.PartyLegalEntity.RegName = invrecordset.Fields.Item("CompnyName").Value.ToString();
                            GenerateIRNGetJson.ActngSuplParty.PartyLegalEntity.RegNameAR = "";

                            GenerateIRNGetJson.ActngSuplParty.Party.SchemeID = invrecordset.Fields.Item("CmpId").Value.ToString();
                           GenerateIRNGetJson.ActngSuplParty.Party.PartyID = invrecordset.Fields.Item("TaxIdNum2").Value.ToString();
                            GenerateIRNGetJson.ActngSuplParty.Party.SellerIDNumber = invrecordset.Fields.Item("TaxIdNum2").Value.ToString(); ;
                            GenerateIRNGetJson.ActngSuplParty.Party.SchemeIDAR = "";
                            GenerateIRNGetJson.ActngSuplParty.Party.PartyIDAR = "";
                            GenerateIRNGetJson.ActngSuplParty.Party.SellerIDNumberAR = "";

                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.SellerCode = "";
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.StrName = invrecordset.Fields.Item("StreetNo").Value.ToString();
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.AdlStrName = invrecordset.Fields.Item("Street").Value.ToString();
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.PlotIdentification = "";
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.BldgNumber = invrecordset.Fields.Item("Building").Value.ToString();
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.CityName = invrecordset.Fields.Item("City").Value.ToString();
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.PostalZone = invrecordset.Fields.Item("ZipCode").Value.ToString();
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.CntrySubentityCd = invrecordset.Fields.Item("CodeCountry").Value.ToString();
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.CitySubdivisionName = invrecordset.Fields.Item("County").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.Cntry = invrecordset.Fields.Item("CodeCountry").Value.ToString();

                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.StrNameAR = "";
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.AdlStrNameAR = "";
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.PlotIdentificationAR = "";
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.BldgNumberAR = "";
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.CityNameAR = "";
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.PostalZoneAR = "";
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.CntrySubentityCdAR = "";
                            GenerateIRNGetJson.ActngSuplParty.PostalAddress.CitySubdivisionNameAR = "";

                            clsModule.objaddon.objglobalmethods.WriteErrorLog("Seller Details Complete");

                            //Buyer Details

                            GenerateIRNGetJson.ActngCustomerParty.PartyTaxScheme.CompanyID = invrecordset.Fields.Item("LicTradNum").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PartyTaxScheme.CompanyIDAR = "";
                            GenerateIRNGetJson.ActngCustomerParty.PartyLegalEntity.RegName = invrecordset.Fields.Item("CardName").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PartyLegalEntity.RegNameAR = "";

                            GenerateIRNGetJson.ActngCustomerParty.Party.SchemeID = invrecordset.Fields.Item("U_IDType").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.Party.PartyID = "";
                            GenerateIRNGetJson.ActngCustomerParty.Party.BuyerIDNumber = invrecordset.Fields.Item("AddID").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.Party.SchemeIDAR = "";
                            GenerateIRNGetJson.ActngCustomerParty.Party.PartyIDAR = "";
                            GenerateIRNGetJson.ActngCustomerParty.Party.SellerIDNumberAR = "";

                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.BuyerCode = "";
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.StrName = invrecordset.Fields.Item("StreetNoB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.AdlStrName = invrecordset.Fields.Item("StreetB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.PlotIdentification = "";
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.BldgNumber = invrecordset.Fields.Item("BuildingB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.CityName = invrecordset.Fields.Item("CityB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.PostalZone = invrecordset.Fields.Item("ZipCodeB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.CntrySubentityCd = invrecordset.Fields.Item("CodeCountryB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.CitySubdivisionName = invrecordset.Fields.Item("CountyB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.Cntry = invrecordset.Fields.Item("CodeCountryB").Value.ToString();

                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.StrNameAR = invrecordset.Fields.Item("U_AraStreetB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.AdlStrNameAR = invrecordset.Fields.Item("U_AraPOS").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.PlotIdentificationAR = "";
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.BldgNumberAR = invrecordset.Fields.Item("U_AraBlockB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.CityNameAR = invrecordset.Fields.Item("U_AraCityB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.PostalZoneAR = invrecordset.Fields.Item("U_AraZipB").Value.ToString();
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.CntrySubentityCdAR = "";
                            GenerateIRNGetJson.ActngCustomerParty.PostalAddress.CitySubdivisionNameAR = "";

                            clsModule.objaddon.objglobalmethods.WriteErrorLog("Buyer Details Complete");



                            GenerateIRNGetJson.LegalMonetaryTotal.LineExtAmt = invrecordset.Fields.Item("Totgross").Value.ToString();
                            GenerateIRNGetJson.LegalMonetaryTotal.AlwTotalAmt = invrecordset.Fields.Item("Allownace").Value.ToString();
                            GenerateIRNGetJson.LegalMonetaryTotal.TaxExclAmt = invrecordset.Fields.Item("TaxExclusive").Value.ToString();
                            GenerateIRNGetJson.LegalMonetaryTotal.TaxInclAmt = invrecordset.Fields.Item("Totnet").Value.ToString();
                            GenerateIRNGetJson.LegalMonetaryTotal.PrepaidAmt = "0.00";
                            //invrecordset.Fields.Item("Roundtot").Value.ToString()
                            GenerateIRNGetJson.LegalMonetaryTotal.PayableAmt = invrecordset.Fields.Item("Totnet1").Value.ToString();


                            clsModule.objaddon.objglobalmethods.WriteErrorLog("Document Details Complete");


                            //Line Details
                            for (int i = 0; i < invrecordset.RecordCount; i++)
                            {
                                GenerateIRNGetJson.InvLine.Add(new InvLine
                                {
                                    ItemCode = invrecordset.Fields.Item("ItemsellerID").Value.ToString(),
                                    ID = invrecordset.Fields.Item("LineNum").Value.ToString(),
                                    Note = "",
                                    InvdQty = invrecordset.Fields.Item("Quantity").Value.ToString(),
                                    InvQtyUom = invrecordset.Fields.Item("UomCode").Value.ToString(),
                                    LineExtAmt = invrecordset.Fields.Item("Gross").Value.ToString(),
                                    TaxTotal = new TaxTotal()
                                    {
                                        TaxAmt = invrecordset.Fields.Item("taxamt").Value.ToString(),
                                        RoundingAmt = invrecordset.Fields.Item("Linenet").Value.ToString(),
                                    },
                                    Item = new Item()
                                    {
                                        Name = invrecordset.Fields.Item("Dscription").Value.ToString(),
                                        SellersItemID = invrecordset.Fields.Item("ItemsellerID").Value.ToString(),
                                        BuyerItemID = invrecordset.Fields.Item("ItemBuyerID").Value.ToString(),
                                        StdItemID = "",
                                        NameAR = "",
                                        SellersItemIDAR = "",
                                        BuyerItemIDAR = "",
                                        StdItemIDAR = "",
                                        ClasTaxCat = new ClasTaxCat()
                                        {
                                            ID = invrecordset.Fields.Item("TaxCat").Value.ToString(),
                                            Percent = invrecordset.Fields.Item("Taxrate").Value.ToString(),
                                            TaxExemptionReason = invrecordset.Fields.Item("Reason").Value.ToString(),
                                            TaxExemptionReasonCd = invrecordset.Fields.Item("Reasoncode").Value.ToString(),
                                            IDAR = "",
                                            PercentAR = "",
                                            TaxExemptionReasonAR = "",
                                            TaxExemptionReasonCdAR = "",
                                        },
                                        Price = new Price()
                                        {
                                            PriceAmt = invrecordset.Fields.Item("PriceAmt").Value.ToString(),
                                            BaseQty = "1",
                                            BaseQtyUoM = "",
                                            BaseQtyUoMAR = "",
                                        },
                                        AlwChg = new AlwChg()
                                        {
                                            AlwChgReason = "",
                                            Amt = invrecordset.Fields.Item("DiscAmt").Value.ToString(),
                                            BaseAmt = invrecordset.Fields.Item("BaseAmt").Value.ToString(),
                                            BaseAmtAR = "",

                                        },
                                    },
                                    PaidVATCategoryTaxableAmt = "",
                                    PaidVATCategoryTaxAmt = "",
                                    PrepaymentDocType = "",
                                    PrepaymentID = "",
                                    PrepaymentIssueDate = "",
                                    PrepaymentIssueTime = "",
                                    PrepaymentUUID = ""
                                });
                                if (invrecordset.Fields.Item("LineAllow").Value.ToString() != "0")
                                {
                                    GenerateIRNGetJson.AlwChg.Add(new AlwChg
                                    {
                                        AlwChgDiscountID = "1",
                                        Indicator = "False",
                                        AlwChgReason = "Discount",
                                        Amt = invrecordset.Fields.Item("LineAllow").Value.ToString(),
                                        MFN = Math.Round(((Convert.ToDecimal(invrecordset.Fields.Item("LineAllow").Value.ToString()) / Convert.ToDecimal(invrecordset.Fields.Item("BaseAmt").Value.ToString())) * 100),2).ToString(),
                                        BaseAmt = invrecordset.Fields.Item("BaseAmt").Value.ToString(),
                                        TaxCat = new TaxCat()
                                        {
                                            ID = invrecordset.Fields.Item("TaxCat").Value.ToString(),
                                            Percent = invrecordset.Fields.Item("Taxrate").Value.ToString()
                                        }
                                    });
                                }
                                invrecordset.MoveNext();
                            }


                            string sql = GetDownpayment(DocEntry, TransType);
                            DPMrecset.DoQuery(sql);

                            if (DPMrecset.RecordCount > 0)
                            {
                                for (int i = 0; i < DPMrecset.RecordCount; i++)
                                {
                                    GenerateIRNGetJson.InvLine.Add(new InvLine
                                    {
                                        ItemCode = DPMrecset.Fields.Item("ItemsellerID").Value.ToString(),
                                        ID = (invrecordset.RecordCount + (i + 1)).ToString(),
                                        Note = "",
                                        InvdQty = DPMrecset.Fields.Item("Quantity").Value.ToString(),
                                        InvQtyUom = DPMrecset.Fields.Item("UomCode").Value.ToString(),
                                        LineExtAmt = DPMrecset.Fields.Item("Gross").Value.ToString(),
                                        TaxTotal = new TaxTotal()
                                        {
                                            TaxAmt = DPMrecset.Fields.Item("taxamt").Value.ToString(),
                                            RoundingAmt = DPMrecset.Fields.Item("Linenet").Value.ToString(),
                                        },
                                        Item = new Item()
                                        {
                                            Name = DPMrecset.Fields.Item("Dscription").Value.ToString(),
                                            SellersItemID = DPMrecset.Fields.Item("ItemsellerID").Value.ToString(),
                                            BuyerItemID = DPMrecset.Fields.Item("ItemBuyerID").Value.ToString(),
                                            StdItemID = "",
                                            NameAR = "",
                                            SellersItemIDAR = "",
                                            BuyerItemIDAR = "",
                                            StdItemIDAR = "",
                                            ClasTaxCat = new ClasTaxCat()
                                            {
                                                ID = DPMrecset.Fields.Item("TaxCat").Value.ToString(),
                                                Percent = DPMrecset.Fields.Item("Taxrate").Value.ToString(),
                                                TaxExemptionReason = DPMrecset.Fields.Item("Reason").Value.ToString(),
                                                TaxExemptionReasonCd = DPMrecset.Fields.Item("Reasoncode").Value.ToString(),
                                                IDAR = "",
                                                PercentAR = "",
                                                TaxExemptionReasonAR = "",
                                                TaxExemptionReasonCdAR = "",
                                            },
                                            Price = new Price()
                                            {
                                                PriceAmt = DPMrecset.Fields.Item("PriceAmt").Value.ToString(),
                                                BaseQty = "1",
                                                BaseQtyUoM = "",
                                                BaseQtyUoMAR = "",
                                            },
                                            AlwChg = new AlwChg()
                                            {
                                                AlwChgReason = "",
                                                Amt = DPMrecset.Fields.Item("DiscAmt").Value.ToString(),
                                                BaseAmt = DPMrecset.Fields.Item("BaseAmt").Value.ToString(),
                                                BaseAmtAR = "",

                                            },
                                        },
                                        PaidVATCategoryTaxableAmt = DPMrecset.Fields.Item("DPTaxableAmt").Value.ToString(),
                                        PaidVATCategoryTaxAmt = DPMrecset.Fields.Item("DPTaxAmt").Value.ToString(),
                                        PrepaymentDocType = DPMrecset.Fields.Item("DPDoctype").Value.ToString(),
                                        PrepaymentID = DPMrecset.Fields.Item("DPID").Value.ToString(),
                                        PrepaymentIssueDate = clsModule.objaddon.objglobalmethods.DateFormat(clsModule.objaddon.objglobalmethods.Getdateformat(DPMrecset.Fields.Item("DPIssudt").Value.ToString()), "dd/MM/yyyy", "yyyy-MM-dd"),
                                        PrepaymentIssueTime = clsModule.objaddon.objglobalmethods.ConverttoTime(DPMrecset.Fields.Item("DPIssutime").Value.ToString()),
                                        PrepaymentUUID = DPMrecset.Fields.Item("DPUUID").Value.ToString(),
                                    });
                                    DPMrecset.MoveNext();
                                }
                            }




                            requestParams = JsonConvert.SerializeObject(GenerateIRNGetJson);
                       
                        if (!(Einvstus == "CLEARED"))
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Generating Einvoice. Please Wait...." + DocEntry, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                            Queryparameter = new Dictionary<string, string>();
                            Queryparameter.Add("autoExecuteRules", "true");
                            Queryparameter.Add("transformationName", objRs.Fields.Item("U_TranName").Value.ToString());

                            head = new Dictionary<string, string>();
                            head.Add("authorization", "Bearer " + Accesstkn);

                            datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString(), headers: head, Queryparameter: Queryparameter);
                            if (datatable.Rows.Count > 0)
                            {
                                blnstatus = true;
                            }
                        }
                            string msg = "";
                            Response js = new Response();
                            if (blnstatus)
                            {
                               
                                string BaseSysPath = Getbasepath();
                                string SysPath = BaseSysPath + GenerateIRNGetJson.ReferenceNumber + "_";
                                SysPath += GenerateIRNGetJson.BlngRefIssueDt;

                                string FileName = SysPath + "_PDF.pdf";
                                string FilePDFA= SysPath + "_PDFA3.pdf";
                                string Xmlpath = SysPath + "_Xml.xml";

                            List<string> PathDOCList = new List<string>();

                            Thread.Sleep(10000);

                                GetXML(DocEntry, TransType, Accesstkn,Xmlpath,ref datatable);
                                if (datatable.Rows.Count == 0)
                                {


                                   
                                    strSQL = @"Select T0.""U_Live"",T0.""U_UATUrl"",T0.""U_LiveUrl"",T0.""U_AuthKey"",T0.""U_SerConfig"",T1.""U_URLType"",T1.""U_URL"", ";
                                    strSQL += @"Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LiveUrl"",T1.""U_URL"") End as URL,T0.""U_DevID"",T0.""U_Startdate"" ";
                                    strSQL += @" from ""@EICON"" T0 join ""@EICON1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";
                                    strSQL += @" and T1.""U_URLType""='Get E-Invoice' ";

                                    objRs.DoQuery(strSQL);
                                    if (objRs.RecordCount == 0)
                                    {
                                        clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Get E-Invoice\". Please update in E-invoice Configuration... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                        return false;
                                    }

                                Queryparameter = new Dictionary<string, string>();
                                Queryparameter.Add("financialyear", GenerateIRNGetJson.FinancialYear);
                                    Queryparameter.Add("ref_nm", GenerateIRNGetJson.ReferenceNumber);
                                    Queryparameter.Add("invoicetypecode", GenerateIRNGetJson.InvTypeCd);

                                    head = new Dictionary<string, string>();
                                    head.Add("authorization", "Bearer " + Accesstkn);


                                    datatable = Get_API_Response("", objRs.Fields.Item("URL").Value.ToString(), httpMethod: "GET", headers: head, Queryparameter: Queryparameter);

                              
                                    if (datatable.Rows.Count > 0)
                                    {
                                     if(clsModule.objaddon.objglobalmethods.CheckIfColumnExists(datatable, "errors"))
                                        {
                                            Errors error = JsonConvert.DeserializeObject<Errors>(datatable.Rows[0]["errors"].ToString());
                                            js.ErrorResponse.errors = error;
                                        }
                                       else if (clsModule.objaddon.objglobalmethods.CheckIfColumnExists(datatable, "Invoice"))
                                        {
                                            js = JsonConvert.DeserializeObject<Response>(datatable.Rows[0]["Invoice"].ToString());
                                        }
                                        else
                                        {
                                            
                                           

                                            js = new Response();
                                            js.ErrorResponse.errors.entities.Add(new Entity
                                            {
                                                records = new List<Record> { new Record { errors = new List<string> { "Regenerate After few Minutes" } } }
                                            });


                                        }                                        
                                    }

                                   
                                }
                                else
                                {
                                    if (clsModule.objaddon.objglobalmethods.CheckIfColumnExists(datatable, "errors"))
                                    {
                                        Errors error = JsonConvert.DeserializeObject<Errors>(datatable.Rows[0]["errors"].ToString());
                                        js.ErrorResponse.errors = error;
                                    }
                                   else
                                    {

                                        js = new Response();
                                        js.ErrorResponse.errors.entities.Add(new Entity
                                        {
                                            records = new List<Record> { new Record { errors = new List<string> { "Regenerate After few Minutes" } } }
                                        });

                                    }
                                }
                            Einvlog = E_Invoice_Logs(DocEntry, js, TransType, "Create", Type);


                            GetPDF(DocEntry, TransType, FileName);
                            GetPDFA3(DocEntry, TransType, FilePDFA);


                            PathDOCList.Add(FileName);
                            PathDOCList.Add(FilePDFA);
                            PathDOCList.Add(Xmlpath);
                            clsModule.objaddon.objglobalmethods.saveattachment(DocEntry, PathDOCList, TransType);


                        }

                    }

                    else
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("No data found for this invoice...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_None);
                    }
                    GenerateIRNGetJson = null;

                }

            }
            catch (Exception ex)
            {
                clsModule.objaddon.objglobalmethods.WriteErrorLog(ex.StackTrace);
                clsModule.objaddon.objapplication.StatusBar.SetText("Error_IRN: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            return true;
        }
        private bool E_Invoice_Logs(string InvDocEntry, Response einv, string ObjType, string Type, string TranType)
        {
            try
            {
                blnRefresh = false;
            
                SAPbobsCOM.GeneralData oGeneralData;
                SAPbobsCOM.GeneralDataParams oGeneralParams;
                SAPbobsCOM.GeneralService oGeneralService;

                oGeneralService = clsModule.objaddon.objcompany.GetCompanyService().GetGeneralService("EINVLOG");
                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
              
                objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                string Errors = "";
                string warning = "";
                bool err = false;

                foreach (Entity item in einv.ErrorResponse.errors.entities)
                {

                    Errors += string.Join("\n ", item.records[0].errors) + "\n";
                    warning  += string.Join("\n ", item.records[0].warnings) + "\n";

                    foreach (EInvoice.Models.Field field in item.records[0].fields)
                    {
                        
                        Errors +=  string.Join("\n ", field.errors)+"\n";
                        warning +=  string.Join("\n ", field.warnings) + "\n";
                    }
                    err = true;    
                }

                foreach (object item in einv.ErrorResponse.errors.errors)
                {

                    Errors += string.Join("\n ", item.ToString()) + "\n";                                  
                    err = true;
                }
                if (Type == "Create")
                {
                    if (!err)
                    {
                        oGeneralData.SetProperty("U_QRCod", string.IsNullOrEmpty(einv.AdditionalDocumentReference.QR.Attachment.EmbeddedDocumentBinaryObject) ? "" : einv.AdditionalDocumentReference.QR.Attachment.EmbeddedDocumentBinaryObject);
                        oGeneralData.SetProperty("U_RawQR", string.IsNullOrEmpty(einv.AdditionalDocumentReference.QR.Attachment.EmbeddedDocumentBinaryObject) ? "" : einv.AdditionalDocumentReference.QR.Attachment.EmbeddedDocumentBinaryObject);
                        oGeneralData.SetProperty("U_UUID", einv.UUID);
                        oGeneralData.SetProperty("U_PIH", einv.AdditionalDocumentReference.PIH.Attachment.EmbeddedDocumentBinaryObject);
                        oGeneralData.SetProperty("U_InvHash", einv.ID);
                        oGeneralData.SetProperty("U_ICV", einv.AdditionalDocumentReference.UUID._UUID);
                        oGeneralData.SetProperty("U_EINVStat", string.IsNullOrEmpty(einv.AdditionalDocumentReference.QR.Attachment.EmbeddedDocumentBinaryObject) ? "FAILED" : "CLEARED");
                        oGeneralData.SetProperty("U_INVTyp", ObjType);
                        oGeneralData.SetProperty("U_IssueDt", einv.IssueDate);
                        oGeneralData.SetProperty("U_Issuetm", einv.IssueTime);
                        oGeneralData.SetProperty("U_GenDt", einv.ZatcaResponseDate.Date.ToString("yyyy-MM-dd"));
                        oGeneralData.SetProperty("U_Gentm", einv.ZatcaResponseDate.TimeOfDay.ToString());
                        oGeneralData.SetProperty("U_DocEntry", InvDocEntry);
                        oGeneralParams = oGeneralService.Add(oGeneralData);

                        if (TranType == "E-Invoice")
                        {
                            saveEinvfields(InvDocEntry, einv, ObjType);
                            blnRefresh = true;
                        }
                    }
                    else
                    {
                        oGeneralData.SetProperty("U_EINVStat","FAILED");
                        oGeneralData.SetProperty("U_INVTyp", ObjType);
                        oGeneralData.SetProperty("U_DocEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_ErrList", Errors);
                        oGeneralData.SetProperty("U_WarnList", warning);
                        oGeneralParams = oGeneralService.Add(oGeneralData);

                        SAPbobsCOM.Documents objsalesinvoice = null;
                        switch (ObjType)
                        {
                            case "INV":
                                objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);

                                break;
                            case "CRN":
                                objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                                break;
                            case "DPI":
                                objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDownPayments);

                                break;
                        }
                        objsalesinvoice.GetByKey(Convert.ToInt32(InvDocEntry));
                        
                        objsalesinvoice.UserFields.Fields.Item("U_Warn").Value = warning;
                        objsalesinvoice.UserFields.Fields.Item("U_Error").Value = Errors;
                        objsalesinvoice.UserFields.Fields.Item("U_EinvStatus").Value = "FAILED";
                                               
                       int  ret=objsalesinvoice.Update();
                        if (ret!=0)
                        {
                            string strerr="";
                            clsModule.objaddon.objcompany.GetLastError(out ret, out strerr);
                            clsModule.objaddon.objglobalmethods.WriteErrorLog(strerr);
                        }
                        blnRefresh = true;
                    }
                   

                }

                objRs = null;
                return true;
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objglobalmethods.WriteErrorLog(ex.ToString());
                clsModule.objaddon.objapplication.StatusBar.SetText("E_Invoice_Logs: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
        }

        private bool saveEinvfields(string DocEntry, Response einv, string TransType)
        {

            SAPbobsCOM.Documents objsalesinvoice =null;
            switch (TransType)
            {
                case "INV": 
                    objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);

                    break;
                case "CRN":
                    objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                    break;
                case "DPI":
                    objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDownPayments);

                    break;
            }
            objsalesinvoice.GetByKey(Convert.ToInt32( DocEntry));
            objsalesinvoice.UserFields.Fields.Item("U_PIHNo").Value = einv.AdditionalDocumentReference.PIH.Attachment.EmbeddedDocumentBinaryObject;
            objsalesinvoice.UserFields.Fields.Item("U_UUIDNo").Value = einv.UUID;
            objsalesinvoice.UserFields.Fields.Item("U_InvoiceHashNo").Value = einv.ID;
            objsalesinvoice.UserFields.Fields.Item("U_EinvStatus").Value = (string.IsNullOrEmpty(einv.AdditionalDocumentReference.QR.Attachment.EmbeddedDocumentBinaryObject) ? "FAILED" : "CLEARED");
            objsalesinvoice.UserFields.Fields.Item("U_Issuedt").Value = einv.ZatcaResponseDate.Date.ToString("yyyy-MM-dd");
            objsalesinvoice.UserFields.Fields.Item("U_Warn").Value = "";
            objsalesinvoice.UserFields.Fields.Item("U_Error").Value = "";
            objsalesinvoice.UserFields.Fields.Item("U_ICVNo").Value = einv.AdditionalDocumentReference.UUID._UUID;
            objsalesinvoice.CreateQRCodeFrom =einv.AdditionalDocumentReference.QR.Attachment.EmbeddedDocumentBinaryObject;
            objsalesinvoice.Update();


            return true;
        }

     
        private DataTable Get_API_Response(string JSON, string URL, string httpMethod = "POST", string contenttype = "application/json",
           Dictionary<string, string> headers = null, NameValueCollection formdata1 = null, string filepath = "", Dictionary<string, string> Queryparameter = null)
        {
            try
            {
                if (Queryparameter != null)
                {
                    URL +="?";
                    int i = 0;
                    foreach (var item in Queryparameter)
                    {
                        if (i!=0)
                        {
                            URL += "&";
                        }
                        URL += item.Key + "=" + item.Value;
                        i++;
                    }
                }

                clsModule.objaddon.objglobalmethods.WriteErrorLog(URL);
                clsModule.objaddon.objglobalmethods.WriteErrorLog(JSON);

                DataTable datatable = new DataTable();
                HttpWebRequest webRequest;
                webRequest = (HttpWebRequest)WebRequest.Create(URL);
                webRequest.Method = httpMethod;
                byte[] byteArray = new byte[] { };
                if (!string.IsNullOrEmpty(JSON))
                {
                    webRequest.ContentType = contenttype;
                    byteArray = Encoding.UTF8.GetBytes(JSON);
                    webRequest.ContentLength = byteArray.Length;
                }
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        webRequest.Headers.Add(item.Key, item.Value);
                    }
                }

                if (formdata1 != null)
                {
                    string boundary = "----" + Guid.NewGuid().ToString("N");

                    string formDataString = clsModule.objaddon.objglobalmethods.BuildFormData(formdata1, boundary);


                    byte[] formDataBytes = Encoding.UTF8.GetBytes(formDataString);
                    webRequest.ContentType = contenttype + "; boundary=" + boundary;

                    webRequest.ContentLength = formDataBytes.Length;
                    using (Stream requestStream = webRequest.GetRequestStream())
                    {
                        requestStream.Write(formDataBytes, 0, formDataBytes.Length);
                    }

                }
                else
                {
                    if (byteArray.Length != 0)
                    {
                        webRequest.ContentType = contenttype;
                        using (Stream requestStream = webRequest.GetRequestStream())
                        {
                            requestStream.Write(byteArray, 0, byteArray.Length);
                        }
                    }
                }

                try
                {
                    using (WebResponse response = webRequest.GetResponse())
                    {
                        if (response is HttpWebResponse httpResponse)
                        {
                            if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode== HttpStatusCode.Accepted)
                            {
                                switch (httpResponse.ContentType)
                                {
                                    case "application/pdf":
                                    case "application/xml":

                                        using (Stream responseStream = response.GetResponseStream())
                                        {
                                            if (responseStream != null)
                                            {
                                                string outputPath = filepath;
                                                if (File.Exists(outputPath))
                                                    File.Delete(outputPath);

                                                using (FileStream fileStream =  new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                                                {
                                                    byte[] buffer = new byte[4096];
                                                    int bytesRead;

                                                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                                    {
                                                        fileStream.Write(buffer, 0, bytesRead);
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            using (Stream responseStream = response.GetResponseStream())
                                            {
                                                StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                                                string Json = rdr.ReadToEnd();
                                                clsModule.objaddon.objglobalmethods.WriteErrorLog(Json);
                                                datatable = clsModule.objaddon.objglobalmethods.Jsontodt(Json);
                                            }
                                        }
                                        break;
                                }
                            }
                        }

                    }

                }
                catch (WebException webEx)
                {
                    if (webEx.Response is HttpWebResponse httpWebResponse)
                    {
                        switch(httpWebResponse.StatusCode)
                        {
                            case HttpStatusCode.BadRequest:
                            case (System.Net.HttpStatusCode)422:
                            using (Stream errorResponseStream = httpWebResponse.GetResponseStream())
                        {
                            StreamReader rdr = new StreamReader(errorResponseStream, Encoding.UTF8);
                            string Json = rdr.ReadToEnd();
                            clsModule.objaddon.objglobalmethods.WriteErrorLog(Json);
                            datatable = clsModule.objaddon.objglobalmethods.Jsontodt(Json);
                        }
                                break;
                    }

                }
                     
                }



                return datatable;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string Getbasepath()
        {
            string path;
            string lstrquery;
            path = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"AttachPath\" from OADP");
            lstrquery = "SELECT CAST(t2.\"AttachPath\" AS nvarchar) AS \"Apath\"  FROM OUSR t1 LEFT JOIN OUDG t2 ON t1.\"DfltsGroup\" = t2.\"Code\" WHERE USER_CODE = '" + clsModule.objaddon.objcompany.UserName + "' ";
            path += clsModule.objaddon.objglobalmethods.getSingleValue(lstrquery);

            return path;
        }
        public void buttonenable(SAPbouiCOM.Form oForm)
        {
            try
            {


                SAPbouiCOM.Form oUDFForms;
                SAPbouiCOM.Button button = null;
                string Einvsts;
                string status;
                string DocEntry;
                string user;
                string tablename = "";
                switch (oForm.Type.ToString())
                {
                    case "133":
                        tablename = "OINV";

                        break;
                    case "179":
                        tablename = "ORIN";
                        break;
                    case "65300":
                        tablename = "ODPI";
                        break;
                    default:
                        return;
                }
                
                button = (SAPbouiCOM.Button)oForm.Items.Item("btneinv").Specific;
                EnabledMenu(oForm);
                Einvsts = oForm.DataSources.DBDataSources.Item(tablename).GetValue("U_EinvStatus", 0); 
                status = oForm.DataSources.DBDataSources.Item(tablename).GetValue("DocStatus", 0);
                DocEntry = oForm.DataSources.DBDataSources.Item(tablename).GetValue("DocEntry", 0);
                user = oForm.DataSources.DBDataSources.Item(tablename).GetValue("Usersign", 0);

              

                if (string.IsNullOrEmpty(DocEntry))
                {
                    button.Item.Enabled = true;
                    return;
                }
                string Docuser = "";
                string expectuser = clsModule.objaddon.objglobalmethods.getSingleValue("SELECT Max(CAST(\"U_ExpctUser\" AS nvarchar)) AS \"ExpctUser\" FROM \"@EICON\" e;");
                Docuser = oForm.DataSources.DBDataSources.Item(tablename).GetValue("Usersign", 0);
                if (!string.IsNullOrEmpty(Docuser))
                {
                    user = clsModule.objaddon.objglobalmethods.getSingleValue(" SELECT \"USER_CODE\"  FROM OUSR o WHERE o.USERID = " + Docuser);
                    List<string> outputList = new List<string>(expectuser.Split(','));

                    foreach (string item in outputList)
                    {
                        string repl = item.Replace("'", "");
                        if (user == repl)
                        {
                            button.Item.Enabled = false;
                            return;
                        }

                    }
                }
                if (status == "C")
                {
                    string Confset = clsModule.objaddon.objglobalmethods.getSingleValue("SELECT  \"U_CloseInv\" FROM \"@EICON\" e WHERE \"Code\" = '01'");
                    if (Confset == "False")
                    {
                        button.Item.Enabled = false;
                        return;
                    }
                }
                string cancel = oForm.DataSources.DBDataSources.Item(tablename).GetValue("CANCELED", 0);

                if (cancel == "Y")
                {
                    button.Item.Enabled = false;
                    return;
                }
                if (string.IsNullOrEmpty(DocEntry))
                {
                    button.Item.Enabled = true;
                    return;
                }

                switch (Einvsts)
                {
                    case "CLEARED":
                    case "REPORTED":
                        Einvsts = "CLEARED";
                        break;
                }

                if (string.IsNullOrEmpty(Einvsts))
                {
                    button.Item.Enabled = true;
                    return;
                }

                else if (Einvsts!="CLEARED")
                {
                    button.Item.Enabled =true;
                    return;
                }

              

                List<string> Checkdoc = new List<string>();
                List<string> savedoc = new List<string>();
                string strsql;
                DataTable dt = new DataTable();


                strsql = "select \"DocNum\",\"DocDate\" from  " + tablename + " where \"DocEntry\"=" + DocEntry;
                dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(strsql);
                if (dt.Rows.Count > 0)
                {
                    string BaseSysPath = Getbasepath();
                    string SysPath = BaseSysPath + Convert.ToString(dt.Rows[0]["DocNum"]) + "_";
                    SysPath += clsModule.objaddon.objglobalmethods.DateFormat(clsModule.objaddon.objglobalmethods.Getdateformat(Convert.ToString(dt.Rows[0]["DocDate"])), "dd/MM/yyyy", "yyyy-MM-dd");
                    Checkdoc.Add(SysPath + "_PDF.pdf");
                    Checkdoc.Add(SysPath + "_PDFA3.pdf");
                    Checkdoc.Add(SysPath + "_Xml.xml");
                }

                strsql = "SELECT CAST(T1.\"trgtPath\" AS varchar)AS \"Trgtpath\",CAST(T1.\"FileName\" AS varchar) AS \"Filename\"," +
                     " CAST(T1.\"FileExt\" AS varchar) AS \"FileExt\"  FROM " + tablename + " T0 left join ATC1 T1 on T0.\"AtcEntry\" = T1.\"AbsEntry\" Where T0.\"DocEntry\" =" + DocEntry;
                dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(strsql);
                foreach (DataRow path in dt.Rows)
                {
                    string FileName = path["Trgtpath"].ToString() + "\\" + path["Filename"].ToString() + "." + path["FileExt"].ToString();
                    savedoc.Add(Path.GetFileName(FileName));
                }
                bool notfound = false;
                foreach (string item in Checkdoc)
                {
                    string checkfileName = Path.GetFileName(item);
                 

                    if (savedoc.IndexOf(checkfileName) == -1)
                    {
                        notfound = true;
                        break;
                    }
                }

                if (status == "C" && !notfound)
                {
                    button.Item.Enabled = false;
                    return;
                }

               

                button.Item.Enabled = notfound;


            }

            catch (Exception ex)
            {
                return;
            }
        }

    }
}
