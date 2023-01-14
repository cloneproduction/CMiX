//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;

//namespace CMiX.Core.Presentation.Controls
//{
//    public class CMiXListBox : ListBox

//    {
//        public CMiXListBox()
//        {
//            DefaultStyleKey = typeof(ListBox);
//        }

//        public override void OnApplyTemplate()
//        {
//            base.OnApplyTemplate();
//            this.SelectionChanged += CMiXListBox_SelectionChanged;
//            //AddDoubleClickEventStyle(this, new MouseButtonEventHandler(listView1_MouseDoubleClick));
//        }

//        private void listBoxItem_DoubleClick(object sender, MouseButtonEventArgs e)
//        {
//            Console.WriteLine("DoubleClickListBox");
//        }

//        protected override DependencyObject GetContainerForItemOverride()
//        {
//            return new CMiXListBoxItem();
//        }

//        protected override bool IsItemItsOwnContainerOverride(object item)
//        {
//            return item is CMiXListBoxItem;
//        }

//        private void CMiXListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            //Console.WriteLine("POEUT");
//        }

//        //private void AddDoubleClickEventStyle(ListBox listBox, MouseButtonEventHandler mouseButtonEventHandler)
//        //{
//        //    if (listBox.ItemContainerStyle == null)
//        //    {
//        //        listBox.ItemContainerStyle = new Style(typeof(ListBoxItem));
//        //    }

//        //    listBox.ItemContainerStyle.Setters.Add(new EventSetter()
//        //    {
//        //        Event = MouseDoubleClickEvent,
//        //        Handler = mouseButtonEventHandler
//        //    });
//        //}
//    }
//}
