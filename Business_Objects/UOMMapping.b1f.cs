using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EInvoice.Common;
using Newtonsoft.Json;
using SAPbobsCOM;
using SAPbouiCOM.Framework;

namespace EInvoice.Business_Objects
{
    [FormAttribute("UOMMAP", "Business_Objects/UOMMapping.b1f")]
    class UOMMapping : UserFormBase
    {
        public UOMMapping()
        {
        }
        public static SAPbouiCOM.Form objform;
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Matrix0 = ((SAPbouiCOM.Matrix)(this.GetItem("Mt_UOM").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.Button0.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button0_ClickAfter);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private SAPbouiCOM.Matrix Matrix0;

        private void OnCustomInitialize()
        {
            objform = clsModule.objaddon.objapplication.Forms.GetForm("UOMMAP",0);
            Matrix0.Columns.Item(clsModule.objaddon.objglobalmethods.GetColumnindex(Matrix0, "DocEntry")).Visible = false;
            clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix0, "UOM", "#");            
            Application.SBO_Application.SetStatusBarMessage("Loading...", SAPbouiCOM.BoMessageTime.bmt_Short,false);
            GetMappedjson();
            

        }

        private enum column
        {
            sno = 0,
            DocEntry,
            UntCd,
            UtDes,
            UOM,
        }
        private void GetMappedjson()
        {
            DataTable dt = new DataTable();
            objform.Freeze(true);
            string lstrquery = "SELECT \"DocEntry\" ,\"U_GUnitCod\" ,\"U_GUnitDes\" ,\"U_UOMCod\" FROM \"@UOMMAP\"";
            dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(lstrquery);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                Matrix0.AddRow(dt.Rows.Count - 1);
                foreach (DataRow Drow in dt.Rows)
                {

                    ((SAPbouiCOM.EditText)Matrix0.Columns.Item("#").Cells.Item(i).Specific).String = i.ToString();
                    ((SAPbouiCOM.EditText)Matrix0.Columns.Item("UntCd").Cells.Item(i).Specific).Value = Drow["U_GUnitCod"].ToString();
                    ((SAPbouiCOM.EditText)Matrix0.Columns.Item("UtDes").Cells.Item(i).Specific).Value = Drow["U_GUnitDes"].ToString();
                    ((SAPbouiCOM.EditText)Matrix0.Columns.Item("UOM").Cells.Item(i).Specific).Value = Drow["U_UOMCod"].ToString();
                    ((SAPbouiCOM.EditText)Matrix0.Columns.Item("DocEntry").Cells.Item(i).Specific).Value = Drow["DocEntry"].ToString();

                    Matrix0.CommonSetting.SetCellEditable(i, (int)column.UntCd, false);
                    Matrix0.CommonSetting.SetCellEditable(i, (int)column.UtDes, false);
                    i++;
                }
            }
            else
            {

                dt = clsModule.objaddon.objglobalmethods.JsonFiletodt("Mapjson.json");
                if (dt.Rows.Count > 0)
                {
                    int i = 1;
                    Matrix0.AddRow(dt.Rows.Count - 1);
                    foreach (DataRow Drow in dt.Rows)
                    {
                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("#").Cells.Item(i).Specific).String = i.ToString();
                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("UntCd").Cells.Item(i).Specific).Value = Drow["Unit Code"].ToString();
                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("UtDes").Cells.Item(i).Specific).Value = Drow["Unit Description"].ToString();
                        Matrix0.CommonSetting.SetCellEditable(i, (int)column.UntCd, false);
                        Matrix0.CommonSetting.SetCellEditable(i, (int)column.UtDes, false);
                        i++;


                    }
                }
            }
            objform.Freeze(false);
        }
        private SAPbouiCOM.Button Button0;

        private void Button0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.FormMode == 2)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText("Processing...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                if (UomMap())
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Data Saved Successfully...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                SAPbouiCOM.Form objform = clsModule.objaddon.objapplication.Forms.ActiveForm;
                objform.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
            }

        }
        public void Addrow()
        {
            try
            {
                clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix0, "UOM", "#");
            }
            catch (Exception ex)
            {

            }
        }


        private bool UomMap()
        {
            try
            {
                bool Flag = false;
                GeneralService oGeneralService;
                GeneralData oGeneralData;
                GeneralDataParams oGeneralParams;

                oGeneralService = clsModule.objaddon.objcompany.GetCompanyService().GetGeneralService("AUOMMAP");
                oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);


                for (int i = 1; i <= Matrix0.VisualRowCount; i++)
                {
                    if (((SAPbouiCOM.EditText)Matrix0.Columns.Item("UntCd").Cells.Item(i).Specific).String != "")
                    {
                        Flag = false;
                                                                                                              
                        if (((SAPbouiCOM.EditText)Matrix0.Columns.Item("DocEntry").Cells.Item(i).Specific).String != "")
                        {
                            Flag = true;
                            oGeneralParams.SetProperty("DocEntry", ((SAPbouiCOM.EditText)Matrix0.Columns.Item("DocEntry").Cells.Item(i).Specific).String);
                            oGeneralData = oGeneralService.GetByParams(oGeneralParams);                                                 
                        }
                        oGeneralData.SetProperty("U_GUnitCod", ((SAPbouiCOM.EditText)Matrix0.Columns.Item("UntCd").Cells.Item(i).Specific).String);
                        oGeneralData.SetProperty("U_GUnitDes", ((SAPbouiCOM.EditText)Matrix0.Columns.Item("UtDes").Cells.Item(i).Specific).String);
                        oGeneralData.SetProperty("U_UOMCod", ((SAPbouiCOM.EditText)Matrix0.Columns.Item("UOM").Cells.Item(i).Specific).String);
                        if (Flag == true)
                        {
                            oGeneralService.Update(oGeneralData);
                        }
                        else
                        {
                            oGeneralParams = oGeneralService.Add(oGeneralData);
                        }
                    }
                }
                return true;
            }

            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
        }
    }
}
