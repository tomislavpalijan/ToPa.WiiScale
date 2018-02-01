using System;
using System.Windows;
using System.Windows.Controls;
using WiiScale.Logic.UI.Model;

namespace WiiScale.Logic.UI.Helper
{
    

    public class AccountCardDetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ActiveTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var account = item as Account;
            if (account == null) return null;

            return ActiveTemplate;
        }
    }
}