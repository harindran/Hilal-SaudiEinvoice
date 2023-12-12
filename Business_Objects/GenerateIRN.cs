
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EInvoice.Models
{
  

    public class saplogin
    {
    public string CompanyDB { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
    }

    


public class ActngCustomerParty
    {
        public PostalAddress PostalAddress { get; set; } = new PostalAddress();
        public PartyTaxScheme PartyTaxScheme { get; set; } = new PartyTaxScheme();
        public PartyLegalEntity PartyLegalEntity { get; set; } = new PartyLegalEntity();
        public Party Party { get; set; } = new Party();
    }

    public class ActngSuplParty
    {
        public Party Party { get; set; } = new Party();
        public PostalAddress PostalAddress { get; set; } = new PostalAddress();
        public PartyTaxScheme PartyTaxScheme { get; set; } = new PartyTaxScheme();
        public PartyLegalEntity PartyLegalEntity { get; set; } = new PartyLegalEntity();
    }

    public class AlwChg
    {
        public string Indicator { get; set; }
        public string AlwChgReason { get; set; }
        public string Amt { get; set; }
        public string BaseAmt { get; set; }
        public string MFN { get; set; }
        public string AlwChgDiscountID { get; set; }

        [JsonProperty("BaseAmt.AR")]
        public string BaseAmtAR { get; set; }
        public TaxCat TaxCat { get; set; } = new TaxCat();
    }

    public class ClasTaxCat
    {
        public string ID { get; set; }
        public string Percent { get; set; }
        public string TaxExemptionReasonCd { get; set; }
        public string TaxExemptionReason { get; set; }

        [JsonProperty("ID.AR")]
        public string IDAR { get; set; }

        [JsonProperty("Percent.AR")]
        public string PercentAR { get; set; }

        [JsonProperty("TaxExemptionReasonCd.AR")]
        public string TaxExemptionReasonCdAR { get; set; }

        [JsonProperty("TaxExemptionReason.AR")]
        public string TaxExemptionReasonAR { get; set; }
    }

    public class InvLine
    {
        public string ItemCode { get; set; }
        public string ID { get; set; }
        public string Note { get; set; }
        public string InvdQty { get; set; }
        public string InvQtyUom { get; set; }
        public string LineExtAmt { get; set; }
        public List<AlwChg> AlwChg { get; set; } = new List<AlwChg>();
        public TaxTotal TaxTotal { get; set; } = new TaxTotal();
        public Item Item { get; set; } = new Item();
        public string PrepaymentID { get; set; }

        [JsonProperty("Prepayment.UUID")]
        public string PrepaymentUUID { get; set; }
        public string PrepaymentIssueDate { get; set; }
        public string PrepaymentIssueTime { get; set; }
        public string PrepaymentDocType { get; set; }
        public string PaidVATCategoryTaxableAmt { get; set; }
        public string PaidVATCategoryTaxAmt { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public string SellersItemID { get; set; }
        public string BuyerItemID { get; set; }
        public string StdItemID { get; set; }

        [JsonProperty("Name.AR")]
        public string NameAR { get; set; }

        [JsonProperty("SellersItemID.AR")]
        public string SellersItemIDAR { get; set; }

        [JsonProperty("BuyerItemID.AR")]
        public string BuyerItemIDAR { get; set; }

        [JsonProperty("StdItemID.AR")]
        public string StdItemIDAR { get; set; }
        public ClasTaxCat ClasTaxCat { get; set; } = new ClasTaxCat();
        public Price Price { get; set; } = new Price();
        public AlwChg AlwChg { get; set; } = new AlwChg();
    }

    public class LegalMonetaryTotal
    {
        public string LineExtAmt { get; set; }
        public string AlwTotalAmt { get; set; }
        public string TaxExclAmt { get; set; }
        public string TaxInclAmt { get; set; }
        public string PrepaidAmt { get; set; }
        public string PayableAmt { get; set; }
        public string ChgTotalAmt { get; set; }                    

    }

    public class Party
    {
        public string SchemeID { get; set; }
        public string PartyID { get; set; }
        public string SellerIDNumber { get; set; }
        public string BuyerIDNumber { get; set; }

        [JsonProperty("SchemeID.AR")]
        public string SchemeIDAR { get; set; }

        [JsonProperty("PartyID.AR")]
        public string PartyIDAR { get; set; }

        [JsonProperty("SellerIDNumber.AR")]
        public string SellerIDNumberAR { get; set; }

        [JsonProperty("BuyerIDNumber.AR")]
        public string BuyerIDNumberAR { get; set; }
    }

    public class PartyLegalEntity
    {
        public string RegName { get; set; }

        [JsonProperty("RegName.AR")]
        public string RegNameAR { get; set; }
    }

    public class PartyTaxScheme
    {
        public string CompanyID { get; set; }

        [JsonProperty("CompanyID.AR")]
        public string CompanyIDAR { get; set; }
    }

    public class PostalAddress
    {
        public string SellerCode { get; set; }
        public string StrName { get; set; }
        public string AdlStrName { get; set; }
        public string PlotIdentification { get; set; }
        public string BldgNumber { get; set; }
        public string CityName { get; set; }
        public string PostalZone { get; set; }
        public string CntrySubentityCd { get; set; }
        public string CitySubdivisionName { get; set; }

        [JsonProperty("StrName.AR")]
        public string StrNameAR { get; set; }

        [JsonProperty("AdlStrName.AR")]
        public string AdlStrNameAR { get; set; }

        [JsonProperty("BldgNumber.AR")]
        public string BldgNumberAR { get; set; }

        [JsonProperty("PlotIdentification.AR")]
        public string PlotIdentificationAR { get; set; }

        [JsonProperty("CityName.AR")]
        public string CityNameAR { get; set; }

        [JsonProperty("PostalZone.AR")]
        public string PostalZoneAR { get; set; }

        [JsonProperty("CntrySubentityCd.AR")]
        public string CntrySubentityCdAR { get; set; }

        [JsonProperty("CitySubdivisionName.AR")]
        public string CitySubdivisionNameAR { get; set; }
        public string BuyerCode { get; set; }
        public string Cntry { get; set; }

        [JsonProperty("Cntry.AR")]
        public string CntryAR { get; set; }
    }

    public class Price
    {
        public string PriceAmt { get; set; }
        public string BaseQty { get; set; }
        public string BaseQtyUoM { get; set; }

        [JsonProperty("BaseQtyUoM.AR")]
        public string BaseQtyUoMAR { get; set; }
    }

    public class GenerateIRN
    {
        public string ReferenceNumber { get; set; }
        public string FinancialYear { get; set; }

        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceTime { get; set; }

        public string InvTypeCd { get; set; }
        public string InvSubtype { get; set; }
        public string ThirdPartyInvoice { get; set; }
        public string NominalInvoice { get; set; }
        public string ExportInvoice { get; set; }
        public string SummaryInvoice { get; set; }
        public string SelfBilledinvoice { get; set; }
        public string ConversionRate { get; set; }
        public string Note { get; set; }
        public string OrderRef { get; set; }
        public string BlngRef { get; set; }
        public string BlngRefIssueDt { get; set; }
        public string ContractDocRef { get; set; }
        public string DocCurrencyCd { get; set; }

        [JsonProperty("Delivery.ActualDeliveryDate")]
        public string DeliveryActualDeliveryDate { get; set; }

        [JsonProperty("Delivery.LatestDeliveryDate")]
        public string DeliveryLatestDeliveryDate { get; set; }

        [JsonProperty("PymtMeans.PymtMeansCode")]
        public string PymtMeansPymtMeansCode { get; set; }

        [JsonProperty("PymtMeans.InstructionNoteReason")]
        public string PymtMeansInstructionNoteReason { get; set; }

        [JsonProperty("PymtTerms.Note")]
        public string PymtTermsNote { get; set; }

        [JsonProperty("PymtTerms.PayeeAccountID")]
        public string PymtTermsPayeeAccountID { get; set; }
        public ActngSuplParty ActngSuplParty { get; set; } = new ActngSuplParty();
        public ActngCustomerParty ActngCustomerParty { get; set; } = new ActngCustomerParty();
        public List<InvLine> InvLine { get; set; } = new List<InvLine>();
        public List<AlwChg> AlwChg { get; set; } = new List<AlwChg>();
        public LegalMonetaryTotal LegalMonetaryTotal { get; set; } = new LegalMonetaryTotal();
    }

    public class TaxCat
    {
        public string ID { get; set; }
        public string Percent { get; set; }
    }

    public class TaxTotal
    {
        public string TaxAmt { get; set; }
        public string RoundingAmt { get; set; }
    }



    // response
  
 
    public  class PIH
    {
        public string ID { get; set; }
        public Attachment Attachment { get; set; } = new Attachment();
    }

 
    public  class QR
    {
        public string ID { get; set; }
        public Attachment Attachment { get; set; } = new Attachment();
    }

    public class Response
    {
        public string UBLVersionID { get; set; }
        public string ProfileID { get; set; }
        public string ID { get; set; }
        public string UUID { get; set; }
        public string IssueDate { get; set; }
        public string IssueTime { get; set; }
        public DateTime ZatcaResponseDate { get; set; }
        public int InvoiceTypeCode { get; set; }
        public string DocumentCurrencyCode { get; set; }
        public string TaxCurrencyCode { get; set; }
        public AdditionalDocumentReference AdditionalDocumentReference { get; set; } = new AdditionalDocumentReference();
        public ErrorResponse ErrorResponse { get; set; } = new ErrorResponse();
    }

  
    public class UUID
    {
        public string ID { get; set; }
        [JsonProperty("UUID")]
        public string _UUID { get; set; }
    }

    public  class Attachment
    {
        public string EmbeddedDocumentBinaryObject { get; set; }
    }

    public class AdditionalDocumentReference
    {
        public UUID UUID { get; set; } = new UUID();
        public PIH PIH { get; set; } = new PIH();
        public QR QR { get; set; } = new QR();
    }




    //error
    
    public class Entity
    {
        public string id { get; set; }
        public List<Record> records { get; set; } = new List<Record>();
    }

    public class Errors
    {
        public List<Entity> entities { get; set; } = new List<Entity>();
        public List<object> errors { get; set; } = new List<object>();
        public List<object> warnings { get; set; } = new List<object>();
    }

    public class Field
    {
        public string id { get; set; }
        public string value { get; set; }
        public List<string> errors { get; set; } = new List<string>();
        public List<object> warnings { get; set; } = new List<object>();
    }

    public class Record
    {
        public string id { get; set; }
        public List<Field> fields { get; set; } = new List<Field>();
        public List<object> grids { get; set; } = new List<object>();
        public List<string> errors { get; set; } = new List<string>();
        public List<object> warnings { get; set; } = new List<object>();
    }

    public class ErrorResponse
    {
        public string timestamp { get; set; }
        public string path { get; set; }
        public string status { get; set; }
        public string taxillaErrorCode { get; set; }
        public string msg { get; set; }
        public Errors errors { get; set; } = new Errors();
    }



}

