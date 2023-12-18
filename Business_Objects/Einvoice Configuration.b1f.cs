using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using EInvoice.Common;
using SAPbobsCOM;
using SAPbouiCOM.Framework;

namespace EInvoice.Business_Objects
{
    [FormAttribute("EINVCON", "Business_Objects/Einvoice Configuration.b1f")]
    public class Einvoice_Configuration : UserFormBase
    {
        public static SAPbouiCOM.Form oForm;
        private string FormName = "EINVCON";
        private int Formcnt ;
        private bool loaddata = false;


        public Einvoice_Configuration()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Matrix0 = ((SAPbouiCOM.Matrix)(this.GetItem("Item_0").Specific));
            this.Matrix0.ValidateAfter += new SAPbouiCOM._IMatrixEvents_ValidateAfterEventHandler(this.Matrix0_ValidateAfter);
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("TBUrl").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("TUUrl").Specific));
            this.EditText1.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.EditText1_KeyDownAfter);
            this.OptionBtn0 = ((SAPbouiCOM.OptionBtn)(this.GetItem("RUat").Specific));
            this.OptionBtn1 = ((SAPbouiCOM.OptionBtn)(this.GetItem("RLive").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("TAuth").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("TDocSer").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_9").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_10").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_11").Specific));
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("Item_12").Specific));
            this.Button2 = ((SAPbouiCOM.Button)(this.GetItem("Item_13").Specific));
            this.Button3 = ((SAPbouiCOM.Button)(this.GetItem("Item_14").Specific));
            this.Button4 = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.Button4.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button4_ClickAfter);
            this.Button4.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button4_ClickBefore);
            this.Button5 = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_2").Specific));
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("TDBName").Specific));
            this.StaticText5 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_5").Specific));
            this.EditText5 = ((SAPbouiCOM.EditText)(this.GetItem("TDBPass").Specific));
            this.EditText6 = ((SAPbouiCOM.EditText)(this.GetItem("ExptUser").Specific));
            this.StaticText6 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_6").Specific));
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("Item_7").Specific));
            this.Grid0.DoubleClickAfter += new SAPbouiCOM._IGridEvents_DoubleClickAfterEventHandler(this.Grid0_DoubleClickAfter);
            this.EditText7 = ((SAPbouiCOM.EditText)(this.GetItem("TdevID").Specific));
            this.StaticText7 = ((SAPbouiCOM.StaticText)(this.GetItem("LdevID").Specific));
            this.Matrix1 = ((SAPbouiCOM.Matrix)(this.GetItem("Mcrys").Specific));
            this.Matrix1.ValidateAfter += new SAPbouiCOM._IMatrixEvents_ValidateAfterEventHandler(this.Matrix1_ValidateAfter);
            this.EditText8 = ((SAPbouiCOM.EditText)(this.GetItem("Tcrysp").Specific));
            this.Folder2 = ((SAPbouiCOM.Folder)(this.GetItem("Item_21").Specific));
            this.Folder3 = ((SAPbouiCOM.Folder)(this.GetItem("Item_22").Specific));
            this.Folder4 = ((SAPbouiCOM.Folder)(this.GetItem("Item_23").Specific));
            this.StaticText9 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_24").Specific));
            this.StaticText10 = ((SAPbouiCOM.StaticText)(this.GetItem("StrtDt").Specific));
            this.EditText9 = ((SAPbouiCOM.EditText)(this.GetItem("Tstdt").Specific));
            this.CheckBox0 = ((SAPbouiCOM.CheckBox)(this.GetItem("Item_4").Specific));
            this.EditText10 = ((SAPbouiCOM.EditText)(this.GetItem("Item_8").Specific));
            this.StaticText8 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_15").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);

        }

        private SAPbouiCOM.Matrix Matrix0;

        private void OnCustomInitialize()
        {
            OptionBtn0.GroupWith("RLive");
            clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix0, "crurl", "#");
            clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix1, "crurl", "#");
            
            Loaddata();
            oForm.Items.Item("Item_21").Click();
            oForm.ClientHeight = Button4.Item.Top + 25;
            
        }

        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.OptionBtn OptionBtn0;
        private SAPbouiCOM.OptionBtn OptionBtn1;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.Button Button1;
        private SAPbouiCOM.Button Button2;
        private SAPbouiCOM.Button Button3;
        private SAPbouiCOM.Button Button4;
        private SAPbouiCOM.Button Button5;

        private bool E_Invoice_Config()
        {
            try
            {
                bool Flag = false;

                GeneralService oGeneralService;
                GeneralData oGeneralData;
                GeneralDataParams oGeneralParams;
                GeneralDataCollection oGeneralDataCollection;
               

                oGeneralService = clsModule.objaddon.objcompany.GetCompanyService().GetGeneralService("EICONFIG");
                oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);

                //GeneralData oChild;
                //oGeneralDataCollection = oGeneralData.Child("EICON1");
                //oChild = oGeneralDataCollection.Add();
                try
                {
                    oGeneralParams.SetProperty("Code", "01");
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    Flag = true;
                }
                catch (Exception ex)
                {
                    Flag = false;
                }


                oGeneralData.SetProperty("Code", "01");
                oGeneralData.SetProperty("Name", "01");
                oGeneralData.SetProperty("U_Live", Convert.ToString(((OptionBtn0.Selected == true) ? 'N' : 'Y')));
                oGeneralData.SetProperty("U_UATUrl", EditText0.Value);
                oGeneralData.SetProperty("U_LiveUrl", EditText1.Value);                
                oGeneralData.SetProperty("U_AuthKey", EditText2.Value);
                oGeneralData.SetProperty("U_SerConfig", EditText3.Value);
                oGeneralData.SetProperty("U_DBUser", EditText4.Value);
                oGeneralData.SetProperty("U_DBPass", EditText5.Value);
                oGeneralData.SetProperty("U_ExpctUser", EditText6.Value);
                oGeneralData.SetProperty("U_DevID", EditText7.Value);
                oGeneralData.SetProperty("U_Cryspath", EditText8.Value);
                oGeneralData.SetProperty("U_TranName", EditText10.Value);
                oGeneralData.SetProperty("U_CloseInv", CheckBox0.Checked.ToString());
                
                if (!string.IsNullOrEmpty(EditText9.Value))
                {
                    DateTime startDate;
                    if (DateTime.TryParseExact(EditText9.Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                    {
                        oGeneralData.SetProperty("U_Startdate", startDate);
                    }                   

                }
                else
                {
                    oGeneralData.SetProperty("U_Startdate", "");
                }


                int rowcount = 0;

                for (int i = 1; i <= Matrix0.VisualRowCount; i++)
                {
                    if (((SAPbouiCOM.EditText)Matrix0.Columns.Item("url").Cells.Item(i).Specific).String != "")
                    {


                        if (rowcount + 1 > oGeneralData.Child("EICON1").Count)
                        {
                            oGeneralData.Child("EICON1").Add();
                        }


                        oGeneralData.Child("EICON1").Item(rowcount).SetProperty("U_URLType", ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("urltype").Cells.Item(i).Specific).Selected.Value);
                        oGeneralData.Child("EICON1").Item(rowcount).SetProperty("U_URL", ((SAPbouiCOM.EditText)Matrix0.Columns.Item("url").Cells.Item(i).Specific).String);
                        rowcount++;
                    }
                }

                rowcount = 0;
                for (int i = 1; i <= Matrix1.VisualRowCount; i++)
                {
                    if (((SAPbouiCOM.EditText)Matrix1.Columns.Item("FileNm").Cells.Item(i).Specific).String != "")
                    {


                        if (rowcount + 1 > oGeneralData.Child("EICON2").Count)
                        {
                            oGeneralData.Child("EICON2").Add();
                        }

                        
                        oGeneralData.Child("EICON2").Item(rowcount).SetProperty("U_DocType", ((SAPbouiCOM.ComboBox)Matrix1.Columns.Item("DocType").Cells.Item(i).Specific).Selected.Value);
                        oGeneralData.Child("EICON2").Item(rowcount).SetProperty("U_TransType", ((SAPbouiCOM.ComboBox)Matrix1.Columns.Item("TransType").Cells.Item(i).Specific).Selected.Value);
                        oGeneralData.Child("EICON2").Item(rowcount).SetProperty("U_FileNm", ((SAPbouiCOM.EditText)Matrix1.Columns.Item("FileNm").Cells.Item(i).Specific).String);
                        rowcount++;
                    }
                }

                
                if (Flag == true)
                {
                    oGeneralService.Update(oGeneralData);
                    return true;
                }
                else
                {
                    oGeneralParams = oGeneralService.Add(oGeneralData);
                    return true;
                }
            }

            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
        }

        private void EditText1_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

        }

        private void Matrix0_ValidateAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (loaddata) return;
                switch (pVal.ColUID)
                {
                    case "url":
                        clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix0, "url", "#");
                        break;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        private void Matrix1_ValidateAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (loaddata) return;
                switch (pVal.ColUID)
                {
                    case "FileNm":
                        clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix1, "FileNm", "#");
                        break;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }
        private void Button4_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;


        }

        private void Button4_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            E_Invoice_Config();
        }

        private void Loaddata()
        {

            string strSQL = "";
            oForm = clsModule.objaddon.objapplication.Forms.GetForm(FormName, Formcnt);
            oForm.Freeze(true);
            try
            {

                strSQL = @"Select T0.""U_Live"",T0.""U_UATUrl"",T0.""U_LiveUrl"",T0.""U_AuthKey"",T0.""U_SerConfig"",T1.""U_URLType"",T1.""U_URL"",
                            T0.""U_DBPass"",T0.""U_DBUser"",T0.""U_ExpctUser"",T0.""U_DevID"",T0.""U_Cryspath"",T0.""U_Startdate"",T0.""U_CloseInv"",""U_TranName"" ";
                strSQL += @" from ""@EICON"" T0 join ""@EICON1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";


                DataTable dt = new DataTable();
                dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);

                if (dt.Rows.Count > 0)
                {
                    loaddata = true;
                    EditText0.Value = dt.Rows[0]["U_UATUrl"].ToString();
                    EditText1.Value = dt.Rows[0]["U_LiveUrl"].ToString();
                    EditText2.Value = dt.Rows[0]["U_AuthKey"].ToString();
                    EditText3.Value = dt.Rows[0]["U_SerConfig"].ToString();
                    EditText4.Value = dt.Rows[0]["U_DBUser"].ToString();
                    EditText5.Value = dt.Rows[0]["U_DBPass"].ToString();
                    EditText6.Value = dt.Rows[0]["U_ExpctUser"].ToString();
                    EditText7.Value = dt.Rows[0]["U_DevID"].ToString();
                    EditText8.Value = dt.Rows[0]["U_Cryspath"].ToString();
                    EditText10.Value = dt.Rows[0]["U_TranName"].ToString();
                    DateTime startDate;

                    if (DateTime.TryParseExact(dt.Rows[0]["U_Startdate"].ToString(), CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns(), CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                    {
                        EditText9.Value = startDate.ToString("yyyyMMdd");
                    }

                    foreach (DataRow Drow in dt.Rows)
                    {

                       

                      
                        //DateTime.Today.ToString("yyyyMMdd")
                      
                                         
                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("url").Cells.Item(Matrix0.VisualRowCount).Specific).String = Drow["U_URL"].ToString();
                        ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("urltype").Cells.Item(Matrix0.VisualRowCount).Specific).Select(Drow["U_URLType"].ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                       // ((SAPbouiCOM.EditText)Matrix0.Columns.Item("#").Cells.Item(Matrix0.VisualRowCount).Specific).String = Matrix0.VisualRowCount.ToString();
                        Matrix0.AddRow();
                    }

                    strSQL = @"Select T1.""U_DocType"",T1.""U_TransType"",T1.""U_FileNm"" ";
                    strSQL += @" from ""@EICON"" T0 join ""@EICON2"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";
                    
                    dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);

                    if (dt.Rows.Count>0)
                    {

                        foreach (DataRow Drow in dt.Rows)
                        {
                            ((SAPbouiCOM.EditText)Matrix1.Columns.Item("FileNm").Cells.Item(Matrix1.VisualRowCount).Specific).String = Drow["U_FileNm"].ToString();                            
                            ((SAPbouiCOM.ComboBox)Matrix1.Columns.Item("TransType").Cells.Item(Matrix1.VisualRowCount).Specific).Select(Drow["U_TransType"].ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                            ((SAPbouiCOM.ComboBox)Matrix1.Columns.Item("DocType").Cells.Item(Matrix1.VisualRowCount).Specific).Select(Drow["U_DocType"].ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                            // ((SAPbouiCOM.EditText)Matrix1.Columns.Item("#").Cells.Item(Matrix1.VisualRowCount).Specific).String = Matrix1.VisualRowCount.ToString();
                            Matrix1.AddRow();
                        }
                    }
                    oForm.Items.Item("TBUrl").Click();
                   

                    loaddata = false;
                }


                if (!(OptionBtn0.Selected == true | OptionBtn1.Selected == true))
                {
                    strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_Live\" from \"@EICON\" where \"Code\"='01'");
                    if (strSQL == "Y")
                    {
                        OptionBtn1.Item.Click();
                    }
                    else
                    {
                        OptionBtn0.Selected = true;
                    }
                }
                oForm.PaneLevel = 20;

                strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_CloseInv\" from \"@EICON\" where \"Code\"='01'");
                if (strSQL == "True")
                {
                    CheckBox0.Checked = true;
                }

                oForm.Freeze(false);
                
            }

            catch (Exception ex)
            {
                loaddata = false;
                oForm.Freeze(false);
            }


        }
        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            Formcnt = pVal.FormTypeCount;
        }

        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.EditText EditText4;
        private SAPbouiCOM.StaticText StaticText5;
        private SAPbouiCOM.EditText EditText5;
        private SAPbouiCOM.EditText EditText6;
        private SAPbouiCOM.StaticText StaticText6;
        private SAPbouiCOM.Grid Grid0;

        private void Grid0_DoubleClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            string cellValue = Grid0.DataTable.GetValue(0, pVal.Row).ToString();


            if (string.IsNullOrEmpty(EditText6.Value))
              {
                EditText6.Value = "'"+cellValue+"'";
            }
            else
            {
                EditText6.Value +="," + "'" + cellValue + "'";
            }
            
        }

        private SAPbouiCOM.EditText EditText7;
        private SAPbouiCOM.StaticText StaticText7;
        private SAPbouiCOM.Matrix Matrix1;
        private SAPbouiCOM.EditText EditText8;
        private SAPbouiCOM.Folder Folder2;
        private SAPbouiCOM.Folder Folder3;
        private SAPbouiCOM.Folder Folder4;
        private SAPbouiCOM.StaticText StaticText9;
        private SAPbouiCOM.StaticText StaticText10;
        private SAPbouiCOM.EditText EditText9;
        private SAPbouiCOM.CheckBox CheckBox0;
        private SAPbouiCOM.EditText EditText10;
        private SAPbouiCOM.StaticText StaticText8;
    }
}
