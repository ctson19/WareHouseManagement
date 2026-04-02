namespace WareManagement.Helpers;

public static class ReceiptStatuses
{
    public const string Draft = "Draft";
    public const string Confirmed = "Confirmed";
    public const string Cancelled = "Cancelled";
}

public static class ImportReceiptTypes
{
    public const string Purchase = "Purchase";
    public const string TransferIn = "TransferIn";
    public const string StockAdjustIncrease = "StockAdjustIncrease";
}

public static class ExportReceiptTypes
{
    public const string Sale = "Sale";
    public const string TransferOut = "TransferOut";
    public const string StockAdjustDecrease = "StockAdjustDecrease";
}

public static class TransferStatuses
{
    public const string Pending = "Pending";
    public const string InTransit = "InTransit";
    public const string Received = "Received";
}

public static class LocationTypes
{
    public const string Zone = "Zone";
    public const string Rack = "Rack";
    public const string Bin = "Bin";
}

public static class PartnerTypes
{
    public const string Supplier = "Supplier";
    public const string Customer = "Customer";
}

public static class StockTransactionTypes
{
    public const string In = "IN";
    public const string Out = "OUT";
}

public static class StockReferenceTypes
{
    public const string ImportReceipt = "ImportReceipt";
    public const string ExportReceipt = "ExportReceipt";
}
