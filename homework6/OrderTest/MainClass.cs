﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace ordertest {

    class MainClass {
        public static void Main() {
            try {
                Customer customer1 = new Customer("13288654452", "Customer1");
                Customer customer2 = new Customer("15644832267", "Customer2");

                Goods milk = new Goods(1, "Milk", 69.9);
                Goods eggs = new Goods(2, "eggs", 4.99);
                Goods apple = new Goods(3, "apple", 5.59);

                OrderDetail orderDetails1 = new OrderDetail(1.ToString(), apple, 800);
                OrderDetail orderDetails2 = new OrderDetail(2.ToString(), eggs, 2);
                OrderDetail orderDetails3 = new OrderDetail(3.ToString(), milk, 1);

                Order order1 = new Order(customer1);
                Order order2 = new Order(customer2);
                Order order3 = new Order(customer2);
                order1.AddDetails(orderDetails1);
                order1.AddDetails(orderDetails2);
                order1.AddDetails(orderDetails3);
                //order1.AddOrderDetails(orderDetails3);
                order2.AddDetails(orderDetails2);
                order2.AddDetails(orderDetails3);
                order3.AddDetails(orderDetails3);

                OrderService os = new OrderService();
                os.AddOrder(order1);
                os.AddOrder(order2);
                os.AddOrder(order3);

                //Console.WriteLine("GetAllOrders");
                //List<Order> orders = os.QueryAllOrders();
                //foreach (Order od in orders)
                //    Console.WriteLine(od.ToString());

                //Console.WriteLine("GetOrdersByCustomerName:'Customer2'");
                //orders = os.QueryByCustomerName("Customer2");
                //foreach (Order od in orders)
                //    Console.WriteLine(od.ToString());

                //Console.WriteLine("GetOrdersByGoodsName:'apple'");
                ////orders = os.QueryByGoodsName("apple");
                ////foreach (Order od in orders)
                ////    Console.WriteLine(od.ToString());
                //os.QueryByGoodsName("apple").ForEach(
                //    od => Console.WriteLine(od));

                //Console.WriteLine("GetOrdersByPrice:1000");
                //orders = os.QueryByPrice(1000);
                //foreach (Order od in orders)
                //    Console.WriteLine(od.ToString());

                //Console.WriteLine("Remove order(id=2) and qurey all");
                os.RemoveOrder(2.ToString());
                //os.QueryAllOrders().ForEach(
                //    od => Console.WriteLine(od));

                //XML序列化
                XmlSerializer xmlser = new XmlSerializer(typeof(List<Order>));
                string xmlFileName = "s.xml";
                os.XmlSerializeExport(xmlser, xmlFileName);

                //XML反序列化
                os.XmlSerializeImport(xmlser, xmlFileName);
                Console.WriteLine("GetAllOrders");
                os.QueryAllOrders().ForEach(
                    od => Console.WriteLine(od));

                Console.ReadKey();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
