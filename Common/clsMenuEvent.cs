using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.Common
{
    class clsMenuEvent
    {
        SAPbouiCOM.Form objform;
        SAPbouiCOM.Form oUDFForm;

        public void MenuEvent_For_StandardMenu(ref SAPbouiCOM.MenuEvent pVal, ref bool BubbleEvent)
        {
            try
            {
                if (!pVal.BeforeAction)
                {

                    switch (clsModule.objaddon.objapplication.Forms.ActiveForm.TypeEx)
                    {
                        case "179"://AR Credit Memo    
                            //Default_Sample_MenuEvent(pVal, BubbleEvent)
                            break;
                    }
                }
                else
                {
                    switch (clsModule.objaddon.objapplication.Forms.ActiveForm.TypeEx)
                    {

                        case "179"://AR Credit Memo                        
                            break;
                    }                  
                }        
            }
            catch (Exception ex)
            {

            }
        }

        private void Default_Sample_MenuEvent(SAPbouiCOM.MenuEvent pval, bool BubbleEvent)
        {
            try
            {
                objform = clsModule.objaddon.objapplication.Forms.ActiveForm;
                if (pval.BeforeAction == true)
                {
                }

                else
                {
                    SAPbouiCOM.Form oUDFForm;
                    try
                    {
                        oUDFForm = clsModule.objaddon.objapplication.Forms.Item(objform.UDFFormUID);
                    }
                    catch (Exception ex)
                    {
                        oUDFForm = objform;
                    }

                    switch (pval.MenuUID)
                    {
                        case "1281": // Find
                            {
                               // oUDFForm.Items.Item("U_IRNNo").Enabled = true;                               
                                break;
                            }
                
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        } 
    }
}
