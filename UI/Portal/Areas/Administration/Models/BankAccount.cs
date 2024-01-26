using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Administration.Models
{
    public class BankAccount
    {
        public string Name { get; set; }
        public string Bank { get; set; }
        public string Currency { get; set; }
        public string Number { get; set; }
        public string Country { get; set; }
        public string Type { get; set; }
        public string ABANumber { get; set; }
        public string Swift { get; set; }
        public string Comments { get; set; }
        public string QR { get; set; }

        public BankAccount() { }

        public BankAccount(BEntities.Sales.BankAccount Item)
        {
            Name = Item.Holder;
            Bank = Item.Bank;
            Currency = Item.Currency;
            Number = Item.Number;
            Country = Item.Country;
            Type = Item.Type;
            ABANumber = Item.ABANumber;
            Swift = Item.Swift;
            Comments = Item.Comments;
            QR = Item.QR;
        }
    }

    public class BankAccountGroup
    {
        public string Name { get; set; }
        public List<BankAccount> Items { get; set; }

        public BankAccountGroup()
        {
            Items = new List<BankAccount>();
        }

        public BankAccountGroup(string name)
        {
            Name = name;
            Items = new List<BankAccount>();
        }
    }
}
