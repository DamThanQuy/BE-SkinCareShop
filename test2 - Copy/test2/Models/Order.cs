using System;
using System.Collections.Generic;

namespace test2.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string DeliveryStatus { get; set; } = null!;

    public string DeliveryAddress { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string OrderDelivery { get; set; } = null!;

    public string WardCode { get; set; } = null!;

    public string WardName { get; set; } = null!;

    public int ToDistrictId { get; set; }

    public string Note { get; set; } = null!;

    public virtual ICollection<CancelRequest> CancelRequests { get; set; } = new List<CancelRequest>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
