using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EInvoice.Common;
using SAPbouiCOM.Framework;
using System.Data;

namespace EInvoice.Business_Objects
{
   // [FormAttribute("EINVMULss", "Business_Objects/Einvoice.b1f")]
     class Einvoice : UserFormBase
    {
        public Einvoice()
        {
        }
        public static SAPbouiCOM.Form objform;
        private clsGlobalMethods stf = new clsGlobalMethods();
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Button4 = ((SAPbouiCOM.Button)(this.GetItem("BDis").Specific));
            this.Button5 = ((SAPbouiCOM.Button)(this.GetItem("Item_11").Specific));
            this.ComboBox0 = ((SAPbouiCOM.ComboBox)(this.GetItem("Item_0").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("Item_1").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("Item_2").Specific));
            this.Button6 = ((SAPbouiCOM.Button)(this.GetItem("BGenarate").Specific));
            this.Button6.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button6_ClickBefore);
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
            objform = clsModule.objaddon.objapplication.Forms.ActiveForm;
            objform.Freeze(true);
            ((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            ((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            ((SAPbouiCOM.EditText)objform.Items.Item("EDFrmDt").Specific).Value = DateTime.Today.ToString("yyyyMMdd");
            ((SAPbouiCOM.EditText)objform.Items.Item("EBToDt").Specific).Value = DateTime.Today.ToString("yyyyMMdd");
            objform.Items.Item("EDFrmDt").Click();
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("DocEntry").Visible = true;
            objform.Freeze(false);
        }
        private SAPbouiCOM.ComboBox ComboBox1;
        private SAPbouiCOM.StaticText StaticText1;

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                objform = clsModule.objaddon.objapplication.Forms.GetForm("EINVMUL", pVal.FormTypeCount);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private SAPbouiCOM.Grid Grid0;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.Button Button0;

        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

        }

        private SAPbouiCOM.Button Button1;

        private void Button0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            DataTable dt = new DataTable();


            string lstrquery = @"SELECT t1.""DocEntry"",t1.""DocNum"",t1.""DocDate"",t2.""CardName"" ,t2.""CardCode"" ,t2.""Phone1"",t1.""DocTotal"" ,""ShipToCode"",t3.""GSTRegnNo"",";
            if (clsModule.HANA)
            {
                lstrquery += @"  IFNULL(o.""U_Remarks"",'') U_Remarks FROM ";
            }
            else
            {
                lstrquery += @" isnull(o.""U_Remarks"",'') U_Remarks FROM ";
            }

            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value)
            {
                case "INV":
                    lstrquery += "oinv t1 ";
                    break;
                case "CRN":
                    lstrquery += "ORIN t1 ";
                    break;
            }


            lstrquery += @"inner Join OCRD t2 ON t2.""CardCode"" = t1.""CardCode""
                                inner JOIN crd1 t3 ON  t3.""Address"" = t1.""ShipToCode""  AND t3.""AdresType"" = 'S'";
            lstrquery += @" LEFT JOIN ""@ATPL_EINV"" o ON o.""U_BaseEntry"" =t1.""DocEntry"" ";
            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value)
            {
                case "INV":
                    lstrquery += " and o.\"U_DocObjType\"='13'";
                    break;
                case "CRN":
                    lstrquery += " and o.\"U_DocObjType\"='14'";
                    break;
            }
            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value)
            {

                case "E-way":
                    if (String.IsNullOrEmpty(clsModule.EwayNo))
                    {
                        switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value)
                        {
                            case "INV":
                                lstrquery += @"JOIN inv26 i ON i.""DocEntry"" =t1.""DocEntry"" ";
                                break;
                            case "CRN":
                                lstrquery += @"JOIN RIN26 i ON i.""DocEntry"" =t1.""DocEntry"" ";
                                break;
                        }
                    }
                    break;

            }

            lstrquery += @" WHERE T1.""DocDate"">='" + ((SAPbouiCOM.EditText)objform.Items.Item("EDFrmDt").Specific).Value + "'";
            lstrquery += @" And t1.""DocDate"" <='" + ((SAPbouiCOM.EditText)objform.Items.Item("EBToDt").Specific).Value + "'";
            lstrquery += @" AND t1.""DocStatus"" <>'C'";

            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value)
            {
                case "E-way":
                    if (String.IsNullOrEmpty(clsModule.EwayNo))
                    {
                        lstrquery += @" AND (i.""TransID""<>'' or i.""VehicleNo"" <>'')";
                        if (clsModule.HANA)
                        {
                            lstrquery += @" AND IFNULL(i.""EWayBillNo"",'')=''";
                        }
                        else
                        {
                            lstrquery += @"And isnull(i.EWayBillNo,'')=''";
                        }
                    }                  
                     else
                    {
                        lstrquery = lstrquery + "AND (IFNULL(t1.\"" + clsModule.EwayUDF + @""",'')<>''";
                        lstrquery = lstrquery + " OR IFNULL(t1.\"" + clsModule.EwayTransportId + @""",'')<>'')";
                    }
                    break;
                case "E-Invoice":
                    if (clsModule.HANA)
                    {
                        lstrquery += @" AND IFNULL(t1.""U_IRNNo"",'')=''";
                    }
                    else
                    {
                        lstrquery += @"And isnull(t1.U_IRNNo,'')=''";
                    }
                    break;

            }
            lstrquery += @"Order by ""DocDate""";

            if (!clsModule.HANA)
            {
                lstrquery = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(lstrquery);
            }
            dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(lstrquery);
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();
            objform.Items.Item("GRDet").LinkTo = "DocEntry";
            if (dt.Rows.Count > 0)
            {
                objform.Freeze(true);

                int i = 0;
                foreach (DataRow Drow in dt.Rows)
                {
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Add();
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Checkbox", i, "N");
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Number", i, Drow["DocNum"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("DocEntry", i, Drow["DocEntry"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Date", i, stf.Getdateformat(Drow["DocDate"].ToString()));
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Customer", i, Drow["CardName"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Mobile", i, Drow["phone1"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Total", i, Drow["DocTotal"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Remarks", i, Drow["U_Remarks"]);

                    i++;
                }
                objform.Items.Item("BGenarate").Visible = true;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("Checkbox").Visible = true;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("Remarks").Visible = true;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("E-Way Bill No").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("E-Way Bill Date").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("EWB Expiration Date").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("IRN NO").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("ACK Date").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("ACK No").Visible = false;

                objform.Freeze(false);
                this.Grid0.AutoResizeColumns();
            }
        }

        private SAPbouiCOM.Button Button2;

        private void Button2_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            bool checkvalue = false;
            objform.Freeze(true);
            for (int i = 0; i < Grid0.Rows.Count; i++)
            {
                string ss2 = ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Columns.Item("Checkbox").Cells.Item(i).Value.ToString();
                if (ss2 == "Y")
                {
                    checkvalue = true;
                    break;
                }
            }
            objform.Freeze(false);
            if (!checkvalue)
            {
                Application.SBO_Application.SetStatusBarMessage("Please Select Checkbox !!!!", SAPbouiCOM.BoMessageTime.bmt_Short, true);
                BubbleEvent = false;
            }
        }

        private void Button2_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            //throw new System.NotImplementedException();

            if (Grid0.Rows.Count > 0)
            {

                for (int i = 0; i < Grid0.Rows.Count; i++)
                {
                    string lstrdocentry = ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Columns.Item("DocEntry").Cells.Item(i).Value.ToString();
                    string lstrcheckbox = ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Columns.Item("Checkbox").Cells.Item(i).Value.ToString();
                    string TransType = ((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value;
                    string Type = ((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value;
                    DataTable dt = new DataTable();
                    string jsonstring = "";
                    if (lstrcheckbox == "Y")
                    {
                        switch (Type)
                        {
                            case "E-Invoice":
                             //   clsModule.objaddon.objInvoice.Generate_Cancel_IRN(ClsARInvoice.EinvoiceMethod.CreateIRN, lstrdocentry, TransType,Type, ref dt,false,ref jsonstring);
                                break;                          
                        }
                    }


                }
            }

        }

        private void ComboBox0_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();

        }

        private SAPbouiCOM.Button Button3;

        private void Button3_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            string ObjType = ((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value;
            string tb = "";
            DataTable dt = new DataTable();
            SAPbouiCOM.EditTextColumn oColumns;
            string lstrquery = @"SELECT t1.""DocEntry"",""DocNum"",""DocDate"",t2.""CardName"" ,t2.""CardCode"" ,t2.""Phone1"",t1.""DocTotal"" ,""ShipToCode"",t3.""GSTRegnNo"" ,t1.""U_IRNNo"" ,t1.""U_AckDate"" ,t1.""U_AckNo"" ";

            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                lstrquery += @" ,i.""EWayBillNo"" ,i.""EwbDate"" ,i.""ExpireDate""  ";
            }
            else
            {
                lstrquery += @" ,t1."""+ clsModule.EwayNo + @""" EWayBillNo ,'' ""EwbDate"" ,'' ""ExpireDate"" ";
            }

            lstrquery+=" FROM ";
            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value)
            {
                case "INV":
                    lstrquery += "oinv t1 ";
                    oColumns = (SAPbouiCOM.EditTextColumn)((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "13";
                    break;
                case "CRN":

                    oColumns = (SAPbouiCOM.EditTextColumn)((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "14";
                    lstrquery += "ORIN t1 ";
                    break;
            }


            lstrquery += @"inner Join OCRD t2 ON t2.""CardCode"" = t1.""CardCode""
                                inner JOIN crd1 t3 ON  t3.""Address"" = t1.""ShipToCode""  AND t3.""AdresType"" = 'S'
                                AND t3.""GSTRegnNo"" <> '' ";

            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                switch (ObjType)
                {
                    case "INV":
                        tb = "INV26";
                        break;
                    case "CRN":
                        tb = "RIN26";
                        break;
                }

                lstrquery += @"Left JOIN " + tb + @" i ON i.""DocEntry"" =t1.""DocEntry"" ";
            }
            lstrquery += @" WHERE T1.""DocDate"">='" + ((SAPbouiCOM.EditText)objform.Items.Item("EDFrmDt").Specific).Value + "'";
            lstrquery += @" And t1.""DocDate"" <='" + ((SAPbouiCOM.EditText)objform.Items.Item("EBToDt").Specific).Value + "'";

            string TransType = ((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value;
            switch (TransType)
            {
                case "E-way":
                    if (String.IsNullOrEmpty(clsModule.EwayNo))
                    {

                        lstrquery += @" AND (i.""TransID""<>'' or i.""VehicleNo"" <>'')";
                        if (clsModule.HANA)
                        {
                            lstrquery += @" AND IFNULL(i.""EWayBillNo"",'')<>''";
                        }
                        else
                        {
                            lstrquery += @"And isnull(i.EWayBillNo,'')<>''";
                        }
                    }
                    else
                    {
                        lstrquery += @" And (IFNULL(t1.""" + clsModule.EwayUDF + @""",'')<>''";
                        lstrquery += @" or IFNULL(t1.""" + clsModule.EwayTransportId + @""",'')<>'')";
                    }
                    break;
                case "E-Invoice":
                    if (clsModule.HANA)
                    {
                        lstrquery += @" AND IFNULL(t1.""U_IRNNo"",'')<>''";
                    }
                    else
                    {
                        lstrquery += @"And isnull(t1.U_IRNNo,'')<>''";
                    }
                    break;

            }
            lstrquery += @"Order by ""DocDate""";

            if (!clsModule.HANA)
            {
                lstrquery = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(lstrquery);
            }
            dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(lstrquery);
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("IRN NO").Visible = TransType == "E-Invoice";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("ACK Date").Visible = TransType == "E-Invoice";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("ACK No").Visible = TransType == "E-Invoice";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("E-Way Bill No").Visible = TransType == "E-way";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("E-Way Bill Date").Visible = TransType == "E-way";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("EWB Expiration Date").Visible = TransType == "E-way";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();
            objform.Items.Item("GRDet").LinkTo = "DocEntry";
            if (dt.Rows.Count > 0)
            {
                objform.Freeze(true);
                int i = 0;
                foreach (DataRow Drow in dt.Rows)
                {
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Add();
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Checkbox", i, "N");
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Number", i, Drow["DocNum"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("DocEntry", i, Drow["DocEntry"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Date", i, stf.Getdateformat(Drow["DocDate"].ToString()));
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Customer", i, Drow["CardName"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Mobile", i, Drow["phone1"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Total", i, Drow["DocTotal"]);

                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("E-Way Bill No", i, Drow["EWayBillNo"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("E-Way Bill Date", i, Drow["ExpireDate"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("EWB Expiration Date", i, Drow["DocTotal"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("IRN NO", i, Drow["U_IRNNo"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("ACK Date", i, Drow["U_AckDate"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("ACK No", i, Drow["U_AckNo"]);

                    i++;
                }
              
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("Checkbox").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("Remarks").Visible = false;
                objform.Items.Item("BGenarate").Visible = false;
                objform.Freeze(false);
                this.Grid0.AutoResizeColumns();
            }

        }

        private void ComboBox1_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();
        }

        private void Grid0_LinkPressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
       
        }

        private void Grid0_LinkPressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {


        }

        private void Grid0_DoubleClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            objform.Freeze(true);
            if (pVal.Row == -1)
            {
                for (int i = 0; i < Grid0.Rows.Count; i++)
                {
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Checkbox", i, "Y");
                }
                   
            }
            objform.Freeze(false);
        }

        

        private SAPbouiCOM.Button Button4;
        private SAPbouiCOM.Button Button5;



        private SAPbouiCOM.ComboBox ComboBox0;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.Button Button6;

        private void Button6_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            throw new System.NotImplementedException();

        }
    }
}
