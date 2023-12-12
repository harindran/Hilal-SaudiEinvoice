using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.Common
{
    class clsRightClickEvent
    {
        SAPbouiCOM.Form objform;
        clsGlobalMethods objglobalMethods= new clsGlobalMethods();
        SAPbouiCOM.ComboBox ocombo;
        SAPbouiCOM.Matrix objmatrix;
        string strsql;
        SAPbobsCOM.Recordset objrs;


        public void RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, ref bool BubbleEvent)
        {
            try
            {
                switch (clsModule.objaddon.objapplication.Forms.ActiveForm.TypeEx)
                {
                    case "UOMMAP":
                        if (eventInfo.ItemUID == "Mt_UOM")
                        {
                            GenSettings_RightClickEvent(ref eventInfo,ref BubbleEvent);
                        }
                         break;                        
                }
            }
            catch (Exception ex)
            {
            }
        }


        private void RightClickMenu_Add(string MainMenu, string NewMenuID, string NewMenuName, int position)
        {
            SAPbouiCOM.Menus omenus;
            SAPbouiCOM.MenuItem omenuitem;
            SAPbouiCOM.MenuCreationParams oCreationPackage;
            oCreationPackage =(SAPbouiCOM.MenuCreationParams)clsModule.objaddon.objapplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams);
            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
            omenuitem = clsModule.objaddon.objapplication.Menus.Item(MainMenu); // Data'
            if (!omenuitem.SubMenus.Exists(NewMenuID))
            {
                oCreationPackage.UniqueID = NewMenuID;
                oCreationPackage.String = NewMenuName;
                oCreationPackage.Position = position;
                oCreationPackage.Enabled = true;
                omenus = omenuitem.SubMenus;
                omenus.AddEx(oCreationPackage);
            }
        }

        private void RightClickMenu_Delete(string MainMenu, string NewMenuID)
        {
            SAPbouiCOM.MenuItem omenuitem;
            omenuitem = clsModule.objaddon.objapplication.Menus.Item(MainMenu); // Data'
            if (omenuitem.SubMenus.Exists(NewMenuID))
            {
                clsModule.objaddon.objapplication.Menus.RemoveEx(NewMenuID);
            }
        }

        private void GenSettings_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, ref bool BubbleEvent)
        {
            try
            {
                SAPbouiCOM.Form objform;
                SAPbouiCOM.Matrix Matrix0;
                objform =clsModule. objaddon.objapplication.Forms.ActiveForm;
                Matrix0 =(SAPbouiCOM.Matrix) objform.Items.Item("Mt_UOM").Specific;

                

               switch (eventInfo.ColUID)
                    {
                    case "#":
                        objform.EnableMenu("1293", true);
                        objform.EnableMenu("1292", true);
                        string ss2 = ((SAPbouiCOM.EditText)Matrix0.Columns.Item("#").Cells.Item(eventInfo.Row).Specific).String;
                        if ( ss2== "")
                        {
                            objform.EnableMenu("1293", false); // Remove Row Menu
                        }
                        break;
                    default:
                        objform.EnableMenu("1293", false);
                        objform.EnableMenu("1292", false);                       
                       
                        break;
                }

                              
            }
            catch (Exception ex)
            {
            }
        }


    }
}
