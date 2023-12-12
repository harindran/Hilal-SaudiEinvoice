using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EInvoice.Common;
using SAPbobsCOM;
using SAPbouiCOM.Framework;

namespace EInvoice.Business_Objects
{
    [FormAttribute("FormSelect", "Business_Objects/Choose.b1f")]
    class Choose : UserFormBase
    {
        public string Query;
        public SAPbouiCOM.DataTable Datatable;
        public SAPbouiCOM.Form ActualForm;
        private SAPbouiCOM.Form Form;
        private int FormCount = 0;
        public Choose()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("Item_0").Specific));
            this.Grid0.DoubleClickAfter += new SAPbouiCOM._IGridEvents_DoubleClickAfterEventHandler(this.Grid0_DoubleClickAfter);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.VisibleAfter += new VisibleAfterHandler(this.Form_VisibleAfter);

        }

        private void editable()
        {
            for (int i = 0; i < this.Grid0.Columns.Count; i++)
            {
                SAPbouiCOM.GridColumn column = this.Grid0.Columns.Item(i);
                column.Editable = false;
            }
        }

        private void Loaddata()
        {
            if (Query == null) { return; }
            
            
                        this.Grid0.DataTable.ExecuteQuery(Query);
                        this.Grid0.AutoResizeColumns();
                        editable();



                        return;
                           
        }
        private SAPbouiCOM.Grid Grid0;

        private void OnCustomInitialize()
        {
            Form = clsModule.objaddon.objapplication.Forms.GetForm("FormSelect", FormCount);
        }

        private void Grid0_DoubleClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

            Datatable = this.Grid0.DataTable;

          
            ((SAPbouiCOM.EditText)ActualForm.Items.Item("ET0009").Specific).Value = Datatable.GetValue("TransCode", pVal.Row).ToString();
            ((SAPbouiCOM.EditText)ActualForm.Items.Item("ET00010").Specific).Value = Datatable.GetValue("TransName", pVal.Row).ToString();
            ((SAPbouiCOM.EditText)ActualForm.Items.Item("ET00011").Specific).Value = Datatable.GetValue("TransID", pVal.Row).ToString();
            ((SAPbouiCOM.ComboBox)ActualForm.Items.Item("ET00012").Specific).Select(Datatable.GetValue("TransMode", pVal.Row).ToString(),SAPbouiCOM.BoSearchKey.psk_ByDescription);
            ((SAPbouiCOM.ComboBox)ActualForm.Items.Item("ET00013").Specific).Select(Datatable.GetValue("VehicleTyp", pVal.Row).ToString(), SAPbouiCOM.BoSearchKey.psk_ByDescription);
            ((SAPbouiCOM.EditText)ActualForm.Items.Item("ET00014").Specific).Value = Datatable.GetValue("VehicleNo", pVal.Row).ToString();
   
            this.Form.Close();
        }   

        private void Form_VisibleAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            Loaddata();

        }
    }
}
