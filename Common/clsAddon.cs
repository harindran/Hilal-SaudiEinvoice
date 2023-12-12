using EInvoice.Business_Objects;
using SAPbouiCOM.Framework;
using System;
using System.Data;
using System.IO;

namespace EInvoice.Common
{
    class clsAddon
    {
        public clsMenuEvent objmenuevent;
        public clsGlobalMethods objglobalmethods;
        public SAPbouiCOM.Application objapplication;
        public SAPbobsCOM.Company objcompany;
        public clsRightClickEvent objrightclickevent;
       
        public ClsARInvoice objInvoice;              
        public string[] HWKEY = { "L1653539483", "X1211807750","K1600107675" };
        #region Constructor
        public clsAddon()
        {

        }
        #endregion

        public void Intialize(string[] args)
        {
            try
            {
                Application oapplication;
                if ((args.Length < 1))
                    oapplication = new Application();
                else
                    oapplication = new Application(args[0]);
                objapplication = Application.SBO_Application;

              

              
                if (isValidLicense())
                {
                    objapplication.StatusBar.SetText("Establishing Company Connection Please Wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    objcompany = (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany();

                    Create_DatabaseFields(); // UDF & UDO Creation Part    
                    Menu(); // Menu Creation Part
                    Create_Objects(); // Object Creation Part
                    Add_Authorizations();
                    objapplication.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(objapplication_AppEvent);
                   objapplication.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(objapplication_MenuEvent);
                  objapplication.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(objapplication_ItemEvent);               
                   objapplication.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(ref FormDataEvent);
                    objapplication.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(objapplication_RightClickEvent);

              

                    objapplication.StatusBar.SetText("Addon Connected Successfully..!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    oapplication.Run();
                }
                else
                {
                    objapplication.StatusBar.SetText("Addon Disconnected due to license mismatch..!!!", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return;
                }
            }          
            catch (Exception ex)
            {
                
                WriteErrorLog(ex.Message);
                objapplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }
        public void WriteErrorLog(string Str)
        {
            try
            {

                string Foldername;
                Foldername = @"Log";
                if (Directory.Exists(Foldername))
                {
                }
                else
                {
                    Directory.CreateDirectory(Foldername);
                }

                FileStream fs;
                string chatlog = Foldername + @"\Log_" + DateTime.Now.ToString("ddMMyy") + ".txt";
                if (File.Exists(chatlog))
                {
                }
                else
                {
                    fs = new FileStream(chatlog, FileMode.Create, FileAccess.Write);
                    fs.Close();
                }
                string sdate;
                sdate = Convert.ToString(DateTime.Now);
                if (File.Exists(chatlog) == true)
                {
                    var objWriter = new StreamWriter(chatlog, true);
                    objWriter.WriteLine(sdate + " : " + Str);
                    objWriter.Close();
                }
                else
                {
                    var objWriter = new StreamWriter(chatlog, false);
                }
            }
            catch (Exception)
            {


            }
        }
        public bool isValidLicense()
        {
            return true;
            try
            {
                if (clsModule.HANA)
                {
                    try
                    {
                        if (objapplication.Forms.ActiveForm.TypeCount > 0)
                        {
                            for (int i = 0; i <= objapplication.Forms.ActiveForm.TypeCount - 1; i++)
                                objapplication.Forms.ActiveForm.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                objapplication.Menus.Item("257").Activate();
                SAPbouiCOM.EditText objedit = (SAPbouiCOM.EditText)objapplication.Forms.ActiveForm.Items.Item("79").Specific;

                string CrrHWKEY = objedit.Value.ToString();
                objapplication.Forms.ActiveForm.Close();

                for (int i = 0; i <= HWKEY.Length - 1; i++)
                {
                    if (HWKEY[i] == CrrHWKEY)
                    {
                        return true;
                    }

                }

                System.Windows.Forms.MessageBox.Show("Installing Add-On failed due to License mismatch");
                return false;
            }
            catch (Exception ex)
            {
                objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            return true;
        }

        public void Create_Objects()
        {
            objmenuevent = new clsMenuEvent();
            objrightclickevent = new clsRightClickEvent();
            objglobalmethods = new clsGlobalMethods();
            objInvoice = new ClsARInvoice();          
        }

        private void Create_DatabaseFields()
        {
            objapplication.StatusBar.SetText("Creating Database Fields.Please Wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            var objtable = new clsTable();
            objtable.FieldCreation();
            objapplication.StatusBar.SetText(" Database Created Successfully...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
        }

        #region Menu Creation Details

        private void Menu()
        {
            
            if (objapplication.Menus.Item("2048").SubMenus.Exists("E-Invoice"))
                return;                      
            CreateMenu("", objapplication.Menus.Item("2048").SubMenus.Count+1, "E-Invoice", SAPbouiCOM.BoMenuType.mt_POPUP, "E-Invoice", "2048");            
            CreateMenu("", objapplication.Menus.Item("E-Invoice").SubMenus.Count + 1, "E- Invoice Configuration", SAPbouiCOM.BoMenuType.mt_STRING, "EINVCON", "E-Invoice");
            CreateMenu("", objapplication.Menus.Item("E-Invoice").SubMenus.Count + 1, "E- Invoice Log", SAPbouiCOM.BoMenuType.mt_STRING, "EILOGVIEW", "E-Invoice");
            CreateMenu("", objapplication.Menus.Item("E-Invoice").SubMenus.Count + 1, "E- Invoice Multiple", SAPbouiCOM.BoMenuType.mt_STRING, "EINVMUL", "E-Invoice");

        }

        public void Add_Authorizations()
        {
            try
            {

                

                clsModule.objaddon.objglobalmethods.AddToPermissionTree("Altrocks Tech", "ATPL_ADD-ON", "", "", 'N');
                clsModule.objaddon.objglobalmethods.AddToPermissionTree("E-invoice", "ATPL_EINV", "", "ATPL_ADD-ON", 'N');
                clsModule.objaddon.objglobalmethods.AddToPermissionTree("E-invoice Configuration", "ATPL_EINVCON", "EINVCON", "ATPL_EINV", 'N');
                clsModule.objaddon.objglobalmethods.AddToPermissionTree("E-invoice Log", "ATPL_EILOGVIEW", "EILOGVIEW", "ATPL_EINV", 'N');
                clsModule.objaddon.objglobalmethods.AddToPermissionTree("E-invoice Multiple", "ATPL_EINVMUL", "EINVMUL", "ATPL_EINV", 'N');
           


            }
            catch (Exception ex)
            {

            }
        }

        private void CreateMenu(string ImagePath, int Position, string DisplayName, SAPbouiCOM.BoMenuType MenuType, string UniqueID, string ParentMenuID)
        {
            try
            {
                SAPbouiCOM.MenuCreationParams oMenuPackage;
                SAPbouiCOM.MenuItem parentmenu;
                parentmenu = objapplication.Menus.Item(ParentMenuID);
                if (parentmenu.SubMenus.Exists(UniqueID.ToString()))
                    return;
                oMenuPackage = (SAPbouiCOM.MenuCreationParams)objapplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams);
                oMenuPackage.Image = ImagePath;
                oMenuPackage.Position = Position;
                oMenuPackage.Type = MenuType;
                oMenuPackage.UniqueID = UniqueID;
                oMenuPackage.String = DisplayName;
                parentmenu.SubMenus.AddEx(oMenuPackage);
            }
            catch (Exception ex)
            {
                objapplication.StatusBar.SetText("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_None);
            }
        }

        #endregion

        public bool FormExist(string FormID)
        {
            bool FormExistRet = false;
            try
            {
                FormExistRet = false;
                foreach (SAPbouiCOM.Form uid in clsModule.objaddon.objapplication.Forms)
                {
                    if (uid.TypeEx == FormID)
                    {
                        FormExistRet = true;
                        break;
                    }
                }
                if (FormExistRet)
                {
                    clsModule.objaddon.objapplication.Forms.Item(FormID).Visible = true;
                    clsModule.objaddon.objapplication.Forms.Item(FormID).Select();
                }
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }

            return FormExistRet;

        }

        #region VIRTUAL FUNCTIONS
        public virtual void Menu_Event(ref SAPbouiCOM.MenuEvent pVal, ref bool BubbleEvent)
        { }

        public virtual void Item_Event(string oFormUID, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        { }

        public virtual void RightClick_Event(ref SAPbouiCOM.ContextMenuInfo oEventInfo, ref bool BubbleEvent)
        { }

        public virtual void FormData_Event(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        { }


        #endregion

        #region ItemEvent

        private void objapplication_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
           try
            {
                switch (pVal.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                        switch (pVal.FormTypeEx)
                        {
                            case "133":
                            case "179":
                            case "65300":
                            case "-133":
                            case "-179":
                            case "-65300":
                                clsModule.objaddon.objglobalmethods.WriteErrorLog(pVal.EventType.ToString() + pVal.ActionSuccess + pVal.FormTypeEx);
                                objInvoice.Item_Event(FormUID, ref pVal, ref BubbleEvent);
                                break;
                        }
                        break;

                    case SAPbouiCOM.BoEventTypes.et_GOT_FOCUS:
                        switch (pVal.FormTypeEx)
                        {
                            case "133":
                            case "179":
                            case "65300":
                                clsModule.objaddon.objglobalmethods.WriteErrorLog(pVal.EventType.ToString() + pVal.ActionSuccess);
                                objInvoice.Item_Event(FormUID, ref pVal, ref BubbleEvent);
                                break;
                            default:
                                return;
                        }
                        break;
                    case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:                        
                        if (pVal.FormTypeEx != "425") return;
                        clsModule.objaddon.objglobalmethods.WriteErrorLog(pVal.EventType.ToString() + pVal.ActionSuccess);
                        objInvoice.Item_Event(FormUID, ref pVal, ref BubbleEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_CLICK:
                        if (pVal.BeforeAction) return;                        
                            switch (pVal.FormTypeEx)
                        {
                            case "133":
                            case "179":
                            case "65300":
                                clsModule.objaddon.objglobalmethods.WriteErrorLog(pVal.EventType.ToString() + pVal.ActionSuccess);                                
                                objInvoice.Item_Event(FormUID, ref pVal, ref BubbleEvent);
                                break;                                
                        }                           
                        break;
                    case SAPbouiCOM.BoEventTypes.et_FORM_DRAW:
                        if (pVal.FormTypeEx == "179")
                        {
                            clsModule.objaddon.objglobalmethods.WriteErrorLog(pVal.EventType.ToString() + pVal.ActionSuccess);
                            objInvoice.Item_Event(FormUID, ref pVal, ref BubbleEvent);

                        }
                        break;                        
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        #endregion

        #region FormDataEvent

        private void FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                switch (BusinessObjectInfo.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE:                        
                    case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD:                        
                    case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                        objInvoice.FormData_Event(ref BusinessObjectInfo, ref BubbleEvent);
                        break;
                }
                
            }
            catch (Exception ex)
            {
               //throw ex;
            }
        }

        #endregion

        #region MenuEvent
        private void objapplication_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {

           
            BubbleEvent = true;
            if (pVal.BeforeAction == false)
                {
                    switch (pVal.MenuUID)
                    {
                        case "EINVCON":
                            Einvoice_Configuration Einvoice = new Einvoice_Configuration();
                            Einvoice.Show();
                            break;
                        case "EILOGVIEW":
                            EILog EILog = new EILog();
                            EILog.Show();
                            break;
                        case "EINVMUL":
                            EIMul EIMULLog = new EIMul();
                            EIMULLog.Show();
                            break;
                        case "1304":
                            SAPbouiCOM.Form activefrm = clsModule.objaddon.objapplication.Forms.ActiveForm;
                            objInvoice.buttonenable(activefrm);
                            break;
                        case "1287":
                            SAPbouiCOM.Form activefrmq = clsModule.objaddon.objapplication.Forms.ActiveForm;
                            switch (activefrmq.Type.ToString())
                            {
                                case "133":
                                case "179":
                                case "65300":
                                    Cleartext(activefrmq);
                                    objInvoice.EnabledMenu(activefrmq);
                                    break;

                            }
                            break;

                    }
                }
            }
            catch (Exception)
            {

              
            }
        }

        #endregion
        public void Cleartext(SAPbouiCOM.Form oForm)
        {
           
          
            try
            {
                oForm.Freeze(true);                
                //oForm.Items.Item("einv").Click();
                switch (oForm.Type.ToString())
                {
                    case "133":
                    case "179":
                    case "65300":
                        //   SAPbouiCOM.Form oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);
                        clsModule.objaddon.objglobalmethods.WriteErrorLog("Start Clear Form");
                        Editclear(oForm, "txtPIH");
                        Editclear(oForm, "txtUUID");
                        Editclear(oForm, "txtInvHash");
                        Editclear(oForm, "txtICV");
                        Editclear(oForm, "txtEinvSt");
                        Editclear(oForm, "txtWarn");
                        Editclear(oForm, "txtError");
                        Editclear(oForm, "txtIssueDt");
                        clsModule.objaddon.objglobalmethods.WriteErrorLog("Complete Clear Form");
                        break;
                    case "-133":
                    case "-179":
                    case "-65300":
                        clsModule.objaddon.objglobalmethods.WriteErrorLog("start  Clear UDF Form");
                        Editclear(oForm, "U_PIHNo");
                        Editclear(oForm, "U_UUIDNo");
                        Editclear(oForm, "U_InvoiceHashNo");
                        Editclear(oForm, "U_ICVNo");
                        Editclear(oForm, "U_EinvStatus");
                        Editclear(oForm, "U_Warn");
                        Editclear(oForm, "U_Error");
                        Editclear(oForm, "U_Issuedt");
                        clsModule.objaddon.objglobalmethods.WriteErrorLog("Complete  Clear UDF Form");
                        break;


                }                
                
            }
            catch (Exception ex)
            {

               
            }
            finally
            {

               oForm.Freeze(false);
            }

        }
        private void Editclear(SAPbouiCOM.Form oUDFForm, string name)
        {
            try
            {

                SAPbouiCOM.Item oItem = oUDFForm.Items.Item(name);
                SAPbouiCOM.EditText oEditText = (SAPbouiCOM.EditText)oItem.Specific;                
                if (!string.IsNullOrEmpty(oEditText.String))
                {
                    oUDFForm.Items.Item("einv").Click();
                    oEditText.Item.Enabled = true;                    
                    oEditText.Value = "";
                    oEditText.String = "";
                    oUDFForm.Items.Item("112").Click();
                    oEditText.Item.Enabled = false;
                }
            }
            catch (Exception ex)
            {

                return;
            }


        }
        #region RightClickEvent

        private void objapplication_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                switch (objapplication.Forms.ActiveForm.TypeEx)
                {
                    case "138":
                        objrightclickevent.RightClickEvent(ref eventInfo, ref BubbleEvent);
                        break;
                    case "UOMMAP":
                        objrightclickevent.RightClickEvent(ref eventInfo, ref BubbleEvent);
                        break;
                }

            }
            catch (Exception ex) { }

        }

        #endregion

        #region AppEvent

        private void objapplication_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    try
                    {
                        System.Windows.Forms.Application.Exit();
                        if (objapplication != null)
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(objapplication);
                        if (objcompany != null)
                        {
                            if (objcompany.Connected)
                                objcompany.Disconnect();
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(objcompany);
                        }
                        GC.Collect();

                    }
                    catch (Exception ex)
                    {
                    }
                    break;

            }
        }

        #endregion

    }


}
