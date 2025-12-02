using System;

namespace MobileTracker.Models
{
    public class Invoice
    {
        public string? Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "draft"; // e.g., draft, sent, paid
        public InvoiceItem[]? Items { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
                throw new ArgumentException("ClientId is required", nameof(ClientId));
            if (Amount < 0)
                throw new ArgumentOutOfRangeException(nameof(Amount), "Amount cannot be negative");
        }
    }

    public class InvoiceItem
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }
}
