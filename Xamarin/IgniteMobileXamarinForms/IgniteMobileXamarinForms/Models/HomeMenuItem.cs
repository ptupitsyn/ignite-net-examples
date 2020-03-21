using System;
using System.Collections.Generic;
using System.Text;

namespace IgniteMobileXamarinForms.Models
{
    public enum MenuItemType
    {
        Browse,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
