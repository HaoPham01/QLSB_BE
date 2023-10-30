﻿namespace QLSB_APIs.DTO
{
    public class ExpenseReceiptDTO
    {
        public int ErId { get; set; }

        public int? FieldId { get; set; }

        public int? AdminId { get; set; }

        public string? Type { get; set; }

        public string? Content { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
