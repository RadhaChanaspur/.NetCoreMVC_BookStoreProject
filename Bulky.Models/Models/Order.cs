using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string ApplicationUserId{ get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser applicationUser { get; set; }


        public DateTime OrderDate { get; set; } 
        public DateTime ShippingDate { get; set; }
        public double OrderTotal {  get; set; } 

        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber {  get; set; }
        public string? Carrier {  get; set; }   

        public DateTime PaymentDate { get; set; } // adding this seperately apart from order date, since for company user they can make payment within 30 days of order booking
        public DateOnly PaymentDue {  get; set; }   

        public string? PaymentRefernecId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string Ciy {  get; set; }
        [Required]
        public string State {  get; set; }
        [Required]
        public string PineCode { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
