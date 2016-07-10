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
        #endregion

        #region Commands
        public ICommand AddItem { get; set; }
        public ICommand IncrementItem { get; set; }
        public ICommand DecrementItem { get; set; }
        public ICommand RemoveItem { get; set; }
        #endregion

        public MainWindowVM()
        {
            // Commands
            AddItem = new RelayCommand<Object>(_addItem);
            IncrementItem = new RelayCommand<string>(_incrementItem);
            DecrementItem = new RelayCommand<string>(_decrementItem);
            RemoveItem = new RelayCommand<string>(_removeItem);
            // Properties
            ShoppingItems = new ObservableCollection<ShoppingItem>();
        }

        #region Command Implementation

        private void _incrementItem(string itemCode)
        {
            var item = ShoppingItems.FirstOrDefault(i => i.Code == itemCode);
            if (item != null)
            {
                item.Qty += 1;
            }
            CalculateTotalAmount();
        }

        private void _decrementItem(string itemCode)
        {
            var item = ShoppingItems.FirstOrDefault(i => i.Code == itemCode);
            if (item != null)
            {
                if (item.Qty > 1)
                {
                    item.Qty -= 1;
                }
            }
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
            if (item != null)
            {
                item.Qty += 1;
            }
            else
            {
                var newShoppingItem = new ShoppingItem();
                newShoppingItem.Code = element.Attributes["Code"].Value;
                newShoppingItem.Description = element.GetElementsByTagName("Name")[0].InnerText;
                newShoppingItem.Price = decimal.Parse(element.GetElementsByTagName("Price")[0].InnerText);
                newShoppingItem.Qty = 1;
                newShoppingItem.Discount = decimal.Parse(element.GetElementsByTagName("Discount")[0].InnerText);
                newShoppingItem.PropertyChanged += NewShoppingItem_PropertyChanged;
                ShoppingItems.Add(newShoppingItem);
            }
            CalculateTotalAmount();
        }

        private void NewShoppingItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CalculateTotalAmount();
        }
        #endregion

        #region Private Members
        private void CalculateTotalAmount()
        {
            decimal totalAmount = 0;
            foreach(var item in ShoppingItems)
            {
                totalAmount = totalAmount + item.Amount;
            }
            TotalAmount = totalAmount;
        }
        #endregion
    }
}
