using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfOptimusPoc.vmBase;

namespace WpfOptimusPoc.Model
{
    public class ShoppingItem : ViewModelBase
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

        private int _qty;
        public int Qty
        {
            get { return _qty; }
            set {
                _qty = value;
                _amount = _unitAmount * Qty;
                RaisePropertyChanged("Qty");
                RaisePropertyChanged("Amount");
            }
        }

        private decimal _price;
        public decimal Price
        {
            get { return _price; }
            set {
                _price = value;
                _unitAmount = Price - (Price * Discount) / 100;
                _amount = _unitAmount * Qty;
                RaisePropertyChanged("Price");
                RaisePropertyChanged("Amount");
            }
        }


        private decimal _discount;
        public decimal Discount
        {
            get { return _discount; }
            set {
                _discount = value;
                _unitAmount = Price - (Price * Discount)/100;
                _amount = _unitAmount * Qty;
                RaisePropertyChanged("Discount");
                RaisePropertyChanged("Amount");
            }
        }


        private decimal _unitAmount;
        public decimal UnitAmount
        {
            get { return _unitAmount; }
        }

        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
        }

    }
}
