using Models;

namespace Minimarket.DTOs
{
    public class PurchaseListDto
    {
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; }
    public DateTime Date { get; set; }
    public string NroDoc { get; set; }
    public decimal Total { get; set; }
    }
}