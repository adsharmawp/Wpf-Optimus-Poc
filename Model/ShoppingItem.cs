using WpfOptimusPoc.vmBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace WpfOptimusPoc.Model
{
    public class ShoppingItem : ViewModelBase, IDataErrorInfo
    {
        private string _code;
        public string Code
        {
            get { return _code; }
            set { _code = value; RaisePropertyChanged("Code"); }
        }

        private string _description;        
        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
        }
        
        public string _quantity;
        public string Quantity
        {
            get { return _quantity; }
            set {
                _quantity = value;
                int quantityActual;
                if(int.TryParse(_quantity, out quantityActual))
                {
                    QuantityActual = quantityActual;
                    Amount = UnitAmount * QuantityActual;
                    RaisePropertyChanged("Amount");
                    if(QuantityActual == MaxQuantity)
                    {
                        IsMaxQuantity = true;
                        RaisePropertyChanged("IsMaxQuantity");
                    }
                    else
                    {
                        IsMaxQuantity = false;
                        RaisePropertyChanged("IsMaxQuantity");
                    }
                }
                RaisePropertyChanged("Quantity");
            }
        }
        public int QuantityActual { get; private set; }
        public decimal UnitAmount { get; private set; }
        public decimal Amount { get; private set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsMaxQuantity { get; set; }
        
        private int _maxQuantity;
        public int MaxQuantity
        {
            get { return _maxQuantity; }
            set { _maxQuantity = value; RaisePropertyChanged("MaxQuantity"); }
        }

        private decimal _price;
        public decimal Price
        {
            get { return _price; }
            set {
                _price = value;
                UnitAmount = Price - (Price * DiscountPercentage) / 100;
                Amount = UnitAmount * QuantityActual;
                RaisePropertyChanged("Price");
                RaisePropertyChanged("Amount");
            }
        }
        
        #region Validation
        public string Error { get { throw new NotImplementedException(); } }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "QuantityActual":
                    case "Quantity":
                        if (QuantityActual < 0 ) return "Invalid quantity.";
                        if (QuantityActual > MaxQuantity) return "Insufficient Stock";
                        break;
                    case "Amount":
                        if (Amount < 0) return "Invalid Amount.";
                        break;
                    case "Description":
                        if (string.IsNullOrEmpty(Description)) return "Description required.";
                        break;
                }
                return "";
            }
         }
        #endregion

        #region public function
        public void IncrementQuantityByOne()
        {
            QuantityActual += 1;
            Quantity = QuantityActual.ToString();
        }

        public void DecrementQuantityByOne()
        {
            QuantityActual -= 1;
            Quantity = QuantityActual.ToString();
        }
        #endregion
    }
}
