﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using OrderTest;

namespace ordertest {
    /// <summary>
    /// OrderService:provide ordering service,
    /// like add order, remove order, query order and so on
    /// 实现添加订单、删除订单、修改订单、查询订单（按照订单号、商品名称、客户等字段进行查询)
    /// </summary>
    public class OrderService { 

        private Dictionary<string, Order> orderDict;
        public Dictionary<string, Order> Dict
        {
            get { return orderDict; }
        }
        /// <summary>
        /// OrderService constructor
        /// </summary>
        public OrderService() {
            orderDict = new Dictionary<string, Order>();
        }

        public void xsltToHtml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"..\..\s.xml");

            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToRoot();

            XslCompiledTransform xt = new XslCompiledTransform();
            xt.Load(@"..\..\s.xslt");

            FileStream outFileStream = File.OpenWrite(@"..\..\s.html");
            XmlTextWriter writer =
                new XmlTextWriter(outFileStream, System.Text.Encoding.UTF8);
            xt.Transform(nav, null, writer);
        }

        public void XmlSerializeExport(XmlSerializer xmlser,string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            //foreach(Order order in orderDict.Values.ToList())
            //{
            //    xmlser.Serialize(fs, order);
            //}
            xmlser.Serialize(fs, orderDict.Values.ToList());
            fs.Close();
            string xml = File.ReadAllText(fileName);
            Console.WriteLine(xml);
        }
        public void XmlSerializeImport(XmlSerializer xmlser,string fileName)
        {
            orderDict.Clear();
            FileStream fs = new FileStream(fileName, FileMode.Open);
            List<Order> orders = xmlser.Deserialize(fs) as List<Order>;
            foreach(Order order in orders)
            {
                AddOrder(order);
            }
        }
        /// <summary>
        /// add new order
        /// </summary>
        /// <param name="order">the order will be added</param>
        public void AddOrder(Order order) {
            //if (orderDict.ContainsKey(order.Id))
            //    throw new Exception($"order-{order.Id} is already existed!");
            //orderDict[order.Id] = order;
            if (orderDict.ContainsKey(order.Id))
                throw new Exception($"order-{order.Id} is already existed!");
            else
            {
                using (var db = new OrderDB())
                {
                    db.Order.Add(order);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// cancel order
        /// </summary>
        /// <param name="orderId">id of the order which will be canceled</param> 
        public void RemoveOrder(string orderId) {
              //orderDict.Remove(orderId);
            using (var db = new OrderDB())
            {
                var order = db.Order.Include("details").SingleOrDefault(o => o.Id == orderId);
                db.OrderDetail.RemoveRange(order.Details);
                db.Order.Remove(order);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// query all orders
        /// </summary>
        /// <returns>List<Order>:all the orders</returns> 
        public List<Order> QueryAllOrders() {
            //return orderDict.Values.ToList();
            using (var db = new OrderDB())
            {
                return db.Order.Include("details").ToList<Order>();
            }
        }

        /// <summary>
        /// query by orderId
        /// </summary>
        /// <param name="orderId">id of the order to find</param>
        /// <returns>List<Order></returns> 
        public Order GetById(string orderId) {
            //return orderDict[orderId];
            using (var db = new OrderDB())
            {
                return db.Order.Include("details").
                  SingleOrDefault(o => o.Id == orderId);
            }
        }

        /// <summary>
        /// query by goodsName
        /// </summary>
        /// <param name="goodsName">the name of goods in order's orderDetail</param>
        /// <returns></returns> 
        public List<Order> QueryByGoodsName(string goodsName) {
            //var query = orderDict.Values.Where(order =>
            //        order.Details.Where(d => d.Goods.Name == goodsName)
            //        .Count() > 0
            //    );
            //return query.ToList();
            using (var db = new OrderDB())
            {
                return db.Order.Include("details")
                    .Where(order => order.Details.Where(d => d.Goods.Name == goodsName).Count() > 0)
                    .ToList<Order>();
            }

        }

        /// <summary>
        /// query by customerName
        /// </summary>
        /// <param name="customerName">customer name</param>
        /// <returns></returns> 
        public List<Order> QueryByCustomerName(string customerName) {
            //var query=orderDict.Values
            //    .Where(order => order.Customer.Name == customerName);
            //return query.ToList();
            using (var db = new OrderDB())
            {
                return db.Order.Include("details")
                    .Where(order => order.Customer.Name == customerName)
                    .ToList<Order>();
            }
        }

        public List<Order> QueryByPrice(double price)
        {
            using (var db = new OrderDB())
            {
                return db.Order.Include("details")
                    .Where(order => order.Amount > price)
                    .ToList<Order>();
            }
            //var query = orderDict.Values
            //    .Where(order => order.Amount> price);
            //return query.ToList();
        }


        /// <summary>
        /// edit order's customer
        /// </summary>
        /// <param name="orderId"> id of the order whoes customer will be update</param>
        /// <param name="newCustomer">the new customer of the order which will be update</param> 
        public void UpdateCustomer(string orderId, Customer newCustomer) {

            if (orderDict.ContainsKey(orderId)) {
                orderDict[orderId].Customer = newCustomer;
            } else {
                throw new Exception($"order-{orderId} is not existed!");
            }
        }

        /*other edit function with write in the future.*/
    }
}
