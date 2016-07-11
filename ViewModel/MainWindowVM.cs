using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using WpfOptimusPoc.Model;
using WpfOptimusPoc.vmBase;

namespace WpfOptimusPoc.ViewModel
{
    class MainWindowVM : ViewModelBase
    {
        #region Properties
        private ObservableCollection<ShoppingItem> _shoppingItems;
        public ObservableCollection<ShoppingItem> ShoppingItems
        {
            get { return _shoppingItems; }
            set { _shoppingItems = value; }
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set { _totalAmount = value; RaisePropertyChanged("TotalAmount"); }
        }
        
        private decimal _totalQuantity;
        public decimal TotalQuantity
        {
            get { return _totalQuantity; }
            set { _totalQuantity = value; RaisePropertyChanged("TotalQuantity"); }
        }
        #endregion

        #region Commands
        public ICommand AddItem { get; set; }
        public ICommand IncrementItem { get; set; }
        public ICommand DecrementItem { get; set; }
        public ICommand RemoveItem { get; set; }
        public ICommand MaxQuantity { get; set; }
        #endregion

        public MainWindowVM()
        {
            // Commands
            AddItem = new RelayCommand<Object>(_addItem);
            IncrementItem = new RelayCommand<string>(_incrementItem);
            DecrementItem = new RelayCommand<string>(_decrementItem);
            RemoveItem = new RelayCommand<string>(_removeItem);
            MaxQuantity = new RelayCommand<string>(_maxQuantity);
            // Properties
            ShoppingItems = new ObservableCollection<ShoppingItem>();
        }

        #region Command Implementation

        private void _incrementItem(string itemCode)
        {
            var item = ShoppingItems.FirstOrDefault(i => i.Code == itemCode);
            if(item.QuantityActual < item.MaxQuantity) item.IncrementQuantityByOne();
            CalculateTotalAmount();
        }

        private void _decrementItem(string itemCode)
        {
            var item = ShoppingItems.FirstOrDefault(i => i.Code == itemCode);
            if (item.QuantityActual > 1) item.DecrementQuantityByOne();
            CalculateTotalAmount();
        }
        
        private void _removeItem(string itemCode)
        {
            var item = ShoppingItems.FirstOrDefault(i => i.Code == itemCode);
            ShoppingItems.Remove(item);
            CalculateTotalAmount();
        }

        private void _addItem(Object obj)
        {
            XmlElement element = obj as XmlElement;
            var item = ShoppingItems.FirstOrDefault(i => i.Code == element.Attributes["Code"].Value);
            if (item != null && item.QuantityActual < item.MaxQuantity)
            {
                item.IncrementQuantityByOne();
            }
            else if (item == null)
            {
                var newShoppingItem = new ShoppingItem();
                newShoppingItem.Code = element.Attributes["Code"].Value;
                newShoppingItem.Description = element.GetElementsByTagName("Name")[0].InnerText;
                newShoppingItem.Price = decimal.Parse(element.GetElementsByTagName("Price")[0].InnerText);
                newShoppingItem.Quantity = "1";
                newShoppingItem.DiscountPercentage = decimal.Parse(element.GetElementsByTagName("Discount")[0].InnerText);
                newShoppingItem.MaxQuantity = int.Parse(element.GetElementsByTagName("Quantity")[0].InnerText);
                newShoppingItem.PropertyChanged += NewShoppingItem_PropertyChanged;
                ShoppingItems.Add(newShoppingItem);
            }
            CalculateTotalAmount();
        }

        private void NewShoppingItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Quantity")
            {
                var item = (sender as ShoppingItem);
                if(item.QuantityActual == 0)
                {
                    var result = MessageBox.Show("Are you sure you want to remove this item?", "Confirm", MessageBoxButton.OKCancel);
                    if(result == MessageBoxResult.OK)
                    {
                        ShoppingItems.Remove(item);
                    }
                    else
                    {
                        item.Quantity = "1";
                    }
                }
            }
            CalculateTotalAmount();
        }


        private void _maxQuantity(string itemCode)
        {
            // TODO: need to implement
            var item = ShoppingItems.FirstOrDefault(i => i.Code == itemCode);
            if(item != null && item.Quantity== "*")
            {
                var result = MessageBox.Show("Are you sure you want to use max quantity for this item?", "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    item.Quantity = item.MaxQuantity.ToString();
                }
                else
                {
                    item.Quantity = "1";
                }
            }
            CalculateTotalAmount();
        }
        #endregion

        #region Private Members
        private void CalculateTotalAmount()
        {
            decimal totalAmount = 0;
            int totalQuantity = 0;
            foreach(var item in ShoppingItems)
            {
                totalAmount = totalAmount + item.Amount;
                totalQuantity = totalQuantity + item.QuantityActual;
            }
            TotalAmount = totalAmount;
            TotalQuantity = totalQuantity;
        }
        #endregion
    }
}
