using EInvoice.Common;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EInvoice.Business_Objects
{
    [FormAttribute("EILOGVIEW", "Business_Objects/EILog.b1f")]
    public class EILog : UserFormBase
    {
        public static SAPbouiCOM.Form objform;
        public EILog()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("Frmdt").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("Todt").Specific));
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("Grid").Specific));
            this.Grid0.LinkPressedAfter += new SAPbouiCOM._IGridEvents_LinkPressedAfterEventHandler(this.Grid0_LinkPressedAfter);
            this.Grid0.LinkPressedBefore += new SAPbouiCOM._IGridEvents_LinkPressedBeforeEventHandler(this.Grid0_LinkPressedBefore);
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_5").Specific));
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.ComboBox1 = ((SAPbouiCOM.ComboBox)(this.GetItem("Ttrntyp").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Ltrntyp").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_0").Specific));
            this.ComboBox0 = ((SAPbouiCOM.ComboBox)(this.GetItem("Cstus").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);

        }

        private void OnCustomInitialize()
        {


            EditText1.Value = DateTime.Today.ToString("yyyyMMdd");
            EditText0.Value = DateTime.Today.ToString("yyyyMMdd");
            Loaddata();
            string qr = "SELECT DISTINCT  \"U_EINVStat\" as \"col1\" ,\"U_EINVStat\"  as \"col2\" FROM \"@EILOG\" e WHERE  \"U_EINVStat\"<>'' order by   \"col1\" ";
            clsModule.objaddon.objglobalmethods.Load_Combo(ComboBox0, qr, new[] { "NOT_SENT,NOT_SENT", "ALL,ALL" });

            ComboBox1.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            ComboBox0.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
        }

        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.Grid Grid0;
        private SAPbouiCOM.Button Button0;

        private void Loaddata()
        {
            try
            {

                string series = "";
                string lstrquery = "SELECT o2.\"U_Prefix\"  FROM OUSR o LEFT JOIN OUBR o2 ON o2.\"Code\" = o.\"Branch\" WHERE o.USER_CODE = '" + clsModule.objaddon.objcompany.UserName + "' ";
                SAPbobsCOM.Recordset rs = clsModule.objaddon.objglobalmethods.GetmultipleRS(lstrquery);
                if (rs.RecordCount > 0)
                {

                    series = rs.Fields.Item("U_Prefix").Value.ToString();
                }

                objform.Freeze(true);






                DataTable dt = new DataTable();
                string query = "SELECT ";
                query += " \"DocEntry\" as \"Internal Number\", ";
                query += " \"DocNum\" as \"Doc No.\", ";

                query += " \"DocDate\" as \"Posting Date\", ";
                query += "  (SELECT \"SeriesName\"  FROM NNM1 n WHERE n.\"Series\"=s.\"Series\"  ) AS \"Series\", ";

                query += " \"INVTyp\" as \"Doc Type\", ";
                query += " \"QRCode\" as \"QR Code\", ";
                query += " \"EINVStat\" as \"E-Inv Status\", ";
                query += " \"QrStat\" as \"QR Status\", ";
                query += " \"IssueDt\" as \"E-Inv Date\", ";
                query += " \"Issuetm\" as \"E-Inv time\", ";
                query += " \"ErrList\" as \"Error Message\", ";
                query += " \"WarnList\" as \"Warning Message\", ";
                query += " \"SellVat\" as \"Seller VAT\", ";
                query += " \"BuyVat\" as \"Buyer VAT\", ";
                query += " \"ICV\" as \"ICV\", ";
                query += " \"UUID\" as \"UUID\", ";
                query += " \"PIH\" as \"PIH\", ";
                query += " \"InvHash\" as \"InvHash\", ";
                query += " \"RawQR\" as \"RawQR\", ";
                query += " \"DeviceId\" as \"DeviceId\", ";
                query += " \"INVXml\" as \"XML\", ";
                query += " \"valid\" as \"valid\", ";
                query += " \"UniqID\" as \"UniqID\", ";
                query += " \"UniqReqID\" as \"UniqReqID\" ";
                //query += " \"INVTyp\" as \"INVTyp\" ";

                query += " FROM(";
                query += "SELECT COALESCE(IDoc.\"DocNum\",CDoc.\"DocNum\",DDoc.\"DocNum\") as \"DocNum\", ";
                query += " COALESCE(IDoc.\"DocEntry\",CDoc.\"DocEntry\",DDoc.\"DocEntry\") as \"DocEntry\", ";
                query += " COALESCE(IDoc.\"DocDate\",CDoc.\"DocDate\",DDoc.\"DocDate\") as \"DocDate\", ";
                query += " COALESCE(IDoc.\"Series\",CDoc.\"Series\",DDoc.\"Series\") as \"Series\", ";

                query += " Elog.\"U_Status\" as \"Status\",ELog.\"U_QRCod\" as \"QRCode\",ELog.\"U_RawQR\" as \"RawQR\",Elog.\"U_UUID\" as \"UUID\",Elog.\"U_PIH\" as \"PIH\", ";
                query += " Elog.\"U_InvHash\" as \"InvHash\",Elog.\"U_ICV\" as \"ICV\",Elog.\"U_DeviceId\" as \"DeviceId\" ,Elog.\"U_SellVat\" as \"SellVat\", ";
                query += " Elog.\"U_BuyVat\" as \"BuyVat\",Elog.\"U_QrStat\" as \"QrStat\" ,Elog.\"U_EINVStat\" as \"EINVStat\", ";
                query += " Elog.\"U_INVTyp\" as \"INVTyp\" ,Elog.\"U_IssueDt\" as \"IssueDt\" ,ELog.\"U_Issuetm\" as \"Issuetm\",Elog.\"U_GenDt\" as \"GenDt\" , ";
                query += " Elog.\"U_Gentm\" as \"Gentm\",Elog.\"U_INVXml\" as \"INVXml\" ,Elog.\"U_WarnList\" as \"WarnList\",Elog.\"U_msg\" as \"msg\" , ";
                query += " Elog.\"U_Valid\" as \"valid\",Elog.\"U_UniqID\" as \"UniqID\",Elog.\"U_UniqReqID\" as \"UniqReqID\",Elog.\"U_Id\" as \"ID\", ";
                query += " Elog.\"U_Vat\" as \"Vat\",Elog.\"U_ErrList\" as \"ErrList\" ,";
                query += " ROW_NUMBER () OVER (PARTITION BY  COALESCE(IDoc.\"DocNum\",CDoc.\"DocNum\",DDoc.\"DocNum\"),\"U_INVTyp\" ORDER BY TO_TIME(\"U_Gentm\") DESC) AS rowss ";


                query += " FROM \"@EILOG\" ELog ";
                query += " Left JOIN \"OINV\" IDOC ON ELOG.\"U_DocEntry\" = IDOC.\"DocEntry\" and ELOG.\"U_INVTyp\"='INV' ";
                query += " Left JOIN \"ORIN\" CDOC ON ELOG.\"U_DocEntry\" = CDOC.\"DocEntry\" and ELOG.\"U_INVTyp\"='CRN' ";
                query += " Left JOIN \"OINV\" DDOC ON ELOG.\"U_DocEntry\" = DDOC.\"DocEntry\" and ELOG.\"U_INVTyp\"='DBN' ";


                query += " Left JOIN \"OUSR\" UIDOC ON  UIDOC.USERID =IDOC.\"UserSign\"  ";
                query += " Left JOIN \"OUSR\" UCDOC ON  UCDOC.USERID =CDOC.\"UserSign\"  ";
                query += " Left JOIN \"OUSR\" UDDOC ON  UDDOC.USERID =DDOC.\"UserSign\"  ";



                query += " where ELog.\"U_DocEntry\" <> 0 ";

                if (!string.IsNullOrEmpty(EditText0.Value.ToString()))
                {
                    query += " and (";
                    query += " IDOC.\"DocDate\" >= '" + EditText0.Value.ToString() + "'or ";
                    query += " CDOC.\"DocDate\" >= '" + EditText0.Value.ToString() + "'or ";
                    query += " DDOC.\"DocDate\" >= '" + EditText0.Value.ToString() + "' ";
                    query += ")";
                }
                if (!string.IsNullOrEmpty(EditText1.Value.ToString()))
                {
                    query += " and (";
                    query += " IDOC.\"DocDate\" <= '" + EditText1.Value.ToString() + "'or ";
                    query += " CDOC.\"DocDate\" <= '" + EditText1.Value.ToString() + "'or ";
                    query += " DDOC.\"DocDate\" <= '" + EditText1.Value.ToString() + "' ";
                    query += ")";
                }

                if (!string.IsNullOrEmpty(ComboBox1.Value.ToString()))
                {
                    if (ComboBox1.Value.ToString() != "ALL")
                        query += " and ELOG.\"U_INVTyp\" = '" + ComboBox1.Value.ToString() + "' ";

                }
                if (!string.IsNullOrEmpty(clsModule.objaddon.objglobalmethods.getSingleValue("SELECT Max(CAST(\"U_ExpctUser\" AS nvarchar)) AS \"ExpctUser\" FROM \"@EICON\" e;")))
                {

                    query += @" AND( UIDOC.USER_CODE NOT in(" + clsModule.objaddon.objglobalmethods.getSingleValue("SELECT Max(CAST(\"U_ExpctUser\" AS nvarchar)) AS \"ExpctUser\" FROM \"@EICON\" e;") + ") or ";
                    query += @"  UCDOC.USER_CODE NOT in(" + clsModule.objaddon.objglobalmethods.getSingleValue("SELECT Max(CAST(\"U_ExpctUser\" AS nvarchar)) AS \"ExpctUser\" FROM \"@EICON\" e;") + ") or ";
                    query += @"  UDDOC.USER_CODE NOT in(" + clsModule.objaddon.objglobalmethods.getSingleValue("SELECT Max(CAST(\"U_ExpctUser\" AS nvarchar)) AS \"ExpctUser\" FROM \"@EICON\" e;") + ") ";
                    query += ")";
                }


                if (!string.IsNullOrEmpty(series))

                {

                    query += @" AND( IDOC.""Series""  in(" + "SELECT \"Series\" FROM nnm1 WHERE \"SeriesName\" ='" + series + "' AND \"ObjectCode\"  ='13'" + ") or ";
                    query += @"  CDOC.""Series""  in(" + "SELECT \"Series\" FROM nnm1 WHERE \"SeriesName\" ='" + series + "' AND \"ObjectCode\"  ='14'" + ") or ";
                    query += @"  DDOC.""Series""  in(" + "SELECT \"Series\" FROM nnm1 WHERE \"SeriesName\" ='" + series + "' AND \"ObjectCode\"  ='13'" + ") ";
                    query += ")";

                }


                if (!string.IsNullOrEmpty(ComboBox0.Value.ToString()))
                {
                    switch (ComboBox0.Value.ToString())
                    {
                        case "ALL":
                            break;
                        case "NOT_SENT":
                            query += " and ELOG.\"U_EINVStat\" in('') ";
                            break;
                        default:
                            query += " and ELOG.\"U_EINVStat\" in('" + ComboBox0.Value.ToString() + "') ";
                            break;
                    }
                }


                query += ")s";
               
                query += " ORDER BY s.\"DocEntry\",s.\"IssueDt\",s.\"Issuetm\" ";
                clsModule.objaddon.objapplication.StatusBar.SetText("Preparing Data..", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                //   dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(query);
                clsModule.objaddon.objglobalmethods.WriteErrorLog(query);
                Grid0.DataTable.ExecuteQuery(query);


                SAPbouiCOM.EditTextColumn oColumns;
                oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("Internal Number");
                oColumns.LinkedObjectType = "13";
                Colsetting();

                this.GetItem("Grid").LinkTo = "DocEntry";

                clsModule.objaddon.objapplication.StatusBar.SetText("Opereation Completed Successfully...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            catch (Exception ex)
            {

                return;
            }
            finally
            {
                objform.Freeze(false);

            }
        }
        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            #region"fd"
            Loaddata();
            #endregion
        }

        private void Colsetting()
        {
            for (int i = 0; i < this.Grid0.Columns.Count; i++)
            {
                this.Grid0.Columns.Item(i).TitleObject.Sortable = true;                
                this.Grid0.Columns.Item(i).Width = 100;                
            }
            this.Grid0.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single;
        }

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
           
                try
                {
                    objform = clsModule.objaddon.objapplication.Forms.GetForm("EILOGVIEW", pVal.FormTypeCount);

                }
                catch (Exception ex)
                {

                    throw ex;
                }

            

        }

        private SAPbouiCOM.ComboBox ComboBox1;
        private SAPbouiCOM.StaticText StaticText3;

        private void Grid0_LinkPressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            
            SAPbouiCOM.EditTextColumn oColumns;
            oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
            string lstrtype = Grid0.DataTable.Columns.Item("INVTyp").Cells.Item(pVal.Row).Value.ToString();
            switch (lstrtype)
            {
                case "CRN":
                    oColumns.LinkedObjectType = "14";
                    break;
                default:
                    oColumns.LinkedObjectType = "13";
                    break;
            }
            BubbleEvent = true;
        }

        private void Grid0_LinkPressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
           
            
            

        }

        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.ComboBox ComboBox0;
    }
}

